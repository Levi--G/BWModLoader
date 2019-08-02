using UnityEngine;
namespace ModGUI
{
    public class ModGUI : MonoBehaviour
    {
        static bool debugEnabled;
        static int currentScreen;
        static Vector2 scrollPosition;
        //static Dictionary<string, string> test;
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
                                              new string[] { "Mods", "Logs"}, 2);
            if(currentScreen == 0)
            {
                ModWindow();
            }
            else
            {
                LogWindow();
            }
        }

        void LogWindow()
        {
            GUI.Label(new Rect(0, 100, 100, 25), "LogWindow");
        }
        void ModWindow()
        {
            GUI.Label(new Rect(0, 100, 100, 25), "ModWindow");
            scrollPosition = GUI.BeginScrollView(new Rect(0, 100, size.x, size.y-100), scrollPosition, new Rect(0, 0, size.x, 50));
            int modNum = 0;
            foreach (Component mod in ModLoader.Loader.modObjects.GetComponents(typeof(Component)))
            {
                GUI.Label(new Rect(0, modNum * 25, 100, 25), mod.name);
                modNum++;
            }
            GUI.EndScrollView();
        }
    }
    /*
    static class DebugWindows
    {
        public static void LogWindow()
        {
            GUI.Label(new Rect(0, 100, 100, 25), "LogWindow");
        }
        public static void ModWindow()
        {
            GUI.Label(new Rect(0, 100, 100, 25), "ModWindow");
            scrollPosition = GUI.BeginScrollView(new Rect(10, 300, 100, 100), scrollPosition, new Rect(0, 0, 220, 200));
        }
    }*/
}