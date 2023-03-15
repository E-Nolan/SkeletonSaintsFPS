using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public GameObject CinematicGameObject;

    private Vector3 mainCameraPosition;
    private GameObject tempCinematic;
    private float _timer;

    void Awake()
    {
        mainCameraPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer > 6)
        {
            _timer = 0f;
            if(tempCinematic == null)
                tempCinematic = Instantiate(CinematicGameObject, mainCameraPosition,
                    CinematicGameObject.transform.rotation);
        }
        else
        {
            _timer += Time.deltaTime;

            if (Input.anyKeyDown && tempCinematic.activeSelf)
            {
                Destroy(tempCinematic);
                _timer = 0f;
            }
        }
    }
}
