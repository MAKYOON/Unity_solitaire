using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InputUtilisateurScript : MonoBehaviour
{
    public GameObject slot1;
    private SolitaireScript solitaire;
    private float timer;
    private float tempsDoubleClique = 0.3f;
    private int clickCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        solitaire = FindObjectOfType<SolitaireScript>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > tempsDoubleClique)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                //Verification de ce sur quoi l'utilisateur clique.
                if (hit.collider.CompareTag("Deck"))
                {
                    //Il clique sur le deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Carte"))
                {
                    //Il clique sur une carte.
                    Carte(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Haut"))
                {
                    //Il clique sur une partie du haut.
                    Haut(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bas"))
                {
                    //Il clique sur une partie du bas.
                    Bas(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck ()
    {
        //Action suite à un clique sur le Deck.
        print("Clique Deck.");
        solitaire.TirerDuDeck();
    }

    void Carte(GameObject selection)
    {
        //Action suite à un clique sur une carte.
        print("Clique Carte.");

        //Si la carte sur laquelle on clique est face verso et si elle n'est pas bloquer ==> la retourner.
        if (!selection.GetComponent<SelectableScript>().faceRecto)
        {
            if (!Bloque(selection))
            {
                selection.GetComponent<SelectableScript>().faceRecto = true;
                slot1 = this.gameObject;
                GetComponent<ScoreScript>().ScoreInt = GetComponent<ScoreScript>().ScoreInt + 10;
                print("Score +10, carte verso retournée.");
            }
        }
        else if (selection.GetComponent<SelectableScript>().dansLeDeck)
        {
            //Si la carte sur laquelle on clique est dans le deck et n'est pas bloquer ==> la sélectionner.
            if (!Bloque(selection))
            {
                if(slot1 == selection) //Si une carte subit un double clique.
                {
                    if (DoubleClique())
                    {
                        //Tentative d'action automatique.
                        ActionAutomatique(selection);
                    }
                }
                else
                {
                    slot1 = selection;
                }
            }
        }
        else
        {
            //Si la carte est visible et qu'il n'y a pas de carte sélectionnée pour l'instant ==> la sélectionner.
            if (slot1 == this.gameObject)
            {
                slot1 = selection;
            }

            //Si il y a déjà une carte sélectionnée et qu'on ne clique pas sur la carte déjà sélectionnée, et si la carte sur laquelle on clique peut porter celle sélectionnée précédemment, on la met par-dessus, sinon, on la sélectionne à la place.

            else if (slot1 != selection)
            {
                if (Empilable(selection))
                {
                    Empiler(selection);
                }
                else
                {
                    slot1 = selection;
                }
            }
            //Si c'est la même carte sur laquelle on a déjà cliqué et qu'il y a peu de temps entre les deux cliques, c'est un double clique, alors si la carte peut aller en haut, elle y va.
        }
    }

    void Haut(GameObject selection)
    {
        //Action suite à un clique sur le haut.
        print("Clique Haut.");
        if (slot1.CompareTag("Carte"))
        {
            //Si la carte est un As sur un emplacement vide alors empiler.
            if (slot1.GetComponent<SelectableScript>().valeur == 1)
            {
                Empiler(selection);
            }
        }
    }

    void Bas(GameObject selection)
    {
        //Action suite à un clique sur le bas.
        print("Clique Bas.");
        //Si la carte est un Roi sur un emplacement vide alors empiler.
        if (slot1.CompareTag("Carte"))
        {
            if (slot1.GetComponent<SelectableScript>().valeur ==13)
            {
                Empiler(selection);
            }
        }
    }

    bool Empilable(GameObject selection)
    {
        SelectableScript s1 = slot1.GetComponent<SelectableScript>();
        SelectableScript s2 = selection.GetComponent<SelectableScript>();
        //Comparaison pour voir si ils sont empilés.

        if (!s2.dansLeDeck)
        {
            //Si il est dans la pile du haut il doit aller de l'as vers le roi en gardant sa couleur.
            if (s2.haut)
            {
                if (s1.couleur == s2.couleur || (s1.valeur == 1 && s2.couleur == null))
                {
                    if (s1.valeur == s2.valeur + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else //Si il est dans la pile du bas il doit aller du roi vers l'as en alternant les couleurs.
            {
                if (s1.valeur == s2.valeur - 1)
                {
                    bool carte1Rouge = true;
                    bool carte2Rouge = true;

                    if (s1.couleur == "P" || s1.couleur == "T")
                    {
                        carte1Rouge = false;
                    }

                    if (s2.couleur == "P" || s2.couleur == "T")
                    {
                        carte2Rouge = false;
                    }

                    if (carte1Rouge == carte2Rouge)
                    {
                        print("Non superposable.");
                        return false;
                    }
                    else
                    {
                        print("Superposable.");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Empiler(GameObject selection)
    {
        //Si on le met sur un emplacement vide on pose directement la carte.
        //Sinon on la met avec un décalage en y.

        SelectableScript s1 = slot1.GetComponent<SelectableScript>();
        SelectableScript s2 = selection.GetComponent<SelectableScript>();
        float decalageY = 0.3f;

        if (s2.valeur == 0) //haut || (!s2.haut && s1.valeur == 13))
        {
            decalageY = 0;
        }

        slot1.transform.position = new Vector3(selection.transform.position.x, selection.transform.position.y - decalageY, selection.transform.position.z - 0.05f);
        slot1.transform.parent = selection.transform; //Pour déplacer l'enfant avec le parent.

        if (s1.dansLeDeck) //Retirer la carte de la pile pour ne pas la dupliquer.
        {
            solitaire.sortisVisible.Remove(slot1.name);
        }
        else if (s1.haut && s2.haut && s1.valeur == 1) // Permet le mouvement des cartes en haut.
        {
            solitaire.positionHaut[s1.rang].GetComponent<SelectableScript>().valeur = 0;
            solitaire.positionHaut[s1.rang].GetComponent<SelectableScript>().couleur = null;
        }
        else if (s1.haut)
        {
            solitaire.positionHaut[s1.rang].GetComponent<SelectableScript>().valeur = s1.valeur - 1;
        }
        else
        {
            solitaire.bas[s1.rang].Remove(slot1.name);
        }

        s1.dansLeDeck = false;
        s1.rang = s2.rang;

        if (s2.haut)
        {
            solitaire.positionHaut[s1.rang].GetComponent<SelectableScript>().valeur = s1.valeur;
            solitaire.positionHaut[s1.rang].GetComponent<SelectableScript>().couleur = s1.couleur;
            s1.haut = true;
            GetComponent<ScoreScript>().ScoreInt = GetComponent<ScoreScript>().ScoreInt + 15;
            print("Score +15, dans la pile du haut.");
        }
        else
        {
            s1.haut = false;
            GetComponent<ScoreScript>().ScoreInt = GetComponent<ScoreScript>().ScoreInt + 10;
            print("Score +10, dans la pile du bas.");
        }

        slot1 = this.gameObject;
    }

    bool Bloque(GameObject selection)
    {
        SelectableScript s2 = selection.GetComponent<SelectableScript>();
        if (s2.dansLeDeck == true)
        {
            if (s2.name == solitaire.sortisVisible.Last())
            {
                return false;
            }
            else
            {
                print(s2.name + " est bloqué par " + solitaire.sortisVisible.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitaire.bas[s2.rang].Last()) // vérification si c'est la carte du bas.
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    bool DoubleClique()
    {
        if (timer < tempsDoubleClique && clickCount == 2)
        {
            print("Double Clique.");
            return true;
        }
        else
        {
            GetComponent<ScoreScript>().ScoreInt = GetComponent<ScoreScript>().ScoreInt - 10;
            print("Score -10, double clique impossible.");
            return false;
        }
    }

    void ActionAutomatique (GameObject selection)
    {
        for (int i = 0; i< solitaire.positionHaut.Length; i++)
        {
            SelectableScript empiler = solitaire.positionHaut[i].GetComponent<SelectableScript>();
            if (selection.GetComponent<SelectableScript>().valeur == 1) // Si c'est un As.
            {
                if (solitaire.positionHaut[i].GetComponent<SelectableScript>().valeur == 0) //Et qu'une position en haut est libre.
                {
                    slot1 = selection;
                    Empiler(empiler.gameObject); //Mettre l'as en haut dans la première position trouvée.
                    break;
                }
            }
            else
            {
                if ((solitaire.positionHaut[i].GetComponent<SelectableScript>().couleur == slot1.GetComponent<SelectableScript>().couleur) && (solitaire.positionHaut[i].GetComponent<SelectableScript>().valeur == slot1.GetComponent<SelectableScript>().valeur-1))
                {
                    //Si c'est la dernière carte.
                    if (AucunEnfant(slot1))
                    {
                        slot1 = selection;
                        //Trouver un endroit en haut qui convienne et empiler si possible.
                        string nomDeLaDerniereCarte = empiler.couleur + empiler.valeur.ToString();
                        if (empiler.valeur == 1)
                        {
                            nomDeLaDerniereCarte = empiler.couleur + "A";
                        }
                        if (empiler.valeur == 11)
                        {
                            nomDeLaDerniereCarte = empiler.couleur + "V";
                        }
                        if (empiler.valeur == 12)
                        {
                            nomDeLaDerniereCarte = empiler.couleur + "D";
                        }
                        if (empiler.valeur == 13)
                        {
                            nomDeLaDerniereCarte = empiler.couleur + "R";
                        }
                        GameObject derniereCarte = GameObject.Find(nomDeLaDerniereCarte);
                        Empiler(derniereCarte);
                        break;
                    }
                }
            }
        }
    }

    bool AucunEnfant(GameObject carte)
    {
        int i = 0;
        foreach(Transform child in carte.transform)
        {
            i++;
        }
        if (i ==0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
