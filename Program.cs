// See https://aka.ms/new-console-template for more information

using The_World.GameData;
using The_World.GameData.GameMechanics;

Console.WriteLine("Enter player name:");
var player = Player.CreateNewPlayer(Console.ReadLine()!, "");

var currentArea = WorldBuilder.BuildWorld();

Console.WriteLine($"Welcome, {player.Name}, to The World!");
Console.WriteLine($"You find yourself in the {currentArea.Name}.");
Console.WriteLine(currentArea.Description);

bool keepPlaying = true;
while (keepPlaying)
{
    Console.Write(">> ");
    var input = Console.ReadLine()?.Trim().ToLower();
    if (string.IsNullOrWhiteSpace(input))
        continue;
    var commandParts = input.Split(' ', 2);
    var command = commandParts[0];
    var argument = commandParts.Length > 1 ? commandParts[1] : "";
    /* Process commands
     * TODO: Expand command processing
     * For now, we only handle "look" and "quit"
     * "look" can be used as "look" or "look [target]"
     * e.g., "look sword" to look at a specific item
     * or "look" to look around the area
     * You can add more commands like "get", "attack", etc.
     * we also need:
     * "go" to move between areas
     * "help" to list commands
     * "inventory" to show player inventory (once that is implemented!)
     *
     * This is all very basic command parsing.
     * TODO: Implement a more robust command parser as a separate class.
     * Code should eventually be simply "ProcessCommand(input);" or similar.
     */
    switch (command)
    {
        case "look":
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine(currentArea.LookAround());
            }
            else
            {
                Console.WriteLine(currentArea.LookAt(argument));
            }
            break;
        case "quit":
            keepPlaying = false;
            Console.WriteLine("Thanks for playing!");
            break;
        default:
            Console.WriteLine($"Unknown command: {command}");
            break;
    }
}

// TODO: Implement game loop, movement between areas, interaction with items and creatures, etc.
// This is just a starting point!
// TODO: Consider implementing a Command Pattern for better command handling.
// TODO: Consider implementing an Event System for game events (e.g., player leveling up, creature encounters, etc.).
// TODO: Consider implementing a Save/Load system to persist game state.
// TODO: Consider implementing Unit Tests for game mechanics and logic.
// TODO: Consider implementing Logging for debugging and tracking game events.
// TODO: Consider implementing Different Kinds of Creatures and Items with unique behaviors and properties.
//       NPCs, Enemies, Quest Items, etc.
// TODO: Consider implementing Combat Mechanics for player and creature interactions.
//       Research - Turn-Based Combat, and State Machines for managing combat states.