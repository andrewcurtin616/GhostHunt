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
        StartCoroutine("Approach");
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
                state = ghostState.Stun;
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
            StartCoroutine("Teleport");
        }
    }

    IEnumerator Stun()
    {
        state = ghostState.Stun;
        yield return new WaitForSeconds(3.5f);
        //teleport
    }

    IEnumerator Teleport()
    {
        //fade,play noise,anim, etc. (disappear)
        yield return new WaitForSeconds(2.5f);
        if (state == ghostState.Hiding)
        {
            StartCoroutine("Hide");
            StopCoroutine("Teleport");
            yield return null;
        }
        state = ghostState.Teleporting;
        //choose spot to be in, then fade back in and approach
    }

    IEnumerator Hide()
    {
        //decide where to hide
        //wait for decor to be shook (loop)
        yield return null;
        //reveal, scare, then teleport
    }

    IEnumerator Capture()
    {
        state = ghostState.Capture;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
