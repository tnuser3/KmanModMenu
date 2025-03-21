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

        public static int TapIndex = 7;

        #endregion

        #region CycleActions

        public static void HandTapIndex(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 11) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    TapIndex = 7;
                    button.Additive = "Grass";
                    break;
                case 1:
                    TapIndex = 0;
                    button.Additive = "Wall";
                    break;
                case 2:
                    TapIndex = 3;
                    button.Additive = "Pillow";
                    break;
                case 3:
                    TapIndex = 8;
                    button.Additive = "Bark";
                    break;
                case 4:
                    TapIndex = 10;
                    button.Additive = "Wood";
                    break;
                case 5:
                    TapIndex = 22;
                    button.Additive = "Crystal";
                    break;
                case 6:
                    TapIndex = 30;
                    button.Additive = "Glass";
                    break;
                case 7:
                    TapIndex = 143;
                    button.Additive = "Rope Creak";
                    break;
                case 8:
                    TapIndex = 72;
                    button.Additive = "Cymbal";
                    break;
                case 9:
                    TapIndex = 85;
                    button.Additive = "Bite";
                    break;
                case 10:
                    TapIndex = 213;
                    button.Additive = "Ambience";
                    break;
            }
        }

        public static void Config_Example(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            // Config logic
        }

        #endregion
    }
}