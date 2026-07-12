using The_World.Engine.Commands;
using The_World.GameData.Abilities;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;
using The_World.GameData.Items;

namespace The_World.Engine.States;

/// <summary>
/// Turn-based combat, pushed on top of Exploring. Initiative is rolled on
/// entry; every turn-consuming player action (attack, cast, use, failed
/// flee) gives the enemy a swing back. Victory pops back to exploring;
/// defeat tears the stack down to Game Over.
/// </summary>
public class CombatState(string creatureKey, Creature enemy) : GameStateBase
{
    private bool _initialized;

    public override string Name => "In Combat";

    public override string GetPrompt(GameContext ctx) =>
        $"[Fighting {enemy.Name} ({enemy.Stats.Health} HP) | You: {ctx.Player.Stats.Health}/{ctx.Player.Stats.MaxHealth} HP, {ctx.Player.Stats.Mana} MP] > ";

    public override void OnEnter(GameContext ctx)
    {
        // Commands built here (not the constructor) so they can close over ctx-heavy helpers.
        if (!_initialized)
        {
            _initialized = true;
            CommandList.AddRange(
            [
                new DelegateCommand("attack", "Swing your weapon.", (c, _) => TakeTurn(c, PlayerAttack), aliases: ["a", "hit", "strike"]),
                new DelegateCommand("cast", "Use an ability (costs mana and your turn).", Cast, "cast <ability>", "c"),
                new DelegateCommand("use", "Drink or eat something from your pack (costs your turn).", Use, "use <item>", "drink"),
                new DelegateCommand("flee", "Try to escape (Dexterity contest - failing costs your turn).", Flee, aliases: ["run", "escape"]),
                new DelegateCommand("look", "Size up your opponent.", (c, _) => c.IO.WriteLine(enemy.Look()), aliases: ["l", "examine"]),
                new DelegateCommand("abilities", "List your abilities.", ShowAbilities, aliases: ["skills"]),
                new DelegateCommand("stats", "Your character sheet.", (c, _) => c.IO.WriteLine(c.Player.CharacterSheet()), aliases: ["status"]),
            ]);
        }

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"*** {enemy.Name} moves to attack! ***", ConsoleColor.Red);
        ctx.IO.WriteLine(enemy.Description);

