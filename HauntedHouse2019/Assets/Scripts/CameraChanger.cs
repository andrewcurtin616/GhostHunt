using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField]
    float xMin;
    [SerializeField]
    float xMax;
    [SerializeField]
    float zMin;
    [SerializeField]
    float zMax;
    [SerializeField]
    Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            FindObjectOfType<CameraController>().ChangeCamera(xMin, xMax, zMin, zMax, newPos);
    }
}
