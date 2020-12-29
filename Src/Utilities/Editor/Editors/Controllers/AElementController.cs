using System.Collections.Generic;

namespace Utilities.Editor.Editors.Controllers {
    /// <summary>
    /// Base Class for controlling groups of elements being edited - eg arrays or structs or classes
    /// </summary>
    public abstract class AElementController {
        protected abstract List<AEditor> CreateColumns();

        private List<AEditor> _controlColumns = null;

        public IEnumerable<AEditor> ControlColumns {
            get { return this._controlColumns ?? (this._controlColumns = CreateColumns()); }
        }
    }
}
 