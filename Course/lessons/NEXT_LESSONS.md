# What Comes Next

Sketches.  They get written once the lessons before them have actually been
taught and I know what the room is like.

The course runs in three units before the fork.  **Unit 1** is values:  one good
small type, built four times over.  **Unit 2** is the domain model:  what happens
when a program needs twenty of them and they have to fit together.  **Unit 3** is
behavior:  the tools for making one call do more than one thing.

Everything past the fork stays deliberately open, because designing it now
would mean guessing at choices the students have yet to make.

---

## Unit 1 - Values

*Lessons 1 through 4, plus a milestone.  Dice as the worked example, Cards as
the transfer.*

Lessons 1 to 4 are written, and Lesson 4 lives in
[`lesson_04_collections.md`](lesson_04_collections.md) rather than here.  The
milestone below is where the four of them turn into something a person can
play.

---

## Lesson 4 - Collections  (written)

*Many things, and what many costs.*

**Written up in full:**  [`lesson_04_collections.md`](lesson_04_collections.md),
with a starter project in `Course/starters/Lesson04_Collections`.  What follows
is the sketch it was built from, kept because the reasoning is still the
reasoning.

**The one new idea:**  a collection is itself an object, with its own state, its
own rules, and its own opinions about what may go into it.

**Why it lands here:**  Lesson 3 ended with a student saying some version of *"an
ace is 1 or 11 depending on the rest of the hand."* That sentence is a
collection asking to exist.  Wait for it -- if the room gets there on its own, the
lesson introduces itself.

**The felt problem:**  the starter has five loose `Card` variables and a `Deck`
that's a bare `List<Card>` with everything public.  Shuffle it.  Deal from it.
Deal from it twice and get the same card twice.  Deal from an empty one and
crash.  Every failure is a `List` doing exactly what a `List` does, in a place
that needed something with rules.

**What gets built:**

| | |
|---|---|
| `Deck` | a mutable collection of immutable cards -- the Lesson 1 contrast, finally load-bearing |
| `Hand` | a collection with a *limit*, which is where the rules start |
| `Inventory` | the same shape again, limited by weight -- proof the idea transfers |
| Fisher-Yates shuffle | ten lines, seeded, and their first real algorithm in C# |

**New syntax:**  `List<T>`, `Dictionary<K,V>`, `foreach`, indexers,
`IReadOnlyList<T>` as a return type.  Generics are consumed here:  they call
`List<Card>`, and defining `Deck<T>` waits for another day.

**Where the Lesson 1 material that got cut lands:**  this is the lesson that
earns `{ get; private set; }` and deep immutability, both of which were pulled
out of Lesson 1 for being a second idea.  A `Deck`'s contents genuinely change,
so mutation-through-methods is finally *needed*, with the need felt first.  And a
`Deck` that hands out its internal `List` has handed out the ability to modify
it -- `init` protected the slot, and whatever the slot points at stayed open.
`IReadOnlyList<T>` is the answer, and by here it will feel earned.

**LINQ arrives here, once the loops are written by hand** -- so `.Where()` and
`.OrderBy()` land as recognition.  One worked example, then it is available.

**Cards challenge:**  their `Deck` becomes real.  Fifty-two cards, shuffleable,
dealable, honest about running out.  Then:  deal a five-card hand and score it
with the two rule sets from Lesson 3 -- same cards, two different answers.

**Argument to leave open:**  should `Deck.Draw()` on an empty deck throw, return
null, or return false in the Try pattern?  All three are defensible; Lesson 2
gave them the framework to argue it.

---

## Milestone - a game made of Cards

*Two meetings.  The end of Unit 1, and the first thing they build that a person
can sit down and play.*

Four lessons of toolkit, and every piece of it is theirs.  This is where it
becomes a game.

**The deliverable:**  a complete, playable card game.  Console, one opponent,
win conditions, and an ending.  War is the floor.  Blackjack is the one most
students reach for, and it pays off the ace argument they have been having since
Lesson 3.  Go Fish needs matching and a little memory.  Anything they can scope
and defend is fair.

