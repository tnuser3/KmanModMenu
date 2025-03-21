#region

using System.Collections.Generic;
using BepInEx;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using KmanModMenu.Utilties;
using static KmanModMenu.Utilities.Inputs;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class Spammer
    {
        private static bool canShoot;
        private static float shootDelay;
        public static List<float> regDelays = new List<float>();

        public static string[] getRandomSet()
        {
            return new[]
            {
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,

                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,

                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,

                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName,
                CosmeticsController.instance
                    .allCosmetics[Random.Range(1, CosmeticsController.instance.allCosmetics.Count)].itemName
            };
        }


        public static void ExecuteDoor()
        {
            var go = GameObject.Find(
                "Environment Objects/LocalObjects_Prefab/CityToBasement/DungeonEntrance/DungeonDoor_Prefab");
            if (go)
            {
                if (PhotonNetwork.CurrentRoom == null)
                {
                    var gtDoor = go.GetComponent<GTDoor>();
                    if (gtDoor)
                    {
                        gtDoor.ChangeDoorState(GTDoor.DoorState.HeldOpen);
                        gtDoor.ChangeDoorState(GTDoor.DoorState.Closing);
                    }
                }
                else
                {
                    var pv = go.GetComponent<PhotonView>();
                    if (pv)
                    {
                        pv.RPC("ChangeDoorState", RpcTarget.All, GTDoor.DoorState.HeldOpen);
                        pv.RPC("ChangeDoorState", RpcTarget.All, GTDoor.DoorState.Closing);
                    }
                }
            }
        }

        public static void ExecuteBracelet()
        {
            if (RightTrigger || UnityInput.Current.GetKeyDown(KeyCode.B))
            {
                if (Time.time > shootDelay)
                {
                    canShoot = !canShoot;
                    shootDelay = 0f;
                }

                if (canShoot)
                {
                    if (PhotonNetwork.CurrentRoom == null)
                        GorillaTagger.Instance.offlineVRRig.nonCosmeticLeftHandItem.EnableItem(true);
                    else
                        GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("EnableNonCosmeticHandItemRPC",
                            RpcTarget.All, new object[] { true, false });
                }
                else
                {
                    if (PhotonNetwork.CurrentRoom == null)
                        GorillaTagger.Instance.offlineVRRig.nonCosmeticLeftHandItem.EnableItem(false);
                    else
                        GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("EnableNonCosmeticHandItemRPC",
                            RpcTarget.All, new object[] { false, false });
                }
            }
        }

        public static void ExecuteHandTap()
        {
            if ((RightTrigger || UnityInput.Current.GetKey(KeyCode.H)) && regDelays[0] < Time.time)
            {
                regDelays[0] = Time.time + 0.05f;
                if (PhotonNetwork.CurrentRoom == null)
                    GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(ConfigHandler.TapIndex, false, 9999f);
                else
                    GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("RPC_PlayHandTap", RpcTarget.All,
                        new object[] { ConfigHandler.TapIndex, false, 999999f });
            }
        }

        public static void ExecuteCosmetic()
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                GorillaTagger.Instance.offlineVRRig.cosmeticSet =
                    new CosmeticsController.CosmeticSet(getRandomSet(), CosmeticsController.instance);
                GorillaTagger.Instance.offlineVRRig.tryOnSet =
                    new CosmeticsController.CosmeticSet(getRandomSet(), CosmeticsController.instance);
                GorillaTagger.Instance.offlineVRRig.prevSet.CopyItems(GorillaTagger.Instance.offlineVRRig.mergedSet);
                GorillaTagger.Instance.offlineVRRig.mergedSet.MergeSets(GorillaTagger.Instance.offlineVRRig.tryOnSet,
                    GorillaTagger.Instance.offlineVRRig.cosmeticSet);
                var component = GorillaTagger.Instance.offlineVRRig.GetComponent<BodyDockPositions>();
            }
            else
            {
                if (RightTrigger)
                {
                    if (GorillaTagger.Instance.offlineVRRig.inTryOnRoom == false)
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = false;
                        GorillaTagger.Instance.offlineVRRig.transform.position =
                            new Vector3(-49.4993f, 18.2303f, -117.3594f);
                    }

                    GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("RPC_UpdateCosmeticsWithTryon",
                        RpcTarget.All, new object[] { getRandomSet(), getRandomSet() });
                }
                else
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = true;
                }
            }
        }
    }
}