# Building a Game in C# - Lesson Series Guiding Principles

The project-specific companion to [`meta/CS_GUIDING_PRINCIPLES.md`](../meta/CS_GUIDING_PRINCIPLES.md)
and [`meta/LESSON_DESIGN_FRAMEWORK.md`](../meta/LESSON_DESIGN_FRAMEWORK.md).  Those
documents ask, for each of ten commitments, what the equivalent move is for *this*
base project.  This is my answer for this one.

I wrote it before the lessons.  When a later decision is ambiguous, this is what
decides it.

---

## The Base Project

This series starts from an empty folder, and that is the first and largest
departure from the framework.

The Processing-Sprite series handed students a working library and taught them to
call it.  Here the students build the library.  Every tool the game eventually runs
on -- dice, cards, inventory, entities, a world, a command loop -- gets written by
the students, in order, each one arriving because the lesson before it made the
absence felt.

[`The World`](../README.md), the finished text adventure sitting in this
repository, serves a different purpose.  It is the **existence proof**: a complete
game built out of exactly these parts, available to me for worked examples and
available to students who want to see where a road goes.  It stays a reference.
The destination is whatever the students build.

## Audience

High school, moderately advanced.  Every student has taken computer science
before, in at least two languages, and C# is new to all of them.

That single fact reshapes almost everything.  They arrive with the concepts and
need the vocabulary:  they have written objects, loops, and functions for years,
while the words *value type*, *init-only*, and *extension method* will be new on
the first day.  So my job is renaming what they already hold, and I should teach it
that way.

They also arrive carrying assumptions they hold as facts.  A student who learned
objects in Python believes objects are mutable the way they believe water is wet.
That is a decision Python made, and making the decision visible is the
intellectual center of this course.

And they can be told things.  Framework Principles 5 and 6 -- hide the abstraction
layer, give advanced syntax a jargon-free cover story -- were written for sixth
graders, and they invert here.  I say more about that under *Adaptations*.

### The actual subject

This course teaches **object-oriented design re-learned with a functional bent**,
which is where C# itself has been going for a decade and which departs sharply
from what these students were taught.

| The shape they arrive with | The shape this course builds |
|---|---|
| an object **has** state you change | a value **is** its data, fixed once built |
| `setSides(4)` -- reach in and modify | build a different one and use that |
| equal if it is the **same object** | equal if it **says the same thing** |
| a type owns every behavior anyone wants from it | behavior lives beside the data, in modules |
| validate wherever you remember to | valid by construction, or refused |

The three opening lessons make one argument, and each step is safe only because
of the one before it.  Lesson 1: the data is fixed once it is built.  Lesson 2: therefore exactly
one moment remains when the data could be wrong, so close that moment.  Lesson 3:
therefore everything outside can be handed ordinary access, so behavior is free
to move out.

Say that to students outright, around Lesson 3.  They are clever enough to want
the reason the style coheres, and walking them into it instead is condescension.

The stance that makes this teachable is to hold both columns as equals.  They are
different bets about what goes wrong in a large program, and each one buys
something the other gives up.  The right-hand column is the one they have yet to
meet.  Both are defensible, and I should say so every time it comes up.

Course context:  four meetings per cycle, one semester, roughly 45 meetings, half
credit.  Homework is capped at fifteen minutes a night.  I treat that cap as a
design constraint and build around it -- see *Guardrails*.

## Entry Approach

**Objects First**, for an unusual reason.

The usual reason to open with objects is that students are meeting their first
one.  These students have made hundreds.  I start here because the object is where
C# diverges most sharply from what they already know, and where the divergence
pays the most.

Lesson 1 opens with a program that works, does exactly what its code says, and
produces a result every student recognizes as wrong.  The bug is aliasing -- two
names for one mutable object.  Most of them have shipped it.  C# is the first
language any of them have met that offers to make it impossible.

That is the entry.  Here is a thing you already know how to get wrong, and here is
a language with opinions about it.

## Narrative Thread

Two strands, running the whole semester.

### Strand A - The Toolkit (shared, teacher-led)

The class builds one library together, in the same order, lesson by lesson.
Everyone's is the same shape, and everyone typed their own.  This is the *imitate*
strand:  worked examples, live coding, a common vocabulary, and a guarantee that
every student walks out with the tool whatever else happened that day.

