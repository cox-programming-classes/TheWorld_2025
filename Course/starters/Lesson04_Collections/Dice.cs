namespace Toolkit;

/// <summary>
/// Your <c>Dice</c> from Lessons 1 through 3, finished.
///
/// The three properties are init-only, so a set of dice is fixed once it
/// exists.  The constructor is private, so <c>Of()</c> is the only way in and
/// every rule is written down once.  Display and game rules moved out in
/// Lesson 3, so this file knows what dice ARE and stays quiet about the rest.
///
/// You are done with this file today.  Read it if you want the reminder,
/// then leave it alone -- today's work is all in <c>DiceBag.cs</c>.
/// </summary>
public record Dice
{
    /// <summary>How many dice are rolled.  The 2 in "2d6+3".</summary>
    public int Count { get; init; }

    /// <summary>How many faces each die has.  The 6 in "2d6+3".</summary>
    public int Sides { get; init; }

    /// <summary>A flat number added after rolling.  The +3 in "2d6+3".</summary>
    public int Modifier { get; init; }

    // ---- The limits.  Structural facts are const;  house rules are properties. ----

    /// <summary>A random result needs at least two outcomes, in any game.</summary>
    public const int MinSides = 2;

    /// <summary>This table's ceiling on how many dice one roll uses.</summary>
    public static int MaxCount => 100;

    /// <summary>This table's ceiling on faces per die.</summary>
    public static int MaxSides => 1000;

    /// <summary>This table's ceiling on the size of a flat modifier.</summary>
    public static int MaxModifier => 20;

    private Dice(int count, int sides, int modifier)
    {
        Count = count; Sides = sides; Modifier = modifier;
    }

    /// <summary>The general front door.  Every rule lives here.</summary>
    public static Dice Of(int count = 1, int sides = 6, int modifier = 0)
    {
        if (count < 1 || count > MaxCount)
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"A roll uses between 1 and {MaxCount} dice.");

        if (sides < MinSides || sides > MaxSides)
            throw new ArgumentOutOfRangeException(
                nameof(sides), sides, $"A die has between {MinSides} and {MaxSides} faces.");

        if (Math.Abs(modifier) > MaxModifier)
            throw new ArgumentOutOfRangeException(
                nameof(modifier), modifier, $"A modifier stays within {MaxModifier}.");

        return new Dice(count, sides, modifier);
    }

    /// <summary>An attack roll:  one d20 plus a bonus.</summary>
    public static Dice Attack(int bonus = 0) => Of(1, 20, bonus);

    /// <summary>The classic ability score roll.</summary>
    public static Dice AbilityScore() => Of(3, 6);

    // ---- Shared presets.  Safe to share because a Dice is fixed once built. ----

    public static Dice D4 { get; } = Of(1, 4);
    public static Dice D6 { get; } = Of(1, 6);
    public static Dice D8 { get; } = Of(1, 8);
    public static Dice D10 { get; } = Of(1, 10);
    public static Dice D12 { get; } = Of(1, 12);
    public static Dice D20 { get; } = Of(1, 20);

    /// <summary>Reads "2d6+3" and hands back dice, or false for anything else.</summary>
    public static bool TryParse(string? notation, out Dice dice)
    {
        dice = D6;
        if (string.IsNullOrWhiteSpace(notation)) return false;

        var text = notation.Trim().ToLowerInvariant();
        var d = text.IndexOf('d');
        if (d < 0) return false;

        var countText = text[..d];
        var rest = text[(d + 1)..];
        var count = countText.Length == 0 ? 1 : 0;
        if (countText.Length > 0 && !int.TryParse(countText, out count)) return false;

        var modifier = 0;
        var sign = rest.LastIndexOfAny(['+', '-']);
        if (sign > 0)
        {
            if (!int.TryParse(rest[sign..], out modifier)) return false;
            rest = rest[..sign];
        }

        if (!int.TryParse(rest, out var sides)) return false;

        try { dice = Of(count, sides, modifier); return true; }
        catch (ArgumentOutOfRangeException) { return false; }
    }

    /// <summary>The notation these dice were built from.  "2d6+3".</summary>
    public string Notation => Modifier switch
    {
        0 => $"{Count}d{Sides}",
        > 0 => $"{Count}d{Sides}+{Modifier}",
        _ => $"{Count}d{Sides}{Modifier}"
    };

    /// <summary>Rolls, using a supplied source of randomness so a test can repeat it.</summary>
    public int Roll(Random random)
    {
        var total = 0;
        for (var i = 0; i < Count; i++) total += random.Next(1, Sides + 1);
        return total + Modifier;
    }

    public override string ToString() => Notation;
}
