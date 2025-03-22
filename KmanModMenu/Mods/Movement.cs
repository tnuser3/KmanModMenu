using ExitGames.Client.Photon;
using GorillaLocomotion.Swimming;
using KmanModMenu.Harmony;
using KmanModMenu.Utilities;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using UnityEngine;
using static KmanModMenu.Utilities.Inputs;

namespace KmanModMenu.Mods
{
    /// <summary>
    /// Movement Class, all movment mods are stored here
    /// </summary>
    internal class Movement// for a more clear view of this class do ctrl+m then ctrl+o
    {
        #region Variables

        private static bool AntiRepeatTeleport;
        private static VRRig teleportRig;
        private static float yOffset = 1.2f;
        private static float time;
        public static float mult = 7.5f;
        private static GameObject checkPoint;
        private static WaterVolume[] wv;
        public static float flySpeed = 12;
        private static bool gravityToggled;
        private static float LongArmsOffset;
        private static bool isnoclipped;
        private static MeshCollider[] array;

        #endregion

        #region Functions

        public static void Flight()
        {
            try
            {
                if (RightPrimary)
                {
                    GorillaLocomotion.GTPlayer.Instance.transform.position +=
                        GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.forward * Time.deltaTime *
                        flySpeed;
                    GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                }

                if (RightSecondary)
                {
                    if (!gravityToggled && Physics.gravity != Vector3.zero)
                    {
                        Physics.gravity = Vector3.zero;
                        gravityToggled = true;
                    }
                    else if (!gravityToggled && Physics.gravity == Vector3.zero)
                    {
                        Physics.gravity = Vector3.up * -9.81f;
                        gravityToggled = true;
                    }
                }
                else
                {
                    gravityToggled = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToJson());
            }
        }

        public static void FastSpin()
        {
            if (RightTrigger && LeftTrigger)
                GorillaLocomotion.GTPlayer.Instance.Turn(-30f);
        }

        public static void FastSwim()
        {
            if (GorillaLocomotion.GTPlayer.Instance.InWater)
                GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity =
                    GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody.velocity * 1.01f;
        }

        public static void BHop()
        {
            if (GorillaLocomotion.GTPlayer.Instance.IsHandTouching(false) && RightGrip)
            {
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>()
                    .AddForce(Vector3.up * 200f, ForceMode.Impulse);
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().AddForce(
                    GorillaTagger.Instance.offlineVRRig.rightHandPlayer.transform.right * 330f, ForceMode.Impulse);
            }

            if (GorillaLocomotion.GTPlayer.Instance.IsHandTouching(true) && LeftGrip)
            {
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().velocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>()
                    .AddForce(Vector3.up * 200f, ForceMode.Impulse);
                GorillaLocomotion.GTPlayer.Instance.GetComponent<Rigidbody>().AddForce(
                    -GorillaTagger.Instance.offlineVRRig.leftHandPlayer.transform.right * 330f, ForceMode.Impulse);
            }
        }

        public static void Checkpoint()
        {
            if (RightGrip)
            {
                if (checkPoint == null)
                {
                    checkPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    checkPoint.transform.localScale = Vector3.one / 10;
                    var renderer = checkPoint.GetComponent<Renderer>();
                    var collider = checkPoint.GetComponent<Collider>();

                    if (renderer != null) renderer.material.color = Color.red;
                    if (collider != null) collider.enabled = false;
                }
                else
                {
                    var renderer = checkPoint.GetComponent<Renderer>();

                    if (renderer != null) renderer.material.color = Color.red;
                    checkPoint.transform.position = GorillaLocomotion.GTPlayer.Instance.rightHandFollower.transform.position;
                }
            }
            else if (RightTrigger)
            {
                var renderer = checkPoint.GetComponent<Renderer>();

                if (renderer != null) renderer.material.color = Color.blue;
                Teleport.Send(checkPoint.transform.position + Vector3.up * 1.2f);
            }
            else if (checkPoint != null)
            {
                var renderer = checkPoint.GetComponent<Renderer>();

                if (renderer != null) renderer.material.color = Color.red;
            }
        }

        public static void CleanCheckpoint()
        {
            UnityEngine.Object.Destroy(checkPoint);
            checkPoint = null;
        }

