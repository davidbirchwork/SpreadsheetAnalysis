using System.Collections.Generic;
using Utilities.Command;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// Specifies a Boolean editor - a checkbox
    /// </summary>
    public class BooleanEditorAttribute : AEditorAttribute {
        private readonly string _name;
        private readonly string _description;
        private readonly bool _default;        

        #region ctors        

        public BooleanEditorAttribute(string name, string description, bool defaultValue) {
            this._name = name;
            this._description = description;
            this._default = defaultValue;
        }

        #endregion

        #region Overrides of AEditorAttribute

        public override List<AEditor> GetEditors(CommandHistory history = null) {
            return new List<AEditor> {
                                         new BooleanEditor(this._name,this._description,this._default) {
                                                                              History = history
                                                                        }
                                     };
        }

        #endregion
    }
}
