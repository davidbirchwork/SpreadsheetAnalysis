using System;
using System.Xml.Linq;
using Utilities.Command;
using Utilities.Loggers;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// A command to enabling undoing of an edit to a cell in a field
    /// </summary>
    /// <typeparam name="T">type of field</typeparam>
    public class EditFieldCommand<T> : ICommand{
        private readonly string _name;
        private readonly XElement _element;
        private readonly T _oldvalue;
        private readonly T _newvalue;
        private readonly Func<T, XElement, string> _valueSettor;

        public EditFieldCommand(string name, XElement element, T oldvalue, T newvalue, Func<T, XElement, string> valueSettor) {
            this._name = name;
            this._element = element;
            this._oldvalue = oldvalue;
            this._newvalue = newvalue;
            this._valueSettor = valueSettor;
        }

        #region Implementation of ICommand

        public bool Execute() {
            string successMessage = this._valueSettor.Invoke(this._newvalue, this._element);
            return string.IsNullOrEmpty(successMessage) ? 
                Logger.SUCCESS("Successfully set value of "+this._name+" to "+this._newvalue) : 
                Logger.FAILURE("Failed to set value of "+this._name+" to "+this._newvalue+ " Error was "+successMessage);
        }

        public string Description {
            get {
                return "Setting " + this._name + " to " + this._newvalue;
            }
        }

        public ICommand UndoCommand {
            get {
                return this._undoCommand ??
                       (this._undoCommand = new EditFieldCommand<T>(this._name,this._element, this._newvalue, this._oldvalue, this._valueSettor) { UndoCommand = this });
            }
            private set {
                if (value != null) {
                    this._undoCommand = value;
                }
            }
        }

        private ICommand _undoCommand;

        public bool IsUndoable {
            get { return true; }
        }

        #endregion
    }
}
