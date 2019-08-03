using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace ModLoader
{
    public static class Utils
    {
        static readonly string dllpath = new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        static readonly string path = new FileInfo(dllpath).Directory.FullName;
        public static string modsPath = path + "\\Mods";
        public static string assetsPath = modsPath + "\\Assets";

        //all known mods files and their classes
        public static Dictionary<FileInfo, List<Type>> allMods = new Dictionary<FileInfo, List<Type>>();
        //Log history
        public static List<string> logs = new List<string>();
        //Object that holds our mods
        public static GameObject modObjects;

        //checks if mod is loaded
        public static bool IsLoaded(Type mod)
        {
            if (modObjects.GetComponent(mod) != null)
            {
                return true;
            }
            return false;
        }
        //Refresh all known mods
        public static void RefreshModFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(modsPath);
            //Clears known mods
            allMods.Clear();

            //Find all files to refresh
            foreach (FileInfo file in dir.GetFiles("*.dll"))
            {

                allMods.Add(file, new List<Type>());
                foreach(Type mod in FindModTypes(file))
                {
                    //Save mod type and file path
                    allMods[file].Add(mod);
                }
                Log("Found dll: " + file.Name);
            }
        }
        //Find all mod classes in a file
        static List<Type> FindModTypes(FileInfo file)
        {
            List<Type> mods = new List<Type>();
            try
            {
                Assembly modDll = Assembly.LoadFrom(file.FullName);
                Type[] modType = modDll.GetTypes();
                foreach (Type t in modType)
                {
                    Log("Found type in " + file.Name + ": " + t.Name);
                    if (t.IsClass && t.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        mods.Add(t);
                    }
                }
            }
            catch (Exception e)
            {
                Log("Exception raised while loading mod " + file.Name);
                Log(e.Message);
                Log("Skipped loading this mod");
            }
            return mods;
        }
        //will load a mod from memory
        public static void Load(FileInfo file)
        {
            if (allMods.ContainsKey(file))
            {
                foreach (Type mod in allMods[file])
                {
                    //if mod is not loaded
                    if (modObjects.GetComponent(mod) == null)
                    {
                        modObjects.AddComponent(mod);
                        Log("Loaded: " + mod.Name + " From file: " + file.Name);
                    }
                }
            }
        }
        //Unloads a mod from game
        public static void Unload(FileInfo file)
        {
            foreach(Type mod in allMods[file])
            {
                //if mod is loaded
                if (modObjects.GetComponent(mod) != null)
                {
                    UnityEngine.Object.Destroy(modObjects.GetComponent(mod));
                    Log("Unloaded: " + mod.Name + " From file: " + file.Name);
                }
            }
        }
        //Log to debugger and output.txt
        public static void Log(string output)
        {
            Console.WriteLine("[BWML]" + output);
            logs.Add(output);
        }
        public static void DebugLog(string output)
        {
            #if DEBUG
            Log(output);
            #endif // DEBUG
        }
    }
}
