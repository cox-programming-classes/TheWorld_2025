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

### 22.  Register settled before the first sentence

Numbered 22 so the cross-references in entries 1 through 21 keep pointing where
they point.  It sits at the head of this section because it runs ahead of every
other entry in it.

**Answer three questions in one line before drafting a word:**

| Question | What it settles |
|---|---|
| Who is the audience? | Vocabulary, how much gets assumed, what needs spelling out |
| How serious is it? | Whether a joke belongs in the room at all |
| What is the vibe? | Which of his registers this document is written from |

**Where it came from.**  On 2026-09-09, an email to the Winsor student body about
the FIRST Tech Challenge season Kickoff.  The first draft arrived in the register
of `teaching_philosophy.md`, because "email to the student body" sounded
institutional.  His correction:  *"this one can be less formal.  it is in my
character as the whimsical, eccentric robotics mentor/teacher.  Think Cliff Stoll
vibes."*

**The rule.**  [`APPALACHIAN_ACADEMIC_STYLE.md`](APPALACHIAN_ACADEMIC_STYLE.md)
says what the voice is.  The register inside that voice moves with the role he is
writing from.  A letter to administrators and an email from the robotics mentor
are one voice at two settings.  So the question to ask is which hat he is
wearing, and the document type answers it badly:  "school-wide email" covers a
fire-drill notice and a Kickoff invitation both.

Ask, and ask early.  A register named in one line gets corrected in one line.  A
register recovered from a finished draft costs a rewrite, and this one cost two.

**Then hold it, which is the half that hides.**  The Kickoff email was rewritten
for the whimsical mentor and measured at **zero contractions per thousand
words** -- the same score as `teaching_philosophy.md`, a formal essay written to
administrators.  Second person had climbed sevenfold and every verb had stayed in
Sunday clothes.  A declared register with the old prose underneath it sits one
edit away from looking finished.

Entry 4 is this same error from the other end.  There a contraction count taken
from a formal essay became a rule banning contractions everywhere.  Here a
declared informal register kept the formal count.  Both come from treating one
document's numbers as the voice.

**What survives a register shift.**  Entries 1 through 3 are mechanical and hold
at every setting.  Entry 11 holds too, and the interaction is worth naming:  the
load-bearing sentence stays plainest wherever the dial sits.  In the Kickoff
email the whimsy runs everywhere except the apology for a date that collided with
Rosh Hashanah, which is the highest-stakes passage in the document and is written
flattest on purpose.  A register governs the prose around the important sentence.

**The check.**  Register leaves countable traces, so a declared register is
testable.  Run this over the draft and over a document already in the register
you want, then compare the two.

```bash
register() {
  for f in "$@"; do
    w=$(wc -w < "$f")
    c=$(grep -oiE 'n[^[:alnum:] ]t\b|[^[:alnum:] ](re|ve|ll|m)\b|\b(it|that|there|here|what|who|let|we|you|they)[^[:alnum:] ]s\b' "$f" | wc -l)
    y=$(grep -oiE '\b(you|your|yours)\b' "$f" | wc -l)
    m=$(grep -oiE '\b(moreover|furthermore|therefore|thus|hence|accordingly|pursuant|whereby|herein|in order to|at this time|please be advised|kindly)\b' "$f" | wc -l)
    awk -v f="$f" -v w="$w" -v c="$c" -v y="$y" -v m="$m" \
      'BEGIN{printf "%-30s %5d words  contr %5.1f  2nd %5.1f  formal %5.1f\n", f, w, c*1000/w, y*1000/w, m*1000/w}'
  done
}
```

Two measured poles, for calibration.  Both figures are per thousand words:

| Document | Contractions | Second person |
|---|---|---|
| `teaching_philosophy.md`, formal essay to administrators | 1.6 | 1.0 |
| The Kickoff email as he sent it, whimsical robotics mentor | 23.4 | 28.1 |

**His own informal markers, taken from the version he actually sent.**  Editing
the Kickoff draft he rewrote my closing, *"I would like us in the room when it
does,"* as *"It's gonna be a party~"*.  Two things there.  `gonna` is a register
the draft had reached for and missed, and the tilde is his:  it turns up in his
own messages, `Class III~Class VII` and `permission slips to their parents~`.  So
his informal setting carries a punctuation mark of its own, and a draft that
arrives at the whimsical setting through vocabulary alone is still a step short.

The position of that edit is entry 12 landing on schedule.  My closing was the
composed literary one, sitting exactly where entry 12 says a reached-for landing
collects, and it was the sentence he replaced.

