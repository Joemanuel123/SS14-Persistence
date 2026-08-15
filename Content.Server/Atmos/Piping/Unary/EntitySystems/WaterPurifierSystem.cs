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
using JetBrains.Annotations;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems;

[UsedImplicitly]
public sealed class WaterPurifierSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<WaterPurifierComponent, AtmosDeviceUpdateEvent>(OnWaterPurifierUpdated);
    }
    // comments are for helping myself inderstand wtf am i doing
    // alsoo, pretty much copy-pasted GasCondenserSystem
    private void OnWaterPurifierUpdated(Entity<WaterPurifierComponent> entity, ref AtmosDeviceUpdateEvent args)
    {

        // checks that the machine has power, has an inlet pipe, and a solution container to put water into
        if (!(TryComp<ApcPowerReceiverComponent>(entity, out var receiver) && _power.IsPowered(entity, receiver))
            || !_nodeContainer.TryGetNode(entity.Owner, entity.Comp.Inlet, out PipeNode? inlet)
            || !_solution.ResolveSolution(entity.Owner, entity.Comp.SolutionId, ref entity.Comp.Solution, out var solution))
        {
            return;
        }

        // no room in the container or no gas in the pipe code stops
        if (solution.AvailableVolume == 0 || inlet.Air.TotalMoles == 0)
            return;

        // how much water vapor is in the pipe, if it's equal or lower than 0 stops
        var waterMolesAvailable = inlet.Air.GetMoles(Gas.WaterVapor);
        if (waterMolesAvailable <= 0)
            return;

        // If the gas has no reagent equivalent, it stops
        if (_atmosphereSystem.GetGas(Gas.WaterVapor).Reagent is not { } waterReagent)
            return;

        // If I understand this correctly, GasMixture makes a lil false gas container, same vol and temp of the inlet pipe, but with only the waper vapor
        var waterMix = new GasMixture(inlet.Air.Volume) { Temperature = inlet.Air.Temperature };
        waterMix.SetMoles(Gas.WaterVapor, waterMolesAvailable);

        // moles of water vapor can be converted this game-tick
        var waterMolesToConvert = NumberOfMolesToConvert(receiver, waterMix, args.dt);

        // safety check to never condense more water vapor than the amount in the pipes
        var checkwaterMolesToConvert = MathF.Min(waterMolesToConvert, waterMolesAvailable);
        if (checkwaterMolesToConvert <= 0)
            return;

        // how much water reagent units the water vapor becomes
        var moleToReagentMultiplier = entity.Comp.MolesToReagentMultiplier;

        // Limits the amount added to the available space in the chem container
        var amount = FixedPoint2.Min(FixedPoint2.New(checkwaterMolesToConvert * moleToReagentMultiplier), solution.AvailableVolume);
        if (amount <= 0)
            return;

        // adds the condensed water to the chem container
        solution.AddReagent(waterReagent, amount);

        // moles of water vaporto remove after adding the water reagent
        inlet.Air.AdjustMoles(Gas.WaterVapor, -checkwaterMolesToConvert + (amount.Float() / moleToReagentMultiplier));


        _solution.UpdateChemicals(entity.Comp.Solution.Value);
    }

    public float NumberOfMolesToConvert(ApcPowerReceiverComponent comp, GasMixture mix, float dt)
    {
        var hc = _atmosphereSystem.GetHeatCapacity(mix, true);
        var alpha = 0.8f;
        var energy = comp.Load * dt;
        return energy / (alpha * hc);
    }
}
