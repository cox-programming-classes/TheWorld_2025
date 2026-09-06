# Lesson 4 - Collections

*Many things, and what many costs.*

**Two meetings, ~50 min each.**  Closes Unit 1.  The milestone follows.

---

## The One New Idea

**A collection is itself an object**, with its own state, its own rules, and its
own opinions about what may go into it.

That is the whole lesson.  `List<T>` is a tool for holding things, and holding
things is the entire job it agreed to do.  Every rule about *which* things, *how
many*, and *who may reach in* belongs to a type the student writes.

This lesson also collects three debts from Lesson 1.  `{ get; private set; }`,
deep immutability, and `IReadOnlyList<T>` were all pulled out of Lesson 1 for
being a second idea.  A `DiceBag`'s contents genuinely change, so mutation
through methods is finally **needed**, and the need is felt before the syntax
arrives.

---

## Learning Goals

- Write a type that wraps a collection and owns the rules about it
- Say what `IReadOnlyList<T>` buys, and what it leaves open
- Use `{ get; private set; }` for state that changes, and defend it against
  both `init` and a public setter
- Write Fisher-Yates from the idea, in place, seeded
- Decide how a collection reports running out, and defend the choice
- Read a `foreach` loop and the LINQ that replaces it, and pick between them

---

## New Vocabulary

- **collection** -- an object whose job is holding other objects.
- **`List<T>`** -- the workhorse.  Ordered, resizable, and entirely without
  opinions.
- **`IReadOnlyList<T>`** -- a view of a list that offers reading and counting.
  A statement about what a caller may do.
- **encapsulation** -- keeping state private and letting methods be the way it
  changes.  Lesson 2 did this for construction;  today does it for change.
- **Fisher-Yates** -- the shuffle.  Walk from the end, swap each item with one
  chosen from the part you have yet to visit.
- **LINQ** -- `.Where()`, `.Count()`, `.OrderBy()`.  Loops with names.

---

## Warm-Up (~7 min)

Run it before saying anything.  Six scenes of output, and the room reads it
against `DiceBag.cs` on the projector.

Then one question, and take real answers:

> **`DiceBag.Add()` checks the capacity and refuses.  Scene 2 puts nine dice in
> a bag of six anyway.  Who is at fault?**

The answer worth landing:  `Add` did its job perfectly.  So did `List`.  The
fault is that a rule was written in a place anybody could walk around.

**A rule with a bypass is a suggestion.**  Put that on the board and leave it.

---

## Direct Instruction (~13 min)

### 1.  A List is indifferent (3 min)

Ask what `List<Dice>` knows about dice bags.  It knows how to hold things, and that is
the end of the list -- the same type serves shopping lists, pixel buffers, and enemies.

Generality is the feature.  It is also why the rules have to live somewhere
else.  Today's type is the somewhere else.

### 2.  Lesson 1, at a larger size (4 min)

**This is the beat that matters most.**  Put Scene 4 up.

```csharp
public List<Dice> Contents { get; init; } = [];   // sealed slot

var snapshot = mine.Contents;   // "just looking"
snapshot.Clear();               // the bag is empty
```

Draw it as boxes, the way Lesson 1 did.  `init` sealed the slot.  The `List`
the slot points at is still wide open, and one `var` hands a caller the bag's
insides.

Then the sentence to say out loud:  **making the reference fixed and making the
thing it points at fixed are two separate decisions.**  Lesson 1 made the first
one.  Today makes the second.

### 3.  Who may change what (3 min)

Three options for a member, side by side, and each one is a claim:

| Form | Says |
|---|---|
| `{ get; init; }` | set once, at construction, and settled |
| `{ get; set; }` | anybody, any time, for any reason |
| `{ get; private set; }` | this changes, and this type decides when |

`DrawnCount` is the example.  It is a fact about what happened.  A caller
writing to it is a caller telling a lie.

### 4.  Honest about empty (3 min)

Draw from an empty bag and read the crash aloud:  *Index was out of range.*
Lesson 2's move, arriving again -- those are `List`'s words, about an index,
and the word "index" belongs to `List` alone.

Then leave the question open, because it genuinely is:  **throw, return null,
or the Try pattern?**  Lesson 2 gave them the framework.  Let them argue and
let them pick.  Take a show of hands and write the count on the board;  it is
worth revisiting at the milestone when their deck runs out mid-game.

---

## Guided Activity

### Step 1 - Read the bag and list the bypasses

Read `DiceBag.cs` all the way through.  Two columns:  every rule the type is
trying to enforce, and how a caller gets around it.

Expect:  capacity (bypassed by `Contents.Add`), contents are dice (bypassed the
same way), draw removes (still to be written), `DrawnCount` is a record of fact
(bypassed by the public setter).

### Step 2 - Close the list

Make the list private, hand out a read-only view.

```csharp
private readonly List<Dice> _contents = [];
public IReadOnlyList<Dice> Contents => _contents;
```

Re-run.  **Scene 4 stops compiling, and that is the win** -- CS1061,
`IReadOnlyList<Dice>` does not contain a definition for `Clear`.

Scene 2 stops compiling too.  Delete those three lines and say what deleting
them proves.

### Step 3 - Make Draw draw

It should remove what it hands back, and it should be honest when the bag is
empty.  The three options from the board are all live.  Whichever they pick,
they defend it in the wrap-up.

### Step 4 - Re-run and read the difference

Scenes 2, 3, and 4 all read differently now.  Five draws, five different dice,
and a bag that shrinks.

### Step 5 - Fix the shuffle

