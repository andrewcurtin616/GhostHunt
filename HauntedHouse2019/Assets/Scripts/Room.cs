using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private void Awake()
    {
        
    }
    void Start()
    {
        RoomExit();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            RoomEnter();
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
            RoomExit();
    }
    void RoomEnter()
    {
        foreach (Renderer render in GetComponentsInChildren<Renderer>())
            render.enabled = true;
    }
    void RoomExit()
    {
        foreach (Renderer render in GetComponentsInChildren<Renderer>())
            render.enabled = false;
    }
}
