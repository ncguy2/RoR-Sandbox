using RoR2;
using Sandbox.Command.Attribute;

namespace Sandbox.Command {
    [SandboxCommand]
    public class SpawnEquipmentCommand : SpawnFromEnumCommand<EquipmentIndex> {
        public override string Key() {
            return "spawnEquipment";
        }

        protected override int ToInteger(EquipmentIndex obj) {
            return (int) obj;
        }

        protected override PickupIndex MakeIndex(int idx) {
            return new PickupIndex((EquipmentIndex) idx);
        }

        protected override EquipmentIndex GetDefault() {
            return EquipmentIndex.None;
        }

        protected override bool AreEqual(EquipmentIndex a, EquipmentIndex b) {
            return a == b;
        }
    }
}