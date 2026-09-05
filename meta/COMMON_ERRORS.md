# Common Errors - prose and writing style

A running list of the mistakes that actually happen when an assistant drafts or
edits prose for this repository.  Every entry here was caught in real work.

Read this alongside [`APPALACHIAN_ACADEMIC_STYLE.md`](APPALACHIAN_ACADEMIC_STYLE.md).
That document says what the voice is.  This one says where it goes wrong.

**Adding to the list:**  when a correction happens, add the pattern, the fix, and
a command that finds it again.  An entry earns its place by being catchable.

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
borrowed cleverness.  It appears roughly four times in eighteen thousand words
of Jason's own writing, against sixty-one in one assistant draft of comparable
length.

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

---

## Applying a rule across a repository

### 8.  Structural punctuation is exempt

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

The list markers deserve a note.  Protect them by **position**, since a
sentence can legitimately end on a digit:  `Read Scene 1.  Which exhibit
worries you most?`  Match `^\s*\d+\.\s` at the start of a line, hold that part
aside, and apply the rule to the rest.

The C# nullable is the genuine landmine, because `Dice? ByName` is
indistinguishable from a sentence by pattern alone.  Apply the rule, then repair
it in `.cs` files only.  That repair is safe there because every prose sentence
in those comments ends on a lowercase word.

### 9.  Generated files keep their own copy

Fixing the source and declaring victory leaves behind whatever the build step
writes from its own hardcoded strings.  In this repository the Canvas HTML is
generated from JSON by a PowerShell script, and that script carried four pieces
of prose of its own:  a banner, a stage-table footer, a helper-box caption, and
a file header.

**Check the generator, then rebuild, then audit the output.**

### 10.  Self-referential examples break

Applying a rule to the document that documents the rule turns the examples into
nonsense.  A memory file explaining that `Dice? ByName` must stay single-spaced
had its own example double-spaced.  Another said `the typographic -- has become
a tell`, which now says something else entirely.

When a document quotes the thing it is correcting, that quotation is a citation
and it stays exactly as written.

### 11.  The rule hides behind markup

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
syntax in entry 8.  `// TODO (Step 4): sides is wide open` and `Dice? dice`
carry no markers, so a marker-anchored pattern leaves them alone by
construction, and the landmine defuses itself.

---

## Verifying the work

Four ways an audit reported clean when the work was still dirty.

### 12.  Case-sensitive search

`grep 'nothing'` misses `Nothing` at the start of a sentence, which is where it
most often appears.  Roughly a dozen instances survived a first pass this way.
Use `grep -i` for prose audits.

### 13.  Backticks inside a double-quoted shell string

```bash
grep -ohE "[a-z)\`*]\. [A-Z]" $FILES     # backtick opens command substitution
grep -ohE '[a-z)`*]\. [A-Z]' $FILES      # single quotes keep it literal
```

The first form reported zero matches across nine hundred real ones, and the
count looked like success.  **A surprising zero deserves a sanity check against
a pattern known to match.**

The same family, one layer up:  a heredoc written through a tool call can
arrive with its backslashes already consumed, so a script that reads correctly
in the message writes `\|` as `|` and `\x{2014}` as the character itself.
Building the backslash with `chr(92)` sidesteps the whole question.  When a
generated file shows the right characters and the wrong escapes, suspect
transit before suspecting the regex.

### 14.  Counting capture groups

`my $c = () = $t =~ /(a)(b)/g;` in list context returns one element per capture
group per match, so a pattern with two groups reports double.  Divide by the
group count, or count with a group-free pattern.

### 15.  grep in a byte locale

With `LANG` unset, GNU grep reads a **bracket expression** one byte at a time.
A class holding the two typographic dashes becomes the four bytes they are
built from, and it then matches any UTF-8 character sharing one of those bytes.
An audit written that way reported hits in eight files, and the matches were a
right arrow, a filled bullet, and a less-than-or-equal sign.

Alternation is byte-safe, because each branch matches as a literal byte
sequence.  The distinction is the whole entry:

```bash
grep -rnE '[—–]' .                            # a class:  byte-matched, silently wrong
grep -rn  '—\|–' .                            # alternation:  correct
perl -ne 'print if /[\x{2014}\x{2013}]/' f    # what I reach for now
```

Run the audit through the same engine that ran the fix.  A pass applied with
perl and checked with grep is two regex dialects and two different ideas about
what counts as one character, and the disagreement shows up as either a
phantom hit or a silent miss.

---

## The shape of a good fix

Most entries here share a structure worth naming.  A rule that reads as
mechanical almost always has a boundary where the mechanism stops applying, and
that boundary is usually the difference between **prose** and **syntax**, or
between **composition** and **citation**.

Find the boundary before running the pass.  Then verify with a search built
from the opposite assumption, and read a sample of the output before
trusting the count.
