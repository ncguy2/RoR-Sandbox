using RoR2;
using Sandbox.Command.Attribute;

namespace Sandbox.Command {
    [SandboxCommand]
    public class SpawnItemCommand : SpawnFromEnumCommand<ItemIndex> {
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

        protected override bool areEqual(ItemIndex a, ItemIndex b) {
            return a == b;
        }
    }
}