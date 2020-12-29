namespace Utilities.Command
{
    /// <summary>
    /// Represents a command
    /// </summary>
    public interface ICommand 
    {        
        /// <summary>
        /// Executes this command
        /// </summary>
        /// <returns>success</returns>
        bool Execute();

        /// <summary>
        /// Returns a Friendly Description of this Command
        /// </summary>
        /// <value>description of command</value>
        string Description {get;}

        /// <summary>
        /// Returns a command which will Undo this command
        /// </summary>
        /// <returns>Undo Command</returns>
        ICommand UndoCommand { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is undoable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is undoable; otherwise, <c>false</c>.
        /// </value>
        bool IsUndoable { get; }
        
    }
}