using The_World.Engine.Commands;

namespace The_World.Engine.States;

/// <summary>
/// The Lich is dust. Bask, then either keep wandering the world you saved
/// (Elder Maera owes you a reward...) or start a new legend.
/// </summary>
public class VictoryState : GameStateBase
{
    public override string Name => "Victorious";

    public override string GetPrompt(GameContext ctx) => "[VICTORY] > ";

    public override void OnEnter(GameContext ctx)
    {
        if (CommandList.Count == 0)
            CommandList.AddRange(
            [
                new DelegateCommand("continue", "Keep exploring the world you saved.",
                    (c, _) => c.States.Pop(c), aliases: ["explore", "wander"]),
                new DelegateCommand("newgame", "Begin a new legend.",
                    (c, _) => c.StartNewGame(new CharacterCreationState()), aliases: ["new", "restart"]),
            ]);

        ctx.IO.WriteLine();
        ctx.IO.WriteLine("""
              +==================================+
              |   THE  LICH  IS  DESTROYED  !    |
              +==================================+
            """, ConsoleColor.Yellow);
        ctx.IO.WriteLine("""
            Malakhar's crown rolls to a stop at your feet. The green candles
            gutter out one by one, and for the first time in three hundred
            years, the Barrow is merely dark - not hungry.

            Somewhere above, in Willowbrook, the evening bell is ringing.
            It sounds like it's ringing for you.
            """);
        ctx.IO.WriteLine(ctx.Player.CharacterSheet(), ConsoleColor.Cyan);
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("'continue' to keep exploring (Maera would love the news), 'newgame' for a fresh legend, 'quit' to end here.", ConsoleColor.DarkGray);
    }
}