The algorithm in `Shuffle` is already correct.  It builds a copy, shuffles the
copy properly, and hands the copy back to a caller who drops it.

Ask them to find the bug in the *signature* before touching the body.  Then make
it work on `_contents` and return `void`.

Worth saying while it is on screen:  Fisher-Yates walks from the end and picks
each partner from the part it has yet to visit.  Picking from the whole list
every time is the common mistake, it looks fine, and it is measurably biased.
A student who wants to prove that has a histogram from Lesson 3 and a good
afternoon ahead of them.

### Step 6 - DrawnCount tells the truth

`{ get; private set; }`.  One word, and the type is now the only thing that can
write down what happened.

### Step 7 - The initializer stops working

`Standard()` built its bag with a collection initializer.  That reached the list
directly, so it is gone.  Rebuild it with `Add()`.

This step looks like tidying and it is the actual lesson:  **the factory now
goes through the same door as everybody else.**  If `Add` refuses a seventh die,
`Standard` finds out the same way a caller would.

### Step 8 - A loop, and then a name for it

Count the d20s in the bag with a `foreach`.  Everyone writes it;  it is four
lines and they have written it a hundred times in other languages.

Then:

```csharp
public int CountOf(int sides) => _contents.Count(d => d.Sides == sides);
```

**This is the first LINQ in the course, and it arrives as recognition.**  They
know what the loop does, so the method reads as the loop with a name on it.
One worked example is enough.  It is now available and it stays optional.

---

## Make It Yours - the Cards challenge

Their `Card` has been waiting for this since Lesson 1.

- **`Deck`.**  Fifty-two cards, shuffled, dealt one at a time, honest about
  running out.  The same three problems, on their own type.
- **`Hand`.**  A collection with a limit.  Five cards in, and it refuses the
  sixth.
- **Then the payoff:**  deal a five-card hand and score it with both rule sets
  from Lesson 3.  Same cards, two games, two answers, and each rules file
  ignorant of the other.

That last one is the moment Unit 1 closes.  Four lessons of separate ideas, and
they run together in one line of output.

---

## Homework (<=15 min)

Get `Deck` shuffling and dealing.  That is the whole ask.

`Hand` and the two-game scoring are Meeting B work, in class, with help
available.  A student who arrives with a deck that deals is ready.

If they finish early:  deal until it runs out, and make it end the way they
argued for in class.

---

<!--
## Wrap-Up (~5 min, end of Meeting A)

**Ask:**  You picked a way for `Draw` to report an empty bag.  Which one, and
what would change if your game had fifty decks running at once?

**Looking for:**  a defence tied to *who is calling*.  A programmer's bug wants
a throw.  A game loop that expects to run out wants the Try pattern.  The
student who says "it depends on whether running out is normal" has the whole
of Lesson 2 in one sentence.

## Meeting B (~50 min)

5 recap on the bypass list from Step 1 / 15 finish the guided steps / 25 Cards
/ 5 wrap.

Meeting B is mostly Cards, because `Deck` and `Hand` are where the transfer
happens and it is worth the room's time to have it happen with help nearby.

## Wrap-Up (~5 min, end of Meeting B)

**Ask:**  Your `Deck` and your `Hand` are both collections with rules.  Name one
rule each has that the other would find absurd.

**Looking for:**  a deck refuses duplicates and runs out;  a hand has a size
limit and does not care what is in it.  Same shape, different opinions, which is
the sentence the next unit is built on.
-->

---

## Teacher Notes

### Pacing

**Pacing.**  Meeting A:  7 warm-up / 13 instruction / 22 guided (steps 1-5) /
5 wrap.  Meeting B:  5 recap / 15 guided (steps 6-8) / 25 Cards / 5 wrap.

### The one to protect

Step 2 and the compile error.  Everything else in the lesson is mechanical;
that error is where "a collection is an object with rules" becomes something
the compiler will enforce for them.  If time runs short, cut Step 8.

### The escape hatch, and whether to mention it

`IReadOnlyList<Dice>` hands back the same object, so this works:

```csharp
((List<Dice>)bag.Contents).Clear();      // and the bag is empty
```

It is a statement of intent rather than a lock.  **Somebody in the room will
find this**, and the honest answer is that it is a real hole with two real
fixes:  `_contents.AsReadOnly()` returns a genuine wrapper that refuses the
cast, and returning a copy costs an allocation and settles it completely.

Take it seriously when it comes up.  The interesting question is what a type is
protecting against -- an accident, or an attacker.  Most of the time it is an
accident, and the intent is enough.

### Bugs you will actually see

- **CS1061 on `Contents.Add`** after Step 2, inside `Standard()`.  That is Step
  7 arriving early.  Let them find it themselves;  it is the best moment in the
  lesson.
- **A shuffle that picks from the whole list** every iteration.  It runs, it
  looks shuffled, it is biased.  Worth a mention and worth leaving alone unless
  somebody wants the histogram.
- **`RemoveAt(0)` instead of the end.**  Correct, and O(n) per draw.  Fine at
  52 cards.  Say so, and say why you are saying so.
- **A `Deck` that shuffles by sorting on a random key.**  It works.  Ask what
  happens when two keys collide, then let it stand.
- **Returning `_contents` from a method** after carefully hiding the field.
  The hole reopens quietly.  Ask what the return type is.

### What Unit 2 needs from this

`Deck` and `Hand` demonstrate the same shape carrying different rules.  That
observation is what Lesson 7 cashes in when `Player` and `Creature` want a base
class and get parts instead.  Plant it in the Meeting B wrap-up and leave it.
