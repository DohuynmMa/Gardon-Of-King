using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI_Enter_MiniGame : MonoBehaviour
{
    public GameMode gameMode = GameMode.None;
    public int level = 1;
    private void Start()
    {
        DOVirtual.DelayedCall(0.1f, () =>
        {
            gameObject.SetActive(DataManager.Instance.data.miniGameLevel >= level);
        });
    }
    public void OnButtonClick()
    {
        if (gameMode == GameMode.None) return;
        DialogMainMenu.Instance.changeScene(gameMode, 101, level, 10);
    }

}
