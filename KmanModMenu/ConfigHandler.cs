#region

using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion;
using Newtonsoft.Json;
using StealLol.Functions.Mods.Movement;
using StealLol.Functions.Mods.Overpowered;
using StealLol.Utilities.Modules;
using UnityEngine;

#endregion

namespace StealLol.Functions
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

        public static cgStink GetPrefs()
        {
            if (!PlayerPrefs.HasKey("stealstinkta")) return new cgStink();

            return JsonConvert.DeserializeObject<cgStink>(PlayerPrefs.GetString("stealstinkta"));
        }

        public static void SavePrefs(cgStink v)
        {
            var actualPrefs = GetPrefs();
            PlayerPrefs.SetString("stealstinkta", JsonConvert.SerializeObject(new cgStink
            {
                speedboost = v.speedboost == -1 ? actualPrefs.speedboost : v.speedboost,
                playformstype = v.playformstype == -1 ? actualPrefs.playformstype : v.playformstype,
                espcolor = v.espcolor == -1 ? actualPrefs.espcolor : v.espcolor,
                lockLongarms = v.lockLongarms == -1 ? actualPrefs.lockLongarms : v.lockLongarms,
                streamable = v.streamable == -1 ? actualPrefs.streamable : v.streamable,
                handtap = v.handtap == -1 ? actualPrefs.handtap : v.handtap,
                networkMenu = v.networkMenu == -1 ? actualPrefs.networkMenu : v.networkMenu,
                flight = v.flight == -1 ? actualPrefs.flight : v.flight
            }));
        }

        public class cgStink
        {
            public int flight { get; set; }
            public int networkMenu { get; set; }
            public int handtap { get; set; }
            public int speedboost { get; set; }
            public int playformstype { get; set; }
            public int espcolor { get; set; }
            public int lockLongarms { get; set; }
            public int streamable { get; set; }
        }

        #region staticValues

        public static bool DefaultEspColour = true;
        public static Camera[] camsInScene;
        public static int TapIndex = 7;

        #endregion

        #region CycleActions

        public static void FlightSpeed(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 4) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    Flight.multiplier = 12;
                    button.Additive = "1x";
                    break;
                case 1:
                    Flight.multiplier = 18f;
                    button.Additive = "1.5x";
                    break;
                case 2:
                    Flight.multiplier = 26;
                    button.Additive = "2x";
                    break;
                case 3:
                    Flight.multiplier = 38;
                    button.Additive = "3x";
                    break;
            }
        }

        public static void fakeMenu(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    Plugin.FakeMenu = false;
                    button.Additive = "Off";
                    Player.Instance.inOverlay = false;
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                    Player.Instance.bodyCollider.attachedRigidbody.isKinematic = false;
                    break;
                case 1:
                    Plugin.FakeMenu = true;
                    button.Additive = "On";
                    break;
            }
        }

        public static void ChangePieceType(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 9) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    button.Additive = "Terrain";
                    DELETEME.curPiece = DELETEME.PieceTypes.Terrain8x16;
                    break;
                case 1:
                    button.Additive = "Ballista";
                    DELETEME.curPiece = DELETEME.PieceTypes.CastleBallista;
                    break;
                case 2:
                    button.Additive = "Floor";
                    DELETEME.curPiece = DELETEME.PieceTypes.Floor01;
                    break;
                case 3:
                    button.Additive = "Ladder";
                    DELETEME.curPiece = DELETEME.PieceTypes.Ladder01;
                    break;
                case 4:
                    button.Additive = "Arm Shelf";
                    DELETEME.curPiece = DELETEME.PieceTypes.ArmShelf;
                    break;
                case 5:
                    button.Additive = "Tree";
                    DELETEME.curPiece = DELETEME.PieceTypes.TreeMiddle;
                    break;
                case 6:
                    button.Additive = "Terrain Short";
                    DELETEME.curPiece = DELETEME.PieceTypes.Terrain4x8;
                    break;
                case 7:
                    button.Additive = "Terrain Tall";
                    DELETEME.curPiece = DELETEME.PieceTypes.Terrain8x8;
                    break;
                case 8:
                    button.Additive = "Vent";
                    DELETEME.curPiece = DELETEME.PieceTypes.Wind01;
                    break;
            }
        }

        public static void ChangePaperPlaneMult(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 4) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    button.Additive = "1x";
                    PaperPlanesS.Multiplier = 10;
                    break;
                case 1:
                    button.Additive = "2x";
                    PaperPlanesS.Multiplier = 20;
                    break;
                case 2:
                    button.Additive = "5x";
                    PaperPlanesS.Multiplier = 50;
                    break;
                case 3:
                    button.Additive = "10x";
                    PaperPlanesS.Multiplier = 100;
                    break;
            }
        }

        public static void ChangeFirecrackerMult(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 4) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    button.Additive = "1x";
                    FirecrackerMods.multiplier = 10;
                    break;
                case 1:
                    button.Additive = "2x";
                    FirecrackerMods.multiplier = 20;
                    break;
                case 2:
                    button.Additive = "5x";
                    FirecrackerMods.multiplier = 50;
                    break;
                case 3:
                    button.Additive = "10x";
                    FirecrackerMods.multiplier = 100;
                    break;
            }
        }

        public static void ChangeSnowballSize(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;
            if (buttonCfgIndex[button] >= 5) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    button.Additive = "1x";
                    GSnowBall.Size = 1;
                    break;
                case 1:
                    button.Additive = "2x";
                    GSnowBall.Size = 2;
                    break;
                case 2:
                    button.Additive = "4x";
                    GSnowBall.Size = 4;
                    break;
                case 3:
                    button.Additive = "8x";
                    GSnowBall.Size = 8;
                    break;
                case 4:
                    button.Additive = "15x";
                    GSnowBall.Size = 15;
                    break;
            }
        }

        public static void NetworkedMenu(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    NetworkMenu.enabled = true;
                    button.Additive = "On";
                    break;
                case 1:
                    NetworkMenu.enabled = false;
                    button.Additive = "Off";
                    break;
            }
        }

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

        public static void SpeedBoostMultiplier(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 6) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    SpeedBoost.mult = 7.5f;
                    button.Additive = "1.3x";
                    break;
                case 1:
                    SpeedBoost.mult = 8f;
                    button.Additive = "1.45x";
                    break;
                case 2:
                    SpeedBoost.mult = 8.5f;
                    button.Additive = "1.6x";
                    break;
                case 3:
                    SpeedBoost.mult = 9.5f;
                    button.Additive = "1.8x";
                    break;
                case 4:
                    SpeedBoost.mult = 15f;
                    button.Additive = "2x";
                    break;
                case 5:
                    SpeedBoost.mult = 20f;
                    button.Additive = "5x";
                    break;
            }
        }

        public static void ElfMultiplier(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 5) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    DELETEME.multiplier = 2f;
                    button.Additive = "2x";
                    break;
                case 1:
                    DELETEME.multiplier = 2.5f;
                    button.Additive = "2.5x";
                    break;
                case 2:
                    DELETEME.multiplier = 5f;
                    button.Additive = "5x";
                    break;
                case 3:
                    DELETEME.multiplier = 10f;
                    button.Additive = "10x";
                    break;
                case 4:
                    DELETEME.multiplier = 50f;
                    button.Additive = "50x";
                    break;
            }
        }


        public static void PlatformType(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 3) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    Platforms.Platform = Platforms.PlatformType.Normal;
                    button.Additive = "Normal";
                    break;
                case 1:
                    Platforms.Platform = Platforms.PlatformType.Sticky;
                    button.Additive = "Sticky";
                    break;
                case 2:
                    Platforms.Platform = Platforms.PlatformType.Invis;
                    button.Additive = "Invis";
                    break;
            }
        }

        public static void ESPColour(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    DefaultEspColour = true;
                    button.Additive = "Purple";
                    break;
                case 1:
                    DefaultEspColour = false;
                    button.Additive = "RGB";
                    break;
            }
        }

        public static void LockLongArms(Plugin.ConfigButton button)
        {
            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    LongArms.Lock = false;
                    button.Additive = "Off";
                    break;
                case 1:
                    LongArms.Lock = true;
                    button.Additive = "On";
                    break;
            }
        }

        public static void DescreteMode(Plugin.ConfigButton button)
        {
            if (camsInScene == null)
                camsInScene = GameObject.FindObjectsOfType<Camera>().Where(pred => pred != Camera.main).ToArray();

            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            if (Camera.main)
                Camera.main.cullingMask = -1;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    foreach (var c in camsInScene)
                        c.cullingMask = -1;
                    button.Additive = "Off";
                    break;
                case 1:
                    foreach (var c in camsInScene)
                        c.cullingMask &= ~(1 << LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody"));
                    button.Additive = "On";
                    break;
            }
        }


        public static void StreamableESP(Plugin.ConfigButton button)
        {
            if (camsInScene == null)
                camsInScene = GameObject.FindObjectsOfType<Camera>().Where(pred => pred != Camera.main).ToArray();

            buttonCfgIndex[button] += 1;

            if (buttonCfgIndex[button] >= 2) buttonCfgIndex[button] = 0;

            switch (buttonCfgIndex[button])
            {
                case 0:
                    foreach (var c in camsInScene)
                        c.cullingMask &= ~(1 << LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody"));
                    if (Camera.main)
                        Camera.main.cullingMask = -1;
                    button.Additive = "Off";
                    break;
                case 1:
                    foreach (var c in camsInScene)
                        c.cullingMask = -1;
                    if (Camera.main)
                        Camera.main.cullingMask = -1;
                    button.Additive = "On";
                    break;
            }
        }

        #endregion
    }
}