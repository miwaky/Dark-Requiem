using Raylib_cs;
using static Raylib_cs.Raylib;
using DarkRequiem.npc;
using DarkRequiem.map;
using DarkRequiem.manager;
using System;
using System.Collections.Generic;
namespace DarkRequiem.interact
{

    class InteractNpc
    {
        public int IdDialog { get; private set; }
        public string? Dialog { get; private set; }
        public string NpcName { get; private set; }
        public static string? CurrentDialog = null; //Stocke le texte affiché
        public static int DialogX = 0;
        public static int DialogY = 0;


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

                 { 1, new InteractNpc(1, "Guard","1,3,2")}

        };
        //Fonction pour Initialiser le dialogue quand on est en état de collision avec un Npc
        public static string? InitTalk(Npc collidedNpc)
        {
            foreach (var entry in InteractNpc.DialogDictionary)
            {
                InteractNpc interactNpc = entry.Value;

                if (interactNpc.NpcName == collidedNpc.Name)
                {
                    //Stocke le dialogue dans CurrentDialog pour l'afficher chaque frame
                    CurrentDialog = interactNpc.Dialog;
                    DialogX = collidedNpc.Colonne * 16;// Convertir en pixels
                    DialogY = (collidedNpc.Ligne - 1) * 16;// Dessiner au-dessus du PNJ

                    return CurrentDialog;
                }
            }
            return null; //Retourne null si aucun dialogue n'est trouvé
        }
        //Fonction pour appelé un message au dessus de la tête du npc.
        public static void ShowTalk()
        {
            if (!string.IsNullOrEmpty(CurrentDialog))
            {
                int textHeight = 4;
                int textWidth = MeasureText(CurrentDialog, textHeight); //Calculer la largeur du texte

                // Affiche le texte à chaque frame
                DrawText(CurrentDialog, DialogX - textWidth / 2, DialogY, textHeight, Color.White);
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
        public InteractDoor(int pIdInteractDoor, int pOriginColonne, int pOriginLigne, string pOriginMap, int pTargetColonne, int pTargetLigne, string pTargetMap)
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

             { 1, new InteractDoor(1, 25, 38, "forest", 40, 65, "dungeon")}, //Porte dans défini dans "forest" qui amene au donjon
             { 2, new InteractDoor(2, 40, 65, "dungeon", 25, 38, "forest")}, //Porte dans défini dans "dungeon" qui ramene à la foret
          // { 3, new InteractDoor(3, 40, 6, "forest", 39, 15, "dungeon")} //debug tp
   

        };
    }

    public class InteractObject
    {
        public int IdObject { get; private set; }
        public int ObjectLigne { get; private set; }
        public int ObjectColonne { get; private set; }
        public int ObjectQuantity { get; private set; }
        public string ObjectMap { get; private set; }
        public InteractObject(int pIdObject, int pObjectQuantity, int pObjectLigne, int pObjectColonne, string pObjectMap)
        {
            IdObject = pIdObject;
            ObjectQuantity = pObjectQuantity;
            ObjectLigne = pObjectLigne;
            ObjectColonne = pObjectColonne;
            ObjectMap = pObjectMap;

        }

    }


    public class BreakableObject
    {

        public static void HandleBreakableTile(MapInfo map, int col, int ligne)
        {

            map.SetTuileToZero(col, ligne, "Breakable");
            //Console.WriteLine("Objet cassé !");

            Random rand = new Random();
            int roll = rand.Next(0, 100);

            if (roll < 25)
            {
                roll = rand.Next(0, 2);
                if (roll == 1)
                {
                    Money money = Money.GenerateMoney(col, ligne);
                    GameManager.ActiveObjects.Add(money);
                }
                else
                {
                    Potion potion = Potion.GenerateHeal(col, ligne);
                    GameManager.ActiveObjects.Add(potion);
                }

            }
            else
            {
                //Console.WriteLine("Rien trouvé.");
            }
        }
    }
}



