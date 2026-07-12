using The_World.GameData.GameMechanics;

namespace The_World.GameData.Items;

/// <summary>
/// What a consumable does when you use it.
/// </summary>
public enum ConsumableKind
{
    Healing,
    ManaRestore
}

/// <summary>
/// An item that is used up for an effect: potions, herbs, dubious mushrooms.
/// </summary>
/// <param name="Kind">What the consumable affects.</param>
/// <param name="Potency">Dice rolled to determine how strong the effect is.</param>
public record Consumable(
    string Name,
    string Description,
    double Weight,
    int Value,
    ConsumableKind Kind,
    Dice Potency)
    : Item(Name, Description, Weight, Value)
{
    public override string Look()
        => $"{Name}  [{KindLabel}: {Potency}]  ({Weight:0.#} lbs, {Value} gold){Environment.NewLine}{Description}";

    private string KindLabel => Kind switch
    {
        ConsumableKind.Healing => "Restores health",
        ConsumableKind.ManaRestore => "Restores mana",
        _ => "Unknown effect"
    };

    /// <summary>
    /// Apply this consumable's effect to a target's stats.
    /// </summary>
    /// <returns>A message describing what happened.</returns>
    public string Consume(StatChart target, Random? rng = null)
    {
        var amount = Potency.Roll(rng);
        return Kind switch
        {
            ConsumableKind.Healing =>
                $"You feel warmth spread through you. Restored {target.Heal(amount)} health.",
            ConsumableKind.ManaRestore =>
                $"Your mind sharpens. Restored {target.RestoreMana(amount)} mana.",
            _ => "Nothing happens. How disappointing."
        };
    }
}
