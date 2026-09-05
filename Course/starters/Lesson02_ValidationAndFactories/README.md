# Lesson 2 - Data Validation and Factory Methods

```
dotnet run
```

`Program.cs` is a report card on your `Dice` type.  Run it now.  Run it again
after every change.  The output should get less embarrassing.

| File | What it is |
|---|---|
| `Dice.cs` | Your dice from Lesson 1 -- now with a private constructor and factory methods. |
| `Program.cs` | Three scenes.  **Leave these alone** until the very end. |

## Where we left off

Lesson 1 ended with dice that are fixed the moment they are built.  That closed off every way a set of
dice could go wrong, save one.

If a value can only ever be set while the object is being built, then the moment
it is built is the *only* moment it can be wrong.  That's a much better problem
than the one you started with, and it's today's problem.

## One door in

Open `Dice.cs` and look at the constructor.  It's **private**.

```csharp
private Dice(int count, int sides, int modifier)
{
    Count = count; Sides = sides; Modifier = modifier;
}
```

It puts the three numbers where they go and trusts them completely.  It can
afford that, because this file is the only place that can reach it.  Try writing
`new Dice(0, 0)` in `Program.cs` -- the compiler rejects it.

Everything that *can* call it is a **factory method** on `Dice` itself:

```csharp
Dice.Of(2, 6, 3)        // the general front door -- this is where the rules live
Dice.Attack(3)          // a named archetype
Dice.ByName("attack")   // a key from a save file
Dice.TryParse("2d6+3")  // text from somewhere beyond your reach
Dice.D20                // one everybody reaches for
```

And here's the part worth noticing:  **every one of the others defers to
`Of()`.** They all end up there.  `Attack` is one line.  `TryParse` does string work and
then hands off.  One door, one place the rules are written, every caller
through it.

> A constructor gets one name and has to accept whatever it's given.
> A factory gets as many names as your game has ideas -- and gets to refuse.

## Throw, or hand back false?

Two different failures.  Two different answers.

| The bad value came from | Because | So |
|---|---|---|
| your own code | you made a mistake | **throw** -- fail loudly, at the mistake |
| a file, a player, a mod, a network | Tuesday happened | **`TryParse`** -- return `false`, carry on |

```csharp
Dice.Parse("2d6+3")                        // I wrote this.  If it's wrong, that's a bug.
Dice.TryParse(playerTyped, out var dice)   // They wrote this.  If it's wrong, that's Tuesday.
```

Both go through the same guards.  `TryParse` catches the refusal and turns it
into an answer.  **One copy of every rule.**

## Normalize, or reject?

A value can be wrong in two different ways, and they want different answers.

- **Formatting is noise.**  `" 2d6 "`, `"2D6"`, `"2d6"` are one claim typed by
  three people.  Clean it up quietly; everything meaningful survives.  (`TryParse` already does
  this for you; find the line.)
- **Meaning is a claim.**  `"0d6"` says something false.  Refuse it.

The line between them can be genuinely hard to find, and *you* have to put it
somewhere.

## Syntax you may be meeting for the first time

```csharp
throw new ArgumentOutOfRangeException(nameof(value), value, "message");
nameof(value)                     // the identifier's name, as text
public const int MinSides = 2;    // fixed at compile time
public static int MaxCount => 100;// computed every time it's asked for
out var dice                      // a second thing the method hands back
key?.Trim().ToLowerInvariant()    // if key is null, stop here and hand back null
made?.ToString() ?? "fallback"    // use the left, unless it's null
text[..i]  /  text[i..]           // slice a string before / from an index
```

### Why `Dice` uses two different forms for its limits

Look at the top of `Dice.cs`.  `MinSides` is a `const`.  The other three are
`static` properties.  That is a claim about how permanent each number is.

| | Form | Because |
|---|---|---|
| `MinSides` | `public const int MinSides = 2;` | **Structural.**  A random result needs at least two faces to choose between, in this game or any other.  That number is permanent. |
| `MaxCount`, `MaxSides`, `MaxModifier` | `public static int MaxCount => 100;` | **House rules.**  This table's numbers.  Another game picks different ones and is still playing with dice. |

The form you pick tells the next reader which kind of number they're looking at.
That's worth more than the two characters it costs you.

### Three things worth knowing about `const`

1. **A `const` is already `static`.** You reach it as `Dice.MinSides`, straight
   off the type.  If you learned `static final` somewhere else, your fingers
   will try `public static const` exactly once -- that's **CS0504**.
2. **`public static int MaxCount => 100;` is a property.**  It's computed each
   time it's asked for, which is why it *needs* the `static` keyword that a
   `const` carries on its own.
3. **Only a `const` can appear in a pattern.**  The guards in `Of()` could have
   been written as patterns -- `count is >= 1 and <= MaxCount` -- which is
   idiomatic modern C#. A pattern requires a `const`, and `MaxCount` is a
   property.  Try it and you'll get **CS9135**, *"a constant value is expected."*

Go break number 3 on purpose and read the error.  Knowing which compiler errors
are real design rules and which are just syntax is worth five minutes.

## Today's steps

1. Run it.  Read Scene 1.  Which exhibit worries you most, and why?  For each
   `ROLLED` line, finish:  *"Six months from now this is a bug report that
   says ______."*
2. In `Of()`, `sides` is wide open.  Give it a rule, next to the `count` guard that's
   already written.  Use `MinSides` and `MaxSides`.  Re-run.
3. `modifier` is wide open too.  Give it a rule, using `MaxModifier`.  Decide for
   yourself whether a negative modifier is legal -- it is, but *how* negative?
4. Grow `TryParse` until Scene 2 loads what should load and refuses the rest.
   Two things to add, in order, re-running each time:
   **(a)** a missing count -- `"d20"` means `"1d20"`; **(b)** a modifier --
   `"3d8+2"`, `"1d4-1"`.
5. Add three archetypes to `Dice` your game would actually use.  Make them call
   `Of()` -- if you find yourself re-typing a rule, you've gone wrong.
6. Cards -- the challenge.  Your own `Card.cs`, carried from Lesson 1.

Between steps 2 and 5, notice how little you had to touch.  Every rule you wrote
went in one method, and `Attack`, `AbilityScore`, `ByName`, `Parse`, `TryParse`
and all six `Dice.D*` presets got it for free.

## Two arguments worth having

Both are genuinely open.  Pick a side and be able to defend it.

- **Is `"2 d 6"` the same claim as `"2d6"`?** You and the person next to you
  will disagree.  Both of you need a reason.
- **Is `1d20-50` valid?**  It parses.  It rolls.  Its best possible result is -30.
  Is that the `Dice` type's problem, or the game's?

## Done for today?

> Point at one rule you wrote.  Say whether it protects against a bug in your
> code or bad data from outside it -- and show me where it lives *because* of that.
