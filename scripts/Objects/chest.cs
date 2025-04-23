using DarkRequiem.manager;
using DarkRequiem.player;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkRequiem.objects
{
    public class Chest
    {
        public int Id { get; private set; }
        public int Colonne { get; set; }
        public int Ligne { get; set; }
        public string MapName { get; private set; }
        public bool IsOpened { get; private set; } = false;
        public Objects Contenu { get; private set; }

        public IEventCommand? OnOpenEvent { get; set; }

        public Chest(int id, int col, int lig, string mapName, Objects contenu)
        {
            Id = id;
            Colonne = col;
            Ligne = lig;
            MapName = mapName;
            Contenu = contenu;
        }

        public void Open(Player player)
        {
            if (IsOpened) return;

            AudioManager.Play("OpenChest");

            if (Contenu is Potion potion)
            {
                InventoryHelper.AddPotion(ref player, 1);
                //NotificationManager.Add("+1 Potion !");
                AudioManager.Play("ItemAcquire");
            }
            else if (Contenu is Money money)
            {
                InventoryHelper.AddMoney(ref player, money.moneyAdded);
                //NotificationManager.Add($"+{money.moneyAdded} Or");
                AudioManager.Play("ItemAcquire");
            }
            else if (Contenu is QuestObject quest)
            {
                InventoryHelper.AddQuestItem(ref player, quest);
                //NotificationManager.Add($"Objet obtenu : {quest.Name}");
                AudioManager.Play("ItemAcquire");
            }

            IsOpened = true;

            OnOpenEvent?.Execute();
        }

        public static Texture2D TextureClosed;
        public static Texture2D TextureOpen;

        public static void Load()
        {
            TextureClosed = LoadTexture("assets/images/objects/chest_closed.png");
            TextureOpen = LoadTexture("assets/images/objects/chest_open.png");
        }

        public static void Unload()
        {
            UnloadTexture(TextureClosed);
            UnloadTexture(TextureOpen);
        }
    }
}
