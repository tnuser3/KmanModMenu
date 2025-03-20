#region

using System.Collections.Generic;

#endregion

namespace KmanModMenu
{
    internal class ConfigHandler
    {
        public static Dictionary<Plugin.ConfigButton, int> buttonCfgIndex = new Dictionary<Plugin.ConfigButton, int>();

        public static void Cycle(Plugin.ConfigButton button)
        {
            if (button == null || button.CycleAction == null) return;

            if (!buttonCfgIndex.ContainsKey(button)) buttonCfgIndex.Add(button, 0);

            button.CycleAction(button);
        }

        #region Variables

        #endregion

        #region CycleActions

        public static void Config_Example(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            // Config logic
        }

        #endregion
    }
}