using The_World.GameData.GameMechanics;

namespace TheWorld.Tests;

public class DiceTests
{
    [Fact]
    public void Roll_StaysWithinBounds()
    {
        var rng = new Random(1);
        var dice = new Dice(2, 6, 3);
        for (var i = 0; i < 1000; i++)
        {
            var roll = dice.Roll(rng);
            Assert.InRange(roll, dice.Minimum, dice.Maximum);
        }
    }

    [Fact]
    public void MinimumAndMaximum_AreComputedFromShape()
    {
        var dice = new Dice(3, 8, 2);
        Assert.Equal(5, dice.Minimum);   // 3 ones + 2
        Assert.Equal(26, dice.Maximum);  // 3 eights + 2
    }

    [Theory]
    [InlineData("d20", 1, 20, 0)]
    [InlineData("2d6", 2, 6, 0)]
    [InlineData("3d8+2", 3, 8, 2)]
    [InlineData("1d4-1", 1, 4, -1)]
    [InlineData("  2D10+5 ", 2, 10, 5)]
    public void Parse_ReadsStandardNotation(string notation, int count, int sides, int modifier)
    {
        var dice = Dice.Parse(notation);
        Assert.Equal(count, dice.Count);
        Assert.Equal(sides, dice.Sides);
        Assert.Equal(modifier, dice.Modifier);
    }

    [Theory]
    [InlineData("")]
    [InlineData("banana")]
    [InlineData("2x6")]
    [InlineData("0d6")]
    [InlineData("2d1")]
    [InlineData("d6+")]
    public void TryParse_RejectsNonsense(string notation)
    {
        Assert.False(Dice.TryParse(notation, out _));
    }

    [Fact]
    public void Parse_Nonsense_Throws()
    {
        Assert.Throws<FormatException>(() => Dice.Parse("elephant"));
    }

    [Theory]
    [InlineData(1, 6, 0, "1d6")]
    [InlineData(2, 8, 3, "2d8+3")]
    [InlineData(1, 4, -1, "1d4-1")]
    public void ToString_UsesStandardNotation(int count, int sides, int modifier, string expected)
    {
        Assert.Equal(expected, new Dice(count, sides, modifier).ToString());
    }

    [Fact]
    public void RollDetailed_ReportsCriticals_OnSingleDie()
    {
        // A d1-like die isn't allowed, so walk seeds until we see both extremes.
        var sawCrit = false;
        var sawFumble = false;
        var rng = new Random(99);
        for (var i = 0; i < 500 && !(sawCrit && sawFumble); i++)
        {
            var result = Dice.D20.RollDetailed(rng);
            Assert.Equal(result.Rolls.Sum(), result.Total);
            if (result.Rolls[0] == 20) { Assert.True(result.IsCriticalSuccess); sawCrit = true; }
            if (result.Rolls[0] == 1) { Assert.True(result.IsCriticalFailure); sawFumble = true; }
        }
        Assert.True(sawCrit && sawFumble, "expected both a nat 20 and a nat 1 in 500 rolls");
    }

    [Fact]
    public void RollDetailed_MultipleDice_NeverCritical()
    {
        var rng = new Random(5);
        for (var i = 0; i < 100; i++)
        {
            var result = new Dice(2, 6).RollDetailed(rng);
            Assert.False(result.IsCriticalSuccess);
            Assert.False(result.IsCriticalFailure);
        }
    }

    [Fact]
    public void WeightedDice_FavorTheirFace()
    {
        var rng = new Random(42);
        var fair = new Dice(2, 6);
        var loaded = new WeightedDice(2, 6, FavoredFace: 6);

        var fairTotal = 0.0;
        var loadedTotal = 0.0;
        const int trials = 4000;
        for (var i = 0; i < trials; i++)
        {
            fairTotal += fair.Roll(rng);
            var roll = loaded.Roll(rng);
            Assert.InRange(roll, 2, 12);
            loadedTotal += roll;
        }

        // Fair 2d6 averages 7; the loaded dice should sit clearly above it.
        Assert.True(loadedTotal / trials > fairTotal / trials + 0.8,
            $"loaded average {loadedTotal / trials:0.00} should exceed fair {fairTotal / trials:0.00}");
    }

    [Fact]
    public void WeightedDice_RollsPolymorphically()
    {
        // A WeightedDice held as a Dice must still cheat - Roll is virtual.
        Dice dice = new WeightedDice(1, 6, FavoredFace: 6);
        var rng = new Random(7);
        var total = 0;
        for (var i = 0; i < 2000; i++)
            total += dice.Roll(rng);
        Assert.True(total / 2000.0 > 3.9, $"average {total / 2000.0:0.00} should be biased above fair 3.5");
    }

    [Fact]
    public void DiceCup_SumsAllDice()
    {
        var cup = new DiceCup([new Dice(1, 6), new Dice(1, 8), new Dice(1, 4, 2)]);
        var rng = new Random(3);
        for (var i = 0; i < 200; i++)
            Assert.InRange(cup.RollAll(rng), 5, 20);
    }
}
