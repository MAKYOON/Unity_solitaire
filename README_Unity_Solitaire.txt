#FRANCOIS Olivier #Jeu Solitaire Unity

###########################################################################################

Exercice : Jeu Solitaire Unity

GENERAL :
Le jeu rencontre quelques bugs précisés plus bas mais semble tourner relativement bien !
Pour récupérer les cartes du talon, cliquez simplement sur ce dernier.

L'exercice a été très difficile mais extrêmement enrichissant ! Vous trouverez divers commentaires liés aux scripts plus bas dans ce TXT.

Par rapport à ce qui était demandé, je n'ai pas réussi à utiliser les animations pour faire identifier la fin du jeu. Je l'ai donc fait à l'aide de la fonction "HasWin" dans le GameManager.
Je n'ai pas non plus réussi à gérer le talon comme dans le jeu original. Le talon sort effectivement trois cartes mais la première recouvre la deuxième qui recouvre la troisième.
Ce choix a été fait pour éviter de tenter le joueur de déplacer les trois cartes du talon d'un coup (ce qui aurait été possible puisque seule les cartes retournées ne peuvent pas être déplacées).

Il reste également quelques bug que je n'ai pas réussi à résoudre.

Concernant les assets, je remercie Kévin pour m'avoir partagé les texture de ses cartes ! (auxquelles j'ai rajouté le logo ludus).


____KNOWNBUGS_____
-Il arrive que lorsqu'il reste une seule carte à poser, le jeu lance l'écran de victoire.
-Si l'on vise bien avec le curseur, il est possible de déplacer un roi sur l'origine d'une colonne même si cette dernière n'est pas vide.
-Quand on clique sur une carte du talon et qu'on la déplace à un endroit impossible, cette dernière revient sur le talon mais légèrement décalée. Il devient alors possible au joueur de prendre deux cartes du talon d'un coup.
-Si le talon est vide, il est possible d'y déposer un roi.


____DECKMANAGER____

	*LIGNE 12 : public Sprite[] sprites = new Sprite[52];
Deux problèmes ici.
-Ce tableau de sprite contient les différentes textures des cartes. Le problème est que pour remplir ce tableau, la seule solution que j'ai trouvé a été de le remplir à la main, case par case, dans l'inspector avec les différents sprites de mes cartes.
-Je me suis arrangé en le remplissant pour que le sprite contenu dans sprites[i] et la carte i crée dans mon script "CreateDeck" correspondent entre eux. Cela veut dire que si on inverse l'index de deux textures, la texture associée à la carte crée ne sera pas la bonne. C'est donc un peu fragile.


	*LIGNE 127 : for (int j = 0; j < 2 - i; j++)
La boucle utilisée est un for alors qu'il aurait été plus judicieux de la faire avec un while (puisque le nombre de passage dans la boucle n'est pas nécessairement 2). Ne trouvant pas comment écrire un while adapté à ce que je voulais faire, j'ai bricolé avec un break;.


____CARDMANAGER____
	*GENERAL
J'ai l'impression de ne pas avoir bien appréhender la façon de concevoir mes accesseurs/mutateurs. J'ai tenté de faire plus ou moins comme dans le TP du pong sur Pascal mais je crois que je me suis un peu embrouillé tout seul.


____DRAGGABLE & DROP ZONE____
Ces deux scripts marchent ensemble et gèrent le drag & drop du jeu :
-La première fonction appelée est OnBeginDrag dans le script Draggable. Elle sert à stocker le parent d'origine de la carte pour le retourner si jamais le drop n'est pas possible.
-La seconde fonction appelée est OnDrag, toujours dans le script Draggable. C'est grâce à elle que la carte suit le curseur tout au long du drag.
-La troisième fonction appelée OnDrop, dans le script DropZone. C'est cette fonction qui va déterminer si le drop est possible et s'il faut changer le parent de la carte.
-La dernière fonction appelée est OnEndDrag. Elle gère le décalage des cartes pour donner l'effet d'empilage.

	*LIGNE 45 : canReceive = ValueChecked(draggedCard, draggedCard);
On passe la même carte en argument deux fois. Le problème est que quand j'ai écrit la fonction ValueChecked, je pensais que les piles pouvaient recevoir n'importe quelle carte. Par conséquent, je ne vérifiais les valeurs que si la carte était dropée sur une autre carte.
Pour vérifier que la valeur de la carte qu'on drop soit correcte dans le cas où on la drop sur une pile vide sans avoir à écrire/réecrire une fonction, je passe donc draggedCard en argument là où la fonction attendrait la référence de la carte recevante.
Néanmoins, j'ai arrangé le script pour que ça ne pose pas de problème lors de l'éxécution : dans le cas d'un drop d'une carte sur une pile vide, la fonction n'utilisera à aucun moment la référence du premier argument. Le fait de passer l'argument est cependant nécessaire pour la fonction se lance (ValueChecked(null, draggedCard); ne marchant pas).

____GAMEMANAGER____

	*LIGNE 45 : float scoreBonus = ((-1000 / 240) * Mathf.Floor(timeSinceBegining) + 1000);
Cette ligne sert à déterminer le score en fonction du temps.
Pour cela, j'ai crée une fonction linéaire (y = ax + b) pour ne pas m'embêter, telle que :
-Le maximum que l'on puisse gagné soit 1000 (l'ordonné à l'origine est donc b = 1000)
-Quand on arrive à 300 secondes (5 minutes), le temps ne rapporte plus de score.
	-> Il fallait donc trouver a tq : 0 = a*240 + 1000
	<=> a = -1000/240
On a donc bien : score = score + "ax + b" où a = (-1000/240), x = Mathf.Floor(timeSinceBegining) et b = 1000.

	*LIGNES 70 - 104 : bool HasWin()
Je n'ai pas trouvé comment faire pour utiliser une animation qui identifie la fin du jeu, du coup j'ai écrit une fonction.
Le problème est que cette dernière est trèèèès chaotique ; elle contient entre autre 3 boucles imbriquées.
Elle semble de plus avoir une faille puisque pendant mes tests, j'ai rencontré un bug que je n'ai pas réussi à le reproduire : la fonction renvoie "TRUE" alors qu'il reste encore une carte à placer (et donc qu'une pile ne possède pas 13 cartes).

###########################################################################################