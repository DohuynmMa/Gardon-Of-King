using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Enter : MonoBehaviour
{
    public int chapter;
    public int level;
    private void Start()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            if(chapter != 99)
            {
                if (DataManager.Instance.data.chapter > chapter)
                {
                    gameObject.SetActive(true);
                }
                else if (DataManager.Instance.data.chapter == chapter)
                {
                    gameObject.SetActive(DataManager.Instance.data.level >= level);
                }
                else gameObject.SetActive(false);
            }
            else gameObject.SetActive(TestManager.Instance.œ‘ æ≤‚ ‘”√πÿø®);
        });
    }
    public void OnButtonClick()
    {
        DialogMainMenu.Instance.startGame(chapter, level);
    }

}
