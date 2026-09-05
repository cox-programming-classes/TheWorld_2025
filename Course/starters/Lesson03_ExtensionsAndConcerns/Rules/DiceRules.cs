namespace Toolkit.Rules;

/// <summary>
/// What dice MEAN at this particular table.
///
/// Nearly empty on purpose.  Deciding what belongs in here is the hard half of
/// today's lesson, and there are two rounds of it.
///
/// ─── Round one:  the methods ───────────────────────────────────────────────
///
/// Dice.cs contains four candidates:
///
///     IsCritical(int[] faces)
///     IsFumble(int[] faces)
///     Beats(int roll, int difficultyClass)
///     ChanceOfBeating(int difficultyClass, ...)
///
/// The test that decides:  imagine two games using the exact same Dice type --
/// your dungeon crawler, and somebody's board game about trains.
///
///     * Does the trains game need this to behave identically?
///       Then it is part of what dice ARE. Leave it on Dice.
///
///     * Would the trains game want it to mean something else, or drop it
///       entirely?  Then it is a rule of YOUR game, and it belongs in here.
///
/// They give different answers, and at least one is genuinely arguable.  Be
/// ready to defend where you put it.
///
/// ─── Round two:  the numbers ───────────────────────────────────────────────
///
/// Then run the same test on the four limits at the top of Dice.cs, and find
/// that it catches them too.  See the Step 6 TODO below.
/// </summary>
public static class DiceRules
{
    // ─────────────────────────────────────────────────────────────────────────
    //  The worked example
    // ─────────────────────────────────────────────────────────────────────────
    //
    //  An `extension` block is how C# lets you add members of any kind --
    //  properties included -- to a type somebody else owns.  Two flavours, and
    //  you will want both today:
    //
    //      extension(Dice)          <- bare type name:  STATIC members.
    //      {                           these hang off the TYPE:  Dice.MaxCount
    //          public static int MaxCount => 100;
    //      }
    //
    //      extension(Dice dice)     <- with a name:  INSTANCE members.
    //      {                           these hang off a VALUE:  myDice.IsLegalHere
    //          public bool IsLegalHere => dice.Count <= Dice.MaxCount;
    //      }
    //
    //  Same three ingredients as an extension method -- a static class, a static
    //  member, and a receiver declared up front.  Same rule, too:  a real member
    //  on the type always beats an extension member with the same name.

    extension(Dice dice)
    {
        /// <summary>
        /// Whether these dice are legal AT THIS TABLE -- as opposed to whether
        /// they are dice at all, which is Dice's own business.
        /// </summary>
        public bool IsLegalHere =>
            dice.Count <= Dice.MaxCount
            && dice.Sides <= Dice.MaxSides
            && Math.Abs(dice.Modifier) <= Dice.MaxModifier;
    }

    // TODO (Step 3): move whichever of the four METHODS belong here.  Put them
    //                in an extension(Dice dice) block like the one above, and
    //                write `dice.` where the original stayed silent.

    // TODO (Step 6): now the harder one.  Look at the four limits on Dice:
    //
    //                    MinSides    = 2        (a const)
    //                    MaxCount    => 100     (a static property)
    //                    MaxSides    => 1000
    //                    MaxModifier => 20
    //
    //                Run the trains test on each.  Does a board game about
    //                trains need this exact number for the thing to be a die at
    //                all -- or is it just what THIS table happens to allow?
    //
    //                Three of them fail that test.  Move those three into an
    //                extension(Dice) block in this file.  The lines copy across
    //                completely unchanged, which is itself the point.  Then
    //                delete them from Dice.cs.
    //
    //                Two things then happen, and both are the lesson:
    //
    //                  1.  IsLegalHere above keeps working, untouched.  It still
    //                     reads `Dice.MaxCount`.  It reads a real member and an
    //                     extension member the same way -- and so does every
    //                     other line of code in the program.
    //
    //                  2.  Dice.Of() stops compiling, because the limits have
    //                     left its sight.  Resist fixing that with a `using` --
    //                     that would make the dice depend on one game's
    //                     rulebook, which is backwards.  Fix it by asking what
    //                     those guards were doing in Of() in the first place.
    //
    //                         Of()'s job:    guarantee this is a die.
    //                         Your job here:  decide whether it is a die you are
    //                                        allowed to use at this table.
    //
    //                Two of Of()'s guards get shorter.  The third disappears
    //                completely.  Work out which, and be ready to say why that
    //                is the right outcome.
    //
    //                This is also, finally, the answer to the question Lesson 2
    //                left open.  Is 1d20-50 valid?  Dice says yes -- it is a
    //                perfectly well-formed die.  This file refuses it.  Last week
    //                that distinction had nowhere to live.  Now it does.
}
