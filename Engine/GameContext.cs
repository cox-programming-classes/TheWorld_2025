using The_World.Engine.StateMachine;
using The_World.GameData;
using The_World.GameData.Areas;

namespace The_World.Engine;

/// <summary>
/// Everything the game knows about right now, in one place:
/// the player, where they are, the state machine, story flags, and the
/// shared random source (seedable, for reproducible runs and tests).
///
/// Commands and states receive this instead of reaching for globals.
/// </summary>
public class GameContext(IGameIO io, Random rng)
{
    public IGameIO IO { get; } = io;

    /// <summary>One random source for the whole game - pass a seeded Random for reproducible runs.</summary>
    public Random Rng { get; } = rng;

    public GameStateMachine States { get; } = new();

    /// <summary>Created by the CharacterCreation state; never null after that.</summary>
    public Player Player { get; set; } = null!;

    /// <summary>Where the player is standing. Set when the world is built.</summary>
    public Area CurrentArea { get; set; } = null!;

    /// <summary>The main loop runs while this is true.</summary>
    public bool IsRunning { get; set; } = true;

    /// <summary>Story flags: quest progress, who's been exposed as a cheat, etc.</summary>
    public HashSet<string> Flags { get; } = [];

    /// <summary>The player's quest journal, in the order things happened.</summary>
    public List<string> Journal { get; } = [];

    public bool HasFlag(string flag) => Flags.Contains(flag);

    public void SetFlag(string flag) => Flags.Add(flag);

    /// <summary>
    /// Wipe the story and start over: fresh flags, fresh journal, fresh hero.
    /// Ends with the character creation state on a clean stack.
    /// </summary>
    public void StartNewGame(IGameState characterCreation)
    {
        Flags.Clear();
        Journal.Clear();
        Player = null!;
        CurrentArea = null!;
        States.Reset(this);
        States.Push(this, characterCreation);
    }
}
