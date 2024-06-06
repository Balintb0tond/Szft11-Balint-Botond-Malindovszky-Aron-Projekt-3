using System;
using System.Collections.Generic;
using DungeonRush;

class Program
{
    static char[,] map = new char[25, 25];
    static Monster[,] monsterLocations = new Monster[25, 25];
    static Boss[,] bossLocations = new Boss[25, 25];
    static bool[,] treasureLocations = new bool[25, 25];
    static Weapon[,] weaponLocations = new Weapon[25, 25];
    static Armor[,] armorLocations = new Armor[25, 25];
    static int playerX, playerY;
    static int healthPotions = 2;
    static Character player;

    static Random rand = new Random();

    static void Main(string[] args)
    {
        InitializeMap();

        player = SelectCharacter();
        PlacePlayer();

        Console.WriteLine("Use WASD keys to move. Press 'Q' to quit.");

        while (true)
        {
            Console.Clear();
            DisplayMap();
            if (player.TotalAttack >= 450)
            {
                Console.WriteLine($"Character: {player.Type}, Attack: {player.TotalAttack} and the player heals for 20% of its attack, Life: {player.TotalLife}.");
            }
            else
            {
                Console.WriteLine($"Character: {player.Type}, Attack: {player.TotalAttack}, Life: {player.TotalLife}.");
            }
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
                map[i, j] = '_';
            }
        }