It starts with `Dice` because dice are the smallest object in games that is
genuinely interesting.  Dice are pure immutable data, and they sit right next to
`Random`, which is the most flagrantly *mutable* machine in the standard library.
That contrast carries the whole first lesson and it costs a single line to set up.

### Strand B - Cards (personal, student-led)

Every lesson, the same idea arrives a second time as a problem the student solves
alone.  Dice are the worked example.  Cards are theirs.

I chose cards over "make up your own object" for four reasons.  The domain comes
free, so a student starts building in the first minute with the premise
already supplied.  Cards carry the exact contrast the course opens on, because a `Card` is
immutable -- the Ace of Spades stays the Ace of Spades -- while a `Deck` is mutable
state through and through, and a student who builds both has discovered
facts-versus-state on their own.  Cards get harder in the right places:  by Lesson
3 `card.Value` turns out to be unanswerable, since an ace is 1 or 11 in
blackjack, high in war, and the queen of spades is worth thirteen points of
misery in hearts, and the student has to notice that putting `Value` on `Card`
quietly picks a side.  And cards end up load-bearing, because by Lesson 5 a deck
is a real, tested tool:  any student whose game wants cards has one, and every
other student has a shuffled, drawn-from, dealt-out collection they wrote
themselves.

Cards are also the assessment instrument.  Strand B is where I find out whether
the lesson landed, because the student built it alone.

### The fork

Around meeting 16 the two strands merge and students choose what they are
building.  Everything after that point is scaffolding for their choice.  I have
written the arc past the fork in pencil.

## Concept Progression

| # | Title | The one new idea | Toolkit gains | Cards track |
|---|---|---|---|---|
| 1 | First Objects | Immutability is declared member by member, and `record` governs something else entirely | `Dice` (immutable) | `Card` exists; rank/suit are facts |
| 2 | Validation & Factory Methods | A type should be impossible to construct in an invalid state | Private ctor, guards, factory methods, `TryParse` | `Card` refuses bad data; a real 52-card deck |
| 3 | Extensions & Separating Concerns | Behavior can live beside the data it describes | `Display` / `Rules` split | Card display; two games' rules, side by side |
| 4 | Collections | Many things, and what many costs | `Deck`, `Hand`, `Inventory`, shuffle | The deck becomes real |
| -- | **MILESTONE** | -- | *a playable card game, out of the whole unit* | -- |
| 5 | Composition | A class can be made of other classes | `Player`, `AbilityScores`, `Health` | `Creature`, from the same parts |
| 6 | Value Types | A small type can make an illegal value impossible to express | `AbilityScore` and `Health` with rules of their own | one `Creature` field given the same treatment |
| 7 | Shared Parts | Shared structure can come from shared parts | the assembled model, related by what it is made of | a third entity out of the existing pieces |
| 8 | Polymorphism | One call, more than one behavior | `WeightedDice` -- the die that cheats | creatures that act differently on their turn |
| 9 | Interfaces | A promise about behavior, held by unrelated types | `IRoller`, `IDescribable` | anything that can be drawn from |
| 10 | State & Events | Objects that announce what happened | damage, healing, `LeveledUp` | an encounter that reports itself |
| -- | **THE FORK** | -- | *students choose what they are building* | -- |
| 11+ | Systems & Build | commands, a loop, a world, saving -- and DTOs, where primary constructors and `with` finally arrive | *depends on the fork* | |

Lessons 1-4 are written, which is all of Unit 1.  Lessons 5 through 10 are
sketched in
[`lessons/NEXT_LESSONS.md`](lessons/NEXT_LESSONS.md).  Everything past the fork
stays deliberately unwritten, because writing it now would be guessing at choices
the students have yet to make.

## Guardrails

The commitments I hold to when a lesson starts arguing for more.

### On pacing

**One new idea per lesson.**  Techniques count separately.  Lesson 2 introduces
guard clauses, `TryParse`, and factory methods, and those are three techniques
answering one question:  where does "is this legal?" live?

**Count the files.  The file count is the scope.**  Lesson 1 is two files, and that
is the constraint doing its work.  When a starter needs a fifth file to make its
point, I am looking at two lessons.

