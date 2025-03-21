#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace KmanModMenu.Mods.Visual
{
    internal class BoxESP
    {
        public static float time;
        private static readonly Dictionary<VRRig, GameObject> box = new Dictionary<VRRig, GameObject>();

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
                    box[rig].transform.position = rig.transform.position;

                    var rend = box[rig].GetComponent<Renderer>();
                    if (rend)
                    {
                        rend.material = new Material(Shader.Find("GUI/Text Shader"));
                        rend.material.color = Color.white;
                    }
                }
                else
                {
                    var newBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    newBox.layer = LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody");
                    newBox.transform.localScale = new Vector3(0.4f, 1.6f, 0.4f);
                    box.Add(rig, newBox);
                    var rend = newBox.GetComponent<Renderer>();
                    newBox.GetComponent<Collider>().enabled = false;
                    if (rend)
                        rend.material = new Material(Shader.Find("GUI/Text Shader"));
                }
            }
        }
    }
}