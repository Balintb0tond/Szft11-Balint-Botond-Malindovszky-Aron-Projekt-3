using System;

class Program
{
    static char[,] map = new char[25, 25];
    static bool[,] monsterLocations = new bool[25, 25];
    static bool[,] treasureLocations = new bool[25, 25];
    static int playerX, playerY;
    static int healthPotions = 2;
    static Character player;

    static Random rand = new Random();

    static void Main(string[] args)
    {
        InitializeMap();
        PlaceMonsters(10);
        PlaceTreasures(6);

        player = SelectCharacter();
        PlacePlayer();

        Console.WriteLine("Use WASD keys to move. Press 'Q' to quit.");

        while (true)
        {
            Console.Clear();
            DisplayMap();
            Console.WriteLine($"Character: {player.Type}, Attack: {player.Attack}, Life: {player.Life}, Potions: {healthPotions}");
            char input = Console.ReadKey().KeyChar;
            if (input == 'q' || input == 'Q')
                break;

            MovePlayer(input);
            CheckLocation();
        }
    }

    static void InitializeMap()
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                int tile = rand.Next(100);
                if (tile < 60)
                    map[i, j] = '_'; // Plains
                else if (tile < 75)
                    map[i, j] = '~'; // Rivers
                else if (tile < 90)
                    map[i, j] = '^'; // Mountains
                else
                    map[i, j] = '='; // Swamps
            }
        }
    }

    static void PlaceMonsters(int count)
    {
        PlaceItems(count, monsterLocations);
    }

    static void PlaceTreasures(int count)
    {
        PlaceItems(count, treasureLocations);
    }

    static void PlaceItems(int count, bool[,] locations)
    {
        for (int i = 0; i < count; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(25);
                y = rand.Next(25);
            }
            while (locations[x, y]);
            locations[x, y] = true;
        }
    }

    static Character SelectCharacter()
    {
        Console.Write("\nSelect your character (1-Warrior, 2-Mage, 3-Rogue): ");
        switch (Console.ReadKey().KeyChar)
        {
            case '1': return new Character("Warrior", 7, 3);
            case '2': return new Character("Mage", 6, 2);
            case '3': return new Character("Rogue", 6, 3);
            default: return SelectCharacter();
        }
    }

    static void PlacePlayer()
    {
        do
        {
            playerX = rand.Next(25);
            playerY = rand.Next(25);
        } while (map[playerX, playerY] == '~' || map[playerX, playerY] == '^'); // Avoid placing on impassable terrains initially
    }

    static void DisplayMap()
    {
        for (int i = 0; i < 25; i++)
        {
            for (int j = 0; j < 25; j++)
            {
                if (i == playerX && j == playerY)
                    Console.Write('P');
                else
                    Console.Write(map[i, j]);
            }
            Console.WriteLine();
        }
    }

    static void MovePlayer(char input)
    {
        switch (input)
        {
            case 'w': case 'W': if (playerX > 0) playerX--; break;
            case 's': case 'S': if (playerX < 24) playerX++; break;
            case 'a': case 'A': if (playerY > 0) playerY--; break;
            case 'd': case 'D': if (playerY < 24) playerY++; break;
        }
    }

    static void CheckLocation()
    {
        if (monsterLocations[playerX, playerY])
        {
            Fight();
            monsterLocations[playerX, playerY] = false; // Remove the monster after the fight
        }
        else if (treasureLocations[playerX, playerY])
        {
            Console.WriteLine("You found a treasure!");
            treasureLocations[playerX, playerY] = false;
        }
    }

    static void Fight()
    {
        int monsterAttack = rand.Next(1, 7);
        int playerRoll = player.Attack + rand.Next(1, 7);
        if (monsterAttack >= playerRoll)
        {
            player.Life--;
            Console.WriteLine("You've been hit! Current life: " + player.Life);
            if (player.Life == 0)
            {
                Console.WriteLine("You have died. Game over!");
                Environment.Exit(0);
            }
        }
        else
        {
            Console.WriteLine("You defeated the monster!");
        }
    }
}

class Character
{
    public string Type;
    public int Attack;
    public int Life;

    public Character(string type, int attack, int life)
    {
        Type = type;
        Attack = attack;
        Life = life;
    }
}


class Slime
{
    public int Attack;
    public int Life;
}