<!--
This guardrail exists because my first draft of Lesson 1 broke it badly:  five
files and six scenes covering aliasing, value/reference semantics, value
equality, deep immutability through a List, AND encapsulated mutable state.
Every piece was good.  The lesson was unteachable.

What got cut, and where it went:
  { get; private set; } + mutation methods  -> Lesson 4 (a Deck's contents
      genuinely change, so the need is felt)
  deep immutability / record holding a List -> Lesson 4, same reason
  ReferenceEquals vs ==                     -> stayed in Lesson 1, but only
      as the TOOL for one step, and only there

The tell was file count.  Notice it early.
-->

**Every meeting ends with a running program.**  Running, today.

**Every starter runs before a student edits it.**  Open the folder, press Run, see
output.  Every time.

**Student work goes in files the student creates.**  The Cards track lives in a
`Card.cs` they make themselves in Lesson 1 and carry forward.  It is the one file
in this course that belongs entirely to them.

### On homework

**Homework consolidates syntax already taught.**  Fifteen minutes alone is barely
enough time to get unstuck, so I keep new syntax in the room where I can help.

**Every lesson opens fresh.**  A student who skips the homework entirely walks in
able to participate.  That is what makes the cap real; the moment homework becomes
load-bearing, the cap turns into a fiction and the students find out.

**Homework is consolidation or personalization.**  Finish the thing you started,
or make one decision about your own build and write down why.

### On content

**LINQ waits for Lesson 4 at the earliest**, until there is a collection worth
querying.  It is the most tempting shortcut in the language, and it hides exactly
the loop a student should write once by hand.

**Dice stay fully immutable.**  They are the course's reference example of a
value.  When something about dice seems to want to change, that is a signal the
thing belongs somewhere else -- which is precisely how Lesson 3's `DiceRules`
earns its existence.

**Primary constructors and `with` wait for DTOs**, post-fork, wherever save/load
lands.  I hold them as a single guardrail, because they are the same tool doing
the same job:  terse construction of a type whose data stands on its own.

| | primary constructor | `with` | factory gate |
|---|---|---|---|
| **has invariants** -- `Dice`, `Card` | no | no | yes |
| **is a shape** -- save records, config lines, messages | yes | yes | no |

Both forms bypass validation.  On a type with invariants that opens a hole, since
a primary constructor is a public front door with nowhere to put a rule and
`with` routes around the factory entirely.  A DTO carries data alone, so both
forms are correct there and the terseness comes free.

Introducing either one early teaches students to reach for the unguarded form by
default and bolt validation on afterward, which is the habit this course exists
to replace.  Introduced *with DTOs*, they arrive attached to the case where they
win, and the contrast teaches the criterion:  **the shape of the construction
should match whether the type has something to protect.**

<!--
This resolves a tension cleanly and it is worth knowing why.  Held on its own,
"when do we teach `with`?" is awkward:  it is a genuine convenience, and the
honest objection (it bypasses the factory) sounds like a language wart.
Attached to DTOs it stops being a wart and becomes the point -- `with` is safe
exactly where the data stands on its own.

In Lesson 1 the same guardrail comes free.  The fix in Scene 2 is
`new Dice { Count = 1, Sides = 20, Modifier = 5 }`, barely longer than `with`
and it says the important thing out loud:  construction replaces modification.
Students should be able to make that move the long way before a shortcut for
it means anything.

Mechanical footnote:  a primary constructor is always public in C#
(`record Dice private(...)` is CS1514), so the top-left cell of that table is
unavailable as well as discouraged.
-->

**Inheritance waits for Lesson 5.** Composition and extension methods come
first, so that `virtual` arrives as the answer to a question the students have
already asked out loud.

**This course leaves async, reflection, dependency injection, and ORMs alone,
permanently.**  All four are real, all four live in the finished game's
neighborhood, and any one of them would eat a semester.

**Every line in a starter uses syntax a student has met, or syntax the file
itself explains.**  Scaffolding helpers get a one-line comment.  Every line earns
its explanation.

### On stance

**Treat their prior language fairly.**  "Python allows this and C# refuses" is a
comparison.  "C# is safer than Python" is a ranking, and it is false often enough
to be worth avoiding entirely.  The move is always to ask what each language
decided and what the decision cost.

