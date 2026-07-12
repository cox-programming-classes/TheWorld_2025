namespace The_World.Engine.Commands;

/// <summary>
/// The Command Pattern the original TODOs asked for: every action the
/// player can take is an object with a name, aliases, help text, and an
/// Execute method. States expose lists of these.
/// </summary>
public interface ICommand
{
    /// <summary>Primary name the player types ("attack").</summary>
    string Name { get; }

    /// <summary>Shorthand ("a", "fight", "kill").</summary>
    string[] Aliases { get; }

    /// <summary>One-liner for the help listing.</summary>
    string Description { get; }

    /// <summary>Usage hint, e.g. "attack &lt;creature&gt;".</summary>
    string Usage { get; }

    void Execute(GameContext ctx, string argument);
}

/// <summary>
/// A command built from a delegate - saves a class-per-verb explosion
/// while keeping the Command Pattern's shape.
/// </summary>
public sealed class DelegateCommand(
    string name,
    string description,
    Action<GameContext, string> execute,
    string? usage = null,
    params string[] aliases) : ICommand
{
    public string Name { get; } = name;
    public string[] Aliases { get; } = aliases;
    public string Description { get; } = description;
    public string Usage { get; } = usage ?? name;

    public void Execute(GameContext ctx, string argument) => execute(ctx, argument);

    public bool Matches(string word) =>
        Name.Equals(word, StringComparison.OrdinalIgnoreCase)
        || Aliases.Any(a => a.Equals(word, StringComparison.OrdinalIgnoreCase));
}
