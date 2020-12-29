using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;
using Infralution.Controls.VirtualTree;
using System.Collections;
using Utilities.Loggers;
using Utilities.Tree.Columns;

namespace Utilities.Tree {

    /// <summary>
    /// A subclass of Infralution's Virtual Tree which enables binding to an XTreeView system for editing XML
    /// </summary>
    public class XVirtualTreeView : VirtualTree {

        private XTreeView _xTree;
        public XTreeView XTree {
            get {
                return _xTree;
            }
            set {
                if (value != null) {
                    _xTree = value;
                    this.UpdateXTreeView();
                }
            }
        }

        #region parent child        

        protected override IList GetChildrenForRow(Row row) {
            XElement elem = (row.Item as XElement);
            if (elem == null || this.XTree.GetChildren == null) {
                return new List<XElement>();
            }
            List<XElement> children = this.XTree.GetChildren.Invoke(elem);
            return children ?? new List<XElement>();            
        }

        /// <summary>
        /// Gets the parent for item.
        /// </summary>
        /// <param name="item">The XElement to get parent of</param>
        /// <returns>The XElement.Parent element (or null)</returns>
        protected override object GetParentForItem(object item) {
            XElement elem = item as XElement;
            return elem == null ? null : elem.Parent;
        }

        #endregion

        #region get set celldata        

        protected override void OnGetCellData(Row row, Column column, CellData cellData) {
            XElement xElement = (row.Item as XElement);
            if (xElement == null) {
                cellData.Error = "Invalid XElement!";
                return;
            }

            // find the column 
            AxTreeColumn xcolumn = column as AxTreeColumn;
            if (xcolumn == null) {
                cellData.Value = "Unknown Column";
                cellData.Error = "";
                Logger.DEBUG("Column is not a Valid " + typeof(AxTreeColumn).FullName + " column name = " + column.Name);
                return;
            }

            // call the column's data preparer
            if (!xcolumn.GetData(xElement, cellData)) {
                cellData.Value = "Unknown Value";
                cellData.Error = "";
                Logger.DEBUG("Column failed to get data - " + xcolumn.Name + " on " + xElement);
                return;
            }
        }

        protected override bool SetValueForCell(Row row, Column column, object oldValue, object newValue) {
            XElement xElement = (row.Item as XElement);
            if (xElement == null) {                
                return false;
            }

            // find the column 
            AxTreeColumn axTreeColumn = column as AxTreeColumn;
            if (axTreeColumn == null) {                
                return Logger.DEBUG("Column is not a Valid " + typeof(AxTreeColumn).FullName +
                             " column name = " + column.Name);                
            }

            // call the column's data setter
            string error = axTreeColumn.SetValue(xElement, oldValue, newValue);

            if (error != null) {
                return Logger.FAILURE("Failure to update value on Column " + axTreeColumn.Name + ": " + error);
            } else {                
                Row rowtoupdate = row;
                while (rowtoupdate != null) {
                    this.UpdateRowData(rowtoupdate);
                    rowtoupdate = rowtoupdate.ParentRow;
                }
                
                return Logger.SUCCESS("Updated value on Column " + axTreeColumn.Name);
            }
        }

        protected override bool ColumnInContext(Column column) {
            if (column == this.MainColumn || this.XTree.ShowColumn == null) { // note the maincolumn doesn't seem to be set.
                return true;
            }
            return this.FocusRow != null && this.XTree.ShowColumn.Invoke(column, this.FocusRow);
        }

        #endregion

        #region XTreeView Methods

        private void UpdateXTreeView() {
            this.BeginInit();
            
            this.XTree.OnColumnsChanged += UpdateColumns;
            UpdateColumns();

            // now sort out on selection changed

            if (this.XTree.OnItemSelected != null) {
                this.SelectionChanged += XVirtualTreeViewOnItemSelected;
                this._onItemSelectedDelgateAdded = true;
            } else {
                if (this._onItemSelectedDelgateAdded) {
                    this.SelectionChanged -= XVirtualTreeViewOnItemSelected;
                    this._onItemSelectedDelgateAdded = false;
                }
            }
           
            // now sort out on double click

            if (this.XTree.OnDoubleClick != null) {
                this.DoubleClick += XVirtualTreeViewDoubleClick;
                this._onDoubleClickDelgateAdded = true;
            } else {
                if (this._onDoubleClickDelgateAdded) {
                    this.DoubleClick -= XVirtualTreeViewDoubleClick;
                    this._onDoubleClickDelgateAdded = false;
                }
            }

            // now sort out context menu            

            if (this.XTree.ContextMenuCommands != null && this.XTree.ContextMenuCommands.Count > 0) {

                List<MenuItem> menuitems = new List<MenuItem>();

                foreach (KeyValuePair<string, EventHandler> menuCommand in this.XTree.ContextMenuCommands) {                    
                    menuitems.Add(new MenuItem(menuCommand.Key, menuCommand.Value ));
                }
                
                this.ContextMenu = new ContextMenu(menuitems.ToArray());
            }

            // sort out root data item
            this.DataSource = this.XTree.RootElement.Invoke();
            this.ShowRootRow = this.XTree.ShowRootNode;

            //sort out refreshing
            this.XTree.OnUpdateDataSource += OnUpdateDataSource;

            this.EndInit();
            this.Update();            
        }

        private delegate void OnUpdateDataSourceCallback();

        private void OnUpdateDataSource() {
            if (this.InvokeRequired) {
                OnUpdateDataSourceCallback d = OnUpdateDataSource;
                this.Invoke(d, new object[] { });
                return;
            }

            this.DataSource = this.XTree.RootElement.Invoke();
            this.UpdateRows(true);
        }

        private delegate void UpdateColumnsCallback();

        private void UpdateColumns() {
            if (this.InvokeRequired) {
                UpdateColumnsCallback d = UpdateColumns;
                this.BeginInvoke(d);
                return;                
            }

        // sort out columns:
            this.Columns.Clear();
            this.MainColumn = this.XTree.MainColumn;
            this.MainColumn.MainColumn = true;
            this.Columns.Add(this.XTree.MainColumn);
            foreach (AxTreeColumn column in this.XTree.Columns) {
                this.Columns.Add(column);
                column.ContextSensitive = true; 
                column.MainColumn = false;
                //column.AutoSizePolicy = ColumnAutoSizePolicy.AutoSize;                kind of works but not quite - eg makes some col's too narrow and some too large :(
            }
        }

        private bool _onItemSelectedDelgateAdded;

        private void XVirtualTreeViewOnItemSelected(object sender, EventArgs e) {
            XElement element = this.SelectedItem as XElement;
            if (element != null) {
                this.XTree.OnItemSelected.Invoke(element);
            }
        }

        private bool _onDoubleClickDelgateAdded;        

        private void XVirtualTreeViewDoubleClick(object sender, EventArgs e) {
            XElement element = this.FocusItem as XElement;
            if (element != null) {
                this.XTree.OnDoubleClick.Invoke(element);
            }
        }        

        #endregion
    }

}
