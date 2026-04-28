using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public static Dictionary<string, Action> actions = new Dictionary<string, Action>();
    public static List<string> dontRemoveDialogUIName = new List<string>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public static void registerUI(UIHelper ui, bool dontRemoveWhenChangeScene = false)
    {
        if (ui is not HiddenUI)
        {
            Debug.Log("已注册UI: " + ui.GetName());
        }
        var uiActions = UIHelper.GetAllActions(ui);
        foreach ((var key, var value) in uiActions)
        {
            if (actions.ContainsKey(key)) continue;
            actions.Add(key, value);
            if (!dontRemoveWhenChangeScene) continue;
            var name = key.Split(".")[0];
            if (!dontRemoveDialogUIName.Contains(name))
            {
                print("场景同步UI:" + name);
                dontRemoveDialogUIName.Add(name);
            }
        }
    }
    public void executeAction(string id)
    {
        if (actions.ContainsKey(id))
        {
            Action action = actions[id];
            action.Invoke();
        }
    }
    public void PFReRegisterUI()
    {
        var keys = actions.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            bool shouldRemove = true;
            foreach (var s in dontRemoveDialogUIName)
            {
                if (key.Contains(s))
                {
                    shouldRemove = false;
                    break;
                }
            }
            if (shouldRemove)
            {
                actions.Remove(key);
                keys = actions.Keys.ToList();
                i--;
            }
        }
    }
    internal static void fadeOutAllChild(GameObject gameObject)
    {
        if(gameObject == null) return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            var childTransform = gameObject.transform.GetChild(i).gameObject;
            childTransform.GetComponent<UI_FadeInFadeOut>().UI_FadeOut_Event();
        }
    }
    internal static void fadeInAllChild(GameObject gameObject)
    {
        if (gameObject == null) return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childTransform = gameObject.transform.GetChild(i).gameObject;
            childTransform.GetComponent<UI_FadeInFadeOut>().UI_FadeIn_Event();
        }
    }
    internal static void scaleIn(GameObject gameObject)
    {
        scaleIn(gameObject, 0.2f);
    }
    internal static void scaleIn(GameObject gameObject, float duration)
    {
        if (gameObject == null) return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childTransform = gameObject.transform.GetChild(i).gameObject;
            if (childTransform.tag == "Shadow")
            {
                childTransform.SetActive(true);
                continue;
            }
            var transform = childTransform.GetComponent<RectTransform>();
            transform.DOScaleY(transform.localScale.x, duration);
        }
    }
    internal static void scaleOut(GameObject gameObject)
    {
        if (gameObject == null) return;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            GameObject childTransform = gameObject.transform.GetChild(i).gameObject;
            if (childTransform.tag == "Shadow")
            {
                childTransform.SetActive(false);
                continue;
            }
            var transform = childTransform.GetComponent<RectTransform>();
            transform.DOScaleY(0, 0.2f);
        }
    }
    public static void openUrl(string url)
    {
        Application.OpenURL(url);
    }
}
