# Writing Common Errors - prose and style

A running list of the mistakes that actually happen when an assistant drafts or
edits prose for this repository.  Every entry here was caught in real work.

Read this alongside [`APPALACHIAN_ACADEMIC_STYLE.md`](APPALACHIAN_ACADEMIC_STYLE.md).
That document says what the voice is.  This one says where it goes wrong.

**Scope is prose.**  The errors students make in C# -- "record means immutable",
picking a shuffle partner from the whole list every time -- live in the teacher
notes of the lesson that addresses each one.  When enough of those accumulate to
be worth collecting, they belong in a `CS_COMMON_ERRORS.md` beside
[`CS_GUIDING_PRINCIPLES.md`](CS_GUIDING_PRINCIPLES.md), so that the two lists
stay separately searchable.

**Adding to the list:**  when a correction happens, add the pattern, the fix, and
a command that finds it again.  An entry earns its place by being catchable.

**The corpus, and how far it stretches.**  Several entries below cite counts
measured against `teaching_philosophy.md`.  Two cautions come with those
numbers.

The first is mechanical.  That file has since been revised to meet the
rendering rules, so the counts describe it as it stood at commit `6d5ebe4`,
before the pass.  Read them there:

```bash
git show 6d5ebe4:meta/teaching_philosophy.md
```

The second matters more.  That document was built over a long back-and-forth
between Jason and an assistant, and much of that conversation lives outside this
repository's history.  So it is strong evidence for what he *endorses*, having
read every line and argued about many of them, and weaker evidence for what he
would write with no assistant in the room.  Where an entry below says "his own
writing", read "a document he shaped closely."  The distinction matters when a
count is doing the arguing.

---

## Rendering rules

These three are mechanical, and they are the ones most likely to survive a
careless pass because they look like noise.

### 1.  The typographic em-dash

Em-dashes belong in this prose and they are used heavily.  Write them as `--`
with a space on each side.

| | |
|---|---|
| Looks like | `a trade — and here is what you pay` |
| Write | `a trade -- and here is what you pay` |

The HTML entity `&mdash;` is the same error wearing a costume.  So is `&ndash;`.

**Why it matters:**  the typographic character has become a marker that gets
written work dismissed as machine output before anyone reads the argument.  The
goal is prose that reads as thoughtfully crafted and AI-assisted, so the tell
comes out.

**Headings take a single hyphen.**  `# Lesson 1 - First Objects`, with prose
keeping `--`.

```bash
grep -rn '—\|&mdash;\|&ndash;' --include='*.md' --include='*.json' .
grep -rnE '^#{1,6} .* -- ' --include='*.md' .
```

### 2.  One space after a sentence

Every sentence-terminal mark takes two spaces:  periods, question marks,
exclamation points, and colons.

**Why it matters, and this one is load-bearing:**  Jason is dyslexic, and the
double space is what lets him see where one sentence ends and the next begins.
Treat it as an accessibility requirement.  A formatter or a linter that
normalizes it away has broken something real.

```bash
grep -rnE '[a-z)][.?!:] ["A-Z]' --include='*.md' .
```

**In HTML output, emit `&nbsp;` for the second space.**  HTML collapses runs of
whitespace, so a double space typed into the source renders as one.  Canvas's
rich text editor and macOS text substitution both fight a hand-typed double
space as well.  Any generator producing HTML should convert `([.?!:])  ` into
`$1&nbsp; ` on the way out, which keeps the source as plain prose and the page
readable.

I got this wrong once by advising against it as over-engineering.  For an
accessibility habit the entity is the correct tool, and the live Canvas page was
already using it by hand.

### 3.  Definition by negation

The single most persistent tic, and the one that reads most obviously as
borrowed cleverness.  It appears roughly four times in three thousand words of
`teaching_philosophy.md`, against sixty-one in one assistant draft of comparable
length.

