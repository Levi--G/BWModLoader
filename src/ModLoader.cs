using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BWModLoader
{
    public class ModLoader
    {
        public static ModLoader Instance { get; internal set; }

        private static readonly string dllpath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        public static string FolderPath => Path.GetDirectoryName(dllpath);
        public static string ModsPath => FolderPath + "\\Mods";
        public static string AssetsPath => ModsPath + "\\Assets";
        public static string LogPath => ModsPath + "\\Logs";

        public ModLogger Logger;

        /// <summary>
        /// All known mods files
        /// </summary>
        private readonly Dictionary<FileInfo, List<Type>> allMods = new Dictionary<FileInfo, List<Type>>();

        /// <summary>
        /// Gets all known mods files
        /// </summary>
        /// Prevent modification from outside by making copy
        public Dictionary<FileInfo, List<Type>> GetAllMods() => new Dictionary<FileInfo, List<Type>>(allMods);

        /// <summary>
        /// GameObject that holds our mods
        /// </summary>
        public GameObject ModObjects { get; } = new GameObject();

        public ModLoader(ModLogger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Checks if a mod is loaded
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public bool IsLoaded(FileInfo file)
        {
            foreach (Type mod in allMods[file])
            {
                if (ModObjects.GetComponent(mod) != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Refresh all known mods
        /// </summary>
        public void RefreshModFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(ModsPath);
            //Unloads & clears known mods
            foreach (var mod in GetAllMods())
            {
                RemoveModFile(mod.Key);
            }

            //Find all files to refresh
            foreach (FileInfo file in dir.GetFiles("*.dll"))
            {
                AddModFile(file);
            }
        }

        /// <summary>
        /// Adds and registers a modfile
        /// </summary>
        public void AddModFile(FileInfo file)
        {
            //Save mod types and file path
            allMods.Add(file, LoadModTypes(file));
            Logger.Log("Added dll: " + file.Name);
        }

        /// <summary>
        /// Adds and registers a modfile
        /// </summary>
        public void RemoveModFile(FileInfo file)
        {
            Unload(file);
            //Save mod types and file path
            if (allMods.ContainsKey(file))
                allMods.Remove(file);
            Logger.Log("Removed dll: " + file.Name);
        }

        /// <summary>
        /// Refreshes and reloads mod
        /// Has no use
        /// </summary>
        [Obsolete("Method has no use in current implementation")]
        public void ReloadModFile(FileInfo file)
        {
            if (!allMods.TryGetValue(file, out var types)) { return; }

            Unload(file);

            //Save mod types and file path
            allMods[file] = LoadModTypes(file);
            Logger.Log("Refreshed dll: " + file.Name);

            Load(file);
        }

        /// <summary>
        /// Finds and loads all mod classes in a file
        /// </summary>
        /// <param name="file">The file to load</param>
        /// <returns></returns>
        private List<Type> LoadModTypes(FileInfo file)
        {
            List<Type> mods = new List<Type>();
            try
            {
                file.Refresh();
                if (file.Exists)
                {
                    Assembly modDll = Assembly.LoadFile(file.FullName);
                    Type[] modType = modDll.GetTypes();
                    foreach (Type t in modType)
                    {
                        if (t.IsClass && typeof(MonoBehaviour).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                        {
                            mods.Add(t);
                            Logger.Log("Found type in " + file.Name + ": " + t.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("Exception raised while loading mod " + file.Name);
                Logger.Log(e.Message);
                Logger.Log("Skipped loading this mod");
            }
            return mods;
        }

        /// <summary>
        /// will load a mod from memory
        /// </summary>
        /// <param name="file"></param>
        public void Load(FileInfo file)
        {
            if (allMods.TryGetValue(file, out var types))
            {
                foreach (Type mod in types)
                {
                    //if mod is not loaded
                    if (ModObjects.GetComponent(mod) == null)
                    {
                        ModObjects.AddComponent(mod);
                        Logger.Log("Loaded: " + mod.Name + " From file: " + file.Name);
                    }
                }
            }
        }

        //Unloads a mod from game
        public void Unload(FileInfo file)
        {
            if (allMods.TryGetValue(file, out var types))
            {
                foreach (Type mod in types)
                {
                    //if mod is loaded
                    if (ModObjects.GetComponent(mod) != null)
                    {
                        UnityEngine.Object.Destroy(ModObjects.GetComponent(mod));
                        Logger.Log("Unloaded: " + mod.Name + " From file: " + file.Name);
                    }
                }
            }
        }
    }
}