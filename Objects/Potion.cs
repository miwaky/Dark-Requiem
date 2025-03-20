using DarkRequiem.objects;
using DarkRequiem.player;

public class Potion : Objects
{
    public int heal { get; private set; }

    public Potion(int pId, string pName, string pType, int pSpriteId, int pQuantity, int pHeal)
        : base(pId, pName, pType, pSpriteId, pQuantity)
    {
        heal = pHeal;
    }

    public static Dictionary<int, Potion> PotionDictionary = new Dictionary<int, Potion>
    {
        { 1, new Potion(1, "Potion faible", "potion", 4, 1, 100) },
        { 2, new Potion(2, "Potion puissante", "potion", 5, 1, 150) }
    };

    public static int HealPlayer(ref Player player, int pPdvHeal)
    {
        int currentHp = player.Hp;
        int maxHp = player.MaxHp;

        if (currentHp == maxHp)
        {
            Console.WriteLine("La vie est déjà au maximum !");
            return currentHp;
        }

        currentHp += pPdvHeal;

        if (currentHp > maxHp)
        {
            currentHp = maxHp; //  Ne pas dépasser la vie maximale
        }

        player.Hp = currentHp;
        Console.WriteLine($"Le joueur récupère {pPdvHeal} PV ! PV actuels : {player.Hp}/{player.MaxHp}");

        return player.Hp;
    }
}