using The_World.GameData.Items;

namespace The_World.GameData.GameMechanics;

/// <summary>
/// Creates common Item archetypes for reuse - the counterpart to
/// CreatureFactory. Every item can also be created by string key
/// (used by dialogue effects and starting equipment).
/// </summary>
public static class ItemFactory
{
    // --- Weapons ------------------------------------------------------------

    public static Weapon RustySword() => new(
        "Rusty Sword", "An old and worn sword, still sharp enough to be dangerous.",
        3.5, 5, new Dice(1, 6));

    public static Weapon Dagger() => new(
        "Dagger", "Short, wicked, and easy to slip between ribs.",
        1.0, 8, new Dice(1, 4), Finesse: true);

    public static Weapon Shortsword() => new(
        "Shortsword", "A dependable soldier's blade. Nothing fancy, everything functional.",
        3.0, 12, new Dice(1, 6));

    public static Weapon GnarledStaff() => new(
        "Gnarled Staff", "A walking stick that has clearly walked through some things.",
        4.0, 10, new Dice(1, 6));

    public static Weapon IronSword() => new(
        "Iron Sword", "Well-balanced and keen-edged. A proper weapon at last.",
        4.0, 35, new Dice(1, 8));

    public static Weapon RunedWarstaff() => new(
        "Runed Warstaff", "Ancient sigils crawl along its length, humming faintly with power.",
        4.0, 80, new Dice(1, 8), AttackBonus: 1);

    public static Weapon SteelGreatsword() => new(
        "Steel Greatsword", "Six feet of gleaming persuasion. Requires commitment to swing.",
        9.0, 90, new Dice(2, 6));

    // --- Armor --------------------------------------------------------------

    public static Armor ApprenticeRobes() => new(
        "Apprentice Robes", "Scratchy wool robes. The hood makes you look mysterious, at least.",
        2.0, 8, DefenseBonus: 1);

    public static Armor LeatherArmor() => new(
        "Leather Armor", "Boiled leather, scuffed but sturdy.",
        8.0, 15, DefenseBonus: 2);

    public static Armor ChainMail() => new(
        "Chain Mail", "Interlocking steel rings. Heavy, jingly, and very reassuring.",
        20.0, 60, DefenseBonus: 4);

    public static Armor PlateArmor() => new(
        "Plate Armor", "A walking fortress. Turns sword blows into dents and curses.",
        35.0, 150, DefenseBonus: 6);

    // --- Consumables ----------------------------------------------------------

    public static Consumable HealingHerb() => new(
        "Healing Herb", "A small herb known for its medicinal properties.",
        0.2, 4, ConsumableKind.Healing, new Dice(1, 8));

    public static Consumable HealingPotion() => new(
        "Healing Potion", "A ruby-red draught that tastes of cherries and regret.",
        0.5, 15, ConsumableKind.Healing, new Dice(2, 8, 2));

    public static Consumable GreaterHealingPotion() => new(
        "Greater Healing Potion", "The good stuff. Practically glows.",
        0.5, 40, ConsumableKind.Healing, new Dice(4, 8, 4));

    public static Consumable ManaPotion() => new(
        "Mana Potion", "A swirling blue liquid that fizzes on the tongue.",
        0.5, 15, ConsumableKind.ManaRestore, new Dice(2, 8, 2));

    public static Consumable MoonpetalBlossom() => new(
        "Moonpetal Blossom", "A silver flower that blooms only for those who truly look.",
        0.1, 30, ConsumableKind.Healing, new Dice(3, 8, 3));

    // --- Quest items -----------------------------------------------------------

    public static QuestItem BarrowKey() => new(
        "Barrow Key", "A heavy iron key, cold to the touch. It smells of old earth and older secrets.");

    public static QuestItem StolenGoods() => new(
        "Stolen Goods", "A merchant's strongbox stamped 'BRAM'S PROVISIONS', pried at but unopened.",
        Weight: 4.0);

    public static QuestItem Phylactery() => new(
        "Malakhar's Phylactery", "A cracked soul-vessel, dark and finally silent. Proof the Lich is no more.");

    // --- Valuables (for looting and selling) -----------------------------------

    public static Item WolfPelt() => new(
        "Wolf Pelt", "Thick grey fur. A furrier would pay well for this.", 3.0, 8);

    public static Item SilverLocket() => new(
        "Silver Locket", "A delicate locket holding a tiny portrait of someone long forgotten.", 0.1, 25);

    public static Item AmberRing() => new(
        "Amber Ring", "A golden ring with a fly trapped in its amber stone. Vintage.", 0.1, 35);

    public static Item AncientTome() => new(
        "Ancient Tome", "A crumbling book of pre-Imperial history. Collectors would duel over it.", 2.0, 60);

    // --- Creation by key --------------------------------------------------------

    private static readonly Dictionary<string, Func<Item>> Registry = new(StringComparer.OrdinalIgnoreCase)
    {
        ["rusty_sword"] = RustySword,
        ["dagger"] = Dagger,
        ["shortsword"] = Shortsword,
        ["gnarled_staff"] = GnarledStaff,
        ["iron_sword"] = IronSword,
        ["runed_warstaff"] = RunedWarstaff,
        ["steel_greatsword"] = SteelGreatsword,
        ["apprentice_robes"] = ApprenticeRobes,
        ["leather_armor"] = LeatherArmor,
        ["chain_mail"] = ChainMail,
        ["plate_armor"] = PlateArmor,
        ["healing_herb"] = HealingHerb,
        ["healing_potion"] = HealingPotion,
        ["greater_healing_potion"] = GreaterHealingPotion,
        ["mana_potion"] = ManaPotion,
        ["moonpetal_blossom"] = MoonpetalBlossom,
        ["barrow_key"] = BarrowKey,
        ["stolen_goods"] = StolenGoods,
        ["phylactery"] = Phylactery,
        ["wolf_pelt"] = WolfPelt,
        ["silver_locket"] = SilverLocket,
        ["amber_ring"] = AmberRing,
        ["ancient_tome"] = AncientTome,
    };

    /// <summary>
    /// Create an item from its registry key (e.g. "healing_potion").
    /// </summary>
    /// <exception cref="ArgumentException">Unknown key.</exception>
    public static Item CreateByKey(string key) =>
        Registry.TryGetValue(key?.Trim() ?? "", out var create)
            ? create()
            : throw new ArgumentException($"No item registered with key '{key}'.", nameof(key));

    /// <summary>All registered keys - used by tests to verify the registry.</summary>
    public static IEnumerable<string> Keys => Registry.Keys;
}
