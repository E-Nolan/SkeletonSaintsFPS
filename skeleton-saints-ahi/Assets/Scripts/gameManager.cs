using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Information -----")]
    public GameObject player;
    public playerController playerScript;

    private void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    void Update()
    {
        
    }
}
