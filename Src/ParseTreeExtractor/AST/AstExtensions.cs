using System.Collections.Generic;
using System.Linq;
using Irony.Parsing;

namespace ParseTreeExtractor.AST {
    public static class AstExtensions {
        public static List<ParseTreeNode> GetDescendentsOfType(this ParseTreeNode tree, string type) {
            List<ParseTreeNode> nodes = new List<ParseTreeNode>(); 

            Queue<ParseTreeNode> queue = new Queue<ParseTreeNode>();
            queue.Enqueue(tree);
            while (queue.Any()) {
                var popped = queue.Dequeue();
                if (popped?.Term.Name == type) {
                    nodes.Add(popped);
                }

                if (popped.ChildNodes.Any()) {
                    foreach (var child in popped.ChildNodes) {
                        queue.Enqueue(child);
                    }
                }
            }

            return nodes;
        }
    }
}