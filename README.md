# The World

*A text adventure of dice and doom.*

Something old is waking under the barrow north of Willowbrook, the dead
aren't staying put, and the village elder is paying. Roll a hero, pack more
potions than you think dignified, and go introduce Lich Malakhar to the
business end of your character sheet.

```
dotnet run                  # play
dotnet run -- --seed 42     # play with reproducible dice
dotnet test                 # run the 108-test suite
```

Requires the .NET 10 SDK.

## Playing

Character creation rolls your ability scores the traditional way (4d6, drop
the lowest) and offers three classes — **Warrior**, **Mage**, **Rogue** —
each with its own stats, growth, starting kit, and special abilities.

Type `help` at any time. *What the help lists depends on what the game is
being* — that's the state machine (see below). A few highlights:

| While...   | You can...                                                            |
|------------|-----------------------------------------------------------------------|
| Exploring  | `look`, `go north` (or just `n`), `get`/`drop`/`equip`/`use`, `talk`, `attack`, `search`, `rest`, `stats`, `journal` |
| In combat  | `attack`, `cast <ability>`, `use <potion>`, `flee`, `look`, `inventory` |
| Talking    | pick numbered dialogue choices, `leave`                                |
| Trading    | `list`, `buy`, `sell`, `leave`                                         |
| Gambling   | `bet <gold>`, `watch`, `accuse`, `leave`                               |

Tips from the management: the tavern sells rumors cheaper than the shop
sells potions, `search` rewards the clever, deer are not combatants, and
Finn's dice deserve close observation.

## Design

Built for an object-oriented design course; the interesting decisions:

### Push-down state machine
`GameStateMachine` keeps a *stack* of `IGameState`s. Each state owns its own
command set and prompt, so the available verbs change with the game mode:

```
Exploring ── attack goblin ──▶ push Combat        (win/flee pops back)
Exploring ── talk elder ─────▶ push Dialogue
Dialogue ── "show me wares" ─▶ push Shopping      (leave pops back to the
Dialogue ── "deal me in" ────▶ push Gambling       same dialogue node)
anywhere ── death ───────────▶ reset, push GameOver
Lich dies ───────────────────▶ push Victory
```

Pushing suspends a state; popping resumes it exactly where it was
(`OnResume`). That's why you can browse a shop mid-conversation and land
back on the same dialogue beat.

### Command Pattern
Every verb is an `ICommand` (name, aliases, usage, `Execute`).
`CommandProcessor` resolves input against the current state's commands plus
globals (`help`, `quit`), after giving the state's raw-input hook first
refusal — that's how dialogue reads bare numbers and exploring accepts bare
directions.

### Dice all the way down
`Dice` is a record (`2d6+3` parses and prints), rolls take an optional
`Random` so the whole game is seedable, `DiceResult` keeps individual die
faces for crit/fumble detection, and `WeightedDice` overrides `Roll`
polymorphically — which is why Finn keeps winning until you catch him.
Combat is d20 vs. defense with crits doubling damage dice; checks are
d20 + ability modifier vs. DC.

### Data-driven NPCs
NPCs are records holding a `DialogueTree` — nodes, choices, and declarative
effects (`SetFlag`, `GiveItem`, `OpenShop`, `StartGambling`, ...). Choices
gate on story flags or carried items, so quests are data, not code. A test
validates every tree so a mistyped node id can't ship.

### Records with discipline
The domain model is records: value types where value semantics help
(`Dice`, items), records with encapsulated mutation where the shell
established the pattern (`StatChart.TakeDamage`, `Player.AddExperience` —
which raises a `LeveledUp` event the engine narrates). The item hierarchy
(`Weapon`/`Armor`/`Consumable`/`QuestItem`) is a closed set of derived
records, pattern-matched like a discriminated union.

## Project layout

```
Program.cs                  entry point (arg parsing + engine start)
Engine/
  GameEngine.cs             the main loop
  GameContext.cs            player, area, flags, journal, RNG, IO
  IGameIO.cs                console abstraction (tests use a scripted one)
  Finder.cs                 fuzzy lookup ("gob" finds the Goblin Scout)
  StateMachine/             IGameState + the push-down stack
  Commands/                 ICommand, DelegateCommand, CommandProcessor
  States/                   the eight game states
GameData/
  Player.cs, PlayerClass.cs, Stats.cs, Abilities/
  Items/                    hierarchy + Inventory + Equipment
  Creatures/                Creature, Npc, dialogue trees
  Areas/                    Area record + fluent AreaBuilder
  GameMechanics/            Dice, GameMath, CombatMath, factories, WorldBuilder
TheWorld.Tests/             xUnit suite (mechanics, world integrity, engine integration)
```

## Future work

Save/load (the world graph is cyclic, so it wants real serialization
design), a logging seam behind `IGameIO`, creature respawns, and whatever
is scratching at the door of the Old Mill at night.
