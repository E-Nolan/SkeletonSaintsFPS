using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGate : MonoBehaviour
{
    public GameObject gate;
    public GameObject sensor;
    //0 = no key card needed
    //1 = key card 01 needed
    //2 = key card 02 needed
    [SerializeField] bool[] entryLevel = new bool[3];

    //determines the y coordinate to the gate (the difference from the starting position and how high you want it to go)
    [SerializeField] int gateY;

    //determineshow fast the gate will be opening
    [SerializeField] float openingSpeed;
    

    private Vector3 startPos;
    private Vector3 endPos;
    private Material card1;
    private Material card2;

    bool isOpen;

    
    private void Start()
    {
        startPos = gate.transform.position;
        endPos = new Vector3(gate.transform.position.x, gate.transform.position.y + gateY, gate.transform.position.z);
        card1 = Resources.Load("Materials/KeyCards/Card01", typeof(Material)) as Material;
        card2 = Resources.Load("Materials/KeyCards/Card02", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
        updateSensor();
        if(isOpen)
        {
            moveGate(endPos);
        }
        else
        {
            moveGate(startPos);
        }
    }

    private void updateSensor()
    {
        if(entryLevel[0])
        {
            sensor.SetActive(false);
        }
        else if(entryLevel[1])
        {
            sensor.SetActive(true);
            sensor.GetComponent<MeshRenderer>().materials[0] = card1;
        }
        else if(entryLevel[2])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card2;
        }
    }

    //moves the gate according tpo the goalPos
    private void moveGate(Vector3 goalPos)
    {
        float dist = Vector3.Distance(transform.position, goalPos);
        if(dist > .1f)
        {
            gate.transform.position = Vector3.Lerp(gate.transform.position, goalPos, openingSpeed * Time.deltaTime);
        }
    }

    //triggers gate logic when  player walks into the trigger
    private void OnTriggerEnter(Collider other)
    {
        if(!isOpen && other.CompareTag("Player"))
        {
            if (entryLevel[0] || entryLevel[1] && gameManager.instance.kCard01 == true || entryLevel[2] && gameManager.instance.kCard02 == true)
            {
                isOpen = !isOpen;
            }
        }
    }

    //activates when the player leavers the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if(isOpen && other.CompareTag("Player"))
        {
            isOpen = !isOpen;
        }
    } 
}