That word count was wrong here for a while, given as eighteen thousand, which is
the file's size in **bytes** read off `ls -l`.  The ratio is what carries the
argument and the ratio survives the correction, so the rule stands.  Check a
count before it goes into an argument;  `wc -w` and `wc -c` disagree by a factor
of six on ordinary prose.

| Looks like | Write |
|---|---|
| `a map, not a grade` | `a map:  where you are and what to try next` |
| `It is a trade, not a free win.` | `It is a trade.  Here is what you pay.` |
| `nothing outside can call it` | `only this file can call it` |
| `does not compile` | `the compiler rejects it` |
| `no fixed list` | `the list stays open` |
| `Do not settle it.` | `Leave it open.` |

The ban covers the whole family:  `never`, `nothing`, `nobody`, `neither`,
`without`, `none`, and `rather than` or `instead of` when they are doing
setup-and-knockdown work.  Imperatives count too, so `Do not cut the warm-up`
becomes `Protect the warm-up`.

**The method:**  rewrite the negative as a positive assertion.  A positive form
almost always exists, and reaching for it forces more precision than the
negative had.

**The one exception is citation.**  Keep a negation when it is somebody else's
words:  a compiler error (`'minValue' cannot be greater than maxValue`), a book
title (*Consider static factory methods instead of constructors*), or a
quotation of the text being replaced.

---

## Voice

### 4.  Inferring the voice from samples

An assistant measured contraction counts in `teaching_philosophy.md`, found two
against twenty-three uncontracted forms, and turned that into a rule banning
contractions.  That document is a formal essay written to administrators.  The
style guide asks for prose that stays *speakable*, which points the other way
entirely.

**The fix:**  read [`APPALACHIAN_ACADEMIC_STYLE.md`](APPALACHIAN_ACADEMIC_STYLE.md)
first.  Ask for a guide when one seems likely to exist.  Statistics over a
sample tell you what one document did, and the guide tells you what the voice is.

### 5.  Manufacturing the voice

The guide is explicit about this and it deserves repeating:  work from Jason's
actual language.  Adding dialect, phonetic spellings, rustic imagery, or folksy
sayings produces a costume.  "Appalachian Academic" describes plain speech
carrying scholarly substance, and the plainness lives in word choice and
cadence.

The harder line:  writing in his voice stops short of inventing personal
relationships, experiences, regional memories, or claims of shared identity.

**A subtler form:  borrowing a phrase and softening what it weighs.**  I wrote
"Print them.  Sit with it." about five deliberately broken `Card` objects, and
"the consequence worth sitting with" about a property that reads the same as a
field.  *Sitting with* something carries moral weight.  It is what a person does
with grief, or guilt, or a decision that cost somebody else.  Ordinary academic
confusion asks far less than that, and spending the phrase there wears the
register down for everybody who needs it later.

Jason uses it once, in `teaching_philosophy.md`:  "tolerance for sitting with
unresolved problems", meaning genuine ambiguity a person has to endure.  That is
the real sense, and it is the only one in his writing.

**The test:**  ask what the phrase costs the person doing it.  Where the honest
answer is "some attention", write `look at`, `read it back`, or `notice`.  This
is the same failure as entry 8 seen from the other side:  there a word was
stretched to cover four jobs, here a phrase was borrowed at full weight and
spent on a small one.

```bash
grep -rniE '\b(sit|sitting|sat) with\b' .
```

### 6.  The writer goes missing

Drafts drift into an impersonal register, especially in documents that feel
institutional.  His voice uses first person, and a guiding-principles document
for a course is his argument.  It should sound like one.

Watch for the hedge, too.  `This document explores the relationship between...`
hedges the claim away.  When the intent is to persuade, say so.

### 7.  Everything becomes bullets

From the style guide:  use headings and lists when they help a reader navigate,
and keep an essay as prose by default.

