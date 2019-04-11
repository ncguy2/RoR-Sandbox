using UnityEngine;

namespace Sandbox.Utilities {
    public class UnityUtils {
        public static bool RayTrace(out RaycastHit hit) {
            Camera camera = Camera.main;

            if (camera == null) {
                SandboxMain.Log("Unable to find camera reference.");
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

            SandboxMain.Log("Unable to find spawn location.");
            return false;
        }
    }
}