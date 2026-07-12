using The_World.Engine.Commands;

namespace The_World.Engine.States;

/// <summary>
/// Death. The stack is cleared before this is pushed - the only ways out
/// are a fresh hero or the door.
/// </summary>
public class GameOverState(string epitaph) : GameStateBase
{
    public override string Name => "Game Over";

    public override string GetPrompt(GameContext ctx) => "[Game Over] > ";

    public override void OnEnter(GameContext ctx)
    {
        if (CommandList.Count == 0)
            CommandList.Add(new DelegateCommand("newgame", "Rise again as a new hero.",
                (c, _) => c.StartNewGame(new CharacterCreationState()), aliases: ["new", "restart", "again"]));

        ctx.IO.WriteLine();
        ctx.IO.WriteLine("""
              +----------------------------------+
              |         YOU  HAVE  DIED          |
              +----------------------------------+
            """, ConsoleColor.DarkRed);
        ctx.IO.WriteLine($"  {epitaph}", ConsoleColor.DarkGray);
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("The World forgets no one - but it does keep going.");
        ctx.IO.WriteLine("Type 'newgame' to roll a new hero, or 'quit' to rest for good.", ConsoleColor.DarkGray);
    }
}