The apostrophe is written as the bracket expression `[^[:alnum:] ]`, which keeps
the pattern clear of entry 18's quoting trap and matches a straight and a curly
apostrophe alike.  Verified in both directions:  four hits on `I'm sorry, I
won't, we'll see, that's it`, zero on that same sentence spelled out, and zero on
`the author's argument, the students' work`, so possessives leave the count
alone.

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

### 23.  The metaphor that arrives before its setup

Numbered 23 so the cross-references in entries 1 through 22 keep pointing where
they point.  It belongs here, beside entry 5, because it is the third way a
borrowed phrase goes wrong.

Entry 5 covers a phrase invented to sound like Jason, and the sub-point above
covers one of his borrowed at full weight and spent on something small.  This is
the remaining case:  **his phrase, at its right weight, placed before the thing
that earns it.**

**Where it came from.**  On 2026-09-10, the Anvil review prompts in
`Course/flint/`.  Jason flagged *"put a door on it"* as weird language.  The
phrase is his, four times over -- `lesson_02_validation_and_factories.md` has
"Today we put a door on that moment", `Dice.cs` has "One door in", and Scene 3
prints "the same door everything else goes through".  In the lesson plan it
arrives *after* board item 3, titled "One door", has introduced the constructor
as the door and the factories as the way in.

The draft used it in board item 1, two items early.  So the metaphor named a
thing the reader had yet to meet, and it read as invented even though every word
of it was his.

**Why compression causes it.**  These prompts are written to a hard character
budget, and a trimming pass cuts setup sentences first -- they explain, so they
look expendable, where the payoff line is vivid and looks load-bearing.  Cut
enough setup and the payoff stands alone.  Six trim passes on that
file removed the sentence that introduced the door and kept the one that used it.

**The fix:**  for each figure of speech, find the sentence that introduces it and
check it still comes first.  Where the setup is gone, either restore it or say
the thing plainly:  "One moment is left, and closing it is the whole of today."

**The check** is provenance plus position, and a grep only does the first half.

```bash
grep -rniF 'put a door' Course/lessons Course/starters   # is the phrase his?
```

