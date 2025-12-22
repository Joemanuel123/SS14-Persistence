using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.GridControl.Components;
using JetBrains.Annotations;
using Robust.Shared.Serialization;

namespace Content.Shared.GridControl.Systems;

[UsedImplicitly]
public abstract partial class SharedBluespaceParkingSystem : EntitySystem
{
    [Dependency] protected readonly ItemSlotsSystem _itemSlotsSystem = default!;
    [Dependency] protected readonly ILogManager _log = default!;

    public const string Sawmill = "BluespaceParking";
    protected ISawmill _sawmill = default!;

    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _log.GetSawmill(Sawmill);

        SubscribeLocalEvent<BSPAnchorKeyComponent, ComponentRemove>(OnComponentRemove);
        SubscribeLocalEvent<BSPAnchorKeyComponent, ComponentInit>(OnComponentInit);
    }


    private void OnComponentInit(EntityUid uid, BSPAnchorKeyComponent component, ComponentInit args)
    {
        _itemSlotsSystem.AddItemSlot(uid, BSPAnchorKeyComponent.PrivilegedIdCardSlotId, component.PrivilegedIdSlot);
    }

    private void OnComponentRemove(EntityUid uid, BSPAnchorKeyComponent component, ComponentRemove args)
    {
        _itemSlotsSystem.RemoveItemSlot(uid, component.PrivilegedIdSlot);
    }

    [Serializable, NetSerializable]
    public sealed partial class BSPAnchorKeyDoAfterEvent : DoAfterEvent
    {
        public BSPAnchorKeyDoAfterEvent()
        {
        }

        public override DoAfterEvent Clone() => this;
    }

}
