using System;

namespace Utilities.Editor {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditorIgnoreAttribute : Attribute {
    }
}
