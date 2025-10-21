using The_World.GameData.Creatures;

namespace The_World.GameData.GameMechanics;

/// <summary>
/// Creates common Creature archetypes for reuse.
/// This helps avoid duplication of code when creating
/// similar creatures in multiple areas or scenarios.
///
/// TODO: Expand this factory with more creature archetypes as needed.
/// Make sure to keep the methods flexible with parameters for customization.
/// 
/// </summary>
public static class CreatureFactory
{
    /// <summary>
    /// Helper method to build a Goblin creature archetype.
    /// You might use this in multiple areas.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public static Creature BuildGoblinArchetype(
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