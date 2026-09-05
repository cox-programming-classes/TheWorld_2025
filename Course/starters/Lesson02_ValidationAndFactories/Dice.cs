namespace Toolkit;

/// <summary>
/// A set of dice:  Count dice with Sides sides, plus a flat Modifier.
///
/// This is your Dice from Lesson 1, with the change you made -- the three
/// properties are init-only, so a set of dice is fixed once it exists.
///
/// Which leaves exactly one moment when dice could be wrong:  the moment they
/// are built.  So this version closes that moment too.  The constructor is
/// PRIVATE, and this file is the only place that can reach it.  Every way to
/// make dice goes through a factory method below, and that is where the rules
/// live.
///
/// One door in.  One place the rules are written.  That is the whole design.
/// </summary>
public record Dice
{
    // Two different forms below, and the difference is on purpose.  The form
    // you choose is a claim about how permanent the number is.

    // MinSides is a `const`, because it is structural.  A random result needs
    // at least two faces to choose between -- in this game, in a board game
    // about trains, in any game anyone will ever write.  That number is
    // permanent, so the compiler may as well bake it in.
    //
    // Worth knowing while you are here:  a const is ALREADY static.  You reach
    // it as Dice.MinSides, straight off the type, and writing
    // `public static const` is a compile error -- CS0504.  If you learned
    // `static final` somewhere else, your fingers will try it exactly once.

    /// <summary>The fewest sides a die may have.  Two faces is the floor for randomness.</summary>
    public const int MinSides = 2;

    // The next three are static properties, because they are house rules
    // as opposed to structure.  They are this table's numbers, and another game
    // would pick different ones and still be playing with dice.
    //
    // Written this way they can be changed, read from a config, or moved out
    // of this type entirely.  Hold that thought; Lesson 3 moves them.

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
    /// Private, and that is the point.
    ///
    /// It puts the three numbers where they go and trusts them completely.  It
    /// can afford that, because this file is the only place that can reach it.
    /// Everything that CAN call it is a factory method twenty lines below, and
    /// those do the checking.
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

    /// <summary>
    /// Builds a set of dice, or refuses to.  The general-purpose front door;
    /// everything else in this section eventually comes through here.
    /// </summary>
    /// <param name="count">How many dice.</param>
    /// <param name="sides">How many sides on each.</param>
    /// <param name="modifier">A flat amount added to the total.</param>
    public static Dice Of(int count = 1, int sides = 6, int modifier = 0)
    {
        // A guard clause:  check it, complain immediately, get out.  The
        // alternative is to accept it, carry on, and fail somewhere else an
        // hour later, with the origin of the bad value long since lost.
        if (count < 1 || count > MaxCount)
            throw new ArgumentOutOfRangeException(
                nameof(count), count, $"A roll uses between 1 and {MaxCount} dice.");

        // TODO (Step 2): `sides` is wide open.  One side makes a token, zero
        //                sides makes a paradox, and -4 sides makes
        //                Random.Next() throw from deep inside the standard
        //                library, which is a genuinely miserable way to find
        //                out.  Use MinSides and MaxSides.

        // TODO (Step 3): `modifier` is wide open too.  Use MaxModifier.
        //                Decide for yourself whether a negative modifier is
        //                legal -- it is, but how negative?

        return new Dice(count, sides, modifier);
    }

    /// <summary>An attack roll:  d20 plus whatever you are good at.</summary>
    /// <param name="bonus">The attacker's bonus to hit.</param>
    public static Dice Attack(int bonus = 0) => Of(1, 20, bonus);

    /// <summary>Rolling up a character: 4d6.</summary>
    public static Dice AbilityScore() => Of(4, 6);

    // TODO (Step 5): add three archetypes your game would actually use.
    //                Ideas, but yours are better:  Damage(int sides),
    //                Initiative(), Percentile().
    //
    //                Notice that Attack() and AbilityScore() leave every rule
    //                to Of(), and inherit every check it makes.  Yours should
    //                too.
    //
    //                Then answer the question that matters:  which of these
    //                deserve to be methods here, and which are just Of(...)
    //                wearing a hat?

