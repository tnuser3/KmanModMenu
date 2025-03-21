using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Harmony
{
    internal class Misc
    {
        [HarmonyPatch(typeof(GorillaQuitBox), "OnBoxTriggered")]
        private class GorillaQuitBoxPatcher
        {
            private static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GameObject), "CreatePrimitive")]
        internal class GameObjectPatch
        {
            private static void Postfix(GameObject __result)
            {
                var renderer = __result.GetComponent<Renderer>().material;
                renderer.shader = Shader.Find("GorillaTag/UberShader");
                renderer.color = Color.white;
            }
        }
    }
}
