using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//REMARQUE : Seul le talon possède un DeckManager.
public class DeckManager : MonoBehaviour
{
    [HideInInspector] public int m_DeckSize = 52;

    public Sprite[] sprites = new Sprite[52];

    public GameObject cardPrefab;
    public RectTransform parentToGo;

    public void CreateDeck()
    //BUT : Créer toutes les cartes.
    {
        //Initialisation de myColor et myValue pour être sûr de commencer au début des enum.
        int myColor = 0;
        int myValue = 1;

        //Pour chaque card du deck (0 à m_DeckSize - 1)
        for (int i = 0; i < m_DeckSize; i++)
        {
            //Transtypage de la couleur et de la valeur et affectation.
            CardManager.Card myCard = new CardManager.Card( (CardManager.CardValue)myValue, (CardManager.CardColor)myColor );

            //On incrémente d'abord la couleur pour faire tous les 2, puis tous les 3, puis...
            myColor++;
            
            //GetNames renvoie un tableau contenant les noms du type spécifié (qu'on  récupère grâce à typeof).
            //On prend donc la longueur de ce tableau pour savoir combien d'élément myColor comporte, et donc quand il faut le reset et passer à l'affectation de la valeur supérieure.
            if (myColor > Enum.GetNames(typeof(CardManager.CardColor)).Length - 1)
            {
                myColor = 0;
                myValue++;
            }

            //Instanciation de la carte. On lui attribue ensuite son parent, sa texture, son nom dans la hierarchie puis on s'assure qu'elle soit visible (=non retournée).
            GameObject cardInstance = Instantiate(cardPrefab);
            cardInstance.GetComponent<Transform>().SetParent(GameObject.FindGameObjectWithTag("Talon").transform, false);
            cardInstance.GetComponent<Image>().sprite = sprites[i];
            cardInstance.transform.name = "Card_" + myCard.value + "_" + myCard.color;
            cardInstance.GetComponent<CardManager>().hiddenImage.SetActive(false);

            //On sauvegarde les informations (couleur + valeur) de la carte grâce au composant CardManager (non statique) attaché sur chaque carte et à la fonction SetCard().
            cardInstance.GetComponent<CardManager>().SetCard(myCard);
        }
    }

    public void ShuffleDeck()
    //BUT : Mélanger le deck.
    //Puisque l'on ne cherchera qu'à accéder à la carte GetChild(0) du talon à chaque fois, il suffit de changer l'index des sibling.
    {
        //Pour i = 0; i < nb_d'enfants_du_talon (=m_DeckSize)
        for (int i = 0; i < GameObject.FindGameObjectWithTag("Talon").transform.childCount; i++)
        {
            //On récupère le transform de la carte i (= l'enfant i du talon) puis on change l'index de son sibling avec un nombre aléatoire entre 0 et le nb d'enfants du talon.
            Transform cardToMove = GameObject.FindGameObjectWithTag("Talon").transform.GetChild(i);
            cardToMove.transform.SetSiblingIndex(UnityEngine.Random.Range(0, GameObject.FindGameObjectWithTag("Talon").transform.childCount));
        }
    }

    public void DistribCards()
    //BUT : Faire la distribution des cartes (1 carte sur la première pile, deux sur la deuxième, trois sur la troisième...)
    //Dans cette fonction, la distribution se fait par "couches" : une carte pour les piles 0 à 6 puis une carte pour les piles 1 à 6 puis 2 à 6...
    {
        int nbStacks = 6; //6 au lieu de 7 car on commence la boucle à 0.

        for (int i = 0; i <= nbStacks; i++)
        {
            for (int j = i; j <= nbStacks; j++)
            {
                //On récupère la référence du transform de la première carte du talon (GetChild(0)) et le transform de la pile vers laquelle la carte sera envoyée.
                Transform cardToMove = GameObject.FindGameObjectWithTag("Talon").transform.GetChild(0);
                Transform stackToGo = GameObject.FindGameObjectWithTag("ColStack").transform.GetChild(j);

                //La carte placée quand i = j correspond à la dernière carte que la pile i recevra. Quand i!=j, il faut donc masquer la carte (en activant son hiddenImage).
                if (j != i)
                {
                    cardToMove.GetComponent<CardManager>().hiddenImage.SetActive(true);
                }
                
                //A l'étape i, chaque pile (sur laquelle une carte peut être déposée) a i-1 cartes. Il faut donc rentrer dans l'enfant de l'enfant (...) i-1 fois.  
                for (int k = 0; k <= i-1; k++)
                {
                    //Si le premier enfant est bien une carte c'est bon, on entre dedans (Getchild(0)).
                    if (stackToGo.GetChild(0).GetComponent<CardManager>() != null)
                    {
                        stackToGo = stackToGo.GetChild(0);
                    }
                    //Sinon, c'est que l'on est sûrement en train de déposer la carte sur une autre carte, auquel cas le premier enfant est (par défaut) la texture de la carte.
                    //Dans ce cas, il faut donc rentrer dans le second enfant (Getchild(1)).
                    else
                    {
                        stackToGo = stackToGo.GetChild(1);
                    }
                }
                
                //Finalement, on attribue le parent ciblé et on décale la carte pour l'effet d'empilage.
                cardToMove.SetParent(stackToGo);
                cardToMove.localPosition = new Vector2(0, -10);
            }
        }

    }

