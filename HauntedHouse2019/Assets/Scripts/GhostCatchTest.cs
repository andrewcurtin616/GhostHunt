using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCatchTest : MonoBehaviour
{
    Room room;
    PlayerController player;

    void Start()
    {
        room = transform.parent.parent.GetComponent<Room>();
        player = FindObjectOfType<PlayerController>();
        StartCoroutine("CatchTest");
        /*Debug.Log("minx: " + room.boundaries.x);
        Debug.Log("maxx: " + room.boundaries.y);
        Debug.Log("minz: " + room.boundaries.z);
        Debug.Log("maxz: " + room.boundaries.w);*/
    }

    void Update()
    {
        
    }

    IEnumerator CatchTest()
    {
        yield return new WaitForSeconds(2.5f);
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + 1.25f, transform.position.z);
        
        //move up
        while (transform.position.y < newPos.y-0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, newPos, 0.05f);
            yield return null;
        }
        
        yield return new WaitForSeconds(0.25f);

        //pick rnd point, turn to face and move forward
        //restricted movement to +-1 x and z limit of room

        int testInt = 0;
        int randInt = Random.Range(3, 8);
        float turnTightness = 0;
        float speed = 0;
        while (testInt < randInt) //until caught
        {
            newPos.x = Random.Range(room.boundaries.x, room.boundaries.y );
            newPos.y = transform.localPosition.y;
            newPos.z = Random.Range(room.boundaries.z, room.boundaries.w);
            turnTightness = Random.Range(0.05f, 0.15f);
            speed = Random.Range(0.05f, 0.1f);
            float safetyTimer = Time.time + 5f;
            while (Vector3.Distance(transform.localPosition,newPos) > 0.5f || safetyTimer<Time.time)
            {
                //move forward towards newPos, turning to face with random turn tightness
                Vector3 newDirection = (newPos - transform.localPosition).normalized;
                transform.forward = Vector3.Lerp(transform.forward, newDirection, turnTightness);
                transform.Translate(Vector3.forward * speed, Space.Self);
                transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, room.boundaries.x - 0.15f, room.boundaries.y + 0.15f),
                    transform.localPosition.y, Mathf.Clamp(transform.localPosition.z, room.boundaries.z - 0.15f, room.boundaries.w + 0.15f));
                yield return null;
            }
            //testInt++;
            yield return null;
        }
        
        //caught, move towards player
        while (Vector3.Distance(transform.position, player.transform.position) < 0.2f)
        {

            yield return null;
            break;
        }
        transform.position = new Vector3(transform.position.x, 1.25f, transform.position.z);
        StartCoroutine("CatchTest");
    }
}
