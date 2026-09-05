# Lesson 3 - Extensions and Separating Concerns

*Some of what a thing can do belongs somewhere else.*

**Two meetings.**  Starter: [`starters/Lesson03_ExtensionsAndConcerns/`](../starters/Lesson03_ExtensionsAndConcerns/) --
four files:  `Dice.cs`, `Display/DiceDisplay.cs`, `Rules/DiceRules.cs`, `Program.cs`.

---

## The One New Idea

> **Behavior can live beside the data it describes.**

This is the lesson where the functional bent stops being a style preference and
starts paying rent.  In the shape they were taught, a type is data *plus* every
behavior anyone will ever want from it, and it grows forever.  In the shape C#
has been moving toward, the data is one small thing and behaviors are modules of
functions that sit beside it -- added, swapped, and dropped while the data
type ever knowing.

Extension methods are how C# makes that readable.

<!--
The three lessons form one argument, and it's worth saying to them at the
top of today:

    L1: the data is fixed once it is built
    L2: the data arrives valid or refused
    L3: the data does very little

Each one makes the next possible.  You can only move behavior off a type
safely because the type is immutable and always valid -- every caller is
already held at arm's length, so ordinary access is all anyone requires.
That is the actual reason this style works, and clever students
will appreciate hearing it said outright.
-->

## Learning Goals

Students will be able to:

- Write an extension method -- and an extension *property*, instance or static.
- Decide whether a behavior belongs *on* a type or *beside* it, using a test
  they can apply to code they are seeing for the first time.
- Apply that same test to a **constant**, and notice that a number can encode
  policy just as much as a method can.
- Use namespaces and `using` deliberately -- as a statement about what a file is
  allowed to depend on.
- Explain what extension methods do in place of polymorphism, and predict the
  two places that bites.

## New Vocabulary

- **extension method** -- a static method C# lets you call as though it were an
  instance method on another type.
- **extension member** -- the general form.  C# 14's `extension(Dice)` /
  `extension(Dice dice)` blocks add properties too, static or instance.
- **separation of concerns** -- keeping "what a thing is," "how it's shown," and
  "what it means here" in different places.
- **coupling** -- what a piece of code has to know about in order to work.
- **refactoring** -- changing structure while behavior stays identical.  The output
  is the proof.
- **compile-time binding** -- the compiler picks which method runs, using the type
  the variable is *declared* as, leaving what it has at runtime out of it.

---

<!--
## Warm-Up (~8 min)

Hold off on running anything.  Open Dice.cs on the projector and scroll through it
slowly, top to bottom, in silence.  It is around 220 lines.

Then one question:

  "How many different jobs is this type doing?"

Take the list on the board.  They'll get to roughly:

    what dice ARE          Count, Sides, Modifier, the guards
    how dice ROLL          Roll, RollEach, Minimum, Maximum, Average
    how dice are MADE      Parse, TryParse, the static D4..D20
    how dice LOOK          Describe, PrintHistogram, PrintRoll, ToBoxArt
    what dice MEAN here    IsCritical, IsFumble, Beats, ChanceOfBeating

Then two questions that set up everything:

  1. "Which of these did somebody do wrong?"
     Every one of them.  Every method was a reasonable idea on the day it was written.
     Say that explicitly.  This is a lesson about accumulation, and a student
     who reads it as a lesson about bad code learns the wrong thing entirely
     -- namely, be clever up front.

  2. "Dice.cs uses Console.ForegroundColor.  What does this type now require,
      in order to exist?"
     -> A terminal.  A screen.  A human looking at it.
     Then:  "We want to unit-test this.  And put it on a web page.  And run it
     on a server where the console is absent entirely."

Leave this on the board all lesson:

    A type that knows what a screen is can only ever live where screens are.

Now run it.  Everything works, beautifully.  That's the point.
-->

<!--
## Direct Instruction (~12 min)

### 1.  The three ingredients (3 min)

Open Display/DiceDisplay.cs.  Point at exactly three things:

    public static class DiceDisplay                1. static class
        public static string Describe(             2. static method
            this Dice dice)                        3. `this` on parameter one

That's the whole mechanism.

### 2.  Why bother, if the call site is identical?  (3 min)

