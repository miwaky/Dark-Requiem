
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;


namespace DarkRequiem.player
{

    public class Ui
    {
        public static Texture2D HeartEmpty { get; private set; }
        public static Texture2D Heart1 { get; private set; }

        public static Texture2D Heart2 { get; private set; }
        public static Texture2D Heart3 { get; private set; }
        public static Texture2D Heart4 { get; private set; }
        public static int NbrDeCoeurActif { get; set; }
        public static int NbrDeCoeur { get; set; }
        public static void TextureLoadUi()
        {
            HeartEmpty = LoadTexture("assets/images/Ui/Hearts_Red_5.png");
            Heart1 = LoadTexture("assets/images/Ui/Hearts_Red_4.png");
            Heart2 = LoadTexture("assets/images/Ui/Hearts_Red_3.png");
            Heart3 = LoadTexture("assets/images/Ui/Hearts_Red_2.png");
            Heart4 = LoadTexture("assets/images/Ui/Hearts_Red_1.png");

        }

        public static void UiPlayer(Player player)
        {

            // Nombre total de cœurs selon MaxHp (1 cœur = 4 points de vie)
            int NbrDeCoeur = player.MaxHp / 4;

            int vieRestante = player.Hp; // Vie restante du joueur
            int x = 0;

            for (int i = 0; i < NbrDeCoeur; i++)
            {
                Texture2D textureCoeur;

                // Calcule précisément l'état de chaque cœur :
                if (vieRestante >= 4)
                {
                    textureCoeur = Heart4;   // cœur complet
                    vieRestante -= 4;
                }
                else if (vieRestante == 3)
                {
                    textureCoeur = Heart3;   // 3/4 de cœur
                    vieRestante -= 3;
                }
                else if (vieRestante == 2)
                {
                    textureCoeur = Heart2;   // 1/2 cœur
                    vieRestante -= 2;
                }
                else if (vieRestante == 1)
                {
                    textureCoeur = Heart1;   // 1/4 de cœur
                    vieRestante -= 1;
                }
                else
                {
                    textureCoeur = HeartEmpty; // cœur vide
                }

                //Argent UI : 
                float scale = 2.0f;
                DrawTextureEx(textureCoeur, new Vector2(10 + x, 10), 0f, scale, Color.White);
                x += (int)(textureCoeur.Width * scale);

                DrawText("Argent : " + player.MoneyInventory, 10, 50, 20, Color.White);
                DrawText("Potion : " + player.PotionInventory, 10, 70, 20, Color.White);

            }
        }

    }
}