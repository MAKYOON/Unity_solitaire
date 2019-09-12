using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    public bool familyDropZone = false;


    public void OnDrop(PointerEventData eventData)
    //BUT : Si la carte peut être drop sur l'objet à qui ce composant appartient, lui attribuer son transform comme parent.
    //RQ : eventData.pointerDrag renvoie l'objet que l'on est en train de drag.
    {
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();          //Référence du Draggable de l'objet que l'on est en train de drag
        CardManager draggedCard = eventData.pointerDrag.GetComponent<CardManager>();          //Référence du cardManager de l'objet que l'on est en train de drag
        
        //Si la carte peut être déposée sur l'objet à qui ce composant appartient, alors son parent devient le transform de cet objet.
        if (CanReceiveCard(draggedCard))
        {
            draggable.parentToReturnTo = transform;
        }
    }

    bool CanReceiveCard(CardManager draggedCard)
    //BUT : Déterminer si la DropZone peut recevoir la carte ou non.
    //ENTREE : La carte que l'on veut déposer.
    //SORTIE : TRUE si on peut recevoir
    {
        //Si la DropZone a déjà un enfant qui est une carte, alors on ne peut pas drop de toute façon.
        bool canReceive = !IsChildACard();
       
        if (canReceive)
        {
            //Vérification des règles sur la couleur et sur la valeur des cartes reçues/recevantes.
            CardManager receivingCard = this.GetComponent<CardManager>();

            //On s'assure que la carte recevante est bien une carte (et pas une dropzone quelconque).
            if (receivingCard != null)          
            {
                canReceive = ColorChecked(receivingCard, draggedCard) && ValueChecked(receivingCard, draggedCard);
            }
            else
            {
                canReceive = ValueChecked(draggedCard, draggedCard);
            }
        }
        return (canReceive);
    }

    bool IsChildACard()
    //BUT : Déterminer si la DropZone a déjà un enfant direct qui est une carte.
    //SORTIE : TRUE si la DropZone a un enfant direct qui est une carte, FALSE sinon.
    {
        bool tempIsChildACard = false;
        int childNb = transform.childCount;
        int i = 0;
        
        //Tant que l'on n'a pas parcouru tous les enfants OU que l'on en a pas trouvé un possédant le composant Draggable (qui est propre aux cartes)...
        while (i < childNb && tempIsChildACard)
        {
            CardManager childOfDropzone = transform.GetChild(i).GetComponent<CardManager>(); //Référence de l'enfant i de l'objet sur lequel on veut drop

            if (childOfDropzone != null)
            {
                tempIsChildACard = true;
            }

            i++;
        }
        return tempIsChildACard;
    }

    bool ColorChecked (CardManager receivingCard, CardManager draggedCard)
    //BUT : Déterminer si la carte reçue a la couleur nécessaire pour s'empiler sur la carte (ou pile) recevante.
    //ENTREE : receiving card : la carte recevante ; draggedCard : la carte reçue
    //SORTIE : TRUE si les couleurs peuvent s'empiler, FALSE sinon.
    {
        //Si le parent le plus éloigné est une familyDropZone (une des 4 piles), on lui vérifie des règles différentes.
        if (GetStackParent(receivingCard.transform).GetComponent<DropZone>().familyDropZone)
        {
            return (draggedCard.GetCard().color == receivingCard.GetCard().color);
        }
        else
        {
            //On commence par déterminer la couleur (rouge ou noire) de la carte reçue et recevante
            bool draggedIsRed = (draggedCard.GetCard().color == CardManager.CardColor.carreau || draggedCard.GetCard().color == CardManager.CardColor.coeur);
            bool receivingIsRed = (receivingCard.GetCard().color == CardManager.CardColor.carreau || receivingCard.GetCard().color == CardManager.CardColor.coeur);

            //FAUX si la carte reçue et la recevante sont toutes les deux rouges OU toutes les deux noires... 
            return (draggedIsRed != receivingIsRed);
        }
    }

    bool ValueChecked (CardManager receivingCard, CardManager draggedCard)
    //BUT : Déterminer si la carte reçue a la valeur nécessaire pour s'empiler sur la carte (ou pile) recevante.
    //ENTREE : receiving card : la carte recevante ; draggedCard : la carte reçue
    //SORTIE : TRUE si les valeurs peuvent s'empiler, FALSE sinon.
    {
        //Si la pile d'origine de l'objet contenant ce script n'est autre que lui-même (correspond au cas où la DropZone est une pile vide)...
        if (GetStackParent(transform) == transform)
        {
            //Si l'on est sur une pile de famille, alors la DropZone ne peut recevoir qu'un as
            if (GetComponent<DropZone>().familyDropZone)
            {
                return (draggedCard.GetCard().value == CardManager.CardValue.As);
            }
            //Sinon c'est qu'on est juste sur une colonne vide et par conséquent on ne peut recevoir qu'un roi.
            else
            {
                return (draggedCard.GetCard().value == CardManager.CardValue.Roi);
            }
        }
        else if (GetStackParent(receivingCard.transform).GetComponent<DropZone>().familyDropZone)
        {
            int valueReceivingCard = (int)receivingCard.GetCard().value;
            int valueDraggedCard = (int)draggedCard.GetCard().value;
            return (valueDraggedCard - valueReceivingCard == 1);
        }
        else
        {
            int valueReceivingCard = (int)receivingCard.GetCard().value;
            int valueDraggedCard = (int)draggedCard.GetCard().value;

            return (valueReceivingCard - valueDraggedCard == 1);
        }
    }

    public Transform GetStackParent(Transform parent)
    //BUT : Déterminer de quelle pile la carte vient (talon, une des sept colonnes ou une des piles de famille).
    //ENTREE : Transform parent : le transform contenant le parent de l'objet dont on veut savoir l'origine.
    //SORTIE : La pile dont est issue la carte.
    {
        Transform currentParent = parent;
        int i = 0;
        //Un stack est définit par la présence d'un composant DropZone ET de l'absence d'un Draggable.
        //Le i < 20 sert à s'assurer qu'on finisse bien par sortir de la boucle au cas où on ne trouve pas le parent le plus éloigné qui est une pile.
        while (currentParent.GetComponent<DropZone>() != null && currentParent.GetComponent<Draggable>() != null && i < 20)
        {
            currentParent = currentParent.parent;
            i++;
        }
        return (currentParent);
    }
}
