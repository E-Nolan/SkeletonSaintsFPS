using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class killCondition : eventCondition
{
    public string CollectionName;


    public int enemiesMax;
    public int enemiesLeft;

    public TaskListUI_Kill ConditionUI;

    private void Awake()
    {
        enemiesLeft = enemiesMax;
    }
    public override bool CheckCompletion()
    {
        if (enemiesLeft <= 0)
        {
            satisfied = true;
        }
        else
        {
            satisfied = false;
        }
        return base.CheckCompletion();
    }
    public override void ResetCondition()
    {
        base.ResetCondition();
        enemiesLeft = enemiesMax = 0;
    }


    public void UpdateKillUI(killCondition killCond)
    {
        if (killCond.ConditionUI != null)
        {
            killCond.ConditionUI.ConditionToggle.isOn = satisfied;
            killCond.ConditionUI.ConditionalUIText.text = CollectionName;
            killCond.ConditionUI.killCountText.text = enemiesLeft.ToString() + "   /   " + enemiesMax.ToString();
        }
    }

}
