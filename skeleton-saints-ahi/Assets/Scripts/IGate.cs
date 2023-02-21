using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGate : MonoBehaviour
{
    public GameObject gate;
    [SerializeField] bool[] entryLevel = new bool[3];
    public Vector3 endPos;
    [SerializeField] float openingSpeed;
    [SerializeField] float delay;

    private Vector3 startPos;
    bool isOpen;

    //0 = no key card needed
    //1 = key card 01 needed
    //2 = key card 02 needed
    private void Start()
    {
        startPos = gate.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            moveGate(endPos);
        }
        else
        {
            moveGate(startPos);
        }
    }

    private void moveGate(Vector3 goalPos)
    {
        float dist = Vector3.Distance(transform.position, goalPos);
        if(dist > .1f)
        {
            gate.transform.position = Vector3.Lerp(gate.transform.position, goalPos, openingSpeed * Time.deltaTime);
        }
    }

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

    private void OnTriggerExit(Collider other)
    {
        if(isOpen && other.CompareTag("Player"))
        {
            isOpen = !isOpen;
        }
    } 
}
