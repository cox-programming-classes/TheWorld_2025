using The_World.Engine.Commands;
using The_World.GameData.Creatures;
using The_World.GameData.GameMechanics;

namespace The_World.Engine.States;

/// <summary>
/// Conversation mode: the NPC's dialogue tree drives everything.
/// The player picks numbered choices; choices can gate on flags/items and
/// trigger effects (quests, gifts, shops, dice games...). Shops and games
/// push their own states on top of this one - when they pop, the
/// conversation resumes right where it was.
/// </summary>
public class DialogueState(Npc npc) : GameStateBase
{
    private DialogueNode _node = npc.Dialogue.Start;
    private List<DialogueChoice> _visible = [];

    public override string Name => "Talking";

    public override string GetPrompt(GameContext ctx) => $"[Talking to {npc.Name}] > ";

    public override void OnEnter(GameContext ctx)
    {
        if (CommandList.Count == 0)
            CommandList.Add(new DelegateCommand("leave", "End the conversation.",
                (c, _) => c.States.Pop(c), aliases: ["bye", "farewell", "goodbye"]));

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"--- {npc.Name} ---", ConsoleColor.Cyan);
        ctx.IO.WriteLine(npc.Description, ConsoleColor.DarkGray);
        Render(ctx);
    }

    public override void OnResume(GameContext ctx)
    {
        // Back from a shop or a dice game - pick up the thread.
        Render(ctx);
    }

    private void Render(GameContext ctx)
    {
        ctx.IO.WriteLine();
        ctx.IO.WriteLine(_node.Text);
        ctx.IO.WriteLine();

        _visible = _node.Choices.Where(choice => IsVisible(ctx, choice)).ToList();
        for (var i = 0; i < _visible.Count; i++)
            ctx.IO.WriteLine($"  {i + 1}. {_visible[i].Text}", ConsoleColor.Yellow);
        ctx.IO.WriteLine("(Choose a number, or 'leave'.)", ConsoleColor.DarkGray);
    }

    private static bool IsVisible(GameContext ctx, DialogueChoice choice)
    {
        if (choice.RequiredFlag is not null && !ctx.HasFlag(choice.RequiredFlag))
            return false;
        if (choice.ForbiddenFlag is not null && ctx.HasFlag(choice.ForbiddenFlag))
            return false;
        if (choice.RequiredItem is not null && ctx.Player.Inventory.Find(choice.RequiredItem) is null)
            return false;
        return true;
    }

    public override bool TryHandleRaw(GameContext ctx, string input)
    {
        if (!int.TryParse(input.Trim(), out var number))
            return false;
        if (number < 1 || number > _visible.Count)
        {
            ctx.IO.WriteLine($"Pick a number between 1 and {_visible.Count}.");
            return true;
        }

        Choose(ctx, _visible[number - 1]);
        return true;
    }

    private void Choose(GameContext ctx, DialogueChoice choice)
    {
        ctx.IO.WriteLine($"> {choice.Text}", ConsoleColor.DarkGray);

        var endConversation = false;
        GameStateBase? stateToPush = null;

        foreach (var effect in choice.Effects)
        {
            switch (effect.Kind)
            {
                case DialogueEffectKind.EndConversation:
                    endConversation = true;
                    break;

                case DialogueEffectKind.OpenShop:
                    stateToPush = new ShoppingState(npc);
                    break;

                case DialogueEffectKind.StartGambling:
                    stateToPush = new GamblingState(npc);
                    break;

                case DialogueEffectKind.SetFlag:
                    ctx.SetFlag(effect.Arg);
                    break;

                case DialogueEffectKind.GiveItem:
                    var item = ItemFactory.CreateByKey(effect.Arg);
                    if (ctx.Player.Inventory.TryAdd(item))
                        ctx.IO.WriteLine($"[Received: {item.Name}]", ConsoleColor.Yellow);
                    else
                    {
                        ctx.CurrentArea.Items[UniqueKey(ctx.CurrentArea.Items, item.Name)] = item;
                        ctx.IO.WriteLine($"[Your pack is full - the {item.Name} is set at your feet.]", ConsoleColor.Yellow);
                    }
                    break;

                case DialogueEffectKind.TakeItem:
                    var taken = ctx.Player.Inventory.Find(effect.Arg);
                    if (taken is not null)
                    {
                        ctx.Player.Inventory.Remove(taken);
                        ctx.IO.WriteLine($"[Handed over: {taken.Name}]", ConsoleColor.Yellow);
                    }
                    break;

                case DialogueEffectKind.GiveGold:
                    if (int.TryParse(effect.Arg, out var amount) && amount > 0)
                    {
                        ctx.Player.AddGold(amount);
                        ctx.IO.WriteLine($"[Received: {amount} gold]", ConsoleColor.Yellow);
                    }
                    break;

                case DialogueEffectKind.HealPlayer:
                    HealPlayer(ctx, effect.Arg);
                    break;

                case DialogueEffectKind.AddJournal:
                    ctx.Journal.Add(effect.Arg);
                    ctx.IO.WriteLine("[Journal updated]", ConsoleColor.DarkCyan);
                    break;
            }
        }

        if (endConversation)
        {
            ctx.States.Pop(ctx);
            return;
        }

        if (choice.NextNodeId is not null)
        {
            _node = npc.Dialogue.Get(choice.NextNodeId)
                    ?? throw new InvalidOperationException(
                        $"Dialogue node '{choice.NextNodeId}' missing from {npc.Name}'s tree.");
            if (stateToPush is null)
            {
                Render(ctx);
                return;
            }
            // Show the new node's text as the send-off, then open the shop/game;
            // OnResume re-renders the node when we return.
            ctx.States.Push(ctx, stateToPush);
            return;
        }

        if (stateToPush is not null)
        {
            ctx.States.Push(ctx, stateToPush);
            return;
        }

        // No destination and nothing pushed: the conversation has run its course.
        ctx.States.Pop(ctx);
    }

    private void HealPlayer(GameContext ctx, string arg)
    {
        var cost = int.TryParse(arg, out var c) ? Math.Max(0, c) : 0;
        if (cost > 0 && !ctx.Player.SpendGold(cost))
        {
            ctx.IO.WriteLine($"[You can't afford it - {cost} gold needed, you have {ctx.Player.Gold}.]", ConsoleColor.Red);
            return;
        }
        ctx.Player.Stats.FullRestore();
        ctx.IO.WriteLine(cost > 0
            ? $"[{cost} gold paid. Health and mana fully restored!]"
            : "[Health and mana fully restored!]", ConsoleColor.Green);
    }
}
