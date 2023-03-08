using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class collectionItem : MonoBehaviour, ICollectibleItem
{
    public collectionCondition parentCondition;
    public void Collect()
    {
        parentCondition.FoundObjectives++;
        parentCondition.CheckCompletion();
    }
}
