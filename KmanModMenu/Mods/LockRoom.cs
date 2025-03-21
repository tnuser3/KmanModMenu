#region

using ExitGames.Client.Photon;
using GorillaNetworking;
using KmanModMenu.Utilties;
using Photon.Pun;
using Photon.Realtime;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class LockRoom
    {
        public static void Execute()
        {
            var archiveCosmetics = CosmeticsController.instance.currentWornSet.ToDisplayNameArray();
            var itjustworks = new[]
            {
                "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.",
                "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU.", "LMAJU."
            };
            CosmeticsController.instance.currentWornSet =
                new CosmeticsController.CosmeticSet(itjustworks, CosmeticsController.instance);
            GorillaTagger.Instance.offlineVRRig.cosmeticSet =
                new CosmeticsController.CosmeticSet(itjustworks, CosmeticsController.instance);
            GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("RPC_UpdateCosmeticsWithTryon",
                RpcTarget.AllBuffered,
                new object[] { itjustworks, CosmeticsController.instance.tryOnSet.ToDisplayNameArray() });
        }

        public static void UnlockRoom()
        {
            var raiseEventOptions = new RaiseEventOptions
            {
                CachingOption = EventCaching.DoNotCache,
                TargetActors = new[]
                {
                    PhotonNetwork.LocalPlayer.ActorNumber
                }
            };
            PhotonNetwork.RaiseEvent(0, null, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}