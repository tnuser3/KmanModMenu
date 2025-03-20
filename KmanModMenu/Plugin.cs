using BepInEx;
using KmanModMenu.Utilities;
using ModIOBrowser.Implementation;
using System;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;
using static KmanModMenu.Utilities.Inputs;

namespace KmanModMenu
{
    [BepInPlugin("com.kman.modmenu", "modmenu", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        #region Initializer
        static bool _initialized = false;

        public void Awake()
        {
            if (_initialized) return;
            var go = new GameObject("KmanModMenu");
            go.AddComponent<Plugin>();
            DontDestroyOnLoad(go);

            _initialized= true;

            Destroy(this);
        }
        #endregion

        public void LateUpdate()
        {
            try
            {
                if (LeftSecondary && menu == null)
                {
                    Draw();
                    if (refrence == null)
                    {
                        refrence = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        refrence.transform.parent = GorillaLocomotion.Player.Instance.rightControllerTransform;
                        refrence.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                        refrence.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    }
                }
                else
                {
                    if (!LeftSecondary && menu != null)
                    {
                        if (FakeMenu)
                        {
                            GorillaLocomotion.Player.Instance.inOverlay = false;
                            GorillaTagger.Instance.offlineVRRig.enabled = true;
                            GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.isKinematic = false;
                        }

                        Destroy(menu);
                        menu = null;
                        Destroy(refrence);
                        refrence = null;
                    }
                }

                if (LeftSecondary && menu != null)
                {
                    menu.transform.position = GorillaLocomotion.Player.Instance.leftControllerTransform.position;
                    menu.transform.rotation = GorillaLocomotion.Player.Instance.leftControllerTransform.rotation;
                }

                foreach (var b in Movement)
                    if (b != null && b.Enabled && b.onClick != null)
                        b.onClick();

                foreach (var b in Config)
                    if (b != null && b.Enabled && b.onClick != null)
                        b.onClick();

                foreach (var b in Overpowered)
                    if (b != null && b.Enabled && b.onClick != null)
                        b.onClick();

                foreach (var b in Player)
                    if (b != null && b.Enabled && b.onClick != null)
                        b.onClick();

                foreach (var b in Visual)
                    if (b != null && b.Enabled && b.onClick != null)
                        b.onClick();
            }
            catch (Exception _)
            {
                Console.WriteLine(_.ToJson());
            }
        }

        #region BuildMenu
        /// <summary>
        /// Builds the mod menu
        /// </summary>
        public static void Draw()
        {
            try
            {
                menu = new GameObject("menuParent")
                {
                    transform =
                    {
                        localScale = new Vector3(0.1f, 0.3f, 0.4f) * GorillaLocomotion.Player.Instance.scale
                    }
                };

                var bg = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var rb = bg.GetComponent<Rigidbody>();
                var col = bg.GetComponent<BoxCollider>();

                if (rb) Destroy(rb);

                if (col) Destroy(col);

                bg.transform.parent = menu.transform;
                bg.transform.rotation = Quaternion.identity;
                bg.transform.localScale = new Vector3(0.1f, 0.91f, 0.86f);
                bg.transform.localPosition = new Vector3(0.5f, 0f, 0.012f);

                var lerp = bg.AddComponent<ColourLerp>();
                lerp.StartColor = new Color32(39, 7, 99, 255);
                lerp.EndColor = new Color32(44, 8, 112, 255);

                canvasObject = new GameObject("Canvas")
                {
                    transform =
                    {
                        parent = menu.transform
                    }
                };
                var canvas = canvasObject.AddComponent<Canvas>();
                var canvasScaler = canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();
                canvas.renderMode = RenderMode.WorldSpace;
                canvasScaler.dynamicPixelsPerUnit = 1000f;
                canvasObject.transform.localScale *= GorillaLocomotion.Player.Instance.scale;

                var text = new GameObject("textObj").AddComponent<Text>();

                text.transform.SetParent(canvasObject.transform);

                text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text.text = "Kman Mod Menu";
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                var rt = text.GetComponent<RectTransform>();
                rt.localPosition = Vector3.zero;
                rt.sizeDelta = new Vector2(0.28f, 0.05f);
                rt.position = new Vector3(0.06f, 0f, 0.1495f);
                rt.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                GenerateButtons();
            }
            catch (Exception _)
            {
                Console.WriteLine(_.ToJson());
            }
        }

        /// <summary>
        /// Builds the mod buttons
        /// </summary>
        public static void GenerateButtons()
        {
            try
            {
                var cPage = Home;
                switch (CurrentPage)
                {
                    case PageType.Movement: cPage = Movement; break;
                    case PageType.Player: cPage = Player; break;
                    case PageType.Visual: cPage = Visual; break;
                    case PageType.Config: cPage = Config; break;
                    case PageType.OP: cPage = Overpowered; break;
                }

                var array2 = cPage.Skip(PageNumber * PageSize).Take(PageSize).ToArray();
                for (var i = 0; i < array2.Length; i++) CreateButton(array2[i], i);

                if (CurrentPage == PageType.Home) return;

                // Page Buttons

                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.12f, 0.78f);
                gameObject.transform.localPosition = new Vector3(0.56f, 0.53f, 0f);

                gameObject.AddComponent<ButtonInclineCollider>().inc = 1;

                var lerp = gameObject.AddComponent<ColourLerp>();
                lerp.StartColor = new Color32(39, 7, 99, 255);
                lerp.EndColor = new Color32(44, 8, 112, 255);


                var tiosber = new GameObject();
                var text = tiosber.AddComponent<Text>();

                text.transform.SetParent(canvasObject.transform);

                text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text.text = "<";
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                var component = text.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.064f, 0.155f, 0.006f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                var gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(gameObject2.GetComponent<Rigidbody>());
                gameObject2.GetComponent<BoxCollider>().isTrigger = true;
                gameObject2.transform.parent = menu.transform;
                gameObject2.transform.rotation = Quaternion.identity;
                gameObject2.transform.localScale = new Vector3(0.09f, 0.12f, 0.78f);
                gameObject2.transform.localPosition = new Vector3(0.56f, -0.53f, 0f);

                gameObject2.AddComponent<ButtonInclineCollider>().inc = -1;

                var lerp1 = gameObject2.AddComponent<ColourLerp>();
                lerp1.StartColor = new Color32(39, 7, 99, 255);
                lerp1.EndColor = new Color32(44, 8, 112, 255);

                var text2nd = new GameObject();
                var text2 = text2nd.AddComponent<Text>();

                text2.transform.SetParent(canvasObject.transform);

                text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text2.text = ">";
                text2.fontSize = 1;
                text2.alignment = TextAnchor.MiddleCenter;
                text2.resizeTextForBestFit = true;
                text2.resizeTextMinSize = 0;
                var component2 = text2.GetComponent<RectTransform>();
                component2.localPosition = Vector3.zero;
                component2.sizeDelta = new Vector2(0.2f, 0.03f);
                component2.localPosition = new Vector3(0.064f, -0.155f, 0.006f);
                component2.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));

                // backButton

                var backButton = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(backButton.GetComponent<Rigidbody>());
                backButton.GetComponent<BoxCollider>().isTrigger = true;
                backButton.transform.parent = menu.transform;
                backButton.transform.rotation = Quaternion.identity;
                backButton.transform.localScale = new Vector3(0.09f, 0.8f, 0.08f);
                backButton.transform.localPosition = new Vector3(0.56f, 0f, 0.51f);

                backButton.AddComponent<ButtonCollider>().refrence = new Button
                {
                    onClick = () =>
                    {
                        CurrentPage = PageType.Home;
                        PageNumber = 0;
                    }
                };

                var lerp3 = backButton.AddComponent<ColourLerp>();
                lerp1.StartColor = new Color32(39, 7, 99, 255);
                lerp1.EndColor = new Color32(44, 8, 112, 255);

                var imrealylgay = new GameObject();
                var text3 = imrealylgay.AddComponent<Text>();

                text3.transform.SetParent(canvasObject.transform);

                text3.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text3.text = "Back";
                text3.fontSize = 1;
                text3.alignment = TextAnchor.MiddleCenter;
                text3.resizeTextForBestFit = true;
                text3.resizeTextMinSize = 0;

                var component23 = text3.GetComponent<RectTransform>();
                component23.localPosition = Vector3.zero;
                component23.sizeDelta = new Vector2(0.2f, 0.03f);
                component23.localPosition = new Vector3(0.064f, -0.003f, 0.205f);
                component23.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            catch (Exception _)
            {
                Console.WriteLine(_.ToJson());
            }
        }

