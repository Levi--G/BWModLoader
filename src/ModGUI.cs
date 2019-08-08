using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
namespace BWModLoader.ModGUI
{
    public class ModGUI : MonoBehaviour
    {
        static bool debugEnabled;
        static int currentScreen;
        static Vector2 scrollPosition;
        static Vector2 size;
        static Vector2 position;
        void Start()
        {
            currentScreen = 0;
            scrollPosition = Vector2.zero;
            debugEnabled = false;
            size = new Vector2(1000, 1000);
            position = new Vector2((Screen.width / 2) - (size.x / 2),
                                   (Screen.height / 2) - (size.x / 2));
        }

        void Update()
        {
            if (Input.GetKeyUp("insert"))
            {
                debugEnabled = !debugEnabled;
            }

        }

        void OnGUI()
        {
            if (debugEnabled)
            {
                GUI.ModalWindow(0, new Rect(position, size), DebugWindow, "[BWML]Debug Menu");
            }
        }

        void DebugWindow(int windowID)
        {
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            currentScreen = GUI.SelectionGrid(new Rect(25, 25, size.x-50, 75), currentScreen,
                                              new string[] { "Mods", "Logs", "Debug"}, 3);
            switch (currentScreen)
            {
                case 0:
                    ModWindow();
                    break;
                case 1:
                    LogWindow();
                    break;
                case 2:
                    TestingWindow();
                    break;
            }
        }
        void TestingWindow()
        {
            if(GUI.Button(new Rect(0,100,size.x,25), "Get Refrences"))
            {
                Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
                foreach(Assembly a in asms)
                {
                    ModLoader.Instance.Logger.Log(a.FullName);
                }
            }
        }
        void LogWindow()
        {
            int logNum = 0;
            if (ModLoader.Instance.Logger.Logs.Any())
            {
                scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y - 100), 
                                                     scrollPosition, new Rect(0, 0, size.x, 25*ModLoader.Instance.Logger.Logs.Count));
                if(GUI.Button(new Rect(0,0,size.x,25), "Clear Logs"))
                {
                    ModLoader.Instance.Logger.Logs.Clear();
                }
                foreach (string log in ModLoader.Instance.Logger.Logs)
                {
                    logNum++;
                    GUI.Label(new Rect(0, 25 * logNum, 1000, 25), log);
                }
                GUI.EndScrollView();
            }
        }
        void ModWindow()
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y-100), scrollPosition, new Rect(0, 0, size.x, 50));
            if (GUI.Button(new Rect(0, 0, size.x, 25), "Reload all mods"))
            {
                ModLoader.Instance.RefreshModFiles();
            }
            int modNum = 0;
            var allmods = ModLoader.Instance.GetAllMods();
            foreach (FileInfo file in allmods.Keys)
            {
                modNum++;
                GUI.Label(new Rect(0, modNum * 25, 100, 25), file.Name);
                if (!ModLoader.Instance.IsLoaded(file))
                {
                    if (GUI.Button(new Rect(100, modNum * 25, 100, 25), "Enable"))
                    {
                        ModLoader.Instance.Load(file);
                    }
                }
                else
                {
                    if (GUI.Button(new Rect(100, modNum * 25, 100, 25), "Disable"))
                    {
                        ModLoader.Instance.Unload(file);
                    }
                }
                if (GUI.Button(new Rect(200, modNum * 25, 100, 25), "Reload"))
                {
                    ModLoader.Instance.Unload(file);
                    ModLoader.Instance.RefreshModFiles();
                }
            }
            GUI.EndScrollView();
        }
    }
}