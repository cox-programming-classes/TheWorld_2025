# Flint prompts

Two prompts per lesson, for two helpers doing two different jobs.

**Sparky** works alongside a student who is mid-task, and students reach it from
the button on every Canvas Page and Assignment.

| Lesson | Sparky |
|---|---|
| 1, First Objects | [`lesson-01-first-objects.md`](lesson-01-first-objects.md) |
| 2, Data Validation and Factory Methods | [`lesson-02-validation-and-factories.md`](lesson-02-validation-and-factories.md) |
| 3, Extensions and Separating Concerns | [`lesson-03-extensions-and-concerns.md`](lesson-03-extensions-and-concerns.md) |
| 4, Collections | [`lesson-04-collections.md`](lesson-04-collections.md) |

**Anvil** is where a student goes afterward to review, and where a student who
missed the meeting goes first.

| Lesson | Anvil |
|---|---|
| 1, First Objects | [`lesson-01-first-objects-review.md`](lesson-01-first-objects-review.md) |
| 2, Data Validation and Factory Methods | [`lesson-02-validation-and-factories-review.md`](lesson-02-validation-and-factories-review.md) |
| 3, Extensions and Separating Concerns | [`lesson-03-extensions-and-concerns-review.md`](lesson-03-extensions-and-concerns-review.md) |
| 4, Collections | [`lesson-04-collections-review.md`](lesson-04-collections-review.md) |

## Setting one up

1. Create a Flint activity for the lesson.
2. Paste everything **below the `---`** in that lesson's file as the activity
   instructions.  The text above the rule is a note to you.
3. Copy the activity's chat URL.
4. Set `flintUrl` in that lesson's `../canvas/content/NN.json`, then re-run
   `build_canvas_html.ps1`.  The Page and the Assignment both pick it up.

```bash
grep -rn '"flintUrl"' ../canvas/content/              # what is set
grep -rln PASTE_FLINT ../canvas/html ../canvas/partials # what is still waiting
```

The URL lives in the JSON because the HTML is generated:  editing the HTML
directly works until the next build overwrites it.  A lesson whose `flintUrl`
is still empty emits `PASTE_FLINT_URL_HERE` on both files, flagged with a
`<!-- FLINT-LINK -->` comment.

**Set so far:**  Lessons 1 through 3.  Lesson 4 is waiting on its activity.

**The Anvil activities still want a wired-up home.**  `content/NN.json` carries one
`flintUrl`, and `build_canvas_html.ps1` emits one `FlintBox` per Page and per
Assignment, so a second URL needs a second JSON key and a second button before
the review helper appears on Canvas.  Until that lands, the four review prompts
paste into Flint and get shared by link.  That work touches generated HTML on
live pages, so it is deliberately a separate decision.

## Where the Sparky row went

The Course Links sidebar on the front page carried one for a while, and it came
out.  Flint hands out per-activity URLs, so a sidebar row would have to point at
one particular lesson and go stale the week after.  Every Page and every
Assignment already carries its own helper button, which is the route a stuck
student is on anyway.

The reasoning sits in a comment in
[`../canvas/partials/hero.html`](../canvas/partials/hero.html), along with last
year's MagicSchool join code, so this stays a decision rather than an
oversight.

## What each prompt carries

Sparky opens with the context a student would assume it already had:  what the
course is, where the lesson sits in the argument, what went on the board, what
the homework asks, and the four rubric stages that assignment is graded against.
A helper that knows the homework can point at it.  One that guesses invents a
different assignment.

Past the context, Sparky needs three things a general coding assistant lacks,
and each one takes explaining.

**What to hold back.**  Every lesson deliberately saves syntax for later.
Lesson 1 saves `with`, Lesson 2 saves primary constructors, Lesson 3 saves
`virtual`.  A helpful assistant hands those over in the first reply and skips
the idea the lesson was built around.  Each prompt names what is held, why, and
what to say when a student finds it anyway.

