using System.Text.Json;
using DarkRequiem.npc;
using DarkRequiem.interact;

namespace DarkRequiem.map
{
    public class Layer
    {
        public string type { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public List<int> data { get; set; } = new List<int>();
    }

    public class Tileset
    {
        public int firstgid { get; set; }
        public string name { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public int imagewidth { get; set; }
        public int imageheight { get; set; }
        public int tilewidth { get; set; }
        public int tileheight { get; set; }
        public int tilecount { get; set; }
        public int columns { get; set; }
    }

    public class Ennemy
    {
        public int Id { get; set; }
        public string Type { get; set; } = "ennemy";
        public int Colonne { get; set; }
        public int Ligne { get; set; }
        public int SpriteID { get; set; }
        public int MaxHp { get; set; } = 1000;
        public int Hp { get; set; } = 1000;
        public int Attack { get; set; } = 5;
        public int Defense { get; set; } = 3;
        public string TextureType { get; set; } = "monster";
    }

    public class MapJSONDatas
    {
        public int width { get; set; }
        public int height { get; set; }
        public int tilewidth { get; set; }

        public List<Layer> layers { get; set; } = new();
        public List<Tileset> tilesets { get; set; } = new();
        public List<Ennemy> Ennemies { get; set; } = new();

    }

    public class MapInfo
    {
        public string NomCarte { get; set; } = "undefined";
        public int Largeur { get; private set; }
        public int Hauteur { get; private set; }
        public int TailleTuile { get; private set; }
        public List<Layer> Calques { get; private set; } = new();
        public List<Tileset> Tilesets { get; private set; } = new();
        public List<Ennemy> Ennemies { get; private set; } = new();

        public MapInfo(string filePath)
        {
            Load(filePath);
        }

        public void Load(string filePath)
        {
            MapJSONDatas? donneesMap;

            try
            {
                string contenuJSON = File.ReadAllText(filePath);
                donneesMap = JsonSerializer.Deserialize<MapJSONDatas>(contenuJSON);
                if (donneesMap == null)
                    throw new InvalidOperationException($"Contenu du JSON invalide : {filePath}");

                NomCarte = Path.GetFileNameWithoutExtension(filePath);
                Largeur = donneesMap.width;
                Hauteur = donneesMap.height;
                TailleTuile = donneesMap.tilewidth;
                Calques.AddRange(donneesMap.layers);
                Tilesets.AddRange(donneesMap.tilesets);


                // Charger les monstres
                LoadMonster(donneesMap);

                Console.WriteLine($" Carte {NomCarte} chargée avec succès !");

            }
            catch (Exception e)
            {
                Console.WriteLine($" Erreur lors du chargement de la carte : {e.Message}");
                return;
            }
        }

        //Methode pour chargé les monstres en récupérant les textures
        public void LoadMonster(MapJSONDatas donneesMap)
        {
            Layer? enemyLayer = donneesMap.layers.FirstOrDefault(l => l.name == "ennemy");
            if (enemyLayer != null)
            {
                int firstGid = donneesMap.tilesets.FirstOrDefault(t => t.name == "monster")?.firstgid ?? 1284;

                int id = 1;
                for (int i = 0; i < enemyLayer.data.Count; i++)
                {
                    int tileValue = enemyLayer.data[i];

                    if (tileValue != 0)
                    {
                        int colonne = i % Largeur;
                        int ligne = i / Largeur;
                        int spriteID = tileValue - firstGid;

                        Console.WriteLine($"tileValue: {tileValue}, firstGid: {firstGid}, spriteID: {spriteID}");


                        string type = NpcData.TypeParSpriteID.ContainsKey(spriteID)
     ? NpcData.TypeParSpriteID[spriteID]
     : "mushroom";


                        var stats = NpcData.StatsParType[type];

                        donneesMap.Ennemies.Add(new Ennemy
                        {
                            Id = id++,
                            Type = type,
                            Colonne = colonne,
                            Ligne = ligne,
                            SpriteID = spriteID,
                            MaxHp = stats.MaxHp,
                            Hp = stats.Hp,
                            Attack = stats.Attack,
                            Defense = stats.Defense,
                            TextureType = stats.TextureType
                        });

                        Console.WriteLine($"[NPC] tileValue={tileValue}, spriteID={spriteID}, type={type}");

                    }
                }

                this.Ennemies = donneesMap.Ennemies;
            }
        }
        public void SetTuileToZero(int colonne, int ligne, string nomLayer)
        {
            Layer? layer = Calques.Find(l => l.name == nomLayer);
            if (layer == null) return;

            int index = ligne * Largeur + colonne;
            if (index >= 0 && index < layer.data.Count)
            {
                layer.data[index] = 0;
            }
        }
        public int InfoTuilles(int colonne, int ligne, string nomLayer)
        {
            Layer? layer = Calques.Find(layer => layer.name == nomLayer);
            if (layer == null)
            {
                throw new KeyNotFoundException($"Le calque {nomLayer} n'existe pas !");
            }

            if (colonne < 0 || colonne >= Largeur || ligne < 0 || ligne >= Hauteur)
            {
                return -1;
            }

            int index = ligne * Largeur + colonne;

            return layer.data[index];
        }

        public int ConvertirIDTuileEnIDReel(int idTuile)
        {
            Tileset? tileset = Tilesets.FirstOrDefault(ts => idTuile >= ts.firstgid && idTuile < ts.firstgid + ts.tilecount);
            return tileset != null ? idTuile - tileset.firstgid : 0;
        }
    }
}