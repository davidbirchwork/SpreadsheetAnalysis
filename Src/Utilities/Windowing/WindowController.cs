using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Utilities.Config;
using Utilities.Loggers;

namespace Utilities.Windowing {
    public class WindowController {
        private Form MdiForm { get; set; }

       [ImportMany]
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public Lazy<IWindow, IWindowAttribute>[] WindowTypes { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Local

        public WindowController(Form mdiForm) {
            this.MdiForm = mdiForm;
            try {
                Logger.INFO("Starting MEF composition");
            } catch (Exception e) {
                ReflectionTypeLoadException tLException = e.InnerException.InnerException as ReflectionTypeLoadException;
                var loaderMessages = new StringBuilder();
                loaderMessages.AppendLine("While trying to load composable parts the follwing loader exceptions were found: ");
                foreach (var loaderException in tLException.LoaderExceptions) {
                    loaderMessages.AppendLine(loaderException.Message);
                }

                // this is one of our custom exception types.   
                throw new Exception(loaderMessages.ToString(), tLException);
            }  

            CompositionContainer container = AppConfig.GetCompositionContainer();
            container.ComposeParts(this);

            this._windowMap = new Dictionary<string, WeakReference>();

         //  this is the other way to avoid import many:  
            //this.WindowTypes = AppConfig.GetCompositionContainer().GetExports<IWindow, IWindowAttribute>();            

            foreach (Lazy<IWindow, IWindowAttribute> windowType in this.WindowTypes) {
                if (windowType.Metadata.InstanceRequired) {
                    if (windowType.Value != null) {                       
                        this._windowMap.Add(windowType.Metadata.WindowTypeName, new WeakReference(CreateNewWindow(windowType)));                        
                    }
                }
            }            

            Logger.SUCCESS("Composed with MEF");
        }

        private delegate IWindow CreateNewWindowCallback(Lazy<IWindow, IWindowAttribute> windowType);

        private IWindow CreateNewWindow(Lazy<IWindow, IWindowAttribute> windowType) {
            
            if (this.MdiForm.InvokeRequired) {
                CreateNewWindowCallback callback = CreateNewWindow;
                return (IWindow) this.MdiForm.Invoke(callback, new object[] {windowType});
            }

            //create the form
            IWindow window;
            if (!windowType.Metadata.AllowMultiple && !((Form)windowType.Value).IsDisposed) {
                window = windowType.Value;

                if (this.MdiForm.MdiChildren.Any(f => f.GetType().Equals(window.GetType()))) {
                    Logger.FAILURE("You may only include a single " + windowType.Metadata.WindowTypeName + " Window");
                    return null;
                }
            } else {
                window = (IWindow)Activator.CreateInstance(windowType.Value.GetType());
            }

            // display it
            Form form = (Form)window; // cast should be ok..            
            form.MdiParent = this.MdiForm;
            form.Show();            

            return window;
        }

        /// <summary>
        /// Create a new window of a given IWindow Type
        /// </summary>
        /// <param name="type">The type of window to create - should be an IWindow</param>
        /// <returns> the IWindow created</returns>
        private IWindow CreateIWindow(Type type) {
            var windowTypes = this.WindowTypes.Where(def => def.Value.GetType().Equals(type));
            if (windowTypes.Count() > 0) {
                return CreateNewWindow(windowTypes.First());
            } else {
                Logger.FAILURE("Could not find type " + type.FullName + " Amongst known window types");
            }
            return null;
        }

        /// <summary>
        /// Ensures that there is at least one IWindow of the type specified amongst the forms MDI children
        /// </summary>
        /// <param name="type">The type to ensure</param>
        /// <param name="count">number of window instances to ensure</param>        
        public void EnsureIWindow(Type type, int count = 1) {
            int windowcount = this.MdiForm.MdiChildren.Where(f => f.GetType().Equals(type)).Count();
            if (windowcount >= count) return;

            for (int c = 1; c <= (count - windowcount); c++) {
                var windowTypes = this.WindowTypes.Where(def => def.Value.GetType().Equals(type));
                if (windowTypes.Count() > 0) {
                    CreateNewWindow(windowTypes.First());
                } else {
                    Logger.FAILURE("Could not find type " + type.FullName + " Amongst known window types");
                }
            }
        }

        public ToolStripItem[] GetMenuItems() {
            List<ToolStripItem> items = new List<ToolStripItem>();
            // now init the menu and create the required ones
            foreach (Lazy<IWindow, IWindowAttribute> windowType in this.WindowTypes) {
                Lazy<IWindow, IWindowAttribute> type = windowType; // avoiding access to modified closure?                
                items.Add(new ToolStripLabel(windowType.Metadata.WindowTypeName, null, false,
                                             (thesender, theargs) =>
                                             CreateNewWindow(type)) {
                                                                        AutoSize = true,
                                                                        ToolTipText =
                                                                            @"Create a new: " +
                                                                            windowType.Metadata.
                                                                                WindowTypeName +
                                                                            @" - " +
                                                                            windowType.Metadata.
                                                                                WindowTypeDescription
                                                                    });

            }

            return items.ToArray();
        }

        private readonly Dictionary<string, WeakReference> _windowMap;

        private delegate void BindCallback(string windowName, Type windowType, Dictionary<string, object> windowData);

        public void Bind(string windowName, Type windowType, Dictionary<string, object> windowData) {

            if (this.MdiForm.InvokeRequired) {
                BindCallback callback = Bind;
                this.MdiForm.Invoke(callback, new object[] { windowName,windowType,windowData });
                return;                
            }

            IWindow window = null;
            if (this._windowMap.ContainsKey(windowName)) {
                window = this._windowMap[windowName].Target as IWindow;
                if (window == null) {
                    this._windowMap.Remove(windowName);
                } else {
                    Form form = window as Form; // see if the form is still alive
                    if (form == null || !this.MdiForm.MdiChildren.Contains(form)) {
                        window = null;
                        this._windowMap.Remove(windowName);
                    }
                }
            }

            if (window == null) {
                window = CreateIWindow(windowType);
                this._windowMap.Add(windowName, new WeakReference(window));
            }

            window.BindTo(windowData);

        }

        public void Bind(string windowName, Type windowType, string windowDataName, object windowData) {            
            this.Bind(windowName, windowType,  new Dictionary<string, object> {{windowDataName, windowData}});
        }

        private delegate void RequestRefreshnyTypeCallback(Type type);
        private delegate void RequestRefreshbyNameCallback(string windowName);

        /// <summary>
        /// Requests the refresh of every window of a given type
        /// </summary>
        /// <param name="type">The type.</param>
        public void RequestRefresh(Type type) {
            if (this.MdiForm.InvokeRequired) {
                RequestRefreshnyTypeCallback d = this.RequestRefresh;
                this.MdiForm.BeginInvoke(d, new object[] {type});
                return;
            }

            foreach (IWindow window in this.MdiForm.MdiChildren.Where(f => f.GetType().Equals(type)).Cast<IWindow>()) {
                window.RefreshView();
            }
        }

        public void RequestRefresh(string windowName) {
            if (this.MdiForm.InvokeRequired) {
                RequestRefreshbyNameCallback d = this.RequestRefresh;
                this.MdiForm.BeginInvoke(d, new object[] { windowName });
                return;
            }

            foreach (IWindow window in
                _windowMap.Where(map => map.Key == windowName).Select(windowmap => windowmap.Value.Target).OfType<IWindow>()) {
                window.RefreshView();
            }
        }
    }
}
