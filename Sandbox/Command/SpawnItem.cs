using RoR2;

namespace Sandbox.Command {
    public class SpawnItem : SpawnFromEnumCommand<ItemIndex> {
        public override string key() {
            return "spawnItem";
        }

        protected override int toInteger(ItemIndex obj) {
            return (int) obj;
        }

        protected override PickupIndex makeIndex(int idx) {
            return new PickupIndex((ItemIndex) idx);
        }

        protected override ItemIndex getDefault() {
            return ItemIndex.None;
        }
    }
}