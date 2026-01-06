using Content.Shared.GridControl.Components;
using Content.Shared.GridControl.Systems;
using Robust.Shared.Containers;

namespace Content.Server.GridControl.Systems;

public sealed partial class BluespaceParkingSystem : SharedBluespaceParkingSystem
{
    private void InitializeEvents()
    {
        SubscribeLocalEvent<BSPParkingTargetComponent, MoveEvent>(OnTargetMove);
        SubscribeLocalEvent<BSPAnchorKeyComponent, ComponentStartup>(UpdateUserInterface);
        SubscribeLocalEvent<BSPAnchorKeyComponent, EntInsertedIntoContainerMessage>(UpdateUserInterface);
        SubscribeLocalEvent<BSPAnchorKeyComponent, EntRemovedFromContainerMessage>(UpdateUserInterface);
    }

    private void OnTargetMove(Entity<BSPParkingTargetComponent> ent, ref MoveEvent args)
    {
        CancelRoutine(ent, "Grid has moved.");
    }

}
