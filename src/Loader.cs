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
            
            ModLoader loader = new ModLoader(logger);
            ModLoader.Instance = loader;
            loader.LoadClientModFiles();
            logger.Log("All Mods have been Loaded!");
            GameObject ModGUI = new GameObject();
            ModGUI.AddComponent<ModGUI.ModGUI>();
            UnityEngine.Object.DontDestroyOnLoad(ModGUI);
            logger.Log("GUI has been loaded");
        }
    }
}
