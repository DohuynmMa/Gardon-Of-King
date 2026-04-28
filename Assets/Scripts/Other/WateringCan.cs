using UnityEngine;

public class WateringCan : MonoBehaviour
{
    public bool usingWateringCan = false;
    public GameObject lockObj;
    public GameObject aimTag;
    public GameObject num2;
    public GameObject wateringCanButton;
    public bool watering;
    Vector2 oldButtonSize;
    public Entity willWaterEntity;
    private void Start()
    {
        num2.SetActive(!Application.isMobilePlatform);
        oldButtonSize = wateringCanButton.GetComponent<RectTransform>().sizeDelta;
    }
    private void Update()
    {
        if (GameManager.Instance.gameMode != GameMode.SRCS) return;
        if (Input.GetKeyDown(KeyCode.Alpha2) && !DialogConsole.Instance.inTyping)
        {
            if (DataManager.Instance.data.hasWateringCan)
            {
                useWateringCan();
            }
        }
        if (DataManager.Instance.data.hasWateringCan)
        {
            lockObj.SetActive(false);
            gameObject.SetActive(true);
            if(!watering) updatePos();
        }
        else
        {
            lockObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
    public void useWateringCan()
    {
        if (DataManager.Instance.data.hasWateringCan)
        {
            if (!usingWateringCan && !HandManager.Instance.handingEntity && !HandManager.Instance.shovel.usingShovel)
            {
                Sounds.tap.play();
                usingWateringCan = true;
                wateringCanButton.GetComponent<RectTransform>().sizeDelta *= 1000;
            }
            else wateringPlant();
        }
    }
    private void updatePos()
    {
        if (usingWateringCan)
        {
            HandManager.Instance.wateringCanFollow();
        }
        else
        {
            transform.position = lockObj.transform.position;
        }
    }
    public void wateringPlant()
    {
        if (willWaterEntity != null)
        {
            watering = true;
            GetComponent<Animator>().SetTrigger("water");
            Sounds.watering.play();
        }
        else
        {
            usingWateringCan = false;
        }
        wateringCanButton.GetComponent<RectTransform>().sizeDelta = oldButtonSize;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Entity")
        {
            Entity entity = other.GetComponent<Entity>();
            if (entity.entityGroup == EntityGroup.enemy || entity.hasParent) return;
            if (!usingWateringCan) return;
            if (!entity.needingWater) return;
            if (entity.moveSpeed == 0)//判断是不是植物
            {
                aimTag.SetActive(true);
                willWaterEntity = entity;
                Vector2 colliderPos2 = entity.boxCollider.bounds.center;
                Vector3 colliderPos3 = new Vector3(colliderPos2.x, colliderPos2.y, entity.transform.position.z);
                aimTag.transform.position = colliderPos3;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Entity")
        {
            if (other.GetComponent<Entity>().moveSpeed == 0)//判断是不是植物
            {
                willWaterEntity = null;
                aimTag.SetActive(false);
            }
        }
    }
    private void waterEvent()
    {
        if (willWaterEntity == null) return;
        willWaterEntity.watered();
        usingWateringCan = false;
        watering = false;
    }
}