**Leave the arguable cases open.**  Several lessons carry a genuinely contested
design question where two answers are both defensible:  is `1d20-50` valid dice?
does `Value` belong on `Card`?  The temptation to settle it is enormous.  Leave it
open.  The student's job is to make a call and defend it, and a teacher who
answers has taken the assignment away.

**Name the scaffolding as temporary, out loud.**  Every starter is a thing being
handed over, and students should know the handover is coming.

## Adaptations from the Framework

The framework is written for grades 6-8.  Four of its ten principles change shape
for this audience, and I record the departures here so they stay deliberate.

| Principle | As written | Here |
|---|---|---|
| **5.** Treat abstraction layers as tools, not lessons | Hide what is under the library; students press the buttons | Inverted.  **The students build the library.**  They write every layer they use.  The only opaque tool is the .NET runtime itself -- `Random`, `Console`, `List<T>`. |
| **6.** Give advanced syntax a jargon-free cover story | Withhold the technical term | Inverted.  These students hold two languages' worth of concepts already, and withholding a name is condescension.  **Name it honestly, defer the depth.**  "This is an init-only property.  It means set-once.  What that implies is Lesson 2." |
| **9.** Vocabulary only after the experience | Introduce *variable*, *loop*, *object* once seen | Holds, with a twist.  They have the experiences and *some* of the words, and the words mean subtly different things in C#. The work is re-anchoring:  `record` and `class` diverge, and so do `==` and `is`. |
| **10.** End every lesson with personalization | A "Make It Yours" section | Becomes Strand B. Personalization runs the whole semester as a parallel track, and it carries the assessment. |

One structural departure.  The framework puts Direct Instruction and Guided
Activity in HTML comments so they stay teacher-only.  **Here the Guided Activity
is student-visible.**  These students work ahead, work at different speeds, and
work when absent, and with homework capped, class time is where the building
happens.  They need the steps in writing.  Direct Instruction and Teacher Notes
stay mine.

## On Assessment

Strand B is the evidence.  Students build Cards alone, so what a student's `Card`
looks like after Lesson 3 reads their grasp of separation of concerns far better
than a quiz would.

The defense is the assessment.  Every lesson closes with a question in the same
shape:  *name a call you made, and say who would disagree with you and what they
would want instead.*  A student who can answer that has the judgment this course
is actually about.  A student who wrote correct code and stalls on the question
has rules-based reasoning and a building yet to go up on top of it, and knowing
which one I am looking at is worth the two minutes.

Every closing question turns on the student's own build and their own reasoning.
The showcase is a showcase.

## Quick Reference - C# for people who already program

The things that most often surprise a student arriving from Python, JavaScript,
or Java.  Hand this out in Lesson 1.

| C# | What it means | The trap |
|---|---|---|
| `record` | A type compared by its values, with `with` for free | **It governs comparison.**  Mutability is a separate declaration, and a record with `{ get; set; }` misbehaves like anything else. |
| `class` | A type compared by identity | `a == b` comes out false even when every field matches. |
| `{ get; init; }` | Set once, while the object is built | It protects the slot.  Whatever the slot points at stays open. |
| `{ get; private set; }` | Only this type may change it | This is how "mutable but disciplined" is spelled. |
| `with` | A copy, changing named members | Runs the `init` accessors and skips the constructor. |
| `struct` | A value type -- copied on assignment | You will meet these later.  Assume `class` for now. |
| `var` | "Work out the type for me" | Resolved at compile time.  The type is then fixed forever. |
| `const` | A compile-time constant | **Already `static`** -- `static const` is CS0504.  Java's `static final` habit dies here. |
| `static int X => 100;` | A property, recomputed each read | It is a property, so it needs the `static` keyword that `const` carries on its own. |
| Either non-`const` form | -- | A pattern requires a `const`: `v is <= X` gives CS9135 otherwise. |
| `int` / `double` / `decimal` | Whole / fast-approximate / slow-exact | `7 / 2` gives `3`.  `0.1 + 0.2` lands just past `0.3`. |
| `string` | Text, fixed at creation | Every "modification" builds a second one. |
| `null` + `?` | An absent value, declared | The compiler warns you.  Listen to it. |

---

*When a lesson in this series pulls against something written here, the lesson is
what needs fixing -- or this document needs to change, on purpose.*
