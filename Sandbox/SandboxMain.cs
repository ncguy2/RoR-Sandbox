using System;
using Harmony;
using Sandbox.Command;
using Sandbox.Network;
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
        }

        private static void OnGui(UnityModManager.ModEntry obj) {
            GUILayout.Label("Registered commands: ");
            foreach (ICommand cmd in CmdHandler.getCommands()) {
                GUILayout.Label("    " + cmd.key());
            }
        }

        private static void OnUpdate(UnityModManager.ModEntry arg1, float delta) {
            if (NetworkServer.active != previousServerState) {
                if (NetworkServer.active) {
                    NetHandler.start();
                } else {
                    NetHandler.stop();
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

            if (!NetworkServer.active) {
                Mod.Logger.Log("[Server] Chat command invoked on client");
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

        public static void Log(string msg) {
            Mod.Logger.Log(msg);
        }
    }
}