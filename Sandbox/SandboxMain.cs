using System;
using Harmony;
using RoR2;
using Sandbox.Command;
using Sandbox.Network;
using Sandbox.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityModManagerNet;
using Console = RoR2.Console;

namespace Sandbox {
    [EnableReloading]
    public class SandboxMain {
        private static HarmonyInstance harmony;

        private static float timeout = 0;
        private static string lastCmd = null;
        private static bool previousServerState = false;
        private static HudContainer hudContainer;

        public static UnityModManager.ModEntry Mod { get; private set; }
        public static CommandHandler CmdHandler { get; private set; }
        public static NetworkCommandListener NetHandler { get; private set; }

        public static void Load(UnityModManager.ModEntry entry) {
            Mod = entry;
            entry.OnUnload = Unload;
            entry.OnFixedUpdate = OnUpdate;
            entry.OnGUI = OnGui;

            CmdHandler = new CommandHandler();
            NetHandler = new NetworkCommandListener();

            harmony = HarmonyInstance.Create(entry.Info.Id);
            harmony.PatchAll();

            Console.onLogReceived += ChatOnChatChanged;

            GameObject gameObject = new GameObject();
            gameObject.AddComponent<SetDontDestroyOnLoad>();
            hudContainer = gameObject.AddComponent<HudContainer>();
        }

        private static void OnGui(UnityModManager.ModEntry obj) {
            GUILayout.Label("Registered commands: ");
            foreach (Command.Command cmd in CmdHandler.getCommands()) {
                GUILayout.Label("    " + cmd.key());
            }
        }

        private static void OnUpdate(UnityModManager.ModEntry arg1, float delta) {
            if (NetworkServer.active != previousServerState) {
                if (NetworkServer.active) {
                    NetHandler.Start();
                } else {
                    NetHandler.Stop();
                }

                previousServerState = NetworkServer.active;
            }

            if (timeout < 0) {
                return;
            }

            timeout -= delta;
            lastCmd = null;
        }

        private static void ChatOnChatChanged(Console.Log log) {
            string cmd = log.message;

            if (cmd.Contains("ChatChanged")) {
                return;
            }

            if (NetworkServer.active) {
                Mod.Logger.Log("[Server] Chat command invoked on server");
                return;
            }

            while (cmd.Contains("<noparse>")) {
                cmd = cmd.Replace("<noparse>", "");
            }

            while (cmd.Contains("</noparse>")) {
                cmd = cmd.Replace("</noparse>", "");
            }

            if (!cmd.Contains("/")) {
                return;
            }

            cmd = cmd.Split('/')[1];

            while (cmd.Contains("<")) {
                cmd = cmd.Replace("<", "");
            }

            while (cmd.Contains(">")) {
                cmd = cmd.Replace(">", "");
            }

            if (cmd.Equals(lastCmd, StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            Mod.Logger.Log($"[ChatChanged] [{log.logType}] {cmd} (LastCmd: {lastCmd}");

            lastCmd = cmd;
            timeout = .5f;

            CmdHandler.invokeCommand(cmd);
        }

        private static bool Unload(UnityModManager.ModEntry arg) {
            Console.onLogReceived -= ChatOnChatChanged;
            harmony.UnpatchAll();
            return true;
        }

        public static void Log(string msg, bool logToHud = false) {
            Mod.Logger.Log(msg);
            if (logToHud) {
                toHud(msg);
            }
        }

        public static void toHud(string msg) {
            hudContainer.Add(msg);
        }
    }
}