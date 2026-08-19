using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Chemistry.Components;

namespace Content.Server.Atmos.Piping.Unary.Components;

/// <summary>
/// Converts ONLY a gas into it's respective reagent. Used for the BaseWaterPurifier
/// </summary>
/// 
/// Inspired by the GasCondenser system
[RegisterComponent]
[Access(typeof(WaterPurifierSystem))]
public sealed partial class WaterPurifierComponent : Component
{
    /// <summary>
    /// The ID for the pipe node.
    /// </summary>
    [DataField]
    public string Inlet = "pipe";

    /// <summary>
    /// The ID for the solution.
    /// </summary>
    [DataField]
    public string SolutionId = "tank";

    /// <summary>
    /// The solution that gas is purified into.
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float GasToReagentPerSecond = 10f; // 10u of reagent added per second

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    /// <summary>
    /// What gas you want to convert into reagent. Check "Content.Shared\Atmos\Atmospherics.cs" for the gas ID
    /// </summary>
    public int GastoCondense = 5; //Water vapor

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Efficiency = 0.9f;

}
