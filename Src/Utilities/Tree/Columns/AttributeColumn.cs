using System;
using System.Xml.Linq;
using Infralution.Controls;
using Infralution.Controls.VirtualTree;
using Utilities.Loggers;

//TODO should we have a custom caption?

//TODO make this class generic? then we have Func<string, T> _formatter;  ??
namespace Utilities.Tree.Columns {
    /// <summary>
    /// This class enables an XElement's attribute to be used as a column
    /// </summary>
    public class AttributeColumn : AxTreeColumn {

        /// <summary>
        /// Gets or sets the attribute to be read for the cell value
        /// </summary>
        /// <value>The attribute name</value>
        private XName AttributeName { get; set; }

        private bool ReadOnly { get; set; }

        /// <summary>
        /// the formatter to use to format the attribute value to the output value
        /// </summary>
        private readonly Func<string, object> _formatter;

        /// <summary>
        /// this function is used to move from an object back to a storeable string
        /// </summary>
        private readonly Func<object, string> _unformat;

        /// <summary>
        /// This function validates the input 
        /// arguments: Xelement, old value, new value
        /// either return an error message or return null to accept the value
        /// </summary>
        private readonly Func<XElement, string, string, string> _validate;

        /// <summary>
        /// default value to use if the attribute doesn't exist. 
        /// </summary>
        private readonly String _defaultValue = "";

        #region ctors

        /// <summary>
        /// Initializes a new Readonly instance of the <see cref="AttributeColumn"/> class without formatters
        /// </summary>
        /// <param name="attribute">the attribute to be read for the cell value.</param>
        /// <param name="defaultvalue">default value to use if attribute doesn't exist or is null or empty or whitespace</param>
        /// <param name="isreadonly">if set to <c>true</c> [isreadonly].</param>
        /// <param name="colName">name of the column - otherwise the attribute name</param>
        public AttributeColumn(XName attribute, string defaultvalue = "", bool isreadonly = true, string colName =null)
            : this(attribute, s => s, defaultvalue, isreadonly, colName ?? attribute.ToString()) {               
        }

        /// <summary>
        /// Initializes a new Readonly instance of the <see cref="AttributeColumn"/> class with formatters
        /// </summary>
        /// <param name="attribute">the attribute to be read for the cell value.</param>
        /// <param name="formatter">the formatter to use to format the attribute value to the output value</param>
        /// <param name="defaultvalue">default value to use if attribute doesn't exist or is null or empty or whitespace</param>
        /// <param name="isreadonly">if set to <c>true</c> [isreadonly].</param>
        /// <param name="colName">name of the column - otherwise the attribute name</param>
        public AttributeColumn(XName attribute, Func<string, object> formatter, string defaultvalue = "", bool isreadonly = true, string colName = null)
            : this(attribute, isreadonly, isreadonly ? (Func<XElement, string, string, string>)((e, o, n) => "this column is read only") : (e, o, n) => null, formatter, o => o.ToString(), defaultvalue, colName ?? attribute.ToString()) {
        }

        /// <summary>
        /// Initializes an editable new instance of the <see cref="AttributeColumn"/> class with out formatters
        /// </summary>
        /// <param name="attribute">the attribute to be read for the cell value.</param>                
        /// <param name="validator"> This function validates the input arguments: Xelement, old value, new value  either return an error message or return null to accept the value</param>
        /// <param name="defaultvalue">default value to use if attribute doesn't exist or is null or empty or whitespace</param>        
        /// <param name="colName">name of the column - otherwise the attribute name</param>
        public AttributeColumn(XName attribute, Func<XElement, string, string, string> validator, string defaultvalue = "", string colName = null) :
            this(attribute, false, validator, s => s, o => o.ToString(), defaultvalue, colName ?? attribute.ToString()) {
        }

        /// <summary>
        /// Initializes an editable new instance of the <see cref="AttributeColumn"/> class with formatters
        /// </summary>
        /// <param name="attribute">the attribute to be read for the cell value.</param>        
        /// <param name="formatter">the formatter to use to format the attribute value to the output value</param>
        /// <param name="validator"> This function validates the input arguments: Xelement, old value, new value  either return an error message or return null to accept the value</param>
        /// <param name="defaultvalue">default value to use if attribute doesn't exist or is null or empty or whitespace</param>
        /// <param name="unformatter"> this function is used to move from an object back to a storeable string </param>
        /// <param name="colName">name of the column - otherwise the attribute name</param>
        public AttributeColumn(XName attribute, Func<XElement, string, string, string> validator, Func<string, object> formatter, Func<object, string> unformatter, string defaultvalue = "", string colName = null) :
            this(attribute, false, validator, formatter, unformatter, defaultvalue, colName ?? attribute.ToString()) {
        }

