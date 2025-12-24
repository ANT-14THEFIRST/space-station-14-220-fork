using Content.Shared.Blocking;
using Content.Shared.Item.ItemToggle.Components;

namespace Content.Shared.SS220.ItemToggle;

public sealed class ItemToggleBlockingDamageSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<ItemToggleBlockingDamageComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<ItemToggleBlockingDamageComponent, ItemToggledEvent>(OnToggleItem);
    }

    private void OnDecreaseBlock(Entity<ItemToggleBlockingDamageComponent> ent, BlockingComponent blockingComponent)
    {


        Dirty(ent);
    }

    private void OnMapInit(Entity<ItemToggleBlockingDamageComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<BlockingComponent>(ent.Owner, out var blockingComponent))
        {
            return;
        }

        //ent.Comp.OriginalActiveModifier = blockingComponent.ActiveBlockDamageModifier;
        OnDecreaseBlock(ent, blockingComponent);
    }

    private void OnToggleItem(Entity<ItemToggleBlockingDamageComponent> ent, ref ItemToggledEvent args)
    {
        if (!TryComp<BlockingComponent>(ent.Owner, out var blockingComponent))
            return;

        if (args.Activated)
        {
            Dirty(ent);
        }
        else
        {
            OnDecreaseBlock(ent, blockingComponent);
        }
    }
}
