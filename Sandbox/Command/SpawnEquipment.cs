using RoR2;

namespace Sandbox.Command {
    public class SpawnEquipment : SpawnFromEnumCommand<EquipmentIndex> {
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
    }
}