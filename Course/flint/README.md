# Flint prompts - Sparky

One prompt per lesson for **Sparky**, the AI helper students reach from the
button on every Canvas Page and Assignment.

| Lesson | Prompt |
|---|---|
| 1, First Objects | [`lesson-01-first-objects.md`](lesson-01-first-objects.md) |
| 2, Data Validation and Factory Methods | [`lesson-02-validation-and-factories.md`](lesson-02-validation-and-factories.md) |
| 3, Extensions and Separating Concerns | [`lesson-03-extensions-and-concerns.md`](lesson-03-extensions-and-concerns.md) |

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

**Set so far:**  all three.

## Why the sidebar has no Sparky row

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

**Which questions have no answer.**  Several lessons carry a genuinely contested
design question:  whether face-up belongs to the card or the table, whether
`1d20-50` is valid dice, where `ChanceOfBeating` goes.  The teacher leaves those
open on purpose, and Sparky settling one takes the assignment away.

**Where the assessment lives.**  The Cards track is unassisted by design.  Every
prompt draws the same line:  help freely with syntax, hold back on design.

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

**Flint caps activity instructions at roughly 12,000 characters.**  Every prompt
fits under it.  Lesson 1 has room to spare.  Lessons 2 and 3 sit within a few
hundred characters of the ceiling, so anything added to those two wants
something else taken out.  Check before pasting:

```bash
for f in lesson-0*.md; do
  printf '%-46s %6d\n' "$f" "$(sed '1,/^---$/d' "$f" | wc -c)"
done
```

Only the text below the `---` goes into Flint, so the note at the top of each
file is free.  If the real cap turns out lower than 12,000, cut in this order:
the Rider inspection lists, then the stuck-point bullets, then the board summary
in "What happened in class".  The rubric table, the homework, the submission
flow, and the hold-back list earn their place -- they are what a general
assistant gets wrong, and they are why Sparky exists.

## Keeping these in step with the lessons

These prompts restate specifics from the lesson plans -- error codes, scene
numbers, the shape of the starter code.  When a lesson changes, the prompt needs
the same edit.

Source of truth is [`../lessons/`](../lessons/) for the teaching arc and
[`../starters/`](../starters/) for what the code actually does.
