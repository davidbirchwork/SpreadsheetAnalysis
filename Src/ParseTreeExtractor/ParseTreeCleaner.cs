using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Irony.Parsing;
using log4net;
using ParseTreeExtractor.AST;
using ParseTreeExtractor.Domain;
using XLParser;

namespace ParseTreeExtractor {
    internal static class ParseTreeCleaner {

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private class ParentChild {
            public ParentChild(ParseTreeNode parent, ParseTreeNode child) {
                Parent = parent;
                Child = child;
            }

            public ParseTreeNode Parent { get; set; }
            public ParseTreeNode Child { get; set; }
        }

        public static ExcelFormulaGrammar CleanParseTrees(Extraction extraction) {

            List<ParseRefactoring> refactorings = new List<ParseRefactoring> {
                AstRefactorings.SkipReferenceBeforeFunctionCalls,
                AstRefactorings.RemoveFormulaEqNode,
                AstRefactorings.InlineFunctionNames,
                AstRefactorings.RemoveConstantNodes,
                AstRefactorings.RemoveNumberNodes,
                AstRefactorings.RemoveArgumentNodes,
                AstRefactorings.TruncateReferences,
                AstRefactorings.RemoveFormulaNodes
            };

            Log.Info("applying refactoring with " + refactorings.Count + " operators");

            ExcelFormulaGrammar grammar = null;

            int i = 0;

            foreach (var cellPair in extraction.ParseTrees) {
                //.Where(kvp => kvp.Key.CellName == "Q13")
                i++;
                var rootNode = cellPair.Value;
                if (cellPair.Value == null) continue;
                grammar = (ExcelFormulaGrammar) rootNode.Term.Grammar;


                Stack<ParentChild> workQueue = new Stack<ParentChild>(); // depth first to avoid large queues 

                workQueue.Push(new ParentChild(null, rootNode));

                while (workQueue.Any()) {
                    var todo = workQueue.Pop();
                    var node = todo.Child;

                    //refactor until we stop, yes this may work down the tree
                    var changes = true;

                    while (changes) {
                        changes = false;
                        foreach (var r in refactorings) {
                            changes = changes || r(ref node, grammar);
                        }
                    }


                    // recurse
                    var orphans = node.ChildNodes.ToArray();
                    node.ChildNodes.Clear();
                    foreach (var c in orphans) {
                        workQueue.Push(new ParentChild(node, c));
                    }

                    // deal with changing the root node
                    if (todo.Parent == null) {
                        rootNode = node;
                    }
                    else {
                        todo.Parent.ChildNodes.Add(node);
                    }

                }

                // save changes 
                extraction.ParseTrees.AddOrUpdate(cellPair.Key, rootNode, (k, v) => rootNode);

                if (i % 1000 == 0) {
                    Log.Info("refactored " + i);
                }
            }

            Log.Info("Finished refactoring");
            return grammar;
        }
    }
}