        var playerInit = CombatMath.RollInitiative(ctx.Player.Stats.DexterityModifier, ctx.Rng);
        var enemyInit = CombatMath.RollInitiative(enemy.Stats.DexterityModifier, ctx.Rng);
        ctx.IO.WriteLine($"Initiative: you {playerInit}, {enemy.Name} {enemyInit}.", ConsoleColor.DarkGray);
        if (enemyInit > playerInit)
        {
            ctx.IO.WriteLine($"The {enemy.Name} is faster!", ConsoleColor.Red);
            EnemyTurn(ctx);
        }
        else
        {
            ctx.IO.WriteLine("You have the initiative. (attack, cast, use, flee...)");
        }
    }

    /// <summary>
    /// Run a turn-consuming player action, then give the enemy its swing
    /// (if it's still standing and the fight is still on).
    /// </summary>
    private void TakeTurn(GameContext ctx, Action<GameContext> playerAction)
    {
        playerAction(ctx);
        if (enemy.Stats.IsAlive && ctx.Player.Stats.IsAlive && ctx.States.Current == this)
            EnemyTurn(ctx);
    }

    // --- Player actions ------------------------------------------------------

    private void PlayerAttack(GameContext ctx)
    {
        var player = ctx.Player;
        var outcome = CombatMath.ResolveAttack(
            player.AttackBonus, player.DamageDice, player.DamageBonus, enemy.DefenseValue, ctx.Rng);

        ctx.IO.WriteLine($"You attack: d20 [{outcome.AttackRoll.Rolls[0]}] + {player.AttackBonus} = {outcome.AttackTotal} vs defense {enemy.DefenseValue}", ConsoleColor.DarkGray);

        if (outcome.Fumble)
        {
            ctx.IO.WriteLine("Fumble! Your weapon twists in your grip and bites nothing but air.", ConsoleColor.Red);
            return;
        }
        if (!outcome.Hit)
        {
            ctx.IO.WriteLine($"The {enemy.Name} evades your strike.");
            return;
        }

        enemy.Stats.TakeDamage(outcome.Damage);
        ctx.IO.WriteLine(outcome.Critical
            ? $"CRITICAL HIT! You deal {outcome.Damage} damage to the {enemy.Name}!"
            : $"You hit the {enemy.Name} for {outcome.Damage} damage.",
            outcome.Critical ? ConsoleColor.Magenta : ConsoleColor.Green);

        if (!enemy.Stats.IsAlive)
            Victory(ctx);
        else
            ctx.IO.WriteLine(enemy.HealthDescription(), ConsoleColor.DarkGray);
    }

    private void Cast(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Cast what? Try 'abilities'.");
            return;
        }
        var ability = ctx.Player.Class.Abilities.FirstOrDefault(ab =>
                ab.Name.Equals(argument.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? ctx.Player.Class.Abilities.FirstOrDefault(ab =>
                ab.Name.StartsWith(argument.Trim(), StringComparison.OrdinalIgnoreCase));
        if (ability is null)
        {
            ctx.IO.WriteLine($"You don't know any ability called '{argument}'.");
            return;
        }
        if (!ctx.Player.Stats.SpendMana(ability.ManaCost))
        {
            ctx.IO.WriteLine($"Not enough mana ({ctx.Player.Stats.Mana} of {ability.ManaCost} needed).");
            return;
        }

        TakeTurn(ctx, c =>
        {
            var power = ability.RollPower(c.Player.Stats, c.Rng);
            if (ability.Kind == AbilityKind.Heal)
            {
                var healed = c.Player.Stats.Heal(power);
                c.IO.WriteLine(string.Format(ability.FlavorText, enemy.Name), ConsoleColor.Green);
                c.IO.WriteLine($"You recover {healed} health.", ConsoleColor.Green);
                return;
            }

            c.IO.WriteLine(string.Format(ability.FlavorText, enemy.Name), ConsoleColor.Cyan);
            enemy.Stats.TakeDamage(power);
            c.IO.WriteLine($"The {ability.Name} hits the {enemy.Name} for {power} damage!", ConsoleColor.Cyan);
            if (!enemy.Stats.IsAlive)
                Victory(c);
            else
                c.IO.WriteLine(enemy.HealthDescription(), ConsoleColor.DarkGray);
        });
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
            ctx.IO.WriteLine($"No time to fiddle with the {item.Name} - the {enemy.Name} is right there!");
            return;
        }

        TakeTurn(ctx, c =>
        {
            c.Player.Inventory.Remove(consumable);
            c.IO.WriteLine(consumable.Consume(c.Player.Stats, c.Rng), ConsoleColor.Green);
        });
    }

    private void Flee(GameContext ctx, string _)
    {
        if (CombatMath.ResolveFlee(ctx.Player.Stats.Dexterity, enemy.Stats.DexterityModifier, ctx.Rng))
        {
            ctx.IO.WriteLine($"You break away and put distance between yourself and the {enemy.Name}!", ConsoleColor.Yellow);
            ctx.States.Pop(ctx);
            return;
        }
        ctx.IO.WriteLine($"You turn to run, but the {enemy.Name} cuts you off!", ConsoleColor.Red);
        EnemyTurn(ctx);
    }

    private void ShowAbilities(GameContext ctx, string _)
    {
        ctx.IO.WriteLine($"Mana: {ctx.Player.Stats.Mana}/{ctx.Player.Stats.MaxMana}", ConsoleColor.Cyan);
        foreach (var ability in ctx.Player.Class.Abilities)
            ctx.IO.WriteLine("  " + ability.Summary());
    }

    // --- Enemy turn ------------------------------------------------------------

    private void EnemyTurn(GameContext ctx)
    {
        var player = ctx.Player;

        // Creatures with a special ability use it when they can afford it,
        // roughly 40% of the time. Predictable enough to plan around,
        // random enough to fear.
        if (enemy.Special is { } special
            && enemy.Stats.Mana >= special.ManaCost
            && ctx.Rng.Next(100) < 40)
        {
            enemy.Stats.SpendMana(special.ManaCost);
            var power = special.RollPower(enemy.Stats, ctx.Rng);
            ctx.IO.WriteLine(string.Format(special.FlavorText, "you"), ConsoleColor.Red);
            player.Stats.TakeDamage(power);
            ctx.IO.WriteLine($"{special.Name} deals {power} damage to you!", ConsoleColor.Red);
        }
        else
        {
            var outcome = CombatMath.ResolveAttack(
                enemy.TotalAttackBonus, enemy.DamageDice, 0, player.DefenseValue, ctx.Rng);
            ctx.IO.WriteLine($"The {enemy.Name} {enemy.AttackVerb} you: {outcome.AttackTotal} vs your defense {player.DefenseValue}", ConsoleColor.DarkGray);

            if (outcome.Fumble)
                ctx.IO.WriteLine($"The {enemy.Name} overreaches and stumbles. A gift!", ConsoleColor.Green);
            else if (!outcome.Hit)
                ctx.IO.WriteLine("You twist aside - a miss!");
            else
            {
                player.Stats.TakeDamage(outcome.Damage);
                ctx.IO.WriteLine(outcome.Critical
                    ? $"A vicious blow! You take {outcome.Damage} damage!"
                    : $"You take {outcome.Damage} damage.",
                    ConsoleColor.Red);
            }
        }

        if (!player.Stats.IsAlive)
            Defeat(ctx);
    }

    // --- Endings ----------------------------------------------------------------

    private void Victory(GameContext ctx)
    {
        var player = ctx.Player;
        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"The {enemy.Name} falls! ***", ConsoleColor.Green);

        ctx.CurrentArea.Creatures.Remove(creatureKey);

        var xp = GameMath.ScaledExperience(enemy.XP, enemy.Level, player.Level.Value);
        ctx.IO.WriteLine($"You gain {xp:0.#} experience.", ConsoleColor.Yellow);

        if (enemy.Gold > 0)
        {
            player.AddGold(enemy.Gold);
            ctx.IO.WriteLine($"You loot {enemy.Gold} gold.", ConsoleColor.Yellow);
        }
        foreach (var item in enemy.Loot)
        {
            if (player.Inventory.TryAdd(item))
                ctx.IO.WriteLine($"You take: {item.Name}", ConsoleColor.Yellow);
            else
            {
                ctx.CurrentArea.Items[UniqueKey(ctx.CurrentArea.Items, item.Name)] = item;
                ctx.IO.WriteLine($"The {enemy.Name} drops {item.Name}, but your pack is full - it lies here.", ConsoleColor.Yellow);
            }
        }

        // AddExperience may fire the LeveledUp event (printed by its listener).
        player.AddExperience(xp);

        if (enemy.IsFinalBoss)
        {
            ctx.SetFlag(GameData.GameMechanics.NpcFactory.FlagLichSlain);
            ctx.Journal.Add($"Lich Malakhar is destroyed. The Barrow is quiet at last. Elder Maera should hear of this.");
            ctx.States.Pop(ctx);
            ctx.States.Push(ctx, new VictoryState());
            return;
        }

        ctx.States.Pop(ctx);
    }

    private void Defeat(GameContext ctx)
    {
        ctx.IO.WriteLine();
        ctx.IO.WriteLine("Darkness closes in...", ConsoleColor.DarkRed);
        var cause = $"Slain by {(enemy.Name.StartsWith("Lich") ? "" : "a ")}{enemy.Name} in the {ctx.CurrentArea.Name}, at level {ctx.Player.Level.Value}.";
        ctx.States.Reset(ctx);
        ctx.States.Push(ctx, new GameOverState(cause));
    }
}
