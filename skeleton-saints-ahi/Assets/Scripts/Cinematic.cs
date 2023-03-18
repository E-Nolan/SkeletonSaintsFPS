using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cinematic : MonoBehaviour
{
    [Header("----- Cameras -----")]
    [SerializeField] private Camera cam;
    [SerializeField] private Camera uiCamera;

    [Header("----- Camera Vars -----")]
    [SerializeField] private GameObject nameText;
    [SerializeField] private GameObject pressAnyKeyText;
    [SerializeField] private GameObject speedLines;
    [Range(0.1f, 5f)] [SerializeField] private float camSpeed;
    [SerializeField] private Vector3 cameraOffset;

    [Header("----- Targets -----")]
    [SerializeField] private GameObject[] targets;
    [Range(0.1f, 10f)] [SerializeField] private float focusTimer;
    [HideInInspector] public GameObject CurrentTarget;
    [SerializeField] private bool reachedTargetPosition;
    [SerializeField] private bool isWaiting;

    private Camera camMain;
    private GameObject mainMenu;
    private Vector3 targetPosition;
    private int _index;

    void Awake()
    {
        camMain = Camera.main;

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            targets = GameObject.FindGameObjectsWithTag("Enemy");
        }

        CurrentTarget = targets[0];
        mainMenu = menuManager.instance.mainMenu;


        //uiCamera.gameObject.SetActive(false);
        pressAnyKeyText.gameObject.SetActive(true);
        nameText.gameObject.SetActive(false);
        speedLines.gameObject.SetActive(false);
        camMain.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        reachedTargetPosition = false;

        if (focusTimer <= 0)
            focusTimer = 1f;

        _index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_index >= targets.Length)
            _index = 0;

        CurrentTarget = targets[_index];

        targetPosition = targets[_index].transform.position +
                         new Vector3(0f, targets[_index].transform.localScale.y * 2, 0f);

        reachedTargetPosition = Math.Abs(transform.position.x - targetPosition.x) < 0.01f;

        if (reachedTargetPosition)
        {
            if(!isWaiting)
                StartCoroutine(Delay(focusTimer));

            transform.position = targetPosition;
            nameText.GetComponent<TextMeshPro>().SetText(targets[_index].name);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * camSpeed * 10f);
        }

        nameText.GetComponent<TextMeshPro>().enabled = isWaiting;

        if (isWaiting)
        {
            //uiCamera.gameObject.SetActive(true);
            nameText.gameObject.SetActive(true);
            speedLines.gameObject.SetActive(true);
        }
        else
        {
            //uiCamera.gameObject.SetActive(false);
            nameText.gameObject.SetActive(false);
            speedLines.gameObject.SetActive(false);
        }

        transform.LookAt(targets[_index].transform.position.normalized);
    }

    // Cinematic prefab object is intended to be destroyed when not in use
    // and Instantiated when needed
    void OnDestroy()
    {
        if (camMain != null)
        {
            transform.position = camMain.transform.position;
            camMain.gameObject.SetActive(true);
            mainMenu.gameObject.SetActive(true);
        }

    }

    private IEnumerator Delay(float delay)
    {
        isWaiting = true;
        yield return new WaitForSeconds(delay);
        isWaiting = false;
        reachedTargetPosition = false;
        _index++;
    }

}
