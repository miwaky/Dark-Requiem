using DarkRequiem.objects;
using DarkRequiem.player;


public class Money : Objects
{
    public int moneyAdded { get; private set; }
    public Money(int pId, string pName, string pType, int pSpriteId, int pMoney, int pColonne, int pLigne)
        : base(pId, pName, pType, pSpriteId)
    {
        moneyAdded = pMoney;
        colonne = pColonne;
        ligne = pLigne;
    }

    public static Money GenerateMoney(int colonne, int ligne)
    {
        return new Money(2, "Gold", "money", 3, 5, colonne, ligne);
    }

    public static void AddMoney(ref Player player, int pMoney)
    {
        player.Inventory.AddGold(pMoney);
        Console.WriteLine($"+{pMoney} or → total : {player.Inventory.Gold}");
    }
}
