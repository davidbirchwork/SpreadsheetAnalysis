namespace Utilities.UserConfig {
    /// <summary>
    /// interface for Config files to implement
    /// classes implementing this MUST be XML Serialisable!
    /// </summary>    
    public interface IConfigFile {

        /// <summary>
        /// Returns the default config file.
        /// </summary>
        /// <returns></returns>
        IConfigFile ReturnDefault();

        /// <summary>
        /// Notifies the config file that they have updated.
        /// </summary>
        void NotifyUpdated();
    }
}
