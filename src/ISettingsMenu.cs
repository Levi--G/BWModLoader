using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BWModLoader
{
    /// <summary>
    /// needs to be moved to sdk once available
    /// </summary>
    public interface ISettingsMenu
    {
        IUIElement GetMenu();
    }
    public interface IUIElement
    {
        void Draw();
    }
}
