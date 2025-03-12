using System.Text.Json;

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
        public int firstgid { get; set; }  // Numéro du premier id dans ce tileset
        public string image { get; set; } = string.Empty; // Chemin de l'image du tileset
        public int imagewidth { get; set; } // Largeur de l'image du tileset
        public int imageheight { get; set; } // Hauteur de l'image du tileset
        public int tilewidth { get; set; } // Largeur d'une tuile
        public int tileheight { get; set; } // Hauteur d'une tuile
        public int tilecount { get; set; } // Nombre total de tuiles dans le tileset
        public int columns { get; set; } // Nombre de colonnes dans l'image du tileset
    }

    public class MapJSONDatas
    {
        public int width { get; set; }
        public int height { get; set; }
        public int tilewidth { get; set; }

        public List<Layer> layers { get; set; } = new List<Layer>();
        public List<Tileset> tilesets { get; set; } = new List<Tileset>();
    }

    public class Map
    {
        public string NomCarte { get; set; } = "undefined";
        public int Largeur { get; private set; }
        public int Hauteur { get; private set; }
        public int TailleTuile { get; private set; }
        public List<Layer> Calques { get; private set; } = new List<Layer>();
        public List<Tileset> Tilesets { get; private set; } = new List<Tileset>();

        //  Ajout d'un constructeur qui charge la carte depuis un fichier JSON
        public Map(string filePath)
        {
            Load(filePath);
        }

        public void Load(string filePath)
        {
            try
            {
                string contenuJSON = File.ReadAllText(filePath);
                MapJSONDatas? donneesMap = JsonSerializer.Deserialize<MapJSONDatas>(contenuJSON);
                if (donneesMap == null)
                {
                    throw new InvalidOperationException($"Contenu du JSON invalide : {filePath}");
                }

                NomCarte = Path.GetFileNameWithoutExtension(filePath); // 🔥 Déduit le nom de la carte
                Largeur = donneesMap.width;
                Hauteur = donneesMap.height;
                TailleTuile = donneesMap.tilewidth;
                Calques.AddRange(donneesMap.layers);
                Tilesets.AddRange(donneesMap.tilesets);

                Console.WriteLine($" Carte {NomCarte} chargée avec succès !");
            }
            catch (Exception e)
            {
                Console.WriteLine($" Erreur lors du chargement de la carte : {e.Message}");
            }
        }

        public int InfoTuilles(int colonne, int ligne, string nomLayer)
        {
            int index = ligne * Largeur + colonne;
            Layer? layer = Calques.Find(layer => layer.name == nomLayer);
            if (layer == null)
            {
                throw new KeyNotFoundException($"le calque {nomLayer}");
            }
            return layer.data[index];
        }

        public int ConvertirIDTuileEnIDReel(int idTuile)
        {
            Tileset? tileset = null;
            foreach (Tileset ts in Tilesets)
            {
                if (idTuile >= ts.firstgid && idTuile < ts.firstgid + ts.tilecount)
                {
                    tileset = ts;
                    break;
                }
            }
            if (tileset != null)
            {
                return idTuile - tileset.firstgid;
            }
            else return 0;
        }

    }
}