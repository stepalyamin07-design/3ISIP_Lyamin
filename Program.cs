using System;
using System.Collections.Generic;

namespace TextRoguelike
{
    public abstract class Item
    {
        public string Name { get; protected set; }

        public Item(string name)
        {
            Name = name;
        }
        public abstract void Use(Player player);
    }

    public class Weapon : Item
    {
        public int Attack { get; private set; }

        public Weapon(string name, int attack) : base(name)
        {
            Attack = attack;
        }

        public override void Use(Player player)
        {
            player.EquipWeapon(this);
        }

        public override string ToString()
        {
            return $"{Name} (Атака: {Attack})";
        }
    }
    public class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(string name, int defense) : base(name)
        {
            Defense = defense;
        }

        public override void Use(Player player)
        {
            player.EquipArmor(this);
        }

        public override string ToString()
        {
            return $"{Name} (Защита: {Defense})";
        }
    }
    public class HealthPotion : Item
    {
        public HealthPotion() : base("Лечебное зелье") { }

        public override void Use(Player player)
        {
            player.HealFull();
            Console.WriteLine("Вы выпили лечебное зелье и полностью восстановили здоровье!");
        }
    }
    public abstract class Enemy
    {
        public string Name { get; protected set; }
        public int MaxHP { get; protected set; }
        public int CurrentHP { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        protected Random random;

        public Enemy(string name, int hp, int attack, int defense)
        {
            Name = name;
            MaxHP = hp;
            CurrentHP = hp;
            Attack = attack;
            Defense = defense;
            random = new Random();
        }

        public virtual void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
        }

        public virtual int CalculateDamage(Player player)
        {
            return Attack;
        }

        public virtual void ApplySpecialEffect(Player player) { }

        public virtual string GetBattleInfo()
        {
            return $"{Name} (HP: {CurrentHP}/{MaxHP}, Атака: {Attack}, Защита: {Defense})";
        }

        public bool IsAlive => CurrentHP > 0;
    }


    public class Goblin : Enemy
    {
        private double critChance = 0.2;

        public Goblin() : base("Гоблин", 30, 8, 3) { }

        public override int CalculateDamage(Player player)
        {
            if (random.NextDouble() < critChance)
            {
                Console.WriteLine("Гоблин наносит критический удар!");
                return (int)(Attack * 1.5);
            }
            return Attack;
        }
    }


    public class Skelet : Enemy
    {
        public Skelet() : base("Скелет", 25, 10, 2) { }

        public override int CalculateDamage(Player player)
        {
            return Attack;
        }
    }
    public class Mage : Enemy
    {
        private double freezeChance = 0.25;

        public Mage() : base("Маг", 20, 12, 1) { }

        public override void ApplySpecialEffect(Player player)
        {
            if (random.NextDouble() < freezeChance)
            {
                player.ApplyFreeze();
                Console.WriteLine("Маг заморозил вас! Вы пропустите следующий ход.");
            }
        }
    }
    public class GoblinBoss : Enemy
    {
        private double critChance = 0.3;

        public GoblinBoss() : base("ВВГ (Босс Гоблин)", 60, 12, 4) { }

        public override int CalculateDamage(Player player)
        {
            if (random.NextDouble() < critChance)
            {
                Console.WriteLine("ВВГ наносит сокрушительный критический удар!");
                return (int)(Attack * 1.5);
            }
            return Attack;
        }
    }


    public class SkeletonBoss : Enemy
    {
        public SkeletonBoss() : base("Ковальский (Босс Скелет)", 63, 13, 3) { }

        public override int CalculateDamage(Player player)
        {

            return Attack;
        }
    }


    public class MageBoss : Enemy
    {
        private double freezeChance = 0.35;

        public MageBoss() : base("Архимаг C++ (Босс Маг)", 36, 19, 1) { }

        public override void ApplySpecialEffect(Player player)
        {
            if (random.NextDouble() < freezeChance)
            {
                player.ApplyFreeze();
                Console.WriteLine("Архимаг C++ заморозил вас магией компиляции! Вы пропустите следующий ход.");
            }
        }
    }
    public class SkeletonBoss2 : Enemy
    {
        private double freezeChance = 0.4;

        public SkeletonBoss2() : base("Пестов С-- (Босс Скелет)", 33, 18, 1) { }

        public override int CalculateDamage(Player player)
        {

            return Attack;
        }

        public override void ApplySpecialEffect(Player player)
        {
            if (random.NextDouble() < freezeChance)
            {
                player.ApplyFreeze();
                Console.WriteLine("Пестов С-- заморозил вас своей леденящей душу улыбкой! Вы пропустите следующий ход.");
            }
        }
    }

    public class Player
    {
        public int MaxHP { get; private set; } = 100;
        public int CurrentHP { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        public Armor CurrentArmor { get; private set; }
        public bool IsFrozen { get; private set; }
        private Random random;

        public Player()
        {
            CurrentHP = MaxHP;
            CurrentWeapon = new Weapon("Ржавый меч", 5);
            CurrentArmor = new Armor("Кожанная куртка", 3);
            random = new Random();
        }

        public int GetAttack()
        {
            return CurrentWeapon?.Attack ?? 0;
        }

        public int GetDefense()
        {
            return CurrentArmor?.Defense ?? 0;
        }

        public void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
        }

        public void HealFull()
        {
            CurrentHP = MaxHP;
        }

        public void EquipWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            CurrentArmor = armor;
        }

        public void ApplyFreeze()
        {
            IsFrozen = true;
        }

        public void ClearFreeze()
        {
            IsFrozen = false;
        }

        public int CalculateDamage(Enemy enemy)
        {
            return GetAttack();
        }

        public int CalculateDefense(int incomingDamage)
        {
            int defenseValue = GetDefense();
            double blockPercentage = (random.Next(70, 101) / 100.0);
            int blockedDamage = (int)(defenseValue * blockPercentage);
            return Math.Max(0, incomingDamage - blockedDamage);
        }

        public string GetStatus()
        {
            return $"Игрок (HP: {CurrentHP}/{MaxHP}, Оружие: {CurrentWeapon}, Доспехи: {CurrentArmor})";
        }

        public bool IsAlive => CurrentHP > 0;
    }


    public class Game
    {
        private Player player;
        private Random random;
        private int turnCount;
        private List<Func<Enemy>> normalEnemies;
        private List<Func<Enemy>> bossEnemies;
        private List<Func<Item>> items;

        public Game()
        {
            player = new Player();
            random = new Random();
            turnCount = 0;

            InitializeEnemies();
            InitializeItems();
        }

        private void InitializeEnemies()
        {
            normalEnemies = new List<Func<Enemy>>
            {
                () => new Goblin(),
                () => new Skelet(),
                () => new Mage()
            };

            bossEnemies = new List<Func<Enemy>>
            {
                () => new GoblinBoss(),
                () => new SkeletonBoss(),
                () => new MageBoss(),
                () => new SkeletonBoss2()
            };
        }

        private void InitializeItems()
        {
            items = new List<Func<Item>>
            {
                () => new HealthPotion(),
                () => new Weapon("Стальной меч", random.Next(8, 12)),
                () => new Weapon("Острый кинжал", random.Next(6, 10)),
                () => new Weapon("Боевой топор", random.Next(10, 15)),
                () => new Armor("Кольчуга", random.Next(4, 7)),
                () => new Armor("Латные доспехи", random.Next(6, 10)),
                () => new Armor("Магический плащ", random.Next(3, 6))
            };
        }

        public void StartGame()
        {
            Console.WriteLine("Добро пожаловать в текстовую пошаговую игру-рогалик!");
            Console.WriteLine("Каждый ход вы будете встречать либо сундук, либо врага.");
            Console.WriteLine("Каждые 10 ходов вас ждет встреча с боссом!");
            Console.WriteLine();

            while (player.IsAlive)
            {
                turnCount++;
                Console.WriteLine($"=== Ход {turnCount} ===");
                Console.WriteLine(player.GetStatus());
                Console.WriteLine();
                if (player.IsFrozen)
                {
                    Console.WriteLine("Вы заморожены и пропускаете ход!");
                    player.ClearFreeze();
                    Console.WriteLine();
                    continue;
                }

                if (random.Next(2) == 0)
                {
                    EncounterChest();
                }
                else
                {
                    EncounterEnemy();
                }

                Console.WriteLine();

                if (!player.IsAlive)
                {
                    Console.WriteLine("Игра окончена! Вы погибли...");
                    break;
                }
                Console.Write("Нажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private void EncounterChest()
        {
            Console.WriteLine("Вы нашли сундук!");
            Item item = items[random.Next(items.Count)]();

            if (item is HealthPotion)
            {
                Console.WriteLine("В сундуке лечебное зелье!");
                item.Use(player);
            }
            else if (item is Weapon weapon)
            {
                Console.WriteLine($"В сундуке оружие: {weapon}");
                Console.WriteLine($"Ваше текущее оружие: {player.CurrentWeapon}");
                Console.Write("Хотите взять новое оружие? (y/n): ");

                if (Console.ReadLine().ToLower() == "y")
                {
                    weapon.Use(player);
                    Console.WriteLine($"Вы экипировали: {weapon}");
                }
                else
                {
                    Console.WriteLine("Вы оставили оружие в сундуке.");
                }
            }
            else if (item is Armor armor)
            {
                Console.WriteLine($"В сундуке доспехи: {armor}");
                Console.WriteLine($"Ваши текущие доспехи: {player.CurrentArmor}");
                Console.Write("Хотите взять новые доспехи? (y/n): ");

                if (Console.ReadLine().ToLower() == "y")
                {
                    armor.Use(player);
                    Console.WriteLine($"Вы экипировали: {armor}");
                }
                else
                {
                    Console.WriteLine("Вы оставили доспехи в сундуке.");
                }
            }
        }

        private void EncounterEnemy()
        {
            Enemy enemy;
            if (turnCount % 10 == 0)
            {
                Console.WriteLine("ВНИМАНИЕ! Появился БОСС!");
                enemy = bossEnemies[random.Next(bossEnemies.Count)]();
            }
            else
            {
                enemy = normalEnemies[random.Next(normalEnemies.Count)]();
            }

            Console.WriteLine($"Вы встретили: {enemy.GetBattleInfo()}");
            Console.WriteLine();

            Battle(enemy);
        }

        private void Battle(Enemy enemy)
        {
            while (player.IsAlive && enemy.IsAlive)
            {

                PlayerTurn(enemy);
                if (!enemy.IsAlive) break;

                EnemyTurn(enemy);
            }

            if (!enemy.IsAlive)
            {
                Console.WriteLine($"Вы победили {enemy.Name}!");
            }
        }

        private void PlayerTurn(Enemy enemy)
        {
            Console.WriteLine("Ваш ход:");
            Console.WriteLine("1 - Атаковать");
            Console.WriteLine("2 - Защищаться");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();

            if (choice == "1")
            {
                int damage = player.CalculateDamage(enemy);
                enemy.TakeDamage(damage);
                Console.WriteLine($"Вы нанесли {damage} урона {enemy.Name}!");
                Console.WriteLine($"{enemy.Name} (HP: {enemy.CurrentHP}/{enemy.MaxHP})");
            }
            else if (choice == "2")
            {

                Console.WriteLine("Вы готовитесь к защите...");

            }
            else
            {
                Console.WriteLine("Неверный выбор, вы пропускаете ход!");
            }

            Console.WriteLine();
        }

        private void EnemyTurn(Enemy enemy)
        {
            if (!enemy.IsAlive) return;

            Console.WriteLine($"Ход {enemy.Name}:");

            enemy.ApplySpecialEffect(player);


            if (player.IsFrozen)
            {
                return;
            }

            int damage = enemy.CalculateDamage(player);


            if (random.Next(100) < 40)
            {
                Console.WriteLine($"Вы увернулись от атаки {enemy.Name}!");
                return;
            }


            int finalDamage = player.CalculateDefense(damage);

            player.TakeDamage(finalDamage);
            Console.WriteLine($"{enemy.Name} наносит вам {finalDamage} урона!");
            Console.WriteLine($"Ваше HP: {player.CurrentHP}/{player.MaxHP}");

            Console.WriteLine();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Game game = new Game();
            game.StartGame();

            Console.WriteLine("Спасибо за игру!");
            Console.ReadKey();
        }
    }
}
