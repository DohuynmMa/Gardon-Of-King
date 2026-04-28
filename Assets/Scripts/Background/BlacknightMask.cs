using Assets.Scripts.Utils;
using DG.Tweening;
using UnityEngine;
public class BlacknightMask : MonoBehaviour
{
    public bool enable = true;
    public GameObject blackBg;
    public float scaleMagn = 1f;
    public Vector3 initialScale;
    SunManager sm;
    private void Awake()
    {
        if (SunManager.Instance != null) sm = SunManager.Instance;
    }
    private void Start()
    {
        if (Utils.scene == "GameMenu") enable = false;
        blackBg.SetActive(enable);
        gameObject.SetActive(enable);
    }
    private void Update()
    {
        if (enable && GameManager.Instance.inGame)
        {
            Vector3 mouseWorldPosition = transform.position;
            if (Application.isMobilePlatform)
            {
                mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            }
            else
            {
                mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            mouseWorldPosition.z = 0;
            transform.position = Vector3.MoveTowards(transform.position, mouseWorldPosition,10 * Vector3.Distance(mouseWorldPosition, transform.position) * Time.deltaTime);
            if (sm == null) return;
            scaleMagn = 1 + (sm.sunPoint / sm.maxSunPoint) * 0.2f;
            transform.localScale = initialScale * scaleMagn;
        }
    }
}
