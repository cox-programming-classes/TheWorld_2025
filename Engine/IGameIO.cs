namespace The_World.Engine;

/// <summary>
/// Abstraction over the console so the engine can be driven by tests
/// (or, someday, something fancier than a terminal).
/// </summary>
public interface IGameIO
{
    /// <summary>Read a line of input. Null means end-of-input (treat as quit).</summary>
    string? ReadLine();

    void Write(string text);

    void WriteLine(string text = "");

    void WriteLine(string text, ConsoleColor color);
}

/// <summary>
/// The real console, with a splash of color.
/// </summary>
public sealed class ConsoleIO : IGameIO
{
    public ConsoleIO()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch (IOException)
        {
            // Output is redirected or the host console objects; plain text is fine.
        }
    }

    public string? ReadLine() => Console.ReadLine();

    public void Write(string text) => Console.Write(text);

    public void WriteLine(string text = "") => Console.WriteLine(text);

    public void WriteLine(string text, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ForegroundColor = previous;
    }
}
