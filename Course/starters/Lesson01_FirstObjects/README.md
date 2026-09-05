# Lesson 1 - First Objects

```
dotnet run
```

Two files.  Both already work.  Read them, run them, then make them honest.

| File | What it is |
|---|---|
| `Dice.cs` | A set of dice.  Three numbers and a way to roll them. |
| `Program.cs` | Three scenes.  Run them in order; each sets up the next. |

## The object you already know how to write

You have built `Dice` before, in another language.  Some fields, a getter and a
setter on each, methods underneath.  That shape gets taught first almost
everywhere:  *an object is a box of state, and methods are how you reach in and
change it.*

This course leans the other way, because modern C# does:

| The shape you were taught | The shape we're going to build |
|---|---|
| an object **has** state you change | a value **is** its data, and holds still |
| `setSides(4)` -- reach in and modify | build a different one and use that |
| two objects are equal if they're the *same object* | two values are equal if they *say the same thing* |
| behavior lives on the object | behavior can live beside the data (Lesson 3) |

Both are defensible.  They are different bets about what goes wrong in a big
program, and each buys something the other gives up.  The second one is where C#
has been heading for a decade, and it is the one you have yet to meet.

## What `record` gives you, and what it leaves to you

`Dice` is already a `record`.  That gets you value equality for free:

```csharp
dice == other    // compares the VALUES it holds
```

Immutability is a separate decision, made per member.  Scene 2 is a record
misbehaving exactly the way any mutable object would, because the word `record`
governs comparison and leaves mutability to you.

Here is where you make that call -- one keyword, on each property:

| You write | Set when | Changed later by |
|---|---|---|
| `public int Sides { get; set; }` | any time | anybody, from anywhere |
| `public int Sides { get; init; }` | while the object is being built | sealed from then on |

(There's a third, `private set` -- "only this type may change it." That's for
things that genuinely *should* change.  It waits for Lesson 4.)

Every option here is a choice.  `{ get; set; }` is the one you make by staying
quiet.

## Picking a primitive

`Dice` is three `int`s.  That was a decision.

| Type | Holds | Reach for it when |
|---|---|---|
| `int` | whole numbers, ±2.1 billion | counting anything -- dice, gold, hit points |
| `double` | fast decimals, **approximate** | measurements, averages, physics |
| `decimal` | slow decimals, **exact** | money, or anywhere `0.1 + 0.2` must equal `0.3` |
| `bool` | `true` / `false` | a yes/no fact |
| `char` | one character, `'A'` | one character |
| `string` | text, fixed at creation | text |

Scene 1 shows why "approximate" matters, and why `7 / 2` is `3`.  That second one
runs clean, stays quiet, and sails through every test you throw at it.

## Syntax you may be meeting for the first time

```csharp
record Dice { ... }                     // a type defined by its values
new Dice { Count = 2, Sides = 6 }       // an object initializer
public int Minimum => Count + Modifier; // a property computed on demand
x switch { > 0 => "a", _ => "b" }       // pick a value based on x
$"{Count}d{Sides}"                      // string interpolation
```

## Today's steps

1. Run it.  Read Scene 2.  Explain out loud why the table's die changed -- the
   guilty line hides behind the obvious one.
2. Read Scene 3.  Say which of its three pictures Scene 2 is.
3. In `Dice.cs`, change all three `set` to `init`.  Build.  **Read both errors.**
4. Fix Scene 2, leaving `set` where it lies.

   The die is sealed now, so you will have to *build a different one* -- a
   `1d20+5` of your own, leaving the table's `1d20` alone.  That is the whole
   move this style rests on:  **construction replaces modification.**
5. Fix Scene 3.  This one is harder:  you were proving "one die, two names" *by
   mutating it*, and the compiler has closed that route.
   `ReferenceEquals(first, second)` will tell you the truth instead.
6. Re-run.  The table's die is untouched, you still got your +5, and
   `ReferenceEquals` still says `True`.

   **That last one is the point of the whole lesson.**  The sharing is still
   there.  `first` and `second` are one object with two names, exactly as they
   were this morning.  What changed is that it stopped mattering, because the
   thing they share holds still.

   That's what you bought:  the freedom to stop asking whether you are holding
   the original.
7. Scene 1's equality line was `True` before your change and `True` after.  Say
   why it was settled all along.

## Done for today?

> Name one thing about a set of dice you made impossible to change, and one
> thing you left changeable -- and say who would disagree with you.