**Why it earns two meetings:**  every lesson so far ended with a type.  A game
is the first thing that ends with an *experience*, and the gap between those two
is where students find out what their toolkit actually does.  It is also the
first honest rehearsal for the fork, at a size they can finish.

**What it assesses, and it is the whole unit at once:**

| From | What has to show up |
|---|---|
| Lesson 1 | a `Card` that is fixed once built, and a `Deck` whose contents change |
| Lesson 2 | a deck that arrives complete and legal, every time |
| Lesson 3 | this game's rules living beside `Card` rather than inside it |
| Lesson 4 | a `Deck` and a `Hand` that are honest about running out |

**Scope is the conversation.**  Most first pitches are a whole casino.  Cutting
one down to a thing that finishes is the useful work, and doing it here means
the fork conversation in Unit 3 lands on students who have already had it once.

**Where Cards go after this:**  into a drawer, still in the project.  Unit 2
starts a new thing entirely, and the card game stays where it is, compiling.
Every game needs a mini-game, so whatever a student builds after the fork has a
tavern, a train car, or a waiting room with a deck in it already written.

---

## Unit 2 - The Domain Model

*Lessons 5 through 7.  `Player` as the worked example, `Creature` as the
transfer.*

Unit 1 taught how to build one good small type.  This unit is what happens when
a program needs twenty of them.

The unit runs on one long refactor.  Lesson 5 builds a `Player` that is honest
about being a pile of numbers.  Lessons 6 and 7 turn it into a domain model,
and the students do the turning.  **Each lesson changes the shape of code that
already runs**, which is the Lesson 3 move at a larger scale.

The word for what Lesson 5 builds is **primitive obsession**:  a design that
says everything in `int` and `string` because those were the types at hand.
Students meet the term in Lesson 5 and spend two more lessons working it off.

---

## Lesson 5 - Composition and the Shape of a Player

*A class can be made of other classes.*

**The one new idea:**  a class can be built out of other classes.  A `Player`
**has** a set of abilities, rather than having six loose numbers.

**What they build first, and it is deliberately flat:**  a `Player` with every
value a primitive.  Name, level, XP, current and maximum HP, six ability scores,
gold.  Twelve members, ten of them `int`.  They write it themselves, in class,
and it works.

**The felt problem, and it arrives in the first ten minutes.**  Build one
through a factory, the way Lesson 2 taught:

```csharp
Player.Create("Bree", 1, 0, 10, 10, 14, 12, 13, 8, 15, 11, 50);
```

Swap two of those numbers.  It compiles.  It runs.  The character is wrong for
the rest of the game, and every test passes, because 12 and 13 are both
perfectly legal ints.

**This is the bitter part, and it is the point.**  They know how to make a type
impossible to construct in an invalid state.  They wrote guards in Lesson 2.
And a guard checks a *range*, so every swapped value clears it.  Validation
cannot reach this bug.  Something else has to.

**The arc of the work:**

1. Build the flat `Player`.  Run it.
2. Swap two stats in the call.  Watch it compile and run wrong.
3. Reach for **named arguments**, `strength: 14, dexterity: 12`.  Better, and
   worth teaching -- and optional, so the next person can leave them off.
4. Extract `AbilityScores`, one type holding six named values.  `Player` drops
   from twelve members to seven, and the six-int constructor now lives in a type
   that is *about* being six ints.
5. Extract `Health`, current and maximum together, with the rule that current
   stays within maximum.

**The homework is where it pays off.**  Build the companion `Creature` class,
alone.  A creature needs abilities and health too -- and both types already
exist, so the second entity is nearly free.  Fifteen minutes, and the
extraction justifies itself.

**Hold back:**  inheritance.  Somebody will ask whether `Creature` should extend
`Player`, or whether both should extend an `Entity`.  That is Lesson 7's
question and Lesson 8's tool, and this unit is going to argue with it.  Tell
them they have found the next lesson, then leave it.

