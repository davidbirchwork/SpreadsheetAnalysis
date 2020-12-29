using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Utilities.Config;
using Utilities.Loggers;

namespace Utilities.SaveLoad {
    /// <summary>
    /// Save an load classes
    /// </summary>
    public static class SaveLoad {

        /// <summary>
        /// Prompts the user to load a given file type.
        /// </summary>
        /// <param name="defaultExt">The default ext.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="loadMessage">The load message.</param>
        /// <returns>a filename or null</returns>
        public static string LoadFileType(string defaultExt, string filter, string loadMessage) {

            OpenFileDialog opener = new OpenFileDialog {
                                                           Title = loadMessage,
                                                           InitialDirectory = AppConfig.GetExeDirectory(),
                                                           DefaultExt = defaultExt,
                                                           Filter = filter,
                                                           AutoUpgradeEnabled = true,
                                                           CheckPathExists = true,
                                                           CheckFileExists = true
                                                       };

            return opener.ShowDialog() == DialogResult.OK ? opener.FileName : null;
        }

        public static T Load<T>(string fileName) where T : class, ISaveLoad {
            MemberInfo info = typeof(T);
            FileTypeAttribute[] attributes =
                (FileTypeAttribute[])info.GetCustomAttributes(typeof(FileTypeAttribute), true);
            if (attributes.Length != 1) {
                Logger.FAILURE("Could not save a Load a type not tagged with a single FileTypeAttribute.");
                return default(T);
            }
            FileTypeAttribute options = attributes[0];

            if (!File.Exists(fileName)) {
                MessageBox.Show(string.Format("Could not load file '{0}'", fileName));
                Logger.FAILURE(string.Format("Could not load file '{0}'", fileName));
                return Load<T>();
            }

            T newobj;

            switch (options.Type) {
                case SerializationType.Xml:
                    // XML                     
                    try {
                        newobj = XmlSerialisation.SerialisationController.Deserialize<T>(File.ReadAllText(fileName));
                    } catch (Exception e) {
                        Logger.FAILURE("Load as XML failed with error " + e.Message);
                        return default(T);
                    }

                    break;
                default:
                    Logger.FAILURE("Unknown Serialization Type " + options.Type);
                    return default(T);
            }

            if (newobj == default(T)) {
                Logger.FAILURE("Failed to Type " + options.Type + " from " + fileName);
                return default(T);
            }

            newobj.FileName = fileName;
            Logger.SUCCESS("Successfully loaded a " + typeof(T).Name + " from file " + fileName);
            return newobj;
        }

        public static T Load<T>() where T : class, ISaveLoad {

            MemberInfo info = typeof(T);
            FileTypeAttribute[] attributes =
                (FileTypeAttribute[])info.GetCustomAttributes(typeof(FileTypeAttribute), true);
            if (attributes.Length != 1) {
                Logger.FAILURE("Could not save a Load a type not tagged with a single FileTypeAttribute.");
                return default(T);
            }

            FileTypeAttribute options = attributes[0];

            string initialDir = Directory.Exists(options.AbsoluteFolder)
                                    ? options.AbsoluteFolder
                                    : !string.IsNullOrWhiteSpace(options.RelativeFolder)
                                          ? (Directory.Exists(AppConfig.GetExeDirectory() +
                                                              options.RelativeFolder)
                                                 ? AppConfig.GetExeDirectory() + options.RelativeFolder
                                                 : "CREATEME"
                                            )
                                          : AppConfig.GetExeDirectory();

            if (initialDir == "CREATEME") {
                try {
                    Directory.CreateDirectory(AppConfig.GetExeDirectory() + options.RelativeFolder);
                } catch (Exception e) {
                    Logger.DEBUG("Could not create directory : " + AppConfig.GetExeDirectory() +
                                 options.RelativeFolder + " error: " + e.Message);
                    initialDir = AppConfig.GetExeDirectory();
                }
            }

            OpenFileDialog opener = new OpenFileDialog {
                Title =
                    !string.IsNullOrWhiteSpace(options.LoadMessage)
                        ? options.LoadMessage
                        : "Load File",
                InitialDirectory = initialDir,
                DefaultExt =
                    !string.IsNullOrWhiteSpace(options.DefaultExtension)
                        ? options.DefaultExtension
                        : ".xml",
                Filter = !string.IsNullOrWhiteSpace(options.Filter)
                             ? options.Filter
                             : ".xml",
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                CheckFileExists = true
            };

            return opener.ShowDialog() == DialogResult.OK ? Load<T>(opener.FileName) : default(T);
        }

        public static bool SaveAs(ISaveLoad obj) {
            return Save(obj, true);
        }

        public static bool Save(ISaveLoad obj, bool silent= false) {
            return Save(obj, false, silent);
        }

