using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utilities.Loggers;
using Utilities.Windowing;

namespace Utilities.Command
{
    /// <summary>
    /// Provides a controller class for a set of commands making up a document
    /// Provides Do/Undo/Redo Functionality
    /// todo work out serialisation to xml
    /// </summary>
    [Serializable]
    public class CommandHistory {

        // ReSharper disable InconsistentNaming
        private readonly List<CommandHistoryNode> History = new List<CommandHistoryNode>();
        private readonly Stack<CommandHistoryNode> UndoStack = new Stack<CommandHistoryNode>();

        private int _CommandCount = 0;
        // ReSharper restore InconsistentNaming

        private readonly object _lockobj = new object();

        public WindowController WindowController {get; set;}

        /// <summary>
        /// Execute a given command
        /// </summary>
        /// <param name="command">command to Execute</param>
        /// <returns>true if executed successfully</returns>
        public bool Execute(ICommand command) {
            bool execSuccess;
            lock (_lockobj) {
                execSuccess = command.Execute();
                if (execSuccess) {
                    History.Add(new CommandHistoryNode(command, this._CommandCount++));
                    this.UndoStack.Clear();                    
                }
            }
            if (this.WindowController != null) {
                this.WindowController.RequestRefresh(typeof (HistoryViewer));
            }

            return execSuccess;
        }

        /// <summary>
        /// Undo the last do action (that has not been undone!)
        /// </summary>        
        /// <returns>bool depending on the execution of the undo command</returns>
        public bool Undo() {
            bool undoSuccess = false;
            lock (_lockobj) {
                // find the command,
                CommandHistoryNode nodeToUndo = this.History.LastOrDefault();
                if (nodeToUndo == null) {
                    Logger.FAILURE("All Commands have been undone!");
                } else {                    

                    // now we iterate through all undone nodes
                    while (nodeToUndo != null && nodeToUndo.Command.IsUndoable && 
                          nodeToUndo.NodeUndone != null && nodeToUndo.NodeUndone.Command.IsUndoable ){
                                             
                        //skip back
                        nodeToUndo = nodeToUndo.NodeUndone;                        

                        // move one node further back to get the next one to test
                        int index = History.IndexOf(nodeToUndo);
                        nodeToUndo = index - 1 < 0 ? null : History[index - 1];
                    }                    
                    
                    // now we check we can undo it
                    if (nodeToUndo == null || !nodeToUndo.Command.IsUndoable) {
                        if (nodeToUndo == null) {
                            Logger.FAILURE("There is no Node to undo!");
                        } else {
                            Logger.FAILURE("The following Command cannot be undone: " + nodeToUndo.Command.Description);
                        }
                    } else {

                        // now we try and execute the undo
                        ICommand undoCommand = nodeToUndo.Command.UndoCommand;
                        if (undoCommand.Execute()) {
                            CommandHistoryNode undoNode = new CommandHistoryNode(undoCommand,
                                                                                 this._CommandCount++, nodeUndone: nodeToUndo);
                            this.History.Add(undoNode);
                            UndoStack.Push(undoNode);
                            undoSuccess = true;
                        }

                    }
                }
            }
            return undoSuccess;
        }

        /// <summary>
        /// Redo the last undo command 
        /// </summary>
        /// <returns>bool - result of executing the redo command</returns>
        public bool Redo() {
            // the node will be on the top of the undo stack
            bool redoSuccess = false;
            lock (_lockobj) {
                if (this.UndoStack.Count == 0) {
                    Logger.FAILURE("There is no Command to Re-do!");
                } else {
                    CommandHistoryNode nodeToRedo = this.UndoStack.Pop();

                    // now we try and execute the redo
                    ICommand redoCommand = nodeToRedo.Command.UndoCommand;
                    if (redoCommand.Execute()) {
                        CommandHistoryNode undoNode = new CommandHistoryNode(redoCommand,
                                                                             this._CommandCount++,nodeRedone: nodeToRedo);
                        History.Add(undoNode);                        
                        redoSuccess = true;
                    }

                }
            }
            return redoSuccess;
        }

