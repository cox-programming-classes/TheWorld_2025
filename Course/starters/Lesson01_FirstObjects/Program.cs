using Toolkit;

// ─────────────────────────────────────────────────────────────────────────────
//  Lesson 1 -- First Objects
//
//  Press Run (or:  dotnet run).  All three scenes already work.
//  Your job today is to make it honest.
// ─────────────────────────────────────────────────────────────────────────────

// One source of randomness for the whole program.  It is seeded, so every run
// produces the same numbers -- and so does everybody else's machine.
var rng = new Random(42);

Scene1_ADieIsThreeNumbers(rng);
Scene2_TheCursedD20();
Scene3_WhatAVariableHolds();


/// <summary>
/// Scene 1 -- building a complex thing out of simple things, and what a
/// record hands you for free.
/// </summary>
static void Scene1_ADieIsThreeNumbers(Random rng)
{
    Section("1.  A die is three numbers");

    var dice = new Dice { Count = 2, Sides = 6, Modifier = 3 };

    Console.WriteLine($"These dice are: {dice}");
    Console.WriteLine($"They can roll {dice.Minimum} at worst and {dice.Maximum} at best.");

    Console.Write("Eight rolls:  ");
    for (var i = 0; i < 8; i++)
        Console.Write($"{dice.Roll(rng),4}");
    Console.WriteLine();
    Console.WriteLine($"...and the dice are still {dice}. Rolling them left them alone.");
    Console.WriteLine();

    // Dice is a `record`, so C# compares two of them by their VALUES.
    var alsoTwoDSix = new Dice { Count = 2, Sides = 6, Modifier = 3 };
    Console.WriteLine($"A separately-built 2d6+3 == the first one?  {dice == alsoTwoDSix}");
    Console.WriteLine("Two objects.  Same three numbers.  A record says that counts as equal.");
    Console.WriteLine();

    // Those three numbers are all `int`.  That was a decision, and here is
    // some of what it bought and cost.
    Console.WriteLine("Why int, out of all the choices?");
    Console.WriteLine($"  7 / 2                 -> {7 / 2}      both sides are int, so the answer is int");
    Console.WriteLine($"  7 / 2.0               -> {7 / 2.0}");
    Console.WriteLine($"  0.1 + 0.2 == 0.3      -> {0.1 + 0.2 == 0.3}");
    Console.WriteLine($"  0.1 + 0.2             -> {(0.1 + 0.2):R}   double is fast and approximate");
    Console.WriteLine($"  0.1m + 0.2m == 0.3m   -> {0.1m + 0.2m == 0.3m}   decimal is exact and slow");
    Console.WriteLine();
    Console.WriteLine("A die comes in whole numbers, so int is right here.  It is right sometimes.");
}

/// <summary>
/// Scene 2 -- the party shares one d20.  What could go wrong.
/// </summary>
static void Scene2_TheCursedD20()
{
    Section("2.  The cursed d20");

    var partyDie = new Dice { Count = 1, Sides = 20 };
    Console.WriteLine($"The die on the table:  {partyDie}");

    // Your character has a +5 attack bonus, so you pick up the die and...
    var yourDie = partyDie;
    yourDie.Modifier = 5;

    Console.WriteLine($"Your die:              {yourDie}");
    Console.WriteLine($"The die on the table:  {partyDie}");
    Console.WriteLine();
    Console.WriteLine("The goblin is now attacking you with your own +5 bonus.");
    Console.WriteLine();
    Console.WriteLine("Every line here did exactly what it says.");
    Console.WriteLine("And Dice is a record.  Whatever a record protects, the table's die changed anyway.");

    // TODO (Step 6): once you have fixed this scene, the four lines above
    //                describe a problem you have already solved.  Rewrite them
    //                to say what happens now.  A comment that used to be true
    //                is worse than silence.
}

/// <summary>
/// Scene 3 -- why Scene 2 happened.
/// </summary>
static void Scene3_WhatAVariableHolds()
{
    Section("3.  What a variable holds");

    // An int is a VALUE. Copying the variable copies the number itself.
    int a = 5;
    int b = a;
    b = 9;
    Console.WriteLine($"a = {a}, b = {b}          two boxes, two numbers");

    // A record is a REFERENCE type.  Copying the variable copies the address.
    // Comparing by value and being copied by value are different questions,
    // and a record only answers the first one.
    var first = new Dice { Sides = 20 };
    var second = first;
    second.Sides = 4;
    Console.WriteLine($"first = {first}, second = {second}    two boxes, one die");

    // TODO (Step 5): the line above will stop compiling, and putting `set`
    //                back is off the table.  You will have to prove "one die,
    //                two names" some other way.  Try:
    //
    //                    ReferenceEquals(first, second)
    //
    //                Then answer the real question:  the sharing is still
    //                there.  What happened to it?

    // A string is a reference type that behaves like a value, because it is
    // fixed the moment it exists.  ToUpper() leaves the original alone and
    // builds a second string to hand back.
    string name = "goblin";
    string loud = name.ToUpper();
    Console.WriteLine($"name = {name}, loud = {loud}   a string is fixed at birth");
    Console.WriteLine();
    Console.WriteLine("Scene 2 is the second picture.  That is all it ever was.");
    Console.WriteLine("So:  what would it take to make Dice behave like the third one?");
}

/// <summary>Prints a section heading, padded out to a fixed width.</summary>
static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"── {title} {new string('─', Math.Max(0, 68 - title.Length))}");
}
