using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

/// <summary>
/// A Random whose "rolls" are scripted, so combat outcomes are exact.
/// Dice.Roll asks for Next(1, sides+1); we return the queued values.
/// </summary>
internal sealed class ScriptedRandom(params int[] values) : Random
{
    private readonly Queue<int> _values = new(values);

    public override int Next(int minValue, int maxValue) =>
        _values.Count > 0 ? Math.Clamp(_values.Dequeue(), minValue, maxValue - 1) : minValue;

    public override int Next(int maxValue) => Next(0, maxValue);
}

public class CombatMathTests
{
    [Fact]
    public void ResolveAttack_Natural20_CritsAndDoublesDamageDice()
    {
        // d20 -> 20, then two damage dice rolls (crit): 6 and 4.
        var rng = new ScriptedRandom(20, 6, 4);
        var outcome = CombatMath.ResolveAttack(
            attackBonus: 2, damageDice: new Dice(1, 8), damageBonus: 3, targetDefense: 30, rng);

        Assert.True(outcome.Hit);       // nat 20 hits even vs defense 30
        Assert.True(outcome.Critical);
        Assert.False(outcome.Fumble);
        Assert.Equal(13, outcome.Damage); // 6 + 3 + 4
    }

    [Fact]
    public void ResolveAttack_Natural1_AlwaysMisses()
    {
        var rng = new ScriptedRandom(1);
        var outcome = CombatMath.ResolveAttack(
            attackBonus: 50, damageDice: new Dice(1, 8), damageBonus: 0, targetDefense: 5, rng);

        Assert.False(outcome.Hit);
        Assert.True(outcome.Fumble);
        Assert.Equal(0, outcome.Damage);
    }

    [Fact]
    public void ResolveAttack_MeetingDefense_Hits()
    {
        // d20 -> 10, +3 = 13 vs defense 13: meets it, beats it. Damage die -> 4.
        var rng = new ScriptedRandom(10, 4);
        var outcome = CombatMath.ResolveAttack(3, new Dice(1, 6), 1, 13, rng);
        Assert.True(outcome.Hit);
        Assert.False(outcome.Critical);
        Assert.Equal(5, outcome.Damage);
    }

    [Fact]
    public void ResolveAttack_UnderDefense_Misses()
    {
        var rng = new ScriptedRandom(10);
        var outcome = CombatMath.ResolveAttack(2, new Dice(1, 6), 0, 13, rng);
        Assert.False(outcome.Hit);
        Assert.Equal(0, outcome.Damage);
    }

    [Fact]
    public void ResolveAttack_DamageFloorsAtOne()
    {
        // Hit with a 1d4-and-huge-negative-bonus: still stings for 1.
        var rng = new ScriptedRandom(15, 1);
        var outcome = CombatMath.ResolveAttack(5, new Dice(1, 4), -10, 10, rng);
        Assert.True(outcome.Hit);
        Assert.Equal(1, outcome.Damage);
    }

    [Fact]
    public void RollInitiative_AddsDexterityModifier()
    {
        var rng = new ScriptedRandom(10);
        Assert.Equal(13, CombatMath.RollInitiative(3, rng));
    }

    [Fact]
    public void ResolveFlee_IsAContest()
    {
        // Runner DEX 30 (+10): d20 roll of 2 gives 12 vs DC 10 + (-2) = 8: escape.
        Assert.True(CombatMath.ResolveFlee(30, -2, new ScriptedRandom(2)));
        // Runner DEX 1 (-5): d20 roll of 19 gives 14 vs DC 10 + 5 = 15: caught.
        Assert.False(CombatMath.ResolveFlee(1, 5, new ScriptedRandom(19)));
    }
}
