using Harmony;
using RoR2;
using Sandbox.Command;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local

namespace Sandbox.Overrides {
    [HarmonyPatch(typeof(GenericPickupController))]
    [HarmonyPatch("GrantItem")]
    public class ItemSharing {
        private static void Postfix(CharacterBody body, Inventory inventory, GenericPickupController __instance) {
            if (!SetSharedCommand.ItemsShared) {
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