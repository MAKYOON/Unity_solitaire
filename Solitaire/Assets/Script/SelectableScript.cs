using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableScript : MonoBehaviour
{
    public bool haut = false;
    public string couleur;
    public int valeur;
    public int rang;
    public bool faceRecto = false;
    public bool dansLeDeck = false;

    private string chaineValeur;

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Carte"))
        {
            couleur = transform.name[0].ToString();

            for (int i = 1; i < transform.name.Length; i++)
            {
                char c = transform.name[i];
                chaineValeur = chaineValeur + c.ToString();
            }
            if (chaineValeur == "A")
            {
                valeur = 1;
            }
            if (chaineValeur == "2")
            {
                valeur = 2;
            }
            if (chaineValeur == "3")
            {
                valeur = 3;
            }
            if (chaineValeur == "4")
            {
                valeur = 4;
            }
            if (chaineValeur == "5")
            {
                valeur = 5;
            }
            if (chaineValeur == "6")
            {
                valeur = 6;
            }
            if (chaineValeur == "7")
            {
                valeur = 7;
            }
            if (chaineValeur == "8")
            {
                valeur = 8;
            }
            if (chaineValeur == "9")
            {
                valeur = 9;
            }
            if (chaineValeur == "10")
            {
                valeur = 10;
            }
            if (chaineValeur == "V")
            {
                valeur = 11;
            }
            if (chaineValeur == "D")
            {
                valeur = 12;
            }
            if (chaineValeur == "R")
            {
                valeur = 13;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
