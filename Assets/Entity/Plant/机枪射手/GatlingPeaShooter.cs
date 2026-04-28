using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
/// <summary>
/// ÄãÃÃµÄ À¬»øÒýÇæ
/// </summary>
public class GatlingPeaShooter : Plant
{
    [Header("GatlingPeaShooter:Plant")]
    private int o1 = 14, o2 = 13, o3 = 12, o4 = 11;
    public int temp1, temp2, temp3, temp4;
    public override void Start()
    {
        base.Start();
        DOVirtual.DelayedCall(0.5f, () =>
        {
            foreach (int o in baseSortingOrders)
            {
                if (o == o1) temp1 = Array.IndexOf(baseSortingOrders, o);
                if (o == o2) temp2 = Array.IndexOf(baseSortingOrders, o);
                if (o == o3) temp3 = Array.IndexOf(baseSortingOrders, o);
                if (o == o4) temp4 = Array.IndexOf(baseSortingOrders, o);
            }
        });
    }

    public void fixAnim1()
    {
        if (!tempEnable()) return;
        backOrder();
        baseSortingOrders[temp1] = baseSortingOrders[temp4] + 1;
        baseSortingOrders[temp2] = baseSortingOrders[temp4];
        baseSortingOrders[temp3] = baseSortingOrders[temp4] - 1;
        backOrder();
    }
    public void fixAnim2()
    {
        if (!tempEnable()) return;
        backOrder();
        baseSortingOrders[temp2] = baseSortingOrders[temp1] + 1;
        baseSortingOrders[temp3] = baseSortingOrders[temp1];
        baseSortingOrders[temp4] = baseSortingOrders[temp1] - 1;
    }
    public void fixAnim3()
    {
        if (!tempEnable()) return;
        backOrder();
        baseSortingOrders[temp3] = baseSortingOrders[temp2] + 1;
        baseSortingOrders[temp4] = baseSortingOrders[temp2];
        baseSortingOrders[temp1] = baseSortingOrders[temp2] - 1;
    }
    public void fixAnim4()
    {
        if (!tempEnable()) return;
        backOrder();
        baseSortingOrders[temp4] = baseSortingOrders[temp3] + 1;
        baseSortingOrders[temp1] = baseSortingOrders[temp3];
        baseSortingOrders[temp2] = baseSortingOrders[temp3] - 1;
    }
    private void backOrder()
    {
        if (!tempEnable()) return;
        baseSortingOrders[temp1] = o1;
        baseSortingOrders[temp2] = o2;
        baseSortingOrders[temp3] = o3;
        baseSortingOrders[temp4] = o4;
    }
    private bool tempEnable()
    {
        return temp1 != 0 && temp2 != 0 && temp3 != 0 && temp4 != 0;
    }
}
