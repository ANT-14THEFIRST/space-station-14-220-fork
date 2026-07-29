// © SS220, MIT full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/MIT_LICENSE.TXT

using Content.Shared.Access.Components;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Damage;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.FixedPoint;
using Content.Shared.Flash;
using Content.Shared.Flash.Components;
using Content.Shared.Gravity;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mech;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.EntitySystems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Radio.Components;
using Content.Shared.Random.Helpers;
using Content.Shared.SprayPainter.Components;
using Content.Shared.SS220.AltBlocking;
using Content.Shared.SS220.ArmorBlock;
using Content.Shared.SS220.Weapons.Melee.Events;
using Content.Shared.SS220.Weapons.Ranged.Events;
using Content.Shared.Standing;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Content.Shared.Storage.Components;
using Content.Shared.Weapons.Melee;
using Content.Shared.Whitelist;
using Content.Shared.Wires;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System.Net.NetworkInformation;

namespace Content.Shared.SS220.SiliconComponents;

public sealed partial class SiliconComponentsSystem : EntitySystem
{
    [Dependency] private SharedContainerSystem _container = default!;
    [Dependency] private SharedPopupSystem _popup = default!;
    [Dependency] private SharedDoAfterSystem _doAfter = default!;

    public EntProtoId PilotEjectAction = "ActionMechEject";
    public EntProtoId MechUIOpenAction = "ActionMechOpenUI";
    public EntProtoId CombatModeToggleAction = "ActionCombatModeToggle";
    public EntProtoId MechRelayAction = "ActionMechRelay";

    private static readonly LocId MechArmTooHeavy = "mech-arm-too-heavy";

    private static readonly string PartContainerPrefix = "silicon_component";

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<SiliconComponentsComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<SiliconComponentsComponent, AfterInteractUsingEvent>(OnSiliconInteractedWith);
    }

    private void OnComponentStartup(Entity<SiliconComponentsComponent> ent, ref ComponentStartup args)
    {
        if (!TryComp<ContainerManagerComponent>(ent.Owner, out var containerManager))
            return;

        foreach (PartType part in Enum.GetValues(typeof(PartType)))
        {
            if (ent.Comp.Parts.ContainsKey(part))
                continue;

            ent.Comp.Parts.Add(part, _container.EnsureContainer<ContainerSlot>(ent.Owner, PartContainerPrefix + "_" + Enum.GetName(part), containerManager));
        }
    }

    private void OnSiliconInteractedWith(Entity<SiliconComponentsComponent> ent, ref AfterInteractUsingEvent args)
    {
        if (args.Handled || !args.CanReach || args.Target == null)
            return;

        var used = args.Used;

        if (!TryComp<SiliconPartComponent>(ent.Owner, out var partComp))
            return;

        if (partComp.OccupiedSpace > ent.Comp.ModuleSpace)
        {
            _popup.PopupEntity(Loc.GetString("silicon-component-not-enough-space"), args.User);
            return;
        }

        if (TryComp<WiresPanelComponent>(ent.Owner, out var panelComp) && !panelComp.Open)
            return;

        if (!ent.Comp.Parts.TryGetValue(partComp.Type, out var container))
            return;

        if (container.ContainedEntity != null)
        {
            _popup.PopupEntity(Loc.GetString("silicon-component-slot-occupied"), args.User);
            return;
        }

        _popup.PopupEntity(Loc.GetString("silicon-component-begin-install", ("item", args.Used)), ent.Owner);

        var doAfterEventArgs = new DoAfterArgs(EntityManager, args.User, partComp.TimeToInstall, new InstallSiliconPartEvent(), ent.Owner, target: ent.Owner, used: args.Used)
        {
            BreakOnMove = true,
        };

        _doAfter.TryStartDoAfter(doAfterEventArgs);
        args.Handled = true;
    }
}

[Serializable, NetSerializable]
public sealed partial class InstallSiliconPartEvent : SimpleDoAfterEvent
{
    public PartType Slot;

    public InstallSiliconPartEvent(PartType slot)
    {
        Slot = slot;
    }
}