**Argument to leave open:**  is gold part of a `Player`, or part of an
inventory?  Both are defensible, and the answer differs by what game they are
building.

---

## Lesson 6 - Types That Carry Their Own Rules

*Making the wrong thing impossible to say.*

**The one new idea:**  a small type can make an illegal value impossible to
express, which is a stronger claim than refusing it at the door.

**Where Lesson 5 left it:**  the six ints are grouped, and they are still ints.
`abilities.Strength` hands back an `int`, and the moment it does, the number has
lost every bit of meaning the grouping gave it.  Level and XP are both `int`
too, so passing one where the other belongs still compiles.

**What gets built:**  `AbilityScore` as a type of its own, a record wrapping a
single number, legal between 3 and 18, with `Modifier => (Value - 10) / 2`
living on it.

**The move that makes it land:**  ask where the modifier formula currently is.
It is in three places, because three different callers needed it.  Now it has
one home, and the home is a type so small it feels silly.  That is Lesson 3
applied at a scale that looks absurd until the third caller shows up.

**The distinction worth naming, since it is the unit's spine:**

| Lesson 2 | Lesson 6 |
|---|---|
| the value is checked at the door | the value has a type that only holds legal values |
| a bad number is refused | a bad number has nowhere to live |

**`Health` gets the same treatment**, and it is the better demonstration:  a
`Health` where current exceeds maximum is impossible to build, so every method
downstream can stop asking.  `TakeDamage` hands back a new `Health`, clamped,
and Lesson 1's immutability is what makes that read cleanly.

**Homework:**  give one field on their `Creature` the same treatment, and be
ready to say what it now makes impossible.

---

## Lesson 7 - Shared Parts

*Two things built from the same pieces.*

**The one new idea:**  shared structure can come from shared parts.

**The felt problem, and every prior course set them up for it:**  `Player` and
`Creature` both have `AbilityScores` and `Health` now.  The instinct arriving
from Java is immediate -- extract a base class, call it `Entity`, put the common
things in it.  Ask the room.  Most of them will want it.

**The lesson takes that seriously and then takes it apart.**  Put the two types
side by side and list what each has that the other lacks.  A creature has a loot
table and an XP value for killing it.  A player has an inventory, gold, and a
level.  A base class forces a decision *now* about what is universal, and the
decision is made with the least information anyone will ever have about this
game.

**The demonstration:**  write a method that works on both, with the two types
related only by what they are made of.

```csharp
static int Check(AbilityScores scores, Ability which, Dice d) => ...
```

Called on a player, called on a creature, one implementation.  **The shared
thing is a part rather than a parent.**

**Why this ordering matters for the rest of the course:**  polymorphism arrives
next, and it arrives as a specific tool for a specific job -- one call, more than
one behavior.  Students who meet inheritance first reach for it to share
structure, which is the job it is worst at.  Meeting composition first means the
inheritance lesson can be about what inheritance is genuinely good for.

**Challenge:**  compose a third entity -- a shopkeeper, a summoned pet, a
training dummy -- out of the parts that already exist, leaving every one of
them exactly as it is.

---

## Unit 3 - Behavior

*Lessons 8 through 10.  The tools for making one call do more than one thing.*

---

## Lesson 8 - Polymorphism

*One call, more than one behavior.*

**The one new idea:**  the same line of code can do different things depending on
what the object actually is, decided while the program runs.

**Why it lands here:**  two hooks were planted on purpose.  Lesson 3 established
that extension methods bind at compile time and stay fixed there.
Somebody in Lesson 3 tried to make `Roll` an extension method and was asked what
happens when they want dice that roll differently.  This lesson is the answer to
a question they asked.

**The felt problem -- and it's a good one:**  the starter contains a dice game
against an NPC who wins too often.  Once in a while he loses, which is what
makes it hard to catch.
Students have a seeded `Random`, a working `RollTracker` from Lesson 1, and a
histogram from Lesson 3.  Catch the cheat with evidence.

