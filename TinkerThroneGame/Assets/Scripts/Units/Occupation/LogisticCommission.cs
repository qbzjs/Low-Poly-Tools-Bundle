﻿using System;

public struct LogisticCommission : IComparable<LogisticCommission>
{
    public readonly Inventory sourceInventory;
    public readonly string goodName;
    public readonly uint amount;
    public readonly float priority;

    public LogisticCommission(Inventory sourceInventory, string goodName, uint amount, float priority)
    {
        this.sourceInventory = sourceInventory;
        this.goodName = goodName;
        this.amount = amount;
        this.priority = priority;
    }

    public int CompareTo(LogisticCommission other)
    {
        return priority.CompareTo(other.priority);
    }
}

