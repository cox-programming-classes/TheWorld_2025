using The_World.Engine.Commands;
using The_World.GameData.Creatures;
using The_World.GameData.Items;

namespace The_World.Engine.States;

/// <summary>
/// Commerce mode, pushed on top of a conversation. Buying moves items
/// from the NPC's wares to your pack; selling does the reverse at half
/// price (the merchant has overhead, you understand).
/// </summary>
public class ShoppingState(Npc npc) : GameStateBase
{
    public override string Name => "Trading";

    public override string GetPrompt(GameContext ctx) => $"[Trading with {npc.Name} | Gold: {ctx.Player.Gold}] > ";

    public override void OnEnter(GameContext ctx)
    {
        if (CommandList.Count == 0)
            CommandList.AddRange(
            [
                new DelegateCommand("list", "See what's for sale.", (c, _) => ListWares(c), aliases: ["wares", "browse"]),
                new DelegateCommand("buy", "Buy an item.", Buy, "buy <item>"),
                new DelegateCommand("sell", "Sell something from your pack (half value).", Sell, "sell <item>"),
                new DelegateCommand("look", "Inspect an item (theirs or yours).", LookAt, "look <item>", "l", "examine"),
                new DelegateCommand("inventory", "See what you're carrying.", (c, _) =>
                    c.IO.WriteLine(c.Player.Inventory.Describe()), aliases: ["i", "inv"]),
                new DelegateCommand("leave", "Stop trading.", (c, _) => c.States.Pop(c), aliases: ["done", "exit", "bye"]),
            ]);

        ctx.IO.WriteLine();
        ctx.IO.WriteLine($"{npc.Name} sets out the goods.", ConsoleColor.Cyan);
        ListWares(ctx);
    }

    private void ListWares(GameContext ctx)
    {
        if (npc.Wares.Count == 0)
        {
            ctx.IO.WriteLine($"\"Cleaned me out, you have.\" {npc.Name} has nothing left to sell.");
            return;
        }
        ctx.IO.WriteLine("--- For sale ---", ConsoleColor.Cyan);
        foreach (var item in npc.Wares)
            ctx.IO.WriteLine($"  {item.Name,-26} {item.Value,4} gold");
        ctx.IO.WriteLine($"You have {ctx.Player.Gold} gold. (buy <item>, sell <item>, leave)", ConsoleColor.DarkGray);
    }

    private static Item? FindIn(IEnumerable<Item> items, string query)
    {
        var q = query.Trim();
        const StringComparison ci = StringComparison.OrdinalIgnoreCase;
        var list = items.ToList();
        return list.FirstOrDefault(i => i.Name.Equals(q, ci))
            ?? list.FirstOrDefault(i => i.Name.StartsWith(q, ci))
            ?? list.FirstOrDefault(i => i.Name.Contains(q, ci));
    }

    private void Buy(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Buy what?");
            return;
        }
        var item = FindIn(npc.Wares, argument);
        if (item is null)
        {
            ctx.IO.WriteLine($"\"Don't stock '{argument}', friend.\"");
            return;
        }
        var price = Math.Max(1, item.Value);
        if (ctx.Player.Gold < price)
        {
            ctx.IO.WriteLine($"\"That's {price} gold. Come back when your purse agrees.\" (You have {ctx.Player.Gold}.)");
            return;
        }
        if (!ctx.Player.Inventory.TryAdd(item))
        {
            ctx.IO.WriteLine($"You can't carry the {item.Name} - you're at {ctx.Player.Inventory.TotalWeight:0.#} of {ctx.Player.Inventory.Capacity:0.#} lbs.");
            return;
        }
        ctx.Player.SpendGold(price);
        npc.Wares.Remove(item);
        ctx.IO.WriteLine($"You buy the {item.Name} for {price} gold. ({ctx.Player.Gold} left)", ConsoleColor.Yellow);
    }

    private void Sell(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ctx.IO.WriteLine("Sell what?");
            return;
        }
        var item = ctx.Player.Inventory.Find(argument);
        if (item is null)
        {
            ctx.IO.WriteLine($"You aren't carrying '{argument}'.");
            return;
        }
        if (item is QuestItem)
        {
            ctx.IO.WriteLine($"{npc.Name} pushes the {item.Name} back across the counter. \"That's not for the likes of my shelf.\"");
            return;
        }
        if (item.Value <= 0)
        {
            ctx.IO.WriteLine($"\"I'd be paying you to haul my rubbish.\" {npc.Name} isn't interested in the {item.Name}.");
            return;
        }
        var price = Math.Max(1, item.Value / 2);
        ctx.Player.Inventory.Remove(item);
        ctx.Player.AddGold(price);
        npc.Wares.Add(item);
        ctx.IO.WriteLine($"You sell the {item.Name} for {price} gold. ({ctx.Player.Gold} total)", ConsoleColor.Yellow);
    }

    private void LookAt(GameContext ctx, string argument)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            ListWares(ctx);
            return;
        }
        var item = FindIn(npc.Wares, argument) ?? ctx.Player.Inventory.Find(argument);
        ctx.IO.WriteLine(item is null
            ? $"No '{argument}' on either side of the counter."
            : item.Look());
    }
}
