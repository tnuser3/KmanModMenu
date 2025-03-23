#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace KmanModMenu.Mods.Visual
{
    internal class Tracers
    {
        public static Dictionary<VRRig, LineRenderer> pairs = new Dictionary<VRRig, LineRenderer>();
        public static float time;

        public static void Disable()
        {
            foreach (var pair in pairs.ToArray())
                if (pair.Value != null)
                {
                    GameObject.Destroy(pair.Value);
                    GameObject.Destroy(pair.Value.gameObject);
                    pairs.Remove(pair.Key);
                }
        }

        public static void Execute()
        {
            if (Camera.main)
                Camera.main.cullingMask = -1;
            foreach (var rig in GorillaParent.instance.vrrigs)
                if (rig != null && !rig.isOfflineVRRig)
                {
                    if (pairs.ContainsKey(rig) && pairs[rig])
                    {
                        if (pairs[rig].gameObject.transform.parent != rig.transform)
                            pairs[rig].gameObject.transform.SetParent(rig.transform);

                        pairs[rig].SetPositions(new[]
                        {
                            GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position,
                            rig.transform.position
                        });

                        pairs[rig].startColor = Color.white;
                        pairs[rig].endColor = Color.white;
                    }
                    else
                    {
                        if (pairs.ContainsKey(rig)) pairs.Remove(rig);

                        var TracerGo = new GameObject("lR");
                        TracerGo.layer = LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody");
                        var lr = TracerGo.AddComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.startWidth = 0.012f;
                        lr.endWidth = 0.012f;
                        lr.material = new Material(Shader.Find("GUI/Text Shader"));
                        pairs.Add(rig, lr);
                    }
                }
                else
                {
                    if (pairs.ContainsKey(rig))
                    {
                        GameObject.Destroy(pairs[rig].gameObject);
                        pairs.Remove(rig);
                    }
                }
        }
    }
}