        public static void TeleportGun()
        {
            if (teleportRig == null)
            {
                teleportRig = GameObject.Instantiate(
                    GorillaTagger.Instance.offlineVRRig,
                    GorillaLocomotion.GTPlayer.Instance.transform.position,
                    GorillaLocomotion.GTPlayer.Instance.transform.rotation
                );
                teleportRig.enabled = false;
                teleportRig.transform.position = Vector3.zero;

                teleportRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                teleportRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
            }

            var data = GunLib.Shoot();
            if (data == null) return;

            if (data.isShooting)
            {
                if (data.isTriggered && !AntiRepeatTeleport)
                {
                    Teleport.Send(data.hitPosition);
                    AntiRepeatTeleport = true;
                }
                else if (!data.isTriggered)
                {
                    AntiRepeatTeleport = false;
                }

                time += Time.deltaTime;
                yOffset = Mathf.SmoothStep(1, 1.2f, Mathf.PingPong(time, 1f));
                teleportRig.transform.SetPositionAndRotation(data.hitPosition + Vector3.up * yOffset, GorillaTagger.Instance.bodyCollider.transform.rotation);

                var offlineRig = GorillaTagger.Instance.offlineVRRig;
                teleportRig.leftHand.rigTarget.SetPositionAndRotation(offlineRig.leftHand.rigTarget.localPosition, offlineRig.leftHand.rigTarget.rotation);
                teleportRig.rightHand.rigTarget.SetPositionAndRotation(offlineRig.rightHand.rigTarget.localPosition, offlineRig.rightHand.rigTarget.rotation);
                teleportRig.head.rigTarget.SetPositionAndRotation(offlineRig.head.rigTarget.position, offlineRig.head.rigTarget.rotation);

                var material = teleportRig.mainSkin.material;
                if (material == null || material.shader.name != "UI/Default")
                {
                    material = new Material(Shader.Find("UI/Default"));
                    teleportRig.mainSkin.material = material;
                }

                var fc = new Color32(41, 194, 255, 120);
                if (material.color != fc)
                {
                    material.color = fc;
                    var c32array = Enumerable.Repeat(fc, teleportRig.mainSkin.sharedMesh.colors32.Length).ToArray();
                    var carray = Enumerable.Repeat((Color)fc, teleportRig.mainSkin.sharedMesh.colors.Length).ToArray();
                    teleportRig.mainSkin.sharedMesh.colors32 = c32array;
                    teleportRig.mainSkin.sharedMesh.colors = carray;
                }
            }
            else
            {
                AntiRepeatTeleport = false;
                teleportRig.transform.position = Vector3.zero;
                teleportRig.mainSkin.material = null;
            }
        }

        public static void IronMonkey()
        {
            var RB = GorillaLocomotion.GTPlayer.Instance.bodyCollider.attachedRigidbody;
            var rightController = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform;
            var leftController = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform;

            void ApplyForceAndTrail(Transform controller, bool isRight, bool isGrip)
            {
                if (!isGrip)
                {
                    var trail = controller.gameObject.GetComponent<TrailRenderer>();
                    if (trail) GameObject.Destroy(trail);
                    return;
                }

                RB.AddForce((isRight ? 20f : -20f) * controller.right, ForceMode.Acceleration);

                var trailRenderer = controller.gameObject.GetComponent<TrailRenderer>();
                if (!trailRenderer)
                    trailRenderer = controller.gameObject.AddComponent<TrailRenderer>();

                trailRenderer.material = new Material(Shader.Find("GUI/Text Shader"));
                trailRenderer.endWidth = 0.3f;
                trailRenderer.startWidth = 0.01f;
                trailRenderer.startColor = new Color32(255, 128, 0, 255);
                trailRenderer.endColor = new Color32(89, 45, 0, 0);
                trailRenderer.time = 2;

                GorillaTagger.Instance.StartVibration(!isRight,
                    GorillaTagger.Instance.tapHapticStrength / 50f * RB.velocity.magnitude,
                    GorillaTagger.Instance.tapHapticDuration);
            }

            ApplyForceAndTrail(rightController, true, RightGrip);
            ApplyForceAndTrail(leftController, false, LeftGrip);

            if (LeftGrip || RightGrip) RB.velocity = Vector3.ClampMagnitude(RB.velocity, 50f);
        }

