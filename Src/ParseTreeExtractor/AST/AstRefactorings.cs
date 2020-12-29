using System;
using System.Linq;
using System.Reflection;
using DocumentFormat.OpenXml.EMMA;
using Irony.Parsing;
using XLParser;

namespace ParseTreeExtractor.AST {

    public static class NodeTests {
        public static bool NodeHasFirstChildOfType(this ParseTreeNode node, string name) {
            return node?.ChildNodes?.FirstOrDefault()?.Term?.Name == name;
        }

        public static ParseTreeNode FirstChild(this ParseTreeNode node) {
            return node?.ChildNodes?.FirstOrDefault();
        }
    }

    // too make this return bool and make node a ref 
    public delegate bool ParseRefactoring( ref ParseTreeNode node, ExcelFormulaGrammar grammar);

    public static class AstRefactorings {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool RemoveArgumentNodes(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            if (node.Term?.Name == grammar.Argument.Name) {
                if (node.ChildNodes.Count > 1) {
                    Log.Error("Found Argument Node with multiple children");
                }

                node = node.ChildNodes.FirstOrDefault();
                return true;
            }

            return false;
        }

        public static bool RemoveNumberNodes(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            if (node.Term?.Name == grammar.Number.Name) {
                if (node.ChildNodes.Count > 1) {
                    Log.Error("Found Number Node with multiple children");
                }

                node = node.ChildNodes.FirstOrDefault();
                return true;
            }

            return false;
        }

        public static bool RemoveConstantNodes(ref ParseTreeNode node, ExcelFormulaGrammar grammar)
        {
            if (node.Term?.Name == grammar.Constant.Name)
            {
                if (node.ChildNodes.Count > 1)
                {
                    Log.Error("Found Constant Node with multiple children");
                }

                node = node.ChildNodes.FirstOrDefault();
                return true;
            }

            return false;
        }

        public static bool InlineFunctionNames(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            // before
            // functionCall > FunctionName > Name
            //              > Arguments > [Argument > ...]
            // after 
            // Name > [Argument > ... ]
            if (node.Term?.Name == grammar.FunctionCall.Name) {
                if (node.ChildNodes.Any(c=> c.IsOperator())) {
                    var opNodes = node.ChildNodes.Where(c => c.IsOperator()).ToList();
                    var others = node.ChildNodes.Where(c => !c.IsOperator()).ToList();
                    
                    if (!opNodes.Any() || opNodes.Count() > 1) {
                        Log.Error("multiple operators on a node");
                    }

                    node = opNodes.First();
                    node.ChildNodes.AddRange(others);

                    return true;
                }

                var nameNode = node.ChildNodes.FirstOrDefault(c => c.Term?.Name == grammar.FunctionName.Name);
                var argumentsNode = node.ChildNodes.FirstOrDefault(c => c.Term?.Name == grammar.Arguments.Name);
                if (nameNode != null && argumentsNode != null) {
                    var newnode = nameNode.ChildNodes.First();
                    newnode.ChildNodes.AddRange(argumentsNode.ChildNodes);
                    node = newnode;
                    return true;

                }
            }

            return false;
        }

        public static bool RemoveFormulaEqNode(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            if (node.Term?.Name == grammar.FormulaWithEq.Name) {
                // as this occurs at root we don't need to worry about setting the children here... 
                node = node.ChildNodes.FirstOrDefault(c => c.Term?.Name == grammar.Formula.Name);
                return true;
            }

            return false;
        }

        public static bool TruncateReferences(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            if (node.Term?.Name == grammar.Reference.Name) {
                if (node.ChildNodes.Any()) {
                    if (node.NodeHasFirstChildOfType(grammar.Reference.Name))
                    { // this may be a bug in the grammar  see https://github.com/spreadsheetlab/XLParser/issues/78
                        node = node.ChildNodes.First();
                        return true;
                    }
                    
                    //var tag = node.Tag as Tuple<string, object>;
                    string nodeId = ((Tuple<string, object>) node.Tag).Item1;
                    node.Tag = new Tuple<string, object>(nodeId, node.ChildNodes.ToArray());
                    node.ChildNodes.Clear();
                    return true;
                }
            }

            return false;
        }

        public static bool RemoveFormulaNodes(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            if (node.Term?.Name == grammar.Formula.Name) {
                if (node.ChildNodes.Count > 1) {
                    Log.Error("Found Formula Node with multiple children");
                }

                node = node.ChildNodes.FirstOrDefault();
                return true;
            }

            return false;
        }

        public static bool SkipReferenceBeforeFunctionCalls(ref ParseTreeNode node, ExcelFormulaGrammar grammar) {
            // for some reason the grammar puts a Reference node as the parent of a ReferenceFunctionCall for an If statement
            if (node.Term?.Name == grammar.Reference.Name 
                && node.ChildNodes.Count == 1 
                && node.NodeHasFirstChildOfType(grammar.ReferenceFunctionCall.Name) 
                && node.FirstChild().NodeHasFirstChildOfType(grammar.RefFunctionName.Name) 
                &&  (node.FirstChild().FirstChild().NodeHasFirstChildOfType(grammar.ExcelConditionalRefFunctionToken.Name) 
                || node.FirstChild().FirstChild().NodeHasFirstChildOfType(grammar.ExcelRefFunctionToken.Name)  )) {
                node = node.ChildNodes.First();
                return true;
            }

            return false;
        }
    }
}