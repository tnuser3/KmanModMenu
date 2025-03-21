using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Harmony
{
    internal class GNot
    {

        [HarmonyPatch(typeof(GorillaNot), "SendReport")]
        private class NoSendReport
        {
            private static void Prefix(ref string susReason, ref string susId, ref string susNick,
                GorillaNot __instance)
            {
                if (__instance) GameObject.Destroy(__instance.gameObject);
                Debug.Log(susNick + " was reported! Reason: " + susReason + " ID: " + susId);
                susNick = "";
                susReason = "";
                susId = "";
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "GracePeriod", MethodType.Enumerator)]
        private class NoGracePeriod
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestsJoin), "LateUpdate")]
        private class NoGracePeriod2
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "GracePeriod", MethodType.Enumerator)]
        private class NoGracePeriod3
        {
            public static bool Prefix()
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GorillaNetworkPublicTestJoin2), "LateUpdate")]
        private class NoGracePeriod4
        {
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}