The cheat is `WeightedDice : Dice`, overriding `Roll`.  It *is* a `Dice`.  It goes
everywhere `Dice` goes.  Every existing method works on it unchanged.  That is the
entire concept, discovered by being victimized by it.

**What gets built:**  `virtual` / `override`, `base`, derived records, and
pattern matching over a closed set of types (`item switch { Weapon w => ..., }`)
-- the shape the finished game uses everywhere.

**The sharp bit:**  put a `WeightedDice` in a `Dice`-typed variable and call both
an overridden method and an extension method.  One does the loaded thing.  One
rolls straight.  Lesson 3's gotcha, cashed in.

**Challenge:**  creatures that act differently on their turn.  A base `Creature`,
derived kinds with their own behavior, and one encounter loop that runs the
whole fight while staying ignorant of what any of them are.  Cards are in the
drawer by now;  this unit's thread is the one that carries forward.

**Guardrail:**  inheritance is introduced *here*, deliberately late, and only one
level deep, and abstract base class hierarchies stay out.  If a student builds a five-level
tree, the question is "what would break if you used composition here?" The
answer to reach for is usually composition.

---

## Lessons 9 and 10 - in pencil

- **9, Interfaces.**  A promise about behavior.  `IRoller`, `IDescribable`,
  `IHasValue`.  The move that matters:  a `Deck`, a `Dice`, and a spinner share
  wildly different structures, and every one of them can
  satisfy "produces a random result."
  Probably also where `IGameIO` gets motivated, since it's what lets a game be
  tested with the human left out of the loop.
- **10, State and events.**  Objects that announce what happened and let something
  else decide what to do about it.  Health, damage, `LeveledUp`.  This is the last
  lesson before the fork, and it is the one where the pile of tools starts to
  feel like a *game*.

---

## The Fork - meetings 24-25

Everything to here has been the same for everybody.  Here it stops.

Students commit to what they're building.  The toolkit they've written supports
several directions, and the honest thing is to say so and let them pick:

| Direction | What their toolkit already does | What they'd have to build |
|---|---|---|
| **Card game** | most of it -- deck, hand, rules, scoring | turn structure, an opponent, win conditions |
| **Dungeon crawler** | dice, entities, inventory | a world graph, movement, combat |
| **Text adventure** | items, entities, state | a parser, a command loop, dialogue |
| **Simulation / roguelike** | dice, collections, entities | a grid, a tick loop, generation |
| **Something else** | negotiable | negotiable, with a scope conversation |

**The format:**  a pitch, in class, with three required parts -- what it is, what
the smallest playable version is, and which lesson's tool they're leaning on
hardest.  Scope is the whole conversation.  Nearly every pitch is too big, and
cutting it down with them is the most useful thing that happens in those two
meetings.

Groups are allowed.  Solo is allowed.  Two people building the same game
separately is allowed and often the most interesting outcome, because they
diverge and can see it.

**After the fork, lessons stop being lockstep.**  Meetings 26-41 run as short
mini-lessons on whatever the majority need, plus workbench time.  The lessons
that get written are the ones the pitches demand, which is why they stay
sketches for now.

---

## Post-fork - the DTO lesson

Wherever save/load lands, there's a lesson waiting that pays off two things
deliberately withheld since Lesson 1.

A save file needs a flat, dumb representation of game state.  It carries data
alone, with everything about it open to inspection and a factory gate that would
guard an empty room.  It is a **shape**.  Which makes it the first place in the
course where the terse, unguarded construction tools are exactly correct:

| | primary constructor | `with` | factory gate |
|---|---|---|---|
| `Dice`, `Card` -- has invariants | no | no | yes |
| a save record -- is a shape | yes | yes | no |

