using Harmony;
using RoR2;
using Sandbox.Command;

namespace Sandbox.Overrides {
    [HarmonyPatch(typeof(GenericPickupController))]
    [HarmonyPatch("GrantItem")]
    public class ItemSharing {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedParameter.Local
        private static void Postfix(CharacterBody body, Inventory inventory, GenericPickupController __instance) {
            if (!SetShared.itemsShared) {
                return;
            }

            foreach (PlayerCharacterMasterController playerCharacterMasterController in PlayerCharacterMasterController
                .instances) {
                if (!playerCharacterMasterController.master.alive) {
                    continue;
                }


                CharacterBody characterBody = playerCharacterMasterController.master.GetBody();
                if (characterBody.Equals(body)) {
                    return;
                }

                characterBody.inventory.GiveItem(__instance.pickupIndex.itemIndex);
            }
        }
    }
}