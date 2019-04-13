using RoR2;
using Sandbox.Command.Attribute;

namespace Sandbox.Command {
    [SandboxCommand]
    public class SpawnItemCommand : SpawnFromEnumCommand<ItemIndex> {
        public override string Key() {
            return "spawnItem";
        }

        protected override int ToInteger(ItemIndex obj) {
            return (int) obj;
        }

        protected override PickupIndex MakeIndex(int idx) {
            return new PickupIndex((ItemIndex) idx);
        }

        protected override ItemIndex GetDefault() {
            return ItemIndex.None;
        }

        protected override bool AreEqual(ItemIndex a, ItemIndex b) {
            return a == b;
        }
    }
}