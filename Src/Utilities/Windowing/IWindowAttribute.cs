using System.ComponentModel;

namespace Utilities.Windowing {
    /// <summary>
    /// The Meta data interface used for MEF binding - implemented by <see cref="WindowAttribute"/>
    /// </summary>
    public interface IWindowAttribute {

        /// <summary>
        /// Gets or sets the name of the window type - eg "Log window"
        /// </summary>
        /// <value>The name of the window type.</value>
        string WindowTypeName { get; }

        /// <summary>
        /// Gets or sets the window type description - eg "Shows log messages"
        /// </summary>
        /// <value>The window type description.</value>
        string WindowTypeDescription { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this form MUST have an instance displaying
        /// </summary>
        /// <value><c>true</c> if [instance required]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        bool InstanceRequired { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow multiple instances of this window
        /// </summary>
        /// <value><c>true</c> if [allow multiple window instances]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]
        bool AllowMultiple { get; }
    }
}