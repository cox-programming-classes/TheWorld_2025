using The_World.Engine;
using The_World.Engine.Commands;
using The_World.Engine.StateMachine;

namespace TheWorld.Tests;

/// <summary>
/// Scripted console: canned input lines, captured output.
/// </summary>
public sealed class ScriptedIO(params string[] lines) : IGameIO
{
    private readonly Queue<string> _lines = new(lines);
    private readonly System.Text.StringBuilder _output = new();

    public string Output => _output.ToString();

    public string? ReadLine() => _lines.Count > 0 ? _lines.Dequeue() : null;

    public void Write(string text) => _output.Append(text);

    public void WriteLine(string text = "") => _output.AppendLine(text);

    public void WriteLine(string text, ConsoleColor color) => _output.AppendLine(text);
}

internal sealed class RecordingState(string name, List<string> log) : IGameState
{
    public string Name => name;
    public string GetPrompt(GameContext ctx) => $"[{name}] ";
    public IReadOnlyList<ICommand> Commands { get; } =
        [new DelegateCommand("ping", "test command", (c, _) => c.IO.WriteLine($"pong from {name}"), aliases: ["p"])];

    public void OnEnter(GameContext ctx) => log.Add($"enter:{name}");
    public void OnExit(GameContext ctx) => log.Add($"exit:{name}");
    public void OnResume(GameContext ctx) => log.Add($"resume:{name}");
    public bool TryHandleRaw(GameContext ctx, string input)
    {
        if (input != "raw!")
            return false;
        log.Add($"raw:{name}");
        return true;
    }
}

public class GameStateMachineTests
{
    private static GameContext NewContext(params string[] lines) =>
        new(new ScriptedIO(lines), new Random(1));

    [Fact]
    public void PushPop_FiresLifecycleHooks_InOrder()
    {
        var log = new List<string>();
        var ctx = NewContext();
        var below = new RecordingState("below", log);
        var above = new RecordingState("above", log);

        ctx.States.Push(ctx, below);
        ctx.States.Push(ctx, above);
        ctx.States.Pop(ctx);

        Assert.Equal(["enter:below", "enter:above", "exit:above", "resume:below"], log);
        Assert.Same(below, ctx.States.Current);
    }

    [Fact]
    public void Replace_SwapsTop_WithoutResumingBelow()
    {
        var log = new List<string>();
        var ctx = NewContext();
        ctx.States.Push(ctx, new RecordingState("a", log));
        ctx.States.Replace(ctx, new RecordingState("b", log));

        Assert.Equal(["enter:a", "exit:a", "enter:b"], log);
        Assert.Equal(1, ctx.States.Depth);
    }

    [Fact]
    public void Reset_UnwindsEverything()
    {
        var log = new List<string>();
        var ctx = NewContext();
        ctx.States.Push(ctx, new RecordingState("a", log));
        ctx.States.Push(ctx, new RecordingState("b", log));
        ctx.States.Reset(ctx);

        Assert.True(ctx.States.IsEmpty);
        Assert.Equal(["enter:a", "enter:b", "exit:b", "exit:a"], log);
    }
}

public class CommandProcessorTests
{
    private static (GameContext ctx, ScriptedIO io, List<string> log) Setup(params string[] lines)
    {
        var io = new ScriptedIO(lines);
        var ctx = new GameContext(io, new Random(1));
        var log = new List<string>();
        ctx.States.Push(ctx, new RecordingState("test", log));
        return (ctx, io, log);
    }

    [Fact]
    public void StateCommand_Executes_ByNameAndAlias()
    {
        var (ctx, io, _) = Setup();
        var processor = new CommandProcessor();
        processor.Process(ctx, "ping");
        processor.Process(ctx, "P");
        Assert.Equal(2, io.Output.Split("pong from test").Length - 1);
    }

    [Fact]
    public void RawHandler_WinsOverCommands()
    {
        var (ctx, _, log) = Setup();
        new CommandProcessor().Process(ctx, "raw!");
        Assert.Contains("raw:test", log);
    }

    [Fact]
    public void UnknownCommand_SuggestsHelp()
    {
        var (ctx, io, _) = Setup();
        new CommandProcessor().Process(ctx, "dance");
        Assert.Contains("Unknown command: 'dance'", io.Output);
        Assert.Contains("help", io.Output);
    }

    [Fact]
    public void Help_ListsStateAndGlobalCommands()
    {
        var (ctx, io, _) = Setup();
        new CommandProcessor().Process(ctx, "help");
        Assert.Contains("ping", io.Output);
        Assert.Contains("quit", io.Output);
    }

    [Fact]
    public void Quit_AsksForConfirmation_AndCanBeDeclined()
    {
        var (ctx, io, _) = Setup("n");
        new CommandProcessor().Process(ctx, "quit");
        Assert.True(ctx.IsRunning);
        Assert.Contains("Really quit?", io.Output);

        var (ctx2, _, _) = Setup("y");
        new CommandProcessor().Process(ctx2, "quit");
        Assert.False(ctx2.IsRunning);
    }
}
