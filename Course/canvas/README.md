# Canvas Import - Building a Game in C#

Everything needed to put the lesson series into Canvas.  Same structure and same
look as the Processing Sprite import in [`../canvas_example/`](../canvas_example/).

| What | Where |
|---|---|
| **Front page, Lesson Pages, Assignments, and teacher copies** as Canvas-ready HTML | [`html/`](html/) |
| Lesson content, one JSON file per lesson (the source the HTML is built from) | [`content/`](content/) |
| Course-level content for the front page -- **the course title lives here** | [`course.json`](course.json) |
| The generator that turns the JSON into HTML | [`build_canvas_html.ps1`](build_canvas_html.ps1) |
| Descriptive rubrics as a CSV, for Canvas's rubric importer | [`canvas_rubrics.csv`](canvas_rubrics.csv) |

**Lessons 1–3 are built.** The rest arrive as they are written -- drop a new
`content/NN.json` in and re-run.

## Building the HTML

```powershell
powershell -ExecutionPolicy Bypass -File build_canvas_html.ps1
```

Reads every `content/*.json` and writes 3 files per lesson into `html/` -- a
student-facing Page, a student-facing Assignment, and a teacher copy -- plus
`front-page.html` from `course.json`. 3 lessons → 10 files.

Each output file is a **body fragment**: copy the whole file and paste it into
the Canvas HTML editor (the `</>` button in the Rich Content Editor).  Every
style is inline and every asset is embedded, so Canvas passes it through whole.

**Before publishing:** every Page and Assignment has an AI-helper button with a
placeholder `href="PASTE_FLINT_URL_HERE"`, flagged by a `<!-- FLINT-LINK -->`
comment.  Swap in the Flint chat URL for that lesson -- two per lesson, one on the
Page and one on the Assignment.

**The front page** (`html/front-page.html`) becomes the course home (Pages → ⋮ →
*Use as Front Page*, then Home → *Choose Home Page* → *Pages Front Page*).  Its
`LINK_*` placeholders need re-pointing at the real items once those exist -- the
token list is at the bottom of the generated [`html/README.md`](html/README.md).

**Teacher copies** open with a red "keep this page unpublished" banner.  They
carry the direct instruction notes, the guided-activity script, common mistakes,
and the wrap-up answers held in HTML comments in `../lessons/lesson_*.md`.  Put
them in an unpublished module.

To change wording, edit the lesson's JSON in `content/` and re-run the build.
**Leave the generated HTML alone** -- the next build overwrites it.

---

## Java asides

Any `howItWorks` section can carry an `aside` -- a float-right box, warm
parchment so it reads as marginalia beside the instruction (blue) and the
warnings (orange).  It does one job:  *here is what this looked like in the
language you already know.*

```json
{
  "h": "What `record` governs, and what it leaves to you",
  "aside": {
    "h": "If you're coming from Java",
    "icon": "&#9749;",
    "p": [ "…" ],
    "bullets": [ "…" ],
    "code": "Java:  a.equals(b)\nC#:    a == b"
  },
  "p": [ "…" ]
}
```

Only `h` is required; `icon` defaults to a coffee cup.  The aside is emitted
right after the section heading so the prose wraps around it, and the float is
closed before any code block so code always gets full width.

**Eight of them ship, across the three lessons.** They are titled *"If you're
coming from Java"* so a student arriving from Python or JS can skip them and
keep everything load-bearing.

Three rules for writing one, from the course guardrails:

- **Compare, and hold both as equals.** "Java does X, C# does Y, here is what
  each buys" -- leaving "C# is better" out of it.  The right-hand column is the
  one they have yet to meet, and both are defensible.
- **Say when the two languages agree.** The value/reference aside in Lesson 1
  exists to say *Java works exactly this way* -- which is the point, because it
  means Scene 2's bug was always available to them.
- **Give credit where the idea is older.** Lesson 2's factory aside opens with
  "you may already know this one" and points at *Effective Java*, because a
  student who recognizes it is ahead of the lesson.

### One thing to eyeball on first paste

The aside declares its width twice:

```css
width:38%; width:max(250px,38%); max-width:100%;
```

Canvas strips `<style>` blocks, so media queries are off the table.  The `max()`
version gives a narrow phone a readable full-width block in place of a 120px
sliver; a browser that stops at the first declaration keeps the `38%` on the
line before.  If Canvas's sanitizer ever rejects `max()` it would drop the whole
`style` attribute, so **check one aside on a phone after the first paste**.  If
it looks wrong, delete the `width:max(...)` declaration from `Aside()` in the
generator and rebuild -- you lose the mobile behavior and keep everything else.

---

## How this generator differs from the Processing one

The script is the same one, with six changes.  Four of them exist because the
original had course-specific text baked into the code where the content belongs.

