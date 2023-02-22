using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDoor : MonoBehaviour
{
    [SerializeField] bool isDoubleDoor;
    [SerializeField] GameObject door1;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject sensor;

    [SerializeField] bool[] entryLevel = new bool[4];

    [SerializeField] int door1X;
    [SerializeField] int door1Z;

    [SerializeField] int door2X;
    [SerializeField] int door2Z;

    [SerializeField] float openingSpeed;

    private Vector3 door1StartPos;
    private Vector3 door1EndPos;

    private Vector3 door2StartPos;
    private Vector3 door2EndPos;

    private Material go;
    private Material card1;
    private Material card2;
    private Material card3;

    bool isOpen;

    private void Start()
    {
        door1StartPos = door1.transform.position;
        door2StartPos = door2.transform.position;
        door1EndPos = new Vector3(door1.transform.position.x + door1X, door1.transform.position.y, door1.transform.position.z + door1Z);
        door2EndPos = new Vector3(door2.transform.position.x + door2X, door2.transform.position.y, door2.transform.position.z + door1Z);
        go = Resources.Load("Materials/KeyCards/Go", typeof(Material)) as Material;
        card1 = Resources.Load("Materials/KeyCards/Card01", typeof(Material)) as Material;
        card2 = Resources.Load("Materials/KeyCards/Card02", typeof(Material)) as Material;
        card3 = Resources.Load("Materials/KeyCards/Card03", typeof(Material)) as Material;
    }

    private void Update()
    {
        updateSensor();
        if(isOpen)
        {
            if(isDoubleDoor)
            {
                moveDoubleDoor(door1EndPos, door2EndPos);
            }
            else
            {
                moveDoor(door1EndPos);
            }
        }
        else
        {
            if(isDoubleDoor)
            {
                moveDoubleDoor(door1StartPos, door2StartPos);
            }
            else
            {
                moveDoor(door1StartPos);
            }
        }
    }

    private void updateSensor()
    {
        if (entryLevel[0])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = go;
        }
        else if (entryLevel[1])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card1;
        }
        else if (entryLevel[2])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card2;
        }
        else if (entryLevel[3])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card3;
        }
    }

    private void moveDoubleDoor(Vector3 goalPos1, Vector3 goalPos2)
    {
        float dist1 = Vector3.Distance(transform.position, goalPos1);
        float dist2 = Vector3.Distance(transform.position, goalPos2);
        if(dist1 > .1f || dist2 > .1f)
        {
            if(isDoubleDoor)
            {
                door1.transform.position = Vector3.Lerp(door1.transform.position, goalPos1, openingSpeed * Time.deltaTime);
                door2.transform.position = Vector3.Lerp(door2.transform.position, goalPos2, openingSpeed * Time.deltaTime);
            }
        }
    }

    private void moveDoor(Vector3 goalPos)
    {
        float dist1 = Vector3.Distance(transform.position, goalPos);
        if (dist1 > .1f)
        {
            door1.transform.position = Vector3.Lerp(door1.transform.position, goalPos, openingSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isOpen && other.CompareTag("Player"))
        {
            if (entryLevel[0] || entryLevel[1] && gameManager.instance.kCard01 == true || entryLevel[2] && gameManager.instance.kCard02 == true)
            {
                isOpen = !isOpen;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOpen && other.CompareTag("Player"))
        {
            isOpen = !isOpen;
        }
    }
}
