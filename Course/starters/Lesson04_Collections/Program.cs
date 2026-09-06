using Toolkit;

// ─────────────────────────────────────────────────────────────────────────────
//  Lesson 4 - Collections
//
//  Everything below runs.  Five scenes, and every one of them is a `List`
//  doing exactly what a `List` does, in a place that needed something with
//  rules.  Read the output next to the code before you change anything.
// ─────────────────────────────────────────────────────────────────────────────

Scene("1.  Five loose dice");

var d4 = Dice.D4;
var d6 = Dice.D6;
var d8 = Dice.D8;
var d12 = Dice.D12;
var d20 = Dice.D20;

var rng = new Random(20260906);
Console.WriteLine($"  {d4} rolled {d4.Roll(rng)}");
Console.WriteLine($"  {d6} rolled {d6.Roll(rng)}");
Console.WriteLine($"  {d8} rolled {d8.Roll(rng)}");
Console.WriteLine($"  {d12} rolled {d12.Roll(rng)}");
Console.WriteLine($"  {d20} rolled {d20.Roll(rng)}");
Console.WriteLine();
Console.WriteLine("  Now add a d10.  Count how many lines you would have to touch.");
Console.WriteLine("  Now answer \"how many dice do I have?\" without counting by eye.");

Scene("2.  A rule with a way around it");

var bag = DiceBag.Standard();
Console.WriteLine($"  {bag}   capacity {bag.Capacity}");

// Add() is careful.  It checks, and it refuses.
var accepted = bag.Add(Dice.Of(1, 100));
Console.WriteLine($"  bag.Add(1d100)      -> {accepted}   (the bag is full, so it said no)");

// Contents is public, so the rule is optional.
bag.Contents.Add(Dice.Of(1, 100));
bag.Contents.Add(Dice.Of(1, 100));
bag.Contents.Add(Dice.Of(1, 100));
Console.WriteLine($"  bag.Contents.Add()  -> went in three times without asking");
Console.WriteLine($"  {bag}   <- a bag of six, holding nine");

Scene("3.  \"Draw\"");

var draws = DiceBag.Standard();
Console.Write("  five draws:  ");
for (var i = 0; i < 5; i++) Console.Write($"{draws.Draw()}  ");
Console.WriteLine();
Console.WriteLine($"  the bag still holds {draws.Count}, and DrawnCount says {draws.DrawnCount}");
Console.WriteLine("  Read Draw() and say which word in its name it is failing to do.");

Console.WriteLine();
var empty = new DiceBag { Name = "empty bag", Capacity = 6 };
try
{
    empty.Draw();
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine("  Drawing from an empty bag:");
    Console.WriteLine($"    {ex.GetType().Name}:  {ex.Message.Split('\n')[0]}");
    Console.WriteLine("    Those are somebody else's words again.  That message came from");
    Console.WriteLine("    inside List, about an index.  The word 'index' belongs to List alone.");
}

Scene("4.  A look at the contents");

var mine = DiceBag.Standard();
Console.WriteLine($"  before:  {mine}");

// "I just want to look at what is in there."
var snapshot = mine.Contents;
snapshot.Clear();

Console.WriteLine($"  after a look:  {mine}");
Console.WriteLine();
Console.WriteLine("  Lesson 1, at a larger size.  `Contents` is { get; init; }, so the");
Console.WriteLine("  slot is sealed and the List the slot points at is still wide open.");
Console.WriteLine("  One `var snapshot = ...` and the caller is holding the bag's insides.");

Scene("5.  A shuffle that lands somewhere else");

var table = DiceBag.Standard();
Console.WriteLine($"  before:  {string.Join(", ", table.Contents)}");
table.Shuffle();
Console.WriteLine($"  after:   {string.Join(", ", table.Contents)}");
Console.WriteLine();
Console.WriteLine("  Shuffle() does honest work -- open it and read it, the algorithm is");
Console.WriteLine("  correct.  Say why the bag is unchanged anyway.  The answer is one word");
Console.WriteLine("  in the method signature.");

Scene("Where this is going");

Console.WriteLine("  A `List` is indifferent to every bit of this.  It will hold nine dice in a");
Console.WriteLine("  bag of six, hand out its insides to anyone who asks, and let a shuffle");
Console.WriteLine("  land on a list the bag has already let go of.");
Console.WriteLine();
Console.WriteLine("  It is doing its job.  Holding things is the whole job.  The rules about");
Console.WriteLine("  WHICH things, HOW MANY, and WHO MAY CHANGE THEM belong to a type that");
Console.WriteLine("  you are about to write.");

static void Scene(string title)
{
    Console.WriteLine();
    Console.WriteLine($"── {title} {new string('─', Math.Max(0, 68 - title.Length))}");
    Console.WriteLine();
}
