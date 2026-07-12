using The_World.Engine.Commands;

namespace The_World.Engine.StateMachine;

/// <summary>
/// One mode of play. Each state owns its own command set, so what the
/// player can *do* depends on what the game is *being*: you can't "equip"
/// mid-swordfight and you can't "flee" a shopkeeper (well - you can 'leave').
///
/// States are stacked (see GameStateMachine): Combat pushes on top of
/// Exploring, and popping it lands you exactly where you were.
/// </summary>
public interface IGameState
{
    /// <summary>Display name: "Exploring", "Combat", ...</summary>
    string Name { get; }

    /// <summary>The input prompt - each state shows its own.</summary>
    string GetPrompt(GameContext ctx);

    /// <summary>The commands available while this state is on top.</summary>
    IReadOnlyList<ICommand> Commands { get; }

    /// <summary>Called when the state is pushed onto the stack.</summary>
    void OnEnter(GameContext ctx);

    /// <summary>Called when the state is popped off the stack.</summary>
    void OnExit(GameContext ctx) { }

    /// <summary>
    /// Called when the state above this one pops and this state is on top
    /// again (e.g. combat ends and we're back to exploring).
    /// </summary>
    void OnResume(GameContext ctx) { }

    /// <summary>
    /// First crack at raw input, before command parsing. States that read
    /// free-form input (dialogue choice numbers, your character's name)
    /// return true to consume the line.
    /// </summary>
    bool TryHandleRaw(GameContext ctx, string input) => false;
}
