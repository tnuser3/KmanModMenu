#region

using BepInEx;
using GorillaLocomotion;
using UnityEngine;
using UnityEngine.XR;
using static KmanModMenu.Utilities.Inputs;

#endregion

namespace KmanModMenu.Utilities
{
    internal class GunLib
    {
        public static int Layers = ~((1 << LayerMask.NameToLayer("TransparentFX")) |
                                     (1 << LayerMask.NameToLayer("Ignore Raycast")) |
                                     (1 << LayerMask.NameToLayer("GorillaCosmetics")) |
                                     (1 << LayerMask.NameToLayer("Zone")) |
                                     (1 << LayerMask.NameToLayer("Gorilla Boundary")) |
                                     (1 << LayerMask.NameToLayer("GorillaParticle")) |
                                     (1 << LayerMask.NameToLayer("Gorilla Trigger")));

        public static GameObject pointer;
        public static GunLibData data = new GunLibData(false, false, false);

        public static void GunCleanUp()
        {
            data = new GunLibData(false, false, false);
            if (pointer == null) return;
            Object.Destroy(pointer);
            pointer = null;
        }

        public static GunLibData Shoot()
        {
            if (data == null) data = new GunLibData(false, false, false);
            if (XRSettings.isDeviceActive)
            {
                data.isShooting = RightGrip;
                data.isTriggered = RightTrigger;
                if (data.isShooting)
                {
                    var result =
                        Physics.Raycast(
                            Player.Instance.rightControllerTransform.position -
                            Player.Instance.rightControllerTransform.up, -Player.Instance.rightControllerTransform.up,
                            out var hit, float.PositiveInfinity, Layers);
                    if (result)
                    {
                        if (pointer == null)
                        {
                            pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                            GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                            pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            if (pointer.GetComponent<Renderer>())
                            {
                                pointer.GetComponent<Renderer>().material.color = Color.red;
                                pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            }
                        }
                        else
                        {
                            data.hitPosition = hit.point;

                            var velocity = Vector3.zero;
                            pointer.transform.position = Vector3.SmoothDamp(hit.point, pointer.transform.position,
                                ref velocity, 7f);
                        }

                        if (data.isTriggered)
                        {
                            if (hit.collider != null)
                            {
                                var rig = hit.collider.GetComponentInParent<VRRig>();
                                if (rig != null)
                                {
                                    data.lockedPlayer = rig;
                                    data.isLocked = true;
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.green;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider != null)
                            {
                                var rig = hit.collider.GetComponentInParent<VRRig>();
                                if (rig != null)
                                {
                                    data.lockedPlayer = null;
                                    data.isLocked = false;
                                    GorillaTagger.Instance.StartVibration(false,
                                        GorillaTagger.Instance.tagHapticStrength / 2,
                                        GorillaTagger.Instance.tagHapticDuration / 2);
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.blue;
                                }
                                else
                                {
                                    data.lockedPlayer = null;
                                    data.isLocked = false;
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.red;
                                }
                            }
                            else
                            {
                                data.lockedPlayer = null;
                                data.isLocked = false;
                                if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    GunCleanUp();
                }
            }
            else
            {
                data.isShooting = UnityInput.Current.GetMouseButton(1);
                data.isTriggered = UnityInput.Current.GetMouseButton(0);
                if (data.isShooting)
                {
                    var ray =
                        GorillaTagger.Instance.thirdPersonCamera != null &&
                        GorillaTagger.Instance.thirdPersonCamera.activeSelf &&
                        GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0)
                            ? GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).GetComponent<Camera>()
                                .ScreenPointToRay(UnityInput.Current.mousePosition)
                            : GorillaTagger.Instance.mainCamera.GetComponent<Camera>()
                                .ScreenPointToRay(UnityInput.Current.mousePosition);
                    var result = Physics.Raycast(ray.origin, ray.direction, out var hit, int.MaxValue, Layers);
                    if (result)
                    {
                        if (pointer == null)
                        {
                            pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                            GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                            pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                            if (pointer.GetComponent<Renderer>())
                            {
                                pointer.GetComponent<Renderer>().material.color = Color.red;
                                pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                            }
                        }
                        else
                        {
                            data.hitPosition = hit.point;

                            var velocity = Vector3.zero;
                            pointer.transform.position = Vector3.SmoothDamp(hit.point, pointer.transform.position,
                                ref velocity, 7f);
                        }

                        if (data.isTriggered)
                        {
                            if (hit.collider != null)
                            {
                                var rig = hit.collider.GetComponentInParent<VRRig>();
                                if (rig != null)
                                {
                                    data.lockedPlayer = rig;
                                    data.isLocked = true;
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.green;
                                }
                            }
                        }
                        else
                        {
                            if (hit.collider != null)
                            {
                                var rig = hit.collider.GetComponentInParent<VRRig>();
                                if (rig != null)
                                {
                                    data.lockedPlayer = null;
                                    data.isLocked = false;
                                    GorillaTagger.Instance.StartVibration(false,
                                        GorillaTagger.Instance.tagHapticStrength / 2,
                                        GorillaTagger.Instance.tagHapticDuration / 2);
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.blue;
                                }
                                else
                                {
                                    data.lockedPlayer = null;
                                    data.isLocked = false;
                                    if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                        pointer.GetComponent<Renderer>().material.color = Color.red;
                                }
                            }
                            else
                            {
                                data.lockedPlayer = null;
                                data.isLocked = false;
                                if (pointer != null && pointer.GetComponent<Renderer>() != null)
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    GunCleanUp();
                }
            }

            return data;
        }

        public static GunLibData ShootLocked()
        {
            if (data == null) data = new GunLibData(false, false, false);
            if (XRSettings.isDeviceActive)
            {
                data.isShooting = RightGrip;
                data.isTriggered = RightTrigger;

                if (data.isShooting)
                {
                    if (data.lockedPlayer == null)
                    {
                        var result =
                            Physics.Raycast(
                                Player.Instance.rightControllerTransform.position -
                                Player.Instance.rightControllerTransform.up,
                                -Player.Instance.rightControllerTransform.up, out var hit, float.PositiveInfinity,
                                Layers);
                        if (result)
                        {
                            if (pointer == null)
                            {
                                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                                GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                if (pointer.GetComponent<Renderer>())
                                {
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                                    pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                                }
                            }
                            else
                            {
                                data.hitPosition = hit.point;

                                var velocity = Vector3.zero;
                                pointer.transform.position = Vector3.SmoothDamp(hit.point, pointer.transform.position,
                                    ref velocity, 7f);
                            }

                            if (data.isTriggered)
                            {
                                if (hit.collider != null)
                                {
                                    var rig = hit.collider.GetComponentInParent<VRRig>();
                                    if (rig != null)
                                    {
                                        data.lockedPlayer = rig;
                                        data.isLocked = true;
                                    }
                                    else
                                    {
                                        data.lockedPlayer = null;
                                        data.isLocked = false;
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                }
                            }
                            else
                            {
                                if (hit.collider != null)
                                {
                                    var rig = hit.collider.GetComponentInParent<VRRig>();
                                    if (rig != null)
                                    {
                                        GorillaTagger.Instance.StartVibration(false,
                                            GorillaTagger.Instance.tagHapticStrength / 2,
                                            GorillaTagger.Instance.tagHapticDuration / 2);
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.blue;
                                    }
                                    else
                                    {
                                        data.lockedPlayer = null;
                                        data.isLocked = false;
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (data.isTriggered)
                        {
                            if (pointer == null)
                            {
                                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                                GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                if (pointer.GetComponent<Renderer>())
                                {
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                                    pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                                }
                            }
                            else
                            {
                                data.hitPosition = data.lockedPlayer.transform.position;
                                pointer.transform.position = data.lockedPlayer.transform.position;
                            }

                            if (pointer.GetComponent<Renderer>())
                                pointer.GetComponent<Renderer>().material.color = Color.green;
                        }
                        else
                        {
                            data.lockedPlayer = null;
                            data.isLocked = false;
                            if (pointer.GetComponent<Renderer>())
                                pointer.GetComponent<Renderer>().material.color = Color.red;
                        }
                    }
                }
                else
                {
                    GunCleanUp();
                }
            }
            else
            {
                data.isShooting = UnityInput.Current.GetMouseButton(1);
                data.isTriggered = UnityInput.Current.GetMouseButton(0);

                if (data.isShooting)
                {
                    if (data.lockedPlayer == null)
                    {
                        var ray =
                            GorillaTagger.Instance.thirdPersonCamera != null &&
                            GorillaTagger.Instance.thirdPersonCamera.activeSelf &&
                            GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0)
                                ? GorillaTagger.Instance.thirdPersonCamera.transform.GetChild(0).GetComponent<Camera>()
                                    .ScreenPointToRay(UnityInput.Current.mousePosition)
                                : GorillaTagger.Instance.mainCamera.GetComponent<Camera>()
                                    .ScreenPointToRay(UnityInput.Current.mousePosition);
                        var result = Physics.Raycast(ray.origin, ray.direction, out var hit, int.MaxValue, Layers);
                        if (result)
                        {
                            if (pointer == null)
                            {
                                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                                GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                if (pointer.GetComponent<Renderer>())
                                {
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                                    pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                                }
                            }
                            else
                            {
                                data.hitPosition = hit.point;

                                var velocity = Vector3.zero;
                                pointer.transform.position = Vector3.SmoothDamp(hit.point, pointer.transform.position,
                                    ref velocity, 7f);
                            }

                            if (data.isTriggered)
                            {
                                if (hit.collider != null)
                                {
                                    var rig = hit.collider.GetComponentInParent<VRRig>();
                                    if (rig != null)
                                    {
                                        data.lockedPlayer = rig;
                                        data.isLocked = true;
                                        if (pointer.GetComponent<Renderer>())
                                        {
                                            GorillaTagger.Instance.StartVibration(false,
                                                GorillaTagger.Instance.tagHapticStrength / 2,
                                                GorillaTagger.Instance.tagHapticDuration / 2);
                                            pointer.GetComponent<Renderer>().material.color = Color.blue;
                                        }
                                    }
                                    else
                                    {
                                        data.lockedPlayer = null;
                                        data.isLocked = false;
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                }
                            }
                            else
                            {
                                if (hit.collider != null)
                                {
                                    var rig = hit.collider.GetComponentInParent<VRRig>();
                                    if (rig != null)
                                    {
                                        GorillaTagger.Instance.StartVibration(false,
                                            GorillaTagger.Instance.tagHapticStrength / 2,
                                            GorillaTagger.Instance.tagHapticDuration / 2);
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.blue;
                                    }
                                    else
                                    {
                                        data.lockedPlayer = null;
                                        data.isLocked = false;
                                        if (pointer.GetComponent<Renderer>())
                                            pointer.GetComponent<Renderer>().material.color = Color.red;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (data.isTriggered)
                        {
                            if (pointer == null)
                            {
                                pointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                                GameObject.Destroy(pointer.GetComponent<Rigidbody>());
                                GameObject.Destroy(pointer.GetComponent<SphereCollider>());
                                pointer.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                                if (pointer.GetComponent<Renderer>())
                                {
                                    pointer.GetComponent<Renderer>().material.color = Color.red;
                                    pointer.GetComponent<Renderer>().material.shader = Shader.Find("GUI/Text Shader");
                                }
                            }
                            else
                            {
                                data.hitPosition = data.lockedPlayer.transform.position;
                                pointer.transform.position = data.lockedPlayer.transform.position;
                            }

                            if (pointer.GetComponent<Renderer>())
                                pointer.GetComponent<Renderer>().material.color = Color.green;
                        }
                        else
                        {
                            data.lockedPlayer = null;
                            data.isLocked = false;
                            if (pointer.GetComponent<Renderer>())
                                pointer.GetComponent<Renderer>().material.color = Color.red;
                        }
                    }
                }
                else
                {
                    GunCleanUp();
                }
            }

            return data;
        }

        public class GunLibData
        {
            public GunLibData(bool stateTriggered, bool triggy, bool foundPlayer, VRRig player = null,
                Vector3 hitpos = new Vector3())
            {
                lockedPlayer = player;
                isShooting = stateTriggered;
                isLocked = foundPlayer;
                hitPosition = hitpos;
                isTriggered = triggy;
            }

            public VRRig lockedPlayer { get; set; }

            public bool isShooting { get; set; }

            public bool isLocked { get; set; }

            public Vector3 hitPosition { get; set; }

            public bool isTriggered { get; set; }

            public override string ToString()
            {
                return "GunLibData_ByKman";
            }
        }
    }
}