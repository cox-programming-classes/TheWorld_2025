namespace The_World.GameData.GameMechanics;

/// <summary>
/// Small ASCII depictions used by inspect text. Kept out of factories and
/// builders so the world data stays readable.
/// </summary>
public static class AsciiArt
{
    public static class Areas
    {
        public const string Village = """
              /\        []        /\
             /__\   ___/  \___   /__\
             |  |  |  _    _  |  |  |
             |__|  |_/ \__/ \_|  |__|
                    Willowbrook
            """;

        public const string Tavern = """
              ___________________
             /  RUSTY FLAGON   /|
            /__________________/ |
            |   __      __    | |
            |  |__|    |__|   | /
            |______/\/\_______|/
            """;

        public const string Forest = """
                 /\      /\       /\
                /**\    /^^\     /**\
               /****\  /^^^^\   /****\
                 ||      ||       ||
              .--''------''-------''--.
            """;

        public const string Clearing = """
                \  |  /        .-.
              ---  *  ---   . (   ) .
                /  |  \       `-'
             ~~~  ~~~  ~~~  ~~~  ~~~
            """;

        public const string Hut = """
                    /\
                   /  \
              ____/____\____
             / _   _   _   /|
            /_/ \_/ \_/ \_/ |
            |  herbs dry  | /
            |_____________|/
            """;

        public const string Mill = """
                 _________
                /  OLD   /|
               /  MILL  / |
              /_______ /  |
              |  __   |  O
              | |  |  | /|\
              |_|__|__|/ |
            """;

        public const string Cave = """
                  _________
              ___/  _   _  \___
             /    _( )_( )_    \
            /____/         \____\
                 goblin cave
            """;

        public const string ChiefsDen = """
               .----------------.
              /  trophies       /|
             /______.----------/ |
             |  _   | STRONG  | |
             | (_)  |  BOX    | /
             |______|_________|/
            """;

        public const string Foothills = """
                    /\        /\
                   /  \  /\  /  \
              /\  /    \/  \/    \
             /  \/              /\
            /____________________\
            """;

        public const string Ruins = """
                _|_       __       _|_
                 |       /  \       |
              ___|__    /____\   ___|__
             /     /|   |    |  /     /|
            /_____/ |   |____| /_____/ |
            """;

        public const string Crypt = """
             .---------------------.
             | []   []   []   []  |
             |  .-.      .-.      |
             | (___)    (___)     |
             |____________________|
            """;

        public const string Depths = """
                 _____________
                /  IRON GATE /|
               /____________/ |
               ||  | | |  || |
               ||__|_|_|__|| /
               |___________|/
            """;

        public const string Sanctum = """
                   .-.
                  (   )
                   `-'
              ___  /|\  ___
             /___\_|_|_/___\
             | green fire |
             |____________|
            """;
    }

    public static class Creatures
    {
        public const string Goblin = """
               ,      ,
              /(.-""-.)\
            |\  \/      \/  /|
            | \ / =.  .= \ / |
             \(    ^    )/
             \  '-'  /
              `-----'
            """;

        public const string Wolf = """
               / \__
              (    @\___
              /         O
             /   (_____/
            /_____/   U
            """;

        public const string Bandit = """
                 O
                /|\
               / | \__
                / \
               /   \
              "coin?"
            """;

        public const string Spider = """
              \  |  /   \  |  /
               \ | / .-. \ | /
             --- ( (o o) ) ---
               / | \ `-' / | \
              /  |  \   /  |  \
            """;

        public const string GoblinChief = """
                .-^^^^-.
               /  o  o  \
              |    <>    |
              |  \____/  |
             /|   KEY    |\
              |__________|
            """;

        public const string Golem = """
                 _______
                / _____ \
               | |  _  | |
               | | |_| | |
               | |_____| |
              /___________\
            """;

        public const string Skeleton = """
                 .-.
                (o.o)
                 |=|
                __|__
              //.=|=.\\
             // .=|=. \\
                /   \
            """;

        public const string Wight = """
                  .-.
                 (   )
                  ) (
               .-'   '-.
              /  frost  \
             /___________\
            """;

        public const string Lich = """
                   .-.
                  (o o)
                  | O |
                 /|===|\
                /_|___|_\
                  /   \
              Malakhar
            """;

        public const string Deer = """
               /\   /\
              /  \ /  \
                 (..)
                  /|\
                 / | \
            """;

        public const string Elder = """
                  __
                 /  \
                | () |
                 \__/
                 /||\
                /_||_\
             elder
            """;

        public const string Merchant = """
                 ______
                / ____ \
               | |$  $| |
               | |____| |
                \______/
                 /|  |\
            """;

        public const string Barkeep = """
                  __
                 /__\  _
                (____)(_)
                 /||\
                /_||_\
              tankard ready
            """;

        public const string Gambler = """
                  .-.
                 (o o)
                  |=|
                __/ \__
                | d6 d6 |
            """;

        public const string Hermit = """
                  .-.
                 ( - )
                  |=|
                 /| |\
                /_|_|_\
                 herbs
            """;
    }

    public static class Items
    {
        public const string Sword = """
                  /\
                 /  \
                 |  |
                 |  |
              ___|  |___
                 |__|
                  ||
            """;

        public const string Dagger = """
                  /\
                 /  \
                 |  |
              ___|__|___
                  ||
            """;

        public const string Staff = """
                 __
                /  \
                \__/
                 ||
                 ||
                 ||
                 ||
            """;

        public const string Greatsword = """
                    /\
                   /  \
                  /____\
                    ||
                    ||
              ======||======
                    ||
            """;

        public const string Armor = """
               .--------.
              /|  /\ /\ |\
             /_| /__V__\ |_\
               |  |  |  |
               |__|__|__|
            """;

        public const string Potion = """
                  __
                 /__\
                 |  |
                /____\
                \____/
            """;

        public const string Herb = """
                  _ _
                _{ ' }_
               { `.!.` }
                `-._.-'
            """;

        public const string Blossom = """
                   .-.
              .-. (   ) .-.
             (   ) `-' (   )
              `-'  |  `-'
                   |
            """;

        public const string Key = """
                 __
                /  \
                \__/
                  |
                  |__
                  |__|
            """;

        public const string Strongbox = """
              .-----------.
              | BRAM'S    |
              | PROVISIONS|
              |____   ____|
                   |_|
            """;

        public const string Phylactery = """
                  .---.
                 /  X  \
                |  ___  |
                | /___\ |
                 \_____/
            """;

        public const string Valuable = """
                  __
                .'  '.
               / .--. \
               \ '--' /
                '.__.'
            """;

        public const string Tome = """
              .------------.
              |  ANCIENT   |
              |   TOME     |
              |  _    _    |
              |_| |__| |___|
            """;
    }
}