        /// <summary>
        /// Initializes an editable new instance of the <see cref="AttributeColumn"/> class with format formatters
        /// </summary>
        /// <param name="attribute">the attribute to be read for the cell value.</param>        
        /// <param name="formatter">the formatter to use to format the attribute value to the output value</param>
        /// <param name="readOnly">is this column read only?</param>
        /// <param name="validator"> This function validates the input arguments: Xelement, old value, new value  either return an error message or return null to accept the value</param>
        /// <param name="defaultvalue">default value to use if attribute doesn't exist or is null or empty or whitespace</param>
        /// <param name="unformatter"> this function is used to move from an object back to a storeable string </param>
        /// <param name="colName">name of the column - otherwise the attribute name</param>
        public AttributeColumn(XName attribute, bool readOnly, Func<XElement, string, string, string> validator, Func<string, object> formatter, Func<object, string> unformatter, string defaultvalue = "", string colName = null) {
            if (colName == null) colName = attribute.ToString();
            if (attribute == null) throw new ArgumentNullException("attribute");
            if (formatter == null) throw new ArgumentNullException("formatter");

            this.Name = colName;
            this.Caption = colName;
            this.AttributeName = attribute;
            this._formatter = formatter;
            this.ReadOnly = readOnly; 
            this._defaultValue = defaultvalue;

            //set up column
            this.Name = this.AttributeName.ToString();

            // cell editor
// ReSharper disable DoNotCallOverridableMethodsInConstructor 
            //the method calls make no use of this class so we dont need to worry about un-init'd feilds
            this.CellEditor = new CellEditor(new UniversalEditBox());
// ReSharper restore DoNotCallOverridableMethodsInConstructor
            //this.CellEditor.Control.Enabled = this.ReadOnly; - this could be used but it means that the field is not open for copy and paste

            if (!this.ReadOnly) {
                _validate = validator;
                _unformat = unformatter;
            }
        }

        #endregion

        #region Overrides of AxTreeColumn

        /// <summary>
        /// Gets the data for a cell
        /// </summary>
        /// <param name="xElement">The x element (ie ROW)</param>
        /// <param name="cellData">The cell data event - should be able to set anything</param>
        /// <returns>success or failure</returns>
        public override bool GetData(XElement xElement, CellData cellData) {            

            XAttribute xAttribute = xElement.Attribute(this.AttributeName);
            if (xAttribute == null) {
                cellData.Value = this._defaultValue;
                cellData.Error = "";
                return true; 
            }

            if (string.IsNullOrEmpty(xAttribute.Value) || string.IsNullOrWhiteSpace(xAttribute.Value)) {
                cellData.Value = this._defaultValue;
                cellData.Error = "";
                return true;
            }

            try {                
                cellData.Value = this._formatter(xAttribute.Value);
                cellData.Error = "";
            } catch (Exception e) {
                Logger.FAILURE("StringColumn Formatter failed on column " + this.AttributeName + " value was " +
                               xAttribute.Value + " error was: "+e.Message);
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Sets the value for a cell on an xelement
        /// </summary>
        /// <param name="xElement">The XElement (ie ROW).</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>ERROR MESSAGE - null if no error</returns>
        public override string SetValue(XElement xElement, object oldValue, object newValue) {
            if (this.ReadOnly) {
                return "This field is read only!";
            }

            string oldStringValue;
            string newStringValue;

            // move from obj to string*2
            try {
                oldStringValue = this._unformat(oldValue);                
            } catch (Exception e) {
                string message = "StringColumn '"+this.AttributeName+"' UnFormatter failed on column " + this.AttributeName + " value was " +
                                 oldValue + " error was: " + e.Message;
                Logger.FAILURE(message);
                return message;
            }
            try {
                newStringValue = this._unformat(newValue);
            } catch (Exception e) {
                string message = "StringColumn '" + this.AttributeName + "' UnFormatter failed on column " + this.AttributeName + " value was " +
                                 newValue + " error was: " + e.Message;
                Logger.FAILURE(message);
                return message;
            }
            
            // validate data 
            string errorMessage;
            try {
                errorMessage  = this._validate(xElement, oldStringValue, newStringValue);
            } catch (Exception e) {
                string message = "StringColumn '" + this.AttributeName + "' Validator threw exception failed on column " + this.AttributeName + " oldvalue was " +
                                 oldStringValue + " new value was "+newStringValue + " xelement was" + xElement+" error was: " + e.Message;
                Logger.FAILURE(message);
                return message;
            }

            if (errorMessage != null) { // there was an error
                return errorMessage;                
            }

            // no error so set the attribute
            if (xElement.Attribute(this.AttributeName) != null) {
                xElement.SetAttributeValue(this.AttributeName, newStringValue);
            } else {
                xElement.Add(new XAttribute(this.AttributeName, newStringValue));
            }
            return null; // no error
            
        }

        #endregion
    }
}
