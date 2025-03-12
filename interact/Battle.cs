using DarkRequiem.npc;
using DarkRequiem.manager;

namespace DarkRequiem.interact
{
    public class Battle
    {
        public static void StartCombat(Player player, Npc enemy)
        {
            Console.WriteLine($"{player.Name} engage un combat contre {enemy.Name} sur la carte {enemy.MapName} !");

            while (player.IsAlive() && enemy.IsAlive())
            {
                Console.WriteLine($"{player.Name} attaque {enemy.Name} !");
                enemy.TakeDamage(player.DealDamage());
                Console.WriteLine($"{enemy.Name} a {enemy.HP}/{enemy.MaxHP} HP");

                if (!enemy.IsAlive())
                {
                    Console.WriteLine($"{enemy.Name} est vaincu sur {enemy.MapName} !");
                    GameManager.RemoveNpc(enemy.Id, enemy.MapName, enemy.Colonne, enemy.Ligne); // Supprime l'ennemi mort via GameManager
                    break;
                }

                Console.WriteLine($"{enemy.Name} riposte !");
                player.TakeDamage(enemy.DealDamage());
                Console.WriteLine($"{player.Name} a {player.HP}/{player.MaxHP} HP");

                if (!player.IsAlive())
                {
                    Console.WriteLine("Game Over !");
                    break;
                }
            }
        }
    }
}
