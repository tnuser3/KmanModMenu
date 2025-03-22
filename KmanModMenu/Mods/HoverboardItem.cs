using BepInEx;
using HarmonyLib;
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
        public static void GrabBoard()
        {
            var pool = Traverse.Create(FreeHoverboardManager.instance).Field("freeBoardPool").GetValue() as Stack<FreeHoverboardInstance>;
            if (pool != null && pool.Any())
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position = pool.Peek().transform.position;
                typeof(PhotonNetwork)?.GetMethod("RunViewUpdate", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, Array.Empty<object>());
                
                FreeHoverboardManager.instance.photonView.SendUnlimmitedRPC("GrabBoard_RPC", Photon.Pun.RpcTarget.All, [
                        PhotonNetwork.LocalPlayer.ActorNumber, true, null
                    ]);
            }
        }
        public static void DropBoard()
        {
            GorillaTagger.Instance.offlineVRRig.enabled = true;
            typeof(PhotonNetwork)?.GetMethod("RunViewUpdate", BindingFlags.Static | BindingFlags.NonPublic)?.Invoke(null, Array.Empty<object>());

            FreeHoverboardManager.instance.photonView.SendUnlimmitedRPC("DropBoard_RPC", Photon.Pun.RpcTarget.All, [
                    true,
                        BitPackUtils.PackWorldPosForNetwork(GorillaTagger.Instance.offlineVRRig.transform.position),
                        BitPackUtils.PackQuaternionForNetwork(Quaternion.identity),
                        BitPackUtils.PackWorldPosForNetwork(Vector3.up),
                        BitPackUtils.PackWorldPosForNetwork(Vector3.up),
                        BitPackUtils.PackColorForNetwork(Color.green),
                    ]);
        }
    }
}
