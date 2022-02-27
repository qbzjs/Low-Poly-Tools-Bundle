using System.Collections.Generic;
using UnityEngine;

public class LogisticsUser : InventoryUser
{
    protected Dictionary<string, LogisticValue> logisticValues;

    [SerializeField] private LogisticValue[] specialLogisticValues;
    [SerializeField] private int defaultPriorityBeeingEmpty;
    [SerializeField] private int defaultPriorityBeeingFull;
    [SerializeField] private uint defaultTargetAmount;

    public List<LogisticCommission>[] UpdateLogisticCommissions()
    {
        Dictionary<string, uint> storedGoods = inventory.GetStoredGoods();

        List<LogisticCommission> logisticInCommissions = new();
        List<LogisticCommission> logisticOutCommissions = new();

        foreach (LogisticValue logisticValue in logisticValues.Values)
        {
            uint currentAmount = storedGoods[logisticValue.goodName];

            if (currentAmount == logisticValue.targetAmount) continue;
            else if (currentAmount > logisticValue.targetAmount)
            {
                logisticOutCommissions.Add(new LogisticCommission(this,
                                                                  logisticValue.goodName,
                                                                  currentAmount - logisticValue.targetAmount,
                                                                  logisticValue.PriorityForHigherAmount(currentAmount, inventoryCapacity)));
            }
            else
            {
                logisticInCommissions.Add(new LogisticCommission(this, logisticValue.goodName, logisticValue.targetAmount, logisticValue.PriorityForLowerAmount(currentAmount)));
            }
        }
        return new List<LogisticCommission>[] { logisticInCommissions, logisticOutCommissions };
    }

    protected void SetLogisticsValues()
    {
        if (inventory == null)
        {
            InitializeInventory();
        }

        //TODO switch to only specialized for produktion/construction or default values for storage
        Dictionary<string, LogisticValue> logisticsValues = new();
        if (specialLogisticValues.Length <= 0)
        {
            foreach (Good good in GoodManager.GetInstance().GetGoodDictionary().Values)
            {
                logisticsValues.Add(good.goodName, new LogisticValue(good.goodName, defaultPriorityBeeingEmpty, defaultPriorityBeeingFull, defaultTargetAmount));
            }
        }
        else
        {
            foreach (LogisticValue logisticsValue in specialLogisticValues)
            {
                logisticsValues[logisticsValue.goodName] = logisticsValue;
            }
        }
        this.logisticValues = logisticsValues;
        return;
    }
}