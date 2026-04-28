using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PeaShooter : Plant
{
    [Header("Peashooter:Plant")]
    public GameObject toFixGainBarCanvas;
    public override void Update()
    {
        base.Update();
        if (GameManager.Instance.gameMode == GameMode.SRCS)//–ﬁ∏¥gainBarœ˚ ßŒ Ã‚
        {
            toFixGainBarCanvas.SetActive(true);
        }
    }
}
