using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Harmony
{
    internal class VRRigPatch
    {
        [HarmonyPatch(typeof(VRRig), "ReturnHandPosition")]
        private class RetHandPosPatcher
        {
            public static readonly bool BlockCall = false;

            private static void Postfix(int __result)
            {
                if (BlockCall)
                    __result = 0;
            }
        }

        [HarmonyPatch(typeof(VRRig), "OnDisable")]
        public class OnDisable : MonoBehaviour
        {
            public static bool Prefix(VRRig __instance)
            {
                return __instance == null ? true : !__instance.isOfflineVRRig;
            }
        }
    }
}
