using The_World.Engine.Commands;
using The_World.Engine.States;

namespace The_World.Engine;

/// <summary>
/// The main loop: show the current state's prompt, read a line, hand it to
/// the command processor, repeat until something sets IsRunning false.
/// All the *rules* live in the states; this class just turns the crank.
/// </summary>
public class GameEngine(IGameIO io, int? seed = null)
{
    private readonly GameContext _ctx = new(io, seed is null ? new Random() : new Random(seed.Value));
    private readonly CommandProcessor _processor = new();

    /// <summary>Exposed for tests, which drive the engine with scripted IO.</summary>
    public GameContext Context => _ctx;

    public void Run()
    {
        PrintBanner();
        _ctx.States.Push(_ctx, new CharacterCreationState());

        while (_ctx.IsRunning)
        {
            _ctx.IO.Write(_ctx.States.Current.GetPrompt(_ctx));
            var input = _ctx.IO.ReadLine();
            if (input is null)
            {
                // End of input (Ctrl+Z / piped script ran out): leave gracefully.
                _ctx.IsRunning = false;
                break;
            }
            if (string.IsNullOrWhiteSpace(input))
                continue;

            _processor.Process(_ctx, input);
        }

        _ctx.IO.WriteLine();
        _ctx.IO.WriteLine("The World will be here when you return.");
    }

    private void PrintBanner()
    {
        _ctx.IO.WriteLine("""

            =====================================================
              _____ _   _ _____   __        _____  ____  _     ____
             |_   _| | | | ____|  \ \      / / _ \|  _ \| |   |  _ \
               | | | |_| |  _|     \ \ /\ / / | | | |_) | |   | | | |
               | | |  _  | |___     \ V  V /| |_| |  _ <| |___| |_| |
               |_| |_| |_|_____|     \_/\_/  \___/|_| \_\_____|____/

                       a text adventure of dice and doom
            =====================================================
            """, ConsoleColor.Cyan);
    }
}
