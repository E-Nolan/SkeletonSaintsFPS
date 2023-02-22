using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IKey : MonoBehaviour
{ 
    [Tooltip("Positive = Clockwise | Negative = Counter-Clockwise")]
    [Range(-360, 360)] [SerializeField] int rotationSpeed;
    [SerializeField] AudioClip pickupSound;

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
            switch (gameObject.name)
            {
                case "Card01":
                    Destroy(gameObject);
                    gameManager.instance.card01.color = new Color(gameManager.instance.card01.color.r, gameManager.instance.card01.color.g, gameManager.instance.card01.color.b, 1f);
                    gameManager.instance.keyCard01Text.color = new Color(gameManager.instance.keyCard01Text.color.r, gameManager.instance.keyCard01Text.color.g, gameManager.instance.keyCard01Text.color.b, 1f);
                    gameManager.instance.keyCard[0] = !gameManager.instance.keyCard[0];
                    return;
                case "Card02":
                    Destroy(gameObject);
                    gameManager.instance.card02.color = new Color(gameManager.instance.card02.color.r, gameManager.instance.card02.color.g, gameManager.instance.card02.color.b, 1f);
                    gameManager.instance.keyCard02Text.color = new Color(gameManager.instance.keyCard02Text.color.r, gameManager.instance.keyCard02Text.color.g, gameManager.instance.keyCard02Text.color.b, 1f);
                    gameManager.instance.keyCard[1] = !gameManager.instance.keyCard[1];
                    return;
                case "Card03":
                    Destroy(gameObject);
                    gameManager.instance.card03.color = new Color(gameManager.instance.card03.color.r, gameManager.instance.card03.color.g, gameManager.instance.card03.color.b, 1f);
                    gameManager.instance.keyCard03Text.color = new Color(gameManager.instance.keyCard03Text.color.r, gameManager.instance.keyCard03Text.color.g, gameManager.instance.keyCard03Text.color.b, 1f);
                    gameManager.instance.keyCard[2] = !gameManager.instance.keyCard[];
                    return;

            }
            
        }
    }
}
