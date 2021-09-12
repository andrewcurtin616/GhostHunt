using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    bool openIn;
    bool atDoor;
    BoxCollider hitBox;

    Vector3 frontPos;
    Vector3 backPos;

    GameObject doorAnchor;

    float originalRot;

    private void Awake()
    {
        hitBox = GetComponentInChildren<BoxCollider>();

        //Note: could have different animations for opening in and out
        //but this is fine for now
        frontPos = transform.position + transform.forward;
        backPos = transform.position - transform.forward;
        
    }
    void Start()
    {
        doorAnchor = transform.parent.gameObject;
        originalRot = doorAnchor.transform.localEulerAngles.y;
    }

    void Update()
    {
        if(atDoor && Input.GetKeyDown(KeyCode.Space) && FindObjectOfType<PlayerController>().canMove)
        {
            FindObjectOfType<PlayerController>().canMove = false;
            StartCoroutine("MovePlayer");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
            atDoor = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
            atDoor = false;
    }

    IEnumerator OpenDoor()
    {
        for (int i = 1; i < 91; i++)
        {
            doorAnchor.transform.localEulerAngles = Vector3.up * (i + originalRot);
            yield return null;
        }
        doorAnchor.transform.localEulerAngles = Vector3.up * (90 + originalRot);
        yield return new WaitForSeconds(0.5f);
        for (int i = 89; i > -1; i--)
        {
            doorAnchor.transform.localEulerAngles = Vector3.up * (i + originalRot);
            yield return null;
        }
        DoorClose();
    }
    IEnumerator OpenDoorOut()
    {
        for (int i = 1; i < 91; i++)
        {
            doorAnchor.transform.localEulerAngles = Vector3.up * (-i + originalRot);
            yield return null;
        }
        doorAnchor.transform.localEulerAngles = Vector3.up * (-90 + originalRot);
        yield return new WaitForSeconds(0.5f);
        for (int i = 89; i > -1; i--)
        {
            doorAnchor.transform.localEulerAngles = Vector3.up * (-i + originalRot);
            yield return null;
        }
        DoorClose();
    }
    void DoorClose()
    {
        if ((transform.parent.localEulerAngles.y == 0 || transform.parent.localEulerAngles.y == 180))
            Debug.Log("Y: ");
        doorAnchor.transform.localEulerAngles = Vector3.up * (0 + originalRot);
        hitBox.enabled = true;
        FindObjectOfType<PlayerController>().canMove = true;
    }
    IEnumerator MovePlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        bool isPlayerInFront = Vector3.Dot(transform.forward, player.transform.position - transform.position) > 0;
        
        player.transform.position = isPlayerInFront ? new Vector3(frontPos.x, player.transform.position.y, frontPos.z):
            new Vector3(backPos.x, player.transform.position.y, backPos.z);

        player.transform.LookAt(new Vector3(transform.position.x, player.transform.position.y, transform.position.z));

        yield return new WaitForSeconds(0.25f);
        hitBox.enabled = false;
        if (isPlayerInFront)
            StartCoroutine("OpenDoor");
        else
            StartCoroutine("OpenDoorOut");

        yield return new WaitForSeconds(0.25f);

        float doorTime = Time.time+2;

        Vector3 movePos = isPlayerInFront ? new Vector3(backPos.x, player.transform.position.y, backPos.z) :
        new Vector3(frontPos.x, player.transform.position.y, frontPos.z);

        while (Time.time < doorTime)
        {
            player.transform.position = Vector3.Lerp(player.transform.position, movePos, 0.05f);
            yield return null;
        }
        yield return new WaitForSeconds(0.75f);

        player = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.parent.position, Vector3.one/2);
    }
}
