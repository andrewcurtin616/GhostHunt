using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    Hallway[] hallways;
    Room[] rooms;
    Door[] doors;

    void Start()
    {
        hallways = FindObjectsOfType<Hallway>();
        rooms = FindObjectsOfType<Room>();
        doors = FindObjectsOfType<Door>();
    }
}