The call site reads the same either way, so the value lies elsewhere.  Ask
where.  Steer to:

    * Dice.cs stops needing Console -> testable, serveable, embeddable
    * all display lands in one file -> one place to change how dice look
    * the game's rules stop hiding inside a type claiming to be general

Then the honest cost, in your words, before they find it themselves:

    "You now have to know Describe exists in a file you may have closed.
     That's real.  Namespaces make it findable.  It is a trade, and here is
     what you pay."

### 3.  The test for where things go (3 min)

The transferable part -- the thing that outlives the syntax.

    "Two games use this exact Dice type.  Your dungeon crawler, and somebody's
     board game about trains.

       Does the trains game need this to behave identically?
         -> it's part of what dice ARE. Leave it.
       Would the trains game want it different, or drop it entirely?
         -> it's a rule of YOUR game.  Move it."

Apply it live to the four candidates.  IsCritical and Beats go fast.
ChanceOfBeating is the argument:  pure math about the dice, phrased entirely
in one game's difficulty vocabulary.  Let them fight.  Both placements are
defensible; saying why is the assessment.

If Lesson 2 left "is 1d20-50 valid?" hanging -- and it should have -- this is
where it lands.  That check belongs in Rules.  It always did.  There was
nowhere to put it.

### 4.  The two surprises (3 min)

Have them PREDICT both before running.

Scene 4: two methods named Describe are in scope -- an instance method on
Dice, and the extension in DiceDisplay.  The instance one wins.  The extension
has been sitting there being ignored since they opened the project.

Scene 5: missing.Summarize() is called on null and returns cleanly.

Same fact, twice:  dice.Summarize() is a static call in disguise.  The
compiler rewrote it to DiceDisplay.Summarize(dice) before the program ran,
using the DECLARED type.

    "So an extension stays fixed:  it resolves at compile time, and `virtual`
     is out of its reach.  What you're buying is readability.  That's worth a
     lot.  Spend it on that alone."

Leave `virtual` alone.  Name it as Lesson 5's business and leave the hook.

### 5.  Round two - the numbers (2 min, at the top of Meeting B)

Save this for Meeting B, on its own.

Show the extension block syntax in Rules/DiceRules.cs and name the two
flavours:  extension(Dice) for static members, extension(Dice dice) for
instance members.  Then point at the four limits on Dice and ask the trains
test one more time.

The moment worth engineering:  have them predict what breaks when MaxCount
leaves Dice.  They will say "IsLegalHere," because it says Dice.MaxCount.
It keeps working, because it reads them the same way.  What breaks is Dice's OWN guard,
which is the one place they expect to stay safe, and it breaks because Dice
was quietly enforcing a house rule that belonged to the table.

Then let them find that Modifier's guard has nowhere to go at all.  That
guard was 100% policy.  It leaves, and Dice is more correct for losing it.

Expect pushback:  "so anyone can make 1d20-500 now?" Yes.  And it is a
perfectly good die that this table refuses.  The type's job is to guarantee a
die, and the rulebook belongs elsewhere.  Sit in that discomfort with them for
a minute and leave it there -- it is the same discomfort that puts every rule
in the constructor in the first place.
-->

## Guided Activity

Everything works today.  You're about to change code that works, and you need to
be able to say what you bought.

**The rule for the whole lesson:  `Program.cs` stays exactly as it is.**  If a move
requires editing a call site, you moved something wrong.

### Step 1 - Read `Dice.cs`, all of it

Count the jobs.  Write the list down; you'll check against it at the end.

### Step 2 - Move `PrintHistogram`

Into `Display/DiceDisplay.cs`:

1. Copy the whole method over.
2. Make the first parameter `this Dice dice`.
3. Fix the calls inside -- bare `Roll(rng)` becomes `dice.Roll(rng)`, `Minimum`
   becomes `dice.Minimum`.
4. Delete it from `Dice.cs`.

Re-run.  **Output byte-for-byte identical, `Program.cs` untouched.**

That is what refactoring means, and it is a technique you can practise:  *the
behavior is the control group.*  Output that changed means you edited.

### Step 3 - The hard one

`PrintRoll` calls `IsCritical` and `IsFumble`, and those are game rules.  So it
moves only after you have decided where *those* go.

