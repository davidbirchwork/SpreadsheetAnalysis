using System.Xml.Linq;
using Utilities.Command;

namespace Utilities_Test.Command {
    class AddAttribute : ICommand {

        private readonly XElement _elem;
        private readonly XAttribute _attribute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddAttribute"/> class.
        /// Which adds an XAttribute to an XElement
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="attribute">The attribute.</param>
        internal AddAttribute(XElement element, XAttribute attribute) {
            this._elem = element;
            this._attribute = attribute;
        }

        #region Implementation of ICommand

        /// <summary>
        /// Executes this command
        /// </summary>
        /// <returns>success</returns>
        public bool Execute() {
            if (this._elem == null) return false;
            if (this._attribute == null) return false;

            this._elem.Add(this._attribute);

            return true;
        }       

        /// <summary>
        /// Returns a Friendly Description of this Command
        /// </summary>
        /// <returns>description</returns>
        public string Description {
            get { return "Adding attribute: "+this._attribute.Name; }
        }

        /// <summary>
        /// Returns a command which will Undo this command
        /// </summary>
        /// <returns>Undo Command</returns>
        public ICommand UndoCommand {
            get {
                return this._undoCommand ??
                       (this._undoCommand = new RemoveAttribute(this._elem, this._attribute) {UndoCommand = this});
            }
            set {
                if (value != null) {
                    this._undoCommand = value;
                }
            }
        }

        private ICommand _undoCommand;

        /// <summary>
        /// Gets a value indicating whether this instance is undoable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is undoable; otherwise, <c>false</c>.
        /// </value>
        public bool IsUndoable {
            get { return true; }
        }

        #endregion
    }
}
