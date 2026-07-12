using The_World.GameData.GameMechanics;

namespace The_World.GameData.Abilities;

/// <summary>
/// What an ability does when it goes off.
/// </summary>
public enum AbilityKind
{
    Damage,
    Heal
}

/// <summary>
/// Which ability score powers this ability.
/// </summary>
public enum AbilityScaling
{
    None,
    Strength,
    Dexterity,
    Intelligence
}

/// <summary>
/// A special move or spell: costs mana, rolls dice, does something dramatic.
/// </summary>
/// <param name="Name">What you shout when using it.</param>
/// <param name="Description">What it does, for the ability list.</param>
/// <param name="ManaCost">Mana spent per use.</param>
/// <param name="Power">Dice rolled for the effect's strength.</param>
/// <param name="Kind">Damage or healing.</param>
/// <param name="Scaling">Which ability modifier is added to the roll.</param>
/// <param name="FlavorText">Printed when the ability is used. {0} = target name.</param>
public record Ability(
    string Name,
    string Description,
    int ManaCost,
    Dice Power,
    AbilityKind Kind,
    AbilityScaling Scaling,
    string FlavorText)
{
    /// <summary>
    /// Roll this ability's power for the given caster (dice + scaling modifier).
    /// Always at least 1 - even a weak spell does *something*.
    /// </summary>
    public int RollPower(StatChart caster, Random? rng = null)
    {
        var modifier = Scaling switch
        {
            AbilityScaling.Strength => caster.StrengthModifier,
            AbilityScaling.Dexterity => caster.DexterityModifier,
            AbilityScaling.Intelligence => caster.IntelligenceModifier,
            _ => 0
        };
        return Math.Max(1, Power.Roll(rng) + modifier);
    }

    public string Summary()
        => $"{Name,-18} {ManaCost,3} MP  {Power,-6}  {Description}";
}
