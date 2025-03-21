#region

using System.Collections.Generic;
using System.Reflection;
using GorillaLocomotion.Gameplay;
using Photon.Pun;
using UnityEngine;
using KmanModMenu.Utilties;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class Rope
    {
        private static Dictionary<int, GorillaRopeSwing> swingers;
        private static float SendDelay;

        public static void FindSwingers()
        {
            var arr = typeof(RopeSwingManager).GetField("ropes", BindingFlags.Instance | BindingFlags.NonPublic);
            if (arr != null)
                swingers = arr.GetValue(RopeSwingManager.instance) as Dictionary<int, GorillaRopeSwing>;
        }

        public static void Send(Vector3 target)
        {
            if (swingers == null)
            {
                FindSwingers();
                return;
            }

            if (SendDelay > Time.time) return;
            SendDelay = Time.time + 0.04f;

            foreach (var sw in swingers)
            {
                if (sw.Value == null) continue;
                RopeSwingManager.instance.photonView.SendUnlimmitedRPC("SetVelocity", RpcTarget.All, new object[]
                {
                    sw.Key, 4, target, true, null
                });
            }
        }

        public static void Up()
        {
            Send(Vector3.up * 1000 + Vector3.right * 3);
        }

        public static void Down()
        {
            Send(-Vector3.up * 1000 + Vector3.right * 3);
        }

        public static void Freeze()
        {
            Send(Vector3.zero);
        }
    }
}