namespace DarkRequiem.npc
{

    public class Character
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int MaxHp { get; protected set; }
        public int Hp { get; set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }

        public Character(string name, string type, int maxHp, int hp, int attack, int defense)
        {
            Name = name;
            Type = type;
            MaxHp = maxHp;
            Hp = hp;
            Attack = attack;
            Defense = defense;
        }
        public bool IsAlive()
        {
            return Hp > 0;
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(0, damage - Defense);
            if (actualDamage == 0) actualDamage++;
            Hp -= actualDamage;
            Hp = Math.Max(0, Hp);
        }

        public int DealDamage()
        {
            return Attack;
        }
    }
}
