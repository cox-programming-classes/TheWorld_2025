# Lesson 1 - First Objects

*Complexity, and managing mutability.*

**Two meetings.**  Starter: [`starters/Lesson01_FirstObjects/`](../starters/Lesson01_FirstObjects/) --
two files, `Dice.cs` and `Program.cs`.

---

## The One New Idea

> **Immutability is a property you declare, member by member.  The word `record`
> governs something else entirely.**

Everything in this lesson serves that sentence.  `Dice` is *already* a record on
the first line of the starter, and it *already* has the aliasing bug -- because
its properties say `set`.  The fix is one keyword, three times.

<!--
Resist adding anything to this.  The first draft of this lesson also had
private setters, encapsulated mutation methods, deep-immutability leaks
through a List, and record equality breaking as a result.  Every one of
those is a good lesson, and every one belongs to a later day.

Where the cut material went:
  { get; private set; } and mutation-through-methods  -> Lesson 4 (collections
      need it:  a Deck's contents genuinely change)
  deep immutability / a record holding a List         -> Lesson 4, same reason
  ReferenceEquals vs ==                               -> stays, but only as the
      TOOL for Step 5, and only there
-->

## Learning Goals

Students will be able to:

- Build a complex type out of primitives, and defend each primitive choice.
- Predict when assigning one variable to another makes a copy and when it makes
  a second name for the same thing.
- Change a mutable type into an immutable one and repair the call sites the
  compiler flags.
- Say what immutability actually bought, which goes well past "the bug went away."

## New Vocabulary

Introduced only when there is something on screen to attach it to.

- **value type** -- the variable holds the thing itself.  `int`, `double`, `bool`,
  `char`.  Copying the variable copies the thing.
- **reference type** -- the variable holds *where the thing is*.  Every `class` and
  every `record`.  Copying the variable copies the address.
- **immutable** -- fixed once it exists.  `string` already is.  `Dice` will be.
- **record** -- a type C# compares by the values it holds.
- **init-only** -- settable while the object is being built, and fixed from then on.
- **aliasing** -- two names for one object.

Held back on purpose:  `struct`, `private set`, the heap and the stack, boxing,
defensive copies, and deep versus shallow immutability.

---

<!--
## Warm-Up (~7 min)

Run the starter on the projector and stay quiet.  Scene 2 prints:

    The die on the table:  1d20
    Your die:              1d20+5
    The die on the table:  1d20+5

Ask, in this order, and let them sit:

  1. "What did I do wrong?"
     Let them hunt for a typo.  Every line is correct.

  2. "Who has shipped this bug?"
     Most of them have, in JS or Python, with a list or a dict.  Get two or
     three to describe theirs out loud.  The bug is old to them, and unnamed,
     and that is exactly the leverage.

  3. "In the languages you already know, whose job was it to prevent this?"
     They land on:  mine.  The programmer's.  Whoever happened to remember.

Then read the last line of Scene 2 out loud and leave it up:

    "And Dice is a record.  Whatever a record protects, the die on the table
     just changed anyway."

Several of them will have heard that records are immutable.  Good.  That
belief is the thing this lesson is here to break.

Save the word aliasing for Direct Instruction, where it arrives attached to
something they have already watched fail.
-->

<!--
## Direct Instruction (~13 min)

### 1.  The shape they were taught, and the shape we're building (4 min)

Name it directly.  These students learned OO somewhere Java-shaped, and that
is the frame they will read every line of C# through unless you move it.

Two columns on the board:

    an object HAS state you change    |  a value IS its data, fixed once built
    setSides(4)                       |  build a different one and use it
    equal if it's the SAME object     |  equal if it SAYS the same thing
    behavior lives on the object      |  behavior can live beside it (L3)

Then, and this matters, hold them as equals:

    "Both of these are defensible.  They're different bets about what goes wrong
     in a big program.  The second one is where C# has been going for ten
     years, and it is the one you have yet to meet."

Scene 1 shows the right-hand column already paying out:  two separately built
2d6+3s are ==, and rolling dice leaves the dice alone.

### 2.  What a variable holds (4 min)

Scene 3, drawn on the board as boxes:

    int a = 5; int b = a;        [a|5]  [b|5]        two boxes, two fives
    var first  = new Dice(...);  [first |*]--> (dice)
    var second = first;          [second|*]--> (dice)  two boxes, one die

"Scene 2 is the second picture.  That's all it ever was."

Then string, immediately, because it is the bridge:  string is a reference
type that BEHAVES like a value, because it is fixed at creation.  ToUpper()
builds a second string.  Every one of them knows this, and today is the first time anyone has
been told why it matters.

That is the whole design move, previewed:  make Dice behave the way string
already does.

### 3.  Picking a primitive (3 min)

Still Scene 1.  Three lines, three surprises:

    7 / 2               -> 3       both operands int, so the result is int
    0.1 + 0.2 == 0.3    -> False   double is approximate on purpose
    0.1m + 0.2m == 0.3m -> True    decimal is exact and slow

Most have met the float thing.  Few have met integer division producing a
silently wrong number that sails through every test.  Flag it hard, because it
is the likeliest source of a wrong-but-running number in everything they write
this semester.

Close with:  "A die comes in whole numbers, so int is right here.  It is right
sometimes, and 'the default' is a poor reason to pick anything."

### 4.  The one keyword (2 min)

Only now, with the failure on the board behind you:

    public int Sides { get; set; }    any time      anybody, anywhere
    public int Sides { get; init; }   while built   sealed after

The sentence to say out loud:

    "Every option here is a choice. { get; set; } is the one you make by
     staying quiet."

Then send them to Step 3 and let the compiler do the teaching.

Misconception to preempt, and only this one:
  * "record means immutable." Correct them:  Scene 2 is a record misbehaving
    exactly like any mutable object.  A record gives you value equality, and
    immutability is a separate decision you make per member.
-->

## Guided Activity

Run the program after every change.  That is how you find out what you broke.

### Step 1 - Read Scene 2 and say it out loud

Find the line that caused the problem.  Look past the one that looks guilty.
Explain to the person next to you why `partyDie` changed when the only name
you touched was `yourDie`.

### Step 2 - Read Scene 3

Three pictures.  Say which one Scene 2 is.

Then say what would have to be true about `Dice` for it to behave like the third
picture -- the string one.

### Step 3 - Change one keyword, three times

In `Dice.cs`, change all three `{ get; set; }` to `{ get; init; }`.

Build.  **Read both errors before you touch anything.**  They are the two places
in this program that were quietly relying on being able to reach into an object
that somebody else was also holding.

### Step 4 - Fix Scene 2

Leave `set` where it lies.

The die is sealed now, so you will have to **build a different one** -- your own
`1d20+5`, leaving the table's `1d20` alone.

Re-run.  You should still get your +5, and the table's die should still be `1d20`.

> That is the move the whole style rests on:  **construction replaces
> modification.**  It looks like more typing right now.  Ask yourself what it
> would look like with nine properties on Dice -- and then hold that question,
> because C# has an answer and we will get to it.

### Step 5 - Fix Scene 3

Harder, and worth the time.  Scene 3 proved "one die, two names" *by mutating
it* -- and the compiler has closed that route.

`ReferenceEquals(first, second)` will tell you the truth instead.  Use it.

### Step 6 - Say what you actually bought

Re-run everything.  `ReferenceEquals` still prints `True`.

**The sharing is still there.**  `first` and `second` are still one object with
two names, exactly as they were this morning.  That part stayed the same.

What changed is that it stopped mattering.

> You made aliasing *safe to stop caring about* -- safe to stop asking whether
> you're holding the original.

That is the whole trade, and it's why the rest of this course leans the way it
does.  Write one sentence in your own words at the top of `Program.cs` saying it
back, then fix the now-outdated narration in Scene 2 (there's a TODO marking it
-- a comment that used to be true is worse than silence).

### Step 7 - The one that was settled all along

Scene 1's `dice == alsoTwoDSix` printed `True` before your change and `True`
after.  Say why.

*(If you can answer this cleanly, you've got the distinction the whole lesson
turns on:  how a type is **compared** and whether it can be **changed** are two
unrelated decisions, and `record` only makes the first one for you.)*

---

## Starter Code

[`starters/Lesson01_FirstObjects/`](../starters/Lesson01_FirstObjects/) -- runs
with zero modifications.

```
Dice.cs        a record whose three properties are all mutable
Program.cs     three scenes
README.md      the two dials, the primitives table, the steps
```

Two files.  That is the entire project, and it is deliberate -- see the note under
*The One New Idea*.

---

## Make It Yours - the Cards challenge

*This runs all semester.  Dice are the worked example; cards are yours.*

**Make your own file.**  `Card.cs`, in this folder, created by you and carried
forward to Lessons 2 and 3.

Build a `Card` -- the card alone, with the game left for later.  Then answer, in the design of
the type itself:

> What about a card is fixed for good, and what about it can change?

Have an opinion on:

- **Rank and suit.**  Does the Ace of Spades ever become something else?
- **Face up or face down.**  Is that a fact about the *card*, or about *where the
  card currently is*?  Those have different answers, and the second one may
  belong to the table.
- **How do you say "Queen"?** A string `"Q"`?  A number `12`?  C# has a type built
  for "one of a fixed set of named options." Go find out what it's called.
- **Should two separately-created Aces of Spades be equal?**  Try it before you
  decide.  C# has an opinion and it depends on what you declared.

The first pass can come out wrong.  What matters is that you can say why you
made each call.

---

## Homework (≤15 min)

Get `Card` compiling and printing.  Then three lines of comment at the top:

> One thing about my card that is fixed for good, one thing that can change, and what
> would break if I had that backwards.

It uses only what you already have.  If you're stuck longer than five minutes, stop and bring the
stuck part in -- that's worth more to the next meeting than a finished file.

---

<!--
## Wrap-Up (~5 min, end of Meeting A)

Aim to have the room through Step 4 by here; Steps 5-7 are Meeting B.

Exit ticket, one sentence, around the room:

  "Name one thing about a set of dice you made impossible to change, and one
   thing you left changeable, and why each is right."

Some will say "everything should be locked." Push once:  "Random stays mutable.
Should it?  What could a Random with fixed state ever hand back?" You want them
leaving with immutability as a tool they reach for, because the course needs
mutable state live as an option by Lesson 4.

## Meeting B (~50 min)

  5 min   Recap:  set vs init.  The two-column board from Direct Instruction.
 20 min   Steps 5-7.  Step 6 is the one to protect time for; do it as a whole
          class, out loud, before they read it.  Ask "did the aliasing go
          away?" and wait until somebody works out that it stayed.
 20 min   Cards.
  5 min   Wrap-up (below).

## Wrap-Up (~5 min, end of Meeting B)

Three or four students put their Card on the projector.  Same two questions
each, and stop there:

  "What did you make impossible?"
  "Who would disagree with you?"

The second is the assessment.  A student who can name a reasonable person who
would have built it differently has judgment.  A student who answers "this is
just correct" has a rule they are following.

Expect and welcome the face-up/face-down argument.  Two answers are defensible
and the disagreement IS the lesson.  Leave it open.  If you need a thumb on
the scale, ask "who is holding the card, and do they know what it is?" and
then get out of the way.
-->

<!--
## Teacher Notes

### Pacing

Meeting A: 7 warm-up / 13 instruction / 22 guided (Steps 1-4) / 5 wrap.
Meeting B: 5 recap / 20 guided (Steps 5-7) / 20 cards / 5 wrap.

If Meeting A runs long, cut Direct Instruction section 3 (primitives) and
fold it into the Meeting B recap.  Protect the warm-up, and protect
Step 6 -- those are the two things this lesson is actually for.

### Bugs you will actually see

  * Step 4 is DESIGNED to produce this, and it is the right answer:
        var yourDie = new Dice { Count = 1, Sides = 20, Modifier = 5 };
    Say so out loud.  Some students will feel they cheated, because it looks
    like more work than the line it replaced.  It is more work.  It is also
    the entire idea:  construction replaces modification.

    Somebody will ask whether a shortcut exists.  One does -- it is
    called `with` -- and it waits for a later lesson by design.  Write
    that student's name down and tell them they have found a later lesson.
    `with` sells itself in one sentence once a type has nine properties
    on it, and before that it teaches very little.

  * Step 5 attempted with == where ReferenceEquals belongs, which prints True
    and looks like it worked.  Excellent mistake, and worth catching publicly:
    == is True because the VALUES match, and it would be True for two
    separate dice as well.  It says only that the values match.  This is Step 7
    arriving early, from a student, which is the best way for it to arrive.

  * Somebody will change `set` to `init` and then also try to make Random
    immutable or wonder why it stays mutable.  Take it seriously:  a Random with
    change its internal state can only return one number forever.  State is
    earns its keep; unmanaged state is the enemy.  Say it in those words.

### If they finish early

Point them at GameData/GameMechanics/Dice.cs in the finished game -- the same
record, grown up, with positional parameters, a Parse, and a WeightedDice
that overrides Roll to cheat.  Leave `virtual` alone.  Let them ask, and
write down who asked; that is Lesson 5's warm-up.

Or:  "make Dice able to report its average roll." They'll write
Count * (Sides + 1) / 2 and get an integer, three minutes after being warned
about exactly that in Scene 1.  Losing to integer division twice in one class
teaches it better than any explanation.

### The thing to protect

Step 6.  Everything else in this lesson is syntax they'd have picked up from
a reference page.

The reframe -- the sharing stayed and you made it stop mattering -- is the one
idea that makes the functional-leaning style read as a trade with something
on both sides.  Students who miss it spend the semester thinking
immutability is a rule they're being asked to follow.  Students who get it
spend the semester noticing where it buys them something.

It is also the honest version of what this whole course is about:  what you
thought was neutral ground turns out to be positioned ground, chosen by
somebody, and now it's yours to choose.
-->
