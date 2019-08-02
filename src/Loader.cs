using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace ModLoader
{
    public static class Loader
    {
        public static GameObject modObjects;
        public static Dictionary<Component, FileInfo> loadedMods = new Dictionary<Component, FileInfo>(); 

        public static void Log(string output)
        {
            Console.WriteLine("[BWML]" + output);
            //UnityEngine.Debug.Log("[BWML]" + output);
        }

        public static void DebugLog(string output)
        {
#if DEBUG
            Log(output);
#endif // DEBUG
        }

        static void FindMods(DirectoryInfo path)
        {
            FileInfo[] files = path.GetFiles("*.dll");
            foreach (FileInfo file in files)
            {
                try
                {
                    Assembly modDll = Assembly.LoadFrom(path.FullName + "/" + file.Name);
                    Type[] modType = modDll.GetTypes();
                    foreach (Type t in modType)
                    {
                        DebugLog("Found type in " + file.Name + ": " + t.Name);
                        if (t.IsClass && t.IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            loadedMods.Add(modObjects.AddComponent(t), file);
                            Log("Loaded " + t.Name + " from file: " + file.Name);
                            //CleanMods();
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("Exception raised while loading mod " + file.Name);
                    Log(e.Message);
                    Log("Skipped loading this mod");
                }
            }
        }
        public static void RemoveMod(Component mod)
        {
            UnityEngine.Object.Destroy(mod);
            loadedMods.Remove(mod);
        }
        static void CleanMods()
        {
            foreach(KeyValuePair<Component, FileInfo> mod in loadedMods)
            {
                if (modObjects.GetComponent(mod.Key.GetType()) == null)
                {
                    loadedMods.Remove(mod.Key);
                }
            }
        }

        public static void Load()
        {
            Log("Starting mod loader...");

            string dllpath = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string Path = new FileInfo(dllpath).Directory.FullName;
            string modsPath = Path + "\\Mods";
            string assetsPath = modsPath + "\\Assets";

            DebugLog("Dll dir: "+Path);
            DebugLog("Mods dir: "+modsPath);

            if (!Directory.Exists(modsPath))
            {
                Directory.CreateDirectory(modsPath);
            }

            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            modObjects = new GameObject();

            //For each DLL in "Blackwake/Blackwake_Data/Managed/Mods/"
            //Open them, Get the mod class, then add it in the game.
            DirectoryInfo d = new DirectoryInfo(modsPath);
            FindMods(d);
            foreach(Component mod in loadedMods.Keys)
            {
                Log("Mod \"" + mod.name + "\" loaded from \"" + loadedMods[mod].Name + "\"");
            }
            Log("All Mods have been Loaded!");
            modObjects.AddComponent<ModGUI.ModGUI>();
            Log("GUI has been loaded");

            //Keep mods active
            UnityEngine.Object.DontDestroyOnLoad(modObjects);
        }
    }
}
