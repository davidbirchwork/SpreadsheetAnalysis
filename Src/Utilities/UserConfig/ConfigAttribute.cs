using System;
using System.ComponentModel.Composition;

namespace Utilities.UserConfig {
    /// <summary>
    /// The Meta data interface used for MEF binding - implemented by <see cref="ConfigAttribute"/>
    /// </summary>
    public interface IConfigAttribute {

        /// <summary>
        /// Gets the name of the entity being configured.
        /// </summary>
        /// <value>The name of the config.</value>
        string ConfigName { get; }

        /// <summary>
        /// Gets the description of the entity being configured.
        /// </summary>
        /// <value>The config description.</value>
        string ConfigDescription { get; }

        /// <summary>
        /// Gets the file name of the config file.
        /// </summary>
        /// <value>The name of the config file.</value>
        string ConfigFileName { get; }

        /// <summary>
        /// Gets the app config override entry.
        /// </summary>
        /// <value>The app config override entry.</value>
        string AppConfigOverrideEntry { get; }
    }

    // ReSharper disable UnusedMember.Global
    // ReSharper disable MemberCanBePrivate.Global
    /// <summary>
    /// This Attribute is for annotating <see cref="IConfigAttribute"/> definitions for MEF export
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigAttribute : ExportAttribute, IConfigAttribute {

        public string ConfigName { get; set; }
        public string ConfigDescription { get; set; }
        public string ConfigFileName { get; set; }
        public string AppConfigOverrideEntry { get; set; }

        public ConfigAttribute(string name, string description, string configFileName, string appConfigOverrideEntry)
            : base(typeof(ConfigAttribute)) {
            this.ConfigName = name;
            this.ConfigDescription = description;
            this.ConfigFileName = configFileName;
            this.AppConfigOverrideEntry = appConfigOverrideEntry;
        }
    }
}