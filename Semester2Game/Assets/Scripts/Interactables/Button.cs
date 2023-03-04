using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject assignedObject;
    [SerializeField] private bool buttonState;

    public void Interact(GameObject source)
    {
        assignedObject.GetComponent<IInteractable>().Interact(gameObject);
        buttonState = !buttonState;
    }
}