**Which questions stay open.**  Several lessons carry a genuinely contested
design question:  whether face-up belongs to the card or the table, whether
`1d20-50` is valid dice, where `ChanceOfBeating` goes.  The teacher leaves those
open on purpose, and Sparky settling one takes the assignment away.

**Where the assessment lives.**  The Cards track is unassisted by design.  Every
prompt draws the same line:  help freely with syntax, hold back on design.

## What Anvil carries, and the line it draws

Anvil does a different job from Sparky, because Sparky is written for a student
with the file open and a compiler error on screen.  Two students reach Anvil:
the one revising after the fact, and **the one who missed the meeting
entirely.**

That second student is why these exist, and the line runs through every one of
the four:

> **The instruction is theirs to have.  The decisions stay theirs to make.**

A student who missed Tuesday is owed the class.  So each prompt reconstructs the
meeting -- the warm-up, the questions asked in order, every item that went on the
board, and the code each item was attached to.  Sparky summarises the board in a
paragraph;  Anvil rebuilds it, because for an absent student that paragraph is
the whole lesson.

The guardrails stay exactly where Sparky has them.  The held-back syntax stays
held, the contested design questions stay open, and the Cards work stays
unassisted.  Being absent earns a student the instruction and leaves the
assessment exactly where it was, and each prompt says so in those terms.

Two more things distinguish these from Sparky:

**A menu, offered first.**  Anvil opens by listing the lesson in pieces and waits
for the student to pick, with "I missed class" as item 1.  It is told to prefer a
description over a number, since "I don't get why the table's die changed" is
better information than "2".

**A cumulative recap.**  Lesson N opens by rebuilding Lessons 1 through N-1 and
saying how each one made the next possible -- the data is fixed, then it arrives
valid, then it does very little, then the container owns its rules.  Lesson 1
opens the course, so that room goes to a **C# syntax from zero** table, for
students meeting the language cold.

## The Rider problem, in short

Students work in JetBrains Rider, and **Rider's inspections will offer them the
exact syntax each lesson is holding back.**  Alt+Enter on the right line
produces "Use 'with' expression" in Lesson 1 and "Convert to primary
constructor" in Lesson 2.  A student accepts the suggestion, the code improves,
and the lesson evaporates.

Each prompt lists the inspections that fire in that lesson and tells Sparky to
name what Rider spotted, confirm it is correct, and say which lesson it belongs
to.  Rider being right and the lesson being ordered are both true at once.

Lesson 3 carries the opposite problem:  `extension(Dice)` blocks are C# 14, and
a Rider older than 2025.2 shows valid code as broken.  That prompt tells Sparky
to check `dotnet build` before believing the editor.

## The size limit

**Flint caps activity instructions at roughly 12,000 characters.**  All eight
prompts sit against that ceiling.  The Sparky four measure 10,661 to 11,903;  the
Anvil four measure 11,997 to 12,000.  **Anything added to any of them wants
something else taken out.**  Re-measure before pasting, since these numbers go
stale on the next edit:

```bash
for f in lesson-0*.md; do
  printf '%-52s %6d\n' "$f" "$(sed '1,/^---$/d' "$f" | wc -c)"
done
```

Only the text below the `---` goes into Flint, so the note at the top of each
file is free.

**The cut order differs between the two, and it inverts.**  For Sparky, cut the
Rider inspection lists, then the stuck-point bullets, then the board summary;  the
rubric table, the homework, the submission flow, and the hold-back list earn
their place, because they are what a general assistant gets wrong.

For Anvil the board summary is the whole product, so it goes last.  The Anvil
order is:  the quiz items, then the recap of the guided steps, then the homework.  Each Anvil prompt already leaves out the submission flow, the rubric
stages, and the stuck-point catalogue by design, and says so in its own note --
Sparky sits one button away on the same Canvas page and carries all three.

## Keeping these in step with the lessons

These prompts restate specifics from the lesson plans -- error codes, scene
numbers, the shape of the starter code.  When a lesson changes, the prompt needs
the same edit.

Source of truth is [`../lessons/`](../lessons/) for the teaching arc and
[`../starters/`](../starters/) for what the code actually does.
