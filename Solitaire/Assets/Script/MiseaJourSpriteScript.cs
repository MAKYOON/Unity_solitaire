using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiseaJourSpriteScript : MonoBehaviour
{
    public Sprite carteRecto;
    public Sprite carteVerso;
    private SpriteRenderer spriteRenderer;
    private SelectableScript selectable;
    private SolitaireScript solitaire;
    private InputUtilisateurScript inputUtilisateur;

    // Start is called before the first frame update
    void Start()
    {
        List<string> deck = SolitaireScript.generationDeck();
        solitaire = FindObjectOfType<SolitaireScript>();
        inputUtilisateur = FindObjectOfType<InputUtilisateurScript>();

        int i = 0;
        foreach(string carte in deck)
        {
            if (this.name == carte)
            {
                carteRecto = solitaire.carteRecto[i];
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        selectable = GetComponent<SelectableScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectable.faceRecto == true)
        {
            spriteRenderer.sprite = carteRecto;
        }
        else
        {
            spriteRenderer.sprite = carteVerso;
        }

        if (inputUtilisateur.slot1)
        {

            if (name == inputUtilisateur.slot1.name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
