#region

using KmanModMenu.Utilities;
using UnityEngine;
using static KmanModMenu.Utilities.Inputs;

#endregion

namespace KmanModMenu.Mods.Player
{
    internal class RigMods
    {
        private static bool ghostToggled;
        public static bool pauseRig;

        public static void Clean()
        {
            if (GorillaTagger.Instance != null && GorillaTagger.Instance.offlineVRRig != null)
                GorillaTagger.Instance.offlineVRRig.enabled = true;
        }

        public static void PauseRig()
        {
            if (pauseRig)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;
                GorillaTagger.Instance.offlineVRRig.transform.position =
                    GorillaLocomotion.Player.Instance.bodyCollider.transform.position + new Vector3(0, 0.2f, 0);
                GorillaTagger.Instance.offlineVRRig.transform.rotation =
                    GorillaLocomotion.Player.Instance.bodyCollider.transform.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }

            if (RightGrip)
            {
                if (!ghostToggled)
                {
                    ghostToggled = true;
                    pauseRig = !pauseRig;
                }
            }
            else
            {
                ghostToggled = false;
            }
        }

        public static void FollowGun()
        {
            var data = GunLib.ShootLocked();
            if (data.isShooting && data.isTriggered && data.isLocked)
                if (data.lockedPlayer)
                    GorillaLocomotion.Player.Instance.transform.position =
                        data.lockedPlayer.transform.position + Vector3.up * 3;
        }

        public static void CopyGun()
        {
            var data = GunLib.ShootLocked();
            if (data.isShooting && data.isTriggered && data.isLocked)
            {
                GorillaTagger.Instance.offlineVRRig.enabled = false;

                GorillaTagger.Instance.offlineVRRig.transform.position = data.lockedPlayer.transform.position;

                GorillaTagger.Instance.offlineVRRig.rightHand.rigTarget.transform.position =
                    data.lockedPlayer.rightHand.rigTarget.transform.position;
                GorillaTagger.Instance.offlineVRRig.leftHand.rigTarget.transform.position =
                    data.lockedPlayer.leftHand.rigTarget.transform.position;

                GorillaTagger.Instance.offlineVRRig.transform.rotation = data.lockedPlayer.transform.rotation;

                GorillaTagger.Instance.offlineVRRig.head.rigTarget.rotation = data.lockedPlayer.head.rigTarget.rotation;
            }
            else
            {
                GorillaTagger.Instance.offlineVRRig.enabled = true;
            }
        }

        public static void Ghost()
        {
            if (RightPrimary)
            {
                if (!ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    ghostToggled = true;
                }
                else
                {
                    if (!ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = true;
                        ghostToggled = true;
                    }
                }
            }
            else
            {
                ghostToggled = false;
            }
        }

        public static void Invis()
        {
            if (RightPrimary)
            {
                if (!ghostToggled && GorillaTagger.Instance.offlineVRRig.enabled)
                {
                    GorillaTagger.Instance.offlineVRRig.enabled = false;
                    GorillaTagger.Instance.offlineVRRig.transform.position = Vector3.zero;
                    ghostToggled = true;
                }
                else
                {
                    if (!ghostToggled && !GorillaTagger.Instance.offlineVRRig.enabled)
                    {
                        GorillaTagger.Instance.offlineVRRig.enabled = true;

                        ghostToggled = true;
                    }
                }
            }
            else
            {
                ghostToggled = false;
            }
        }
    }
}