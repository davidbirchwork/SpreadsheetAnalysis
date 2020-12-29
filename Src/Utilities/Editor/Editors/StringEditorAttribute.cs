using System.Collections.Generic;
using System.Linq;
using Utilities.Command;

namespace Utilities.Editor.Editors {

    /// <summary>
    /// Specifies a string editor
    /// </summary>
    public class StringEditorAttribute : AEditorAttribute {

        private readonly bool _notBlank;
        private readonly string _name;
        private string _formatedAllowedCharacters;
        private string _allowedCharacters;
        public string AllowedCharacters {
            get { return _allowedCharacters; }
            set {
                _allowedCharacters = value;
                _formatedAllowedCharacters = value;
                this._formatedAllowedCharacters = this._formatedAllowedCharacters.Replace("a-z", @"abcdefghijklmnopqrstuvwxyz");
                this._formatedAllowedCharacters = this._formatedAllowedCharacters.Replace("A-Z", @"ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                this._formatedAllowedCharacters = this._formatedAllowedCharacters.Replace("0-9", @"0123456789");
            }
        }        

        #region Implementation of IEditorAttribute

        public override List<AEditor> GetEditors(CommandHistory history = null) {
            return new List<AEditor> {
                                         new UniversalEditor<string>(this._name, this.Description,
                                                                     element => element.Value,
                                                                     newString => {
                                                                         if (this._notBlank &&
                                                                             (string.IsNullOrEmpty(newString) ||
                                                                              string.IsNullOrWhiteSpace(newString))) {
                                                                             return
                                                                                 "This column must contain a non blank value.";
                                                                         }
                                                                         if (this._formatedAllowedCharacters != null &&
                                                                             !newString.All(
                                                                                 c =>
                                                                                 this._formatedAllowedCharacters.
                                                                                     Contains(c))) {
                                                                             return
                                                                                 "The string value must contain only the following characters: " +
                                                                                 this.AllowedCharacters;
                                                                         }
                                                                         return null; //no errors
                                                                     }, (newValue, element) => {
                                                                            element.Value = newValue;
                                                                            return null;
                                                                        },
                                                                        history)
                                     };
        }                

        #endregion

        public StringEditorAttribute(string name, string description, bool notBlank) {
            this._notBlank = notBlank;
            this.Description = description;
            this._name = name;
        }
    }
}
