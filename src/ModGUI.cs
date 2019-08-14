using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWModLoader.ModGUI
{
    public enum ScreenType
    {
        MOD,
        LOG
    }

    /// <summary>
    /// Manages contents and behaviour of the Debug Window
    /// </summary>
    public class ModGUI : MonoBehaviour
    {
        bool debugEnabled;
        int currentScreen;
        Vector2 scrollPosition;
        Vector2 size;
        Vector2 position;

        void Start()
        {
            currentScreen = (int)ScreenType.MOD;
            scrollPosition = Vector2.zero;
            debugEnabled = false;
            size = new Vector2(1000, 1000);
            position = new Vector2((Screen.width / 2) - (size.x / 2),
                                   (Screen.height / 2) - (size.x / 2));
        }

        /// <summary>
        /// Toggles the Window when the Hotkey is pressed
        /// </summary>
        void Update()
        {
            if (Input.GetKeyUp("insert"))
            {
                debugEnabled = !debugEnabled;
            }
        }

        /// <summary>
        /// Code that executes if the window was opened
        /// </summary>
        void OnGUI()
        {
            if (debugEnabled)
            {
                GUI.ModalWindow(0, new Rect(position, size), DebugWindow, "[BWML]Debug Menu");
            }
        }

        /// <summary>
        /// Manages contents of the window
        /// </summary>
        /// <param name="windowID">ID of the Debug Window</param>
        void DebugWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            currentScreen = GUI.SelectionGrid(new Rect(25, 25, size.x - 50, 75), currentScreen,
                                              new string[] { "Mods", "Logs", "Debug" }, 3);
            switch (currentScreen)
            {
                case (int)ScreenType.MOD:
                    ModWindow();
                    break;
                case (int)ScreenType.LOG:
                    LogWindow();
                    break;
            }
        }

        /// <summary>
        /// Logging window content
        /// </summary>
        void LogWindow()
        {
            int logNum = 0;
            if (ModLogger.Logs.Any())
            {
                scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y - 100),
                                                     scrollPosition, new Rect(0, 0, size.x, 25 * ModLogger.Logs.Count));
                if (GUI.Button(new Rect(0, 0, size.x, 25), "Clear Logs"))
                {
                    ModLogger.Logs.Clear();
                }
                foreach (string log in ModLogger.Logs)
                {
                    logNum++;
                    GUI.Label(new Rect(0, 25 * logNum, 1000, 25), log);
                }
                GUI.EndScrollView();
            }
        }

        /// <summary>
        /// Mod window content
        /// </summary>
        void ModWindow()
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y - 100), scrollPosition, new Rect(0, 0, size.x, 50));
            int modNum = 0;
            foreach (Mod mod in ModLoader.Instance.AllMods)
            {
                bool newCheckboxStatus = GUI.Toggle(new Rect(5, modNum * 25, 150, 25), mod.LoadedInGame, mod.Name);
                if (newCheckboxStatus)
                {
                    if (!mod.LoadedInGame)
                    {
                        ModLoader.Instance.LoadInGame(mod);
                    }
                }
                else
                {
                    if (mod.LoadedInGame)
                    {
                        ModLoader.Instance.UnloadFromGame(mod);
                    }
                }
                //todo show mod.GetSettingsMenu
                //if (GUI.Button(new Rect(255, modNum * 25, 100, 25), "Menu"))
                //{
                //    foreach (Type mod in ModLoader.Instance.AllMods()[mod])
                //    {
                //        ModLoader.Instance.GameObject[mod].BroadcastMessage("OnSettingsMenu");
                //    }
                //}
                modNum++;
            }
            GUI.EndScrollView();
        }
    }
}