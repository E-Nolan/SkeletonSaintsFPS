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
        transform.Rotate(transform.up * rotationSpeed * Time.deltaTime);
    }

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
                    gameManager.instance.kCard01 = !gameManager.instance.kCard01;
                    return;
                case "Card02":
                    Destroy(gameObject);
                    gameManager.instance.card02.color = new Color(gameManager.instance.card02.color.r, gameManager.instance.card02.color.g, gameManager.instance.card02.color.b, 1f);
                    gameManager.instance.keyCard02Text.color = new Color(gameManager.instance.keyCard02Text.color.r, gameManager.instance.keyCard02Text.color.g, gameManager.instance.keyCard02Text.color.b, 1f);
                    gameManager.instance.kCard02 = !gameManager.instance.kCard02;
                    return;
                case "Card03":
                    Destroy(gameObject);
                    gameManager.instance.card03.color = new Color(gameManager.instance.card03.color.r, gameManager.instance.card03.color.g, gameManager.instance.card03.color.b, 1f);
                    gameManager.instance.keyCard03Text.color = new Color(gameManager.instance.keyCard03Text.color.r, gameManager.instance.keyCard03Text.color.g, gameManager.instance.keyCard03Text.color.b, 1f);
                    gameManager.instance.kCard03 = !gameManager.instance.kCard03;
                    return;

            }
            
        }
    }
}
