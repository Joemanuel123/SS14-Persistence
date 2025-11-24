using Content.Server.Worldgen.Prototypes;
using Content.Server.Worldgen.Systems.Biomes;
using Robust.Shared.Prototypes;

namespace Content.Server.Worldgen.Components;

[RegisterComponent]
[Access(typeof(BiomeSelectionSystem))]
public sealed partial class PersistenceWorldGenComponent : Component
{
    [DataField]
    public int Seed { get; private set; } = 1337;
}

