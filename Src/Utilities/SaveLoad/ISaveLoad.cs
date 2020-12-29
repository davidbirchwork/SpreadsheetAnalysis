namespace Utilities.SaveLoad {

    /// <summary>
    /// Indicates that a file can be saved and loaded with the SaveLoad System
    /// You MUST also Tag with Attribute FileType
    /// </summary>    
    public interface ISaveLoad {

        /// <summary>
        /// Gets or sets the URI to save the file with
        /// </summary>
        /// <value>The URI of the file.</value>
        string FileName { get; set; }
    }
}
