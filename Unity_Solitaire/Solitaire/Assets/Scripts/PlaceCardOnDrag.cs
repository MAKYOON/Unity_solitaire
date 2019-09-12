using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCardOnDrag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnEnterTrigger2D(Collider other)
    {
        other.transform.position = transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
