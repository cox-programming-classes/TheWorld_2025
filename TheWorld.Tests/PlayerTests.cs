using The_World.GameData;
using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

public class PlayerLevelTests
{
    [Fact]
    public void LevelToExperience_RoundTrips()
    {
        PlayerLevel level = 5;
        Assert.Equal(5, level.Value);
        PlayerLevel fromExp = level.Experience;
        Assert.Equal(5, fromExp.Value);
    }

    [Fact]
    public void Level_IsClampedToValidRange()
    {
        Assert.Equal(1, new PlayerLevel(-3).Value);
        Assert.Equal(100, new PlayerLevel(4000).Value);
        Assert.Equal(0, new PlayerLevel(1, -50).Experience);
    }

    [Fact]
    public void AddingExperience_EventuallyLevelsUp()
    {
        PlayerLevel level = 1;
        var withMore = level + 100.0;
        Assert.True(withMore.Value > 1);
        Assert.Equal(level.Experience + 100.0, withMore.Experience);
    }
}

public class PlayerTests
{
    private static Player MakeWarrior(int str = 14, int dex = 12, int intel = 10) =>
        Player.CreateNewPlayer("Testa", PlayerClass.Warrior, new StatChart(30, 10, str, dex, intel));

    [Fact]
    public void CreateNewPlayer_DefaultsBlankNames()
    {
        var player = Player.CreateNewPlayer("   ", PlayerClass.Rogue, new StatChart(20, 10));
        Assert.Equal("Unknown Hero", player.Name);
        Assert.Equal(PlayerClass.Rogue, player.Class);
        Assert.Equal(1, player.Level.Value);
    }

    [Fact]
    public void AddExperience_AcrossThreshold_RaisesEventAndImprovesStats()
    {
        var player = MakeWarrior();
        var oldMaxHealth = player.Stats.MaxHealth;
        var oldStrength = player.Stats.Strength;
        (int oldLvl, int newLvl)? observed = null;
        player.LeveledUp += (_, o, n) => observed = (o, n);

        player.AddExperience(100); // plenty for level 1 -> 2+

        Assert.NotNull(observed);
        Assert.Equal(1, observed.Value.oldLvl);
        Assert.True(observed.Value.newLvl > 1);
        var levelsGained = observed.Value.newLvl - observed.Value.oldLvl;
        Assert.Equal(oldMaxHealth + PlayerClass.Warrior.HealthPerLevel * levelsGained, player.Stats.MaxHealth);
        Assert.Equal(oldStrength + levelsGained, player.Stats.Strength); // warrior primary stat
        Assert.Equal(player.Stats.MaxHealth, player.Stats.Health); // level up fully restores
    }

    [Fact]
    public void AddExperience_BelowThreshold_DoesNotFireEvent()
    {
        var player = MakeWarrior();
        var fired = false;
        player.LeveledUp += (_, _, _) => fired = true;
        player.AddExperience(0.5);
        Assert.False(fired);
    }

    [Fact]
    public void Gold_SpendRefusesOverdraft()
    {
        var player = MakeWarrior();
        player.AddGold(30);
        Assert.False(player.SpendGold(31));
        Assert.Equal(30, player.Gold);
        Assert.True(player.SpendGold(30));
        Assert.Equal(0, player.Gold);
        Assert.False(player.SpendGold(-5));
    }

    [Fact]
    public void AttackBonus_UsesFinesse_WhenDexIsBetter()
    {
        var player = Player.CreateNewPlayer("Nimble", PlayerClass.Rogue,
            new StatChart(20, 10, Strength: 10, Dexterity: 18, Intelligence: 10));

        // Unarmed: strength modifier (0).
        Assert.Equal(0, player.AttackBonus);

        player.Equipment.Equip(ItemFactory.Dagger()); // finesse
        Assert.Equal(4, player.AttackBonus); // DEX +4 beats STR +0

        player.Equipment.Equip(ItemFactory.IronSword()); // not finesse
        Assert.Equal(0, player.AttackBonus); // back to STR
    }

    [Fact]
    public void DefenseValue_CountsDexAndArmor()
    {
        var player = MakeWarrior(dex: 14);
        Assert.Equal(12, player.DefenseValue); // 10 + 2
        player.Equipment.Equip(ItemFactory.ChainMail());
        Assert.Equal(16, player.DefenseValue); // 10 + 2 + 4
    }

    [Fact]
    public void InventoryCapacity_TracksStrength()
    {
        var player = MakeWarrior(str: 10);
        Assert.Equal(50, player.Inventory.Capacity);
        player.Stats.Improve(strength: 4);
        Assert.Equal(70, player.Inventory.Capacity); // capacity delegate sees new STR
    }
}
