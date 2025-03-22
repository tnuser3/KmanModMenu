using DiscordRPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KmanModMenu.Utilities
{
    internal class DiscordRPCHandler
    {
        static Assembly rpcAssembly;

        private static byte[] LoadEmbeddedResource(string resourceName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Resource '{resourceName}' not found.");
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        public static void Initialize()
        {
            byte[] dllBytes = LoadEmbeddedResource("KmanModMenu.Assets.DiscordRPC.dll");
            rpcAssembly = Assembly.Load(dllBytes);
        }

        public static void Start()
        {
            var client = new DiscordRpcClient("1352836495760691231");

            client.OnReady += (sender, e) =>
            {
                Console.WriteLine($"Connected to Discord as {e.User.Username}");
            };

            client.OnError += (sender, e) =>
            {
                Console.WriteLine($"Error: {e.Message}");
            };

            client.Initialize();

            client.SetPresence(new RichPresence
            {
                Details = "KmanModMenu in Gorilla Tag",
                State = "https://discord.gg/hvmBnG6dBd",
                Assets = new DiscordRPC.Assets
                {
                    LargeImageKey = "pngimg_com_-_baby_png17931", // Replace with your image key from Discord Developer Portal
                    LargeImageText = "Logo"
                }, 
                Type = ActivityType.Playing
            });

            
        }
    }
}
