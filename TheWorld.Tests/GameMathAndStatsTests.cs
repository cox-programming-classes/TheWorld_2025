using The_World.GameData;
using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

public class GameMathTests
{
    [Theory]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(12, 1)]
    [InlineData(14, 2)]
    [InlineData(20, 5)]
    [InlineData(8, -1)]
    [InlineData(7, -2)]
    [InlineData(3, -4)]
    public void AbilityModifier_MatchesTabletopTable(int score, int expected)
    {
        Assert.Equal(expected, GameMath.AbilityModifier(score));
    }

    [Fact]
    public void ExperienceForLevel_IsMonotonicallyIncreasing()
    {
        for (var level = 1; level < 100; level++)
            Assert.True(GameMath.ExperienceForLevel(level + 1) > GameMath.ExperienceForLevel(level));
    }

    [Fact]
    public void ScaledExperience_PunishesPunchingDown_AndCapsPunchingUp()
    {
        // Same level: full value.
        Assert.Equal(20, GameMath.ScaledExperience(20, 5, 5));
        // Far below the player: floors at 25%.
        Assert.Equal(5, GameMath.ScaledExperience(20, 1, 20));
        // Far above: caps at 200%.
        Assert.Equal(40, GameMath.ScaledExperience(20, 30, 1));
    }

    [Fact]
    public void CarryCapacity_ScalesWithStrength()
    {
        Assert.Equal(50, GameMath.CarryCapacity(10));
        Assert.Equal(90, GameMath.CarryCapacity(18));
    }

    [Fact]
    public void Check_AlwaysPassesWhenModifierDwarfsDifficulty()
    {
        var rng = new Random(11);
        for (var i = 0; i < 100; i++)
            Assert.True(GameMath.Check(abilityScore: 30, difficulty: 5, rng)); // d20+10 vs 5: min 11
    }

    [Fact]
    public void Check_AlwaysFailsWhenImpossible()
    {
        var rng = new Random(11);
        for (var i = 0; i < 100; i++)
            Assert.False(GameMath.Check(abilityScore: 1, difficulty: 30, rng)); // d20-5 vs 30: max 15
    }
}

public class StatChartTests
{
    [Fact]
    public void NewChart_StartsAtFullHealthAndMana()
    {
        var stats = new StatChart(30, 12, 14, 12, 8);
        Assert.Equal(30, stats.Health);
        Assert.Equal(12, stats.Mana);
        Assert.True(stats.IsAlive);
    }

    [Fact]
    public void TakeDamage_ClampsAtZero_AndReportsActualDamage()
    {
        var stats = new StatChart(20, 0);
        Assert.Equal(15, stats.TakeDamage(15));
        Assert.Equal(5, stats.Health);
        Assert.Equal(5, stats.TakeDamage(50)); // only 5 left to lose
        Assert.Equal(0, stats.Health);
        Assert.False(stats.IsAlive);
    }

    [Fact]
    public void Heal_ClampsAtMax_AndIgnoresNegatives()
    {
        var stats = new StatChart(20, 0);
        stats.TakeDamage(10);
        Assert.Equal(0, stats.Heal(-5));
        Assert.Equal(10, stats.Heal(999));
        Assert.Equal(20, stats.Health);
    }

    [Fact]
    public void SpendMana_RefusesWhatItCannotAfford()
    {
        var stats = new StatChart(10, 8);
        Assert.True(stats.SpendMana(5));
        Assert.Equal(3, stats.Mana);
        Assert.False(stats.SpendMana(4));
        Assert.Equal(3, stats.Mana); // unchanged after refusal
    }

    [Fact]
    public void Improve_RaisesMaximums_AndCurrentAlongside()
    {
        var stats = new StatChart(20, 10, 14, 12, 10);
        stats.TakeDamage(5);
        stats.Improve(health: 8, mana: 2, strength: 1);
        Assert.Equal(28, stats.MaxHealth);
        Assert.Equal(23, stats.Health); // 15 + 8
        Assert.Equal(15, stats.Strength);
    }

    [Fact]
    public void AbilityModifiers_DeriveFromScores()
    {
        var stats = new StatChart(10, 10, Strength: 16, Dexterity: 8, Intelligence: 12);
        Assert.Equal(3, stats.StrengthModifier);
        Assert.Equal(-1, stats.DexterityModifier);
        Assert.Equal(1, stats.IntelligenceModifier);
    }

    [Fact]
    public void FullRestore_TopsEverythingUp()
    {
        var stats = new StatChart(25, 15);
        stats.TakeDamage(20);
        stats.SpendMana(10);
        stats.FullRestore();
        Assert.Equal(25, stats.Health);
        Assert.Equal(15, stats.Mana);
    }
}
