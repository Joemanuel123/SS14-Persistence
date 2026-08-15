using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Chemistry.Components;

namespace Content.Server.Atmos.Piping.Unary.Components;

/// <summary>
/// Only converts water vapor into water reagent. Used for the water purifier 
/// </summary>
/// 
/// Pretty much a copy-paste of GasCondenser
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
    /// The solution that water vapor is condensed into.
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    /// <summary>
    /// For a water purifier, how many U of water are given per each mole of water vapor.
    /// </summary>
    /// Derived from a standard of 500u per canister:
    /// 400u / 1871.71051 moles per canister
    /// </remarks>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MolesToReagentMultiplier = 0.2137f;
}
