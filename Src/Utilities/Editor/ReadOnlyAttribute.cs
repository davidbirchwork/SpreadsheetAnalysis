using System;

namespace Utilities.Editor {
    /// <summary>
    /// Denotes that this field cannot be edited
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute { // todo add a description to this field
    }
}
