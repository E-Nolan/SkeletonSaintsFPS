using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IKey : collectionItem
{
    [SerializeField] gateButton gBUtton;

    [Tooltip("Positive = Clockwise | Negative = Counter-Clockwise")]
    [Range(-360, 360)] [SerializeField] int rotationSpeed;
    [SerializeField] AudioClip pickupSound;

    [SerializeField]
    bool[] entryLevel = new bool[3];

    public void AssignButton()
    {

        if (entryLevel[0])
        {
            gBUtton = GameObject.FindGameObjectWithTag("RedButton").GetComponent<gateButton>();
        }
        else if (entryLevel[1])
        {
            gBUtton = GameObject.FindGameObjectWithTag("BlueButton").GetComponent<gateButton>();
        }
        else
        {
            gBUtton = GameObject.FindGameObjectWithTag("YellowButton").GetComponent<gateButton>();
        }
    }
    // Update is called once per frame
    void Update()
    {
        //rotates the keycards 
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);
    }
    /// <summary>
    /// when a key card is walked over /picked up destroyed the card objects, updates the ui and updates the bool on whether or not we have collected said keycard
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gBUtton.CanInteractYet = true;
            if (pickupSound)
                gameManager.instance.PlayerScript().audioSource.PlayOneShot(pickupSound);
            //Collect();
            if (entryLevel[0])
            {
                Destroy(gameObject);
                hUDManager.instance.card01.color = new Color(hUDManager.instance.card01.color.r, hUDManager.instance.card01.color.g, hUDManager.instance.card01.color.b, 1f);
                hUDManager.instance.keyCard01Text.color = new Color(hUDManager.instance.keyCard01Text.color.r, hUDManager.instance.keyCard01Text.color.g, hUDManager.instance.keyCard01Text.color.b, 1f);
                gameManager.instance.keyCard[0] = true;
            }
            else if (entryLevel[1])
            {
                Destroy(gameObject);
                hUDManager.instance.card02.color = new Color(hUDManager.instance.card02.color.r, hUDManager.instance.card02.color.g, hUDManager.instance.card02.color.b, 1f);
                hUDManager.instance.keyCard02Text.color = new Color(hUDManager.instance.keyCard02Text.color.r, hUDManager.instance.keyCard02Text.color.g, hUDManager.instance.keyCard02Text.color.b, 1f);
                gameManager.instance.keyCard[1] = true;
            }
            else
            {
                Destroy(gameObject);
                hUDManager.instance.card03.color = new Color(hUDManager.instance.card03.color.r, hUDManager.instance.card03.color.g, hUDManager.instance.card03.color.b, 1f);
                hUDManager.instance.keyCard03Text.color = new Color(hUDManager.instance.keyCard03Text.color.r, hUDManager.instance.keyCard03Text.color.g, hUDManager.instance.keyCard03Text.color.b, 1f);
                gameManager.instance.keyCard[2] = true;
            }
        }
    }
}