Use the trains test.  Sort all four:

| | stays on `Dice` | moves to `DiceRules` |
|---|---|---|
| `IsCritical(int[] faces)` | | |
| `IsFumble(int[] faces)` | | |
| `Beats(int roll, int dc)` | | |
| `ChanceOfBeating(int dc, ...)` | | |

At least one will split the room.  Argue it, place it, and write one sentence in
the file saying why.

Then move them, then move `PrintRoll`.

### Step 4 - Move `ToBoxArt`

Straightforward.  `Dice.cs` should now be about half its old size.

Re-read what's left.  Does it match your Step 1 list?

### Step 5 - The method that's been ignored all along

Scene 4 calls `dice.Describe()`.  There are **two** methods that could have
answered -- an instance method on `Dice`, and an extension in `DiceDisplay`.

Work out which one ran.  Then predict what the line prints once you delete the
other.  Then delete `Dice.Describe()` and check.

> The extension method was there the whole time, correct, in scope, and
> completely inert, and every tool you have stayed silent about it.  **An
> instance method always wins, and it wins quietly.**

### Step 6 - Add a presentation of your own

A new one:  `ToShortString()`, a table of several dice in
columns, `ToOdds(int dc)` printing `"needs 15+ : 30%"`.

It should leave `Dice.cs` alone.  If it reaches in there anyway, say what that
means -- that is information worth having.

### Step 7 - Round two:  the numbers

Everything so far moved *methods*.  Now run the trains test on the four limits at
the top of `Dice.cs`:

```csharp
public const  int MinSides    = 2;
public static int MaxCount    => 100;
public static int MaxSides    => 1000;
public static int MaxModifier => 20;
```

Does a board game about trains need this exact number for the thing to be a die
at all -- or is it just what *this table* happens to allow?

**Three of them fail.**  Move those three into an `extension(Dice)` block in
`DiceRules`.  The lines copy across completely unchanged -- that's the point -- and
then delete them from `Dice.cs`.

Two things happen, and both are the lesson:

1. `IsLegalHere` keeps working, **untouched**.  It still reads `Dice.MaxCount`.
   It reads a real member and an extension member the same way,
   and so does every other line in the program.
2. `Dice`'s own guards stop compiling, because the limits have left its sight.
   **Resist fixing that with a `using`** -- that would make the dice
   depend on one game's rulebook, which is backwards.  Fix it by asking what
   those guards were ever doing there.

> `Dice`'s job:  guarantee this is a die.
> `DiceRules`' job:  decide whether it's a die you're allowed to use *here*.

One of the three guards disappears from `Dice` entirely.  Work out which, and be
ready to say why that is the right outcome.

Re-run Scene 3. `1d20-50` now *builds* -- it is a perfectly well-formed die -- and
reports `legal here?  False`.

**That's the question Lesson 2 left hanging.**  There was nowhere to put the
distinction last week.  There is now.

### Step 8 - Prove the fence is real

Delete `using Toolkit.Display;` from the top of `Program.cs`.  Read every error.
That list is a dependency graph, printed by the compiler, for free.  Put it back.

Now the interesting version:  did anything break that caught you by surprise?

---

## Starter Code

[`starters/Lesson03_ExtensionsAndConcerns/`](../starters/Lesson03_ExtensionsAndConcerns/) --
runs with zero modifications.

```
Dice.cs                    ~220 lines doing five jobs
Display/DiceDisplay.cs     where display goes -- two methods already here
Rules/DiceRules.cs         where your game's rules go -- one extension property
Program.cs                 five scenes, which barely change all lesson
README.md                  the three ingredients, extension members, the trains test
```

---

## Make It Yours - the Cards challenge

*In your own `Card.cs`, carried forward.*

