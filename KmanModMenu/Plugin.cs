using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using KmanModMenu.Mods;
using KmanModMenu.Mods.Player;
using KmanModMenu.Utilities;
using ModIOBrowser.Implementation;
using System;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static KmanModMenu.Utilities.Inputs;

namespace KmanModMenu
{
    [BepInPlugin("com.kman.modmenu", "kmanmodmenu", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        #region Initializer
        static bool _initialized = false;

        public void Awake()
        {
            if (_initialized) return;

            /*
            string bepinexConfigPath = Path.Combine(Paths.ConfigPath, "BepInEx.cfg");

            // All this does is checks if the hidemanager is on and if it is then it wont do anything if its not then it goes through the process of creating its own hidden object.
            if (File.Exists(bepinexConfigPath))
            {
                ConfigFile bepinexConfig = new ConfigFile(bepinexConfigPath, true);
                var hideManager = bepinexConfig.Bind("BepInEx", "HideManagerGameObject", false);
                if (!hideManager.Value)
                    goto _init;
                else return;
            }

            _init:*/

            _initialized = true;
            DiscordRPCHandler.Initialize();
            DiscordRPCHandler.Start();
            var go = new GameObject("KmanModMenu");
            go.AddComponent<Plugin>();
            go.AddComponent<Inputs>();
            go.AddComponent<GhostLib>();
            DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;
            new HarmonyLib.Harmony("KmanModMenu").PatchAll();

            Destroy(this);
        }
        #endregion
        Rect window = new Rect(10, 10, 250, 700);
        Vector2 scroll = new Vector2(0, 0);
        void OnGUI()
        {
            window = GUI.Window(193494, window, windowr, "smelly");
        }

        void windowr(int id)
        {
            scroll = GUILayout.BeginScrollView(scroll);
            for (int i = 0; i < Movement.Length; i++)
            {
                if (Movement[i] != null)
                {
                    GUI.contentColor = Movement[i].Enabled ? Color.green : Color.red;
                    if (GUILayout.Button(Movement[i].Name))
                    {
                        OnClick(ref Movement[i]);
                    }
                }
            }

            for (int i = 0; i < Overpowered.Length; i++)
            {
                if (Overpowered[i] != null)
                {
                    GUI.contentColor = Overpowered[i].Enabled ? Color.green : Color.red;
                    if (GUILayout.Button(Overpowered[i].Name))
                    {
                        OnClick(ref Overpowered[i]);
                    }
                }
            }

            for (int i = 0; i < Player.Length; i++)
            {
                if (Player[i] != null)
                {
                    GUI.contentColor = Player[i].Enabled ? Color.green : Color.red;
                    if (GUILayout.Button(Player[i].Name))
                    {
                        OnClick(ref Player[i]);
                    }
                }
            }

            for (int i = 0; i < Visual.Length; i++)
            {
                if (Visual[i] != null)
                {
                    GUI.contentColor = Visual[i].Enabled ? Color.green : Color.red;
                    if (GUILayout.Button(Visual[i].Name))
                    {
                        OnClick(ref Visual[i]);
                    }
                }
            }
            GUILayout.EndScrollView();
        }

        public void LateUpdate()
        {
            if (menu == null)
            {
                Draw();
            }
            if (refrence == null)
            {
                refrence = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                refrence.transform.parent = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform;
                refrence.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                refrence.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            }
            try
            {
                if (LeftSecondary && menu == null)
                {
                    Draw();
                    if (refrence == null)
                    {
                        refrence = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        refrence.transform.parent = GorillaLocomotion.GTPlayer.Instance.rightControllerTransform;
                        refrence.transform.localPosition = new Vector3(0f, -0.1f, 0f);
                        refrence.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    }
                }
                else
                {
                    if (!LeftSecondary && menu != null)
                    {
                        //Destroy(menu);
                        //menu = null;
                        //Destroy(refrence);
                        //refrence = null;
                    }
                }

                if (LeftSecondary && menu != null)
                {
                    menu.transform.position = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.position;
                    menu.transform.rotation = GorillaLocomotion.GTPlayer.Instance.leftControllerTransform.rotation;
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
                        localScale = new Vector3(0.1f, 0.3f, 0.4f) * GorillaLocomotion.GTPlayer.Instance.scale
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
                lerp.StartColor = Color.red * 0.45f;
                lerp.EndColor = Color.red * 0.2f;

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
                canvasObject.transform.localScale *= GorillaLocomotion.GTPlayer.Instance.scale;

                var text = new GameObject("textObj").AddComponent<Text>();

                text.transform.SetParent(canvasObject.transform);

                text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                text.text = "Kman Menu v0.1";
                text.fontSize = 1;
                text.alignment = TextAnchor.MiddleCenter;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 0;

                var rt = text.GetComponent<RectTransform>();
                rt.localPosition = Vector3.zero;
                rt.sizeDelta = new Vector2(0.24f, 0.05f);
                rt.position = new Vector3(0.06f, 0f, 0.145f);
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
                lerp.StartColor = Color.red * 0.45f;
                lerp.EndColor = Color.red * 0.2f;


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
                lerp1.StartColor = Color.red * 0.45f;
                lerp1.EndColor = Color.red * 0.2f;

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

                backButton.AddComponent<ButtonCollider>().refrence = new()
                {
                    onClick = () =>
                    {
                        CurrentPage = PageType.Home;
                        PageNumber = 0;
                    }
                };

                var lerp3 = backButton.AddComponent<ColourLerp>();
                lerp3.StartColor = Color.red * 0.45f;
                lerp3.EndColor = Color.red * 0.2f;

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
                    gameObject.GetComponent<Renderer>().material.color = Color.red * 0.45f;
                else
                {
                    var lerp = gameObject.AddComponent<ColourLerp>();
                    lerp.StartColor = Color.red * 0.45f;
                    lerp.EndColor = Color.red * 0.2f;
                }
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

        public static Button[] Home =
        {
            new()
            {
                Name = "Config",
                onClick = () =>
                {
                    CurrentPage = PageType.Config;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new()
            {
                Name = "Movement",
                onClick = () =>
                {
                    CurrentPage = PageType.Movement;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new()
            {
                Name = "Player",
                onClick = () =>
                {
                    CurrentPage = PageType.Player;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new()
            {
                Name = "Visual",
                onClick = () =>
                {
                    CurrentPage = PageType.Visual;
                    PageNumber = 0;
                },
                isToggle = false
            },
            new()
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
            new()
            {
                CycleAction = ConfigHandler.HandTapIndex,
                Additive = "Grass",
                isToggle = false,
                Name = "Hand Tap Index"
            },
        };

        public static Button[] Movement =
        {
            new()
            {
                Name="Flight",
                onClick = KmanModMenu.Mods.Movement.Flight,
                isToggle = true,
            },
            new()
            {
                Name="Platforms",
                onClick = KmanModMenu.Mods.Movement.Platforms.Execute,
                isToggle = true,
            },
            new()
            {
                Name="Speed Boost",
                onClick = KmanModMenu.Mods.Movement.SpeedBoost,
                onDisable = KmanModMenu.Mods.Movement.CleanSP,
                isToggle = true,
            },
            new()
            {
                Name="Long Arms",
                onClick = KmanModMenu.Mods.Movement.LongArms,
                onDisable = KmanModMenu.Mods.Movement.LongArmsClean,
                isToggle = true,
            },
            new()
            {
                Name="No Clip",
                onClick = KmanModMenu.Mods.Movement.NoClip,
                onDisable = KmanModMenu.Mods.Movement.DisableNoClip,
                isToggle = true,
            },
            new()
            {
                Name="Check Point",
                onClick = KmanModMenu.Mods.Movement.Checkpoint,
                onDisable = KmanModMenu.Mods.Movement.CleanCheckpoint,
                isToggle = true,
            },
            new()
            {
                Name="Teleport Gun",
                onClick = KmanModMenu.Mods.Movement.TeleportGun,
                isToggle = true,
            },
            new()
            {
                Name="Iron Monkey",
                onClick = KmanModMenu.Mods.Movement.IronMonkey,
                isToggle = true,
            },
            new()
            {
                Name="Low Gravity",
                onClick = KmanModMenu.Mods.Movement.LowGrav,
                onDisable = KmanModMenu.Mods.Movement.FixGrav,
                isToggle = true,
            },
            new()
            {
                Name="Fast Swim",
                onClick = KmanModMenu.Mods.Movement.FastSwim,
                isToggle = true,
            },
            new()
            {
                Name="Fast Spin",
                onClick = KmanModMenu.Mods.Movement.FastSpin,
                isToggle = true,
            },
            new()
            {
                Name="Water Walk",
                onClick = KmanModMenu.Mods.Movement.WaterWalk,
                onDisable = KmanModMenu.Mods.Movement.CleanWaterWalk,
                isToggle = true,
            },
        };

        public static Button[] Player =
        {            new()
            {
                Name = "Ghost Monkey",
                onClick = () => RigMods.Ghost(),
                onDisable = () => RigMods.Clean(),
                isToggle = true
            },
            new()
            {
                Name = "Invis Monkey",
                onClick = () => RigMods.Invis(),
                onDisable = () => RigMods.Clean(),
                isToggle = true
            },
            new()
            {
                Name = "Freeze Rig [G]",
                onClick = () => RigMods.PauseRig(),
                onDisable = () => RigMods.Clean(),
                isToggle = true
            },
            new()
            {
                Name = "Copy Gun",
                onClick = () => RigMods.CopyGun(),
                onDisable = () => RigMods.Clean(),
                isToggle = true
            },
            new()
            {
                Name = "Follow Player",
                onClick = () => RigMods.FollowGun(),
                isToggle = true
            },

            new()
            {
                Name = "Tag Gun",
                onClick = () => GamemodeExploits.TagGun(),
                isToggle = true
            },
            new()
            {
                Name = "Tag All",
                onClick = () => GamemodeExploits.TagAll(),
                isToggle = true
            },
            new()
            {
                Name = "Untag Gun [M]",
                onClick = () => GamemodeExploits.UntagGun(),
                isToggle = true
            },
            new()
            {
                Name = "Untag Self [M]",
                onClick = () => GamemodeExploits.UntagSelf(),
                isToggle = true
            },
            new()
            {
                Name = "Anti Tag [M]",
                onClick = () => GamemodeExploits.AntiTag(),
                isToggle = true
            },
            new()
            {
                Name = "Lock Room",
                onClick = () => LockRoom.Execute(),
                onDisable = () => LockRoom.UnlockRoom(),
                isToggle = true
            },
            new()
            {
                Name = "Shop Lift",
                onClick = () => Shoplift.Execute(),
                isToggle = true
            },
            new()
            {
                Name = "Rope Up",
                onClick = () => Rope.Up(),
                isToggle = true
            },
            new()
            {
                Name = "Rope Down",
                onClick = () => Rope.Down(),
                isToggle = true
            },

            new()
            {
                Name = "Cosmetic Spazz",
                onClick = () => Spammer.ExecuteCosmetic(),
                isToggle = true
            },
            new()
            {
                Name = "Braclet Spam [T]",
                onClick = () => Spammer.ExecuteBracelet(),
                isToggle = true
            },
            new()
            {
                Name = "Hand Tap Spam",
                onClick = () => Spammer.ExecuteHandTap(),
                isToggle = true
            },
            new()
            {
                Name = "Door Spam",
                onClick = () => Spammer.ExecuteDoor(),
                isToggle = true
            },

            new()
            {
                Name = "Projectile Spam",
                onClick = () => Projectiles.Execute(),
                isToggle = true
            },

        };

        public static Button[] Visual =
        {
            new()
            {
                Name="Chams",
                onClick = KmanModMenu.Mods.Visual.Chams.Execute,
                onDisable = KmanModMenu.Mods.Visual.Chams.Disable,
                isToggle = true,
            },
            new()
            {
                Name="Wireframe ESP",
                onClick = KmanModMenu.Mods.Visual.BoxFrameESP.Execute,
                onDisable = KmanModMenu.Mods.Visual.BoxFrameESP.Disable,
                isToggle = true,
            },
            new()
            {
                Name="Box ESP",
                onClick = KmanModMenu.Mods.Visual.BoxESP.Execute,
                onDisable = KmanModMenu.Mods.Visual.BoxESP.Disable,
                isToggle = true,
            },
            new()
            {
                Name="Tracers",
                onClick = KmanModMenu.Mods.Visual.Tracers.Execute,
                onDisable = KmanModMenu.Mods.Visual.Tracers.Disable,
                isToggle = true,
            },
            new()
            {
                Name="Bread Crumbs",
                onClick = KmanModMenu.Mods.Visual.BreadCrumbs.Execute,
                onDisable = KmanModMenu.Mods.Visual.BreadCrumbs.Disable,
                isToggle = true,
            },
        };

        public static Button[] Overpowered =
        {
            new() {
                Name="Create Board",
                onClick = () => Task.Run(()=>KmanModMenu.Mods.HoverboardItem.GenerateBoard()),
                isToggle = false,
            },
            new()
            {
                Name="Create Board Gun",
                onClick = () => Task.Run(()=>KmanModMenu.Mods.HoverboardItem.BoardGun()),
                isToggle = true,
            },
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
