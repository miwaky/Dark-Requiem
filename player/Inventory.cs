using System;
using System.Collections.Generic;
using DarkRequiem.objects;
using DarkRequiem.player;
namespace DarkRequiem.manager
{
    public class Inventory
    {
        // Dictionnaire pour stocker les objets avec leur ID unique
        private Dictionary<int, Objects> items;

        public Inventory()
        {
            items = new Dictionary<int, Objects>();
        }

        // Ajouter un objet dans l'inventaire
        public void AddObject(Objects obj)
        {
            if (items.ContainsKey(obj.id))
            {
                // Si l'objet est déjà présent, on augmente la quantité
                items[obj.id].quantity += obj.quantity;
                Console.WriteLine($"Ajout de {obj.name}, quantité maintenant : {items[obj.id].quantity}");
            }
            else
            {
                // Sinon, on l'ajoute
                items[obj.id] = obj;
                Console.WriteLine($"{obj.name} ajouté à l'inventaire.");
            }
        }

        // Supprimer un objet
        public void RemoveObject(int id)
        {
            if (items.ContainsKey(id))
            {
                items.Remove(id);
                Console.WriteLine($"Objet ID {id} retiré de l'inventaire.");
            }
        }

        // Afficher l'inventaire
        public void ShowInventory()
        {
            Console.WriteLine("\nInventaire:");
            foreach (var item in items.Values)
            {
                Console.WriteLine($"- {item.name} (x{item.quantity})");
            }
        }

        // Utiliser un objet
        public void UseObject(int id, Player player)
        {
            if (items.ContainsKey(id))
            {
                var obj = items[id];
                if (obj is Potion potion)
                {
                    Potion.HealPlayer(ref player, potion.heal);
                    obj.quantity--;
                    if (obj.quantity <= 0)
                        RemoveObject(id);
                }
                else if (obj is Weapon weapon)
                {
                    Console.WriteLine($"Vous équipez {weapon.name} (Dégâts: {weapon.dgt})");
                }
            }
            else
            {
                Console.WriteLine("Objet non trouvé dans l'inventaire.");
            }
        }
    }
}