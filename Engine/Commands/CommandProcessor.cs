namespace The_World.Engine.Commands;

/// <summary>
/// The "more robust command parser as a separate class" the old Program.cs
/// wished for. Resolution order:
///   1. the current state's raw-input hook (dialogue numbers, name entry),
///   2. the current state's commands,
///   3. the global commands (help/quit), available everywhere.
/// </summary>
public class CommandProcessor
{
    private readonly List<ICommand> _globals;

    public CommandProcessor()
    {
        _globals =
        [
            new DelegateCommand("help", "List the commands available right now.", Help, aliases: ["?", "commands"]),
            new DelegateCommand("quit", "Leave the game (asks for confirmation).", Quit, aliases: ["exit"])
        ];
    }

    public IReadOnlyList<ICommand> Globals => _globals;

    public void Process(GameContext ctx, string input)
    {
        input = input.Trim();
        if (input.Length == 0)
            return;

        var state = ctx.States.Current;
        if (state.TryHandleRaw(ctx, input))
            return;

        var parts = input.Split(' ', 2, StringSplitOptions.TrimEntries);
        var word = parts[0];
        var argument = parts.Length > 1 ? parts[1] : "";

        var command = FindCommand(state.Commands, word) ?? FindCommand(_globals, word);
        if (command is null)
        {
            ctx.IO.WriteLine($"Unknown command: '{word}'. Type 'help' to see what you can do here.");
            return;
        }

        command.Execute(ctx, argument);
    }

    private static ICommand? FindCommand(IReadOnlyList<ICommand> commands, string word) =>
        commands.FirstOrDefault(c =>
            c.Name.Equals(word, StringComparison.OrdinalIgnoreCase)
            || c.Aliases.Any(a => a.Equals(word, StringComparison.OrdinalIgnoreCase)));

    private void Help(GameContext ctx, string _)
    {
        var state = ctx.States.Current;
        ctx.IO.WriteLine($"--- Commands while {state.Name} ---", ConsoleColor.Cyan);
        foreach (var c in state.Commands)
            ctx.IO.WriteLine($"  {c.Usage,-24} {c.Description}{AliasNote(c)}");
        ctx.IO.WriteLine("--- Always available ---", ConsoleColor.Cyan);
        foreach (var c in _globals)
            ctx.IO.WriteLine($"  {c.Usage,-24} {c.Description}");
    }

    private static string AliasNote(ICommand c) =>
        c.Aliases.Length == 0 ? "" : $"  (also: {string.Join(", ", c.Aliases)})";

    private void Quit(GameContext ctx, string _)
    {
        ctx.IO.Write("Really quit? Your deeds will be forgotten. (y/n) ");
        var answer = ctx.IO.ReadLine();
        if (answer is null || answer.Trim().StartsWith("y", StringComparison.OrdinalIgnoreCase))
        {
            ctx.IO.WriteLine("Thanks for playing!");
            ctx.IsRunning = false;
        }
        else
        {
            ctx.IO.WriteLine("The World isn't done with you yet.");
        }
    }
}
