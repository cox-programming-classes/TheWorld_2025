# What Comes Next

Sketches.  Lessons 4 and 5 are worked out far enough to confirm the arc works,
and they get written once Lessons 1-3 have actually been taught and I know what
the room is like.

Everything past the fork stays deliberately open, because designing it now
would mean guessing at choices the students have yet to make.

---

## Lesson 4 - Collections

*Many things, and what many costs.*

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

## Lesson 5 - Polymorphism

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

**Cards challenge:**  cards that do something when played.  A base `Card`, derived
kinds with different effects, one loop that plays a whole hand while staying ignorant of
what any of them are.

**Guardrail:**  inheritance is introduced *here*, deliberately late, and only one
level deep, and abstract base class hierarchies stay out.  If a student builds a five-level
tree, the question is "what would break if you used composition here?" The
answer to reach for is usually composition.

---

## Lessons 6 and 7 - in pencil

- **6, Interfaces.**  A promise about behavior.  `IRoller`, `IDescribable`,
  `IHasValue`.  The move that matters:  a `Deck`, a `Dice`, and a spinner share
  wildly different structures, and every one of them can
  satisfy "produces a random result."
  Probably also where `IGameIO` gets motivated, since it's what lets a game be
  tested with the human left out of the loop.
- **7, State and events.**  Objects that announce what happened and let something
  else decide what to do about it.  Health, damage, `LeveledUp`.  This is the last
  lesson before the fork, and it is the one where the pile of tools starts to
  feel like a *game*.

---

## The Fork - meetings 16-17

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

**After the fork, lessons stop being lockstep.**  Meetings 18-29 run as short
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
