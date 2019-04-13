using System;
using System.Text.RegularExpressions;
using Harmony;
using RoR2;
using Sandbox.Command;
using Sandbox.Network;
using Sandbox.UI;
using Sandbox.Utilities;
using UnityEngine;
using UnityEngine.Networking;
using UnityModManagerNet;
using Console = RoR2.Console;

// ReSharper disable UnusedMember.Global
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global

namespace Sandbox {
    [EnableReloading]
    public static class SandboxMain {
        private static float _timeout;
        private static string _lastCmd;
        private static bool _previousServerState;
        private static HudContainer _hudContainer;
        private static HarmonyInstance _harmony;

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

            _harmony = HarmonyInstance.Create(entry.Info.Id);
            _harmony.PatchAll();

            Console.onLogReceived += ChatOnChatChanged;

            GameObject gameObject = new GameObject();
            gameObject.AddComponent<SetDontDestroyOnLoad>();
            _hudContainer = gameObject.AddComponent<HudContainer>();
        }

        private static void OnGui(UnityModManager.ModEntry obj) {
            GUILayout.Label("Registered commands: ");
            foreach (Command.Command cmd in CmdHandler.GetCommands()) GUILayout.Label("    " + cmd.Key());
        }

        private static void OnUpdate(UnityModManager.ModEntry arg1, float delta) {
            if (NetworkServer.active != _previousServerState) {
                if (NetworkServer.active)
                    NetHandler.StartServer();
                else
                    NetHandler.StopServer();

                _previousServerState = NetworkServer.active;
            }

            if (_timeout < 0) return;

            _timeout -= delta;
            _lastCmd = null;
        }

        private static void ChatOnChatChanged(Console.Log log) {
            string cmd = log.message;
            Debug(cmd);

            if (!GetDisplayNameFromLine(cmd, out string name)) {
                Debug("Unable to find name from command");
                return;
            }

            Debug("Fetching local player");
            string localPlayerName = UnityUtils.GetLocalNetworkUserName();
            Debug($"Local Player: {localPlayerName}, invoker: {name}");
            if (!localPlayerName.Equals(name)) return;

            if (cmd.Contains("ChatChanged")) return;

            while (cmd.Contains("<noparse>")) cmd = cmd.Replace("<noparse>", "");

            while (cmd.Contains("</noparse>")) cmd = cmd.Replace("</noparse>", "");

            if (!cmd.Contains("/")) return;

            cmd = cmd.Split('/')[1];

            while (cmd.Contains("<")) cmd = cmd.Replace("<", "");

            while (cmd.Contains(">")) cmd = cmd.Replace(">", "");

            if (cmd.Equals(_lastCmd, StringComparison.OrdinalIgnoreCase)) return;

            Mod.Logger.Log($"[ChatChanged] [{log.logType}] {cmd} (LastCmd: {_lastCmd}");
            Debug(cmd);

            _lastCmd = cmd;
            _timeout = .5f;

            CmdHandler.InvokeCommand(cmd);
        }

        private static bool GetDisplayNameFromLine(string line, out string displayName) {
            if (line.Contains("noparse")) {
                Debug($"Potential command: {line}");
                Regex rx = new Regex(@"<noparse>([^<]*)</noparse>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = rx.Matches(line);

                if (matches.Count >= 1) {
                    Debug("Match found");
                    Match match = matches[0];
                    Debug($"Groups: {match.Groups.Count}");
                    Group group = match.Groups[1];
                    Debug($"Group: {group.Value}");
                    displayName = group.Value;
                    return true;
                }

                Debug("No matches");
            }

            displayName = "";
            return false;
        }

        private static bool Unload(UnityModManager.ModEntry arg) {
            Console.onLogReceived -= ChatOnChatChanged;
            _harmony.UnpatchAll();
            return true;
        }

        public static void Log(string msg, bool logToHud = false) {
            Mod.Logger.Log(msg);
            if (logToHud) ToHud(msg);
        }

        public static void ToHud(string msg) {
            _hudContainer.Add(msg);
        }

        public static void Debug(string msg) {
            #if DEBUG
            ToHud(msg);
            #endif
        }
    }
}