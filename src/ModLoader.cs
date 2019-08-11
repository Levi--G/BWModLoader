﻿using System;
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
        private static readonly string folderPath = Path.GetDirectoryName(dllpath);
        public static string ModsPath => folderPath + "\\Mods";
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
        public bool IsLoaded(Type mod)
        {
            return ModObjects.GetComponent(mod) != null;
        }

        /// <summary>
        /// Refresh all known mods
        /// </summary>
        public void RefreshModFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(ModsPath);
            //Unloads & clears known mods
            foreach (var mod in allMods)
            {
                Unload(mod.Key);
            }
            allMods.Clear();

            //Find all files to refresh
            foreach (FileInfo file in dir.GetFiles("*.dll"))
            {
                //Save mod types and file path
                allMods.Add(file, LoadModTypes(file));
                Logger.Log("Found dll: " + file.Name);
            }
        }

        /// <summary>
        /// Refreshes and reloads mod
        /// </summary>
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
                        Logger.Log("Found type in " + file.Name + ": " + t.Name);
                        if (t.IsClass && typeof(MonoBehaviour).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                        {
                            mods.Add(t);
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
                    if (!IsLoaded(mod))
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
                    if (IsLoaded(mod))
                    {
                        UnityEngine.Object.Destroy(ModObjects.GetComponent(mod));
                        Logger.Log("Unloaded: " + mod.Name + " From file: " + file.Name);
                    }
                }
            }
        }
    }
}