using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public abstract class TaskListUIElement : MonoBehaviour
{
    public RectTransform canvasTransform;
    public TextMeshProUGUI EventUIText;
    public TextMeshProUGUI ConditionalUIText;
    public Toggle ConditionToggle;

    private void Awake()
    {
        canvasTransform = GetComponent<RectTransform>();
    }
}
