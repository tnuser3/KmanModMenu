#region

using System;
using System.Reflection;
using BepInEx;
using ExitGames.Client.Photon;
using KmanModMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static KmanModMenu.Utilities.Inputs;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class Projectiles
    {
        private static float ProjTimeout;

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right) TrueLeftHand()
        {
            var rot = GorillaTagger.Instance.leftHandTransform.rotation *
                      GorillaLocomotion.Player.Instance.leftHandRotOffset;
            return (
                GorillaTagger.Instance.leftHandTransform.position + GorillaTagger.Instance.leftHandTransform.rotation *
                GorillaLocomotion.Player.Instance.leftHandOffset, rot, rot * Vector3.up, rot * Vector3.forward,
                rot * Vector3.right);
        }

        public static (Vector3 position, Quaternion rotation, Vector3 up, Vector3 forward, Vector3 right)
            TrueRightHand()
        {
            var rot = GorillaTagger.Instance.rightHandTransform.rotation *
                      GorillaLocomotion.Player.Instance.rightHandRotOffset;
            return (
                GorillaTagger.Instance.rightHandTransform.position +
                GorillaTagger.Instance.rightHandTransform.rotation * GorillaLocomotion.Player.Instance.rightHandOffset,
                rot, rot * Vector3.up, rot * Vector3.forward, rot * Vector3.right);
        }

        private static void SendProj(Vector3 Origin, Vector3 velocity, int projIndex = 0)
        {
            if (PhotonNetwork.CurrentRoom == null || ProjTimeout > Time.time) return;
            ProjTimeout = Time.time + 0.2f;

            var projectileTrackerIndex = 0;
            var tRef = typeof(VRRig).Assembly.GetType("ProjectileTracker");

            if (tRef == null) return;

            var mRef = tRef.GetMethod("IncrementLocalPlayerProjectileCount",
                BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            if (mRef != null)
                projectileTrackerIndex = (int)mRef.Invoke(null, Array.Empty<object>());

            GorillaTagger.Instance.offlineVRRig.RightThrowableProjectileIndex = projIndex;

            PhotonNetwork.RaiseEvent(3, new object[]
            {
                PhotonNetwork.ServerTimestamp,
                0,
                new object[]
                {
                    Origin,
                    velocity,
                    2,
                    projectileTrackerIndex,
                    false,
                    (byte)255,
                    (byte)255,
                    (byte)255,
                    (byte)255
                }
            }, new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All
            }, new SendOptions
            {
                Reliability = true
            });
        }

        public static void Execute()
        {
            if ((!RightGrip && !UnityInput.Current.GetMouseButton(1)) || PhotonNetwork.CurrentRoom == null) return;

            SendProj(GorillaTagger.Instance.offlineVRRig.rightHandTransform.position, TrueRightHand().forward * 10);
        }

        public static void GiveProjectileMods()
        {
            var data = GunLib.ShootLocked();
            if (data != null && data.lockedPlayer)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position =
                    data.lockedPlayer.transform.position - Vector3.one;

                if (data.lockedPlayer.leftMiddle.calcT > 0.25f)
                    SendProj(data.lockedPlayer.leftHand.rigTarget.position,
                        data.lockedPlayer.leftHand.rigTarget.up * 10f);

                if (data.lockedPlayer.rightMiddle.calcT > 0.25f)
                    SendProj(data.lockedPlayer.rightHand.rigTarget.position,
                        data.lockedPlayer.rightHand.rigTarget.up * 10f);
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }
    }
}