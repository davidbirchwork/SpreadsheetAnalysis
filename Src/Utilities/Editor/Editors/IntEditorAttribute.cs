﻿using System.Collections.Generic;
using Utilities.Command;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// Enabled editing an integer value with optional min and max values
    /// </summary>
    public class IntEditorAttribute : AEditorAttribute {
        private readonly string _name;

        // ReSharper disable UnusedMember.Global
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable RedundantDefaultFieldInitializer
        private bool _usingMinValue = false;

        private int _minValue;
        public int MinValue {
            get { return _minValue; }
            set { _minValue = value;
                this._usingMinValue = true;
            }
        }

        private bool _usingMaxValue = false;
        private int _maxValue;
        public int MaxValue {
            get { return _maxValue; }
            set {
                _maxValue = value;
                this._usingMaxValue = true;
            }
        }
        // ReSharper restore RedundantDefaultFieldInitializer
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnusedMember.Global

        #region Implementation of IEditorAttribute

        public override List<AEditor> GetEditors(CommandHistory history = null) {
            return new List<AEditor> {
                                         new UniversalEditor<int>(this._name, this.Description,
                                                                  element => int.Parse(element.Value),
                                                                  i => {
                                                                      if (this._usingMinValue && i < this.MinValue) {
                                                                          return "Value must be at least " +
                                                                                 this.MinValue;
                                                                      }
                                                                      if (this._usingMaxValue && i > this.MaxValue) {
                                                                          return "Value must be at most " +
                                                                                 this.MaxValue;
                                                                      }
                                                                      return null;
                                                                  }, (newValue, xElement) => {
                                                                         xElement.Value = newValue.ToString();
                                                                         return null;
                                                                     }, history)
                                     };
        }        

        #endregion

        #region ctors        

        public IntEditorAttribute(string name, string description) {
            this._name = name;
            this.Description = description;
        }

        #endregion
    }
}