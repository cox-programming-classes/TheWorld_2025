using Toolkit;

// ─────────────────────────────────────────────────────────────────────────────
//  Lesson 2 -- Data Validation and Factory Methods
//
//  Lesson 1 settled who is allowed to change a value once it exists, and the
//  answer was that the door is shut.  Which leaves exactly one moment when a
//  set of dice can be wrong:  the moment it is built.  Today we close that too.
//
//  Notice something before you start:  the compiler rejects `new Dice(...)`
//  anywhere in this file.  Try it.  The constructor is private, so the way to
//  get dice is to ask Dice for them -- and every one of those ways is a method
//  somebody wrote on purpose, with rules in it.
//
//  This program is a report card on your Dice type.  Run it now, then run it
//  again after every change.  The output should get less embarrassing.
// ─────────────────────────────────────────────────────────────────────────────

Scene1_TheMuseumOfBadDice();
Scene2_TextFromSomewhereElse();
Scene3_Factories();


/// <summary>
/// Scene 1 -- eight sets of dice, several of which the game should have refused.
/// </summary>
static void Scene1_TheMuseumOfBadDice()
{
    Section("1.  The museum of bad dice");

    Exhibit("an ordinary roll", () => Dice.Of(2, 6));
    Exhibit("a roll of zero dice", () => Dice.Of(0, 6));
    Exhibit("negative dice", () => Dice.Of(-3, 6));
    Exhibit("two hundred dice", () => Dice.Of(200, 6));
    Exhibit("a one-sided die", () => Dice.Of(1, 1));
    Exhibit("a zero-sided die", () => Dice.Of(1, 0));
    Exhibit("a die with -4 sides", () => Dice.Of(1, -4));
    Exhibit("a modifier from another game", () => Dice.Of(1, 20, 500));

    Console.WriteLine();
    Console.WriteLine("Every exhibit marked ROLLED is dice your game now has to live with.");
    Console.WriteLine("Of() let them through.  Of() is the only thing that could have stopped them.");
}

/// <summary>
/// Tries to build the dice, then tries to roll them, and reports which of the
/// two it got to.
///
/// Those are deliberately separate.  The question today is whether a bad value
/// gets stopped where the mistake was made, or sails on and detonates
/// somewhere else, later, in a message written by a stranger.
/// </summary>
/// <param name="label">What to call this exhibit.</param>
/// <param name="make">How to build it.</param>
static void Exhibit(string label, Func<Dice> make)
{
    Dice dice;
    try
    {
        dice = make();
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"  refused   {label,-30} {ex.Message.Split('(')[0].Trim()}");
        return;
    }

    try
    {
        var rng = new Random(7);
        var rolls = new int[5];
        for (var i = 0; i < 5; i++)
            rolls[i] = dice.Roll(rng);
        Console.WriteLine($"  ROLLED    {label,-30} {dice,-10} -> {string.Join(", ", rolls)}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  ACCEPTED, THEN CRASHED   {label,-19} {ex.GetType().Name}: {ex.Message.Split('(')[0].Trim()}");
    }
}

/// <summary>
/// Scene 2 -- fifteen lines of notation straight from somebody's first draft.
/// </summary>
static void Scene2_TextFromSomewhereElse()
{
    Section("2.  Text from somewhere else");

    // Somebody else's typos, and every one of them has to leave the game standing.
    string[] rulebook =
    {
        "2d6",          // the ordinary case
        "d20",          // count left off -- everyone writes it this way
        "3d8+2",        // with a modifier
        "1d4-1",        // with a negative modifier
        " 4d6 ",        // somebody's spacebar
        "2D6",          // somebody's shift key
        "1d20+3",       // fine
        "2 d 6",        // ...is this the same as "2d6"? your call, and defend it
        "d%",           // real tabletop notation for percentile. out of scope here
        "twenty",       // words
        "0d6",          // zero dice
        "1d1",          // a one-sided die
        "2d6+",         // trailing sign, dangling
        "6d6+500",      // a modifier from another game entirely
        "",             // an empty line
    };

    var loaded = new List<Dice>();
    var rejected = new List<string>();

    foreach (var line in rulebook)
    {
        if (Dice.TryParse(line, out var dice))
            loaded.Add(dice);
        else
            rejected.Add($"'{line}'");
    }

    Console.WriteLine($"loaded {loaded.Count}: {string.Join("  ", loaded)}");
    Console.WriteLine($"rejected {rejected.Count}: {string.Join("  ", rejected)}");
    Console.WriteLine();
    Console.WriteLine("The program stayed up, and every survivor is valid.  That is what");
    Console.WriteLine("a Try- method is for:  failure comes back as an answer.");
    Console.WriteLine();
    Console.WriteLine("And notice TryParse leaves every rule where it found it.  It ends by");
    Console.WriteLine("calling Of(), the same door everything else goes through.");
}

/// <summary>
/// Scene 3 -- once the type defends itself, making things gets pleasant again.
/// </summary>
static void Scene3_Factories()
{
    Section("3.  Factories");

    var rng = new Random(2025);

    var attack = Dice.Attack(3);
    Console.WriteLine($"Dice.Attack(3)       -> {attack}   rolls {attack.Roll(rng)}");

    var stat = Dice.AbilityScore();
    Console.WriteLine($"Dice.AbilityScore()  -> {stat}   rolls {stat.Roll(rng)}");

    Console.WriteLine();
    foreach (var key in new[] { "attack", "d20", "ABILITY SCORE", "d7", "cheese" })
    {
        var made = Dice.ByName(key);
        Console.WriteLine($"  ByName(\"{key}\")".PadRight(28) + " -> " + (made?.ToString() ?? "null, and that is fine"));
    }

    Console.WriteLine();
    Console.WriteLine("Four ways to make dice, four different jobs:");
    Console.WriteLine("  Dice.Of(1, 20, 3)       you know exactly what you want");
    Console.WriteLine("  Dice.Attack(3)          you know what it is FOR");
    Console.WriteLine("  Dice.ByName(\"attack\")   you are holding a key from a save file");
    Console.WriteLine("  Dice.TryParse(text)     you are holding text and hoping");
    Console.WriteLine();
    Console.WriteLine("All four end up at Of().  One door, one set of rules, every caller through it.");
}

/// <summary>Prints a section heading, padded out to a fixed width.</summary>
static void Section(string title)
{
    Console.WriteLine();
    Console.WriteLine($"── {title} {new string('─', Math.Max(0, 68 - title.Length))}");
}
