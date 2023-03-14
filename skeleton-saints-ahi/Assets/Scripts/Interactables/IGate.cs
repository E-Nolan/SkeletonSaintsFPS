using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGate : MonoBehaviour
{
    [SerializeField]
    gateButton parentButton;

    public GameObject gate;
    public GameObject sensor;
    //0 = no key card needed
    //1 = key card 01 needed
    //2 = key card 02 needed
    //3 = key card 03 needed
    [SerializeField] 
    bool[] entryLevel = new bool[4];

    private Animator anim;
    //materail renders for the sensors
    private Material go;
    private Material card1;
    private Material card2;
    private Material card3;

    
    public bool isUnlocked;
    //recognizes if the gate is open or closed
    public bool isOpen;

    private void Awake()
    {
        anim = GetComponentInParent<Animator>();
        isOpen = false;
        
    }
    private void Start()
    {
        go = Resources.Load("Materials/KeyCards/Go", typeof(Material)) as Material;
        card1 = Resources.Load("Materials/KeyCards/Card01", typeof(Material)) as Material;
        card2 = Resources.Load("Materials/KeyCards/Card02", typeof(Material)) as Material;
        card3 = Resources.Load("Materials/KeyCards/Card03", typeof(Material)) as Material;
        AffirmAccessLevel();
    }
    private void AffirmAccessLevel()
    { 
        if(entryLevel[0])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = go;
        }
        else if(entryLevel[1])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card1;
        }
        else if(entryLevel[2])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card2;
        }
        else if(entryLevel[3])
        {
            sensor.SetActive(true);
            sensor.GetComponent<Renderer>().material = card3;
        }
    }

    //triggers gate logic when  player walks into the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            if (!isUnlocked && !parentButton.CanInteractYet)
            {
                hUDManager.instance.setGate(true, false);
            } 
            else if (!isUnlocked && parentButton.CanInteractYet)
            {
                hUDManager.instance.setGate(true, true);
            }
            else if (!isOpen && parentButton.InteractedOnce)
            {
                ActivateGate();
            }
        }
    }

    //activates when the player leavers the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isUnlocked && isOpen)
            {
                ActivateGate();
            }
            hUDManager.instance.setGate(false);
        }
    }
    public void ActivateGate()
    {
        AnimationReaction doorAction = ScriptableObject.CreateInstance<AnimationReaction>();
        doorAction.instruction = 1;
        doorAction.animator = anim;
        doorAction.text = "Activated";
        doorAction.React(this);
        isOpen = !isOpen;
    }
}
