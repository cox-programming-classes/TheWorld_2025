# Lesson 4 - Collections

*Many things, and what many costs.*

Run it first.  Everything in here works, and every scene shows you a `List`
doing exactly what a `List` does, in a place that needed something with rules.

```bash
cd Course/starters/Lesson04_Collections
dotnet run
```

## The files

| File | What it is |
|---|---|
| `Dice.cs` | your dice from Lessons 1 through 3, finished.  Read it if you want the reminder, then leave it alone |
| `DiceBag.cs` | **today's file.**  A bag of dice you draw from, and the source of every problem below |
| `Program.cs` | five scenes.  Leave this alone until the end, the way you did in Lesson 3 |

## The one new idea

**A collection is itself an object**, with its own state, its own rules, and its
own opinions about what may go into it.

`List<T>` holds things.  Holding things is the entire job it signed up for.  It
leaves how many, which ones, and who may reach in entirely to you.  Those are
your rules, and they need somewhere to live.

## What the five scenes show you

1. **Five loose dice.**  Add a sixth and count the lines you have to touch.
2. **A rule with a way around it.**  `Add()` checks the capacity and refuses
   politely.  `Contents.Add()` walks straight past it, three times.
3. **"Draw."**  Five draws, one die, five times over.  Then draw from an empty
   bag and read whose words the crash is written in.
4. **A look at the contents.**  This is Lesson 1 at a larger size.  `init`
   sealed the slot, and the `List` the slot points at is still wide open.
5. **A shuffle that lands somewhere else.**  The algorithm is correct.  Read
   the method signature and say why the bag is unchanged anyway.

## The work

1. Read `DiceBag.cs` all the way through.  Write down every rule it is trying to
   enforce, and next to each one, how somebody could get around it.
2. Make the list **private** and hand out `IReadOnlyList<Dice>`.  Scene 4 stops
   working, and it stops working at compile time -- **CS1061**.
3. Fix `Draw`.  It should remove what it draws, and it should be honest when the
   bag is empty.  **How** it is honest is your call, and Lesson 2 gave you three
   options worth arguing about.
4. Re-run.  Scenes 2, 3, and 4 should all read differently now.
5. Fix `Shuffle`.  The algorithm is already right;  it is working on the wrong
   list.  Fisher-Yates walks from the end, picking a partner from the part it
   has yet to visit.
6. `DrawnCount` is a fact about what happened, so make it
   `{ get; private set; }` and let the bag be the only thing that writes it.
7. Put the bag's own rules back in charge:  `Standard()` can no longer use a
   collection initializer, and working out why is the point of the step.
8. Count the d20s in a bag with a `foreach` loop.  Then meet `.Count(d => ...)`
   and decide for yourself which one you would rather read in six months.

## Make it yours - the Cards challenge

Your `Card` has been waiting for this since Lesson 1.

- **`Deck`.**  Fifty-two cards, shuffled, dealt one at a time, and honest about
  running out.  The same three problems you just fixed, on your own type.
- **`Hand`.**  A collection with a limit.  Deal five cards into it and have it
  refuse the sixth.
- Then **deal a five-card hand and score it with both rule sets from Lesson 3**.
  Same cards, two games, two answers, and each file ignorant of the other.

## Went beyond?

- **The cast escape hatch.**  `IReadOnlyList<Dice>` says what you mean, and a
  determined caller can write `((List<Dice>)bag.Contents).Clear()` and get away
  with it.  Try it.  Then look up `AsReadOnly()` and decide whether the extra
  object is worth it for your `Deck`.
- **A shuffle you can swap.**  Let `Shuffle` take a *strategy* -- a function
  that receives the cards and hands back an order.  Riffle, cut, reverse, or a
  "shuffle" that quietly stacks the deck in your favour.  The bag checks that
  every card came back before it accepts the new order, which is a small design
  lesson hiding inside a card trick.
