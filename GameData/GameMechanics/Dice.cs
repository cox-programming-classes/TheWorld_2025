namespace The_World.GameData.GameMechanics;



/// <summary>
/// Common Dice types for easy access.
/// TODO: Expand this to include more complex dice rolls (e.g., "2d6+3") if needed.
/// var d6p3 = Dice.D6 with { Modifier = 3 };
/// var roll = d6p3.Roll();
/// </summary>
public record Dice(int Count = 1, int Sides = 6, int Modifier = 0)
{
    // Common Dice types TODO: Expand as needed
    public static readonly Dice Coin = new(Sides: 2);
    public static readonly Dice D6 = new();
    
    
    public int Roll() => Random.Shared.Next(1, Sides + 1) + Modifier;
    
    // TODO: Implement more complex roll methods if needed (e.g., rolling multiple dice and summing results).
    
}
