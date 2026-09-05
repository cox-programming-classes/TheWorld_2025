namespace Toolkit.Display;

/// <summary>
/// How dice LOOK. Everything in this file leaves what dice DO alone.
///
/// This is the worked example.  Here is the whole trick, and it is three things:
///
///     * the class is static
///     * the method is static
///     * the first parameter has `this` in front of it
///
/// Once those are true, C# lets you write `dice.Describe()` as though Describe
/// had always been part of Dice.  Dice.cs has yet to hear of it.
///
/// So why bother, if the call site looks identical either way?
///
/// Look at what this file needs to know about.  Consoles.  Colors.  How wide a
/// terminal is.  Dice.cs can be dice with all of that stripped away -- and until today it was
/// carrying all of it.
/// </summary>
public static class DiceDisplay
{
    /// <summary>
    /// One line a player could read:  what the dice are, and what they do.
    /// </summary>
    /// <param name="dice">The dice to describe.</param>
    public static string Describe(this Dice dice)
    {
        var spread = dice.Minimum == dice.Maximum
            ? $"always {dice.Minimum}"
            : $"{dice.Minimum} to {dice.Maximum}, averaging {dice.Average:0.#}";
        return $"{dice} ({spread})";
    }

    /// <summary>
    /// The same idea, and it survives being handed an empty variable.
    ///
    /// Note the `?`.  A real method would throw here, because calling one on
    /// null fails before the body ever starts.  Scene 5 is about why this one
    /// gets away with it.
    /// </summary>
    /// <param name="dice">The dice to summarise, or null.</param>
    public static string Summarize(this Dice? dice) =>
        dice is null ? "(the slot is empty)" : $"{dice}, {dice.Minimum}-{dice.Maximum}";

    // TODO (Step 2): move PrintHistogram here from Dice.cs.
    //                Copy the body across, make the first parameter
    //                `this Dice dice`, and change the bare `Roll(...)` and
    //                `Minimum` to `dice.Roll(...)` and `dice.Minimum`.
    //                Delete it from Dice.cs when it works.
    //                Program.cs should come through untouched.

    // TODO (Step 3): move PrintRoll here too.  This one is more interesting --
    //                it calls IsCritical and IsFumble, and those are game
    //                rules.  Decide where THOSE go before you move this.

    // TODO (Step 4): move ToBoxArt here.

    // TODO (Step 6): now that all the display code is in one place, add one
    //                presentation this file has yet to carry:
    //                  ToShortString()  -- "2d6+3" on its own
    //                  ToTable(...)     -- several dice lined up in columns
    //                  ToOdds(int dc)   -- "needs 15+ : 30% chance"
    //                It should leave Dice.cs untouched.  If it reaches in there
    //                anyway, that is information -- say what it means.
}
