using The_World.Engine;

// The World - a text adventure of dice and doom.
//
// The old command-parsing switch that lived here has grown up into a real
// engine: a push-down state machine (Exploring / Combat / Dialogue /
// Shopping / Gambling / ...), a Command Pattern parser, and a seedable
// random source. Program.cs just wires the console to the engine.
//
// Usage:
//   dotnet run                start a game
//   dotnet run -- --seed 42   reproducible dice (handy for testing)

int? seed = null;
for (var i = 0; i < args.Length - 1; i++)
    if (args[i] is "--seed" or "-s" && int.TryParse(args[i + 1], out var s))
        seed = s;

new GameEngine(new ConsoleIO(), seed).Run();
