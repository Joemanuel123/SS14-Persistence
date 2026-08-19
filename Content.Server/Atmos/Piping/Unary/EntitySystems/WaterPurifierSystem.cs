using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using Content.Shared.Power.EntitySystems;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems;

[UsedImplicitly]
public sealed class WaterPurifierSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;
    [Dependency] private readonly SharedPowerStateSystem _powerState = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WaterPurifierComponent, AtmosDeviceUpdateEvent>(OnWaterPurifierUpdated);
    }
    // This is my first system =]
    // comments are for helping myself inderstand wtf am i doing
    // Inspired by GasCondenserSystem
    private void OnWaterPurifierUpdated(Entity<WaterPurifierComponent> entity, ref AtmosDeviceUpdateEvent args)
    {

        // checks that the machine has power, has an inlet pipe, and a solution container to put water into
        if (!(TryComp<ApcPowerReceiverComponent>(entity, out var receiver) && _power.IsPowered(entity, receiver))
            || !_nodeContainer.TryGetNode(entity.Owner, entity.Comp.Inlet, out PipeNode? inlet)
            || !_solution.ResolveSolution(entity.Owner, entity.Comp.SolutionId, ref entity.Comp.Solution, out var solution))
        {
            _powerState.SetWorkingState(entity.Owner, false);  // I guess I could make a "TryToPurify" check instead of adding all these powerstates
            return;
        }

        // no room in the container or no gas in the pipe code stops
        if (solution.AvailableVolume == 0 || inlet.Air.TotalMoles == 0)
        {
            _powerState.SetWorkingState(entity.Owner, false);
            return;
        }

        // how much water vapor is in the pipe, if it's equal or lower than 0 stops
        var waterMolesAvailable = inlet.Air.GetMoles(Gas.WaterVapor);
        if (waterMolesAvailable <= 0)
        {
            _powerState.SetWorkingState(entity.Owner, false);
            return;
        }

        var gasToReagentPerSecond = entity.Comp.GasToReagentPerSecond;
        var waterMolesToConvert = MathF.Min(gasToReagentPerSecond * args.dt, waterMolesAvailable);
        if (waterMolesToConvert <= 0)
            return;

        // Limits the amount added to the available space in the container
        var amount = FixedPoint2.Min(waterMolesToConvert / 2, solution.AvailableVolume);
        if (amount <= 0)
            return;

        var waterReagent = _atmosphereSystem.GetGas(entity.Comp.GastoCondense).Reagent;
        if (waterReagent is null)
            return;

        // adds the condensed reagent to the chem container
        solution.AddReagent(waterReagent, amount * .9f);

        // moles of gas to remove after adding the water reagent
        inlet.Air.AdjustMoles(entity.Comp.GastoCondense, -waterMolesToConvert * 1.1f);
        _powerState.SetWorkingState(entity.Owner, true);
        _solution.UpdateChemicals(entity.Comp.Solution.Value);
    }

}
