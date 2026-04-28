using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public bool hasDropItem = false;
    public static AchievementManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
}
