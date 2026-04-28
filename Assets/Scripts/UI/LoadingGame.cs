using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingGame : MonoBehaviour
{
    private void Start()
    {
        if (TestManager.Instance.跳过刚开始游戏的加载界面)
        {
            GetComponent<Animator>().speed *= 1000;
        }
    }
    public void finishLoadingGame()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
