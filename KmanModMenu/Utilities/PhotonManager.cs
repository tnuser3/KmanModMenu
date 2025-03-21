#region

using System.Reflection;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

#endregion

namespace KmanModMenu.Utilties
{
    public static class PhotonManager
    {
        public static bool SendUnlimmitedRPC(this PhotonView photonView, string method, Player player,
            object[] parameters)
        {
            return SendUnlimmitedRPC(photonView, method, RpcTarget.AllBuffered, player, parameters);
        }

        public static bool SendUnlimmitedRPC(this PhotonView photonView, string method, RpcTarget player,
            object[] parameters)
        {
            return SendUnlimmitedRPC(photonView, method, player, null, parameters);
        }

        private static bool SendUnlimmitedRPC(PhotonView photonView, string method, RpcTarget target, Player player,
            object[] parameters)
        {
            if (photonView != null && parameters != null && !string.IsNullOrEmpty(method))
            {
                var rpcHash = new Hashtable
                {
                    { 0, photonView.ViewID },
                    { 2, PhotonNetwork.ServerTimestamp + -int.MaxValue },
                    { 3, method },
                    { 4, parameters }
                };

                if (photonView.Prefix > 0) rpcHash[1] = (short)photonView.Prefix;
                if (PhotonNetwork.PhotonServerSettings.RpcList.Contains(method))
                    rpcHash[5] = (byte)PhotonNetwork.PhotonServerSettings.RpcList.IndexOf(method);
                if (player == null)
                {
                    switch (target)
                    {
                        case RpcTarget.All:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.All,
                                    InterestGroup = photonView.Group
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            typeof(PhotonNetwork).GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic)
                                .Invoke(typeof(PhotonNetwork), new object[]
                                {
                                    rpcHash, PhotonNetwork.LocalPlayer
                                });
                            break;

                        case RpcTarget.Others:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.Others,
                                    InterestGroup = photonView.Group
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            break;

                        case RpcTarget.AllBuffered:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.All,
                                    InterestGroup = photonView.Group,
                                    CachingOption = EventCaching.AddToRoomCache
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            typeof(PhotonNetwork).GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic)
                                .Invoke(typeof(PhotonNetwork), new object[]
                                {
                                    rpcHash, PhotonNetwork.LocalPlayer
                                });
                            break;

                        case RpcTarget.OthersBuffered:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.Others,
                                    InterestGroup = photonView.Group,
                                    CachingOption = EventCaching.AddToRoomCache
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            break;

                        case RpcTarget.AllBufferedViaServer:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.All,
                                    InterestGroup = photonView.Group
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            if (PhotonNetwork.OfflineMode)
                                typeof(PhotonNetwork)
                                    .GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic).Invoke(
                                        typeof(PhotonNetwork), new object[]
                                        {
                                            rpcHash, PhotonNetwork.LocalPlayer
                                        });
                            break;

                        case RpcTarget.AllViaServer:
                            return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                                new RaiseEventOptions
                                {
                                    Receivers = ReceiverGroup.All,
                                    InterestGroup = photonView.Group,
                                    CachingOption = EventCaching.AddToRoomCache
                                }, new SendOptions
                                {
                                    Reliability = true,
                                    DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                    Encrypt = false
                                });
                            if (PhotonNetwork.OfflineMode)
                                typeof(PhotonNetwork)
                                    .GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic).Invoke(
                                        typeof(PhotonNetwork), new object[]
                                        {
                                            rpcHash, PhotonNetwork.LocalPlayer
                                        });
                            break;
                    }
                }
                else
                {
                    if (PhotonNetwork.NetworkingClient.LocalPlayer.ActorNumber == player.ActorNumber)
                        typeof(PhotonNetwork).GetMethod("ExecuteRpc", BindingFlags.Static | BindingFlags.NonPublic)
                            .Invoke(typeof(PhotonNetwork), new object[]
                            {
                                rpcHash, PhotonNetwork.LocalPlayer
                            });
                    else
                        return PhotonNetwork.NetworkingClient.LoadBalancingPeer.OpRaiseEvent(200, rpcHash,
                            new RaiseEventOptions
                            {
                                TargetActors = new[]
                                {
                                    player.ActorNumber
                                }
                            }, new SendOptions
                            {
                                Reliability = true,
                                DeliveryMode = DeliveryMode.ReliableUnsequenced,
                                Encrypt = false
                            });
                }
            }

            return false;
        }
    }
}