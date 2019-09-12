using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public Sprite[] cardSprites;
    public GameObject cardPrefab;
    public GameObject[] stacks;
    public GameObject stockButton;

    int nbcards = 52;
    string[] colors = new string[] { "H", "D", "C", "S" };
    string[] values = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13" };

    public static List<Card>[] board = new List<Card>[7];
    protected List<Card> unshuffledDeck = new List<Card>();
    protected List<Card> shuffledDeck = new List<Card>();

    public List<Card> GetShuffledDeck()
    {
        return this.shuffledDeck;
    }
    void Awake()
    {
        shuffledDeck = GenerateShuffledDeck();
        BoardGeneration();
    }

    //Création du deck non mélangé
    public List<Card> GenerateDeck()
    {
        int i = 0;
        int j = 0;
        List<Card> unshuffledDeckTemp = new List<Card>();
        int k = 0 ; 
         //ici on associe chaque couleur i à la valeur j
         while (k < nbcards) 
         {
            Card myCard = new Card();
            string colorTemp = colors[i];
            string valueTemp = values[j];
            myCard.SetValue(int.Parse(valueTemp));
            if (colorTemp == "D" || colorTemp == "H")
                myCard.SetColor("Red");
            else
                myCard.SetColor("Black");
            myCard.SetName(colorTemp + valueTemp);
            unshuffledDeckTemp.Add(myCard);
            i++;
             if (i > 3)
             {
                 i = 0;
                 j++;
             }
             k++;
         }
         //assignation des sprites
        foreach (Card card in unshuffledDeckTemp)
        {
            i = 0;
            while (card.GetName() != cardSprites[i].name)
            {
                i++;
            }
            card.SetSprite(cardSprites[i]);
        }
        return unshuffledDeckTemp;
    }

    //Création du deck mélangé
    public List<Card> GenerateShuffledDeck()
    {
        List<Card> unshuffledDeckTemp = GenerateDeck();
        List<Card> shuffledDeckTemp = new List<Card>();
        
        for (int i = 0; i < nbcards; i++)
        {
            int randomIndex = Random.Range(0, nbcards - i); //on choisit un index entre 0 et 51, et on retire i à chaque fois car si on laisse la valeur maximale à 51, il y aura une erreur de sortie (tentative d'accès à une valeur non existante en tentant de retirer une carte qui n'existe plus)
            shuffledDeckTemp.Add(unshuffledDeckTemp[randomIndex]); // on ajoute dans la nouvelle liste la carte à l'index i...
            unshuffledDeckTemp.RemoveAt(randomIndex); //... et on la retire dans la liste originale
        }
        return shuffledDeckTemp;
    }

    void BoardGeneration()
    {
        //on initialise les 7 listes du tableau (qui correspondent aux cartes distribuées en colonnes)
        for (int i = 0; i < board.Length; i++)
        {
            float yOffset = 0f;
            board[i] = new List<Card>();
            Transform transformTemp;
            for (int j = 0; j < i + 1; j++)
            {
                board[i].Add(shuffledDeck[0]); //on ajoute toujours la première carte du deck
                GameObject newCard = Instantiate(cardPrefab, new Vector2(stacks[i].transform.position.x , stacks[i].transform.position.y - yOffset), Quaternion.identity);
                newCard.SetActive(true);
                transformTemp = (GameObject.FindGameObjectWithTag("Stacks").transform.GetChild(i)); //on met la carte en enfant du stack correspondant
                //et si j!=0, c'est à dire si ce n'est pas la première carte, on la met en enfant de la carte d'au dessus
                if (j!=0)
                {
                    for (int  k = 0;  k < j;  k++)
                    {
                        transformTemp = transformTemp.GetChild(0);
                    }
                }
                newCard.transform.SetParent(transformTemp);
                yOffset += 0.4f; //permet de décaler les cartes
                SpriteRenderer newCardSpriteRenderer = newCard.GetComponent<SpriteRenderer>();
                newCardSpriteRenderer.sortingOrder = j+1; // pour éviter les problèmes de superpositions
                newCard.name = shuffledDeck[0].GetName();
                shuffledDeck.RemoveAt(0);
            }
        }
    }

    void Update()
    {
        
    }
}
