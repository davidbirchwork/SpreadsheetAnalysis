using System;
using System.Diagnostics;

namespace Utilities.Command
{
    /// <summary>
    /// A node for containing an ICommand in a CommandHistory, 
    /// the property NodeUndone stores a link to the command which this command is undoing
    /// the property NodeRedone stores a link to the command which this command is undoing 
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Command#{CommandNum}")]
    public class CommandHistoryNode : IComparable<CommandHistoryNode>,IEquatable<CommandHistoryNode> {

        #region ctors

        /// <summary>
        /// Create a command historyNode
        /// </summary>        
        /// <param name="command">command to exec</param>
        /// <param name="commandNum">used to see if we are at the start of the command history and to avoid comparing commands</param>
        /// <param name="nodeUndone">ref to the historyNode which would undo this command</param>
        /// <param name="nodeRedone">ref to the command which this command is redoing </param>
        public CommandHistoryNode(ICommand command, int commandNum, CommandHistoryNode nodeUndone = null, CommandHistoryNode nodeRedone = null) {            
            this.Command = command;
            this.CommandNum = commandNum;
            this.NodeUndone = nodeUndone;
            this.NodeRedone = nodeRedone;

            this.TimeExecuted = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the time the command was executed.
        /// </summary>
        /// <value>The time the command was executed</value>
        public DateTime TimeExecuted {
            get; private set;
        }

        #endregion 

        #region fields

        /// <summary>
        /// Gets the command this historyNode encapsulates
        /// </summary>
        public ICommand Command {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the CommandNum.
        /// used to see if we are at the start of the command history and to avoid comparing commands
        /// </summary>
        /// <value>The CommandNum.</value>
        public int CommandNum { get; private set; }

        /// <summary>
        /// the property NodeUndone stores a link to the command which this command is undoing
        /// </summary>
        public CommandHistoryNode NodeUndone {
            get;
            private set;// never called during a clean as the undo node itself (ie this node) will be deleted
        }

        /// <summary>
        ///  the property NodeRedone stores a link to the command which this command is redoing 
        /// </summary>
        public CommandHistoryNode NodeRedone {
            get; 
            set; // public as called during clean to prevent holding large chains in memory
        }         

        #endregion

        #region Implementation of IComparable<in CommandHistoryNode>

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// USES CommandNum
        /// </summary>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(CommandHistoryNode other) {
            return this.CommandNum.CompareTo(other.CommandNum);
        }

        #endregion

        #region Implementation of IEquatable<CommandHistoryNode>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// USES CommandNum
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(CommandHistoryNode other) {
            return this.CommandNum == other.CommandNum;
        }

        #endregion

        public override string ToString() {
            return this.TimeExecuted + ":" + this.TimeExecuted.Millisecond + " {ID=" + this.CommandNum + "}";
        }
    }
}