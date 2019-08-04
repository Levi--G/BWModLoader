using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BWModLoader
{
    public static class Loader
    {
        public static void Load()
        {
            string logfile = null;
#if DEBUG
            logfile = ModLoader.LogPath + "\\modloader.log";
#endif
            ModLogger logger = new ModLogger("[BWML]", logfile);
            logger.ClearLog();
            logger.Log("Starting mod loader...");
            logger.DebugLog("Mods dir: " + ModLoader.ModsPath);

            if (!Directory.Exists(ModLoader.ModsPath))
            {
                Directory.CreateDirectory(ModLoader.ModsPath);
            }

            if (!Directory.Exists(ModLoader.AssetsPath))
            {
                Directory.CreateDirectory(ModLoader.AssetsPath);
            }

            //For each DLL in "Blackwake/Blackwake_Data/Managed/Mods/"
            //Open them, Get the mod class, then add it in the game.
            ModLoader loader = new ModLoader(logger);
            ModLoader.Instance = loader;
            loader.RefreshModFiles();
            foreach (FileInfo file in loader.GetAllMods().Keys)
            {
                loader.Load(file);
            }
            logger.Log("All Mods have been Loaded!");
            loader.ModObjects.AddComponent<ModGUI.ModGUI>();
            logger.Log("GUI has been loaded");

            //Keep mods active
            UnityEngine.Object.DontDestroyOnLoad(loader.ModObjects);
        }
    }
}