A hit means the phrase is safe to use;  it leaves open whether this document has
earned it yet.  Read the paragraphs above each use.  The same pass
catches the related damage:  a trim that clips a phrase to something ungrammatical
("has met the design working", from his "has met the design working correctly")
and a trim that paraphrases him where his own words fit ("A metaphor in place of
the word", from his "Hiding the word behind a metaphor").  **After any length-driven pass,
diff the trimmed prose against the source it restates.**

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

**The boundary is genre.**  Concede-first governs an argument, where an admission up
front earns the case a hearing.  A warm letter works the other way.  Revising a first
letter to a parent, Jason put the pleasure of the thing first and the limitation second:
*"It's good to get to work with Anika in upper school now after teaching her in lower
school classes.  I have a decent sense of her at school, but not so much everything
outside."*  An assistant draft had opened that paragraph on the limitation by itself, and
he called it "not particularly warm."  Both orders concede.  The question is what the
passage is for, so ask whether the reader is being persuaded or greeted.

The grep finds the markers, and reading tells you where they sit.  A marker in
the first third of a section is carrying the argument.  One in the closing
paragraph has become a disclaimer.

```bash
grep -rniE '\b(that said|to be fair|of course|admittedly|granted|it is true that|i am (fully )?aware|i will also say|to their credit)\b' --include='*.md' .
```

### 20.  The invented concession

Numbered 20 so the cross-references in entries 11 through 19 keep pointing where they
point.  It belongs here, beside entry 10.

Entry 10 asks for the concession first.  A draft that has absorbed that rule and lacks
anything to concede will manufacture something, and the invention is harder to catch than a
misplaced concession, because the shape looks right.

Three of these landed in six parent letters on 2026-09-09.

| Written | The trouble |
|---|---|
| `I'll admit it duplicates an email you're already sending` | The ask was a cc.  It duplicates a keystroke. |
| `I have a welcome survey, which is thin material` | He taught that student in lower school. |
| `my sense of her is a year old and mostly from a distance` | Invented from an enrollment date.  He was meeting her for the first time. |

The first invents a cost.  The second and third invent a fact, which puts them alongside
the style guide's ban on inventing experience, and a parent is the reader likeliest to know
it is wrong.

**Why the shape survives review:**  a fabricated concession reads as humility, so it
flatters the draft and the drafter both.  It also misdescribes the ask.  "Just cc me"
states the size of the favor;  the invented version made a keystroke sound like an errand.

**The fix:**  name the actual weakness before writing the sentence.  Where one exists,
concede it.  Otherwise state the ask at its true size and move on.  Where the concession
would be a fact about a person, go and get the fact.

**The tell** is a concession about the writer's own knowledge of somebody:  how long he has
known her, how well, what he has to go on.  Those are facts he holds and an assistant
lacks, so they arrive by guess.  Ask.

```bash
grep -rniE "i'?ll admit|i (should|will) be plain|thin material|(all|only) i have|(what|all) i have (of|on) (her|him|them)|mostly from a distance|i (barely|hardly) know" --include='*.md' .
```

It hits Jason's own genuine concessions as well, `I should be plain about my own
temperament` in `teaching_philosophy.md` among them, and those are the model rather than
the error.  So read each hit.  The question a grep cannot answer is whether the weakness is
real, which is the whole of this entry.

### 21.  The concession that assesses the reader

A concession is about the writer.  Point it at the reader's family and it turns into an
appraisal of them.

Repairing the fabrication in entry 20, an assistant wrote to a parent:  *"Knowing Anaya
tells me plenty about your family and fairly little about Isha herself."*  The intent was
sound, to avoid treating two sisters as one person.  What the sentence delivers is a report
on what the writer has worked out about a family from watching one of its children.
Jason's verdict was "creepy," and it is the right verdict.

**The fix:**  keep the knowledge claim on the writer and the student.  *"Isha and I are
starting fresh, though, and I mean to know her on her own terms."*  Same respect for her
separateness, and the family stays out of it.

**The test:**  read the sentence back as though the reader had written it about you.  A line
that would unsettle you coming the other way is doing this.

**Why it comes up here:**  entries 20 and 21 both arrived from one paragraph on 2026-09-09.
The first repair for a fabricated concession overshot into this, so a correction can hand
you the next error.  Read the replacement as carefully as the original.

```bash
grep -rniE "tells me (a lot|plenty|much|something) about (you|your)|(what|how much) i (know|can tell) about (you|your)|i can tell (a lot|that) (about|from) you" --include='*.md' .
```

**The variant that grants permission.**  One step further out than the entry
above, and it arrived from the same week's work.  Apologizing in the Kickoff
email for a date that collided with Rosh Hashanah, an assistant wrote *"If you're
observing, observe -- that's the right call and it's yours to make."*  Jason cut
the sentence whole.

The apology had already finished at *"I'm sorry."*  What followed ruled on the
reader's religious observance and then granted authority the reader held the entire
time.  Entry 21's original sentence reported on a family;  this one issues a
permit.  Both raise the writer a step above the reader while sounding generous,
which is why both survive a read-through.

**The test:**  ask what standing the writer has to say it.  Where the answer is
that the reader decides, apologize and stop.

The apostrophe in the pattern below is written as `.` so the pattern stays clear
of entry 18's quoting trap and catches a straight and a curly apostrophe both.

```bash
grep -rniE 'that.s (the right|your) call|it.s yours to (make|decide)|the call is yours|as (you|is) right for you|whatever you decide is' --include='*.md' .
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

**Where this entry came from, and what has since confirmed it.**  Every other
entry here was earned by a correction Jason made to a draft.  This one arrived
from an assistant reading its own drafting habits, and an audit of
`teaching_philosophy.md` found the pattern absent from his prose.  So it began as
a description of what assistants do to his writing, resting on introspection.

A correction on 2026-09-09 settled it.  In a parent letter Jason changed *"their
classmates are carrying similar things"* to *"dealing with similar things."*  The
figure sat in the closing sentence of its paragraph, which is the position this
entry predicts, and *carrying* is the borrowed weight entry 5 describes being
spent on something small.  The entry now rests on a real correction, and the
footing is as firm as entry 3 or entry 8.

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

**A multi-word phrase wraps, so a line-based search misses it.**  Checking which
figures of speech in a draft were Jason's, a `grep -F` for each phrase reported
"no source" for several that are plainly in the lesson plans:  the source wraps
"a good / afternoon ahead" and "Teach up / to the edge" across a line break, and
`grep` reads one line at a time.  Acting on that report would have stripped his
own phrases as inventions.  Flatten both sides before comparing:

```python
import re
flat = lambda t: re.sub(r'\s+', ' ', t).lower()
```

The same run had a second failure worth knowing:  **`grep` on this machine is
`ugrep`**, which took an unquoted shell variable holding several paths as one
filename and only warned, so every count came back zero.  Pass
paths as a shell array, and treat a zero from a multi-path search as suspect
until one known-present phrase confirms the invocation.

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