    public void GetCardsFromTalon()
    //BUT : Cette fonction est appelée par le bouton placé sur le talon.
    //A l'appel, cette fonction vide le slot à côté du talon s'il est rempli puis y place trois nouvelles cartes du talon.
    {
        //Référence du talon (talon) et du slot à côté du talon (talonSlot)
        GameObject talonSlot = GameObject.FindGameObjectWithTag("TalonSlot");
        GameObject talon = GameObject.FindGameObjectWithTag("Talon");

        //Vidage du slot à côté du talon
        for (int i = 0; i < 3; i++)
        {
            //Si le slot n'est pas vide, il faut remettre son contenu (forcément une carte) dans le talon en commençant par l'enfant le plus éloigné.
            //(Le plus éloigné pour ne pas envoyer dans le talon une carte étant elle-même parent d'une autre carte)
            if (talonSlot.transform.childCount > 0)
            {
                Transform myCard = talonSlot.transform.GetChild(0);
                //Il faudra répéter l'opération 2 fois max pour sortir les cartes une par une.
                for (int j = 0; j < 2 - i; j++)
                {
                    int k = 0;
                    //On balaye les enfants de la carte myCard pour vérifier que cette dernière n'est pas parente d'une carte.
                    while (k < myCard.childCount && myCard.GetChild(k).GetComponent<CardManager>() == null)
                    {
                        k++;
                    }
                    //Si elle a effectivement un enfant qui est une carte, alors cette carte devient myCard (=la carte que l'on veut ranger dans le talon)
                    if (k < myCard.childCount)
                    {
                        myCard = myCard.GetChild(k);
                    }
                    //Sinon, on passe à l'itération suivante.
                    else
                    {
                        break;
                    }
                }
                //Finalement, on attribue le parent selectionné et on déplace la carte. 
                myCard.SetParent(talon.transform);
                myCard.localPosition = Vector2.zero;
            }
        }

        //Placement des trois cartes du talon vers le talonSlot
        for (int i = 0; i < 3; i++)
        {
            //On récupère la carte à déplacer (toujours la 1ère du talon) et on initialise zoneToDrop sur le talonSlot.
            //zoneToDrop servira à définir où la carte sera posée (sur le talonSlot directement, sur un enfant, sur un enfant d'enfant...)
            Transform cardToMove = talon.transform.GetChild(0);
            Transform zoneToDrop = talonSlot.transform; 
            
            //Puisque l'on cherche toujours à empiler les cartes les unes sur les autres, il faudra rentrer i fois dans l'enfant de l'enfant (...)
            //j < i permet de s'assurer qu'on ne rentre pas dans la boucle si talonSlot ne contient pas encore de carte.
            for (int j = 0; j < i; j++)
            {
                int k = 0;
                //On balaye les enfants de zoneToDrop pour trouver une carte.
                while (zoneToDrop.GetChild(k).GetComponent<CardManager>() == null && k < zoneToDrop.childCount)
                {
                    k++;
                }
                //Puis une fois trouvée, cette carte devient notre zone de drop (=parent que l'on veut attribuer). 
                zoneToDrop = zoneToDrop.GetChild(k);
            }

            //Finalement, on attribue le parent selectionné et on déplace la carte. 
            cardToMove.SetParent(zoneToDrop);
            cardToMove.transform.localPosition = Vector2.zero;
        }
    }
}
