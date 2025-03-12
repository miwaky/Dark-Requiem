namespace DarkRequiem.npc
{

    public class CharacterManager
    {
        public string Name { get; set; }
        public int HP { get; set; }
        public int MaxHP { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Type { get; set; }

        public CharacterManager(string pname, string ptype, int php, int pattack, int pdefense)
        {
            Name = pname;
            Type = ptype;
            MaxHP = php;
            HP = php;
            Attack = pattack;
            Defense = pdefense;
        }

        public bool IsAlive()
        {
            return HP > 0;
        }

        public void TakeDamage(int damage)
        {
            int actualDamage = Math.Max(0, damage - Defense);
            HP -= actualDamage;
            HP = Math.Max(0, HP);
        }

        public int DealDamage()
        {
            return Attack;
        }
    }
}
