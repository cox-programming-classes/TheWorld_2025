# Building a Game in C#

A one-semester, half-credit course for high school students who have programmed
before, in at least two languages, with C# new to all of them.

Students build a game.  More to the point, they build everything the game runs
on:  dice, cards, collections, entities, a world, a command loop.  Every piece of
it is theirs, down to the bottom, because the game is the occasion and the
building is the subject.

```
Course/
  README.md                 this file -- pacing, structure, how to run things
  GUIDING_PRINCIPLES.md     why every decision below was made that way
  lessons/                  the lesson plans
  starters/                 one complete, runnable C# project per lesson
```

## Running a starter

Each lesson's starter is a whole, self-contained project.  It stands alone, and
the .NET 10 SDK is the only thing to install.

```
cd Course/starters/Lesson01_FirstObjects
dotnet run
```

Every starter runs the moment it is opened, before a student changes a line.  I
hold that as a hard rule -- see the Guardrails.

Lessons 1–3 stand independent, so a student can start any of them cold.  From
Lesson 4 on, students carry their own files forward, and the starter's job
shifts from "here is the code" to "here is the room your code moves into."

## The two strands

| | Strand A -- **Dice** | Strand B -- **Cards** |
|---|---|---|
| Who drives | the teacher, live, together | the student, alone |
| What it is | the shared toolkit | the same idea, second time, unassisted |
| Where it lives | the starter's files | `Card.cs`, which the student creates |
| What it's for | vocabulary, worked examples, everyone stays afloat | this is where you find out if it landed |

Every lesson does the idea once with dice, then hands it back as cards.  The
brief lives in the lesson plan, and `Card.cs` is made by the student in Lesson 1
and carried forward.  It is the one file in this course that belongs entirely to
them.

## Pacing - 45 meetings

Four meetings per cycle, one semester.  Most lessons run two meetings:  one to
introduce and work through together, one to build, personalize, and get stuck
productively.

| Meetings | Phase | What happens |
|---|---|---|
| 1 | Setup | Toolchain, `dotnet run`, everybody sees output on their own machine |
| 2–7 | **Objects & Types** | Lessons 1–3 -- mutability, validity, where behavior lives |
| 8–15 | **Behavior** | Lessons 4–7 -- collections, polymorphism, interfaces, events |
| 16–17 | **The Fork** | Students choose what they are building.  Pitch, scope, commit. |
| 18–29 | **Systems** | Whatever the fork demands:  a loop, commands, a world, saving |
| 30–41 | **Build** | Student-directed, with short mini-lessons pulled in as needed |
| 42–45 | **Showcase** | Play each other's games; a retrospective on what they'd do differently |

The back half runs longer than the front half on purpose.  Homework is capped, so
class time is where the building actually happens.

Everything past meeting 17 is written in pencil.  The shape of the systems phase
follows from what students choose, and choosing is the point.

## Homework

**Fifteen minutes a night, or thirty every other meeting.  Treat that as a
ceiling.**

Three rules keep it honest.  Homework consolidates syntax already taught, because
fifteen minutes alone is barely time to get unstuck.  Every lesson opens fresh,
so a student who skips the homework entirely walks in able to participate.  And
homework means finishing something started in class, or making one decision
about your own build and writing down why.

The moment homework becomes load-bearing, the cap turns into a fiction and the
students find out.

## What's already in this repository

[`The World`](../README.md) -- a complete text adventure built from exactly these
parts.  It stays a reference, and students build toward their own game instead.
It is there so that when a student asks "where does this go," the answer is
something they can run, read, and steal from.

```
dotnet run                  # play it
dotnet test                 # 114 tests, if you want to show them what that looks like
```

Useful worked examples in it, by lesson:

| After lesson | Go look at |
|---|---|
| 1 | `GameData/GameMechanics/Dice.cs` -- the same record, grown up |
| 2 | `GameData/Items/Item.cs` -- validation that normalizes where ours refuses |
| 3 | `GameData/GameMechanics/` vs `Engine/States/` -- the same split, at scale |
| 5 | `WeightedDice` -- the loaded die that makes Finn win at cards |

## Before teaching this

Read [`GUIDING_PRINCIPLES.md`](GUIDING_PRINCIPLES.md).  It is short, I wrote it
before the lessons, and it settles the arguments the lessons deliberately start.
