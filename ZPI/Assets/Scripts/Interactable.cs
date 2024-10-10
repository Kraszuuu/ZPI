using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //message displayed to player when looking at an interactable.
    public bool useEvents;
    [SerializeField]
    public string promptMessage;


    public void BaseInteract()
    {
        if (useEvents)
        {
            GetComponent<InteractionEvent>().OnInteract.Invoke();
        }
        Interact();
    }
    protected virtual void Interact()
    {
        //we wont have any code written in this function
        //this is a template function to be overriden by our subclasses
    }
}
