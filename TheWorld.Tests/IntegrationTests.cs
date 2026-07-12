using The_World.Engine;
using The_World.Engine.Commands;
using The_World.Engine.States;
using The_World.GameData;
using The_World.GameData.Areas;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

/// <summary>
/// Whole-engine runs driven by scripted console input. The engine exits
/// gracefully when the script runs out (end-of-input = quit).
/// </summary>
public class EngineIntegrationTests
{
    private static ScriptedIO RunGame(int seed, params string[] lines)
    {
        var io = new ScriptedIO(lines);
        new GameEngine(io, seed).Run();
        return io;
    }

    [Fact]
    public void FullFlow_CreateCharacter_Explore_Talk_AndQuit()
    {
        var io = RunGame(42,
            "Hero", "1", "y",          // creation: warrior, accept stats
            "look",
            "talk elder", "1", "1", "1", // quest chain: trouble -> accept -> farewell
            "journal",
            "east",                     // Dark Forest
            "get rusty",                // pick up the rusty sword
            "west",                     // back to the village
            "quit", "y");

        Assert.Contains("Welcome, Hero the Warrior", io.Output);
        Assert.Contains("Willowbrook Village", io.Output);
        Assert.Contains("destroy this Lich", io.Output);
        Assert.Contains("Journal", io.Output);
        Assert.Contains("Dark Forest", io.Output);
        Assert.Contains("You take the Rusty Sword", io.Output);
        Assert.Contains("Thanks for playing!", io.Output);
    }

    [Fact]
    public void CharacterCreation_RejectsSillyClass_AndAllowsReroll()
    {
        var io = RunGame(1,
            "Rerolla", "paladin", "mage", "n", "y",
            "stats");

        Assert.Contains("not a path anyone has walked", io.Output);
        Assert.Contains("cast anew", io.Output);
        Assert.Contains("Level 1 Mage", io.Output);
    }

    [Fact]
    public void Shopping_BuyAndSell_MovesGoldAndGoods()
    {
        var io = RunGame(3,
            "Shopper", "1", "y",
            "talk bram", "1",          // "Show me your wares" -> shop
            "buy healing potion",
            "sell healing potion",     // sell one right back at half price
            "leave",                   // shop -> dialogue
            "5",                       // "Just browsing. Goodbye."
            "quit", "y");

        Assert.Contains("You buy the Healing Potion for 15 gold", io.Output);
        Assert.Contains("You sell the Healing Potion for 7 gold", io.Output);
    }

    [Fact]
    public void UnknownCommands_DoNotCrashAnyState()
    {
        var io = RunGame(9,
            "Fumbler", "3", "y",
            "frobnicate", "look at the sky", "go nowhere", "attack air",
            "talk elder", "99", "banana", "leave",
            "quit", "y");

        Assert.Contains("Unknown command", io.Output);
        Assert.Contains("Thanks for playing!", io.Output);
    }
}

/// <summary>
/// Driving states directly for scenarios the whole-engine tests can't
/// reach deterministically (combat outcomes, victory, defeat).
/// </summary>
public class CombatScenarioTests
{
    private static (GameContext ctx, ScriptedIO io, CommandProcessor processor) BattleContext(
        Player player, Creature enemy, int seed = 42)
    {
        var io = new ScriptedIO();
        var ctx = new GameContext(io, new Random(seed)) { Player = player };
        var arena = AreaBuilder.FromName("Test Arena")
            .WithDescription("Sand, chalk lines, and consequences.")
            .WithCreature("enemy", enemy)
            .Build();
        ctx.CurrentArea = arena;
        ctx.States.Push(ctx, new ExploringState());
        ctx.States.Push(ctx, new CombatState("enemy", enemy));
        return (ctx, io, new CommandProcessor());
    }

    private static Player Champion() =>
        Player.CreateNewPlayer("Champion", PlayerClass.Warrior,
            new StatChart(500, 50, Strength: 30, Dexterity: 20, Intelligence: 10));

