using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static string ClientModsPath => ModsPath + "\\Client";
        public static string ServerModsPath => ModsPath + "\\Server";
        public static string AssetsPath => ModsPath + "\\Assets";
        public static string LogPath => ModsPath + "\\Logs";

        public ModLogger Logger;

        /// <summary>
        /// All known mods files
        /// </summary>
        private readonly List<Mod> allMods = new List<Mod>();

        /// <summary>
        /// Gets all known mods files
        /// </summary>
        public IEnumerable<Mod> AllMods => allMods;

        /// <summary>
        /// GameObject that holds our mods
        /// </summary>
        public GameObject GameObject { get; } = new GameObject();

        public ModLoader(ModLogger logger)
        {
            this.Logger = logger;
            UnityEngine.Object.DontDestroyOnLoad(GameObject);
        }

        /// <summary>
        /// Refresh all known mods
        /// </summary>
        public void LoadClientModFiles()
        {
            //Unloads & clears known mods
            //foreach (var mod in AllMods.Where(m=>m.FullPath.Contains(ClientModsPath)))
            //{
            //    RemoveMod(mod);
            //}

            //Find all files to load
            var mods = Directory.GetFiles(ClientModsPath, "*.dll").Select(file => AddMod(file)).ToList();
            //load them in the game
            mods.ForEach(m => LoadInGame(m));
        }

        /// <summary>
        /// Adds and registers a modfile
        /// </summary>
        public Mod AddMod(string file)
        {
            var mod = new Mod(file);
            allMods.Add(mod);
            LoadModTypes(mod);
            Logger.Log("Added dll: " + mod.Name);
            return mod;
        }

        /// <summary>
        /// Adds and registers a modfile
        /// </summary>
        public void RemoveMod(Mod mod)
        {
            UnloadFromGame(mod);
            //Save mod types and file path
            if (allMods.Contains(mod))
                allMods.Remove(mod);
            Logger.Log("Removed dll: " + mod.Name);
        }

        /// <summary>
        /// Finds and loads all mod classes in a file
        /// </summary>
        /// <param name="mod">The file to load</param>
        /// <returns></returns>
        private void LoadModTypes(Mod mod)
        {
            try
            {
                mod.Types = new List<Mod.ModType>();
                if (File.Exists(mod.FullPath))
                {
                    Assembly modDll = Assembly.LoadFile(mod.FullPath);
                    Type[] modType = modDll.GetTypes();
                    foreach (Type t in modType)
                    {
                        if (t.IsClass && !t.IsAbstract && t.IsPublic && typeof(MonoBehaviour).IsAssignableFrom(t))
                        {
                            mod.Types.Add(new Mod.ModType(t));
                            Logger.Log("Found type in " + mod.Name + ": " + t.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("Exception raised while loading mod " + mod.Name);
                Logger.Log(e.Message);
                Logger.Log("Skipped loading this mod");
            }
        }

        /// <summary>
        /// will load a mod from memory
        /// </summary>
        /// <param name="mod"></param>
        public void LoadInGame(Mod mod)
        {
            if (mod.TypesLoaded)
            {
                foreach (Mod.ModType t in mod.Types)
                {
                    //if mod is not loaded
                    if (!t.LoadedInGame)
                    {
                        t.Instance = (MonoBehaviour)GameObject.AddComponent(t.Type);
                        Logger.Log("Loaded: " + t.Type.Name + " From file: " + mod.Name);
                    }
                }
            }
        }

        //Unloads a mod from game
        public void UnloadFromGame(Mod mod)
        {
            if (mod.LoadedInGame)
            {
                foreach (Mod.ModType t in mod.Types)
                {
                    //if mod is loaded
                    if (t.LoadedInGame)
                    {
                        UnityEngine.Object.Destroy(t.Instance);
                        t.Instance = null;
                    }
                }
                Logger.Log("Unloaded: " + mod.Name);
            }
        }
    }
}