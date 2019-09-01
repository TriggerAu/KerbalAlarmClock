/* Part of KSPPluginFramework
Version 1.2

Forum Thread:https://forum.kerbalspaceprogram.com/topic/60381-ksp-plugin-framework-plugin-examples-and-structure/
Author: TriggerAu, 2014
License: The MIT License (MIT)
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;

namespace KSPPluginFramework
{ 
    public abstract class ConfigNodeStorage : IPersistenceLoad, IPersistenceSave
    {
        #region Constructors
        /// <summary>
        /// Class Constructor
        /// </summary>
        public ConfigNodeStorage() { }
        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="FilePath">Set the path for saving and loading. This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)</param>
        public ConfigNodeStorage(String FilePath) { this.FilePath = FilePath; }
        #endregion

        #region Properties
        private String _FilePath;
        /// <summary>
        /// Location of file for saving and loading methods
        /// 
        /// This can be an absolute path (eg c:\test.cfg) or a relative path from the location of the assembly dll (eg. ../config/test)
        /// </summary>
        public String FilePath
        {
            get { return _FilePath; }
            set
            {
                //Combine the Location of the assembly and the provided string. This means we can use relative or absolute paths
                _FilePath = System.IO.Path.Combine(_AssemblyFolder, value).Replace("\\","/");
            }
        }

        /// <summary>
        /// Gets the filename portion of the FullPath
        /// </summary>
        public String FileName
        {
            get { return System.IO.Path.GetFileName(FilePath); }
        }
        #endregion

        #region Interface Methods
        /// <summary>
        /// Wrapper for our overridable functions
        /// </summary>
        void IPersistenceLoad.PersistenceLoad()
        {
            OnDecodeFromConfigNode();
        }
        /// <summary>
        /// Wrapper for our overridable functions
        /// </summary>
        void IPersistenceSave.PersistenceSave()
        {
            OnEncodeToConfigNode();
        }

        /// <summary>
        /// This overridable function executes whenever the object is loaded from a config node structure. Use this for complex classes that need decoding from simple confignode values
        /// </summary>
        public virtual void OnDecodeFromConfigNode() { }
        /// <summary>
        /// This overridable function executes whenever the object is encoded to a config node structure. Use this for complex classes that need encoding into simple confignode values
        /// </summary>
        public virtual void OnEncodeToConfigNode() { }
        #endregion

        /// <summary>
        /// Test whether the configured FilePath exists
        /// </summary>
        /// <returns>True if its there</returns>
        public Boolean FileExists
        {
            get
            {
                return System.IO.File.Exists(FilePath);
            }
        }
    
        /// <summary>
        /// Loads the object from the ConfigNode structure in the previously supplied file
        /// </summary>
        /// <returns>Succes of Load</returns>
        public Boolean Load()
        {
            return this.Load(FilePath);
        }
        /// <summary>
        /// Loads the object from the ConfigNode structure in a file
        /// </summary>
        /// <param name="fileFullName">Absolute Path to the file to load the ConfigNode structure from</param> 
        /// <returns>Success of Load</returns>
        public Boolean Load(String fileFullName)
        {
            Boolean blnReturn = false;
            try
            {
                LogFormatted_DebugOnly("Loading ConfigNode");
                if (FileExists)
                {
                    //Load the file into a config node
                    ConfigNode cnToLoad = ConfigNode.Load(fileFullName);
                    //remove the wrapper node that names the class stored
                    ConfigNode cnUnwrapped = cnToLoad.GetNode(this.GetType().Name);
                    //plug it in to the object
                    ConfigNode.LoadObjectFromConfig(this, cnUnwrapped);
                    blnReturn = true;
                }
                else
                {
                    LogFormatted("File could not be found to load({0})", fileFullName);
                    blnReturn = false;
                }
            }
            catch (Exception ex)
            {
                LogFormatted("Failed to Load ConfigNode from file({0})-Error:{1}", fileFullName, ex.Message);
                LogFormatted("Storing old config - {0}", fileFullName + ".err-" + string.Format("ddMMyyyy-HHmmss", DateTime.Now));
                System.IO.File.Copy(fileFullName, fileFullName + ".err-" + string.Format("ddMMyyyy-HHmmss", DateTime.Now),true);
                blnReturn = false;
            }
            return blnReturn;
        }

        /// <summary>
        /// Saves the object to a ConfigNode structure in the previously supplied file
        /// </summary>
        /// <returns>Succes of Save</returns>
        public Boolean Save()
        {
            LogFormatted_DebugOnly("Saving ConfigNode");
            return this.Save(FilePath);
        }

        /// <summary>
        /// Saves the object to a ConfigNode structure in a file
        /// </summary>
        /// <param name="fileFullName">Absolute Path to the file to load the ConfigNode structure from</param> 
        /// <returns>Success of Save</returns>
        public Boolean Save(String fileFullName)
        {
            Boolean blnReturn = false;
            try
            {
                if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(fileFullName)))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileFullName));
                }
            }
            catch (Exception ex)
            {
                LogFormatted("Unable to create directory for ConfigNode file({0})-Error:{1}", fileFullName, ex.Message);
                blnReturn = false;
            }

            try
            {
                //Encode the current object
                ConfigNode cnToSave = this.AsConfigNode;
                //Wrap it in a node with a name of the class
                ConfigNode cnSaveWrapper = new ConfigNode(this.GetType().Name);
                cnSaveWrapper.AddNode(cnToSave);
                //Save it to the file
                cnSaveWrapper.Save(fileFullName);
                blnReturn = true;
            }
            catch (Exception ex)
            {
                LogFormatted("Failed to Save ConfigNode to file({0})-Error:{1}", fileFullName, ex.Message);
                blnReturn = false;
            }
            return blnReturn;
        }

        /// <summary>
        /// Returns the current object as a ConfigNode
        /// </summary>
        public ConfigNode AsConfigNode
        {
            get
            {
                try
                {
                    //Create a new Empty Node with the class name
                    ConfigNode cnTemp = new ConfigNode(this.GetType().Name);
                    //Load the current object in there
                    cnTemp = ConfigNode.CreateConfigFromObject(this, cnTemp);
                    return cnTemp;
                }
                catch (Exception ex)
                {
                    LogFormatted("Failed to generate ConfigNode-Error;{0}", ex.Message);
                    //Logging and return value?                    
                    return new ConfigNode(this.GetType().Name);
                }
            }
        }


        #region Assembly/Class Information
        /// <summary>
        /// Name of the Assembly that is running this MonoBehaviour
        /// </summary>
        internal static String _AssemblyName
        { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name; } }

        /// <summary>
        /// Full Path of the executing Assembly
        /// </summary>
        internal static String _AssemblyLocation
        { get { return System.Reflection.Assembly.GetExecutingAssembly().Location; } }

        /// <summary>
        /// Folder containing the executing Assembly
        /// </summary>
        internal static String _AssemblyFolder
        { get { return System.IO.Path.GetDirectoryName(_AssemblyLocation); } }

        #endregion  

        #region Logging
        /// <summary>
        /// Some Structured logging to the debug file - ONLY RUNS WHEN DLL COMPILED IN DEBUG MODE
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void LogFormatted_DebugOnly(String Message, params object[] strParams)
        {
            LogFormatted("DEBUG: " + Message, strParams);
        }

        /// <summary>
        /// Some Structured logging to the debug file
        /// </summary>
        /// <param name="Message">Text to be printed - can be formatted as per String.format</param>
        /// <param name="strParams">Objects to feed into a String.format</param>
        internal static void LogFormatted(String Message, params object[] strParams)
        {
            Message = String.Format(Message, strParams);                  // This fills the params into the message
            String strMessageLine = String.Format("{0},{2},{1}",
                DateTime.Now, Message,
                _AssemblyName);                                           // This adds our standardised wrapper to each line
            UnityEngine.Debug.Log(strMessageLine);                        // And this puts it in the log
        }

        #endregion

    }
}