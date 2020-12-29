using System;
using System.Xml.Linq;
using Utilities.Command;
using Utilities.Loggers;
using Utilities.XmlSerialisation;

namespace Utilities.Editor {
    public class EditCommand : ICommand {
        private readonly string _name;
        private readonly string _oldobj;
        private readonly string _newobj;
        private readonly Func<object,bool> _setObject;
        private readonly bool _returnXml;

        public EditCommand(string name, XElement oldobj, XElement newobj, Func<object, bool> setObject, bool returnXml) : 
            this(name,oldobj.ToString(),newobj.ToString(),setObject, returnXml){
        }

        public EditCommand(string name, string oldobj, string newobj, Func<object, bool> setObject, bool returnXml) {
            this._name = name;
            this._oldobj = oldobj;
            this._newobj = newobj;
            this._setObject = setObject;
            this._returnXml = returnXml;
        }

        #region Implementation of ICommand

        public bool Execute() {
            object newobj = this._returnXml
                                ? XElement.Parse(this._newobj)
                                : SerialisationController.DeserializeFromXmlWithType(this._newobj);
            if (this._setObject.Invoke(newobj)) {
                return Logger.SUCCESS("Successfully edited an Object");
            }
            return Logger.FAILURE("Failed to set edited object to its new value");
        }

        public string Description {
            get { return "Edited " + this._name; }
        }

        public ICommand UndoCommand {
            get {
                return this._undoCommand ??
                       (this._undoCommand = new EditCommand(this._name,this._newobj, this._oldobj, this._setObject, this._returnXml) { UndoCommand = this });
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
