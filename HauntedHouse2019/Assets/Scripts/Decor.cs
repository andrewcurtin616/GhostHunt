using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decor : MonoBehaviour
{
    public bool inspectable;
    public bool inspecting;
    float tempTime;
    //PlayerController player;
    //PlayerControllerV2 player;
    PlayerController player;
    Vector3 originalPosition;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControllerV2>();
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (inspectable && !inspecting && Input.GetKeyDown(KeyCode.Space) && player.canMove
            && Vector3.Angle(player.transform.forward, transform.position - player.transform.position) > 40
            && Vector3.Angle(player.transform.forward, transform.position - player.transform.position) < 60)
        {
            inspecting = true;
            tempTime = Time.time;
            player.inspecting = true;
            player.canMove = true;
            player.tempTime = Time.time;
            player.transform.LookAt(new Vector3(transform.position.x, player.transform.position.y, transform.position.z));
        }
        if (inspecting)
        {
            int xRan = Random.Range(-2, 3);
            int zRan = Random.Range(-2, 3);
            transform.position = new Vector3(transform.position.x + Mathf.Sin(Time.time * 4) * 0.01f * xRan, transform.position.y,
                transform.position.z + Mathf.Sin(Time.time * 4) * 0.01f * zRan);

            if (Time.time > tempTime + 1f)
            {
                player.inspecting = false;
                StopInspecting();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !(other is SphereCollider))
            inspectable = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && !(other is SphereCollider))
            inspectable = false;
    }

    public void StopInspecting()
    {
        inspecting = false;
        transform.position = originalPosition;
    }
}
