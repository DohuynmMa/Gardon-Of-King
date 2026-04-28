using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public int chapter;//章节
    public int level;//关卡
    bool isTouching;
    float touchStartTime;
    private void Update()
    {
        if (Input.GetKey(KeyCode.X) || TestManager.Instance.自动快进对话) // 电脑
        {
            nextDialog();
        }

        if (Application.isMobilePlatform) // Android
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartTime = Time.time;
                        isTouching = true;
                        break;

                    case TouchPhase.Stationary:
                        if (isTouching && Time.time - touchStartTime >= 1f)
                        {
                            // 用户长按超过1秒，快进对话
                            nextDialog();
                        }
                        break;

                    case TouchPhase.Ended:
                        isTouching = false;
                        break;
                }
            }
        }
    }

    public void nextDialog()
    {
        DialogManager.Instance.count++;

        if (chapter == 1)
        {
            switch (level)
            {
                case 1:
                    DialogManager.Instance.c1l1();
                    break;
                case 2:
                    DialogManager.Instance.c1l2();
                    break;
                case 4:
                    DialogManager.Instance.c1l4();
                    break;
                case 5:
                    DialogManager.Instance.c1l5();
                    break;
                case 8:
                    DialogManager.Instance.c1l8();
                    break;
            }
        }
        else if (chapter == 2)
        {
            switch (level)
            {
                case 1:
                    DialogManager.Instance.c2l1();
                    break;
                case 2:
                    DialogManager.Instance.c2l2();
                    break;
                case 3:
                    DialogManager.Instance.c2l3();
                    break;
                case 4:
                    DialogManager.Instance.c2l4();
                    break;
            }
        }
        if (chapter == 100)
        {

            switch (level)
            {
                case 0:
                    DialogManager.Instance.SCRS();
                    break;
                default:
                    break;
            }
        }
    }
}
