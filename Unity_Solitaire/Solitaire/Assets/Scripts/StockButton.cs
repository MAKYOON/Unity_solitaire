using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StockButton : MonoBehaviour
{
    public GameObject stockCards;
    BoardManager boardManager;
    List<Card> stockDeck;
    List<Card>[] board = BoardManager.board;
    SelectFace selectFace;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        boardManager = FindObjectOfType<BoardManager>();
        stockDeck = boardManager.GetShuffledDeck();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = stockDeck.Count + 1;
        /*instancie le restant du paquet sous le GameObject "StockButton", l'idée était ensuite d'essayer de récupérer les 3 premiers enfants
         * du GameObject, et lorsqu'on appuie sur le GameObject, déplacer les cartes, et si on rappuie dessus, vérifier s'il y a déjà des cartes,
         * remettre les 3 cartes visibles en dessous du paquet et déplacer 3 autres cartes
         */
        for (int i = 0; i < stockDeck.Count; i++)
        {
            GameObject newCardInstance = Instantiate(boardManager.cardPrefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            newCardInstance.SetActive(true);
            newCardInstance.name = stockDeck[i].GetName();
            newCardInstance.transform.SetParent(stockCards.transform);
            SpriteRenderer newCardSpriteRenderer = newCardInstance.GetComponent<SpriteRenderer>();
            newCardSpriteRenderer.sprite = stockDeck[i].GetSprite();
            newCardSpriteRenderer.sortingOrder += i; 
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float xOffset = 0f;
            for (int i = 0; i<3; i++)
            {
                stockCards.transform.GetChild(i).localPosition = new Vector2(xOffset, 0);
                xOffset += 0.3f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
