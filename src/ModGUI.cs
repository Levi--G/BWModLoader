using System;
using System.IO;
using UnityEngine;
namespace ModGUI
{
    public class ModGUI : MonoBehaviour
    {
        void OnGUI()
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(5f, 0f, 200f, 20f), "ModLoader v0.3");
        }
    }
}