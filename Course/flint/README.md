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
4. Replace `PASTE_FLINT_URL_HERE` in **two** generated files -- the Page and the
   Assignment for that lesson.  Both live under `../canvas/html/`, and each is
   flagged with a `<!-- FLINT-LINK -->` comment.

```bash
grep -rn 'PASTE_FLINT' ../canvas/
```

Rebuilding the Canvas HTML overwrites those files, so paste the URLs into Canvas
once the build is done.

## The one on the front page

The Course Links sidebar carries a seventh placeholder,
`PASTE_FLINT_CLASS_URL_HERE`, and it wants a **class-wide** Flint URL rather
than a per-lesson one.  A student who is stuck between lessons lands there.

It lives in [`../canvas/partials/hero.html`](../canvas/partials/hero.html),
which is hand-authored and passed through the build untouched.  Edit it there
and the URL survives every rebuild.  That sidebar carried a MagicSchool join
code through last year, and the old URL sits just above the link in a comment
in case it is wanted back.

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
is written to fit under that with room to spare.  Check before pasting:

```bash
for f in lesson-0*.md; do
  printf '%-46s %6d\n' "$f" "$(sed '1,/^---$/d' "$f" | wc -c)"
done
```

Only the text below the `---` goes into Flint, so the note at the top of each
file is free.  If the real cap turns out lower than 12,000, cut in this
order:  the Rider inspection lists, then the stuck-point bullets, then the
board summary in "What happened in class".  The rubric table, the homework, and
the hold-back list earn their place -- they are what a general assistant gets
wrong.

## Keeping these in step with the lessons

These prompts restate specifics from the lesson plans -- error codes, scene
numbers, the shape of the starter code.  When a lesson changes, the prompt needs
the same edit.

Source of truth is [`../lessons/`](../lessons/) for the teaching arc and
[`../starters/`](../starters/) for what the code actually does.
