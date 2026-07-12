using The_World.Engine.Commands;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;

namespace The_World.Engine.States;

/// <summary>
/// Knucklebones! Bet gold, roll 2d6 against the house, high roll wins.
///
/// The house, it must be said, rolls WeightedDice. A sharp-eyed player
/// ('watch', an Intelligence check) can catch the cheat and 'accuse' -
/// Finn pays hush money and swaps in fair dice for good.
/// </summary>
public class GamblingState(Npc npc) : GameStateBase
{
    private const int MaxBet = 25;

    private static readonly Dice FairDice = new(2, 6);
    private static readonly WeightedDice LoadedDice = new(2, 6, Modifier: 0, FavoredFace: 6);

    public override string Name => "Gambling";

    public override string GetPrompt(GameContext ctx) => $"[Knucklebones | Gold: {ctx.Player.Gold}] > ";

    public override void OnEnter(GameContext ctx)
    {
        if (CommandList.Count == 0)
            CommandList.AddRange(
            [
                new DelegateCommand("bet", "Wager gold on a roll of the bones.", Bet, $"bet <1-{MaxBet}>", "roll", "play"),
                new DelegateCommand("watch", "Study the dealer's hands (Intelligence check).", Watch),
                new DelegateCommand("accuse", "Call out a cheat. Be sure first.", Accuse),
                new DelegateCommand("leave", "Step away from the table.", (c, _) => c.States.Pop(c), aliases: ["done", "quit_table", "stand"]),
            ]);

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"{npc.Name} sweeps the table clear and produces two bone dice.", ConsoleColor.Cyan);
        ctx.IO.WriteLine($"\"House rules: you bet, we both roll two dice, high total takes the pot. Ties push. Max stake {MaxBet}.\"");
    }

    private void Bet(GameContext ctx, string argument)
    {
        if (!int.TryParse(argument.Trim(), out var stake) || stake < 1)
        {
            ctx.IO.WriteLine($"Bet how much? (bet <1-{MaxBet}>)");
            return;
        }
        if (stake > MaxBet)
        {
            ctx.IO.WriteLine($"\"Whoa there. {MaxBet} gold ceiling - I'm a gambler, not a bank.\"");
            return;
        }
        if (stake > ctx.Player.Gold)
        {
            ctx.IO.WriteLine($"You can't cover that. (You have {ctx.Player.Gold} gold.)");
            return;
        }

        var yours = FairDice.RollDetailed(ctx.Rng);
        var playsFair = ctx.HasFlag(NpcFactory.FlagFinnPlaysFair);
        var theirs = playsFair ? FairDice.Roll(ctx.Rng) : LoadedDice.Roll(ctx.Rng);

        ctx.IO.WriteLine($"You roll: {yours}", ConsoleColor.Yellow);
        ctx.IO.WriteLine($"{npc.Name} rolls... {theirs}.", ConsoleColor.Yellow);

        if (yours.Total > theirs)
        {
            ctx.Player.AddGold(stake);
            ctx.IO.WriteLine($"\"Bah!\" {npc.Name} slides {stake} gold across, smiling thinly. ({ctx.Player.Gold} gold)", ConsoleColor.Green);
        }
        else if (yours.Total < theirs)
        {
            ctx.Player.SpendGold(stake);
            ctx.IO.WriteLine($"\"Luck's a lady,\" {npc.Name} says, sweeping up your {stake} gold. ({ctx.Player.Gold} gold)", ConsoleColor.Red);
        }
        else
        {
            ctx.IO.WriteLine("A push. The coins stay where they lie.");
        }
    }

    private void Watch(GameContext ctx, string _)
    {
        if (ctx.HasFlag(NpcFactory.FlagFinnPlaysFair))
        {
            ctx.IO.WriteLine($"{npc.Name}'s hands are ostentatiously honest these days.");
            return;
        }
        if (GameMath.Check(ctx.Player.Stats.Intelligence, 13, ctx.Rng))
        {
            ctx.SetFlag(NpcFactory.FlagFinnSuspected);
            ctx.IO.WriteLine("There! A twitch of the wrist as the dice come down - always onto the same faces. Those bones are loaded.", ConsoleColor.Yellow);
        }
        else
        {
            ctx.IO.WriteLine("You watch closely, but his hands are quicker than your eyes.");
        }
    }

    private void Accuse(GameContext ctx, string _)
    {
        if (ctx.HasFlag(NpcFactory.FlagFinnPlaysFair))
        {
            ctx.IO.WriteLine($"\"We're square, friend, square!\" {npc.Name} protests, dice conspicuously fair.");
            return;
        }
        if (ctx.HasFlag(NpcFactory.FlagFinnSuspected))
        {
            ctx.SetFlag(NpcFactory.FlagFinnPlaysFair);
            ctx.Player.AddGold(25);
            ctx.IO.WriteLine($"You describe the wrist-twitch in detail. {npc.Name} goes pale, glances at Hulda, " +
                             "and pushes 25 gold at you with a strained grin. \"An... apology, friend. New dice, watch. Clean as rain.\"", ConsoleColor.Green);
            ctx.IO.WriteLine("(Finn plays fair from now on.)", ConsoleColor.DarkGray);
        }
        else
        {
            var fine = Math.Min(5, ctx.Player.Gold);
            ctx.Player.SpendGold(fine);
            ctx.IO.WriteLine($"{npc.Name} looks mortally wounded. The whole tavern glares at you. " +
                             $"You end up buying an apology round ({fine} gold). Perhaps watch him first next time.", ConsoleColor.Red);
        }
    }
}
