using The_World.GameData.Creatures;

namespace The_World.GameData.GameMechanics;

/// <summary>
/// Builds the world's NPCs and their dialogue trees.
/// Dialogue is pure data (see Dialogue.cs) - the DialogueState interprets it.
/// </summary>
public static class NpcFactory
{
    // Story flags used across NPCs. Kept here so typos can't fork the plot.
    public const string FlagQuestAccepted = "main_quest_accepted";
    public const string FlagLichSlain = "lich_slain";
    public const string FlagElderRewardGiven = "elder_reward_given";
    public const string FlagBramQuestKnown = "bram_quest_known";
    public const string FlagBramRewarded = "bram_rewarded";
    public const string FlagFinnSuspected = "finn_suspected";
    public const string FlagFinnPlaysFair = "finn_plays_fair";

    public static Npc ElderMaera()
    {
        var tree = new DialogueTree("start", new Dictionary<string, DialogueNode>
        {
            ["start"] = new("start",
                "An old woman with river-grey eyes studies you from the steps of the moot hall. " +
                "\"A new face. And an armed one, at that. I am Maera, elder of Willowbrook.\"",
                [
                    new DialogueChoice("\"What troubles this village?\"", "trouble",
                        ForbiddenFlag: FlagQuestAccepted),
                    new DialogueChoice("\"Remind me about the Lich.\"", "reminder",
                        RequiredFlag: FlagQuestAccepted, ForbiddenFlag: FlagLichSlain),
                    new DialogueChoice("\"The Lich is destroyed. It's done.\"", "gratitude",
                        RequiredFlag: FlagLichSlain, ForbiddenFlag: FlagElderRewardGiven,
                        Effects:
                        [
                            new DialogueEffect(DialogueEffectKind.SetFlag, FlagElderRewardGiven),
                            new DialogueEffect(DialogueEffectKind.GiveGold, "200"),
                            new DialogueEffect(DialogueEffectKind.AddJournal,
                                "Elder Maera rewarded me with 200 gold for destroying Lich Malakhar.")
                        ]),
                    new DialogueChoice("\"Who are you, exactly?\"", "about"),
                    new DialogueChoice("\"Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["about"] = new("about",
                "\"Elder of this village these thirty years. I remember when the Barrow Crypt was just " +
                "a sad pile of stones, and not a wound in the world. Memory is mostly what I'm for, these days.\"",
                [
                    new DialogueChoice("\"What troubles this village?\"", "trouble",
                        ForbiddenFlag: FlagQuestAccepted),
                    new DialogueChoice("\"Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["trouble"] = new("trouble",
                "Her face darkens. \"The Lich Malakhar stirs again beneath the Barrow Crypt, north past the " +
                "old ruins. Our cattle sicken. Our dead do not stay put.\" She grips your arm with surprising " +
                "strength. \"The crypt's inner gate needs the old Barrow Key - and goblins carried it off in a raid " +
                "last spring. Their cave lies north of the Dark Forest, east of here.\"",
                [
                    new DialogueChoice("\"I will destroy this Lich for you.\"", "accepted",
                        Effects:
                        [
                            new DialogueEffect(DialogueEffectKind.SetFlag, FlagQuestAccepted),
                            new DialogueEffect(DialogueEffectKind.GiveItem, "healing_potion"),
                            new DialogueEffect(DialogueEffectKind.AddJournal,
                                "Elder Maera asked me to destroy Lich Malakhar beneath the Barrow Crypt. " +
                                "The crypt's inner gate needs the Barrow Key - goblins took it to their cave north of the Dark Forest.")
                        ]),
                    new DialogueChoice("\"Sounds like a problem for someone else.\"", "coward")
                ]),

            ["accepted"] = new("accepted",
                "\"Then Willowbrook's hopes go with you.\" She presses a small flask into your hand. " +
                "\"Take this - brewed it myself. Get the key from those goblins first. And eat something. " +
                "You look thin.\"",
                [
                    new DialogueChoice("\"I won't fail.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["coward"] = new("coward",
                "\"Mm. That's what the last three said.\" She turns back to her ledger. " +
                "\"The graves fill up all the same.\"",
                [
                    new DialogueChoice("\"...Fine. Tell me about this Lich.\"", "trouble"),
                    new DialogueChoice("\"Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["reminder"] = new("reminder",
                "\"The Barrow Key hangs from the belt of the goblin chief - their cave is north of the Dark Forest. " +
                "The crypt itself lies past the mountain foothills and the old ruins, to the north. " +
                "Malakhar waits at the bottom of it. Do not be polite to him.\"",
                [
                    new DialogueChoice("\"I'm on my way.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["gratitude"] = new("gratitude",
                "Maera closes her eyes a long moment. When she opens them, they are wet. " +
                "\"Thirty years I feared that thing under the hill. The village owes you more than this - " +
                "but take it, and our thanks.\" (She presses a heavy purse into your hands: 200 gold!)",
                [
                    new DialogueChoice("\"It was an honor, Elder.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ])
        });

        return Npc.CreateNpc(
            "Elder Maera",
            "A silver-haired woman in a woolen shawl, with eyes that miss nothing.",
            level: 5,
            dialogue: tree);
    }

    public static Npc MerchantBram()
    {
        var tree = new DialogueTree("start", new Dictionary<string, DialogueNode>
        {
            ["start"] = new("start",
                "A round man in a flour-dusted apron beams at you from behind a counter groaning with goods. " +
                "\"Welcome, welcome to Bram's Provisions! Finest stock in Willowbrook - also the only stock, but let's not dwell.\"",
                [
                    new DialogueChoice("\"Show me your wares.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.OpenShop)]),
                    new DialogueChoice("\"Heard any rumors?\"", "rumors",
                        ForbiddenFlag: FlagBramQuestKnown),
                    new DialogueChoice("\"About your stolen strongbox...\"", "quest_reminder",
                        RequiredFlag: FlagBramQuestKnown, ForbiddenFlag: FlagBramRewarded),
                    new DialogueChoice("\"I found your stolen goods.\"", "goods_returned",
                        RequiredItem: "Stolen Goods", ForbiddenFlag: FlagBramRewarded,
                        Effects:
                        [
                            new DialogueEffect(DialogueEffectKind.TakeItem, "Stolen Goods"),
                            new DialogueEffect(DialogueEffectKind.GiveGold, "50"),
                            new DialogueEffect(DialogueEffectKind.SetFlag, FlagBramRewarded),
                            new DialogueEffect(DialogueEffectKind.AddJournal,
                                "Returned Bram's stolen strongbox. He paid 50 gold, and only cried a little.")
                        ]),
                    new DialogueChoice("\"Just browsing. Goodbye.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["rumors"] = new("rumors",
                "Bram leans in, lowering his voice. \"Goblins hit my supply cart on the forest road last month. " +
                "Took a whole strongbox - my best inventory! They den in a cave north of the Dark Forest.\" " +
                "He wrings his apron. \"Bring it back and there's fifty gold in it for you. FIFTY. That's how much I loved that box.\"",
                [
                    new DialogueChoice("\"I'll keep an eye out for it.\"",
                        Effects:
                        [
                            new DialogueEffect(DialogueEffectKind.SetFlag, FlagBramQuestKnown),
                            new DialogueEffect(DialogueEffectKind.AddJournal,
                                "Merchant Bram will pay 50 gold for the strongbox goblins stole from him. Their cave is north of the Dark Forest."),
                            new DialogueEffect(DialogueEffectKind.EndConversation)
                        ])
                ]),

            ["quest_reminder"] = new("quest_reminder",
                "\"My strongbox? Still with those thieving goblins, north of the Dark Forest.\" He sighs like " +
                "a man remembering a lost love. \"Stamped with my own name, it is. Fifty gold on its return.\"",
                [
                    new DialogueChoice("\"I'm working on it.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["goods_returned"] = new("goods_returned",
                "Bram's eyes go wide as saucers. \"My BOX!\" He hugs it. He actually hugs it. " +
                "\"You wonderful, dangerous person. Fifty gold, as promised - and you shop here free of judgment forever.\"",
                [
                    new DialogueChoice("\"A pleasure doing business.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)]),
                    new DialogueChoice("\"Now, about those wares...\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.OpenShop)])
                ])
        });

        return Npc.CreateNpc(
            "Merchant Bram",
            "A cheerfully round shopkeeper whose apron has seen every kind of stain.",
            level: 2,
            dialogue: tree,
            wares:
            [
                ItemFactory.HealingPotion(),
                ItemFactory.HealingPotion(),
                ItemFactory.ManaPotion(),
                ItemFactory.IronSword(),
                ItemFactory.ChainMail(),
                ItemFactory.LeatherArmor()
            ]);
    }

    public static Npc BarkeepHulda()
    {
        var tree = new DialogueTree("start", new Dictionary<string, DialogueNode>
        {
            ["start"] = new("start",
                "The barkeep polishes a tankard with the air of a woman who has heard everything twice. " +
                "\"Welcome to the Rusty Flagon. We've got ale, news, and Finn.\" She nods at the corner table. \"Two of those are worth your coin.\"",
                [
                    new DialogueChoice("\"A drink, please.\" (5 gold)", "drink",
                        Effects: [new DialogueEffect(DialogueEffectKind.HealPlayer, "5")]),
                    new DialogueChoice("\"What's the word around town?\"", "rumor1"),
                    new DialogueChoice("\"Nothing for me, thanks.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["drink"] = new("drink",
                "She slides a foaming tankard down the bar. The first swallow tastes like barley; " +
                "the second like courage.",
                [
                    new DialogueChoice("\"What's the word around town?\"", "rumor1"),
                    new DialogueChoice("\"That hit the spot. Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["rumor1"] = new("rumor1",
                "\"Old Tam swears the sunny clearing east of the forest grows silver moonpetals - says you'll " +
                "never spot them unless you stop and truly SEARCH the place.\" She shrugs. \"Tam also married a scarecrow, so.\"",
                [
                    new DialogueChoice("\"Anything else?\"", "rumor2"),
                    new DialogueChoice("\"Good to know. Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["rumor2"] = new("rumor2",
                "\"A bandit's been squatting in the old mill south of the forest - and travelers say the ruins " +
                "up in the foothills hide more than rubble, if a body takes time to search them. 'Course, the golem objects.\"",
                [
                    new DialogueChoice("\"Anything else?\"", "rumor3"),
                    new DialogueChoice("\"Thanks for the warning.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["rumor3"] = new("rumor3",
                "She leans close. \"The hermit Odo, past the clearing? Was a crypt-priest once, before the Barrow " +
                "went bad. If you're fool enough to go down there, talk to him first.\" She straightens. \"That's the lot. Drink or make room.\"",
                [
                    new DialogueChoice("\"A drink, then.\" (5 gold)", "drink",
                        Effects: [new DialogueEffect(DialogueEffectKind.HealPlayer, "5")]),
                    new DialogueChoice("\"Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ])
        });

        return Npc.CreateNpc(
            "Barkeep Hulda",
            "A broad-shouldered woman with forearms like a blacksmith and the patience of a saint. A tired saint.",
            level: 3,
            dialogue: tree);
    }

    public static Npc FinnTheGambler()
    {
        var tree = new DialogueTree("start", new Dictionary<string, DialogueNode>
        {
            ["start"] = new("start",
                "A wiry man with quick eyes and quicker fingers shuffles a pair of bone dice across his knuckles. " +
                "\"Well now! You look like someone whose purse is too heavy. Finn's the name - Knucklebones is the game. Care to sit?\"",
                [
                    new DialogueChoice("\"Deal me in.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.StartGambling)]),
                    new DialogueChoice("\"How do you play?\"", "rules"),
                    new DialogueChoice("\"You seem... suspiciously lucky.\"", "lucky"),
                    new DialogueChoice("\"Not today, Finn.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["rules"] = new("rules",
                "\"Simplest game under heaven. You bet, we each roll two dice, high roll takes the pot. " +
                "Ties push.\" He grins, all teeth. \"Pure, honest chance.\"",
                [
                    new DialogueChoice("\"Deal me in.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.StartGambling)]),
                    new DialogueChoice("\"Maybe later.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["lucky"] = new("lucky",
                "Finn presses a hand to his chest, deeply wounded. \"Luck is a lady, friend, and I am merely " +
                "her favorite.\" The dice vanish somewhere about his person. \"Sit down and test her yourself. " +
                "Watch me close as you like.\"",
                [
                    new DialogueChoice("\"Fine. Deal me in.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.StartGambling)]),
                    new DialogueChoice("\"I'll pass.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ])
        });

        return Npc.CreateNpc(
            "Finn",
            "A gambler in a patched velvet coat, rolling a pair of dice across his knuckles like they owe him money.",
            level: 2,
            dialogue: tree);
    }

    public static Npc HermitOdo()
    {
        var tree = new DialogueTree("start", new Dictionary<string, DialogueNode>
        {
            ["start"] = new("start",
                "A stooped old man in patched priest's robes tends a pot over the fire. He doesn't look up. " +
                "\"Few find my hut. Fewer knock. You did neither, but come in anyway.\"",
                [
                    new DialogueChoice("\"Can you heal my wounds?\" (10 gold)", "healed",
                        Effects: [new DialogueEffect(DialogueEffectKind.HealPlayer, "10")]),
                    new DialogueChoice("\"What do you have for sale?\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.OpenShop)]),
                    new DialogueChoice("\"Tell me of the Barrow Crypt.\"", "lore"),
                    new DialogueChoice("\"Just passing through. Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["healed"] = new("healed",
                "Odo mutters over you in a language that smells like rain. Warmth follows.",
                [
                    new DialogueChoice("\"Tell me of the Barrow Crypt.\"", "lore"),
                    new DialogueChoice("\"My thanks, father.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ]),

            ["lore"] = new("lore",
                "His stirring stops. \"I kept the crypt shrine, once. Malakhar was a scholar before he was a " +
                "monster - pride was the door death walked in through.\" He finally looks at you, and his eyes are old. " +
                "\"Below the barrow, past the wight, there is an iron gate only the old Barrow Key will open. " +
                "Fill your belly, carry more potions than you think dignified, and when the Lich gathers his power - strike, don't run. " +
                "Hesitation feeds him.\"",
                [
                    new DialogueChoice("\"Can you heal my wounds?\" (10 gold)", "healed",
                        Effects: [new DialogueEffect(DialogueEffectKind.HealPlayer, "10")]),
                    new DialogueChoice("\"Thank you, father. Farewell.\"",
                        Effects: [new DialogueEffect(DialogueEffectKind.EndConversation)])
                ])
        });

        return Npc.CreateNpc(
            "Hermit Odo",
            "A weathered old priest living far from anything, with kind hands and haunted eyes.",
            level: 6,
            dialogue: tree,
            wares:
            [
                ItemFactory.HealingPotion(),
                ItemFactory.GreaterHealingPotion(),
                ItemFactory.ManaPotion(),
                ItemFactory.ManaPotion(),
                ItemFactory.HealingHerb()
            ]);
    }
}
