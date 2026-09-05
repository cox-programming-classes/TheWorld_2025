namespace Toolkit;

/// <summary>
/// A set of dice:  Count dice with Sides sides, plus a flat Modifier.
/// So Count=2, Sides=6, Modifier=3 is the thing a tabletop player writes
/// as "2d6+3".
///
/// Three numbers and a way to roll them.  That's the whole object.
///
/// This is a `record`, which is C#'s way of saying "this type is defined by
/// the values it holds." You get some things for free:  two
/// separately-made 2d6+3s compare as equal, and `with` makes modified copies.
///
/// Notice what is already true down in Roll(): rolling dice leaves the dice
/// alone.  It reads them and hands back a number.  Hold onto that.
///
/// Notice also what is true up here:  every value can be changed by anybody,
/// at any time, from anywhere.  `record` allowed all of it, and it always
/// would have.  Scene 2 is what that costs.
/// </summary>
public record Dice
{
    /// <summary>How many dice are rolled.  The 2 in "2d6+3".</summary>
    public int Count { get; set; } = 1;

    /// <summary>How many sides each die has.  The 6 in "2d6+3".</summary>
    public int Sides { get; set; } = 6;

    /// <summary>A flat amount added to the total after rolling.  The +3 in "2d6+3".</summary>
    public int Modifier { get; set; } = 0;

    /// <summary>
    /// Roll the dice.  Sums Count rolls of a Sides-sided die, then adds Modifier.
    /// </summary>
    /// <param name="rng">
    /// Where the randomness comes from.  Pass in a Random and you decide what
    /// "random" means -- including, when it is seeded, making it repeatable.
    /// </param>
    public int Roll(Random rng)
    {
        var sum = Modifier;
        for (var i = 0; i < Count; i++)
            sum += rng.Next(1, Sides + 1);   // Next(1, 7) gives 1..6 -- the top is exclusive
        return sum;
    }

    /// <summary>The smallest total these dice can produce.</summary>
    public int Minimum => Count + Modifier;

    /// <summary>The largest total these dice can produce.</summary>
    public int Maximum => Count * Sides + Modifier;

    /// <summary>
    /// A record writes a ToString() for free, but the free one prints
    /// Dice { Count = 2, Sides = 6, Modifier = 3 }.
    /// A player would rather read "2d6+3".
    /// </summary>
    public override string ToString()
    {
        // A switch expression:  pick one of these three based on Modifier.
        var mod = Modifier switch
        {
            > 0 => $"+{Modifier}",
            < 0 => $"{Modifier}",    // already carries its own minus sign
            _ => ""
        };
        return $"{Count}d{Sides}{mod}";
    }
}
