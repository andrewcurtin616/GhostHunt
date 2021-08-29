using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;
    Transform resetTransform;
    Vector3 offset;
    bool adjustable;

    [HideInInspector]
    public float xMin = -10;
    [HideInInspector]
    public float xMax = 10;
    [HideInInspector]
    public float zMin = -10;
    [HideInInspector]
    public float zMax = 10;

    private void Awake()
    {
        resetTransform = transform;
    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        Vector3 targetPos = player.transform.position + offset;
        targetPos.x = Mathf.Clamp(targetPos.x, xMin, xMax);
        targetPos.z = Mathf.Clamp(targetPos.z, zMin, zMax);
        transform.position = player.transform.position + offset;


        //Adjustable Camera for Debug Mode
        if (Input.GetKeyUp(KeyCode.CapsLock))
            adjustable = adjustable ? false : true;
        if (adjustable)
        {
            if (Input.GetKey(KeyCode.U))
            {
                transform.position += Vector3.up * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.J))
            {
                transform.position += Vector3.down * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.H))
            {
                transform.position += Vector3.left * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.K))
            {
                transform.position += Vector3.right * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.Y))
            {
                transform.position += Vector3.forward * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.I))
            {
                transform.position += Vector3.back * 0.1f;
                offset = transform.position - player.transform.position;
            }
            if (Input.GetKey(KeyCode.N))
            {
                //zoom in
            }
            if (Input.GetKey(KeyCode.M))
            {
                //zoom out
            }
            if (Input.GetKeyDown(KeyCode.O))
                transform.LookAt(player.transform.position);
        }
    }

    public void ChangeCamera(float xMin,float xMax,float zMin, float zMax,
        Vector3 newPos)
    {

    }
}
