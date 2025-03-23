#region

using System.Linq;
using GorillaLocomotion;
using UnityEngine;

#endregion

namespace KmanModMenu.Utilities
{
    internal class GhostLib : MonoBehaviour
    {
        private static VRRig ghostRig;
        private static Material ghostMaterial;

        public void Update()
        {
            if (ghostRig == null)
            {
                ghostRig = Object.Instantiate(
                    GorillaTagger.Instance.offlineVRRig,
                    GorillaLocomotion.GTPlayer.Instance.transform.position,
                    GorillaLocomotion.GTPlayer.Instance.transform.rotation
                );
                ghostRig.enabled = false;
                ghostRig.transform.position = Vector3.zero;

                ghostRig.transform.Find("VR Constraints/LeftArm/Left Arm IK/SlideAudio").gameObject.SetActive(false);
                ghostRig.transform.Find("VR Constraints/RightArm/Right Arm IK/SlideAudio").gameObject.SetActive(false);
                if (ghostMaterial == null)
                {
                    var colour = new Color32(255, 255, 255, 90);
                    ghostMaterial = new Material(Shader.Find("GUI/Text Shader")) { color = colour };
                    var c32array = Enumerable.Repeat(colour, ghostRig.mainSkin.sharedMesh.colors32.Length).ToArray();
                    var carray = Enumerable.Repeat((Color)colour, ghostRig.mainSkin.sharedMesh.colors.Length).ToArray();
                    ghostRig.mainSkin.sharedMesh.colors32 = c32array;
                    ghostRig.mainSkin.sharedMesh.colors = carray;
                    ghostRig.mainSkin.material.color = colour;
                }
            }

            if (!GorillaTagger.Instance.offlineVRRig.enabled)
            {
                ghostRig.enabled = true;
                ghostRig.rightHandTransform.position = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform.position;
                ghostRig.leftHandTransform.position = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position;
                ghostRig.mainSkin.material = ghostMaterial;
            }
            else
            {
                ghostRig.enabled = false;
                ghostRig.mainSkin.material = null;
                ghostRig.transform.position = Vector3.zero;
            }
        }
    }
}