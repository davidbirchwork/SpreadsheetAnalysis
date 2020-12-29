using System;
using System.Collections.Generic;
using Utilities.Command;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// All editor specification attributes must implement this interface        
    /// todo an NCalc implementation
    /// </summary>
    public abstract class AEditorAttribute : Attribute { // todo we should maybe extend from the XAttribute that avoids blank variables?

        public abstract List<AEditor> GetEditors(CommandHistory history = null);

        /// <summary>
        /// Gets or sets the description of the field being edited
        /// </summary>
        /// <value>The description of the field being edited.</value>
        public string Description { get; set; }
    }
}
