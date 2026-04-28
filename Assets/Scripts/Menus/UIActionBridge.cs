using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionBridge : MonoBehaviour
{
    public static UIActionBridge Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// 用于其他场景按钮传递Action id
    /// </summary>
    /// <param name="id"></param>
    public void a_executeActionUIManager(string id)
    {
        UIManager.Instance.executeAction(id);
    }
}