        /// <summary>
        /// Creates a button component
        /// </summary>
        /// <param name="refrence">the button refrence that you're creating the object for.</param>
        /// <param name="index">the index of the refrence.</param>
        public static void CreateButton(Button refrence, int index)
        {
            try
            {
                var offset = index * 0.14f;

                var gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Destroy(gameObject.GetComponent<Rigidbody>());
                gameObject.GetComponent<BoxCollider>().isTrigger = true;
                gameObject.transform.parent = menu.transform;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(0.09f, 0.85f, 0.113f);
                gameObject.transform.localPosition = new Vector3(0.56f, 0f, 0.23f - offset);
                gameObject.AddComponent<ButtonCollider>().refrence = refrence;

                if (refrence.Enabled)
                    gameObject.GetComponent<Renderer>().material.color = new Color32(73, 16, 181, 255);
                else
                    gameObject.GetComponent<Renderer>().material.color = new Color32(63, 14, 156, 255);
                var text2object = new GameObject();
                var text2 = text2object.AddComponent<Text>();
                text2.transform.SetParent(canvasObject.transform);

                text2.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text2.text = refrence.Name;
                text2.fontSize = 1;
                text2.alignment = TextAnchor.MiddleCenter;
                text2.resizeTextForBestFit = true;
                text2.resizeTextMinSize = 0;
                var component = text2.GetComponent<RectTransform>();
                component.localPosition = Vector3.zero;
                component.sizeDelta = new Vector2(0.2f, 0.03f);
                component.localPosition = new Vector3(0.064f, 0f, 0.0935f - offset / 2.55f);
                component.rotation = Quaternion.Euler(new Vector3(180f, 90f, 90f));
            }
            catch (Exception _)
            {
                Console.WriteLine(_.ToJson());
            }
        }

