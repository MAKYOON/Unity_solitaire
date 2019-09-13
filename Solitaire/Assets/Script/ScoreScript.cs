using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{
    public GameObject PanneauDefaite;
    public GameObject PanneauVictoire;
    public SelectableScript[] pilesSuperieures;
    public float tempsRestant = 300;
    public Text TimerText;
    public Text ScoreText;
    public int ScoreInt;
    //public bool victoirepatate;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      //  victoirepatate = Victoire();
        if (Victoire())
        {
            Fin();
        }

        if (tempsRestant > 0)
        {
            tempsRestant -= Time.deltaTime;
        }
        else
        {
            if (!Victoire())
            {
                Defaite();
            }
        }
        string minute = ((int)tempsRestant / 60).ToString();
        string seconde = ((int)tempsRestant % 60).ToString();

        TimerText.text = (minute + ":" + seconde);

        ScoreText.text = ("Score : "+ScoreInt.ToString());
    }

    public bool Victoire()
    {
        int i = 0;
        foreach (SelectableScript carteHaut in pilesSuperieures)
        {
            i += carteHaut.valeur;
        }
        if (i >= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void Fin()
    {
        print("Vous avez gagné !");
        PanneauVictoire.SetActive(true);
    }

    void Defaite()
    {
        print("Vous avez perdu.");
        PanneauDefaite.SetActive(true);
    }

}
