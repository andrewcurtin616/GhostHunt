using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoor : MonoBehaviour
{
    Renderer door1; //just using Renderer for now, could use Door later
    Renderer door2;

    void Start()
    {
        door1 = GetComponentsInChildren<Renderer>()[0];
        door2 = GetComponentsInChildren<Renderer>()[1];
    }

    public void CheckDoorFade(Color myC)
    {
        door1.material.color = myC;
        door2.material.color = myC;
    }
}
