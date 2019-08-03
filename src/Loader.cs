using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
namespace ModLoader
{
    public static class Loader
    {
        public static void Load()
        {
            Utils.Log("Starting mod loader...");
            Utils.DebugLog("Mods dir: "+Utils.modsPath);

            if (!Directory.Exists(Utils.modsPath))
            {
                Directory.CreateDirectory(Utils.modsPath);
            }

            if (!Directory.Exists(Utils.assetsPath))
            {
                Directory.CreateDirectory(Utils.assetsPath);
            }

            Utils.modObjects = new GameObject();

            //For each DLL in "Blackwake/Blackwake_Data/Managed/Mods/"
            //Open them, Get the mod class, then add it in the game.
            Utils.RefreshModFiles();
            foreach(FileInfo file in Utils.allMods.Keys)
            {
                Utils.Load(file);
            }
            Utils.Log("All Mods have been Loaded!");
            Utils.modObjects.AddComponent<ModGUI.ModGUI>();
            Utils.Log("GUI has been loaded");

            //Keep mods active
            UnityEngine.Object.DontDestroyOnLoad(Utils.modObjects);
        }
    }
}
