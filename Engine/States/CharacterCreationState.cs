using The_World.GameData;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace The_World.Engine.States;

/// <summary>
/// The opening state: name, class, and dice-rolled ability scores
/// (4d6 drop lowest, the traditional way). Entirely raw-input driven -
/// there are no commands here, just questions.
/// </summary>
public class CharacterCreationState : GameStateBase
{
    private enum Step { Name, ClassChoice, Confirm }

    private Step _step = Step.Name;
    private string _name = "";
    private PlayerClass? _class;
    private (int Str, int Dex, int Int) _rolled;

    public override string Name => "Creating a Character";

    public override string GetPrompt(GameContext ctx) => _step switch
    {
        Step.Name => "Name > ",
        Step.ClassChoice => "Class > ",
        _ => "(y/n) > "
    };

    public override void OnEnter(GameContext ctx)
    {
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("Mist parts. Somewhere a road unrolls beneath your boots,", ConsoleColor.DarkCyan);
        ctx.IO.WriteLine("and The World waits to learn who is walking it.", ConsoleColor.DarkCyan);
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("What is your name, adventurer?");
    }

    public override bool TryHandleRaw(GameContext ctx, string input)
    {
        switch (_step)
        {
            case Step.Name:
                HandleName(ctx, input);
                return true;

            case Step.ClassChoice:
                if (IsQuitWord(input))
                    return false; // let the global quit command have it
                HandleClassChoice(ctx, input);
                return true;

            case Step.Confirm:
                if (IsQuitWord(input))
                    return false;
                HandleConfirm(ctx, input);
                return true;

            default:
                return false;
        }
    }

    private static bool IsQuitWord(string input) =>
        input.Equals("quit", StringComparison.OrdinalIgnoreCase)
        || input.Equals("exit", StringComparison.OrdinalIgnoreCase);

    private void HandleName(GameContext ctx, string input)
    {
        // Trim whitespace and any BOM/zero-width stowaways from piped input.
        _name = input.Trim().Trim('\uFEFF', '\u200B');
        _step = Step.ClassChoice;

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"Well met, {(_name.Length == 0 ? "stranger" : _name)}. And what manner of hero are you?");
        ctx.IO.WriteLine();
        for (var i = 0; i < PlayerClass.All.Count; i++)
        {
            var c = PlayerClass.All[i];
            ctx.IO.WriteLine($"  {i + 1}. {c.Name,-8} - {c.Description}", ConsoleColor.Yellow);
            foreach (var ability in c.Abilities)
                ctx.IO.WriteLine($"       * {ability.Name}: {ability.Description}");
        }
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("Choose by number or name.");
    }

    private void HandleClassChoice(GameContext ctx, string input)
    {
        var choice = input.Trim();
        _class = int.TryParse(choice, out var index) && index >= 1 && index <= PlayerClass.All.Count
            ? PlayerClass.All[index - 1]
            : PlayerClass.FindByName(choice);

        if (_class is null)
        {
            ctx.IO.WriteLine($"'{choice}' is not a path anyone has walked. Choose 1-{PlayerClass.All.Count}, or type a class name.");
            return;
        }

        RollStats(ctx);
    }

    private void RollStats(GameContext ctx)
    {
        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"A {_class!.Name}, then. The dice will judge the rest of you.", ConsoleColor.DarkCyan);

        _rolled = (RollScore(ctx), RollScore(ctx), RollScore(ctx));

        // Your calling sharpens its favored talent.
        _rolled = _class.PrimaryStat switch
        {
            GameData.Abilities.AbilityScaling.Strength => (_rolled.Str + 2, _rolled.Dex, _rolled.Int),
            GameData.Abilities.AbilityScaling.Dexterity => (_rolled.Str, _rolled.Dex + 2, _rolled.Int),
            GameData.Abilities.AbilityScaling.Intelligence => (_rolled.Str, _rolled.Dex, _rolled.Int + 2),
            _ => _rolled
        };

        var stats = BuildStats();
        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"  STR {_rolled.Str}   DEX {_rolled.Dex}   INT {_rolled.Int}", ConsoleColor.Yellow);
        ctx.IO.WriteLine($"  Health {stats.MaxHealth}   Mana {stats.MaxMana}", ConsoleColor.Yellow);
        ctx.IO.WriteLine();
        ctx.IO.Write("Keep these scores? (y to accept, n to reroll) ");
        _step = Step.Confirm;
    }

    /// <summary>4d6, drop the lowest - fortune favors the rolled.</summary>
    private int RollScore(GameContext ctx)
    {
        var result = new Dice(4, 6).RollDetailed(ctx.Rng);
        return result.Rolls.Sum() - result.Rolls.Min();
    }

    private StatChart BuildStats()
    {
        var maxHealth = Math.Max(12, _class!.BaseHealth + GameMath.AbilityModifier(_rolled.Str) * 2);
        var maxMana = Math.Max(4, _class.BaseMana + GameMath.AbilityModifier(_rolled.Int) * 2);
        return new StatChart(maxHealth, maxMana, _rolled.Str, _rolled.Dex, _rolled.Int);
    }

    private void HandleConfirm(GameContext ctx, string input)
    {
        var answer = input.Trim().ToLowerInvariant();
        if (answer.StartsWith('n'))
        {
            ctx.IO.WriteLine("The dice are gathered up and cast anew...");
            RollStats(ctx);
            return;
        }
        if (!answer.StartsWith('y'))
        {
            ctx.IO.Write("A simple y or n will do. Keep these scores? ");
            return;
        }

        BeginAdventure(ctx);
    }

    private void BeginAdventure(GameContext ctx)
    {
        var player = Player.CreateNewPlayer(_name, _class!, BuildStats());
        ctx.Player = player;

        // The engine narrates level-ups; the Player record just raises the event.
        player.LeveledUp += (p, oldLevel, newLevel) =>
        {
            ctx.IO.WriteLine();
            ctx.IO.WriteLine($"*** LEVEL UP! You are now level {newLevel}. ***", ConsoleColor.Magenta);
            ctx.IO.WriteLine($"    You feel hardier and your wounds close. ({p.Stats})", ConsoleColor.Magenta);
        };

        // Starting kit: class gear (equipped), a potion, and pocket money.
        player.Equipment.Equip(ItemFactory.CreateByKey(_class!.StartingWeaponKey));
        player.Equipment.Equip(ItemFactory.CreateByKey(_class.StartingArmorKey));
        player.Inventory.TryAdd(ItemFactory.HealingPotion());
        if (_class == PlayerClass.Mage)
            player.Inventory.TryAdd(ItemFactory.ManaPotion());
        player.AddGold(20 + new Dice(2, 6).Roll(ctx.Rng));

        ctx.CurrentArea = WorldBuilder.BuildWorld();

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"Welcome, {player.Name} the {player.Class.Name}, to The World!", ConsoleColor.Green);
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("""
            The cart that carried you this far rattles away down the south road,
            and you are left standing in the village of Willowbrook with a blade,
            a potion, and a rumor: something old is waking under the barrow to
            the north, and the village elder is paying.
            """);
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("(Type 'help' at any time to see what you can do.)", ConsoleColor.DarkGray);

        ctx.States.Replace(ctx, new ExploringState());
    }
}
