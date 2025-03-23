using BepInEx;
using HarmonyLib;
using KmanModMenu.Utilities;
using KmanModMenu.Utilties;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Mods
{
    internal class HoverboardItem
    {
        public static Task GenerateBoard(bool index = false, Vector3 vel = default(Vector3))
        {
            var pool = Traverse.Create(FreeHoverboardManager.instance).Field("freeBoardPool").GetValue() as Stack<FreeHoverboardInstance>;
            if (pool != null && pool.Any())
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = pool.Peek().transform.position;
                typeof(PhotonNetwork)?.GetMethod("RunViewUpdate", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, Array.Empty<object>());

                FreeHoverboardManager.instance.photonView.SendUnlimmitedRPC("GrabBoard_RPC", Photon.Pun.RpcTarget.All, [
                        PhotonNetwork.LocalPlayer.ActorNumber, index, null
                    ]);

                Task.Delay(100).Wait();

                GorillaTagger.Instance.offlineVRRig.enabled = true;
                typeof(PhotonNetwork)?.GetMethod("RunViewUpdate", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, Array.Empty<object>());

                Task.Delay(300).Wait();

                FreeHoverboardManager.instance.photonView.SendUnlimmitedRPC("DropBoard_RPC", Photon.Pun.RpcTarget.All, [
                        index,
                        BitPackUtils.PackWorldPosForNetwork(GorillaTagger.Instance.offlineVRRig.transform.position),
                        BitPackUtils.PackQuaternionForNetwork(Quaternion.identity),
                        BitPackUtils.PackWorldPosForNetwork(vel == default ? Vector3.up : vel),
                        BitPackUtils.PackWorldPosForNetwork(Vector3.up),
                        BitPackUtils.PackColorForNetwork(Color.green),
                    ]);
            }
            return Task.CompletedTask;
        }
        static bool switchs;
        static float timer = 0;
        public static void BoardGun()
        {
            var data = GunLib.Shoot();
            if (data != null)
            {
                if (data.isShooting && data.isTriggered)
                {
                    if (Time.time > timer)
                    {
                        timer = Time.time + 1f;
                        switchs = !switchs;
                        GenerateBoard(switchs, Camera.main.transform.forward * 100);
                    }
                }
            }
        }
    }
}
