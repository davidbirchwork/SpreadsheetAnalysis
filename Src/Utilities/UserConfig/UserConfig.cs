using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Command;
using Utilities.Config;
using Utilities.Loggers;
using Utilities.Windowing;
using Utilities.XmlSerialisation;

namespace Utilities.UserConfig {


    //TODO BUG this is not thread safe! 

    /// <summary>
    /// Class for accessing user config files across many MEF'd components
    /// </summary>
    public static class UserConfig {

        // singleton static variable :)
        private static readonly UserConfigurator Configurator = new UserConfigurator();

        /// <summary>
        /// Access a the configuration file of this type!
        /// </summary>
        /// <typeparam name="TConfigFile">The type of the config file.</typeparam>
        /// <returns>a configuration file class!</returns>
        public static TConfigFile Configure<TConfigFile>() 
            where TConfigFile : IConfigFile {

            return Configurator.GetConfigFile<TConfigFile>();
        }

        public static bool SaveConfiguration(IConfigFile configFile) {
            return Configurator.SaveConfiguration(configFile);
        }

        public static IEnumerable<Tuple<string, string, Action>> GetMenuItems(WindowController windowController, CommandHistory commandHistory) {
            return Configurator.GetMenuItems(windowController, commandHistory);
        }
    }

    /// <summary>
    /// A class for MEF'ing all the IConfigFile
    /// </summary>
    public class UserConfigurator {

        [ImportMany]
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public Lazy<IConfigFile, IConfigAttribute>[] ConfigFiles { get; set; }

        private readonly Dictionary<Type, Tuple<string, IConfigFile>> _configCache = new Dictionary<Type, Tuple<string, IConfigFile>>();

        #region ctor

        public UserConfigurator() {            
            var container = AppConfig.GetCompositionContainer();
            container.ComposeParts(this);
        }

        #endregion

        public TConfigFile GetConfigFile<TConfigFile>()
              where TConfigFile : IConfigFile {

            // check the cache 

            if (this._configCache.ContainsKey(typeof (TConfigFile))) {
                return (TConfigFile) this._configCache[typeof (TConfigFile)].Item2;
            }

            // else go the long way... find the right type
            Lazy<IConfigFile, IConfigAttribute> configFile =
                this.ConfigFiles.FirstOrDefault(cfile => cfile.Value.GetType().Equals(typeof (TConfigFile)));
            if (configFile == null) {
                Logger.ERROR("Could not find config file of type " + typeof (TConfigFile).FullName +
                             " Are you missing a dll? Check your MEF imports in app.config file");
                return default(TConfigFile);
            }

            string appDir = AppConfig.GetExeDirectory();
            string configFilePath = appDir + configFile.Metadata.ConfigFileName;

            // check if its been overridden in AppConfig?
            var overrides = AppConfig.GetValues(configFile.Metadata.AppConfigOverrideEntry);
            if (overrides.Count() > 0) {
                string customFile = appDir + overrides.First();
                if (!File.Exists(customFile)) {                    

                    SafelyLog<TConfigFile>( "Could not find custom config file specified for " +
                                          configFile.Metadata.ConfigName +
                                          " in app.config it was specified to be in " + overrides.First() +
                                          " but this doesnt exist! Loading defaults instead!");
                } else {
                    configFilePath = customFile;
                }
            }

            // now check if it exists

            if (!File.Exists(configFilePath)) {
                SafelyLog<TConfigFile>("Could not find default config file specified for " +
                                       configFile.Metadata.ConfigName +
                                       " creating default one instead!");

                // if not get the default and serialise it to the correct place...
                var config = configFile.Value.ReturnDefault();
                File.WriteAllText(configFilePath, SerialisationController.Serialize(config));
            }

            // now read & return the file! 

            TConfigFile readConfig =
                (TConfigFile) SerialisationController.DeserializeFromXmlWithType(File.ReadAllText(configFilePath));

            //cache it first :)
            this._configCache.Add(typeof (TConfigFile), new Tuple<string,IConfigFile>(configFilePath,readConfig));

            return readConfig;
        }

        private static void SafelyLog<TConfigFile>(string errorMessage) {
            if (typeof(TConfigFile) == typeof(LoggingConfig)) {
                Task.Factory.StartNew(() => {
                                          Thread.Sleep(3000);// wait for log config file to appear!
                                          Logger.FAILURE(errorMessage);
                                      });
            } else {
                Logger.FAILURE(errorMessage);
            }
        }

        public IEnumerable<Tuple<string, string, Action>> GetMenuItems(WindowController windowController, CommandHistory commandHistory) {
            return this.ConfigFiles.Select(cfile => 
                       new Tuple<string, string, Action>(
                                cfile.Metadata.ConfigName,
                                cfile.Metadata.ConfigDescription,
                                () => EditConfig(cfile, windowController, commandHistory)));
        }

        private void EditConfig(Lazy<IConfigFile, IConfigAttribute> configFileMeta, WindowController windowController, CommandHistory commandHistory) { 
            // use reflection to set the generic parameter!
            MethodInfo method = typeof(UserConfigurator).GetMethod("GetConfigFile", new Type[0]).MakeGenericMethod(new[] { configFileMeta.Value.GetType() });
            IConfigFile configFile = (IConfigFile) method.Invoke(this, new object[0]);

            Editor.User.Edit(configFileMeta.Metadata.ConfigName,
                             configFile,
                             windowController,
                             newConfig => SaveConfiguration((IConfigFile) newConfig),
                             commandHistory,
                             false);
        }

        public bool SaveConfiguration(IConfigFile updatedConfigFile) {
            updatedConfigFile.NotifyUpdated();
            Type configType = updatedConfigFile.GetType();
            string cfilepath = this._configCache[configType].Item1;
            File.WriteAllText(cfilepath, SerialisationController.Serialize(updatedConfigFile));
            this._configCache[configType] = new Tuple<string, IConfigFile>(cfilepath, updatedConfigFile);
            return true;
        }
    }
}
