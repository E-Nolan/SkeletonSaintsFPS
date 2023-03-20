using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsRoll : MonoBehaviour
{
    [SerializeField] int endPosY;
    [SerializeField] float scrollSpeed;
    public bool isScrolling = false;
    RectTransform rectangle;
    Rect _rect;
    Vector3 startingPos;
    Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        endPos = new Vector3 (rectangle.position.x, endPosY, rectangle.position.z);
        rectangle = GetComponent<RectTransform>();
        _rect = rectangle.rect;
        startingPos = rectangle.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            _rect.position = Vector3.MoveTowards(_rect.position, endPos, scrollSpeed * Time.deltaTime);
            if (endPosY >= _rect.position.y)
                stopScrolling();
        }
    }

    public void startScrolling()
    {
        rectangle.position = startingPos;
        isScrolling = true;
    }

    public void stopScrolling()
    {
        isScrolling = false;
    }
}
