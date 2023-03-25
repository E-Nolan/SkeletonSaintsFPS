using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossCondition : eventCondition
{
    public string bossName;

    public TaskListUI_Boss ConditionUI;

    public override bool CheckCompletion()
    {
        if (gameManager.instance.bossIsDead)
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
    }


    public void UpdateBossUI(bossCondition bossCond)
    {
        if (bossCond.ConditionUI != null)
        {
            bossCond.ConditionUI.ConditionToggle.isOn = satisfied;
            bossCond.ConditionUI.EventUIText.text = "Find and ";
            bossCond.ConditionUI.ConditionalUIText.text = "Defeat " + bossName;
        }
    }

}