        /// <summary>
        /// Triggers the onclick event if the button isnt on, if the button is on it will trigger the ondisable event if its not null.
        /// </summary>
        /// <param name="refrence">The button you're turning on/off</param>
        public static void OnClick(ref Button refrence)
        {
            try
            {
                if (refrence == null) return;

                if (refrence.isToggle)
                {
                    refrence.Enabled = !refrence.Enabled;

                    if (!refrence.Enabled && refrence.onDisable != null)
                        refrence.onDisable();
                }
                else
                {
                    refrence.onClick();
                }

                ReloadMenu();
            }
            catch (Exception _)
            {
                Console.WriteLine(_.ToJson());
            }
        }

        /// <summary>
        /// Deletes the menu and re-enables it
        /// </summary>
        public static void ReloadMenu()
        {
            if (menu == null) return;
            Destroy(menu);
            menu = null;
            Draw();
        }

        #endregion

        #region Variables
        public static float clickFrame;
        public static GameObject menu, refrence, canvasObject;
        public static int PageNumber, PageSize = 5;
        public static PageType CurrentPage = PageType.Home;
        public static bool FakeMenu;

        public static Button[] Home =
        {
            new Button
            {
                Name = "Config",
                onClick = () =>
                {
                    CurrentPage = PageType.Config;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new Button
            {
                Name = "Movement",
                onClick = () =>
                {
                    CurrentPage = PageType.Movement;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new Button
            {
                Name = "Player",
                onClick = () =>
                {
                    CurrentPage = PageType.Player;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new Button
            {
                Name = "Visual",
                onClick = () =>
                {
                    CurrentPage = PageType.Visual;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new Button
            {
                Name = "OP",
                onClick = () =>
                {
                    CurrentPage = PageType.OP;
                    PageNumber = 0;
                },
                isToggle = false
            }
        };

        public static ConfigButton[] Config =
        {
        };

        public static Button[] Movement =
        {
        };

        public static Button[] Player =
        {
        };

        public static Button[] Visual =
        {
        };

        public static Button[] Overpowered =
        { 
        };

        #endregion

        #region Nested

        [RequireComponent(typeof(Collider))]
        private class ButtonCollider : MonoBehaviour
        {
            public Button refrence;

            public void OnTriggerEnter(Collider other)
            {
                try
                {
                    if (other.gameObject != Plugin.refrence) return;
                    if (Time.time > clickFrame)
                    {
                        clickFrame = Time.time + 0.5f;
                        OnClick(ref refrence);
                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 1f);
                        GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength,
                            GorillaTagger.Instance.tagHapticDuration / 2);
                    }
                }
                catch (Exception _)
                {
                    Console.WriteLine(_.ToJson());
                }
            }
        }

        [RequireComponent(typeof(Collider))]
        private class ButtonInclineCollider : MonoBehaviour
        {
            public int inc;

            public void OnTriggerEnter(Collider other)
            {
                try
                {
                    if (other.gameObject != refrence) return;
                    Console.WriteLine("Trigger activated");
                    if (Time.time > clickFrame)
                    {
                        clickFrame = Time.time + 0.5f;
                        var cPage = Home;
                        switch (CurrentPage)
                        {
                            case PageType.Movement: cPage = Movement; break;
                            case PageType.Player: cPage = Player; break;
                            case PageType.Visual: cPage = Visual; break;
                            case PageType.Config: cPage = Config; break;
                            case PageType.OP: cPage = Overpowered; break;
                            default:
                                PageNumber = 0;
                                CurrentPage = PageType.Home;
                                ReloadMenu();
                                return;
                        }

                        var num = (cPage.Length + PageSize - 1) / PageSize;
                        if (inc != 1)
                        {
                            if (PageNumber < num - 1)
                                PageNumber++;
                            else
                                PageNumber = 0;
                        }
                        else
                        {
                            if (PageNumber > 0)
                                PageNumber--;
                            else
                                PageNumber = num - 1;
                        }


                        ReloadMenu();

                        GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 1f);
                        GorillaTagger.Instance.StartVibration(false, GorillaTagger.Instance.tagHapticStrength,
                            GorillaTagger.Instance.tagHapticDuration / 2);
                    }
                }
                catch (Exception _)
                {
                    Console.WriteLine(_.ToJson());
                }
            }
        }

        public enum PageType
        {
            Home,
            Config,
            Movement,
            Player,
            Visual,
            OP
        }

        public class Button
        {
            public virtual string ToolTip { get; set; }
            public virtual string Controls { get; set; }
            public virtual string Name { get; set; }
            public virtual bool Enabled { get; set; }
            public virtual bool isToggle { get; set; }
            public virtual Action onClick { get; set; }
            public virtual Action onDisable { get; set; }
        }

        public class ConfigButton : Button
        {
            public Action<ConfigButton> CycleAction { get; set; }
            public string Additive { get; set; }

            public override string Name
            {
                get => base.Name + " : " + Additive;
                set => base.Name = value;
            }

            public override Action onClick
            {
                get => () => ConfigHandler.Cycle(this);
                set => base.onClick = value;
            }
        }

        #endregion
    }
}
