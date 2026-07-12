using The_World.GameData.GameMechanics;

namespace The_World.GameData.Items;

/// <summary>
/// What a character currently has equipped: one weapon slot, one armor slot.
/// Equipping into an occupied slot returns the old item so the caller can
/// put it back in the inventory.
/// </summary>
public class Equipment
{
    /// <summary>Bare knuckles. Better than nothing. Barely.</summary>
    public static readonly Dice UnarmedDice = new(1, 3);

    public Weapon? Weapon { get; private set; }
    public Armor? Armor { get; private set; }

    public int AttackBonus => Weapon?.AttackBonus ?? 0;
    public int DefenseBonus => Armor?.DefenseBonus ?? 0;
    public Dice DamageDice => Weapon?.DamageDice ?? UnarmedDice;
    public bool WeaponIsFinesse => Weapon?.Finesse ?? false;

    /// <summary>
    /// Equip a weapon or armor piece.
    /// </summary>
    /// <returns>The item displaced from the slot, if any.</returns>
    public Item? Equip(Item item)
    {
        switch (item)
        {
            case Weapon w:
                var oldWeapon = Weapon;
                Weapon = w;
                return oldWeapon;
            case Armor a:
                var oldArmor = Armor;
                Armor = a;
                return oldArmor;
            default:
                throw new ArgumentException($"{item.Name} cannot be equipped.", nameof(item));
        }
    }

    // (Qualified names: the Weapon/Armor *properties* above would otherwise
    // shadow the type names inside this class.)
    public static bool IsEquippable(Item item) => item is Items.Weapon or Items.Armor;

    /// <summary>
    /// Empty a slot ("weapon" or "armor").
    /// </summary>
    /// <returns>The item removed, or null if the slot was empty/unknown.</returns>
    public Item? Unequip(string slot)
    {
        switch (slot.Trim().ToLowerInvariant())
        {
            case "weapon":
                var w = Weapon;
                Weapon = null;
                return w;
            case "armor" or "armour":
                var a = Armor;
                Armor = null;
                return a;
            default:
                return null;
        }
    }

    public string Describe() => $"""
        Weapon: {(Weapon is null ? "(none - fists, 1d3)" : $"{Weapon.Name} ({Weapon.DamageDice}{(Weapon.Finesse ? ", finesse" : "")})")}
        Armor:  {(Armor is null ? "(none)" : $"{Armor.Name} (+{Armor.DefenseBonus} defense)")}
        """;
}
