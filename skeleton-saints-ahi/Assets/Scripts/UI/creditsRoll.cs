using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsRoll : MonoBehaviour
{
    [SerializeField] Vector3 endPosition;
    [SerializeField] float scrollSpeed;
    public bool isScrolling = false;
    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPosition, scrollSpeed * Time.deltaTime);
        }
    }

    public void startScrolling()
    {
        transform.position = startPosition;
        isScrolling = true;
    }

    public void stopScrolling()
    {
        transform.position = startPosition;
        isScrolling = false;
    }
}
