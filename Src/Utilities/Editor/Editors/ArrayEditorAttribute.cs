using System;
using Utilities.Editor.Editors.Controllers;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// An attribute for specifying an array/list controller.
    /// </summary>
    public class ArrayEditorAttribute : Attribute {        
        private readonly string _name;
        private readonly string _description;
        private readonly Type _type;
        private readonly bool _allowNewItems;
        private readonly bool _allowDelete;
        private readonly bool _ordered;

        private bool _useMaxItems;
        private int _maxItems;
        private int _minItems;

        public int MaxItems {
            get { return this._maxItems; }
            set {
                if (value > 0 || value >= MinItems) {                    
                    this._maxItems = value;
                    this._useMaxItems = true;
                }
            }
        }

        public int MinItems {
            get {
                return _minItems;
            }
            set {
                if (value > 0 || value <= this.MaxItems)
                    this._minItems = value;
            }
        }

        public ArrayEditorAttribute(string name, string description,Type type, bool allowNewItems, bool allowDelete, bool ordered = false) {            
            this._name = name;
            this._description = description;
            this._type = type;
            this._allowNewItems = allowNewItems;
            this._allowDelete = allowDelete;
            this._ordered = ordered;
        }

        public AElementController GetController() {            
            return new ArrayController(this._name, this._description,this._type, this._allowNewItems, this._allowDelete, this.MinItems, this._useMaxItems ? this.MaxItems : int.MaxValue,this._ordered);
        }        
    }
}
