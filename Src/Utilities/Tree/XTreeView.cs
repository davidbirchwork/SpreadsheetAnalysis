using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;
using Utilities.Tree.Columns;

namespace Utilities.Tree {
    /// <summary>
    /// This class stores all the components required for a tree view
    /// (i.e. a data source and a list of columns)
    /// 
    /// Features to add:
    /// > notification of other IWindows
    /// > double click editor
    /// > filtering of children
    /// > linking editing back to the original views via drag and drop. [ie linking back to the ]
    /// > notification of change of values.
    /// </summary>
    public class XTreeView {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public readonly AxTreeColumn MainColumn;
        public readonly List<AxTreeColumn> Columns = new List<AxTreeColumn>();
        public readonly Func<XElement> RootElement;        

        /// <summary>
        /// Decide whether or not a row supports a given column
        /// </summary>
        public Func<Column,Row,bool> ShowColumn;

        /// <summary>
        /// Gets or sets the function to get children.
        /// </summary>
        /// <value>The children of a given node.</value>
        public Func<XElement,List<XElement>> GetChildren { get; set; }

        public bool ShowRootNode = true;        

        #region form interaction methods        

        public Action<XElement> OnItemSelected { get; set; }
        public Action<XElement> OnDoubleClick { get; set; }
        public Dictionary<string, Action<XElement>> MenuCommands { get; set; } // feature split by |'s ?
        public Dictionary<string, EventHandler> ContextMenuCommands { get; set; }        

        #endregion

        #region ctors

        public XTreeView(string name, string description, Func<XElement> rootElement, Func<XElement, List<XElement>> getChildren, AxTreeColumn mainColumn)
            : this(name, description, rootElement, getChildren, mainColumn, new List<AxTreeColumn>()) {
            
        }

        public XTreeView(string name, string description, Func<XElement> rootElement, Func<XElement, List<XElement>> getChildren, AxTreeColumn mainColumn, IEnumerable<AxTreeColumn> columns) {
            if (rootElement == null) throw new ArgumentNullException("rootElement");
            if (mainColumn == null) throw new ArgumentNullException("mainColumn");

            this.Name = name;
            this.Description = description;
            this.RootElement = rootElement;
            this.GetChildren = getChildren;
            this.Columns = new List<AxTreeColumn>();                 
            foreach (AxTreeColumn column in columns) {
                column.MainColumn = false;
                this.Columns.Add(column);
            }
            this.MainColumn = mainColumn;
        }

        #endregion

        public event Action OnUpdateDataSource;
        public event Action OnColumnsChanged;

        /// <summary>
        /// Updates the view.
        /// calls the OnUpdateDataSource event
        /// </summary>
        public void UpdateView() {            
            this.OnUpdateDataSource();
        }

        public void AddColumns(IEnumerable<AxTreeColumn> editors) {
            this.Columns.AddRange(editors);
            OnColumnsChanged();
        }

        public void RemoveColumns(Predicate<AxTreeColumn> predicate) {
            this.Columns.RemoveAll(predicate);
            OnColumnsChanged();
        }
    }
}
