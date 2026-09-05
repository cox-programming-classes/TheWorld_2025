# Lesson 2 - Data Validation and Factory Methods

*Where does "is this legal?" live?*

**Two meetings.**  Starter: [`starters/Lesson02_ValidationAndFactories/`](../starters/Lesson02_ValidationAndFactories/) --
two files:  `Dice.cs`, `Program.cs`.

---

## The One New Idea

> **A type should be impossible to construct in an invalid state.**

The mechanism is one move:  **make the constructor private, and let factory
methods be the only way in.**  Guard clauses, named archetypes, and `TryParse`
are three techniques serving that one idea.

<!--
This follows directly from Lesson 1 and you should say so in the first
minute.  Once a value can only be set while the object is being built, the
moment it is built is the ONLY moment it can be wrong.  Immutability did more
than head off a bug:  it collapsed the entire validation problem down to a
single point.  Today we stand on that point and put a door there.

That argument is the reason the private constructor is worth the ceremony.
Lesson 1 is what gives it teeth, since a locked constructor means something
only when everything after construction is sealed too.
-->

## Learning Goals

Students will be able to:

- Write a guard clause that refuses a bad value where the mistake was made, and
  say what late validation costs instead.
- Make a constructor private and expose factory methods, so that every caller
  comes through the rules.
- Write factories that *compose* -- each one calling the general factory rather
  than restating a rule.
- Choose between **throwing** and **returning false**, based on where the bad
  value came from.

## New Vocabulary

- **guard clause** -- a check at the top that refuses bad input and gets out.
- **invariant** -- something true about an object for its whole life, because the
  type guarantees it for life.
- **exception** -- an error that travels up the call stack looking for someone who
  knows what to do about it.
- **factory method** -- a static method that builds an object, so construction can
  say what it's *for* and can refuse.
- **the Try pattern** -- `TryParse(input, out result)` returning `bool`.  C#'s way
  of saying "this might fail, and failure here is Tuesday."

---

<!--
## Warm-Up (~7 min)

Run the starter.  Scene 1 is a museum of bad dice and four exhibits get in:

    ROLLED    a one-sided die       1d1       -> 1, 1, 1, 1, 1
    ROLLED    a zero-sided die      1d0       -> 1, 1, 1, 1, 1
    ROLLED    a modifier from...    1d20+500  -> 508, 518, 514, ...
    ACCEPTED, THEN CRASHED  a die with -4 sides  ArgumentOutOfRangeException

Ask, in this order:

  1. "Which of these is worst?"
     Most rooms say the crash.  Push back:

  2. "The crash told you immediately, at the line where you made the
      mistake.  The 1d0 shipped.  Which one costs more?"

     This is the reframe the lesson runs on.  Loud failure is cheap.  Quiet
     wrongness is expensive.  They know this from debugging, and today is the
     first time anyone has named it for them.

  3. "Read the crash message. 'minValue cannot be greater than maxValue.'
      Whose words are those?  Did anyone here write 'minValue'?"

     It came from inside Random.  Our bad value walked out of our type, into
     somebody else's code, and detonated there -- in their vocabulary,
     pointing at their variable names.  That is what validating late buys.

Save the words "guard clause" for Direct Instruction.
-->

<!--
## Direct Instruction (~13 min)

### 1.  Immutability collapsed the problem (2 min)

Open with the callback, because it makes the whole style cohere:

    "Last week you made dice that are fixed the moment they are built.  So how many moments are
     there, now, when a set of dice can be wrong?"

One.  The moment it's built.  That is an enormous simplification and it came
free with the decision they already made.  Today we put a door on that moment.

### 2.  Fail where the mistake is (3 min)

Show the `count` guard inside Of() -- already written.  Name it.  Point at the
three parts of the exception:  which parameter, what value, what the rule is.
Contrast with the Random message they just read.

    "An error message is written for the person who has to fix it.  Write it
     for them."

### 3.  One door (5 min)

The move of the lesson.  Have them try, in Program.cs:

    var d = new Dice(0, 0);