Split it into three:  `Toolkit.Cards` (what it is), `Toolkit.Cards.Display` (what
it looks like), `Toolkit.Cards.Rules` (what it's worth).

Display is mechanical:  a short form (`A♠`), a long form (`Ace of Spades`), a
drawn card.

The rules half is where this gets real, because **a card's value belongs to the
game being played**:

> In blackjack an ace is 1 or 11, and a king is 10.
> In war a king beats a jack and an ace beats everything.
> In hearts the queen of spades is worth thirteen points of misery.

One `Card` type.  Three games.  If `card.Value` is a property on `Card`, which of
those three did you just pick a side in -- and where does that leave the others?

Write the rules for **two** of those games as separate extension classes.  Then
look at the two files side by side and say what that arrangement bought you.

**Prove the separation is real:**  delete a `using` and show that exactly the code
you expected stopped compiling.

---

## Homework (≤15 min)

Write **one** extension method that answers a question about your card, in
whichever file it belongs in.

Ideas:  `IsFaceCard()`, `IsSameSuitAs(Card other)`, `BeatsInWar(Card other)`,
`BlackjackValue()`.

One method.  The placement is the assignment -- be ready to say why
it went where it went.

---

<!--
## Wrap-Up (~5 min, end of Meeting A)

Original Dice.cs and their new one, side by side.  Ask:

  "The program does exactly what it did this morning.  Same output, same
   speed, same everything.  What did you get?"

Draw out:
  * Dice.cs can now be tested with the console left out
  * one place to go to change how dice look
  * the game's rules now sit where anyone can see they are rules

Then, honestly, the cost:
  * three files where one used to do
  * you have to know Describe lives somewhere out of sight

Name the cost honestly.  A student who believes every refactor is a
free win will refactor everything, forever.

Exit ticket:

  "Name one thing you left on Dice and one thing you moved off it.  For each,
   who would disagree with you?"

## Meeting B (~50 min)

  5 min   Recap:  the trains test.  That's the whole recap.
 15 min   Finish Steps 5-8.  Protect time for Steps 5 and 7 -- do both together.
 25 min   Cards.  The blackjack/war split is the centre of it.
  5 min   Two students put their two rules files up.  Ask the room:  "could a
          THIRD game drop in with Card.cs untouched?"
-->

<!--
## Teacher Notes

### The move that matters most

Step 2, done on the projector WITH them, out loud, before they read it.
The moment to hit:  run before, run after, point out that Program.cs is
untouched and the output is identical.

For most students this is the first refactor they have run with a control
group.  They change structure and behaviour at once, something breaks, and the
cause is anyone's guess.  Name the technique.  They'll use it for life.

### Where they will go wrong

  * Moving PrintRoll before deciding where IsCritical lives, hitting a
    compile error, and moving IsCritical into Display to make it stop.
    Very common, entirely logical, exactly wrong.  Ask:  "so a natural 20 is
    a display concern now?"
  * Making EVERYTHING an extension, including Roll.  Ask what happens when
    they want a WeightedDice that rolls differently.  Leave it hanging --
    that's Lesson 5, and this is the best possible way for the question to
    arrive.
  * Losing the `this` keyword and getting a confusing error.  One-line fix,
    but let them read the error first.

### The Cards argument

The blackjack-vs-war split is the real assessment.  The strongest students go
somewhere genuinely interesting:  an ace is 1 OR 11 depending on the rest of
the hand, which makes blackjack's value a function of the HAND
-- it's a function of the HAND. That is a real insight about where a rule
lives and it should be praised loudly.  It is also Lesson 4 arriving early.

Weaker responses put Value on Card with a comment saying "blackjack rules."
Fine for today.  Ask one question -- "what does the war game do now?" -- and
move on.  They'll hit it again when hands become real.

### On keeping this in proportion

Separation of concerns is the idea in this course most likely to be
over-applied.  Some will want six files per type.  Say the quiet part:

    "Three similar lines beat a premature helper.  Split when the mixing has
     cost you something you can name."

Dice.cs earned its split by growing for four fictional weeks.  A forty-line
type has earned the right to stay one file.

### If they finish early

GameData/GameMechanics/ versus Engine/States/ in the finished game.  Same
split, thirty files, and every console call in the domain model still goes
through IGameIO. That interface is the seam.  Ask them to find the one place a game state
actually writes output.

### Forward hooks planted today

  * "extensions resolve at compile time"    -> Lesson 5, virtual, WeightedDice
  * "value depends on the hand"             -> Lesson 4, collections
  * "where does 1d20-50 get rejected?"      -> answered today, finally
-->
