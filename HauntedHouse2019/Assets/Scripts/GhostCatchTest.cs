using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCatchTest : MonoBehaviour
{
    Room room;
    PlayerController player;
    Renderer rend;
    Rigidbody rb;

    Vector2 pullDraw = Vector2.zero;
    Vector2 pullBetween = Vector2.zero;

    public int health = 100;

    bool checkTest;

    void Start()
    {
        room = transform.parent.parent.GetComponent<Room>();
        player = FindObjectOfType<PlayerController>();
        //StartCoroutine("CatchTest");
        /*Debug.Log("minx: " + room.boundaries.x);
        Debug.Log("maxx: " + room.boundaries.y);
        Debug.Log("minz: " + room.boundaries.z);
        Debug.Log("maxz: " + room.boundaries.w);*/
        rend = GetComponent<Renderer>();
        rb = player.GetComponent<Rigidbody>();
        StartCoroutine("PullTest");
    }

    void Update()
    {
        /*Vector2 pull = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //pullDraw = pull;
        Vector2 myLine = new Vector2(player.transform.position.x - transform.position.x,
            player.transform.position.z - transform.position.z);
        //pullBetween = myLine;
        Vector3 lookPos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        //pull.x = rb.velocity.x - transform.position.x;
        //pull.y = rb.velocity.z - transform.position.z;
        if (Input.GetKey(KeyCode.E))
        {
            player.transform.LookAt(lookPos);
            //Debug.Log(Vector2.Angle(pullDraw.normalized, pullBetween.normalized));

            //if (Vector2.Angle(pull, myLine) < 10)
              //  rend.material.color = Color.red;

            if (pull != Vector2.zero && Vector2.Angle(pull.normalized, myLine.normalized) < 60)
                rend.material.color = Color.red;
            else
                rend.material.color = Color.white;
        }
        else
        {
            rend.material.color = Color.white;
        }*/

        if (Input.GetKeyDown(KeyCode.E) && !checkTest)
            StartCoroutine("CatchTest");

        if (Input.GetKey(KeyCode.E))
        {
            Vector3 lookPos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
            player.transform.LookAt(lookPos);
            if (Vector3.Distance(transform.position, player.transform.position) > 2.5f && health>0)
                player.transform.Translate((lookPos - player.transform.position).normalized * 0.025f);
        }
    }

    IEnumerator PullTest()
    {
        Vector2 pull = Vector2.zero;
        Vector2 myLine = Vector2.zero;
        while (health > 0)
        {
            pull.x = Input.GetAxis("Horizontal");
            pull.y = Input.GetAxis("Vertical");
            myLine.x = player.transform.position.x - transform.position.x;
            myLine.y = player.transform.position.z - transform.position.z;
            //Vector3 lookPos = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

            if (Input.GetKey(KeyCode.E))
            {
                //player.transform.LookAt(lookPos);

                if (pull != Vector2.zero && Vector2.Angle(pull.normalized, myLine.normalized) < 60)
                {
                    rend.material.color = Color.red;
                    health--;
                    if (Random.Range(-1, 0) == 0)
                        health--;
                    if (Random.Range(-1, 3) >= 0)
                        health--;
                    yield return new WaitForSeconds(0.05f);
                    rend.material.color = Color.white;
                }
                else
                    rend.material.color = Color.white;
            }
            else
            {
                rend.material.color = Color.white;
            }
            yield return null;
        }
        rend.material.color = Color.blue;
    }

    IEnumerator CatchTest()
    {
        //yield return new WaitForSeconds(2.5f);
        checkTest = true;
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
        while (testInt<randInt) //until caught
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
                if (health <= 0)
                    break;
            }
            //testInt++;
            if (health <= 0)
                break;
            yield return null;
        }
        
        //caught, move towards player
        while (Vector3.Distance(transform.position, player.transform.position) > 0.2f)
        {
            newPos = transform.position - player.transform.position;
            transform.Translate(newPos.normalized * 0.05f, Space.Self);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.2f);
            yield return null;
            //break;//temp
        }
        //transform.position = new Vector3(transform.position.x, 1.25f, transform.position.z);
        //StartCoroutine("CatchTest");
        rend.enabled = false;
    }

    private void OnDrawGizmos()
    {
        /*Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pullDraw);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, pullBetween);*/
        
    }
}
