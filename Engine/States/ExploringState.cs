using The_World.Engine.Commands;
using The_World.GameData.Abilities;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace The_World.Engine.States;

/// <summary>
/// The default mode of play: wandering areas, picking things up, poking
/// around, chatting, and starting fights you may or may not win.
/// </summary>
public class ExploringState : GameStateBase
{
    public override string Name => "Exploring";

    private static readonly Dictionary<string, string> DirectionShorthand = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = "north", ["s"] = "south", ["e"] = "east", ["w"] = "west",
        ["u"] = "up", ["d"] = "down"
    };

    public ExploringState()
    {
        CommandList.AddRange(
        [
            new DelegateCommand("look", "Look around, or at something specific.", Look, "look [target]", "l", "x", "examine"),
            new DelegateCommand("go", "Travel through an exit (or just type the direction).", Go, "go <exit>", "move", "walk"),
            new DelegateCommand("get", "Pick up an item.", Get, "get <item>", "take", "grab"),
            new DelegateCommand("drop", "Leave an item here.", Drop, "drop <item>"),
            new DelegateCommand("inventory", "See what you're carrying.", ShowInventory, aliases: ["i", "inv", "pack"]),
            new DelegateCommand("equip", "Wield a weapon or wear armor from your pack.", Equip, "equip <item>", "wield", "wear"),
            new DelegateCommand("unequip", "Stow your weapon or armor.", Unequip, "unequip <weapon|armor>", "remove"),
            new DelegateCommand("use", "Use a consumable item.", Use, "use <item>", "drink", "eat"),
            new DelegateCommand("cast", "Use a restorative ability outside of battle.", Cast, "cast <ability>"),
            new DelegateCommand("abilities", "List your special abilities.", ShowAbilities, aliases: ["skills"]),
            new DelegateCommand("attack", "Start a fight. This changes everything.", Attack, "attack <creature>", "fight", "kill", "a"),
            new DelegateCommand("talk", "Speak with someone.", Talk, "talk <person>", "speak", "t"),
            new DelegateCommand("search", "Search the area carefully (Intelligence check).", Search),
            new DelegateCommand("rest", "Rest to recover. Safer in some places than others.", Rest, aliases: ["camp", "sleep"]),
            new DelegateCommand("stats", "Your character sheet.", (ctx, _) => ctx.IO.WriteLine(ctx.Player.CharacterSheet()), aliases: ["status", "character"]),
            new DelegateCommand("journal", "What you've learned and promised.", ShowJournal, aliases: ["quests", "j"]),
        ]);
    }

    public override string GetPrompt(GameContext ctx) => $"[{ctx.CurrentArea.Name}] > ";

    public override void OnEnter(GameContext ctx)
    {
        ctx.IO.WriteLine();
        ctx.IO.WriteLine(ctx.CurrentArea.Look());
        PrintExits(ctx);
    }

    public override void OnResume(GameContext ctx)
    {
        ctx.IO.WriteLine($"You are in the {ctx.CurrentArea.Name}.");
    }

    // --- Movement -----------------------------------------------------------

    /// <summary>
    /// Bare directions work without 'go': "north", "n", "tavern", "deeper".
    /// </summary>
    public override bool TryHandleRaw(GameContext ctx, string input)
    {
        if (input.Contains(' '))
            return false;
        var query = DirectionShorthand.GetValueOrDefault(input, input);
        // Only exact exit-key matches take the shortcut; anything fuzzier
        // must be an explicit 'go' so commands never get shadowed.
        if (!ctx.CurrentArea.ConnectedAreas.Keys.Any(k => k.Equals(query, StringComparison.OrdinalIgnoreCase)))
            return false;
        Go(ctx, query);
        return true;
    }

    private void Go(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Go where? " + ExitsLine(ctx));
            return;
        }

        var query = DirectionShorthand.GetValueOrDefault(argument.Trim(), argument.Trim());
        var match = Finder.Resolve(ctx.CurrentArea.ConnectedAreas, query, a => a.Name);
        if (match is null)
        {
            ctx.IO.WriteLine($"You can't go '{argument}' from here. {ExitsLine(ctx)}");
            return;
        }

        var (exitKey, destination) = match.Value;

        if (ctx.CurrentArea.LockedExits.TryGetValue(exitKey, out var lockInfo))
        {
            if (ctx.Player.Inventory.Find(lockInfo.RequiredItemName) is null)
            {
                ctx.IO.WriteLine(lockInfo.LockedMessage, ConsoleColor.DarkYellow);
                return;
            }
            ctx.CurrentArea.LockedExits.Remove(exitKey);
            ctx.IO.WriteLine($"You produce the {lockInfo.RequiredItemName}. It turns with a sound like a held breath released.", ConsoleColor.Yellow);
        }

        ctx.CurrentArea = destination;
        ctx.IO.WriteLine();
        ctx.IO.WriteLine(destination.Look());
        PrintExits(ctx);

        var hostiles = destination.Creatures.Values.Where(c => c.IsHostile).Select(c => c.Name).ToList();
        if (hostiles.Count > 0)
            ctx.IO.WriteLine($"! Hostile: {string.Join(", ", hostiles)}", ConsoleColor.Red);
    }

    private void PrintExits(GameContext ctx)
    {
        ctx.IO.WriteLine(ExitsLine(ctx), ConsoleColor.DarkGray);
    }

    private string ExitsLine(GameContext ctx) =>
        ctx.CurrentArea.ConnectedAreas.Count == 0
            ? "There are no exits. That seems ominous."
            : "Exits: " + string.Join(", ", ctx.CurrentArea.ConnectedAreas.Select(kv =>
                $"{kv.Key} ({kv.Value.Name}{(ctx.CurrentArea.LockedExits.ContainsKey(kv.Key) ? ", locked" : "")})"));

    // --- Looking ------------------------------------------------------------

    private void Look(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine(ctx.CurrentArea.LookAround());
            return;
        }

        var area = ctx.CurrentArea;
        var itemMatch = Finder.Resolve(area.Items, argument, i => i.Name);
        if (itemMatch is not null)
        {
            ctx.IO.WriteLine(itemMatch.Value.Value.Look());
            return;
        }
        var creatureMatch = Finder.Resolve(area.Creatures, argument, c => c.Name);
        if (creatureMatch is not null)
        {
            ctx.IO.WriteLine(creatureMatch.Value.Value.Look());
            return;
        }
        var carried = ctx.Player.Inventory.Find(argument);
        if (carried is not null)
        {
            ctx.IO.WriteLine(carried.Look());
            return;
        }
        var exitMatch = Finder.Resolve(area.ConnectedAreas, argument, a => a.Name);
        if (exitMatch is not null)
        {
            ctx.IO.WriteLine(exitMatch.Value.Value.Look());
            return;
        }

        ctx.IO.WriteLine($"There is nothing notable about '{argument}' here.");
    }

    // --- Items ----------------------------------------------------------------

    private void Get(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Get what?");
            return;
        }
        var match = Finder.Resolve(ctx.CurrentArea.Items, argument, i => i.Name);
        if (match is null)
        {
            ctx.IO.WriteLine($"There's no '{argument}' here to take.");
            return;
        }
        var (key, item) = match.Value;
        if (!ctx.Player.Inventory.TryAdd(item))
        {
            ctx.IO.WriteLine($"The {item.Name} is too heavy - you're carrying {ctx.Player.Inventory.TotalWeight:0.#} of {ctx.Player.Inventory.Capacity:0.#} lbs.");
            return;
        }
        ctx.CurrentArea.Items.Remove(key);
        ctx.IO.WriteLine($"You take the {item.Name}.");
    }

    private void Drop(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Drop what?");
            return;
        }
        var item = ctx.Player.Inventory.Find(argument);
        if (item is null)
        {
            ctx.IO.WriteLine($"You aren't carrying '{argument}'.");
            return;
        }
        ctx.Player.Inventory.Remove(item);
        ctx.CurrentArea.Items[UniqueKey(ctx.CurrentArea.Items, item.Name)] = item;
        ctx.IO.WriteLine($"You set the {item.Name} down.");
    }

    private void ShowInventory(GameContext ctx, string _)
    {
        ctx.IO.WriteLine($"Gold: {ctx.Player.Gold}", ConsoleColor.Yellow);
        ctx.IO.WriteLine(ctx.Player.Equipment.Describe());
        ctx.IO.WriteLine(ctx.Player.Inventory.Describe());
    }

    private void Equip(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Equip what?");
            return;
        }
        var item = ctx.Player.Inventory.Find(argument);
        if (item is null)
        {
            ctx.IO.WriteLine($"You aren't carrying '{argument}'.");
            return;
        }
        if (!Equipment.IsEquippable(item))
        {
            ctx.IO.WriteLine($"You try wearing the {item.Name}. It's not a good look, and it isn't armor either.");
            return;
        }

        ctx.Player.Inventory.Remove(item);
        var displaced = ctx.Player.Equipment.Equip(item);
        ctx.IO.WriteLine(item switch
        {
            Weapon w => $"You ready the {w.Name} ({w.DamageDice}{(w.Finesse ? ", finesse" : "")}).",
            Armor a => $"You strap on the {a.Name} (+{a.DefenseBonus} defense).",
            _ => $"You equip the {item.Name}."
        });
        if (displaced is not null)
            StowOrDrop(ctx, displaced);
    }

    private void Unequip(GameContext ctx, string argument)
    {
        var removed = ctx.Player.Equipment.Unequip(argument);
        if (removed is null)
        {
            ctx.IO.WriteLine("Unequip what - your 'weapon' or your 'armor'?");
            return;
        }
        ctx.IO.WriteLine($"You stow the {removed.Name}.");
        StowOrDrop(ctx, removed);
    }

    private void StowOrDrop(GameContext ctx, Item item)
    {
        if (ctx.Player.Inventory.TryAdd(item))
            return;
        ctx.CurrentArea.Items[UniqueKey(ctx.CurrentArea.Items, item.Name)] = item;
        ctx.IO.WriteLine($"Your pack is too full for the {item.Name}; it drops at your feet.");
    }

    private void Use(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Use what?");
            return;
        }
        var item = ctx.Player.Inventory.Find(argument);
        if (item is null)
        {
            ctx.IO.WriteLine($"You aren't carrying '{argument}'.");
            return;
        }
        if (item is not Consumable consumable)
        {
            ctx.IO.WriteLine($"You turn the {item.Name} over in your hands. No obvious 'use' presents itself.");
            return;
        }
        ctx.Player.Inventory.Remove(consumable);
        ctx.IO.WriteLine(consumable.Consume(ctx.Player.Stats, ctx.Rng), ConsoleColor.Green);
        ctx.IO.WriteLine($"({ctx.Player.Stats.Health}/{ctx.Player.Stats.MaxHealth} HP, {ctx.Player.Stats.Mana}/{ctx.Player.Stats.MaxMana} MP)");
    }

    // --- Abilities ---------------------------------------------------------------

    private void ShowAbilities(GameContext ctx, string _)
    {
        ctx.IO.WriteLine($"Abilities of a {ctx.Player.Class.Name} (Mana: {ctx.Player.Stats.Mana}/{ctx.Player.Stats.MaxMana}):", ConsoleColor.Cyan);
        foreach (var ability in ctx.Player.Class.Abilities)
            ctx.IO.WriteLine("  " + ability.Summary());
    }

    private void Cast(GameContext ctx, string argument)
    {
        var ability = FindAbility(ctx, argument);
        if (ability is null)
            return;
        if (ability.Kind != AbilityKind.Heal)
        {
            ctx.IO.WriteLine($"{ability.Name} needs a target that deserves it. (Damaging abilities only work in combat.)");
            return;
        }
        if (!ctx.Player.Stats.SpendMana(ability.ManaCost))
        {
            ctx.IO.WriteLine($"Not enough mana ({ctx.Player.Stats.Mana}/{ability.ManaCost} needed).");
            return;
        }
        var amount = ability.RollPower(ctx.Player.Stats, ctx.Rng);
        var healed = ctx.Player.Stats.Heal(amount);
        ctx.IO.WriteLine(string.Format(ability.FlavorText, "you"), ConsoleColor.Green);
        ctx.IO.WriteLine($"Restored {healed} health. ({ctx.Player.Stats.Health}/{ctx.Player.Stats.MaxHealth} HP, {ctx.Player.Stats.Mana} MP left)");
    }

    private Ability? FindAbility(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Cast what? Try 'abilities' to see your options.");
            return null;
        }
        var ability = ctx.Player.Class.Abilities.FirstOrDefault(a =>
                a.Name.Equals(argument.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? ctx.Player.Class.Abilities.FirstOrDefault(a =>
                a.Name.StartsWith(argument.Trim(), StringComparison.OrdinalIgnoreCase));
        if (ability is null)
            ctx.IO.WriteLine($"You don't know any ability called '{argument}'.");
        return ability;
    }

    // --- People and monsters --------------------------------------------------------

    private void Attack(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Attack what?");
            return;
        }
        var match = Finder.Resolve(ctx.CurrentArea.Creatures, argument, c => c.Name);
        if (match is null)
        {
            ctx.IO.WriteLine($"There's no '{argument}' here to fight.");
            return;
        }
        var (key, creature) = match.Value;

        if (creature is Npc npc)
        {
            ctx.IO.WriteLine($"{npc.Name} takes a startled step back. Willowbrook remembers rudeness - find a real enemy.");
            return;
        }
        if (!creature.IsHostile)
        {
            ctx.IO.WriteLine($"The {creature.Name} bolts before you take a second step. You feel vaguely ashamed.");
            ctx.CurrentArea.Creatures.Remove(key);
            return;
        }

        ctx.States.Push(ctx, new CombatState(key, creature));
    }

    private void Talk(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Talk to whom?");
            return;
        }
        var match = Finder.Resolve(ctx.CurrentArea.Creatures, argument, c => c.Name);
        if (match is null)
        {
            ctx.IO.WriteLine($"There's no '{argument}' here to talk to.");
            return;
        }
        var creature = match.Value.Value;
        if (creature is Npc npc)
        {
            ctx.States.Push(ctx, new DialogueState(npc));
            return;
        }
        ctx.IO.WriteLine(creature.IsHostile
            ? $"The {creature.Name} responds with violence in its eyes. Words won't help here."
            : $"The {creature.Name} regards you blankly. Conversation requires two participants.");
    }

    // --- Area actions -------------------------------------------------------------

    private void Search(GameContext ctx, string _)
    {
        var roll = Dice.D20.RollDetailed(ctx.Rng);
        var total = roll.Total + ctx.Player.Stats.IntelligenceModifier;
        ctx.IO.WriteLine($"You search the area... (d20 {roll.Rolls[0]} + INT {ctx.Player.Stats.IntelligenceModifier} = {total})", ConsoleColor.DarkGray);

        if (total < 12)
        {
            ctx.IO.WriteLine("You find dirt, twigs, and a growing sense that you're being watched.");
            return;
        }
        if (ctx.CurrentArea.HiddenItems.Count == 0)
        {
            ctx.IO.WriteLine("You comb the place thoroughly. Whatever was here to find, you've found it.");
            return;
        }
        foreach (var (key, item) in ctx.CurrentArea.HiddenItems.ToList())
        {
            ctx.CurrentArea.HiddenItems.Remove(key);
            ctx.CurrentArea.Items[UniqueKey(ctx.CurrentArea.Items, item.Name)] = item;
            ctx.IO.WriteLine($"Hidden away, you discover: {item.Name}!", ConsoleColor.Yellow);
        }
    }

    private void Rest(GameContext ctx, string _)
    {
        var area = ctx.CurrentArea;
        if (area.Creatures.Values.Any(c => c.IsHostile))
        {
            ctx.IO.WriteLine("Rest? With hostile company? Deal with them first.");
            return;
        }
        if (area.IsSafe)
        {
            ctx.Player.Stats.FullRestore();
            ctx.IO.WriteLine("You rest deeply and wake whole. Health and mana fully restored.", ConsoleColor.Green);
            return;
        }

        var roll = Dice.D20.Roll(ctx.Rng);
        if (roll <= 5)
        {
            var ambusher = Dice.Coin.Roll(ctx.Rng) == 1
                ? CreatureFactory.BuildGoblinArchetype("Prowling Goblin", "A goblin that thought you looked like easy pickings.")
                : CreatureFactory.Wolf("Prowling Wolf");
            var key = UniqueKey(area.Creatures, ambusher.Name);
            area.Creatures[key] = ambusher;
            ctx.IO.WriteLine("You barely close your eyes before something creeps from the shadows!", ConsoleColor.Red);
            ctx.States.Push(ctx, new CombatState(key, ambusher));
            return;
        }

        var hp = ctx.Player.Stats.Heal(ctx.Player.Stats.MaxHealth / 2);
        var mp = ctx.Player.Stats.RestoreMana(ctx.Player.Stats.MaxMana / 2);
        ctx.IO.WriteLine($"You sleep with one eye open. Recovered {hp} health and {mp} mana.", ConsoleColor.Green);
    }

    private void ShowJournal(GameContext ctx, string _)
    {
        if (ctx.Journal.Count == 0)
        {
            ctx.IO.WriteLine("Your journal is empty. Adventures have a way of fixing that.");
            return;
        }
        ctx.IO.WriteLine("--- Journal ---", ConsoleColor.Cyan);
        foreach (var entry in ctx.Journal)
            ctx.IO.WriteLine($"  * {entry}");
    }
}
