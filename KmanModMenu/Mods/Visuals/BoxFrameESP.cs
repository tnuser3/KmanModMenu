#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace KmanModMenu.Mods.Visual
{
    internal class BoxFrameESP
    {
        public static Dictionary<VRRig, WFCFing> pairs = new Dictionary<VRRig, WFCFing>();

        public static void Disable()
        {
            foreach (var pair in pairs.ToArray())
                if (pair.Value != null)
                {
                    if (pair.Value.GetComponent<MeshFilter>())
                        pair.Value.GetComponent<MeshFilter>().mesh = null;

                    if (pair.Value.mesh)
                        Object.Destroy(pair.Value.mesh);

                    if (pair.Value.frameParent)
                        GameObject.Destroy(pair.Value.frameParent);

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
                    if (pairs.ContainsKey(rig))
                    {
                        if (!pairs[rig].frameParent) continue;

                        if (pairs[rig].frameParent.transform.parent != rig.transform)
                            pairs[rig].frameParent.transform.SetParent(rig.transform);

                        pairs[rig].wireframeMaterial.color = Color.white;
                    }
                    else
                    {
                        var ESPGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        ESPGo.layer = LayerMask.NameToLayer("Temp Rain In City Please Fix Somebody");
                        var fing = ESPGo.AddComponent<WFCFing>();
                        pairs.Add(rig, fing);
                        fing.frameParent = ESPGo;
                        fing.frameParent.transform.SetParent(rig.transform);
                        fing.frameParent.transform.localPosition = Vector3.zero;
                    }
                }
                else
                {
                    if (pairs.ContainsKey(rig))
                    {
                        pairs[rig].GetComponent<MeshFilter>().mesh = null;
                        Object.Destroy(pairs[rig].mesh);
                        GameObject.Destroy(pairs[rig].frameParent);
                        pairs.Remove(rig);
                    }
                }
        }

        [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
        public class WFCFing : MonoBehaviour
        {
            public Mesh mesh;
            public Material wireframeMaterial;
            public float time;
            public GameObject frameParent;

            private void Start()
            {
                mesh = new Mesh();
                GetComponent<MeshFilter>().mesh = mesh;
                cwfb();

                wireframeMaterial = new Material(Shader.Find("GUI/Text Shader"));
                GetComponent<MeshRenderer>().material = wireframeMaterial;
                Destroy(GetComponent<BoxCollider>());
                transform.localScale = new Vector3(0.35f, 0.705f, 0.35f);
            }

            private void cwfb()
            {
                Vector3[] vertices =
                {
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, 0.5f),
                    new Vector3(0.5f, -0.5f, 0.5f),
                    new Vector3(0.5f, 0.5f, 0.5f),
                    new Vector3(-0.5f, 0.5f, 0.5f)
                };

                int[] lines =
                {
                    0, 1, 1, 2, 2, 3, 3, 0,
                    4, 5, 5, 6, 6, 7, 7, 4,
                    0, 4, 1, 5, 2, 6, 3, 7
                };

                mesh.vertices = vertices;
                mesh.SetIndices(lines, MeshTopology.Lines, 0);
            }
        }
    }
}