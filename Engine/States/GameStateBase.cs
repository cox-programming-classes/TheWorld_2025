using The_World.Engine.Commands;
using The_World.Engine.StateMachine;

namespace The_World.Engine.States;

/// <summary>
/// Shared plumbing for game states: a mutable command list the subclass
/// fills in its constructor, and no-op lifecycle hooks to override.
/// </summary>
public abstract class GameStateBase : IGameState
{
    public abstract string Name { get; }

    protected readonly List<ICommand> CommandList = [];

    public IReadOnlyList<ICommand> Commands => CommandList;

    public virtual string GetPrompt(GameContext ctx) => $"[{Name}] > ";

    public virtual void OnEnter(GameContext ctx) { }

    public virtual void OnExit(GameContext ctx) { }

    public virtual void OnResume(GameContext ctx) { }

    public virtual bool TryHandleRaw(GameContext ctx, string input) => false;

    /// <summary>
    /// Derive a dictionary key from an item name ("Healing Potion" ->
    /// "healing_potion"), adding numeric suffixes until it's unique.
    /// </summary>
    protected static string UniqueKey<T>(IDictionary<string, T> dict, string name)
    {
        var baseKey = name.Trim().ToLowerInvariant().Replace(' ', '_');
        if (!dict.ContainsKey(baseKey))
            return baseKey;
        var n = 2;
        while (dict.ContainsKey($"{baseKey}_{n}"))
            n++;
        return $"{baseKey}_{n}";
    }
}