        PlaceMonsters(10);
        PlaceBoss(3);
        PlaceTreasures(6);
        PlaceChests(5); // Place 5 chests on the map
    }

    static void PlaceMonsters(int count)
    {
        List<Monster> monsterList = new List<Monster>
        {
            Monsters.Slime,
            Monsters.Imp,
            Monsters.BloodWarrior,
            Monsters.Orc,
            Monsters.Vampire
        };

        for (int i = 0; i < count; i++)
        {
            int x, y;
            Monster monster = monsterList[rand.Next(monsterList.Count)];
            do
            {
                x = rand.Next(25);
                y = rand.Next(25);
            }
            while (monsterLocations[x, y] != null);
            monsterLocations[x, y] = monster;
        }
    }

    static void PlaceBoss(int count)
    {
        List<Boss> bossList = new List<Boss>
        {
            Bosses.UndeadDragon,
            Bosses.Cerberus,
            Bosses.VexarTheFearDevil
        };

        for (int i = 0; i < count; i++)
        {
            int x, y;
            Boss boss = bossList[rand.Next(bossList.Count)];
            do
            {
                x = rand.Next(25);
                y = rand.Next(25);
            }
            while (bossLocations[x, y] != null);
            bossLocations[x, y] = boss;
        }
    }

    static void PlaceTreasures(int count)
    {
        PlaceItems(count, treasureLocations);
    }

    static void PlaceChests(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int x, y;
            do
            {
                x = rand.Next(25);
                y = rand.Next(25);
            }
            while (weaponLocations[x, y] != null || armorLocations[x, y] != null || map[x, y] == '~' || map[x, y] == '^');

            if (rand.Next(2) == 0)
            {
                weaponLocations[x, y] = new Weapon("Sword", rand.Next(150, 401));
            }
            else
            {
                armorLocations[x, y] = new Armor("Shield", rand.Next(1000, 4001));
            }
        }
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
            case '1': return new Character("Warrior", 100, 3000);
            case '2': return new Character("Mage", 120, 2500);
            case '3': return new Character("Rogue", 140, 2300);
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
                else if (monsterLocations[i, j] != null)
                    Console.Write('M');
                else if (bossLocations[i, j] != null)
                    Console.Write('B');
                else if (weaponLocations[i, j] != null || armorLocations[i, j] != null)
                    Console.Write('C'); // Display chests as 'C'
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
        if (monsterLocations[playerX, playerY] != null)
        {
            Fight(monsterLocations[playerX, playerY]);
            monsterLocations[playerX, playerY] = null; // Remove the monster after the fight
        }
        else if (bossLocations[playerX, playerY] != null)
        {
            FightBoss(bossLocations[playerX, playerY]);
            bossLocations[playerX, playerY] = null; // Remove the boss after the fight
        }
        else if (treasureLocations[playerX, playerY])
        {
            Console.WriteLine("You found a treasure!");
            treasureLocations[playerX, playerY] = false;
        }
        else if (weaponLocations[playerX, playerY] != null)
        {
            Weapon foundWeapon = weaponLocations[playerX, playerY];
            Console.WriteLine($"You found a weapon: {foundWeapon.Name} with {foundWeapon.AttackBoost} attack boost.");
            Console.WriteLine("Do you want to swap it with your current weapon? (y/n)");
            if (Console.ReadKey().KeyChar == 'y')
            {
                player.EquippedWeapon = foundWeapon;
            }
            weaponLocations[playerX, playerY] = null;
        }
        else if (armorLocations[playerX, playerY] != null)
        {
            Armor foundArmor = armorLocations[playerX, playerY];
            Console.WriteLine($"You found an armor: {foundArmor.Name} with {foundArmor.LifeBoost} life boost.");
            Console.WriteLine("Do you want to swap it with your current armor? (y/n)");
            if (Console.ReadKey().KeyChar == 'y')
            {
                player.EquippedArmor = foundArmor;
            }
            armorLocations[playerX, playerY] = null;
        }
    }

    static void Fight(Monster monster)
    {
        Console.WriteLine($"You encountered a {monster.Name}!\nIt's passive abilities: {monster.Skills}");
        if (monster.Name == "Slime")
        {
            while (player.Life > 0 && monster.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Monster HP: {monster.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {monster.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {monster.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine("You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                }

                monster.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (monster.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {monster.Name}!");
                    break;
                }

                double monsterDamage = monster.Attack;
                player.Life -= monsterDamage;
                Console.WriteLine($"The {monster.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
        else if (monster.Name == "Blood Warrior")
        {
            while (player.Life > 0 && monster.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Monster HP: {monster.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {monster.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {monster.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine("You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                }

                monster.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (monster.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {monster.Name}!");
                    break;
                }

                double monsterDamage = monster.Attack + rand.Next(1, 7);
                player.Life -= monsterDamage;
                monster.HP += monsterDamage * 0.5;
                Console.WriteLine($"The {monster.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
        else if (monster.Name == "Orc")
        {
            while (player.Life > 0 && monster.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Monster HP: {monster.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack * 0.9 + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {monster.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {monster.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine("You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack * 0.9 + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack * 0.9 + rand.Next(1, 7);
                }

                int dodge = rand.Next(0, 101);
                if (dodge <= 5)
                {
                    Console.WriteLine($"The {monster.Name} dodged your attack.");
                }
                else
                {
                    monster.HP -= playerDamage;
                    if (playerDamage > 450)
                    {
                        player.Life = playerDamage * 0.2;
                    }
                }


                if (monster.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {monster.Name}!");
                    break;
                }

                double monsterDamage = monster.Attack + rand.Next(1, 7);
                player.Life -= monsterDamage;
                Console.WriteLine($"The {monster.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
        else if (monster.Name == "Vampire")
        {
            while (player.Life > 0 && monster.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Monster HP: {monster.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {monster.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {monster.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine("You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                }

                monster.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (monster.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {monster.Name}!");
                    break;
                }

                double monsterDamage = monster.Attack + rand.Next(1, 7);
                player.Life -= monsterDamage;
                monster.AP += 6;
                if (monster.AP % 20 == 0)
                {
                    monster.HP += monster.HP * (0.1 + monster.AP / 20 / 100);
                }
                else
                {
                    monster.HP += monster.HP * 0.1;
                }
                Console.WriteLine($"The {monster.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
        else
        {
            while (player.Life > 0 && monster.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Monster HP: {monster.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {monster.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {monster.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine("You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                }

                monster.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (monster.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {monster.Name}!");
                    break;
                }

                double monsterDamage = monster.Attack + rand.Next(1, 7);
                player.Life -= monsterDamage;
                Console.WriteLine($"The {monster.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
    }


    static void FightBoss(Boss boss)
    {
        Console.WriteLine($"You encountered a {boss.Name}!\nIt's skills: {boss.Skills}");
        if (boss.Name == "Vexar the Fear Devil")
        {

            int attackCount = 0;
            while (player.Life > 0 && boss.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Boss HP: {boss.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;

                if (attackCount % 6 == 0)
                {
                    Console.WriteLine("You are stunned so you will not attack this turn.");
                }
                else
                {
                    if (input == 'q')
                    {
                        playerDamage = player.TotalAttack * 0.85 + rand.Next(1, 7);
                        Console.WriteLine($"You attacked the {boss.Name} for {playerDamage} damage.");
                    }
                    else if (input == 'e')
                    {
                        if (player.Skills.Count > 0)
                        {
                            Console.WriteLine("Choose a skill:");
                            for (int i = 0; i < player.Skills.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                            }
                            int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                            playerDamage = player.UseSkill(skillIndex);
                            Console.WriteLine($"You used {player.Skills[skillIndex]} on the {boss.Name} for {playerDamage} damage.");
                        }
                        else
                        {
                            Console.WriteLine("You have no skills to use. Defaulting to attack.");
                            playerDamage = player.TotalAttack * 0.85 + rand.Next(1, 7);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Defaulting to attack.");
                        playerDamage = player.TotalAttack * 0.85;
                    }
                }


                boss.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (boss.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {boss.Name}!");
                    break;
                }

                if (boss.HP <= 666)
                {
                    double monsterDamage = boss.Attack * 2 + rand.Next(1, 7);
                    player.Life -= monsterDamage;
                    Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage.");
                    attackCount++;
                }
                else
                {
                    double monsterDamage = boss.Attack + rand.Next(1, 7);
                    player.Life -= monsterDamage;
                    Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage.");
                    attackCount++;
                }


                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }

        else if (boss.Name == "Cerberus")
        {

            while (player.Life > 0 && boss.HP > 0)
            {
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Boss HP: {boss.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {boss.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {boss.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine($"You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack;
                }

                boss.HP -= playerDamage;
                if (playerDamage > 450)
                {
                    player.Life = playerDamage * 0.2;
                }

                if (boss.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {boss.Name}!");
                    break;
                }

                double monsterDamage = boss.Attack * 3 + rand.Next(1, 7);
                player.Life -= monsterDamage;
                player.TotalLife -= 9;
                Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage. You lost 3hp permanently.");
                Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage. You lost 3hp permanently.");
                Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage. You lost 3hp permanently.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }

        else
        {
            bool poison = false;
            while (player.Life > 0 && boss.HP > 0)
            {
                if (poison)
                {
                    Console.WriteLine($"The poison is active. You will be damaged for {player.TotalLife * 0.1}");
                    player.Life -= player.TotalLife * 0.1;
                }
                Console.WriteLine($"Player HP: {player.TotalLife:.}, Boss HP: {boss.HP:.}");
                Console.WriteLine("Choose your action: (q) Attack, (e) Use Skill");
                char input = Console.ReadKey().KeyChar;

                double playerDamage = 0;
                if (input == 'q')
                {
                    playerDamage = player.TotalAttack + rand.Next(1, 7);
                    Console.WriteLine($"You attacked the {boss.Name} for {playerDamage} damage.");
                }
                else if (input == 'e')
                {
                    if (player.Skills.Count > 0)
                    {
                        Console.WriteLine("Choose a skill:");
                        for (int i = 0; i < player.Skills.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}: {player.Skills[i]}");
                        }
                        int skillIndex = int.Parse(Console.ReadKey().KeyChar.ToString()) - 1;
                        playerDamage = player.UseSkill(skillIndex);
                        Console.WriteLine($"You used {player.Skills[skillIndex]} on the {boss.Name} for {playerDamage} damage.");
                    }
                    else
                    {
                        Console.WriteLine($"You have no skills to use. Defaulting to attack.");
                        playerDamage = player.TotalAttack + rand.Next(1, 7);
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid choice. Defaulting to attack.");
                    playerDamage = player.TotalAttack;
                }

                boss.HP -= playerDamage;
                if (player.Attack >= 450)
                {
                    player.Life = playerDamage * 0.2;
                }
                if (boss.HP <= boss.HP * 0.8)
                {
                    boss.HP += boss.HP * 0.25;
                    Console.WriteLine($"The {boss.Name} regenerated 25% of it's healt to go all out.");
                }

                if (boss.HP <= 0)
                {
                    Console.WriteLine($"You defeated the {boss.Name}!");
                    break;
                }

                double monsterDamage = boss.Attack + rand.Next(1, 7);
                player.Life -= monsterDamage;
                poison = true;
                Console.WriteLine($"The {boss.Name} attacked you for {monsterDamage} damage.");

                if (player.Life <= 0)
                {
                    Console.WriteLine("You have died. Game over!");
                    Environment.Exit(0);
                }
            }
        }
    }
}



public class Character
{
    public string Type { get; set; }
    public double Attack { get; set; }
    public double Life { get; set; }
    public List<string> Skills { get; set; }
    public Weapon EquippedWeapon { get; set; }
    public Armor EquippedArmor { get; set; }

    public Character(string type, double attack, double life)
    {
        Type = type;
        Attack = attack;
        Life = life;
        Skills = new List<string>();
        EquippedWeapon = null;
        EquippedArmor = null;
    }

    public double UseSkill(int skillIndex)
    {
        if (skillIndex < 0 || skillIndex >= Skills.Count)
            throw new ArgumentException("Invalid skill index.");

        // Skill logic here; for now we return a fixed value for simplicity
        return Attack; // This is an example, customize as needed
    }

    public double TotalAttack
    {
        get
        {
            return Attack + (EquippedWeapon?.AttackBoost ?? 0);
        }
    }

    public double TotalLife
    {
        get
        {
            return Life + (EquippedArmor?.LifeBoost ?? 0);
        }
        set { Life = value; }
    }
}

public class Weapon
{
    public string Name { get; set; }
    public double AttackBoost { get; set; }

    public Weapon(string name, double attackBoost)
    {
        Name = name;
        AttackBoost = attackBoost;
    }
}

public class Armor
{
    public string Name { get; set; }
    public double LifeBoost { get; set; }

    public Armor(string name, double lifeBoost)
    {
        Name = name;
        LifeBoost = lifeBoost;
    }
}


namespace DungeonRush
{
    public class Monster
    {
        public string Name { get; set; }
        public double HP { get; set; }
        public double Attack { get; set; }
        public int AP { get; set; }
        public string Skills { get; set; }
        public double OriginalHP { get; private set; }
        public double OriginalAttack { get; private set; }
        public int OriginalAP { get; private set; }

        public Monster()
        {
            OriginalHP = HP;
            OriginalAttack = Attack;
            OriginalAP = AP;
        }

        public void ResetStats()
        {
            HP = OriginalHP;
            Attack = OriginalAttack;
            AP = OriginalAP;
        }
    }

    public static class Monsters
    {
        public static Monster Slime = new Monster
        {
            Name = "Slime",
            HP = 1000,
            Attack = 30,
            Skills = "none"
        };

    public static Monster Imp = new Monster
    {
        Name = "Imp",
        HP = 1500,
        Attack = 50,
        Skills = "none"
    };

    public static Monster BloodWarrior = new Monster
    {
        Name = "Blood Warrior",
        HP = 2500,
        Attack = 100,
        Skills = "On hit the unit heals for half of its ATP."
    };



        

        public static Monster Orc = new Monster
        {
            Name = "Orc",
            HP = 4000,
            Attack = 120,
            Skills = "The Orc takes 10% less damage from Attack. The Orc has a 5% chance to dodge an attack."
        };

        public static Monster Vampire = new Monster
        {
            Name = "Vampire",
            HP = 3200,
            Attack = 100,
            AP = 10,
            Skills = "The Vampire heals 1% + (1% for every 20 AP) max healt at the end of its strike. When the Vampire strikes an opponent it raises its AP by 6."
        };
    }
}

public class Boss
{
    public string Name { get; set; }
    public double HP { get; set; }
    public double Attack { get; set; }
    public string Skills { get; set; }
}

public static class Bosses
{
    public static Boss UndeadDragon = new Boss
    {
        Name = "Undead Dragon",
        HP = 5000,
        Attack = 150,
        Skills = "If its health drops below 8% it will regenerate 25% of its max health. With its attack it puts poison on the target that will deal 1% of the player's max HP."
    };

    public static Boss Cerberus = new Boss
    {
        Name = "Cerberus",
        HP = 5500,
        Attack = 60,
        Skills = "Cerberus will attack 3 times and heals for 20% of the damage dealt. Cerberus' attacks permanently take 3 HP from the player's max health."
    };

    public static Boss VexarTheFearDevil = new Boss
    {
        Name = "Vexar the Fear Devil",
        HP = 6666,
        Attack = 250,
        Skills = "Gives the player -15% ATP and AP. At 666 HP it doubles its ATP. Every 6th attack stuns the player."
    };
}

