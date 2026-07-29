// © SS220, MIT full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/MIT_LICENSE.TXT

using Content.Shared.SS220.SiliconComponents;
using JetBrains.Annotations;
using Robust.Client.UserInterface;

namespace Content.Client.SS220.Silicon.UI;

[UsedImplicitly]
public sealed class SiliconBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private SiliconComponentsMenu? _menu;

    public SiliconBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _menu = this.CreateWindow<SiliconComponentsMenu>();
        _menu.SetEntity(Owner);

        _menu.BrainTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Brain));
        };

        _menu.OpticsTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Optics));
        };

        _menu.ServoTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Servo));
        };

        _menu.SpineTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Spine));
        };

        _menu.DriveTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectPartBuiMessage(PartType.Drive));
        };

        _menu.EjectBatteryTextureButtonPressed += () =>
        {
            SendPredictedMessage(new SiliconEjectBatteryBuiMessage());
        };

        _menu.RemoveModuleTextureButtonPressed += module =>
        {
            SendPredictedMessage(new SiliconRemoveModuleBuiMessage(EntMan.GetNetEntity(module)));
        };
    }

    public override void Update()
    {
        _menu?.UpdateBatteryTextureButton();
        _menu?.UpdateTextureButtons();
        _menu?.UpdateModulePanel();
    }
}
