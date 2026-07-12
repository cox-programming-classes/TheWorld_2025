using System.Text.RegularExpressions;

namespace The_World.GameData.GameMechanics;

/// <summary>
/// A set of dice: Count dice with Sides sides, plus a flat Modifier.
/// e.g. new Dice(2, 6, 3) is "2d6+3".
///
/// Common Dice types are provided for easy access:
/// var d6p3 = Dice.D6 with { Modifier = 3 };
/// var roll = d6p3.Roll();
/// </summary>
public record Dice(int Count = 1, int Sides = 6, int Modifier = 0)
{
    // Common Dice types
    public static readonly Dice Coin = new(Sides: 2);
    public static readonly Dice D4 = new(Sides: 4);
    public static readonly Dice D6 = new();
    public static readonly Dice D8 = new(Sides: 8);
    public static readonly Dice D10 = new(Sides: 10);
    public static readonly Dice D12 = new(Sides: 12);
    public static readonly Dice D20 = new(Sides: 20);
    public static readonly Dice D100 = new(Sides: 100);

    /// <summary>
    /// Roll the dice! Sums Count rolls of a Sides-sided die, plus the Modifier.
    /// </summary>
    /// <param name="rng">Optional random source (pass a seeded Random for reproducible rolls).</param>
    public virtual int Roll(Random? rng = null)
    {
        rng ??= Random.Shared;
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
            sum += rng.Next(1, Sides + 1);
        return sum;
    }

    /// <summary>
    /// Roll the dice, but keep the individual die results for flavor text
    /// ("you rolled 4 + 6 + 2 = 12!") and for detecting natural 1s / 20s.
    /// </summary>
    public DiceResult RollDetailed(Random? rng = null)
    {
        rng ??= Random.Shared;
        var rolls = new int[Count];
        for (var i = 0; i < Count; i++)
            rolls[i] = rng.Next(1, Sides + 1);
        return new DiceResult(this, rolls);
    }

    /// <summary>
    /// The smallest total this dice set can produce.
    /// </summary>
    public int Minimum => Count + Modifier;

    /// <summary>
    /// The largest total this dice set can produce.
    /// </summary>
    public int Maximum => Count * Sides + Modifier;

    /// <summary>
    /// Parse standard dice notation: "d20", "2d6", "3d8+2", "1d4-1".
    /// </summary>
    public static Dice Parse(string notation)
    {
        if (!TryParse(notation, out var dice))
            throw new FormatException($"'{notation}' is not valid dice notation (try something like '2d6+3').");
        return dice;
    }

    public static bool TryParse(string? notation, out Dice dice)
    {
        dice = D6;
        if (string.IsNullOrWhiteSpace(notation))
            return false;

        var match = Regex.Match(
            notation.Trim(),
            @"^(?<count>\d*)[dD](?<sides>\d+)(?:(?<sign>[+-])(?<mod>\d+))?$");
        if (!match.Success)
            return false;

        var count = match.Groups["count"].Value is "" ? 1 : int.Parse(match.Groups["count"].Value);
        var sides = int.Parse(match.Groups["sides"].Value);
        var modifier = match.Groups["mod"].Success
            ? int.Parse(match.Groups["mod"].Value) * (match.Groups["sign"].Value == "-" ? -1 : 1)
            : 0;

        if (count < 1 || sides < 2)
            return false;

        dice = new Dice(count, sides, modifier);
        return true;
    }

    /// <summary>
    /// Standard dice notation, e.g. "2d6+3".
    /// </summary>
    public override string ToString() => Modifier switch
    {
        > 0 => $"{Count}d{Sides}+{Modifier}",
        < 0 => $"{Count}d{Sides}{Modifier}",
        _ => $"{Count}d{Sides}"
    };
}

/// <summary>
/// The outcome of a detailed dice roll: the dice that were rolled
/// and each individual die result.
/// </summary>
public record DiceResult(Dice Dice, int[] Rolls)
{
    public int Total => Rolls.Sum() + Dice.Modifier;

    /// <summary>A natural maximum on a single-die roll (e.g. a nat 20). Critical hit!</summary>
    public bool IsCriticalSuccess => Rolls.Length == 1 && Rolls[0] == Dice.Sides;

    /// <summary>A natural 1 on a single-die roll. Fumble!</summary>
    public bool IsCriticalFailure => Rolls.Length == 1 && Rolls[0] == 1;

    public override string ToString() => Dice.Modifier switch
    {
        0 => $"[{string.Join(" + ", Rolls)}] = {Total}",
        > 0 => $"[{string.Join(" + ", Rolls)}] + {Dice.Modifier} = {Total}",
        _ => $"[{string.Join(" + ", Rolls)}] - {-Dice.Modifier} = {Total}"
    };
}

/// <summary>
/// A cheating die! Rolls twice and keeps the result nearest its favorite value.
/// Used by unscrupulous gamblers. Don't let Finn deal.
/// </summary>
public record WeightedDice(int Count = 1, int Sides = 6, int Modifier = 0, int FavoredFace = 6)
    : Dice(Count, Sides, Modifier)
{
    public override int Roll(Random? rng = null)
    {
        rng ??= Random.Shared;
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
        {
            var first = rng.Next(1, Sides + 1);
            var second = rng.Next(1, Sides + 1);
            sum += Math.Abs(first - FavoredFace) <= Math.Abs(second - FavoredFace) ? first : second;
        }
        return sum;
    }
}

/// <summary>
/// A cup holding several different dice, rolled all at once.
/// </summary>
public record DiceCup(List<Dice> Dice)
{
    public int RollAll(Random? rng = null)
    {
        var total = 0;
        foreach (var die in Dice)
            total += die.Roll(rng);
        return total;
    }
}