The compiler rejects it.  The constructor is private.

    "So how did Scene 1 make dice at all?"

Every one of them went through Dice.Of().  Put the list on the board and ask
what each one has in common:

    Dice.Of(2, 6, 3)          the general front door
    Dice.Attack(3)            a named archetype   -> calls Of
    Dice.ByName("attack")     a key from a file   -> calls Attack -> Of
    Dice.TryParse("2d6+3")    text from outside   -> calls Of
    Dice.D20                  a shared preset     -> calls Of

Then the payoff, and let them be the ones to say it:

    "How many places do you have to write the rule that dice need at least
     one die?"

One.  The private constructor is what makes that guarantee real, because the
compiler enforces it.  Politeness and convention would leave it optional.

Contrast with a constructor, explicitly:

    a constructor gets ONE name and must accept what it's given
    a factory gets as many names as the game has ideas -- and can refuse

### 4.  Throw, or hand back false?  (3 min)

    Dice.Of(0, 6)           I wrote this.  If it's wrong, I have a bug.  -> throw
    Dice.TryParse(typed)    A player wrote this.  It's Tuesday.          -> false

Show TryParse in the source ending with a try/catch around Of().  Same rules,
both paths, one copy.  That's the thing to point at.

The `out` parameter is new to most of them.  One sentence:  "a second thing
the method hands back, alongside the bool." Stop there.
-->

## Guided Activity

`Program.cs` is a report card on your `Dice` type.  **Leave it alone.**  Run it
after every change and watch the output get less embarrassing.

First, though, try this in `Program.cs` and read what happens:

```csharp
var d = new Dice(0, 0);
```

The compiler rejects it.  That is the whole design, in one error message.

### Step 1 - Read the museum

Scene 1.  For each exhibit marked `ROLLED`, finish this sentence:

> *"Six months from now, this shows up as a bug report that says ______."*

The one marked `ACCEPTED, THEN CRASHED` is a different kind of bad.  Say how.

### Step 2 - Give `sides` a rule

Inside `Of()`, `count` has a guard and `sides` is wide open.  Write one, using
`MinSides` and `MaxSides`, right next to the guard that's already there.

Re-run.  Three exhibits in Scene 1 change, and Scene 2's counts shift.

### Step 3 - Give `modifier` a rule

Same place.  Use `MaxModifier`.

Decide for yourself whether a negative modifier is legal -- it is, but *how*
negative?  Write down the reason for the number you pick.

### Step 4 - Grow `TryParse`

Scene 2 loads a rulebook straight from somebody's first draft.  Right now
`TryParse` handles `"2d6"` and little else.  Two things to add, in this order,
re-running each time:

- **a.**  A missing count -- `"d20"` should mean `"1d20"`.
- **b.**  A modifier -- `"3d8+2"`, `"1d4-1"`.

Useful:  `text.Split('d')`, `int.TryParse(s, out var n)`,
`text.LastIndexOfAny(new[] { '+', '-' })`, `text[..i]`, `text[i..]`.

**Target:**  Scene 2 loads `2d6  d20  3d8+2  1d4-1  4d6  2d6  1d20+3` and refuses
the rest.

> **Notice something.**  The trimming and lowercasing at the top of `TryParse`
> was written for you.  That's *normalizing* -- formatting is noise, so clean it
> up quietly and three inputs become one claim.  What you're adding is different:
> deciding whether a claim is **true**.  Two different jobs, and it's worth
> knowing which one you're doing.

### Step 5 - Add archetypes

Add three to `Dice` that your game would actually use.  `Damage(int sides)`,
`Initiative()`, `Percentile()` are ideas; yours are better.

**Make them call `Of()`.** If you find yourself re-typing a rule, you've gone
wrong -- look at how `Attack` and `AbilityScore` are one line each.

Then answer the question that matters:  which of these deserve to be methods, and
which are just `Of(...)` wearing a hat?

### Step 6 - Count what you touched

