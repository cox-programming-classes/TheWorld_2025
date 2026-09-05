namespace Toolkit;

/// <summary>
/// A set of dice:  Count dice with Sides sides, plus a flat Modifier.
///
/// This is your Dice from Lesson 2, four weeks later, after everyone on the
/// team added the one small thing they needed.  Every one of them was reasonable.
/// Every method in here was a reasonable idea on the day it was written.
///
/// Read the whole file before you touch anything.  Then answer one question:
///
///     How many different jobs is this type doing?
/// </summary>
public record Dice
{
    /// <summary>The fewest sides a die may have.  Two faces is the floor for randomness.</summary>
    public const int MinSides = 2;

    /// <summary>The most dice one roll may use.</summary>
    public static int MaxCount => 100;

    /// <summary>The most sides a die may have.</summary>
    public static int MaxSides => 1000;

    /// <summary>The largest modifier, in either direction, a roll may carry.</summary>
    public static int MaxModifier => 20;

    // ─────────────────────────────────────────────────────────────────────────
    //  What a set of dice IS
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>How many dice are rolled.  The 2 in "2d6+3".</summary>
    public int Count { get; init; }

    /// <summary>How many sides each die has.  The 6 in "2d6+3".</summary>
    public int Sides { get; init; }

    /// <summary>A flat amount added to the total after rolling.  The +3 in "2d6+3".</summary>
    public int Modifier { get; init; }

    /// <summary>
    /// Private.  Every way to make dice goes through a factory method below,
    /// and that is where the rules live.
    /// </summary>
    private Dice(int count, int sides, int modifier)
    {
        Count = count;
        Sides = sides;
        Modifier = modifier;
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Factory methods -- the only ways in
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Builds a set of dice, or refuses to.</summary>
    /// <param name="count">How many dice.</param>
    /// <param name="sides">How many sides on each.</param>
    /// <param name="modifier">A flat amount added to the total.</param>
    public static Dice Of(int count = 1, int sides = 6, int modifier = 0)
    {
        if (count < 1 || count > MaxCount)
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"A roll uses between 1 and {MaxCount} dice.");

        if (sides < MinSides || sides > MaxSides)
            throw new ArgumentOutOfRangeException(
                nameof(sides), sides, $"A die has between {MinSides} and {MaxSides} sides.");

        if (modifier < -MaxModifier || modifier > MaxModifier)
            throw new ArgumentOutOfRangeException(
                nameof(modifier), modifier, $"A modifier is between -{MaxModifier} and +{MaxModifier}.");

        return new Dice(count, sides, modifier);
    }

    /// <summary>An attack roll:  d20 plus whatever you are good at.</summary>
    /// <param name="bonus">The attacker's bonus to hit.</param>
    public static Dice Attack(int bonus = 0) => Of(1, 20, bonus);

    /// <summary>Rolling up a character: 4d6.</summary>
    public static Dice AbilityScore() => Of(4, 6);

    /// <summary>A weapon's damage die.</summary>
    /// <param name="sides">How many sides the damage die has.</param>
    public static Dice Damage(int sides) => Of(1, sides);

    /// <summary>Looks up dice by name -- for a save file, a config, a console command.</summary>
    /// <param name="key">The name to look up.  Case and spacing are cleaned up for you.</param>
    /// <returns>The dice, or null when the key is unknown.</returns>
    public static Dice? ByName(string? key) => key?.Trim().ToLowerInvariant() switch
    {
        "attack" => Attack(),
        "ability" or "ability score" => AbilityScore(),
        "d4" => D4,
        "d6" => D6,
        "d20" => D20,
        _ => null
    };

    // ─────────────────────────────────────────────────────────────────────────
    //  Rolling
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Rolls the dice and returns the total.</summary>
    /// <param name="rng">Where the randomness comes from.  Seed it to repeat it.</param>
    public int Roll(Random? rng = null)
    {
        rng ??= Random.Shared;
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
            sum += rng.Next(1, Sides + 1);
        return sum;
    }

    /// <summary>Rolls, but keeps the individual faces.  Needed for natural 20s.</summary>
    /// <param name="rng">Where the randomness comes from.</param>
    public int[] RollEach(Random? rng = null)
    {
        rng ??= Random.Shared;
        var faces = new int[Count];
        for (var i = 0; i < Count; i++)
            faces[i] = rng.Next(1, Sides + 1);
        return faces;
    }

    /// <summary>The smallest total these dice can produce.</summary>
    public int Minimum => Count + Modifier;

    /// <summary>The largest total these dice can produce.</summary>
    public int Maximum => Count * Sides + Modifier;

    /// <summary>The total you would get on an average roll, over enough rolls.</summary>
    public double Average => Count * (Sides + 1) / 2.0 + Modifier;

