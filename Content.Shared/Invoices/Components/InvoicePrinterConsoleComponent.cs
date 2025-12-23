using Content.Shared.Access.Systems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.CrewAssignments.Components;
using Content.Shared.CrewRecords.Components;
using Content.Shared.Invoices.Systems;
using Content.Shared.Roles;
using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Invoices.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedInvoicePrinterConsoleSystem))]
public sealed partial class InvoicePrinterConsoleComponent : Component
{
    public static string PrivilegedIdCardSlotId = "InvoicePrinter-privilegedId";

    [DataField]
    public ItemSlot PrivilegedIdSlot = new();

    [DataField]
    public bool StationMode = true;

    /// <summary>
    /// The sound made when printing occurs
    /// </summary>
    [DataField]
    public SoundSpecifier PrintSound = new SoundCollectionSpecifier("PrinterPrint");

    [DataField]
    public int SelectedStation = 0;

}
[Serializable, NetSerializable]
public sealed class PrintInvoice : BoundUserInterfaceMessage
{

    public string InvoiceReason = "";
    public int InvoiceCost = 0;

    public PrintInvoice(string invoiceReason, int invoiceCost)
    {
        InvoiceReason = invoiceReason;
        InvoiceCost = invoiceCost;
    }
}

[Serializable, NetSerializable]
public sealed class ChangeInvoiceMode : BoundUserInterfaceMessage
{

    public ChangeInvoiceMode()
    {
    }
}

[Serializable, NetSerializable]
public sealed class InvoicePrinterStationSelectMessage : BoundUserInterfaceMessage
{
    public int Target;
    public InvoicePrinterStationSelectMessage(int target)
    {
        Target = target;
    }

}


[Serializable, NetSerializable]
public sealed class InvoicePrinterConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
    public bool IdPresent = false;
    public string? TargetName = null;
    public string? IdName = null;
    public bool StationMode = true;
    public int TaxRate = 0;
    public int TaxingStation;

    public string TaxingName;

    public Dictionary<int, string> FormattedStations;

    public int SelectedStation;

    public string SelectedName;
    public InvoicePrinterConsoleBoundUserInterfaceState(bool idpresent, string? idname, string? targetname, bool stationMode, int taxRate, int taxingStation, string taxingName, int selectedStation, Dictionary<int, string> formattedStations, string selectedName)
    {
        IdPresent = idpresent;
        TargetName = targetname;
        IdName = idname;
        StationMode = stationMode;
        TaxRate = taxRate;
        TaxingStation = taxingStation;
        TaxingName = taxingName;
        FormattedStations = formattedStations;
        SelectedStation = selectedStation;
        SelectedName = selectedName;
    }
}

[Serializable, NetSerializable]
public enum InvoicePrinterConsoleUiKey : byte
{
    Key,
}