**Primary constructors** arrive here because a primary constructor is a public
front door with nowhere to put a rule.  On `Dice` that's a hole; on a DTO it's
exactly right.  (It is also the only place it is *possible*, since a primary
constructor is always public in C# and `record Save private(...)` is CS1514.)

**`with`** arrives here for the same reason, and this is what makes it land
cleanly.  Taught in Lesson 1 it would have been a convenience with an ugly
footnote:  it routes around the factory, so on a guarded type it's the one door
validation leaves open.  Taught here, that stops being a wart and becomes the
point -- **`with` is safe precisely where the data stands on its own.**

The criterion is the lesson, and it's one sentence:  *the shape of the
construction should match whether the type has something to protect.*

Students will have spent the whole course building the guarded kind.  Handing
them the unguarded kind last, with the reason attached, is what stops it
becoming the default they reach for.

---

## Parked mini-lesson - the text you send is not the text they see

This belongs wherever a post-fork project first makes text cross a boundary:
`IGameIO`, a terminal renderer, a transcript or save file, a network message, or
a simple protocol.  It should stay a ten-minute piece of mischief, not become an
encoding survey.

Build a string containing explicit ASCII backspace characters (`\x08`).  Put an
extra character on the wire, back over it, and print the character the human is
supposed to see.  A terminal that honors non-destructive backspace displays one
message; a byte dump, redirected file, logger, or deliberately written decoder
can recover the other.  Then ask which one is the "real" message.

That silliness has honest history behind it.  ASCII `BS` meant move the printing
position left, not "delete a character from a string."  Teleprinters and early
document formats used it for overstriking; RFC 678 specified
`character + BS + overstrike character` in 1974, RFC 822 still explicitly
allowed backspaces for overstriking in Internet mail in 1982, and Unix `nroff`
output carried the same idea into the terminal era.  It is exactly the sort of
old control-character trick that was still lying around for BBS and terminal
kids to abuse in the 1990s.

Keep the nastier librarian story too.  It may be older than Windows 95: the
mechanism matches Apple II DOS 3.3 exactly.  Applesoft BASIC sent DOS commands by
printing `CHR$(4)` followed by the command text, and DOS 3.3 filenames could
contain control characters.  Put `CHR$(8)` in a name and `CATALOG` obeyed the
backspaces while drawing it; the characters stored on disk were not the name a
human saw, and the real name could not be typed back through the ordinary input
line.  The apparently undeletable files drove librarians bananas.  ProDOS later
closed the particular door by restricting pathname syntax.

There was a separate Windows 95 version of the prank.  From its MS-DOS prompt,
we could put a file or directory on the desktop whose name contained an OEM
Alt-code character that DOS accepted but Explorer could not reliably translate
back into a path.  `Alt+255`, which looked like a blank, was a common version;
period lists also identify characters such as `Alt+194` as troublesome to
Explorer.  The result again looked undeletable in the GUI but could be renamed
or removed through the DOS spelling that created it.  We may have used literal
backspace there too, or memory may have joined this stunt to the Apple one.  Do
not collapse the two into a certainty the evidence does not support.

Recreate that one with a fake in-memory catalog, never by manufacturing a
hostile filename on a student's actual machine.  It makes the same point at a
second boundary: the directory entry, the path string an API accepts, and the
label a catalog draws are not necessarily interchangeable.

The lesson hiding inside the prank: rendered text, transmitted bytes, and stored
text are three different representations.  Control characters are protocol,
not decoration.  Never put the trick in a real save format or message protocol
without framing and escaping it deliberately, and expect modern terminals,
logs, browsers, and editors to disagree about what `\x08` means.

---

## What the finished game is for

[`The World`](../../README.md) in this repository is a complete text adventure
built out of exactly these parts.  It stays a reference, and every student builds
toward their own game.

It's there so that "where does this go?" has an answer they can run, read, and
steal from -- a push-down state machine, a command pattern, data-driven NPCs,
`WeightedDice` making Finn win at dice, and 108 tests demonstrating why the
domain model routes every console call through IGameIO.

Show it at the fork.  Any earlier and it becomes the assignment.
