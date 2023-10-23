using UnityEngine;

public class GameEventTriggerMustHaveTendedToAllCropsOnDay : GameEventTriggerAdditionalCondition
{
    [SerializeField] private int dayNo;
    public override bool Condition()
    {
        if (GameManager._Instance.CurrentDay == dayNo)
        {
            return GameManager._Instance.TendedToAllCropsToday;
        }
        return true;
    }
}