        /// <summary>
        /// Removes all do/undo branches of the command history
        /// - compacts history but removes do/undo pairs
        /// also clears the undo stack
        /// </summary>
        public void Clean() {
            lock (_lockobj) {

                // find done/undone pairs... 
                List<int> idstoRemove = new List<int>();
                for (int n = this.History.Count - 1; n >= 0; n--) {
                    if (this.History[n].NodeUndone != null) { // if its not been undone then remove it and its partner in crime
                        idstoRemove.Add(this.History[n].CommandNum);
                        idstoRemove.Add(this.History[n].NodeUndone.CommandNum); 
                    }
                }

                // remove redone links to the nodes we are about to remove: 
                foreach (CommandHistoryNode node in
                    History.Where(node => node.NodeRedone != null && idstoRemove.Contains(node.NodeRedone.CommandNum))) {
                    node.NodeRedone = null;
                }

                // and now remove them all
                this.History.RemoveAll(n => idstoRemove.Contains(n.CommandNum));
                

                // oh and clear the undo stack
                this.UndoStack.Clear();
            }
        }

        /// <summary>
        /// Print history to string
        /// </summary>
        /// <param name="full">true = show compact history, false = include do/undo branches</param>
        /// <returns>history</returns>
        public string PrintHistory(bool full) {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("History Dump full: {0}", full) + "\r\n");            

            List<string> descriptions = new List<string>();
            List<int> nodesUndone = new List<int>();
            List<int> nodesRedone = new List<int>();
            for (int n = this.History.Count - 1; n >= 0; n--) {                

                if (this.History[n].NodeUndone != null) {
                    if (!full) {
                        // only ignore if we are not doing full
                        nodesUndone.Add(this.History[n].NodeUndone.CommandNum);
                    }
                    nodesUndone.Add(this.History[n].CommandNum);
                }
                if (this.History[n].NodeRedone != null) {
                    if (!full) {
                        // only ignore if we are not doing full
                        nodesRedone.Add(this.History[n].NodeRedone.CommandNum); // ie we ignore the undo command - which we do anyway above. 
                    }
                    nodesRedone.Add(this.History[n].CommandNum);
                }


                if (full && nodesUndone.Contains(this.History[n].CommandNum)) {
                    descriptions.Add(this.History[n] + " [UNDO]:" +
                                     this.History[n].Command.Description);
                }

                if (nodesRedone.Contains(this.History[n].CommandNum) &&
                    !nodesUndone.Contains(this.History[n].CommandNum)) {

                    descriptions.Add(this.History[n] + " [REDO]:" +
                                     this.History[n].Command.Description);

                }

                if (!nodesRedone.Contains(this.History[n].CommandNum) &&
                    !nodesUndone.Contains(this.History[n].CommandNum)) {

                    if (!full || (this.History[n].NodeRedone == null && this.History[n].NodeRedone == null)) { // full &&
                        descriptions.Add(this.History[n] + " [DO]:" + this.History[n].Command.Description);
                    }
                }

            }

            descriptions.Reverse();

            sb.Append(descriptions.Aggregate("\r\n", (s, d) => s + d + "\r\n")); // simplez

            return sb.ToString();
        }

        /// <summary>
        /// Deletes the history and the undo stack to save memory
        /// </summary>
        public void DeleteHistory() {
            lock (this._lockobj) {
                this.History.Clear();
                this.UndoStack.Clear();
            }
        }

        /// <summary>
        /// Prints the undo stack.
        /// </summary>
        /// <returns>a string with ns separating the description of each command</returns>
        public string PrintUndoStack() {
            StringBuilder sb = new StringBuilder();

            foreach (CommandHistoryNode node in UndoStack) {
                sb.AppendLine(node.Command.Description);
            }

            return sb.ToString();
        }
    }
}