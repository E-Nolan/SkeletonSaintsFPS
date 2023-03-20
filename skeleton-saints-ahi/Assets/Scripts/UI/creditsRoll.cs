using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class creditsRoll : MonoBehaviour
{
    [SerializeField] int endHeight;
    [SerializeField] float scrollSpeed;
    public bool isScrolling = false;
    RectTransform rectangle;
    Rect _rect;
    float startingHeight;
    // Start is called before the first frame update
    void Start()
    {
        rectangle = GetComponent<RectTransform>();
        _rect = rectangle.rect;
        startingHeight = rectangle.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScrolling)
        {
            _rect.height += scrollSpeed * Time.deltaTime;
            if (endHeight >= _rect.height)
                stopScrolling();
        }
    }

    public void startScrolling()
    {
        _rect.height = startingHeight;
        isScrolling = true;
    }

    public void stopScrolling()
    {
        isScrolling = false;
    }
}
