#region

using UnityEngine;

#endregion

namespace KmanModMenu.Utilities
{
    internal class ColourLerp : MonoBehaviour
    {
        public static float time;

        public Color StartColor;
        public Color EndColor;
        private readonly float lerpDuration = 1f;
        private float lerpTime;

        private Renderer renderer;

        private void Start()
        {
            renderer = GetComponent<Renderer>();

            if (renderer.material == null)
            {
                renderer.material = new Material(Shader.Find("GorillaTag/UberShader"));
            }

            renderer.material.color = StartColor;

        }

        private void Update()
        {
            lerpTime += Time.deltaTime;
            var c = Color.Lerp(StartColor, EndColor, Mathf.PingPong(lerpTime / lerpDuration, 1f));
            renderer.material.color = c;

            time = lerpTime;
        }

        private void OnEnable()
        {
            lerpTime = time;
        }
    }
}