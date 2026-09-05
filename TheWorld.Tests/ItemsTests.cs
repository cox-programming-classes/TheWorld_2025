using The_World.GameData;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace TheWorld.Tests;

public class InventoryTests
{
    private static Inventory SmallPack(double capacity = 10) => new(() => capacity);

    [Fact]
    public void TryAdd_RespectsWeightLimit()
    {
        var pack = SmallPack(10);
        Assert.True(pack.TryAdd(new Item("Brick", "heavy", 6)));
        Assert.False(pack.TryAdd(new Item("Anvil", "heavier", 5))); // 11 > 10
        Assert.True(pack.TryAdd(new Item("Feather", "light", 4)));  // exactly 10
        Assert.Equal(10, pack.TotalWeight);
    }

    [Fact]
    public void Find_MatchesExactThenPrefixThenSubstring()
    {
        var pack = SmallPack(100);
        var potion = new Item("Healing Potion", "", 1);
        var greater = new Item("Greater Healing Potion", "", 1);
        pack.TryAdd(greater);
        pack.TryAdd(potion);

        Assert.Same(potion, pack.Find("healing potion")); // exact beats substring
        Assert.Same(greater, pack.Find("great"));          // prefix
        Assert.Same(greater, pack.Find("ter heal"));       // substring
        Assert.Null(pack.Find("sword"));
        Assert.Null(pack.Find("  "));
    }

    [Fact]
    public void Remove_TakesOutTheItem()
    {
        var pack = SmallPack();
        var item = new Item("Rock", "", 1);
        pack.TryAdd(item);
        Assert.True(pack.Remove(item));
        Assert.Equal(0, pack.Count);
        Assert.False(pack.Remove(item));
    }
}

public class EquipmentTests
{
    [Fact]
    public void Equip_SwapsAndReturnsDisplacedItem()
    {
        var equipment = new Equipment();
        Assert.Null(equipment.Equip(ItemFactory.Shortsword()));
        var displaced = equipment.Equip(ItemFactory.IronSword());
        Assert.NotNull(displaced);
        Assert.Equal("Shortsword", displaced!.Name);
        Assert.Equal("Iron Sword", equipment.Weapon!.Name);
    }

    [Fact]
    public void WeaponAndArmor_OccupySeparateSlots()
    {
        var equipment = new Equipment();
        equipment.Equip(ItemFactory.Dagger());
        Assert.Null(equipment.Equip(ItemFactory.LeatherArmor())); // doesn't displace the dagger
        Assert.NotNull(equipment.Weapon);
        Assert.NotNull(equipment.Armor);
        Assert.Equal(2, equipment.DefenseBonus);
        Assert.True(equipment.WeaponIsFinesse);
    }

    [Fact]
    public void Unequip_EmptiesTheSlot()
    {
        var equipment = new Equipment();
        equipment.Equip(ItemFactory.Shortsword());
        var removed = equipment.Unequip("weapon");
        Assert.Equal("Shortsword", removed!.Name);
        Assert.Null(equipment.Weapon);
        Assert.Equal(Equipment.UnarmedDice, equipment.DamageDice);
        Assert.Null(equipment.Unequip("weapon")); // already empty
        Assert.Null(equipment.Unequip("hat"));    // no such slot
    }

    [Fact]
    public void Equip_RefusesNonEquippables()
    {
        var equipment = new Equipment();
        Assert.False(Equipment.IsEquippable(ItemFactory.HealingPotion()));
        Assert.Throws<ArgumentException>(() => equipment.Equip(ItemFactory.HealingPotion()));
    }
}

public class ConsumableTests
{
    [Fact]
    public void HealingConsumable_RestoresHealth()
    {
        var stats = new StatChart(30, 10);
        stats.TakeDamage(20);
        var potion = ItemFactory.HealingPotion();
        var message = potion.Consume(stats, new Random(1));
        Assert.True(stats.Health > 10);
        Assert.Contains("health", message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ManaConsumable_RestoresMana()
    {
        var stats = new StatChart(30, 20);
        stats.SpendMana(15);
        ItemFactory.ManaPotion().Consume(stats, new Random(1));
        Assert.True(stats.Mana > 5);
    }
}

public class ItemFactoryTests
{
    [Fact]
    public void EveryRegisteredKey_CreatesAnItem()
    {
        foreach (var key in ItemFactory.Keys)
        {
            var item = ItemFactory.CreateByKey(key);
            Assert.NotNull(item);
            Assert.False(string.IsNullOrWhiteSpace(item.Name));
        }
    }

    [Fact]
    public void EveryRegisteredItem_HasAsciiArt_AndRendersItWhenInspected()
    {
        foreach (var key in ItemFactory.Keys)
        {
            var item = ItemFactory.CreateByKey(key);

            Assert.False(string.IsNullOrWhiteSpace(item.Art), $"{item.Name} has no ASCII art");
            Assert.Contains(item.Art.TrimEnd(), item.Look());
        }
    }

    [Fact]
    public void CreateByKey_UnknownKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => ItemFactory.CreateByKey("excalibur"));
    }

    [Fact]
    public void StartingEquipmentKeys_OfEveryClass_Exist()
    {
        foreach (var playerClass in PlayerClass.All)
        {
            Assert.IsType<Weapon>(ItemFactory.CreateByKey(playerClass.StartingWeaponKey));
            Assert.IsType<Armor>(ItemFactory.CreateByKey(playerClass.StartingArmorKey));
        }
    }

    [Fact]
    public void QuestItems_AreWorthless_SoTheyCannotBeSold()
    {
        Assert.Equal(0, ItemFactory.BarrowKey().Value);
        Assert.Equal(0, ItemFactory.StolenGoods().Value);
    }
}
