// © SS220, MIT full text: https://raw.githubusercontent.com/SerbiaStrong-220/space-station-14/master/MIT_LICENSE.TXT

namespace Content.Shared.SS220.SiliconComponents;

[RegisterComponent]
public sealed partial class SiliconPartComponent : Component //Yeah-yeah the naming is pretty messed up
{
    [DataField]
    public PartType Type;

    [DataField]
    public int OccupiedSpace;

    [DataField]
    public TimeSpan TimeToInstall = new TimeSpan(0, 0, 5);
}
