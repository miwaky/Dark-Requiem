🎮 Game Design Document : Dark Requiem
📖 1. Présentation Générale
Titre du jeu : Dark Requiem

Genre : Action-aventure RPG, inspiré de Zelda-like

Plateforme : PC

Moteur / Technologie : C#, Raylib

Public cible : Adolescents et adultes amateurs de RPG et de jeux d’aventure rétro

Résumé court :
« Dark Requiem » est un jeu d’action-aventure RPG qui plonge le joueur dans une quête sombre et épique où il doit explorer, résoudre des énigmes, affronter des ennemis et surmonter des pièges pour sauver un monde en péril.

🎯 2. Concept et Objectifs du Jeu
2.1 Objectif du joueur :

Explorer différents environnements (forêt, donjon, village).

Résoudre des énigmes et activer des mécanismes complexes.

Collecter des objets (clés, potions, argent).

Combattre et vaincre des ennemis pour progresser.

2.2 Objectif du jeu :

Proposer un gameplay accessible et prenant.

Favoriser la réflexion avec des puzzles environnementaux.

Offrir des combats dynamiques avec une gestion intelligente des ressources.

Narration immersive qui motive le joueur à avancer.

📜 3. Scénario et Contexte Narratif
3.1 Introduction narrative :

« Dans un royaume dévasté par une ancienne malédiction, vous incarnez un héros choisi pour libérer la terre du mal. Traversez des forêts mystérieuses, explorez des donjons sombres, et trouvez la vérité derrière le Requiem qui plonge le monde dans l’ombre. »

3.2 Personnages principaux :

Héros : Le joueur, sans nom, représente l’espoir du royaume.

Roi : Guide initial du joueur, confie les premières quêtes.

Ennemis principaux : Slime, Bat, Mushroom (ennemis simples récurrents).

🕹️ 4. Gameplay
4.1 Contrôles :


Action	Touche
Déplacement	ZQSD ou Flèches
Attaque	Déplacement contre obstacle
Interaction / Action spéciale	Espace ou touche dédiée
Inventaire rapide	H (utilisation potion)
4.2 Mécaniques :

Déplacements : Par case avec gestion précise des collisions.

Combats : Simples et immédiats (attaque automatique en entrant en collision).

Enigmes :

Interrupteurs conditionnels

Objets interactifs (coffres, portes)

Inventaire : Centralisé pour argent, potions et objets clés.

Événements dynamiques : Spawn conditionnel d’ennemis, activation de pièges, portes, et mécanismes complexes.

4.3 Systèmes avancés :

Gestion d’événements par Commandes :

Chaque action clairement séparée (ouverture porte, spawn ennemis, etc.).

Utilisation du design pattern Command pour découpler le gameplay.

🌍 5. Environnements

Environnement	Description	Éléments clés
Forêt	Zone initiale, tutoriel léger, ennemis faibles	Arbres, obstacles cassables, interrupteurs cachés
Village	Hub de PNJ, commerces et quête principale initiale	Habitants, boutique
Donjon	Environnement complexe, énigmes et combats intensifs	Pièges, coffres, salles secrètes
⚔️ 6. Bestiaire

Nom	PV	Attaque	Défense	Comportement
Slime	1	1	0	Simple, poursuite
Mushroom	2	2	1	Aggressif, attaque directe
Bat	2	1	1	Rapide, déplacement erratique
🧩 7. Éléments interactifs
Coffres : Objets, argent, potions, clés

Switches : Activation/désactivation d’obstacles et pièges

Portes : Ouverture conditionnelle (clé ou interrupteur)

🎵 8. Audio

Type	Description
Musiques	Ambiance distincte par zone (forêt, donjon)
Effets sonores	Attaque, pas, ouverture coffre, activation interrupteur
⚙️ 9. Structure technique
Moteur basé sur Raylib en C#

Utilisation extensive de design pattern Command

Architecture claire :

Scene Manager

Input Manager

Event Manager

Audio Manager

Map Renderer (RenduMap)

🗺️ 10. Interface utilisateur (UI)
Vie (HP) sous forme de cœurs

Argent / Potion affichés clairement

Inventaire rapide et intuitif

📌 11. Exemple d'événement scripté complexe
Ouverture d’un coffre :

Apparition d’une tuile bloquant la sortie

Spawn conditionnel de 2 ennemis

Suppression de la tuile une fois les ennemis vaincus

Exemple technique :

csharp
Copier
Modifier
var chestTrapEvent = new CompositeCommand(new List<IEventCommand>
{
    new SpawnTileCommand(map, 15, 61, 2700, "SwitchObstacle"),
    new SpawnEnemyDirectCommand(slime),
    new SpawnEnemyDirectCommand(bat),
    waitKillCheck,
    removeTile
});
