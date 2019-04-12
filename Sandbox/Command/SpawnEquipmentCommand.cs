using RoR2;
using Sandbox.Command.Attribute;

namespace Sandbox.Command {
    [SandboxCommand]
    public class SpawnEquipmentCommand : SpawnFromEnumCommand<EquipmentIndex> {
        public override string key() {
            return "spawnEquipment";
        }

        protected override int toInteger(EquipmentIndex obj) {
            return (int) obj;
        }

        protected override PickupIndex makeIndex(int idx) {
            return new PickupIndex((EquipmentIndex) idx);
        }

        protected override EquipmentIndex getDefault() {
            return EquipmentIndex.None;
        }

        protected override bool areEqual(EquipmentIndex a, EquipmentIndex b) {
            return a == b;
        }
    }
}