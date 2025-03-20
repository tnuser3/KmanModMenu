#region

using GorillaLocomotion;
using HarmonyLib;
using UnityEngine;

#endregion

namespace KmanModMenu.Harmony
{
    [HarmonyPatch(typeof(Player), "LateUpdate", MethodType.Normal)]
    internal class Teleport
    {
        private static bool isTeleporting;
        private static Vector3 position;

        public static void Send(Vector3 vec)
        {
            isTeleporting = true;
            position = vec;
        }

        public static bool Prefix(Player __instance, ref Vector3 ___lastPosition, ref Vector3[] ___velocityHistory,
            ref Vector3 ___lastHeadPosition, ref Vector3 ___lastLeftHandPosition, ref Vector3 ___lastRightHandPosition,
            ref Vector3 ___currentVelocity)
        {
            if (isTeleporting)
            {
                var vector = position - __instance.bodyCollider.transform.position +
                             __instance.transform.position;
                var reg = __instance.GetComponent<Rigidbody>();
                reg.isKinematic = true;
                reg.velocity = Vector3.zero;
                __instance.bodyCollider.attachedRigidbody.velocity = Vector3.zero;
                __instance.bodyCollider.attachedRigidbody.isKinematic = true;
                ___velocityHistory = new Vector3[__instance.velocityHistorySize];
                ___currentVelocity = Vector3.zero;
                ___lastRightHandPosition = vector;
                ___lastLeftHandPosition = vector;
                ___lastHeadPosition = vector;
                __instance.transform.position = vector;
                ___lastPosition = vector;
                __instance.transform.position = position;
                __instance.transform.rotation = Quaternion.identity;
                __instance.bodyCollider.attachedRigidbody.isKinematic = false;
                isTeleporting = false;
            }

            return !isTeleporting;
        }

        [HarmonyPatch(typeof(Player), "AntiTeleportTechnology")]
        private class AntiTeleport
        {
            public static bool Prefix()
            {
                return false;
            }
        }
    }
}