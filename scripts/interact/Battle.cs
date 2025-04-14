using DarkRequiem.npc;
using DarkRequiem.manager;
using DarkRequiem.player;
namespace DarkRequiem.interact
{
    public class Battle
    {

        public static void Attack(Player player, Npc enemy)
        {
            Console.WriteLine($"{player.Name} attaque {enemy.Name} !");
            enemy.TakeDamage(player.DealDamage());
            Console.WriteLine($"{enemy.Name} a maintenant {enemy.Hp}/{enemy.MaxHp} HP.");

            if (!enemy.IsAlive())
            {
                Console.WriteLine($"{enemy.Name} est vaincu !");
                GameManager.PendingKills.Add(enemy); // destruction plus tard
            }
        }
    }
}
