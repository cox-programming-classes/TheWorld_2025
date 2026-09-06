namespace Toolkit;

/// <summary>
/// A bag of dice you draw from.  Every piece of this runs, and every piece of
/// it is wrong in a way <c>Program.cs</c> will show you.
///
/// Notice it is a <c>class</c> rather than a <c>record</c>.  That is deliberate
/// and it is worth a minute:  two bags holding the same six dice are still two
/// different bags.  A bag has an identity that survives its contents changing,
/// which is exactly the thing a record refuses to have.
/// </summary>
public class DiceBag
{
    /// <summary>What this bag is called.  Fixed when the bag is made.</summary>
    public string Name { get; init; } = "bag";

    /// <summary>
    /// How many dice this bag is willing to hold.
    ///
    /// Read <c>Add</c> below, then read Scene 2 in <c>Program.cs</c>, and ask
    /// what this number is currently worth.
    /// </summary>
    public int Capacity { get; init; } = 6;

    /// <summary>
    /// The dice in the bag.
    ///
    /// TODO (Step 2):  this is public, and it is a `List`.  Between those two
    /// facts, every rule below is a suggestion.
    /// </summary>
    public List<Dice> Contents { get; init; } = [];

    /// <summary>
    /// How many dice have been drawn from this bag.
    ///
    /// TODO (Step 6):  anybody can set this to anything.  It is supposed to be
    /// a fact about what happened.
    /// </summary>
    public int DrawnCount { get; set; }

    private readonly Random _random;

    /// <summary>Builds a bag.  A seed makes a run repeatable, which a test needs.</summary>
    public DiceBag(int seed = 20260906) => _random = new Random(seed);

    /// <summary>A starter bag:  the six standard polyhedrals.</summary>
    public static DiceBag Standard(int seed = 20260906) => new(seed)
    {
        Name = "standard set",
        Capacity = 6,
        Contents = [Dice.D4, Dice.D6, Dice.D8, Dice.D10, Dice.D12, Dice.D20]
    };

    /// <summary>
    /// Puts a die in the bag, if the bag has room.
    ///
    /// This method is careful.  It checks the capacity, it refuses politely,
    /// and it is the only place in the file that knows what the rule is.
    /// Scene 2 walks straight past it.
    /// </summary>
    public bool Add(Dice dice)
    {
        if (Contents.Count >= Capacity) return false;
        Contents.Add(dice);
        return true;
    }

    /// <summary>
    /// Draws a die from the bag.
    ///
    /// TODO (Step 3):  read this against the word "draws" and say what is
    /// missing.  Then run Scene 3 and count how many different dice come out.
    /// </summary>
    public Dice Draw()
    {
        DrawnCount++;
        return Contents[0];
    }

    /// <summary>
    /// Shuffles the bag.
    ///
    /// TODO (Step 5):  this one is a good bug.  It runs, it does real work,
    /// and Scene 5 will show you that the bag is unchanged afterwards.  Find
    /// out why before you fix it.
    /// </summary>
    public List<Dice> Shuffle()
    {
        var shuffled = new List<Dice>(Contents);
        for (var i = shuffled.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }
        return shuffled;
    }

    /// <summary>How many dice are in the bag right now.</summary>
    public int Count => Contents.Count;

    public override string ToString() => $"{Name} ({Count}/{Capacity})";
}
