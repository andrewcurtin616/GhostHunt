using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool canMove;
    public bool canBeHit;
    public float lastHit;
    public bool cameraMode;
    public float tempTime;
    public bool catching;

    public bool inHallway;
    public bool inLitRoom;

    public int health;

    public bool inspecting;

    Rigidbody rb;
    BoxCollider hitbox;
    GameObject flashLight;
    GameObject vacuum;
    Camera firstPerson;

    private void Awake()
    {
        canMove = true;
        canBeHit = true;
        rb = GetComponent<Rigidbody>();
        hitbox = GetComponent<BoxCollider>();
        health = 100;
    }

    void Start()
    {
        flashLight = GameObject.Find("Flashlight");
        vacuum = GameObject.Find("Vacuum");
        VacuuumOff();
        firstPerson = GameObject.Find("FirstPerson").GetComponent<Camera>();
        firstPerson.gameObject.SetActive(false);
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (canMove)
        {
            if (h != 0 || v != 0)
                transform.forward = Vector3.Slerp(transform.forward, new Vector3(h, 0, v), 0.2f);

            rb.velocity = new Vector3(h, 0, v) * 3f;
        }

        if (!canBeHit)
        {
            if (!canMove && Time.time > lastHit + 1.5f) //end fall back
                rb.velocity = Vector3.zero;
            if (!canMove && Time.time > lastHit + 3f) //got up, can move
            { canMove = true; FlashLightOn(); }
            if (Time.time > lastHit + 5f) //I-frames
                canBeHit = true;
        }

        if (catching)
            return;

        if (canMove && Input.GetKeyDown(KeyCode.Tab))
        {
            cameraMode = true;
            canMove = false;
            firstPerson.gameObject.SetActive(true);
            flashLight.transform.localRotation = new Quaternion(0, 0, 0, 0); //***
        }

        if (cameraMode)
        {
            transform.Rotate(Vector3.up, h);

            if ((v > 0 && flashLight.transform.localRotation.x > -0.5) ||
                (v < 0 && flashLight.transform.localRotation.x < 0.5))
            {
                flashLight.transform.Rotate(Vector3.right, -v);
                firstPerson.transform.Rotate(Vector3.right, -v);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                canMove = true;
                cameraMode = false;
                flashLight.transform.localRotation = new Quaternion(0, 0, 0, 0);
                firstPerson.transform.localRotation = new Quaternion(0, 0, 0, 0);
                firstPerson.gameObject.SetActive(false);
            }
        } //end cameraMode

        //Flashlight
        if ((canMove || cameraMode) && !Input.GetKey(KeyCode.E)) //Turn light on and off
        {
            if (Input.GetKeyDown(KeyCode.T)) { FlashLightOff(); }
            else if (Input.GetKeyUp(KeyCode.T)) { FlashLightOn(); }
        }
        //end flashlight

        //Vacuum
        if (canMove && Input.GetKeyDown(KeyCode.E)) //"Attacking"
        {
            VacuumOn();
            FlashLightOff();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            VacuuumOff();
            FlashLightOn();
        }
        if (canMove) //Tilt vacuum up and let it down (separate from light for now)
        {
            TiltTool(vacuum);
            TiltTool(flashLight);
        }

        if (catching)
        {
            if (!Input.GetKey(KeyCode.E))
                catching = false;
        }

        if (inspecting)
        {
            if (Time.time > tempTime + 1f)
            {
                inspecting = false;
                canMove = true;
            }
        }

    } //end update

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            GetHit(10);
        }

        if (other.tag == "Hallway")
        {
            inHallway = true;
        }

        if (other.tag == "Heart")
        {
            if (health < 90)
                health += 10;
            else
                health = 100;
            Destroy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Hallway")
        {
            inHallway = false;
        }
    }

    public void GetHit(int damage)
    {
        //Maybe have damage passed in by caller
        //Could use 0 damage to just show a scare
        //face hit?
        //Lower health
        if (!canBeHit)
            return;
        lastHit = Time.time;
        rb.velocity = -transform.forward * 2.5f; //fall back
        canMove = false;
        canBeHit = false;
        cameraMode = false; //Knock out of cameraMode
        inspecting = false; //knock out of inspecting
        health -= damage;
        VacuuumOff();
        FlashLightOff();
        //if damage == 0, play 'scare' animation else play getHit animation
    }

    void VacuumOn()
    {
        vacuum.GetComponents<SphereCollider>()[0].enabled = true;
        vacuum.GetComponents<SphereCollider>()[1].enabled = true;
        vacuum.GetComponents<SphereCollider>()[2].enabled = true;
        //vacuum.GetComponent<ParticleSystem>().enableEmission = true;
        var vacEmission = vacuum.GetComponent<ParticleSystem>().emission;
        vacEmission.enabled = true;
    }
    void VacuuumOff()
    {
        vacuum.GetComponents<SphereCollider>()[0].enabled = false;
        vacuum.GetComponents<SphereCollider>()[1].enabled = false;
        vacuum.GetComponents<SphereCollider>()[2].enabled = false;
        //vacuum.GetComponent<ParticleSystem>().enableEmission = false;
        var vacEmission = vacuum.GetComponent<ParticleSystem>().emission;
        vacEmission.enabled = false;
    }

    void FlashLightOn()
    {
        if (inLitRoom) { return; }
        flashLight.GetComponents<SphereCollider>()[0].enabled = true;
        flashLight.GetComponents<SphereCollider>()[1].enabled = true;
        flashLight.GetComponents<SphereCollider>()[2].enabled = true;
        flashLight.GetComponent<Light>().enabled = true;
    }
    void FlashLightOff()
    {
        flashLight.GetComponents<SphereCollider>()[0].enabled = false;
        flashLight.GetComponents<SphereCollider>()[1].enabled = false;
        flashLight.GetComponents<SphereCollider>()[2].enabled = false;
        flashLight.GetComponent<Light>().enabled = false;
    }

    void TiltTool(GameObject tool)
    {
        if (Input.GetKey(KeyCode.Q) && tool.transform.localRotation.x >= -0.45f)
            tool.transform.Rotate(-1f, 0, 0);
        if (!Input.GetKey(KeyCode.Q))
        {
            if (tool.transform.localRotation.x > 0)
                tool.transform.localRotation = Quaternion.Euler(0, 0, 0);
            else if (tool.transform.localRotation.x != 0)
                tool.transform.Rotate(1f, 0, 0);
        }
    }

    IEnumerator Catching(Ghost ghost)//have ghost pass in info
    {
        catching = true;
        FlashLightOff();
        Vector3 lookPos = Vector3.zero;

        while (Input.GetKey(KeyCode.E) && ghost.health > 0)
        {
            if (!Input.GetKey(KeyCode.E))
            {
                catching = false;
                StopCoroutine("Catching");
            }

            //point vac at ghost?

            lookPos = new Vector3(ghost.transform.position.x, transform.position.y, ghost.transform.position.z);
            transform.LookAt(lookPos);
            if (Vector3.Distance(transform.position, ghost.transform.position) > 2.5f)
                //transform.Translate((lookPos - transform.position).normalized * 0.075f);
                rb.velocity += (lookPos - transform.position).normalized * 3;
            yield return null;
        }
        canMove = false;
        canBeHit = false;
        Renderer rend = ghost.GetComponent<Renderer>();
        while (rend.enabled)
        {
            yield return null;
        }
        canMove = true;
        canBeHit = true;
        catching = false;
        VacuuumOff();
        FlashLightOn();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
