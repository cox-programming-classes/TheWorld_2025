using The_World.GameData.GameMechanics;

namespace The_World.GameData.Items;

/// <summary>
/// An item you hit things with.
/// </summary>
/// <param name="DamageDice">Dice rolled for damage on a hit.</param>
/// <param name="AttackBonus">Bonus added to attack rolls (a finely made blade).</param>
/// <param name="Finesse">
/// A finesse weapon (dagger, rapier) lets an agile wielder attack with
/// Dexterity instead of Strength - whichever is better.
/// </param>
public record Weapon(
    string Name,
    string Description,
    double Weight,
    int Value,
    Dice DamageDice,
    int AttackBonus = 0,
    bool Finesse = false)
    : Item(Name, Description, Weight, Value)
{
    public override string Look()
        => $"{ArtBlock}{Name}  [Weapon: {DamageDice}{(AttackBonus > 0 ? $", +{AttackBonus} to hit" : "")}{(Finesse ? ", finesse" : "")}]  ({Weight:0.#} lbs, {Value} gold){Environment.NewLine}{Description}";
}
