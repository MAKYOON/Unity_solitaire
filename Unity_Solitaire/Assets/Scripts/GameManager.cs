using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public DeckManager m_DeckManager;
    public GameObject winScreen;
    public GameObject familyStacks;
    public GameObject scoreAndTime;

    [HideInInspector] static float score = 0;

    private float timeSinceBegining = 0f;
    private bool hasWin = false;
    private DropZone[] piles = new DropZone[3];

    private void Awake()
    {
        //On récupère les piles qui contiendront les familles.
        for (int i = 0; i < piles.Length; i++)
        {
            piles[i] = familyStacks.transform.GetChild(i).GetComponent<DropZone>();
        }
    }

    private void Start()
    //BUT : Créer le deck, mélanger et distribuer.
    {
        m_DeckManager.CreateDeck();
        m_DeckManager.ShuffleDeck();
        m_DeckManager.DistribCards();
    }

    void Update()
    //BUT : Mettre à jour les informations à l'écran (timer + score) et vérifier si le joueur a gagné.
    {
        timeSinceBegining += Time.deltaTime;
        DisplayTimeAndScore();

        if (HasWin())
        {
            //y = ax + b (Plus d'informations sur l'origine de ces valeurs dans le README)
            float scoreBonus = ((-1000 / 300) * Mathf.Floor(timeSinceBegining) + 1000);
            if (scoreBonus >=0)
            {
                score = score + scoreBonus;
            }

            //Affichage de la victoire et désactivation du GameManager (pour stoper les compteurs etc).
            winScreen.SetActive(true);
            enabled = false;
        }
    }

    public void AddScore(float amount)
    //BUT : Incrémenter le score.
    //ENTREE : amount : le nnombre de points à rajouter.
    {
        score = score + amount;
    }

    void DisplayTimeAndScore()
    //BUT : Mettre à jour le score et le temps à l'écran.
    {
        scoreAndTime.GetComponent<Text>().text = "Score : " + score + "\nTemps : " + Mathf.Floor(timeSinceBegining);
    }

    bool HasWin()
    //BUT : Déterminer si le joueur a gagné ou non.
    //SORTIE : TRUE si le joueur a gagné, FAUX sinon.
    {
        hasWin = true;
        
        //On parcourt chaque pile
        for (int i = 0; i < piles.Length; i++)
        {
            Transform child = piles[i].GetComponent<Transform>();

            int nbOfCardChild = 0;  //Cette variable compte le nombre de fois où l'on rentre dans un enfant qui est une carte.
            bool goDeeper = true;   //Tant que goDeeper est TRUE, la boucle continuera d'aller dans l'enfant pour y chercher une carte.
            //Recherche de l'enfant ayant le CardManager
            while(goDeeper)
            {
                goDeeper = false;   //Si dans ce passage de la boucle on ne trouve pas d'enfant qui est une carte, on sortira de la boucle.
                for (int j = 0; j < child.childCount; j++)
                {
                    //Si l'enfant est une carte...
                    if (child.GetChild(j).GetComponent<CardManager>() != null)
                    {
                        child = child.GetChild(j);
                        nbOfCardChild++;
                        goDeeper = true;    //Puisque l'on a trouvé un enfant qui est une carte, goDeeper repasse à true pour qu'on aille voir dedans au prochain passage.
                        break;
                    }
                }
            }

            //Le joueur gagne seulement si les 4 piles ont 13 cartes.
            hasWin = hasWin && (nbOfCardChild >= 13);
        }
        return (hasWin);
    }
}

