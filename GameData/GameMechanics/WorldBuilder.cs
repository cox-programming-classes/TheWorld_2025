using The_World.GameData.Areas;
using The_World.GameData.Creatures;

namespace The_World.GameData.GameMechanics;

public class WorldBuilder
{
    /// <summary>
    /// Initialize the WHOLE WORLD here.
    /// </summary>
    /// <returns></returns>
    public static Area BuildWorld()
    {
        var startingArea = AreaBuilder
            .FromName("Dark Forest")
            .WithDescription("A gloomy forest filled with towering trees and eerie sounds.")
            .WithCreature(
                "goblin_1",
                BuildGoblinArchetype())
            .WithCreature(
                "goblin_2",
                BuildGoblinArchetype("Goblin Scout", "A nimble goblin with keen eyes, always on the lookout for intruders."))
            .WithCreature(
                "boss_goblin",
                BuildGoblinArchetype("Goblin Warrior", "A fierce goblin clad in makeshift armor, wielding a crude weapon.", 2))
            .WithItem("rusty_sword", 
                new("Rusty Sword", "An old and worn sword, still sharp enough to be dangerous.", 3.5))
            .Build();
        
        
        return startingArea;
    }
    
    /// <summary>
    /// Helper method to build a Goblin creature archetype.
    /// You might use this in multiple areas.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    private static Creature BuildGoblinArchetype(
        string name = "Goblin", 
        string description = "A small, green humanoid creature with sharp teeth and a mischievous grin.",
        int level = 1)
    {
        return Creature.CreateNewCreature(
            name,
            description,
            new(10+(2*level), 0), // TODO: Stats should probably scale with level better
            level,
            5+(double.Exp(level/100.0)*10)); // TODO: XP scales with level this math sucks
    }
}