    private static Player Weakling() =>
        Player.CreateNewPlayer("Weakling", PlayerClass.Warrior,
            new StatChart(1, 0, Strength: 1, Dexterity: 1, Intelligence: 1));

    [Fact]
    public void SlayingTheFinalBoss_LeadsToVictory()
    {
        var player = Champion();
        player.Equipment.Equip(ItemFactory.SteelGreatsword());
        var lich = CreatureFactory.LichMalakhar();
        var (ctx, io, processor) = BattleContext(player, lich);

        for (var i = 0; i < 200 && lich.Stats.IsAlive && ctx.Player.Stats.IsAlive; i++)
            processor.Process(ctx, "attack");

        Assert.False(lich.Stats.IsAlive, "the champion should overwhelm the lich eventually");
        Assert.IsType<VictoryState>(ctx.States.Current);
        Assert.True(ctx.HasFlag(NpcFactory.FlagLichSlain));
        Assert.Contains("LICH", io.Output);
        Assert.DoesNotContain("enemy", ctx.CurrentArea.Creatures.Keys);
    }

    [Fact]
    public void SlayingARegularFoe_PopsBackToExploring_AndPaysOut()
    {
        var player = Champion();
        player.Equipment.Equip(ItemFactory.SteelGreatsword());
        var goblin = CreatureFactory.BuildGoblinArchetype();
        var (ctx, io, processor) = BattleContext(player, goblin);
        var goldBefore = player.Gold;

        for (var i = 0; i < 100 && goblin.Stats.IsAlive; i++)
            processor.Process(ctx, "attack");

        Assert.False(goblin.Stats.IsAlive);
        Assert.IsType<ExploringState>(ctx.States.Current);
        Assert.True(player.Gold > goldBefore, "goblins carry pocket change");
        Assert.Contains("experience", io.Output);
        Assert.Empty(ctx.CurrentArea.Creatures);
    }

    [Fact]
    public void DyingInCombat_EndsInGameOver_AndNewgameRestarts()
    {
        var (ctx, io, processor) = BattleContext(Weakling(), CreatureFactory.StoneGolem());

        for (var i = 0; i < 100 && ctx.Player is not null && ctx.Player.Stats.IsAlive
             && ctx.States.Current is CombatState; i++)
            processor.Process(ctx, "attack");

        Assert.IsType<GameOverState>(ctx.States.Current);
        Assert.Contains("YOU  HAVE  DIED", io.Output);
        Assert.True(ctx.IsRunning, "death ends the hero, not the program");

        processor.Process(ctx, "newgame");
        Assert.IsType<CharacterCreationState>(ctx.States.Current);
    }

    [Fact]
    public void Fleeing_EventuallyWorks_ForTheNimble()
    {
        var player = Champion(); // DEX 20 vs golem DEX 6: escape odds are excellent
        var (ctx, _, processor) = BattleContext(player, CreatureFactory.StoneGolem());

        for (var i = 0; i < 50 && ctx.States.Current is CombatState; i++)
            processor.Process(ctx, "flee");

        Assert.IsType<ExploringState>(ctx.States.Current);
        Assert.True(player.Stats.IsAlive);
    }

    [Fact]
    public void CastingWithoutMana_DoesNotConsumeTheTurn()
    {
        var player = Player.CreateNewPlayer("Dry", PlayerClass.Mage,
            new StatChart(100, 0, Intelligence: 18)); // zero mana
        var wolf = CreatureFactory.Wolf();
        var (ctx, io, processor) = BattleContext(player, wolf, seed: 7);
        var healthAfterEntry = player.Stats.Health; // initiative may cost a hit

        processor.Process(ctx, "cast firebolt");

        Assert.Contains("Not enough mana", io.Output);
        Assert.Equal(healthAfterEntry, player.Stats.Health); // no free enemy swing
        Assert.Equal(wolf.Stats.MaxHealth, wolf.Stats.Health);
    }
}
