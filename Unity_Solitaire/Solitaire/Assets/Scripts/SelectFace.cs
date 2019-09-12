using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SelectFace : MonoBehaviour
{
    [HideInInspector] public Card thisCard;
    public Transform stockCards;

    public Sprite backFace;
    private BoardManager boardManager;
    private SpriteRenderer spriteRenderer;
    List<Card>[] board = BoardManager.board;

    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //on cherche la carte qui correspond et on récupère ses attributs
        for (int i = 0; i < board.Length; i++)
        {
                foreach (Card card in board[i])
                {
                    if (this.name == card.GetName())
                        thisCard = card;
                }
        }
    }

    void Update()
    {
        //on met la dernière carte de chaque liste à faceUp = true pour la retourner
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i].Count != 0)
            {
                if (thisCard == board[i].Last())
                    thisCard.SetFaceUp(true);
            }
        }
        //permet de différencier les cartes du board et les cartes du stock
        if (gameObject.transform.IsChildOf(stockCards))
        {
            //incomplet pour le moment
        }
        else
        { 
            if (thisCard.GetFaceUp() == true)
            {
                spriteRenderer.sprite = thisCard.GetSprite();
                thisCard.SetCanDrag(true);
            }
            else
            {
                spriteRenderer.sprite = backFace;
                thisCard.SetCanDrag(false);
            }
        }
    }
}
