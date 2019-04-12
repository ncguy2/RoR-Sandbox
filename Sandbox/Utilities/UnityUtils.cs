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
                if (filter(ctrlr)) {
                    return ctrlr;
                }
            }

            return null;
        }
    }
}