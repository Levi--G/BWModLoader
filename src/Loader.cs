using System;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace ModLoader
{
    public static class Loader
    {
        public static GameObject modObjects;
        public static FileInfo[] modFiles;

        public static void Log(string output)
        {
            Console.WriteLine("[BWML]" + output);
            //UnityEngine.Debug.Log("[BWML]" + output);
        }
        public static void Load()
        {
            Log("Starting mod loader...");

            string dllpath = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            string Path = new FileInfo(dllpath).Directory.FullName;
            string modsPath = Path + "\\Mods";
            string assetsPath = modsPath + "\\Assets";

            Log("Dll dir: "+Path);
            Log("Mods dir: "+modsPath);

            if (!Directory.Exists(modsPath))
            {
                Directory.CreateDirectory(modsPath);
            }

            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            DirectoryInfo d = new DirectoryInfo(modsPath);

            modObjects = new GameObject();

            //For each DLL in "Blackwake/Blackwake_Data/Managed/Mods/"
            //Open them, Get the mod class, then add it in the game.
            modFiles = d.GetFiles("*.dll");
            foreach (FileInfo file in modFiles)
            {
                try
                {
                    Assembly modDll = Assembly.LoadFrom(modsPath + "/" + file.Name);
                    Type[] modType = modDll.GetTypes();
                    foreach (Type t in modType)
                    {
                        Log("Found type in " + file.Name + ": " + t.Name);
                        if (t.IsClass && t.IsSubclassOf(typeof(MonoBehaviour)))
                        {
                            modObjects.AddComponent(t);
                            Log("Loaded '" + t.Name + "' in " + file.Name);
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
            Log("All Mods have been Loaded!");
            modObjects.AddComponent<ModGUI.ModGUI>();
            Log("GUI has been loaded");

            //Keep mods active
            UnityEngine.Object.DontDestroyOnLoad(modObjects);
        }
    }
}
