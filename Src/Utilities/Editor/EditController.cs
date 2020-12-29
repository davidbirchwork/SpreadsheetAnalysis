using System;
using System.Xml.Linq;
using Utilities.Command;

namespace Utilities.Editor {
    public class EditController {
        public readonly bool ReadOnly;
        public bool ReturnXml { get; private set; }
        public string Name { get; set; }
        public string OldXElement { get; private set; }
        public Func<object, bool> SetObject { get; private set; }
        public CommandHistory CommandHistory { get; private set; }

        public EditController(string name, string oldXElement, Func<object, bool> setObject, CommandHistory commandHistory, bool returnXml, bool isreadonly) {
            this.Name = name;
            this.OldXElement = oldXElement;
            this.SetObject = setObject;
            this.CommandHistory = commandHistory;
            this.ReturnXml = returnXml;
            this.ReadOnly = isreadonly;
        }

        public bool Validate(XElement xElement) {
            return true; // todo sort this - it should prob be an event using Logger to report errors
        }        
    }
}