    /// <summary>"2d6+3", the way a player would write it.</summary>
    public override string ToString()
    {
        var mod = Modifier switch { > 0 => $"+{Modifier}", < 0 => $"{Modifier}", _ => "" };
        return $"{Count}d{Sides}{mod}";
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  Making dice out of text
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Turns dice notation into Dice, or throws trying.</summary>
    /// <param name="notation">Dice notation, like "2d6+3".</param>
    public static Dice Parse(string notation)
    {
        if (!TryParse(notation, out var dice))
            throw new FormatException($"'{notation}' fell outside dice notation.  Try something like 2d6+3.");
        return dice;
    }

    /// <summary>Tries to turn dice notation into Dice.</summary>
    /// <param name="notation">Dice notation, possibly nonsense.</param>
    /// <param name="dice">The dice, if it worked.</param>
    /// <returns>true if the notation was understood.</returns>
    public static bool TryParse(string? notation, out Dice dice)
    {
        dice = D6;
        if (string.IsNullOrWhiteSpace(notation))
            return false;

        var text = notation.Trim().ToLowerInvariant();

        var modifier = 0;
        var signAt = text.LastIndexOfAny(new[] { '+', '-' });
        if (signAt > 0)
        {
            if (!int.TryParse(text[signAt..], out modifier))
                return false;
            text = text[..signAt];
        }

        var parts = text.Split('d');
        if (parts.Length != 2)
            return false;

        var count = 1;
        if (parts[0].Length > 0 && !int.TryParse(parts[0], out count))
            return false;
        if (!int.TryParse(parts[1], out var sides))
            return false;

        try
        {
            dice = Of(count, sides, modifier);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  The dice everybody reaches for
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>A four-sided die.</summary>
    public static readonly Dice D4 = Of(sides: 4);

    /// <summary>A six-sided die.</summary>
    public static readonly Dice D6 = Of(sides: 6);

    /// <summary>An eight-sided die.</summary>
    public static readonly Dice D8 = Of(sides: 8);

    /// <summary>A ten-sided die.</summary>
    public static readonly Dice D10 = Of(sides: 10);

    /// <summary>A twelve-sided die.</summary>
    public static readonly Dice D12 = Of(sides: 12);

    /// <summary>A twenty-sided die.</summary>
    public static readonly Dice D20 = Of(sides: 20);

    // ─────────────────────────────────────────────────────────────────────────
    //  Everything below this line was added later, by people in a hurry.
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>One line a player could read.  Added by someone in a hurry.</summary>
    public string Describe() => $"{this} [rolls {Minimum}-{Maximum}]";

    /// <summary>Draws a bar chart of what these dice actually do, over N rolls.</summary>
    public void PrintHistogram(int samples = 1000, Random? rng = null)
    {
        rng ??= Random.Shared;
        var tally = new Dictionary<int, int>();
        for (var i = 0; i < samples; i++)
        {
            var total = Roll(rng);
            tally[total] = tally.GetValueOrDefault(total) + 1;
        }

        var tallest = 0;
        foreach (var count in tally.Values)
            if (count > tallest) tallest = count;

        Console.WriteLine($"  {this} over {samples} rolls");
        for (var total = Minimum; total <= Maximum; total++)
        {
            var count = tally.GetValueOrDefault(total);
            var width = tallest == 0 ? 0 : count * 40 / tallest;
            Console.WriteLine($"  {total,4} | {new string('#', width)}");
        }
    }

    /// <summary>Prints a roll, in green if it's a natural max, red if it's a natural 1.</summary>
    public void PrintRoll(Random? rng = null)
    {
        var faces = RollEach(rng);
        var total = Modifier;
        foreach (var face in faces) total += face;

        var original = Console.ForegroundColor;
        if (IsCritical(faces)) Console.ForegroundColor = ConsoleColor.Green;
        else if (IsFumble(faces)) Console.ForegroundColor = ConsoleColor.Red;

        Console.WriteLine($"  {this,-8} [{string.Join(" ", faces)}] {(Modifier != 0 ? $"{Modifier:+#;-#} " : "")}= {total}");
        Console.ForegroundColor = original;
    }

    /// <summary>The dice, drawn in a little box, for the character sheet screen.</summary>
    public string ToBoxArt()
    {
        var label = ToString();
        var inner = label.PadLeft((11 + label.Length) / 2).PadRight(11);
        return $"┌───────────┐{Environment.NewLine}" +
               $"│{inner}│{Environment.NewLine}" +
               $"│ {Minimum,3} - {Maximum,-3} │{Environment.NewLine}" +
               $"└───────────┘";
    }

    /// <summary>A natural max on every die is a critical hit.</summary>
    public bool IsCritical(int[] faces)
    {
        foreach (var face in faces)
            if (face != Sides) return false;
        return faces.Length > 0;
    }

    /// <summary>A natural 1 on every die is a fumble.</summary>
    public bool IsFumble(int[] faces)
    {
        foreach (var face in faces)
            if (face != 1) return false;
        return faces.Length > 0;
    }

    /// <summary>Did this roll meet the difficulty class?</summary>
    public bool Beats(int roll, int difficultyClass) => roll >= difficultyClass;

    /// <summary>Roughly how often these dice beat a DC, found by trying it a lot.</summary>
    public double ChanceOfBeating(int difficultyClass, int samples = 10000, Random? rng = null)
    {
        rng ??= Random.Shared;
        var wins = 0;
        for (var i = 0; i < samples; i++)
            if (Roll(rng) >= difficultyClass) wins++;
        return (double)wins / samples;
    }
}
