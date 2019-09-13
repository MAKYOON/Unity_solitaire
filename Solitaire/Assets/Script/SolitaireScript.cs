 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SolitaireScript : MonoBehaviour
{
    public Sprite[] carteRecto;
    public GameObject cartePrefab;
    public GameObject boutonDeck;
    public GameObject[] positionBas;
    public GameObject[] positionHaut;

    public static string[] couleurs = new string[] { "D", "C", "P", "T" }; //D = Diamant pour Carreau, C = Coeur, P = Pique, T = Trèfle
    public static string[] valeurs = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "V", "D", "R" };
    public List<string>[] bas;
    public List<string>[] hauts;
    public List<string> sortisVisible = new List<string>();
    public List<List<string>> sortisDeck = new List<List<string>>();

    private List<string> bas0 = new List<string>();
    private List<string> bas1 = new List<string>();
    private List<string> bas2 = new List<string>();
    private List<string> bas3 = new List<string>();
    private List<string> bas4 = new List<string>();
    private List<string> bas5 = new List<string>();
    private List<string> bas6 = new List<string>();


    public List<string> deck;
    public List<string> defausse = new List<string>();
    private int etatDeck;
    private int sortis;
    private int sortisReste;

    // Start is called before the first frame update
    void Start()
    {
        bas = new List<string>[] { bas0, bas1, bas2, bas3, bas4, bas5, bas6 };
        creationCartes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void creationCartes()
    {
        deck = generationDeck();
        melange(deck);

        //Tester les cartes dans le deck.
        foreach (string carte in deck)
        {
            print(carte);
        }
        triSolitaire();
        solitaireGestion();
        TriCartesSorties();
    }


    public static List<string> generationDeck() //Fonction pour créer le deck à l'aide des listes de valeurs et des couleurs.
    {
        List<string> nouveauDeck = new List<string>();
        foreach (string c in couleurs)
        {
            foreach (string v in valeurs)
            {
                nouveauDeck.Add(c + v);
            }
        }
        return nouveauDeck;
    }


    void melange<T>(List<T> list) //Fonction de mélange récupérée sur Stack Overflow.
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    void solitaireGestion()
    {
        for (int i = 0; i < 7; i++)
        {

            float decalageY = 0;
            float decalageZ = -0.05f;

            foreach (string carte in bas[i])
            {
                GameObject nouvelleCarte = Instantiate(cartePrefab, new Vector3(positionBas[i].transform.position.x, positionBas[i].transform.position.y - decalageY, positionBas[i].transform.position.z + decalageZ), Quaternion.identity, positionBas[i].transform);
                nouvelleCarte.name = carte;
                nouvelleCarte.GetComponent<SelectableScript>().rang = i;
                if (carte == bas[i][bas[i].Count - 1])
                {
                    nouvelleCarte.GetComponent<SelectableScript>().faceRecto = true;
                }

                decalageY = decalageY + 0.35f;
                decalageZ = decalageZ - 0.05f;
                defausse.Add(carte);
            }
        }

        foreach(string carte in defausse)
        {
            if(deck.Contains(carte))
            {
                deck.Remove(carte);
            }
        }
        defausse.Clear();

    }

    void triSolitaire()
    {
        for (int i = 0; i<7 ; i++)
        {
            for (int j = i; j<7; j++)
            {
                bas[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }
    }

    public void TriCartesSorties()
    {
        sortis = deck.Count / 3;
        sortisReste = deck.Count % 3;
        sortisDeck.Clear();

        int modificateur = 0;
        for (int i = 0; i<sortis; i++)
        {
            List<string> maSorti = new List<string>();
            for (int j = 0; j<3; j++)
            {
                maSorti.Add(deck[j + modificateur]);
            }
            sortisDeck.Add(maSorti);
            modificateur = modificateur + 3;
        }
        if (sortisReste !=0)
        {
            List<string> monReste = new List<string>();
            modificateur = 0;
            for (int k = 0; k<sortisReste; k++)
            {
                monReste.Add(deck[deck.Count - sortisReste + modificateur]);
                modificateur++;
            }
        }
        etatDeck = 0;
    }

    public void TirerDuDeck()
    {
        //Ajout des cartes restantes de la défausse 
        foreach (Transform child in boutonDeck.transform)
        {
            if (child.CompareTag("Carte"))
            {  
                deck.Remove(child.name);
                defausse.Add(child.name);
                Destroy(child.gameObject);
            }
        }

        if (etatDeck<sortis)
        {
            //Piocher 3 nouvelles cartes
            sortisVisible.Clear();
            float decalageX = 2.5f;
            float decalageZ = -0.2f;

            foreach(string carte in sortisDeck[etatDeck])
            {
                GameObject nouvelleCarteHaut = Instantiate(cartePrefab, new Vector3(boutonDeck.transform.position.x + decalageX, boutonDeck.transform.position.y, boutonDeck.transform.position.z+decalageZ), Quaternion.identity, boutonDeck.transform)    ;
                decalageX = decalageX + 0.5f;
                decalageZ = decalageZ - 0.2f;
                nouvelleCarteHaut.name = carte;
                sortisVisible.Add(carte);
                nouvelleCarteHaut.GetComponent<SelectableScript>().faceRecto = true;
                nouvelleCarteHaut.GetComponent<SelectableScript>().dansLeDeck = true;
            }
            etatDeck++;
        }
        else
        {
            //Réempiler le deck
            ReempilerLeDeck();
        }
    }

    void ReempilerLeDeck()
    {
        deck.Clear();
        foreach (string carte in defausse)
        {
            deck.Add(carte);
        }
        defausse.Clear();
        TriCartesSorties();
    }
}