Assistant drafts run bullet-dense, because bullets feel organized and cost less
to write than a paragraph that has to hold an argument together.  A guiding
principles document, a rationale, or anything making a case wants connected
prose.  A step list, a reference table, or a lookup wants a list.

### 8.  "Holds" doing four jobs at once

Jason uses `hold` in one sense, the ordinary one:  to hold a position.  "I hold
authority in my classroom."  "I hold my own view firmly."  "Constructionism
holds that people learn by making."  Thirteen uses in `teaching_philosophy.md`,
and every one of them is that.  (This entry said nine until a recount;  the
claim that matters is the unanimity, and that held.)

An assistant draft grew a second sense, in which a value is personified as
keeping itself motionless:  the data *holds still*, the type *holds it that
way*, dice that *hold still*, behavior that *held*.  Twenty-eight of those
across the course, including the sentence opening the whole three-lesson
argument.  It reads as invented vocabulary, and to a fourteen-year-old it is a
riddle.

**The fix:**  say `fixed`.  "The data is fixed once it is built."  "Because the
type guarantees it."  The rubric already carried the plain phrasing -- *what is
fixed for good and what can change* -- so the words were sitting right there.

**A second sense went the same way:  possession.**  Where `hold` would mean
*has*, he writes `has`.  A record that **has** a `List`, the values it **has**,
you **have** a key from a save file.

**The exception is the bucket.**  `holds` earns its place while the container
analogy is switched on and doing the teaching -- Lesson 1 draws variables as
boxes on the board, and "what a variable holds" is the whole point of that
picture.  "Who is holding the card" is the same thing:  the Cards argument turns
on whether the person physically holding it knows what it is.

The test is the analogy rather than the word.  **The same variable takes either
verb depending on whether the bucket is in play**, so later lessons say a
variable *has* a particular value once the container picture has done its job
and been put away.  Ask which one the sentence is thinking with.

So the word splits three ways:  staying constant becomes `fixed`, possession
becomes `has` unless the bucket is on the board, and holding a position is his
and stays.

These senses stay, and they are why a bare grep for the word is useless:

| Keep | Because |
|---|---|
| `what a variable holds` | the bucket is switched on;  Lesson 1 draws it as a box |
| `who is holding the card` | the physical act is the argument |
| `hold back`, `hold the line` | ordinary idiom |
| `hold both columns as equals` | his own sense |

```bash
grep -rniE '\b(holds?|holding|held) +(still|steady|together|for good)' .
```

**The general lesson.**  A word appearing ninety times across a repository is
doing several different jobs.  Sort the senses before touching any of them, and
read the author's own writing to find out which sense is theirs.

### 9.  The softened directive

An instruction gets dressed as a gentle observation so it will land easier.  The
command survives the costume, and what the reader hears is a writer working
around them.

| Looks like | Write |
|---|---|
| `You might want to run it first` | `Run it first` |
| `It may be worth checking the generator` | `Check the generator` |
| `Consider using a record here` | `Use a record here` |
| `One option would be to split the lesson` | `Split the lesson` |

This is the imperative twin of the hedge in entry 6.  There a claim goes soft;
here an instruction does.  Both come from the same reflex, a wish to sound less
bossy than the sentence actually is.  The softening leaves the command exactly
where it was and lays a coat of apology over it, so the reader ends up knowing
two things:  what they are being told, and that the writer was uneasy about
telling them.

**The boundary is whether the choice is real.**  Where the reader genuinely
picks, `you can` and `one option is` are accurate and they stay.  Where the
writer has already decided, the imperative is the honest form.  Ask what happens
if the reader declines.

```bash
grep -rniE '\b(you (might|may) (want|wish) to|it (might|may) be (worth|helpful|useful)|consider (adding|using|trying|making)|one option (is|would be)|feel free to|you could (try|also)|perhaps (try|add|use))' --include='*.md' --include='*.json' .
```

### 10.  The concession arrives last

