namespace The_World.GameData.Creatures;

/// <summary>
/// Side effects a dialogue choice can trigger. The DialogueState interprets
/// these - the data stays declarative so NPCs are pure data.
/// </summary>
public enum DialogueEffectKind
{
    /// <summary>End the conversation and return to exploring.</summary>
    EndConversation,

    /// <summary>Open this NPC's shop (pushes the Shopping state).</summary>
    OpenShop,

    /// <summary>Sit down for a dice game (pushes the Gambling state).</summary>
    StartGambling,

    /// <summary>Set a story flag. Arg = flag name.</summary>
    SetFlag,

    /// <summary>Give the player an item. Arg = ItemFactory key.</summary>
    GiveItem,

    /// <summary>Take a named item from the player. Arg = item name.</summary>
    TakeItem,

    /// <summary>Give the player gold. Arg = amount.</summary>
    GiveGold,

    /// <summary>Fully restore the player. Arg = gold cost (empty or "0" = free).</summary>
    HealPlayer,

    /// <summary>Add an entry to the player's journal. Arg = the entry text.</summary>
    AddJournal
}

/// <summary>One concrete effect with its argument.</summary>
public record DialogueEffect(DialogueEffectKind Kind, string Arg = "");

/// <summary>
/// One numbered option the player can pick.
/// Choices can be gated behind story flags or items in the player's pack.
/// </summary>
/// <param name="Text">What the player says.</param>
/// <param name="NextNodeId">Where the conversation goes (null = conversation ends).</param>
/// <param name="Effects">Side effects applied, in order, when picked.</param>
/// <param name="RequiredFlag">Only shown if this story flag is set.</param>
/// <param name="ForbiddenFlag">Hidden if this story flag is set.</param>
/// <param name="RequiredItem">Only shown if the player carries this item (by name).</param>
public record DialogueChoice(
    string Text,
    string? NextNodeId = null,
    List<DialogueEffect>? Effects = null,
    string? RequiredFlag = null,
    string? ForbiddenFlag = null,
    string? RequiredItem = null)
{
    public List<DialogueEffect> Effects { get; } = Effects ?? [];
}

/// <summary>
/// One beat of conversation: what the NPC says, and how you may reply.
/// </summary>
public record DialogueNode(string Id, string Text, List<DialogueChoice> Choices);

/// <summary>
/// A whole conversation graph for one NPC.
/// </summary>
public record DialogueTree(string StartNodeId, Dictionary<string, DialogueNode> Nodes)
{
    public DialogueNode Start => Nodes[StartNodeId];

    public DialogueNode? Get(string id) => Nodes.GetValueOrDefault(id);

    /// <summary>
    /// Sanity-check the graph: every NextNodeId must point at a real node,
    /// and the start node must exist. Returns a list of problems (empty = healthy).
    /// Used by unit tests so a typo in a node id can't ship.
    /// </summary>
    public List<string> Validate()
    {
        var problems = new List<string>();
        if (!Nodes.ContainsKey(StartNodeId))
            problems.Add($"Start node '{StartNodeId}' does not exist.");

        foreach (var node in Nodes.Values)
        {
            if (node.Choices.Count == 0)
                problems.Add($"Node '{node.Id}' has no choices - the player would be trapped.");
            foreach (var choice in node.Choices)
                if (choice.NextNodeId is not null && !Nodes.ContainsKey(choice.NextNodeId))
                    problems.Add($"Node '{node.Id}' choice '{choice.Text}' points at missing node '{choice.NextNodeId}'.");
        }
        return problems;
    }
}
