using Toolkit;
using Toolkit.Display;
using Toolkit.Rules;

// ─────────────────────────────────────────────────────────────────────────────
//  Lesson 3 -- Extensions and Separating Concerns
//
//  Everything below works today, and will keep working.
//
//  Three `using` lines sit at the top of this file.  By the end of the lesson
//  they will mean something:  each is this program admitting to a thing it
//  depends on.  Right now two of them are nearly lies -- almost everything
//  actually comes out of Dice.cs, which has quietly become the whole program.
// ─────────────────────────────────────────────────────────────────────────────

Scene1_WhatDiceDo();
Scene2_WhatDiceLookLike();
Scene3_WhatDiceMeanInThisGame();
Scene4_TheWorkedExample();
Scene5_TheOtherThingExtensionsDo();


/// <summary>
/// Scene 1 -- dice being dice.  This part is fine.
/// </summary>
static void Scene1_WhatDiceDo()
{
    Section("1.  What dice do");

    var rng = new Random(11);
    var dice = Dice.Parse("2d6+3");

    Console.WriteLine($"{dice}   min {dice.Minimum}   max {dice.Maximum}   average {dice.Average}");
    Console.Write("Six rolls:  ");
    for (var i = 0; i < 6; i++)
        Console.Write($"{dice.Roll(rng),4}");
    Console.WriteLine();
}

/// <summary>
/// Scene 2 -- dice being drawn.  Look at where these methods live before you
/// enjoy them.
/// </summary>
static void Scene2_WhatDiceLookLike()
{
    Section("2.  What dice look like");

    Console.WriteLine(Dice.Parse("3d6").ToBoxArt());
    Console.WriteLine();

    Dice.Parse("2d6").PrintHistogram(600, new Random(3));
    Console.WriteLine();

    var rng = new Random(99);
    Console.WriteLine("Five d20 attack rolls:");
    for (var i = 0; i < 5; i++)
        Dice.D20.PrintRoll(rng);
}

/// <summary>
/// Scene 3 -- dice being judged by the rules of one specific game.
/// </summary>
static void Scene3_WhatDiceMeanInThisGame()
{
    Section("3.  What dice mean in this game");

    var rng = new Random(2024);
    var attack = Dice.Attack(5);

    foreach (var dc in new[] { 10, 15, 20 })
        Console.WriteLine($"  {attack} vs DC {dc,-3} -> about {attack.ChanceOfBeating(dc, 20000, rng):P0} of the time");

    Console.WriteLine();
    Console.WriteLine($"  faces [20]     critical? {Dice.D20.IsCritical(new[] { 20 })}");
    Console.WriteLine($"  faces [1]      fumble?   {Dice.D20.IsFumble(new[] { 1 })}");
    Console.WriteLine($"  a 17 beats DC 15?        {Dice.D20.Beats(17, 15)}");
    Console.WriteLine();
    Console.WriteLine("Now:  is a natural 20 a fact about a d20, or a rule of this game?");
    Console.WriteLine("Somebody's board game about trains also rolls d20s.  Ask what they'd want.");
    Console.WriteLine();

    // Two different questions, and right now only one of them has anywhere to live.
    Console.WriteLine("Is it a die?  Is it a die you may use HERE?");
    foreach (var text in new[] { "2d6", "1d20+3", "1d20-50", "500d6" })
    {
        if (Dice.TryParse(text, out var d))
            Console.WriteLine($"  {text,-9} -> built as {d,-9} legal here? {d.IsLegalHere}");
        else
            Console.WriteLine($"  {text,-9} -> Dice refused to build it at all");
    }

    Console.WriteLine();
    Console.WriteLine("Lesson 2 left a question hanging:  is 1d20-50 valid?  Look at which");
    Console.WriteLine("column answered just now, and whether that is the right one to have.");
}

/// <summary>
/// Scene 4 -- the worked example, and a method that has been quietly ignored
/// this whole time.
/// </summary>
static void Scene4_TheWorkedExample()
{
    Section("4.  The worked example");

    var dice = Dice.Parse("2d6+3");

    Console.WriteLine($"  dice.Describe()  ->  {dice.Describe()}");
    Console.WriteLine();
    Console.WriteLine("There are TWO methods called Describe that could have answered that.");
    Console.WriteLine("  * an instance method, on Dice");
    Console.WriteLine("  * an extension method, in Display/DiceDisplay.cs");
    Console.WriteLine();
    Console.WriteLine("Both are in scope.  Open both, work out which one just ran, and");
    Console.WriteLine("predict what the line above prints once you delete the other one.");
    Console.WriteLine();
    Console.WriteLine("Then delete `using Toolkit.Display;` from the top of this file.");
    Console.WriteLine("Watch which lines stop compiling.  That is what a namespace is FOR.");
}

/// <summary>
/// Scene 5 -- a method call on an empty variable.
/// </summary>
static void Scene5_TheOtherThingExtensionsDo()
{
    Section("5.  The other thing extensions do");

    Dice? missing = null;

    Console.WriteLine($"  missing.Summarize()  ->  {missing.Summarize()}");
    Console.WriteLine();
    Console.WriteLine("A method call on null that comes back clean.  Explain that.");
    Console.WriteLine();
    Console.WriteLine("Both of today's surprises are the same fact wearing different");
    Console.WriteLine("clothes:  dice.Summarize() is a static call in disguise.  The");
    Console.WriteLine("compiler rewrote it to DiceDisplay.Summarize(dice) before the");
    Console.WriteLine("program ever ran -- using the type the variable is DECLARED as.");
}

/// <summary>Prints a section heading, padded out to a fixed width.</summary>
static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"── {title} {new string('─', Math.Max(0, 68 - title.Length))}");
}
