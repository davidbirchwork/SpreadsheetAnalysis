using System;

namespace Utilities.SaveLoad {

    /// <summary>
    /// This attribute defines how a file should be saved or loaded
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class FileTypeAttribute : Attribute {

        public FileTypeAttribute(SerializationType type, string filter) {
            this.Type = type;
            this.Filter = filter;
        }

        #region required        

        /// <summary>
        /// Gets or sets the type of Save Load to carry out
        /// </summary>
        /// <value>The type of save load to use.</value>
        public SerializationType Type { get; set; }

        /// <summary>
        /// Gets or sets the Filter to use eg "XML|*.xml"
        /// </summary>
        /// <value>The Filter to use in Save/Load.</value>
        public string Filter { get; set; }

        #endregion

        #region optional        

        /// <summary>
        /// Gets or sets the DefaultExtension to use eg "XML|*.xml"
        /// </summary>
        /// <value>The DefaultExtension.</value>
        public string DefaultExtension { get; set; }

        /// <summary>
        /// Gets or sets the Absolute path for the default Folder to save/load        
        /// </summary>
        /// <value>The URI for the default folder.</value>
        public string AbsoluteFolder { get; set; }

        /// <summary>
        /// Gets or sets the Relative (from exe) path for the default Folder to save/load
        /// is overridden by AbsoluteFolder
        /// </summary>
        /// <value>The URI for the default folder.</value>
        public string RelativeFolder { get; set; }

        /// <summary>
        /// A Friendly save message to display to the user - the title of the save dialog
        /// </summary>        
        public string SaveMessage { get; set; }

        /// <summary>
        /// A Friendly load message to display to the user - the title of the load dialog
        /// </summary>        
        public string LoadMessage { get; set; }

        #endregion
    }

    /// <summary>
    /// Specifies the type of serialisation to use
    /// </summary>
    public enum SerializationType {
        /// <summary>
        /// Use an XML serializer
        /// </summary>
        Xml 
        //todo ,binary
        //todo ,ToStringCtor
        //todo ,Custom - via attributes
    }
}
