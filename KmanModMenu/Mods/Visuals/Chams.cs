#region

using System.Linq;
using UnityEngine;

#endregion

namespace KmanModMenu.Mods.Visual
{
    internal class Chams
    {
        public static void Disable()
        {
            foreach (var rig in GorillaParent.instance.vrrigs)
            {
                if (rig == null || rig.isOfflineVRRig) continue;

                rig.mainSkin.material.color = rig.playerColor;
                rig.mainSkin.material.shader = Shader.Find("GorillaTag/UberShader");
                rig.ChangeMaterialLocal(rig.currentMatIndex);
            }
        }

        public static void Execute()
        {
            if (Camera.main)
                Camera.main.cullingMask = -1;
            foreach (var rig in GorillaParent.instance.vrrigs)
                if (!rig.isOfflineVRRig)
                {
                    var fC = new Color(rig.mainSkin.material.color.r, rig.mainSkin.material.color.g,
                        rig.mainSkin.material.color.b, 0.74f);
                    rig.mainSkin.material.shader = Shader.Find("GUI/Text Shader");
                    if (GorillaGameManager.instance is GorillaTagManager)
                    {
                        if (rig.mainSkin.material.name.Contains("fected"))
                            fC = new Color32(255, 111, 0, 190);
                        else
                            fC = new Color32(0, 255, 68, 190);
                    }
                    else if (GorillaGameManager.instance is GorillaHuntManager)
                    {
                        if (rig.setMatIndex == 3)
                            fC = new Color32(0, 145, 255, 190);
                        else
                            fC = new Color32(0, 255, 68, 190);
                    }
                    else if (GorillaGameManager.instance is GorillaPaintbrawlManager manager)
                    {
                        var status = manager.GetPlayerStatus(rig.Creator);
                        if ((status & GorillaPaintbrawlManager.PaintbrawlStatus.RedTeam) >
                            GorillaPaintbrawlManager.PaintbrawlStatus.None)
                            fC = new Color32(255, 111, 0, 190);
                        else
                            fC = new Color32(0, 145, 255, 190);
                    }

                    if (fC != rig.mainSkin.material.color)
                    {
                        var c32array = Enumerable.Repeat((Color32)fC, rig.mainSkin.sharedMesh.colors32.Length)
                            .ToArray();
                        var carray = Enumerable.Repeat(fC, rig.mainSkin.sharedMesh.colors32.Length).ToArray();
                        rig.mainSkin.sharedMesh.colors32 = c32array;
                        rig.mainSkin.sharedMesh.colors = carray;
                        rig.mainSkin.material.color = fC;
                    }
                }
        }
    }
}