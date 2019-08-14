using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BWModLoader
{
    public class Mod
    {

        public string Name => Path.GetFileNameWithoutExtension(FullPath);

        public string FullPath { get; set; }

        public string UniqueFolderPath => SHA1 != null ? Path.Combine(Path.GetDirectoryName(FullPath), SHA1) : null;

        public string UniqueFilePath => UniqueFolderPath != null ? Path.Combine(UniqueFolderPath, Path.GetFileName(FullPath)) : null;

        public string SHA1 { get; set; }

        public List<ModType> Types { get; set; }

        public bool TypesLoaded => Types != null;

        public bool LoadedInGame => TypesLoaded && Types.Any(t => t.LoadedInGame);
        
        public Mod(string fullPath)
        {
            this.FullPath = fullPath;
        }
        
        public IUIElement GetSettingsMenu()
        {
            if (!TypesLoaded) { return null; }
            return (Types.FirstOrDefault(t => typeof(ISettingsMenu).IsAssignableFrom(t.Type))?.Instance as ISettingsMenu)?.GetMenu();
        }

        public class ModType
        {
            public bool LoadedInGame => Instance?.isActiveAndEnabled ?? false;

            public ModType(Type Type)
            {
                this.Type = Type;
            }

            public Type Type { get; set; }
            public MonoBehaviour Instance { get; set; }
        }
    }
}
