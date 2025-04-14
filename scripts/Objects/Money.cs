using DarkRequiem.objects;
using DarkRequiem.player;

public class Money : Objects
{
    public int moneyAdded { get; private set; }

    public Money(int pId, string pName, string pType, int pSpriteId, int pMoney, int pColonne, int pLigne)
        : base(pId, pName, pType, pSpriteId)
    {
        colonne = pColonne;
        ligne = pLigne;
        moneyAdded = pMoney;
    }

    public static Money GenerateMoney(int colonne, int ligne)
    {
        //id, Nom de la money, type, numéro sprite, prix
        return new Money(1, "GreenMoney", "money", 3, 5, colonne, ligne);
    }

    public static int AddMoney(ref Player player, int pMoney)
    {
        int currentMoney = player.MoneyInventory;
        int Newmoney = pMoney;

        currentMoney += Newmoney;

        player.MoneyInventory = currentMoney;

        return player.MoneyInventory;
    }
}