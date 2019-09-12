using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour 
{
    float yOffset = -0.4f;
    private bool canDrag;
    private bool isDragged;
    bool hasBeenDropped = false;

    public Transform stackPiles;
    BoxCollider2D bc2d;
    SelectFace selectFace;
    List<Card>[] board = BoardManager.board;
    SpriteRenderer thisSpriteRenderer;

    void Start()
    {
        bc2d = GetComponent<BoxCollider2D>();
        selectFace = GetComponent<SelectFace>();
        thisSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMouseOver()
    {
        if (canDrag)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragged = true;
                thisSpriteRenderer.sortingOrder = 100; //juste pour s'assurer que quand on la déplace, la carte passe au-dessus de tout
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    { 
        //on récupère les informations de la carte sur laquelle on s'apprête à déposer la notre
        Card otherCard = new Card();
        for (int i = 0; i < board.Length; i++)
        {
            foreach (Card card in board[i])
            {
                if (other.name == card.GetName())
                {
                    otherCard.SetColor(card.GetColor());
                    otherCard.SetValue(card.GetValue());
                }
            }
        }
        bool canBeDropped = false;
        if (isDragged)
        {
            //comparaison couleur/valeur
            if ((selectFace.thisCard.GetColor() != otherCard.GetColor()) && selectFace.thisCard.GetValue() == (otherCard.GetValue()-1))
                canBeDropped = true;
            else if (other.gameObject.transform.IsChildOf(stackPiles.transform))
            {
                StackScript stackScript = FindObjectOfType<StackScript>();
                if (selectFace.thisCard.GetValue() == stackScript.value + 1)
                    canBeDropped = true;
            }
            if (canBeDropped)
            {
                this.transform.SetParent(other.transform); //on met la carte qu'on a drop en enfant de la carte sur laquelle on l'a placée
                transform.localPosition = new Vector2(0, yOffset);
                SpriteRenderer otherSpriteRenderer = other.gameObject.GetComponent<SpriteRenderer>();
                thisSpriteRenderer.sortingOrder = otherSpriteRenderer.sortingOrder + 1; //on met la carte au-dessus de celle sur laquelle on l'a placée
                hasBeenDropped = true; //on affirme que la carte à bien été déplacée et posée
            }
        }

    }

    void Update()
    {
        //on vérifie si la carte qu'on a drag à bien été déplacée, et si oui on la remove du board
        if (hasBeenDropped)
        {
            for (int i = 0; i < board.Length; i++)
            {
                foreach (Card card in board[i])
                {
                    if (selectFace.thisCard.GetName() == card.GetName())
                    {
                        board[i].Remove(card);
                        break;
                    }
                }
            }
        }
        //permet de différencier les cartes du board et les cartes du stock
        if (gameObject.transform.IsChildOf(selectFace.stockCards))
        {
            //incomplet pour le moment
        }
        else
        {
            if (selectFace.thisCard.GetCanDrag() == true)
                canDrag = true;
        }
        if (!canDrag)
            bc2d.enabled = false; //on désactive le boxcollider des cartes non retournées
        else
            bc2d.enabled = true;

        if (isDragged)
        {
            //la carte suit le mouvement de la souris
            Vector2 cursosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector2(cursosPos.x, cursosPos.y);
            if (Input.GetMouseButtonUp(0))
            {
                isDragged = false;
                //on vérifie si c'est l'enfant direct du Stack, si oui on le remet en 0,0 , sinon on applique l'offset
                for (int i = 0; i < board.Length; i++)
                {
                    if (transform.parent == GameObject.FindGameObjectWithTag("Stacks").transform.GetChild(i))
                    {
                        transform.localPosition = new Vector2(0, 0);
                        break;
                    }
                    else
                        transform.localPosition = new Vector2(0, yOffset);
                        thisSpriteRenderer.sortingOrder = (board[i].Count);
                }

            }
        }
    }
}


