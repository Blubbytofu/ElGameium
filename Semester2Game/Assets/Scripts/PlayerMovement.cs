using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject orientation;

    [SerializeField] private bool isGrounded;

    void Start()
    {
        
    }

    void Update()
    {
        isGrounded = Physics.Raycast(orientation.transform.position, -orientation.transform.up, 1.1f);
    }
}
