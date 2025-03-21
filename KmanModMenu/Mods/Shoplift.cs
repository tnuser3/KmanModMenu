#region

using GorillaNetworking;
using KmanModMenu.Utilties;
using Photon.Pun;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class Shoplift
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
            GorillaTagger.Instance.myVRRig.GetView.SendUnlimmitedRPC("RPC_UpdateCosmeticsWithTryon", RpcTarget.All,
                new object[] { itjustworks, CosmeticsController.instance.tryOnSet.ToDisplayNameArray() });
        }
    }
}