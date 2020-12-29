using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Xml.Linq;
using Utilities.Loggers;

namespace Utilities.Windowing {
    // ReSharper disable UnusedMember.Global

    /// <summary>
    /// This represents a window which can be docked in a client form
    /// to use with MEF tag all implementations with the <see cref="WindowAttribute"/> attribute
    /// </summary>
    [InheritedExport]
    public interface IWindow  {
        /// <summary>
        /// Gets the name of the instance - for example "Edit Window - C:\File.txt"
        /// </summary>
        /// <returns>name of the instance</returns>
        string GetInstanceName();

        /// <summary>
        /// Gets the instance description - for example "Window for editing C:\File.txt Project File"
        /// </summary>
        /// <returns>instance description</returns>
        string GetInstanceDescription();        

        XElement SaveAsXML();
        XElement LoadFromXML();

        /// <summary>
        /// Binds a GUI window to a data container by passing it a reference.
        /// </summary>
        /// <param name="objectList">The list of objects to bind to - indexed by name</param>
        void BindTo(Dictionary<string, object> objectList); // note this might need to be a Dictionary one day..

        /// <summary>
        /// Refreshes the view of the form
        /// </summary>
        void RefreshView();
    }

    public static class ObjectDictionaryExtensions {
        public static bool EnsureNameType(this Dictionary<string,object> objectList, string name, Type type) {
            if (objectList == null || !objectList.ContainsKey(name) || !objectList[name].GetType().Equals(type)) {
                return Logger.ERROR("Binding Failed IWindow was not passed a "+type.Name+" named '"+name+"'");
            }
            return true;
        }
    }

    // ReSharper restore UnusedMember.Global
}