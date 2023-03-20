using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsRoll : MonoBehaviour
{
    [SerializeField] int endPosY;
    [SerializeField] float scrollSpeed;
    public bool isScrolling = false;
    RectTransform rectangle;
    Vector3 startingPos;
    Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        rectangle = GetComponent<RectTransform>();
        endPos = new Vector3 (rectangle.position.x, endPosY, rectangle.position.z);
        startingPos = rectangle.position;
    }

    // Update is called once per frame
    void Update()
    {
        // AHHHHHH
        if (isScrolling)
        {
            rectangle.position = Vector3.MoveTowards(rectangle.position, endPos, scrollSpeed * Time.deltaTime);
            if (endPosY <= rectangle.position.y)
                stopScrolling();
        }
    }

    public void startScrolling()
    {
        isScrolling = true;
    }

    public void stopScrolling()
    {
        isScrolling = false;
        rectangle.position = startingPos;
    }
}
