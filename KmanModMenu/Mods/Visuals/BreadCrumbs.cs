#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace KmanModMenu.Mods.Visual
{
    internal class BreadCrumbs
    {
        public static float time;
        private static readonly Dictionary<VRRig, TrailRenderer> box = new Dictionary<VRRig, TrailRenderer>();

        public static void Disable()
        {
            foreach (var pair in box.ToArray())
                if (pair.Value != null)
                {
                    GameObject.Destroy(pair.Value);
                    box.Remove(pair.Key);
                }
        }

        public static void Execute()
        {
            if (Camera.main)
                Camera.main.cullingMask = -1;
            foreach (var rig in GorillaParent.instance.vrrigs)
            {
                if (rig == null || rig.isOfflineVRRig) continue;

                if (box.ContainsKey(rig))
                {
                    var rend = box[rig];
                    if (rend)
                    {
                        rend.transform.position = rig.transform.position;
                        rend.material = new Material(Shader.Find("GUI/Text Shader"));

                        rend.startColor = Color.white;
                        rend.endColor = Color.white;
                    }
                }
                else
                {
                    var newBox = new GameObject();
                    newBox.layer = LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody");
                    newBox.transform.position = rig.transform.position;
                    var tr = newBox.AddComponent<TrailRenderer>();
                    tr.material = new Material(Shader.Find("GUI/Text Shader"));
                    tr.endWidth = 0.05f;
                    tr.startWidth = 0.1f;
                    tr.time = 1;
                    box.Add(rig, tr);
                }
            }
        }
    }
}