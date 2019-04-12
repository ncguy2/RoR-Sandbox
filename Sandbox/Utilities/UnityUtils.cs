using System;
using RoR2;
using UnityEngine;

namespace Sandbox.Utilities {
    public class UnityUtils {
        public static bool RayTrace(out RaycastHit hit) {
            Camera camera = Camera.main;

            if (camera == null) {
                SandboxMain.Log("Unable to find camera reference.", true);
                hit = new RaycastHit();
                return false;
            }

            Transform transform = camera.transform;
            Ray ray = new Ray {
                origin = transform.position,
                direction = transform.forward
            };

            if (Physics.Raycast(ray, out hit)) {
                return true;
            }

            SandboxMain.Log("Unable to find trace location.", true);
            return false;
        }

        public static PlayerCharacterMasterController GetLocalPlayerController() {
            return GetPlayerController(x => x.isLocalPlayer);
        }

        public static PlayerCharacterMasterController GetPlayerControllerByDisplayName(string name) {
            return GetPlayerController(
                x => x.GetDisplayName().Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static PlayerCharacterMasterController GetPlayerController(
            Predicate<PlayerCharacterMasterController> filter) {
            foreach (PlayerCharacterMasterController ctrlr in PlayerCharacterMasterController.instances) {
                SandboxMain.debug($"Player controller: {ctrlr.GetDisplayName()}, isLocal: {ctrlr.isLocalPlayer}");
                if (filter(ctrlr)) {
                    return ctrlr;
                }
            }

            return null;
        }

        public static NetworkUser GetNetworkUser(Predicate<NetworkUser> filter) {
            foreach (NetworkUser user in NetworkUser.readOnlyLocalPlayersList) {
                if (filter(user)) {
                    return user;
                }
            }

            return null;
        }

        public static NetworkUser GetNetworkUserWithName(string name) {
            return GetNetworkUser(x => GetNetworkUserName(x).Equals(name));
        }

        public static NetworkUser GetLocalNetworkUser() {
            return GetNetworkUser(x => x.isLocalPlayer);
        }

        public static string GetLocalNetworkUserName() {
            return GetNetworkUserName(GetLocalNetworkUser());
        }

        public static string GetNetworkUserName(NetworkUser user) {
            return user.userName;
        }
    }
}