        public static void WaterWalk()
        {
            if (wv == null) wv = GameObject.FindObjectsOfType<WaterVolume>();
            foreach (var w in wv)
            {
                var bc = w.gameObject.GetComponent<BoxCollider>();
                if (bc && bc.isTrigger)
                {
                    w.gameObject.layer = LayerMask.NameToLayer("Gorilla Object");
                    w.gameObject.AddComponent<GorillaSurfaceOverride>();
                    bc.isTrigger = false;
                }
            }
        }

        public static void CleanWaterWalk()
        {
            wv = GameObject.FindObjectsOfType<WaterVolume>();

            foreach (var w in wv)
            {
                var bc = w.gameObject.GetComponent<BoxCollider>();
                w.gameObject.layer = LayerMask.NameToLayer("Water");
                GameObject.Destroy(w.gameObject.GetComponent<GorillaSurfaceOverride>());
                if (bc && !bc.isTrigger)
                    bc.isTrigger = true;
            }
        }

        public static void LongArmsClean()
        {
            GorillaLocomotion.GTPlayer.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
            GorillaLocomotion.GTPlayer.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
            LongArmsOffset = 0;
        }

        public static void LongArms()
        {
            if (LeftTrigger) LongArmsOffset += 0.05f;

            if (RightTrigger) LongArmsOffset -= 0.05f;

            if (LeftPrimary)
            {
                GorillaLocomotion.GTPlayer.Instance.leftHandOffset = new Vector3(-0.02f, 0f, -0.07f);
                GorillaLocomotion.GTPlayer.Instance.rightHandOffset = new Vector3(0.02f, 0f, -0.07f);
                LongArmsOffset = 0;
                return;
            }

            GorillaLocomotion.GTPlayer.Instance.rightHandOffset = new Vector3(-0.02f, LongArmsOffset, -0.07f);
            GorillaLocomotion.GTPlayer.Instance.leftHandOffset = new Vector3(-0.02f, LongArmsOffset, -0.07f);
        }

        public static void LowGrav()
        {
            if (Physics.gravity.y != -2.81f)
                Physics.gravity = new Vector3(0, -2.81f, 0);
        }

        public static void FixGrav()
        {
            Physics.gravity = new Vector3(0, -9.81f, 0);
        }

        public static void DisableNoClip()
        {
            if (isnoclipped)
            {
                if (array != null)
                    foreach (var collider in array)
                        collider.enabled = true;

                isnoclipped = false;
            }
        }

        public static void NoClip()
        {
            if (array == null)
                array = Resources.FindObjectsOfTypeAll<MeshCollider>().Where(red => red.enabled).ToArray();

            if (LeftTrigger)
            {
                if (!isnoclipped)
                {
                    if (array != null)
                        foreach (var collider in array)
                            collider.enabled = false;

                    isnoclipped = true;
                }
            }
            else
            {
                if (isnoclipped)
                {
                    if (array != null)
                        foreach (var collider in array)
                            collider.enabled = true;

                    isnoclipped = false;
                }
            }
        }

        public static void CleanSP()
        {
            var ins = GorillaLocomotion.GTPlayer.Instance;
            if (GorillaGameManager.instance != null)
                ins.maxJumpSpeed = GorillaGameManager.instance.LocalPlayerSpeed()[0];
            else
                ins.maxJumpSpeed = 6.5f;
        }

        public static void SpeedBoost()
        {
            GorillaLocomotion.GTPlayer.Instance.maxJumpSpeed = mult;
        }

        #endregion

        #region Nested

        internal class Platforms
        {
            public enum PlatformType { Normal, Sticky, Invis }

            public static PlatformType Platform = PlatformType.Normal;
            private static GameObject RightPlat, LeftPlat;
            public static Color PlatColor = Color.magenta;

            private static readonly Dictionary<int, GameObject> LeftPlat_Networked = [];
            private static readonly Dictionary<int, GameObject> RightPlat_Networked = [];
            private static bool hasSubscribed;

            private static void StartNetworkedJump()
            {
                if (PhotonNetwork.NetworkingClient.IsConnected && !hasSubscribed)
                {
                    hasSubscribed = true;
                    PhotonNetwork.NetworkingClient.EventReceived += NetworkJump;
                }
            }

