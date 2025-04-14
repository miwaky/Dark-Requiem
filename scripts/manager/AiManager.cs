using DarkRequiem.map;
using DarkRequiem.npc;
using DarkRequiem.player;

namespace DarkRequiem.manager
{
    public static class AiManager
    {
        public static void UpdateBehavior(Npc npc, Player player, MapInfo map, List<Npc> allNpcs)
        {
            //Analyse de la distance Joueur -> Monstres
            int distanceToPlayer = Math.Abs(npc.Colonne - player.colonne) + Math.Abs(npc.Ligne - player.ligne);

            if (distanceToPlayer == 1) //Si l'ia est à coté du joueur, il l'attaque
            {
                Attack(npc, player);
            }
            else if (distanceToPlayer <= npc.AggroRange)
            {
                MoveUsingPathfinding(npc, player, map, allNpcs);
            }
            else
            {
                MoveRandom(npc, map, allNpcs);
            }
        }
        private static void MoveUsingPathfinding(Npc npc, Player player, MapInfo map, List<Npc> allNpcs)
        {
            var path = Pathfinder.FindPath(map, npc.Colonne, npc.Ligne, player.colonne, player.ligne);

            if (path.Count > 1)
            {
                var nextMove = path[1]; // path[0] est la position actuelle du NPC

                // Vérifie explicitement que la position suivante est valide (Movable=2 et aucun NPC)
                if (EstPositionValide(nextMove.X, nextMove.Y, map, allNpcs))
                {
                    npc.Colonne = nextMove.X;
                    npc.Ligne = nextMove.Y;
                }
                else
                {
                    //Console.WriteLine($"La position calculée ({nextMove.X}, {nextMove.Y}) n'est pas valide !");
                }
            }
        }



        private static void MoveRandom(Npc npc, MapInfo map, List<Npc> allNpcs)
        {
            Random rnd = new Random();
            List<(int, int)> directions = new()
            {
                (npc.Colonne - 1, npc.Ligne),
                (npc.Colonne + 1, npc.Ligne),
                (npc.Colonne, npc.Ligne - 1),
                (npc.Colonne, npc.Ligne + 1)
            };

            directions = directions.OrderBy(x => rnd.Next()).ToList();

            foreach (var dir in directions)
            {
                if (EstPositionValide(dir.Item1, dir.Item2, map, allNpcs))
                {
                    npc.Colonne = dir.Item1;
                    npc.Ligne = dir.Item2;
                    break;
                }
            }
        }

        private static void Attack(Npc npc, Player player)
        {
            Console.WriteLine($"{npc.Name} attaque {player.Name} !");
            player.TakeDamage(npc.DealDamage());
            Console.WriteLine($"{player.Name} a maintenant {player.Hp}/{player.MaxHp} HP.");
        }

        private static bool EstPositionValide(int col, int lig, MapInfo map, List<Npc> allNpcs)
        {
            if (col < 0 || lig < 0 || col >= map.Largeur || lig >= map.Hauteur)
                return false;

            var stopEnnemy = map.InfoTuilles(col, lig, "StopEnnemy");
            var breakable = map.InfoTuilles(col, lig, "Breakable");

            // Bloqué si StopEnnemy ou Breakable
            if (stopEnnemy == 1 || breakable != 0)
                return false;

            // Non marchable pour les NPC
            if (map.InfoTuilles(col, lig, "Movable") != 2)
                return false;

            // NPC déjà présent
            return !allNpcs.Any(npc => npc.Colonne == col && npc.Ligne == lig);
        }

    }


    public static class Pathfinder
    {
        private class Node
        {
            public int X, Y;
            public Node? Parent;
            public int G, H;
            public int F => G + H;

            public Node(int x, int y, Node? parent = null)
            {
                X = x;
                Y = y;
                Parent = parent;
            }
        }

        public static List<(int X, int Y)> FindPath(MapInfo map, int startX, int startY, int targetX, int targetY)
        {
            List<Node> openList = new();
            HashSet<(int, int)> closedSet = new();

            Node startNode = new Node(startX, startY);
            openList.Add(startNode);

            while (openList.Any())
            {
                Node currentNode = openList.OrderBy(n => n.F).First();
                openList.Remove(currentNode);
                closedSet.Add((currentNode.X, currentNode.Y));

                if (currentNode.X == targetX && currentNode.Y == targetY)
                {
                    return RetracePath(currentNode);
                }

                foreach (var neighbourPos in GetNeighbours(map, currentNode.X, currentNode.Y))
                {
                    if (closedSet.Contains(neighbourPos))
                        continue;

                    int tentativeG = currentNode.G + 1;

                    Node? neighbourNode = openList.FirstOrDefault(n => n.X == neighbourPos.Item1 && n.Y == neighbourPos.Item2);
                    if (neighbourNode == null)
                    {
                        neighbourNode = new Node(neighbourPos.Item1, neighbourPos.Item2, currentNode)
                        {
                            G = tentativeG,
                            H = Math.Abs(neighbourPos.Item1 - targetX) + Math.Abs(neighbourPos.Item2 - targetY)
                        };
                        openList.Add(neighbourNode);
                    }
                    else if (tentativeG < neighbourNode.G)
                    {
                        neighbourNode.Parent = currentNode;
                        neighbourNode.G = tentativeG;
                    }
                }
            }

            // Aucun chemin trouvé
            return new List<(int X, int Y)>();
        }

        private static List<(int, int)> RetracePath(Node node)
        {
            var path = new List<(int, int)>();
            Node current = node;
            while (current != null)
            {
                path.Add((current.X, current.Y));
                current = current.Parent;
            }
            path.Reverse();
            return path;
        }

        private static List<(int, int)> GetNeighbours(MapInfo map, int x, int y)
        {
            List<(int, int)> neighbours = new();

            var possibleMoves = new (int X, int Y)[]
            {
        (x - 1, y),
        (x + 1, y),
        (x, y - 1),
        (x, y + 1)
            };

            foreach (var move in possibleMoves)
            {
                if (move.X >= 0 && move.Y >= 0 && move.X < map.Largeur && move.Y < map.Hauteur)
                {
                    var tile = map.InfoTuilles(move.X, move.Y, "Movable");
                    var stopEnnemy = map.InfoTuilles(move.X, move.Y, "StopEnnemy");
                    var breakable = map.InfoTuilles(move.X, move.Y, "Breakable");

                    if (tile == 2 && stopEnnemy != 1 && breakable == 0)
                    {
                        neighbours.Add(move);
                    }
                }
            }

            return neighbours;
        }

    }
}