using System;

namespace Utilities.Editor.Editors {
    /// <summary>
    /// Tag with this to show that its a validator
    /// Methods should be public static and accept a single parameter and return a string error message or null if validation completes correctly
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple=false,Inherited=true)]
    public class ValidatorAttribute : Attribute {
    }
}