    /// <summary>
    /// Looks up dice by name -- for a save file, a config, a console command.
    /// </summary>
    /// <param name="key">The name to look up.  Case and spacing are cleaned up for you.</param>
    /// <returns>The dice, or null when the key is unknown.</returns>
    public static Dice? ByName(string? key) => key?.Trim().ToLowerInvariant() switch
    {
        "attack" => Attack(),
        "ability" or "ability score" => AbilityScore(),
        "d4" => D4,
        "d6" => D6,
        "d20" => D20,
        _ => null      // an unknown key is Tuesday, and the caller handles it
    };

    // ─────────────────────────────────────────────────────────────────────────
    //  Making dice out of text
    //
    //  Everything above protects us from bad code.  This protects us from bad
    //  DATA -- a rulebook file, a mod someone wrote, a player typing at a
    //  prompt.  Those are different problems and they deserve different answers.
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Turns dice notation into Dice, or throws trying.
    /// Use this when the text came from you, and a failure means you made a mistake.
    /// </summary>
    /// <param name="notation">Dice notation, like "2d6+3".</param>
    public static Dice Parse(string notation)
    {
        if (!TryParse(notation, out var dice))
            throw new FormatException($"'{notation}' fell outside dice notation.  Try something like 2d6+3.");
        return dice;
    }

    /// <summary>
    /// Tries to turn dice notation into Dice.
    /// Use this when the text came from somewhere beyond your reach, and a
    /// failure counts as Tuesday.
    /// </summary>
    /// <param name="notation">Dice notation, possibly nonsense.</param>
    /// <param name="dice">The dice, if it worked.</param>
    /// <returns>true if the notation was understood.</returns>
    public static bool TryParse(string? notation, out Dice dice)
    {
        dice = D6;

        if (string.IsNullOrWhiteSpace(notation))
            return false;

        // Formatting is noise:  clean it up quietly, before anything else looks
        // at it.  Everything meaningful survives, and three inputs become one.
        var text = notation.Trim().ToLowerInvariant();

        // TODO (Step 4): this handles "2d6" and stops there.  Grow it until
        //                Scene 2 loads every line that should work and refuses
        //                the rest.
        //
        //                Two things to add, in this order, running Scene 2
        //                after each:
        //                  a) a missing count:  "d20" should mean "1d20"
        //                  b) a modifier:       "3d8+2", "1d4-1"
        //
        //                Useful:  text.Split('d'), int.TryParse(s, out var n),
        //                text.LastIndexOfAny(new[] { '+', '-' }), text[..i], text[i..]

        var parts = text.Split('d');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out var count))
            return false;
        if (!int.TryParse(parts[1], out var sides))
            return false;

        // Of() is still the only way to make dice, so every rule you write up
        // there applies to text from outside too.  One copy of each rule.
        try
        {
            dice = Of(count, sides);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  The dice everybody reaches for
    //
    //  One object each, shared by the whole program.  Safe only because Lesson 1
    //  made them impossible to change.
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
    //  What a set of dice DOES
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Rolls the dice.  Sums Count rolls of a Sides-sided die, then adds Modifier.
    /// </summary>
    /// <param name="rng">Where the randomness comes from.  Seed it to repeat it.</param>
    public int Roll(Random rng)
    {
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
            sum += rng.Next(1, Sides + 1);
        return sum;
    }

    /// <summary>The smallest total these dice can produce.</summary>
    public int Minimum => Count + Modifier;

    /// <summary>The largest total these dice can produce.</summary>
    public int Maximum => Count * Sides + Modifier;

    /// <summary>"2d6+3", the way a player would write it.</summary>
    public override string ToString()
    {
        var mod = Modifier switch
        {
            > 0 => $"+{Modifier}",
            < 0 => $"{Modifier}",
            _ => ""
        };
        return $"{Count}d{Sides}{mod}";
    }
}