        private static bool Save(ISaveLoad obj, bool saveAs, bool silent ) {
            if (obj == null) {
                return Logger.FAILURE("Could not save a null object.");
            }

            MemberInfo info = obj.GetType();
            FileTypeAttribute[] attributes =
                (FileTypeAttribute[])info.GetCustomAttributes(typeof(FileTypeAttribute), true);
            if (attributes.Length != 1) {
                return Logger.FAILURE("Could not save a object not tagged with a single FileTypeAttribute.");
            }

            FileTypeAttribute options = attributes[0];

            // show the save dialog
            if (saveAs || (!saveAs && !File.Exists(obj.FileName) && !silent)) {
                string initialDir = File.Exists(obj.FileName)
                                        ? Path.GetDirectoryName(obj.FileName)
                                        : Directory.Exists(options.AbsoluteFolder)
                                              ? options.AbsoluteFolder
                                              : !string.IsNullOrWhiteSpace(options.RelativeFolder)
                                                    ? (Directory.Exists(AppConfig.GetExeDirectory() +
                                                                        options.RelativeFolder)
                                                           ? AppConfig.GetExeDirectory() + options.RelativeFolder
                                                           : "CREATEME"
                                                      )
                                                    : AppConfig.GetExeDirectory();

                if (initialDir == "CREATEME") {
                    try {
                        initialDir = AppConfig.GetExeDirectory() + options.RelativeFolder;
                        Directory.CreateDirectory(initialDir);
                    } catch (Exception e) {
                        Logger.DEBUG("Could not create directory : " + AppConfig.GetExeDirectory() +
                                     options.RelativeFolder + " error: " + e.Message);
                        initialDir = AppConfig.GetExeDirectory();
                    }
                }

                SaveFileDialog saver = new SaveFileDialog {
                    Title =
                        !string.IsNullOrWhiteSpace(options.SaveMessage)
                            ? options.SaveMessage
                            : "Save File",
                    InitialDirectory = initialDir,
                    DefaultExt =
                        !string.IsNullOrWhiteSpace(options.DefaultExtension)
                            ? options.DefaultExtension
                            : ".xml",
                    AddExtension = true,
                    Filter = !string.IsNullOrWhiteSpace(options.Filter)
                                 ? options.Filter
                                 : ".xml",
                    AutoUpgradeEnabled = true,
                    FileName = !string.IsNullOrWhiteSpace(obj.FileName)
                                   ? obj.FileName
                                   : "",
                    CheckPathExists = true
                };
                if (saver.ShowDialog() != DialogResult.OK) {
                    return false;
                }

                obj.FileName = saver.FileName;
                //set before save which may yet fail but if so the user still has what they tried to re-do
                // also the saved file has its correct filename!
            }

            switch (options.Type) {
                case SerializationType.Xml:
                    // XML                     
                    try {
                        /* old code... too memory intensive - i dont think we need to bother with UTF8 nonsense...
                        // serialize it
                        string xml = XmlSerialisation.SerialisationController.Serialize(obj);

                        // as an extra check lets parse the xml and then save it

                        // Encode the XML string in a UTF-8 byte array 
                        byte[] encodedString = Encoding.UTF8.GetBytes(xml);

                        // Put the byte array into a stream and rewind it to the beginning 
                        MemoryStream ms = new MemoryStream(encodedString);
                        ms.Flush();
                        ms.Position = 0;

                        // Build the XmlDocument from the MemorySteam of UTF-8 encoded bytes 
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(ms);
                        xmlDoc.Save(obj.FileName);

                         */
                        XmlSerialisation.SerialisationController.SerializeToXElement(obj).Save(obj.FileName);
                        
                    } catch (Exception e) {
                        return Logger.FAILURE("Save as XML failed with error " + e.Message+ "Memory= "+GC.GetTotalMemory(true));
                    }

                    break;
                default:
                    return Logger.FAILURE("Unknown Serialization Type " + options.Type);
            }

            return Logger.SUCCESS("Successfully saved a " + obj.GetType().Name + " to file " + obj.FileName);
        }


        /// <summary>
        /// Saves the with a delegate.
        /// currently doesn't work unless called from a STA thread :(
        /// </summary>
        /// <param name="saveMessage">The save message.</param>
        /// <param name="initialDir">The initial dir.</param>
        /// <param name="extension">The extension.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="saveFunc">The save func.</param>
        /// <returns></returns>
        public static bool SaveWithDelegate(string saveMessage, string initialDir, string extension, string filter, Action<string> saveFunc) {
            SaveFileDialog saver = new SaveFileDialog {
                Title =
                    !string.IsNullOrWhiteSpace(saveMessage)
                        ? saveMessage
                        : "Save File",
                InitialDirectory = initialDir,
                DefaultExt = extension,
                AddExtension = true,
                Filter = filter,
                AutoUpgradeEnabled = true,
                CheckPathExists = true
            };
            if (saver.ShowDialog() == DialogResult.OK) {
                saveFunc.Invoke(saver.FileName);
                return true;
            }
            return false;
        }
    }
}
