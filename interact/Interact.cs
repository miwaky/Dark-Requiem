using System.Threading.Tasks;
using Raylib_cs;
using static Raylib_cs.Raylib;
using DarkRequiem.npc;

namespace DarkRequiem.interact
{

    class InteractNpc
    {
        public int IdDialog { get; private set; }
        public string? Dialog { get; private set; }
        public string NpcName { get; private set; }
        public string? DialogNpc { get; private set; }
        public string? colonne { get; private set; }

        public string? ligne { get; private set; }

        //Variable du dictionnaire de dialogue.
        public InteractNpc(int pidDialog, string pnpcName, string pdialog)
        {
            IdDialog = pidDialog;
            NpcName = pnpcName;
            Dialog = pdialog;
        }
        //Dictionnaire avec les différents dialogues du jeu.
        public static Dictionary<int, InteractNpc> DialogDictionary = new Dictionary<int, InteractNpc>
        {

                 { 1, new InteractNpc(1, "King","Hello warrior ! Go kill the evil guy")}

        };
        //Fonction pour Initialiser le dialogue quand on est en état de collision avec un Npc
        public static string? InitTalk(Npc collidedNpc)
        {
            foreach (var entry in InteractNpc.DialogDictionary)
            {
                InteractNpc interactNpc = entry.Value;

                if (interactNpc.NpcName == collidedNpc.Name)
                {
                    string DialogNpc = interactNpc.Dialog;
                    int colonne = collidedNpc.Colonne;
                    int ligne = collidedNpc.Ligne;
                    ShowTalk(DialogNpc, colonne, ligne);
                    return DialogNpc; // Retourne le dialogue pour l'affichage
                }
            }
            return null; // Retourne null si aucun dialogue n'est trouvé
        }
        //Fonction pour appelé un message au dessus de la tête du npc. 
        public static void ShowTalk(string DialogNpc, int colonne, int ligne)
        {
            if (!string.IsNullOrEmpty(DialogNpc))
            {
                int scale = 16; //  Si la taille de vos tuiles est 16x16 et scale = 4 dans DarkRequiem.cs, alors scale ici devrait être 64 (16 * 4)
                int textHeight = 4;
                int textWidth = MeasureText(DialogNpc, textHeight); // Calculer la largeur du texte

                // Appliquer l'échelle et centrer le texte
                DrawText(DialogNpc, colonne * scale - textWidth / 2, ligne * scale - textHeight * 2, textHeight, Color.White);
            }
        }
    }

    public class InteractDoor
    {
        public int IdDoor { get; private set; }
        public int TargetLigne { get; private set; }
        public int TargetColonne { get; private set; }
        public int OriginLigne { get; private set; }
        public int OriginColonne { get; private set; }
        public string OriginMap { get; private set; }
        public string TargetMap { get; private set; }
        public InteractDoor(int pIdInteractDoor, int pOriginLigne, int pOriginColonne, string pOriginMap, int pTargetLigne, int pTargetColonne, string pTargetMap)
        {
            IdDoor = pIdInteractDoor;
            OriginLigne = pOriginLigne;
            OriginColonne = pOriginColonne;
            OriginMap = pOriginMap;
            TargetLigne = pTargetLigne;
            TargetColonne = pTargetColonne;
            TargetMap = pTargetMap;
        }
        public static Dictionary<int, InteractDoor> GetDoorDictionary = new Dictionary<int, InteractDoor>
        {

                 { 1, new InteractDoor(1, 10, 5, "map_village", 8, 12, "map_village_basement")}, //Porte dans défini dans "village" qui amene au sous sol
        };


        public static void InitDoor(Player _player, Npc collision)
        {

        }
    }


}