Jason's arguments open with the unflattering part.  *"I should be plain about my
own temperament, since it bears on how I do this work.  I run hot, and I do not
apologize for it."*  *"I will also say plainly that I do not hold this position
as gospel."*  The concession comes first, and the case gets made from there.

A draft that leads with the pitch and files the weakness at the end has the same
two pieces in the opposite order, and it reads as sales.  Conceding first reads
as trust, and it earns the turn.

**This is the first thing a tidying pass throws away**, because a concession at
the top looks like a soft opening, and an editor's hand moves it down.  Check
the order after any revision that tightened a passage.

The grep finds the markers, and reading tells you where they sit.  A marker in
the first third of a section is carrying the argument.  One in the closing
paragraph has become a disclaimer.

```bash
grep -rniE '\b(that said|to be fair|of course|admittedly|granted|it is true that|i am (fully )?aware|i will also say|to their credit)\b' --include='*.md' .
```

---

## Where the ornament lands

Two entries about *where* to look.  Both say the same thing from opposite ends:
a limited editing pass finds more by aiming than by sweeping.

### 11.  Ornament on the load-bearing sentence

Fancy words gather where the stakes are highest.  The sentence carrying a
paragraph is the one a writer reaches on, and the reach shows up as abstraction,
stacked modifiers, and a word that would sound strange spoken aloud.

**The rule runs backwards from intuition:**  the more a sentence has riding on
it, the plainer it should be.  A line that is both the most important in its
paragraph and the most decorated in its paragraph is showing nerves.

**The test:**  find the sentence you would keep if you could keep one.  Then
weigh it against its neighbors.  Where it is the longest and the most Latinate
of the group, rewrite it flat and read the paragraph again.  Usually the
paragraph gets faster and the claim gets louder, which is the tell that the
decoration was muffling it.

**Where this entry came from, since it differs from the rest.**  Every other
entry here was earned by a correction Jason made to a draft.  This one arrived
from an assistant reading its own drafting habits, and an audit of
`teaching_philosophy.md` turned up no instance of the pattern in his prose.  So
it describes what assistants do to his writing, and it rests on introspection
until a real correction confirms it.  That is a weaker footing than entry 3 or
entry 8, and worth knowing when the rule and a sentence disagree.

The word list below is a seed, and a seed planted by the same introspection.
Grow it from real corrections, the way every other entry here was grown, and
prune the words that keep turning up honest.  `fundamental` is already one of
those:  in `CS_GUIDING_PRINCIPLES.md` it names the paradigm's basic unit, which
is the word doing its literal job.  The ornament is `fundamentally` sitting in
front of a claim to lend it weight.

```bash
grep -rniE '\b(crucible|tapestry|testament to|profound(ly)?|fundamental(ly)?|at its core|the very (fabric|heart)|underscore[sd]?|illuminate[sd]?|resonate[sd]?|nuanced|intricate|delve|myriad|realm of|serves? to|speaks to)\b' --include='*.md' .
```

### 12.  The closing sentence

Stock phrasing clusters at the end of a section and at a transition.  A middle
paragraph has content to carry it, so it stays honest.  A closing has to land,
and landing is where a writer reaches for a shape borrowed from somewhere else.

The style guide already says to end a passage when its thought has landed.  This
entry says where to find the passages that failed to.

**Spend a limited pass here.**  Reading every last sentence in a document takes a
minute and turns up more than an even sweep of the whole thing.

This command prints the last prose line of every section, which is the
shortlist.  It steps over fenced code, tables, block quotes, horizontal rules,
and the `<!--` / `-->` that wrap the teacher notes, because a first version that
kept them buried the real closings under a list of stray backticks.  Prose
inside a teacher-note comment still gets read, which is right:  those notes are
addressed to a colleague and they close the same way.

The block below opens on **four** backticks, because the awk script matches a
fence and a three-backtick fence would close on its own pattern:

