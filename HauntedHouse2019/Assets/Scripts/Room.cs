using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    Light[] roomLights;
    [HideInInspector]
    public Transform[] decorActive;
    List<Ghost> ghosts;
    public Vector4 boundaries;

    private void Awake()
    {
        if (GetComponent<BoxCollider>() == null)
        {
            Vector3 floorScale = Vector3.one;
            foreach (Transform child in GetComponentsInChildren<Transform>())
                if (child.name == "Floor")
                    floorScale = child.localScale;

            BoxCollider tempBox = gameObject.AddComponent<BoxCollider>();
            tempBox.isTrigger = true;
            tempBox.size = new Vector3(floorScale.x * 10, 1, floorScale.z * 10);
        }

        BoxCollider temp = GetComponent<BoxCollider>();
        /*boundaries = new Vector4(temp.center.x - temp.size.x / 2 + transform.position.x,
            temp.center.x + temp.size.x / 2 + transform.position.x,
            temp.center.z - temp.size.z / 2 + transform.position.z,
            temp.center.z + temp.size.z / 2 + transform.position.z);*/

        boundaries = new Vector4(temp.center.x - temp.size.x / 2 + temp.center.x,
            temp.center.x + temp.size.x / 2 + temp.center.x,
            temp.center.z - temp.size.z / 2 + temp.center.z,
            temp.center.z + temp.size.z / 2 + temp.center.z);
    }
    void Start()
    {
        foreach(Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.name == "Lights")
                roomLights = child.GetComponentsInChildren<Light>();
            if (child.name == "DecorActive")
                decorActive = child.GetComponentsInChildren<Transform>();
            if (child.name == "Ghosts")
                ghosts = new List<Ghost>(child.GetComponentsInChildren<Ghost>());
        }

        

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
    public void TurnOnlights()
    {
        foreach (Light light in roomLights)
            light.color = new Color32(255, 244, 214, 255);
    }
    public void TurnOfflights()
    {
        foreach (Light light in roomLights)
            light.color = Color.black;
    }
    public void CheckGhosts()
    {
        //if no ghosts, turn on lights, make a noise, etc.
        if (ghosts.Count != 0)
            return;

        TurnOnlights();
    }
}
