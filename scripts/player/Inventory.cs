public class InventoryItem
{
    public string Id { get; set; }         // ex: "potion", "gold", "dungeon_key"
    public string Type { get; set; }       // ex: "consumable", "quest", "currency"
    public int Quantity { get; set; } = 1;

    public InventoryItem(string id, string type, int quantity)
    {
        Id = id;
        Type = type;
        Quantity = quantity;
    }
}

namespace DarkRequiem.objects
{
    public class Inventory
    {
        private readonly Dictionary<string, InventoryItem> items = new();

        // Ajoute un objet à l'inventaire
        public void AddItem(string id, string type, int amount = 1)
        {
            if (items.ContainsKey(id))
            {
                items[id].Quantity += amount;
            }
            else
            {
                items[id] = new InventoryItem(id, type, amount);
            }
        }

        // Vérifie si le joueur possède l'objet avec la quantité souhaitée
        public bool HasItem(string id, int amount = 1)
        {
            return items.TryGetValue(id, out var item) && item.Quantity >= amount;
        }

        // Supprime une certaine quantité d'un objet
        public void RemoveItem(string id, int amount = 1)
        {
            if (!items.ContainsKey(id)) return;

            items[id].Quantity -= amount;
            if (items[id].Quantity <= 0)
                items.Remove(id);
        }

        // Récupère la quantité d’un objet
        public int GetItemQuantity(string id)
        {
            return items.TryGetValue(id, out var item) ? item.Quantity : 0;
        }

        // Récupère tous les objets
        public List<InventoryItem> GetAllItems()
        {
            return items.Values.ToList();
        }

        // Optionnel : filtre par type (utile si tu veux afficher les objets de quête uniquement, par ex)
        public List<InventoryItem> GetItemsByType(string type)
        {
            return items.Values.Where(i => i.Type == type).ToList();
        }
    }
}
