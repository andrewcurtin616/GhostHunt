using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    int stunCount;
    bool inLight;
    int health = 100;
    public enum ghostState { Approaching, Attacking, Laughing, Teleporting, Stun, Capture, Hiding };
    public ghostState state;
    Room room;
    PlayerController player;

    private void Awake()
    {
        //GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 0, 1, 0.1f, 0.25f);
        GetComponent<Renderer>().material.color = Random.ColorHSV(0, 1, 1, 1, 0.5f, 1, 0.25f, 0.5f);
        GetComponent<Light>().color = GetComponent<Renderer>().material.color;
    }

    void Start()
    {
        room = transform.parent.parent.GetComponent<Room>();
        player = FindObjectOfType<PlayerController>();
        state = ghostState.Hiding;
        StartCoroutine("Hide");
    }

    void Update()
    {
        if (inLight)
        {
            stunCount++;
            //expand light effect
            if (stunCount >= 30)
            {
                //attackCollider.enabled = false;
                //attackParticle.Stop();
                StopAllCoroutines();
                StartCoroutine("Stun");
                inLight = false;
                stunCount = 0;
            }
        }
        else if (stunCount > 0)
        {
            stunCount--;
            //decrease light effect
        }
        
    }

    IEnumerator Approach()
    {
        state = ghostState.Approaching;
        float tempFloat = 0.01f;
        float tempTime = Time.time + 4f;
        while (true)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, 0.01f);

            if (transform.position.y > 1.5)
                tempFloat = -0.005f;
            if (transform.position.y < 1)
                tempFloat = 0.005f;
            transform.position += Vector3.up * tempFloat;

            if (Vector3.Distance(player.transform.position, transform.position) < 2.5f && Time.time > tempTime)
                break;

            yield return null;
        }
        Vector3 attackPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        StartCoroutine("Attack", attackPosition);
    }

    IEnumerator Attack(Vector3 attackPosition)
    {
        state = ghostState.Attacking;
        float tempTime = Time.time + 2.5f;
        //attack particles
        yield return new WaitForSeconds(0.5f);
        //attackCollider.enabled = true;
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.forward + attackPosition, 0.1f);
            if (Time.time > tempTime + 2.5f)
                break;
            yield return null;
        }

        if (!player.canBeHit)
        {
            state = ghostState.Laughing;
            //noise/anim
            yield return new WaitForSeconds(3);
            StartCoroutine("Teleport");
        }
        else
        {
            //idle, maybe float/hover?
            yield return new WaitForSeconds(1);
            //put teleport here?
            StartCoroutine("Teleport");
        }
    }

    IEnumerator Stun()
    {
        state = ghostState.Stun;
        //noise, etc.
        yield return new WaitForSeconds(3.5f);

        if (Random.Range(-1, 1) < 0)
            state = ghostState.Hiding;

        StartCoroutine("Teleport");
    }

    IEnumerator Teleport()
    {
        //fade,play noise,anim, etc. (disappear)
        GetComponent<Renderer>().enabled = false; //should be fade and anim
        yield return new WaitForSeconds(2.5f);
        if (state == ghostState.Hiding)
        {
            StartCoroutine("Hide");
            StopCoroutine("Teleport");
            yield return null;
        }
        state = ghostState.Teleporting;
        yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        //choose spot to be in, then fade back in and approach

        /*** Should be reworked to use room.boundaries ***/
        BoxCollider temp = room.GetComponent<BoxCollider>();
        float minX = temp.center.x - temp.size.x / 2 + room.transform.position.x;
        float maxX = temp.center.x + temp.size.x / 2 + room.transform.position.x;
        float minZ = temp.center.z - temp.size.z / 2 + room.transform.position.z;
        float maxZ = temp.center.z + temp.size.z / 2 + room.transform.position.z;
        
        Vector3 spawnPos = Vector3.zero;
        while (true)
        {
            spawnPos = player.transform.position - player.transform.forward * 2; //behind
            if (spawnPos.x < maxX && spawnPos.x > minX && spawnPos.z < maxZ && spawnPos.z > minZ)
                break;

            spawnPos = player.transform.position - player.transform.right * 2; //left
            if (spawnPos.x < maxX && spawnPos.x > minX && spawnPos.z < maxZ && spawnPos.z > minZ)
                break;

            spawnPos = player.transform.position + player.transform.right * 2; //right
            if (spawnPos.x < maxX && spawnPos.x > minX && spawnPos.z < maxZ && spawnPos.z > minZ)
                break;

            spawnPos = player.transform.position + player.transform.forward * 2;//front
            if (spawnPos.x < maxX && spawnPos.x > minX && spawnPos.z < maxZ && spawnPos.z > minZ)
                break;

            spawnPos = room.transform.position + Vector3.up;
            break;
        }
        if (Random.Range(-1, 1) < 0)
            spawnPos = new Vector3(Random.Range(minX, maxX),
                room.transform.position.y + 1, Random.Range(minZ, maxZ));
        transform.position = spawnPos;
        GetComponent<Renderer>().enabled = true; //should be fade and anim
        StartCoroutine("Approach");
    }

    IEnumerator Hide()
    {
        //decide where to hide
        //Decor hidingSpot = room.decorActive[Random.Range(0, room.decorActive.Length)];
        //remember to make decor array of Decor scripts rather than transforms
        transform.position = room.decorActive[Random.Range(0, room.decorActive.Length)].position;
        yield return new WaitForSeconds(5f);
        /*while (true)
        {
            //wait for decor to be shook
            yield return null;
            break;
        }*/
        //reveal, scare, then teleport
        state = ghostState.Teleporting;
        StartCoroutine("Teleport");
    }

    IEnumerator Capture()
    {
        StopCoroutine("Stun");
        state = ghostState.Capture;
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Flashlight" &&
            (state == ghostState.Approaching || state == ghostState.Attacking || state == ghostState.Laughing))
            inLight = true;
        if (other.name == "Vacuum" && state == ghostState.Stun)
            StartCoroutine("Capture");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Flashlight")
            inLight = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
