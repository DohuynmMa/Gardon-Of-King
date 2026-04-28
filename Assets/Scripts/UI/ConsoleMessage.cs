using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConsoleMessage : MonoBehaviour
{
    public GameObject bg;
    public TextMeshProUGUI text;
    
    public void waitToDes(int time)
    {
        DOVirtual.DelayedCall(time, () =>
        {
            GetComponent<Animator>().SetTrigger("disappear");
            DOVirtual.DelayedCall(1.2f, () =>
            {
                onMessageDestroy();
            });
        });
    }
    public void onMessageDestroy()
    {
        ConsoleManager.Instance.showingMessages.Remove(this);
        Destroy(gameObject);
    }
}