            private static void NetworkJump(EventData ev)
            {
                if (!Enum.IsDefined(typeof(EventCode), ev.Code)) return;

                switch ((EventCode)ev.Code)
                {
                    case EventCode.RightSpawned:
                    case EventCode.LeftSpawned:
                        HandlePlatformSpawn(ev);
                        break;
                    case EventCode.RightDespawned:
                    case EventCode.LeftDespawned:
                        HandlePlatformDespawn(ev);
                        break;
                }
            }

            private static void HandlePlatformSpawn(EventData ev)
            {
                if (ev.CustomData is not object[] objArr || objArr.Length < 3) return;

                var pos = (Vector3?)objArr[0];
                var rot = (Quaternion?)objArr[1];
                var scale = (Vector3?)objArr[2];

                if (pos == null || rot == null || scale == null) return;

                var platformDict = ev.Code == (byte)EventCode.RightSpawned ? RightPlat_Networked : LeftPlat_Networked;
                if (!platformDict.ContainsKey(ev.Sender))
                    platformDict.Add(ev.Sender, null);

                platformDict[ev.Sender] = CreatePlatform(pos.Value, rot.Value, scale.Value);
            }

            private static void HandlePlatformDespawn(EventData ev)
            {
                var platformDict = ev.Code == (byte)EventCode.RightDespawned ? RightPlat_Networked : LeftPlat_Networked;
                if (platformDict.TryGetValue(ev.Sender, out var platform))
                {
                    GameObject.Destroy(platform);
                    platformDict.Remove(ev.Sender);
                }
            }

            private static GameObject CreatePlatform(Vector3 pos, Quaternion rot, Vector3 scale)
            {
                var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platform.transform.SetPositionAndRotation(pos, rot);
                platform.transform.localScale = scale;
                return platform;
            }

            private static void StartAirJump()
            {
                var (scale, posAdditive, type) = Platform == PlatformType.Sticky
                    ? (new Vector3(0.15f, 0.18f, 0.2825f), Vector3.zero, PrimitiveType.Sphere)
                    : (new Vector3(0.0025f, 0.18f, 0.2825f), new Vector3(0f, -0.0175f, 0f), PrimitiveType.Cube);

                StartNetworkedJump();
                var others = new RaiseEventOptions { Receivers = ReceiverGroup.Others };

                HandlePlatform(ref RightPlat, RightGrip, GorillaLocomotion.GTPlayer.Instance.rightControllerTransform, EventCode.RightSpawned, EventCode.RightDespawned, type, scale, posAdditive, others);
                HandlePlatform(ref LeftPlat, LeftGrip, GorillaLocomotion.GTPlayer.Instance.leftControllerTransform, EventCode.LeftSpawned, EventCode.LeftDespawned, type, scale, posAdditive, others);
            }

            private static void HandlePlatform(ref GameObject platform, bool grip, Transform controllerTransform, EventCode spawnCode, EventCode despawnCode, PrimitiveType type, Vector3 scale, Vector3 posAdditive, RaiseEventOptions others)
            {
                if (grip && platform == null && (controllerTransform == GorillaLocomotion.GTPlayer.Instance.rightControllerTransform || !GunLib.data.isShooting))
                {
                    platform = CreatePlatform(controllerTransform.position + posAdditive, controllerTransform.rotation, scale);
                    platform.layer = LayerMask.NameToLayer("Gorilla Object");
                    var stink = platform.AddComponent<GorillaSurfaceOverride>();
                    stink.overrideIndex = 7;

                    if (Platform != PlatformType.Invis)
                        platform.GetComponent<Renderer>().material.color = PlatColor;
                    else
                        GameObject.Destroy(platform.GetComponent<Renderer>());

                    if (PhotonNetwork.CurrentRoom != null)
                        PhotonNetwork.RaiseEvent((byte)spawnCode, new object[] { platform.transform.position, platform.transform.rotation, platform.transform.localScale }, others, SendOptions.SendReliable);
                }
                else if (!grip && platform != null)
                {
                    if (PhotonNetwork.CurrentRoom != null)
                        PhotonNetwork.RaiseEvent((byte)despawnCode, null, others, SendOptions.SendReliable);

                    GameObject.Destroy(platform);
                    platform = null;
                }
            }

            public static void Execute() => StartAirJump();

            private enum EventCode : byte
            {
                RightSpawned = 45,
                RightDespawned = 46,
                LeftSpawned = 47,
                LeftDespawned = 48
            }
        }

        #endregion
    }
}