| Change | Why |
|---|---|
| **`howItWorks[].aside`** -- a float-right cross-language sidebar | New component.  See above.  Purely additive:  content that omits an `aside` key renders exactly as before. |
| **Series name** comes from `course.json` (`seriesName`, falling back to `title`) | The original hardcoded `Processing Sprite` into every lesson banner. |
| **Section timings** come from the lesson JSON -- `teacher.warmupTime`, `directTime`, `guidedTime`, `wrapupTime` | The original hardcoded `~5/~10/~20 min`.  Our lessons run across two meetings with different pacing, and a teacher doc stating the wrong timing is worse than one that stays silent.  Defaults are the original values. |
| **The scoring note** at the top of every Assignment comes from `course.json` (`assignmentNote`) | The original asserted *"This assignment is **not scored**"* in the generator.  True in a course that sits outside the credit system; **false for this one**, and a decision for the teacher to make, well outside a build script's remit. |
| **The packaging hint** under "What to turn in" comes from `course.json` (`submitHint`) | The original told students to use **Sketch → Show Sketch Folder**, a Processing menu belonging to another toolchain entirely.  Set it to `""` to omit the line. |
| **`*emphasis*`** now renders as `<em>` | `Inline` handled `` `code` `` and `**bold**` while single asterisks fell through, so `*italics*` in the source JSON reached the page as literal asterisk characters.  A bug, and now fixed. |

The emphasis rule is deliberately conservative -- the run must start with a
non-space and stay inside one tag -- so `a * b` in prose and the `style=""`
attributes of already-generated `<code>` spans come through untouched.

### Backward compatibility, measured

Pointed at `../canvas_example/canvas_import/content/`, this script rebuilds the
Processing series and **32 of its 37 files differ from the committed HTML** --
the same 32 before and after the aside feature was added, with zero
`float:right` in the output, because that content leaves the aside key out.  Every
difference is one of the two intended fixes:

- **The banner.** Their `course.json` omits `seriesName`, so it falls back to
  `title` and the banner reads *Make Your Own Game* in place of *Processing
  Sprite*.  Adding `"seriesName": "Processing Sprite"` to that file restores it
  exactly.
- **20 new `<em>` tags**, in the places their content already wrote `*italics*`
  and the old script rendered as literal asterisks.

Timings hold steady, because the Processing lessons leave the timing fields out
and take the original defaults.

Verified by regenerating their series into a temp folder and diffing file by
file.  If you adopt this script for the Processing course, expect exactly those
two changes.

---

## The rubrics

These rubrics are **descriptive**.  They mirror the stage table at the end of
each lesson, and their job is to give a student (and you) a clear picture of
where they are and what to try next -- a *map*.

> **One thing to check before you publish.** The Processing course sits outside
> the credit system, so its assignments say *"This assignment is not scored"* outright.
> This course is half-credit and graded, so that sentence would have been a lie.
> It was hardcoded in the generator and now lives in `course.json` →
> `assignmentNote`.
>
> What it currently says takes the middle position:  the four stages are the
> standard, stated in advance and traceable to specific evidence, and the rubric
> instrument itself stays descriptive.  Whatever grade the course reports is
> determined separately.
>
> **That is my inference from your grading philosophy, and you have yet to tell
> me otherwise.** If the relationship between the stages and the grade should be
> stated outright to students, the sentence goes in `assignmentNote` (per
> assignment) or `stagesIntro` (front page).

### The four stages

Every lesson uses the same named ladder.  Each stage includes the one before it.

| Stage | Meaning |
|---|---|
| **Getting Started** | The build is red, or the change is still ahead of them -- paired with a specific next step to try. |
| **Got It Working** | The lesson's change is made and the program runs. |
| **Made It Mine** ⭐ | …and the same thing was done to the student's own code, unassisted, with the calls defended.  **The goal for everyone.** |
| **Went Beyond** | …and they took it somewhere the lesson left open. |

**"Made It Mine" is the Cards track**, and that carries real weight.  Dice is the
worked example we do together, and Cards is where you find out whether it
landed, because they built it alone.  A student who has done the Dice half and
stopped there has copied a refactor.

**To use the CSV in Canvas as a points-free rubric:**
1. **Course → Rubrics → + Rubric**.
2. Add one criterion for the lesson; paste the **Standard** as its description.
3. Add four ratings from the stage columns (Getting Started → Went Beyond).
4. Check **"Remove points from rubric"** and leave *"Use this rubric for
   assignment grading"* **off**.
5. Attach it to that lesson's assignment.

You can also hand the CSV to students directly as a self-assessment -- it is
written to them, in plain language.

---

## What the students actually submit

Worth knowing before you set up the assignments, because it differs from the
Processing course.  Submissions here are a project folder and a paragraph.

- A **zipped project folder** (`bin/` and `obj/` deleted -- they are build output
  and they are large).
- Their own **`Card.cs`**, which they create in Lesson 1 and carry forward.  This
  is the one file in the course that belongs entirely to them.
- A short **written defense** -- the "Show your thinking" box.  Every lesson asks
  the same shape of question:  *name a call you made, and say who would disagree
  with you and what they'd want instead.*

That last one is the real assessment.  A student who wrote correct code and
stalls on the question has a rule they are following, and knowing which one you
are looking at is worth the two minutes.

---

*Source of truth: [`../lessons/lesson_*.md`](../lessons/).  If you change a lesson,
update its JSON here and re-run the build so the two stay in step.*
