
using DarkRequiem.npc;
using Raylib_cs;
using static Raylib_cs.Raylib;


public class Player : CharacterManager
{
    public int colonne;
    public int ligne;
    public Texture2D Texture { get; private set; }
    public Player(string name, string type, int hp, int attack, int defense, int startCol, int startLigne)
        : base(name, type, hp, attack, defense)
    {
        colonne = startCol;
        ligne = startLigne;
        Texture = LoadTexture("assets/images/characters/player.png");
    }


    public static Player GeneratePlayer(int colonne, int ligne)
    {
        Player hero = new Player("Hero", "Player", 100, 15, 5, 31, 40);
        hero.colonne = colonne; // Utilisation des paramètres passés
        hero.ligne = ligne;
        return hero; // Retourne l'objet Player créé
    }

    public void Close()
    {
        UnloadTexture(Texture);
    }
}