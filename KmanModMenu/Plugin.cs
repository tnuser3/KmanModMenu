using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
            
        }
    }
}
