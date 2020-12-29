using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Utilities.Windowing {
    /// <summary>
    /// This Attribute is for annotating <see cref="IWindow"/> definitions for MEF export
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class WindowAttribute : ExportAttribute, IWindowAttribute {

        public WindowAttribute() : base(typeof(WindowAttribute)) { }

        /// <summary>
        /// Gets or sets the name of the window type - eg "Log window"
        /// </summary>
        /// <value>The name of the window type.</value>
        public string WindowTypeName { get; set; }

        /// <summary>
        /// Gets or sets the window type description - eg "Shows log messages"
        /// </summary>
        /// <value>The window type description.</value>
        public string WindowTypeDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this form MUST have an instance displaying
        /// </summary>
        /// <value><c>true</c> if [instance required]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool InstanceRequired { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple instances of this window
        /// </summary>
        /// <value><c>true</c> if [allow multiple window instances]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        public bool AllowMultiple { get; set; }
    }
}