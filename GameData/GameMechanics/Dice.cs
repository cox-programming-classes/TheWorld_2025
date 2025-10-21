namespace The_World.GameData.GameMechanics;



/// <summary>
/// Common Dice types for easy access.
/// TODO: Expand this to include more complex dice rolls (e.g., "2d6+3") if needed.
/// var d6p3 = Dice.D6 with { Modifier = 3 };
/// var roll = d6p3.Roll();
///
/// </summary>
public record Dice(int Count = 1, int Sides = 6, int Modifier = 0)
{
    // Common Dice types TODO: Expand as needed
    public static readonly Dice Coin = new(Sides: 2);
    public static readonly Dice D6 = new();

    public int Roll()
    {
        var sum = Modifier;
        for (var i = 0; i < Count; i++) 
            sum += Random.Shared.Next(1, Sides + 1);   
        return sum;
    }
    
    // TODO: Implement more complex roll methods if needed (e.g., rolling multiple dice and summing results).
    
}

// TODO: Implement a cheating WeightedDice class for specific game mechanics.


public record DiceCup(List<Dice> Dice)
{
    public int RollAll()
    {
        int total = 0;
        foreach (var die in Dice)
        {
            total += die.Roll();
        }
        return total;
    }
}
