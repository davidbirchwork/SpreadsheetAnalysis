using System;

namespace Utilities.Editor {
    /// <summary>
    /// Denotes that this class cannot be edited and should be displayed using the ToString Method only
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReadOnlyClassAttribute : Attribute{
    }
}
