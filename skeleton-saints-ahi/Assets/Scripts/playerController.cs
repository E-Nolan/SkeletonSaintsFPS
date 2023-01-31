using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [SerializeField] CharacterController controller;

    [SerializeField] int playerSpeed;
    [SerializeField] int jumpTimes;
    [SerializeField] int jumpSpeed;
    [SerializeField] int gravity;


    Vector3 move;
    Vector3 playerVelocity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    void movement()
    {
        move = (transform.right * Input.GetAxis("Horizontal") + 
                transform.forward * Input.GetAxis("Vertical"));

        controller.Move(move * Time.deltaTime * playerSpeed);
    }
}
