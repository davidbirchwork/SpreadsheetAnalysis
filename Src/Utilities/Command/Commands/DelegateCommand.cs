using System;

namespace Utilities.Command.Commands {
    public class DelegateCommand : ICommand {
        private string Name { get; set; }
        private Func<bool> Executor { get; set; }
        private Func<bool> Undor { get; set; }

        #region ctors

        public DelegateCommand(string name, string description, Func<bool> executor, Func<bool> undor = null) {
            this.Description = description;
            this.Name = name;
            this.Executor = executor;
            if (this.Executor == null) {
                throw new ArgumentNullException("executor","Must provide an executor function");
            }            
            this.Undor = undor;
        }

        #endregion

        #region Implementation of ICommand

        public bool Execute() {
            return this.Executor.Invoke();
        }

        public string Description {get; private set;}

        public ICommand UndoCommand {
            get {
                return this._undoCommand ??
                       (this._undoCommand = new DelegateCommand(this.Name,this.Description,this.Undor,this.Executor));
            }
            internal set {
                if (value != null) {
                    this._undoCommand = value;
                }
            }
        }

        private ICommand _undoCommand;        

        public bool IsUndoable {
            get { return this.UndoCommand == null; }
        }

        #endregion
    }
}
