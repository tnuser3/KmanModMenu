using ExitGames.Client.Photon;
using KmanModMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KmanModMenu.Mods
{
    internal class Overpowered
    {
        private static List<float> delays = [];

        public static void Stutter()
        {
            if (Time.time > delays[0])
            {
                ToggleSerilization(false);
                delays[0] = Time.time + 1.1f;
                for (int i = 0; i < 490; i++)
                {
                    Hashtable hashtable = new Hashtable();
                    object[] value = new object[]
                    {
                         PhotonNetwork.ServerTimestamp,
                         76
                    };
                    hashtable.Add(i, value);
                    PhotonNetwork.NetworkingClient.OpRaiseEvent(210, hashtable, new RaiseEventOptions
                    {
                        Receivers = 0
                    }, SendOptions.SendUnreliable);
                }
                PhotonNetwork.CleanRpcBufferIfMine(GorillaTagger.Instance.myVRRig.GetView);
                PhotonNetwork.LocalCleanPhotonView(GorillaTagger.Instance.myVRRig.GetView);
            }
            ToggleSerilization(true);
        }

        public static void StutterGun()
        {
            var data = GunLib.ShootLocked();
            if (data != null && data.lockedPlayer)
            {
                if (Time.time > delays[0])
                {
                    delays[0] = Time.time + 1.1f;
                    ToggleSerilization(false);
                    for (int i = 0; i < 490; i++)
                    {
                        Hashtable hashtable = new Hashtable();
                        object[] value = new object[]
                        {
                         PhotonNetwork.ServerTimestamp,
                         76
                        };
                        hashtable.Add(i, value);
                        PhotonNetwork.NetworkingClient.OpRaiseEvent(210, hashtable, new RaiseEventOptions
                        {
                            TargetActors = new int[]
                            {
                                data.lockedPlayer.Creator.GetPlayerRef().ActorNumber
                            }
                        }, SendOptions.SendUnreliable);
                    }
                    PhotonNetwork.CleanRpcBufferIfMine(GorillaTagger.Instance.myVRRig.GetView);
                    PhotonNetwork.LocalCleanPhotonView(GorillaTagger.Instance.myVRRig.GetView);
                }
                ToggleSerilization(true);
            }
            else
            {
                ToggleSerilization(true);
            }
        }

        public static void ToggleSerilization(bool t)
        {
            foreach (var pv in PhotonNetwork.PhotonViewCollection)
            {
                if (!pv.IsMine) continue;
                pv.Synchronization = t ? ViewSynchronization.Unreliable : ViewSynchronization.Off;
            }
        }

        public static void GiveStutterMods()
        {
            var gun = GunLib.ShootLocked();
            if (gun.isLocked)
                foreach (var crrig in GorillaParent.instance.vrrigs)
                {
                    if (crrig.isOfflineVRRig || crrig == gun.lockedPlayer) continue;

                    if (Vector3.Distance(gun.lockedPlayer.leftHand.rigTarget.position, crrig.head.rigTarget.position) <
                        0.77f
                        || Vector3.Distance(gun.lockedPlayer.rightHand.rigTarget.position,
                            crrig.head.rigTarget.position) < 0.77f)
                        if (delays[0] < Time.time)
                        {
                            delays[0] = Time.time + 1f;
                            ToggleSerilization(false);
                            for (int i = 0; i < 495; i++)
                            {
                                Hashtable hashtable = new Hashtable();
                                object[] value = new object[]
                                {
                                     PhotonNetwork.ServerTimestamp,
                                     76
                                };
                                hashtable.Add(i, value);
                                PhotonNetwork.NetworkingClient.OpRaiseEvent(210, hashtable, new RaiseEventOptions
                                {
                                    TargetActors = new int[]
                                    {
                                        crrig.Creator.GetPlayerRef().ActorNumber
                                    }
                                }, SendOptions.SendUnreliable);
                            }
                            PhotonNetwork.CleanRpcBufferIfMine(GorillaTagger.Instance.myVRRig.GetView);
                            PhotonNetwork.LocalCleanPhotonView(GorillaTagger.Instance.myVRRig.GetView);
                        }
                }
        }

    }
}
