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
        //ColorFade(transform.parent.localEulerAngles.y == 0 || transform.parent.localEulerAngles.y == 180);
        /*Renderer rend = GetComponent<Renderer>();
        Color c = rend.material.color;
        c.a = (transform.parent.localEulerAngles.y == 0 || transform.parent.localEulerAngles.y == 180) ? 0.25f : 1f;
        rend.material.color = c;
        if (GetComponentInParent<DoubleDoor>() != null)
            GetComponentInParent<DoubleDoor>().CheckDoorFade(c);*/

        ColorFade(Physics.Raycast(transform.position, Vector3.forward,2f));
        //ColorFade(FindObjectOfType<PlayerController>().transform.position.z > transform.position.z);

        /*if ((transform.parent.localEulerAngles.y == 0 || transform.parent.localEulerAngles.y == 180))
        {
            Renderer rend = GetComponent<Renderer>();
            Color c = rend.material.color;
            c.a = 0.25f;
            rend.material.color = c;
        }
        else
        {
            Renderer rend = GetComponent<Renderer>();
            Color c = rend.material.color;
            c.a = 1;
            rend.material.color = c;
        }*/
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
        ColorFade(false);
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

    void ColorFade(bool yesFade)
    {
        Renderer rend = GetComponent<Renderer>();
        Color c = rend.material.color;
        c.a = yesFade ? 0.25f : 1f;
        rend.material.color = c;
        if (GetComponentInParent<DoubleDoor>() != null)
            GetComponentInParent<DoubleDoor>().CheckDoorFade(c);

        //could just ask if we're rotated 0,180,360 and if player z pos is greater than ours
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.parent.position, Vector3.one/2);
    }
}