````bash
find . -name '*.md' -not -path './.git/*' -not -path './bin/*' -not -path './obj/*' \
  -exec awk '
    /^```/        {f=!f; next}
    f             {next}
    /^#{1,6} /    {if(p)print FILENAME":"pn": "p; p=""; next}
    /^(---|\||>|<!--|-->)/ {next}
    NF            {p=$0; pn=FNR}
    END           {if(p)print FILENAME":"pn": "p}' {} \;
````

It prints the last *line*, so a wrapped sentence arrives as its tail.  Treat
each hit as a line number to go read.

Read the output as a list.  Side by side the reached-for closings stand out
against each other, where each one hides well enough inside its own paragraph.

---

## Applying a rule across a repository

### 13.  Structural punctuation is exempt

A global find-and-replace for prose punctuation will hit syntax that only looks
like prose.  Every one of these was a real near-miss:

| Exempt | Example |
|---|---|
| JSON keys | `"key": "value"` |
| YAML frontmatter | `name: ascii-typography` |
| CSS declarations | `color: #fff` |
| URLs | `https://example.com` |
| Ordered-list markers | `1. Run it` |
| Column-aligned comment labels | `// TODO (Step 4):` |
| C# nullable types | `Dice? ByName(string? key)` |
| Abbreviations and honorifics | `Dr. John Wells`, `Ph.D.`, `e.g.` |

The honorific is the quiet one.  `Dr. John Wells` in `teaching_philosophy.md`
matches every sentence-break pattern in this file, and it is correct as it
stands, because the period ends an abbreviation and a sentence continues through
it.  A period followed by a capitalized word is a sentence break only when a
sentence actually ended.  Audit that shape by eye;  the list of honorifics is
short enough that a false positive announces itself.

The list markers deserve a note.  Protect them by **position**, since a
sentence can legitimately end on a digit:  `Read Scene 1.  Which exhibit
worries you most?`  Match `^\s*\d+\.\s` at the start of a line, hold that part
aside, and apply the rule to the rest.

The C# nullable is the genuine landmine, because `Dice? ByName` is
indistinguishable from a sentence by pattern alone.  Apply the rule, then repair
it in `.cs` files only.  That repair is safe there because every prose sentence
in those comments ends on a lowercase word.

### 14.  Generated files keep their own copy

Fixing the source and declaring victory leaves behind whatever the build step
writes from its own hardcoded strings.  In this repository the Canvas HTML is
generated from JSON by a PowerShell script, and that script carried four pieces
of prose of its own:  a banner, a stage-table footer, a helper-box caption, and
a file header.

**Check the generator, then rebuild, then audit the output.**

### 15.  Self-referential examples break

Applying a rule to the document that documents the rule turns the examples into
nonsense.  A memory file explaining that `Dice? ByName` must stay single-spaced
had its own example double-spaced.  Another said `the typographic -- has become
a tell`, which now says something else entirely.

When a document quotes the thing it is correcting, that quotation is a citation
and it stays exactly as written.

### 16.  The rule hides behind markup

A sentence that ends inside bold puts the closing `**` between the punctuation
and the space, so an audit anchored on `[.?!:] [A-Z]` walks straight past it.
`**Face up or face down.** Is that a fact about the card` is a single-spaced
sentence break wearing a costume.  One pass over this repository found a hundred
and three of them after two earlier passes had reported clean.

The capital-letter anchor misses two more shapes.  A colon inside bold usually
introduces a lowercase clause, `**The method:** rewrite the negative`, and it
still takes two spaces.  So does a sentence ending on a digit or a brace.

```bash
grep -rnE '[a-z)][.?!:](\*\*|\*) [^ ]' --include='*.md' --include='*.json' .
```

Requiring the emphasis marker is also what keeps the pass clear of the exempt
syntax in entry 13.  `// TODO (Step 4): sides is wide open` and `Dice? dice`
carry no markers, so a marker-anchored pattern leaves them alone by
construction, and the landmine defuses itself.

---

## Verifying the work

