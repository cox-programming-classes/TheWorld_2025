# Lesson 3 - Extensions and Separating Concerns

```
dotnet run
```

Everything works today, and will keep working.  That's the uncomfortable part:
you're about to spend a class period changing code that is behaving perfectly,
and you need to be able to say what you bought.

| File | What it is |
|---|---|
| `Dice.cs` | Your dice, four weeks later, after everyone added one small thing. |
| `Display/DiceDisplay.cs` | Where display goes.  Two methods already here, as the example. |
| `Rules/DiceRules.cs` | Where your game's rules go.  Nearly empty, on purpose. |
| `Program.cs` | Five scenes.  **These barely change all lesson.** That's the point. |

## Read `Dice.cs` first

All of it, before you touch anything.  Then answer:

> How many different jobs is this type doing?

Every method in that file was a reasonable idea on the day it was written, put
there by somebody doing their job.  That's how it always happens.

## The three ingredients

An extension method is exactly three things:

```csharp
public static class DiceDisplay                  // 1. static class
{
    public static string Describe(this Dice dice) // 2. static method
    {                                             // 3. `this` on parameter one
        ...
    }
}
```

Once those are true, `dice.Describe()` compiles as though `Describe` had always
been part of `Dice`.

## What you're actually buying

The call site stays put.  `dice.PrintHistogram()` reads the same before and
after, so the value lies elsewhere -- and it does:

- `Dice.cs` stops needing `Console`.  It stops knowing about colors, terminal
  widths, and box-drawing characters.  **A type that knows what a screen is can
  only ever live where screens are** -- which rules out a test, a server, and a
  web app in one stroke.
- Everything about how dice *look* lands in one file, so the next person who
  wants to change that has one place to go.
- Your game's rules stop being smuggled inside a type that claims to be about
  dice in general.

And the cost, which is real:  you now have to know that `Describe` lives in a
file you may have closed.  Namespaces are how you make that findable.  It is a
trade, and that is what you pay.

## Extensions cover more than methods

C# 14 lets you add **members** of any kind -- properties included -- to a type
somebody else owns.  Two flavours, and you'll want both:

```csharp
extension(Dice)                              // bare type name -> STATIC members
{                                            // these hang off the TYPE
    public static int MaxCount => 100;       //   Dice.MaxCount
}

extension(Dice dice)                         // with a name -> INSTANCE members
{                                            // these hang off a VALUE
    public bool IsLegalHere =>               //   myDice.IsLegalHere
        dice.Count <= Dice.MaxCount;
}
```

Same three ingredients:  a static class, a static member, a receiver declared up
front.  Same rule too -- **a real member on the type always beats an extension
member with the same name.**

The consequence worth sitting with:  once `MaxCount` is an extension property,
`Dice.MaxCount` reads *exactly* as it did when it was a real member.  Every call
site in the program reads them the same way.

## The test for where something belongs

Imagine two games using this exact `Dice` type:  your dungeon crawler, and
somebody's board game about trains.

| | |
|---|---|
| The trains game needs it to behave identically | it's part of what dice **are** -- leave it |
| The trains game would want it to mean something else, or drop it entirely | it's a rule of **your game** -- move it |

Apply that to `IsCritical`, `IsFumble`, `Beats`, and `ChanceOfBeating`.  They
give different answers, and at least one is genuinely arguable.

## Two surprises, one fact

**Scene 4.** There are two methods named `Describe` in scope -- an instance
method on `Dice`, and an extension method in `DiceDisplay`.  Only one of them
ran, and it won cleanly.  The extension method has been sitting there being
ignored this entire time.

**Scene 5.** `missing.Summarize()` is called on `null` and comes back clean.

Both are the same fact:  `dice.Summarize()` is **a static call in disguise**.
The compiler rewrote it into `DiceDisplay.Summarize(dice)` before the program
ever ran, using the type the variable is *declared* as, leaving what it holds
at runtime out of it.

So an extension method binds at compile time and stays fixed there, with
`virtual` out of its reach.  What you're buying is readability.  That's worth a
lot.  Spend it on that alone.

## Namespaces are the fence

`using Toolkit.Display;` earns its place.  It's this file admitting to a
dependency.  Delete it and watch exactly which lines stop compiling -- that's a
dependency graph, printed by the compiler, for free.

## Today's steps

**The rule for the whole lesson:  `Program.cs` stays exactly as it is.** If a move
requires editing a call site, you moved something wrong.

1. Read `Dice.cs`.  Count the jobs.  Write the list down.
2. Move `PrintHistogram` into `DiceDisplay`.  Re-run -- **output byte-for-byte
   identical, `Program.cs` untouched**.  That's what refactoring means:  the
   behavior is the control group.
3. Sort the four rule-ish methods using the trains test, move them, *then* move
   `PrintRoll` (it depends on two of them, so it moves last).
4. Move `ToBoxArt`.  `Dice.cs` should now be about half its old size.  Re-read
   what's left -- does it match your Step 1 list?
5. Delete `Dice.Describe()`, the instance method.  Re-run Scene 4 and watch the
   extension finally get a turn.
6. Add one presentation this file has yet to carry -- `ToShortString()`, a table
   of dice in columns, `ToOdds(int dc)`.  It should leave `Dice.cs` untouched.
7. **Round two:  the numbers.** Run the trains test on the four limits at the top
   of `Dice.cs`.  Three of them fail it.  Move those three into an
   `extension(Dice)` block in `DiceRules` -- the lines copy across *unchanged* --
   and delete them from `Dice.cs`.

   Two things happen and both are the lesson:

   - `IsLegalHere` keeps working, untouched.  It still says `Dice.MaxCount` and
     reads it the same way either side of the move.
   - `Dice`'s own guards stop compiling.  **Resist fixing that with a `using`** --
     that would make the dice depend on one game's rulebook.  Fix it by asking
     what those guards were doing there.

   > `Dice`'s job:  guarantee this is a die.
   > `DiceRules`' job:  decide whether it's a die you may use *here*.

   One guard vanishes from `Dice` entirely.  Work out which, and say why that is
   the right outcome.
8. Delete a `using`.  Read what breaks.  Put it back.  Did anything break that
   caught you by surprise?  Those are the good ones.
9. Cards -- the challenge, in your own `Card.cs`.

## Done for today?

> Name one thing you left on `Dice`, and one thing you moved off it.  For each,
> say who would disagree with you and what they'd want instead.
