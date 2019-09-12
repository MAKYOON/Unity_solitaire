using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//REMARQUE : Chaque carte possède son propre CardManager.
public class CardManager : MonoBehaviour, IPointerDownHandler
{
    public GameObject hiddenImage;
    public GameObject m_GameManager;

    [HideInInspector] public enum CardColor { trefle = 0, carreau = 1, coeur = 2, pique = 3 }
    [HideInInspector] public enum CardValue { Deux = 2, Trois = 3, Quatre = 4, Cinq = 5, Six = 6, Sept = 7, Huit = 8, Neuf = 9, Dix = 10, Valet = 11, Dame = 12, Roi = 13, As = 1 }

    private Card cardInfo;

    public class Card
    {
        public CardValue value;
        public CardColor color;

        public Card(CardValue myValue, CardColor myColor)
        {
            value = myValue;
            color = myColor;
        }
    }

    public void SetCard(Card myCard)
    //BUT : Stocker les valeurs de la carte créer.
    //ENTREE : La carte que l'on a crée.
    {
        cardInfo = new Card(myCard.value, myCard.color);
    }

    public Card GetCard()
    //BUT : Récupérer les informations (couleur et valeur) de la carte
    {
        return cardInfo;
    }

    public void OnPointerDown(PointerEventData eventData)
    //BUT : Rendre une carte visible lorsque cette dernière ne l'est pas et que l'on clique dessus.
    //ENTREE : eventData : les informations sur le curseur.
    //RQ : La fonction ne se lance que lorsque l'on clique sur un objet possédant le composant CartManager.
    {
        int childNb = transform.childCount;
        int i = 0;
        bool hasACardChild = false;
        CardManager childOfDropZone = null;

        //On parcourt tous les enfants de la carte sur laquelle on clique. Si un de ces derniers est une carte, la carte ne pourra pas être dévoilée.
        //(Par conséquent, elle ne pourra pas être déplacée (cf script "Draggable").
        while (i < childNb && !hasACardChild)
        {
            childOfDropZone = transform.GetChild(i).GetComponent<CardManager>();
            hasACardChild = childOfDropZone != null;
            i++;
        }

        //Quand un joueur clique sur une carte qui n'a pas d'enfant et est face cachée, le cache de la carte se désactive et le joueur gagne 50pts.
        if (!hasACardChild && hiddenImage.activeInHierarchy)
        {
            hiddenImage.SetActive(false);
            m_GameManager.GetComponent<GameManager>().AddScore(50f);
        }
    }
}