Four ways an audit reported clean when the work was still dirty, and one check
that runs on the ear.

### 17.  Case-sensitive search

`grep 'nothing'` misses `Nothing` at the start of a sentence, which is where it
most often appears.  Roughly a dozen instances survived a first pass this way.
Use `grep -i` for prose audits.

### 18.  Backticks inside a double-quoted shell string

```bash
grep -ohE "[a-z)\`*]\. [A-Z]" $FILES     # backtick opens command substitution
grep -ohE '[a-z)`*]\. [A-Z]' $FILES      # single quotes keep it literal
```

The first form reported zero matches across nine hundred real ones, and the
count looked like success.  **A surprising zero deserves a sanity check against
a pattern known to match.**

**A surprising hundred deserves the same check.**  Auditing
`teaching_philosophy.md` for single-spaced sentences, I wrote
`[a-z)"][.?!] [^ ]{0,30}` and got about a hundred hits in a file that a
narrower pattern had just called nearly clean.  The `{0,30}` was the bug:  it
matches zero characters, so every *correctly* double-spaced sentence matched on
the period, one space, and an empty tail.  `{1,30}` cut it to a single hit, and
that hit was `Dr. John`.  Any quantifier that can match zero can turn the compliant
text into the report.  Sanity-check a count in both directions, against text you
know is dirty and text you know is clean.

The same family, one layer up:  a heredoc written through a tool call can
arrive with its backslashes already consumed, so a script that reads correctly
in the message writes `\|` as `|` and `\x{2014}` as the character itself.
Building the backslash with `chr(92)` sidesteps the whole question.  When a
generated file shows the right characters and the wrong escapes, suspect
transit before suspecting the regex.

Perl's own escapes bite the same way.  Writing a documented `grep` pattern
into a file through a double-quoted string turned `\b` into an actual
**backspace**, 0x08.  It is invisible in every editor, it survived two readings
of the file, and `cat -A` was what finally showed it, as `^H`.

I then made the same mistake a second time while writing this very entry, which
is the argument for the rule:  anything that writes a regex into prose should
build the backslash with `chr(92)` rather than type it.  A sweep for control
characters is cheap and worth running after any such edit:

```bash
grep -rnP '[\x00-\x08\x0b\x0c\x0e-\x1f]' .
```

Run the audit through the same engine that ran the fix.  A pass applied with
perl and checked with grep is two regex dialects and two different ideas about
what counts as one character, and the disagreement shows up as either a
phantom hit or a silent miss.

### 19.  Reading it as spoken

The check a pattern will always miss.  Read the passage as though speaking it to
the person it addresses.  Keep the lines you would actually say to them, and
rewrite the rest.

**This works well before you can diagnose the problem**, which is what makes it
the first check to run.  The ear rules, and the reason can come afterward.
Every other entry in this file is a name for something the ear had already
caught.

It finds them one register at a time.  A softened directive sounds evasive out
loud.  An ornamented sentence sounds like a speech.  A definition by negation
sounds like somebody arguing with a person who left the room.

When your own ear has gone stale on a draft, hand it to the machine:

```powershell
Add-Type -AssemblyName System.Speech
$s = New-Object System.Speech.Synthesis.SpeechSynthesizer
$s.Rate = -1
$s.Speak((Get-Content -Raw .\Course\lessons\lesson_01_first_objects.md))
```

```bash
say -f Course/lessons/lesson_01_first_objects.md    # macOS
```

Feed it a passage at a time.  A whole file read aloud turns the markup into
noise, and attention goes with it.

---

## The shape of a good fix

Most entries here share a structure worth naming.  A rule that reads as
mechanical almost always has a boundary where the mechanism stops applying, and
that boundary is usually the difference between **prose** and **syntax**, or
between **composition** and **citation**.

Find the boundary before running the pass.  Then verify with a search built
from the opposite assumption, and read a sample of the output before
trusting the count.
