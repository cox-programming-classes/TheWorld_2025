using The_World.GameData.Areas;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

/// <summary>
/// Content sanity: the world graph, the quest chain, and every dialogue
/// tree get checked so a renamed node or missing key can't ship.
/// </summary>
public class WorldTests
{
    private static List<Area> AllAreas()
    {
        var start = WorldBuilder.BuildWorld();
        var seen = new List<Area>();
        var queue = new Queue<Area>();
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            var area = queue.Dequeue();
            if (seen.Contains(area))
                continue;
            seen.Add(area);
            foreach (var next in area.ConnectedAreas.Values)
                queue.Enqueue(next);
        }
        return seen;
    }

    [Fact]
    public void World_HasAtLeastADozenAreas_AllReachableFromStart()
    {
        Assert.True(AllAreas().Count >= 12, $"only {AllAreas().Count} areas reachable");
    }

    [Fact]
    public void EveryPassage_GoesBothWays()
    {
        foreach (var area in AllAreas())
            foreach (var (exitKey, destination) in area.ConnectedAreas)
                Assert.True(destination.ConnectedAreas.Values.Contains(area),
                    $"'{area.Name}' -> '{exitKey}' -> '{destination.Name}' has no way back");
    }

    [Fact]
    public void EveryLockedExit_GuardsARealExit_AndARealItem()
    {
        var itemNames = ItemFactory.Keys.Select(k => ItemFactory.CreateByKey(k).Name).ToHashSet();
        foreach (var area in AllAreas())
            foreach (var (exitKey, exitLock) in area.LockedExits)
            {
                Assert.True(area.ConnectedAreas.ContainsKey(exitKey),
                    $"'{area.Name}' locks nonexistent exit '{exitKey}'");
                Assert.Contains(exitLock.RequiredItemName, itemNames);
            }
    }

    [Fact]
    public void TheBarrowKey_IsObtainable_BeforeItIsNeeded()
    {
        var areas = AllAreas();

        // The key drops from the Goblin Chief...
        var chief = areas.SelectMany(a => a.Creatures.Values)
            .Single(c => c.Name.Contains("Chief"));
        Assert.Contains(chief.Loot, item => item.Name == "Barrow Key");

        // ...and it's what the sanctum gate wants.
        var depths = areas.Single(a => a.Name == "Crypt Depths");
        Assert.Equal("Barrow Key", depths.LockedExits["gate"].RequiredItemName);
    }

    [Fact]
    public void ExactlyOneFinalBoss_AndItIsTheLich()
    {
        var bosses = AllAreas().SelectMany(a => a.Creatures.Values)
            .Where(c => c.IsFinalBoss).ToList();
        var boss = Assert.Single(bosses);
        Assert.Contains("Lich", boss.Name);
        Assert.True(boss.IsHostile);
    }

    [Fact]
    public void HostileCreatures_AreWorthExperience()
    {
        foreach (var creature in AllAreas().SelectMany(a => a.Creatures.Values))
            if (creature.IsHostile)
                Assert.True(creature.XP > 0, $"{creature.Name} is hostile but worth no XP");
    }

    [Fact]
    public void BramsStolenGoods_ExistInTheGoblinDen()
    {
        var den = AllAreas().Single(a => a.Name == "Chief's Den");
        Assert.Contains(den.Items.Values, i => i.Name == "Stolen Goods");
    }

    [Fact]
    public void SafeZones_ContainNoHostiles()
    {
        foreach (var area in AllAreas().Where(a => a.IsSafe))
            Assert.DoesNotContain(area.Creatures.Values, c => c.IsHostile);
    }

    [Fact]
    public void EveryArea_HasAsciiArt_AndRendersItWhenInspected()
    {
        foreach (var area in AllAreas())
        {
            Assert.False(string.IsNullOrWhiteSpace(area.Art), $"{area.Name} has no ASCII art");
            Assert.Contains(area.Art.TrimEnd(), area.Look());
        }
    }

    [Fact]
    public void EveryWorldCreature_HasAsciiArt_AndRendersItWhenInspected()
    {
        foreach (var creature in AllAreas().SelectMany(a => a.Creatures.Values))
        {
            Assert.False(string.IsNullOrWhiteSpace(creature.Art), $"{creature.Name} has no ASCII art");
            Assert.Contains(creature.Art.TrimEnd(), creature.Look());
        }
    }

    [Fact]
    public void NpcInspection_UsesConversationWording_NotMonsterHealthText()
    {
        var elder = NpcFactory.ElderMaera();

        Assert.Contains("willing to talk", elder.Look());
        Assert.DoesNotContain("It looks unharmed", elder.Look());
    }

    public static TheoryData<string> NpcNames() => new(
        "ElderMaera", "MerchantBram", "BarkeepHulda", "FinnTheGambler", "HermitOdo");

    private static Npc CreateNpc(string factoryName) => factoryName switch
    {
        "ElderMaera" => NpcFactory.ElderMaera(),
        "MerchantBram" => NpcFactory.MerchantBram(),
        "BarkeepHulda" => NpcFactory.BarkeepHulda(),
        "FinnTheGambler" => NpcFactory.FinnTheGambler(),
        "HermitOdo" => NpcFactory.HermitOdo(),
        _ => throw new ArgumentException(factoryName)
    };

    [Theory]
    [MemberData(nameof(NpcNames))]
    public void EveryDialogueTree_IsInternallyConsistent(string npcName)
    {
        var npc = CreateNpc(npcName);
        var problems = npc.Dialogue.Validate();
        Assert.True(problems.Count == 0, $"{npc.Name}: {string.Join("; ", problems)}");
        Assert.False(npc.IsHostile, $"{npc.Name} should not be attackable");
    }

    [Theory]
    [MemberData(nameof(NpcNames))]
    public void DialogueEffects_ReferenceRealItemsAndNumbers(string npcName)
    {
        var npc = CreateNpc(npcName);
        foreach (var node in npc.Dialogue.Nodes.Values)
            foreach (var choice in node.Choices)
                foreach (var effect in choice.Effects)
                    switch (effect.Kind)
                    {
                        case DialogueEffectKind.GiveItem:
                            Assert.Contains(effect.Arg, ItemFactory.Keys);
                            break;
                        case DialogueEffectKind.GiveGold:
                        case DialogueEffectKind.HealPlayer when effect.Arg != "":
                            Assert.True(int.TryParse(effect.Arg, out var n) && n >= 0,
                                $"{npc.Name}: {effect.Kind} arg '{effect.Arg}' is not a number");
                            break;
                    }
    }

    [Fact]
    public void MerchantsHaveWares_SoShoppingIsPossible()
    {
        Assert.NotEmpty(NpcFactory.MerchantBram().Wares);
        Assert.NotEmpty(NpcFactory.HermitOdo().Wares);
    }
}
