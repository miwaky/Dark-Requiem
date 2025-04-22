using DarkRequiem.objects;
using DarkRequiem.player;

public class Potion : Objects
{
    public int heal { get; private set; }

    public Potion(int pId, string pName, string pType, int pSpriteId, int pHeal, int pColonne, int pLigne)
        : base(pId, pName, pType, pSpriteId)
    {
        colonne = pColonne;
        ligne = pLigne;
        heal = pHeal;
    }

    public static Potion GenerateHeal(int colonne, int ligne)
    {
        return new Potion(1, "Potion", "potion", 4, 4, colonne, ligne);
    }

    public static int HealPlayer(ref Player player, int pPdvHeal)
    {
        int currentHp = player.Hp;
        int maxHp = player.MaxHp;

        if (currentHp == maxHp)
        {
            //Console.WriteLine("La vie est déjà au maximum !");
            return currentHp;
        }

        currentHp += pPdvHeal;

        if (currentHp > maxHp)
        {
            currentHp = maxHp; //  Ne pas dépasser la vie maximale
        }

        player.Hp = currentHp;
        //Console.WriteLine($"Le joueur récupère {pPdvHeal} PV ! PV actuels : {player.Hp}/{player.MaxHp}");

        return player.Hp;
    }
}