Look back at Steps 2 and 3.  You wrote two rules, in one method.

Now list everything that got those rules for free:  `Attack`, `AbilityScore`,
`ByName`, `Parse`, `TryParse`, and all six `Dice.D*` presets.  Every one of them
stays silent about the limits.

> That's what the private constructor bought you:  **one place to change your
> mind.**

---

## Starter Code

[`starters/Lesson02_ValidationAndFactories/`](../starters/Lesson02_ValidationAndFactories/) --
runs with zero modifications.

```
Dice.cs      private constructor, factories, one rule written and two missing
Program.cs   three scenes -- a report card on the type
README.md    one door, throw-vs-Try, normalize-vs-reject, const vs static property
```

**Note the shape change from Lesson 1.** The properties are still `{ get; init; }`
-- you made that change yourself last lesson.  What's new is that the constructor
is `private` and the public surface is a set of factory methods.

<!--
Why the longhand constructor here, when a primary constructor hands you
init-only properties for free?

Two reasons, and the second is the one that matters.

The mechanical one:  a primary constructor is always public in C#, and
`record Dice private(...)` is a parse error (CS1514).  You choose a primary
constructor or a private constructor.  "The factory is the only thing
exposed" is the point of the lesson, so the private constructor wins and
the three properties are declared longhand.  It costs two lines.

The real one:  a primary constructor is a PUBLIC front door with open space
where a rule would go.  That suits a type whose data stands on its own
perfectly well.  A save-file record, a parsed config line, a message shape:
those are DTOs, and a DTO is a shape.  Gating one behind a factory would be
ceremony guarding an empty room.

So the split students should end up with, once they meet both:

    has invariants to protect  ->  private ctor + factory   (Dice, Card)
    is a shape, data alone     ->  primary constructor      (DTOs)

Primary constructors arrive with DTOs, post-fork, wherever save/load lands.
Hold them back here.  If a student asks why the terse positional form is
missing, that is the honest answer and it is worth giving:  this type has
something to defend, and a DTO carries data alone.
-->

---

## Make It Yours - the Cards challenge

*In your own `Card.cs`, carried forward from Lesson 1.*

Do to `Card` what we did to `Dice`:

1. **Break it on purpose first.**  Before writing a single guard, construct the
   five worst Cards your design allows.  A card missing its suit.  A rank of 47.  A
   rank of -3.  Print them.  Look at what your type was willing to build.
2. Sort each failure into **noise** (fix it quietly -- `"ace of spades"`,
   `"ACE OF SPADES"`, `" AS "`) or **a claim** (refuse it -- rank 47).  Say where
   you put the line.
3. **Make the constructor private.**  Then work out what factory methods your
   game actually wants.  `Card.Of(rank, suit)`?  `Card.AceOf(suit)`?
4. Give `Card` a `TryParse`.  `"AS"` is the ace of spades, `"10H"` the ten of
   hearts.  Then survive:  `"2c"`, `" KS "`, `"1S"`, `"11H"`, `"ZZ"`, `""`, `"10"`.
5. Write a factory that hands back a **full standard deck**.  Fifty-two cards,
   each one distinct, the whole grid covered.  Prove it:  print the count.

**Two questions to be able to answer out loud:**

- Which of your rules protect against a *bug in your code*, and which protect
  against *bad data from outside it*?  Show where each kind lives.
- How many of your factory methods restate a rule?  Every one that does is a
  place your future self can forget to update.

---

## Homework (≤15 min)

Pick **one**:

- Finish `Card.TryParse` and test it against the ten strings above.
- Or, if that's done:  write down three inputs your `Card` still accepts that it
  should refuse -- and *leave them broken*.  Bring the list.

The second choice is worth as much as the first.  Finding what you got wrong is the harder
skill and it's worth more in class tomorrow than a finished file.

---

<!--
## Wrap-Up (~5 min, end of Meeting A)

Put Step 6 on the projector as a counting exercise, out loud:

  "You wrote two rules today.  How many methods enforce them?"

