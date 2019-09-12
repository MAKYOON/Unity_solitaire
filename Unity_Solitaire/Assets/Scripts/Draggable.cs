using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector]  public Transform parentToReturnTo = null;

    public void OnBeginDrag(PointerEventData eventData)
    //Fonction qui est appelée au moment où on commence le drag.
    {
        //Si la carte n'est pas visible, il ne faut pas pouvoir la drag.
        if (GetComponent<CardManager>().hiddenImage.activeInHierarchy == false)
        {
            //On stock le parent actuel de la carte. Ainsi, si ce dernier n'est pas changé avant la fin du drag, la carte sera renvoyée à ce parent.
            parentToReturnTo = transform.parent;
            //En attendant, on lui attribue un parent plus éloigné (plateau du jeu).
            transform.SetParent(transform.parent.parent);

            //Bloquage des raycasts afin de pouvoir continuer de récupérer des informations sur le curseur pendant le drag.
            //(sinon, la fonction OnDrop du script DropZone ne peut pas être appelée)
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    //Fonction qui appelée pendant le drag (à chq mvt du curseur).
    //BUT : Actualisation de la position de la carte que l'on drag pour qu'elle suive le curseur.
    {
        //Si la carte n'est pas visible, il ne faut pas pouvoir la drag.
        if (GetComponent<CardManager>().hiddenImage.activeInHierarchy == false)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Canvas").transform as RectTransform, Input.mousePosition, Camera.main, out pos);
            transform.position = GameObject.FindGameObjectWithTag("Canvas").transform.TransformPoint(pos);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    //Fonction appelée à la fin du drag.
    {
        CardManager myCard = GetComponent<CardManager>();

        //Si la carte n'est pas visible, il ne faut pas pouvoir la drag.
        if (GetComponent<CardManager>().hiddenImage.activeInHierarchy == false)
        {
            //On réattribue un parent à la carte et on remet le raycast (pour pouvoir à nouveau faire un drag).
            transform.SetParent(parentToReturnTo);
            bool onEmptyStack = parentToReturnTo.GetComponent<DropZone>().GetStackParent(parentToReturnTo.transform) == parentToReturnTo;
            bool onFamilyStack = parentToReturnTo.GetComponent<DropZone>().GetStackParent(parentToReturnTo.transform).GetComponent<DropZone>().familyDropZone;
            if (onEmptyStack || onFamilyStack)
            {
                transform.localPosition = new Vector2(0, 0);
            }
            else if (parentToReturnTo.GetComponent<CardManager>() != null && parentToReturnTo.GetComponent<CardManager>().hiddenImage.activeInHierarchy)
            {
                transform.localPosition = new Vector2(0, -10);
            }
            else
            {
                transform.localPosition = new Vector2(0, -25);
            }

            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