Eleven-ish, depending on what they added.  Zero of which mention a limit.
That is the win, and it is worth naming:  a rule written once stays in sync
with itself for free.

Exit ticket:

  "Name one rule you wrote.  Is it protecting you from a bug in your code, or
   from bad data outside it?  Show me where it lives because of that."

## Meeting B (~50 min)

  5 min   Recap:  one door.  Board diagram of everything routing into Of().
 15 min   Finish Steps 4 and 5.
 25 min   Cards.  The deck factory is what most will need help SCOPING --
          see notes.
  5 min   Wrap-up:  two students demo a Card refusing something, and say what
          they decided was noise versus a claim.

If a minute is left, start the argument you plan to leave open (below).
It makes a good thing to walk out arguing about.
-->

<!--
## Teacher Notes

### The `with` hole, which you should know about and leave alone

A record still supports `with` from outside the type, and `with` sidesteps
through your factory.  `Dice.D20 with { Sides = 0 }` would sail straight past
Of() and produce a zero-sided die.

`with` waits for a later lesson by design, and it appears nowhere in Lessons
1 or 2, so students will walk past this on their own.  If a sharp student
finds it, that is a real discovery and worth saying so:  they have found the
one door the factory leaves open, and reconciling those two is exactly why
`with` gets a lesson of its own later, alongside DTOs.

Leave it be.  A hole a student can see is the only kind worth teaching.

### The two arguments to leave open

  1. "Is '2 d 6' the same claim as '2d6'?" Genuinely arguable.  Whitespace
     inside a token stands on different footing from whitespace around it.

  2. "Is 1d20-50 valid?" The honest answer:  it is perfectly good dice
     notation and a terrible game rule, so the check belongs somewhere that
     knows about the game -- a layer still to come.  That layer is
     Lesson 3, and Lesson 3 is built to land exactly there.

If a student gets to "that belongs to the game, and the dice type is the
wrong place for it,"
write their name down.  That is the whole arc of the next lesson arriving a
week early, from a student, which is worth more than anything you can say.

The same shape appears in Cards immediately:  is a rank of 11 invalid, or is
it a Jack?  Depends on the game.  Let them find it.

### Bugs you will actually see

  * TryParse handling the modifier BEFORE the split, so "1d4-1" splits on
    'd' into "1" and "4-1".  Have them print `text` at each stage.
  * IndexOf('-') where LastIndexOfAny belongs -- works until something has a
    negative count.  Let it break; the error teaches it faster than you can.
  * New archetypes written as `new Dice(...)`, which the compiler now rejects,
    and the error is the lesson.  They must go through Of().  This is the
    private constructor doing its job in front of them; point at it.
  * Archetypes that re-check a limit "just to be safe." Ask what happens
    when the limit changes.  Two copies of a rule is one copy plus a future
    bug.
  * Deck factory built with nested loops over hard-coded string arrays.
    Works fine.  Ask what happens when a rank gets misspelled, and let that
    push them toward enums if they have yet to go there.

### On exceptions

Some will want try/catch everywhere, because an exception feels like a crash
they should prevent.  Push back once, clearly:

    "Catching an exception you are powerless to fix is worse than letting it
     through.  It hides the report and leaves the bug where it was."

Program.cs catches in exactly two places, and both are places where the
program genuinely has something to do about it.  Point at that.

### If they finish early

GameData/Items/Item.cs in the finished game validates by NORMALIZING --
negative weight becomes zero, an empty name becomes "Unknown Item", every
value coerces, and construction stays wide open.  That is the opposite call
from the one they just made.  Ask which is right.

(Both are.  It depends on whether a wrong item should stop the game.  It
should let the player keep going with a weird item.  A wrong die probably
should, because every number after it is nonsense.)

### The thing to protect

Step 6, the counting exercise.  Two rules, eleven call sites, zero
duplication.  Skip it and "make the constructor private" reads as ceremony
somebody imposed on them.  With it, it reads as the thing that made the
counting come out that way.
-->
