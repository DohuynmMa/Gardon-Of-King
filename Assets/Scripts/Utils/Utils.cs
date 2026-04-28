using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;

namespace Assets.Scripts.Utils
{
    static class Utils
    {
        public static string scene
        {
            get
            {
                return SceneManager.GetActiveScene().name;
            }
        }

        public static void run(Action action)
        {
            BridgeManager.Instance.BeginInvoke(action);
        }

        /// <summary>
        /// 获取两个2D碰撞箱之间的最近距离
        /// </summary>
        /// <returns></returns>
        public static float dist(this Collider2D one, Collider2D another)
        {
            return Vector3.Distance(one.ClosestPoint(another.bounds.center), another.ClosestPoint(one.bounds.center));
        }
        /// <summary>
        /// 判断阵营是否为friend方
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>

        public static bool isFriend(this EntityGroup group)
        {
            return group == EntityGroup.friend;
        }
        /// <summary>
        /// 移动UI到transform的坐标
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="pos"></param>
        public static void uiMove(GameObject ui, Vector3 pos)
        {
            if (ui == null) return;
            var mScreenPos = Camera.main.WorldToScreenPoint(pos);
            Vector2 mRectPos;
            if (ui.transform.parent.GetComponent<Canvas>() == null)
            {
                Debug.Log("null uiParent");
                return;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(ui.transform.parent.GetComponent<Canvas>().GetComponent<RectTransform>(), mScreenPos, null, out mRectPos);
            ui.GetComponent<RectTransform>().anchoredPosition = mRectPos;
        }

        public static Tower newTower(this GameManager gm, int towerID, Vector3 pos, EntityGroup entityGroup, EntityType attachEntityType,Func<float, float> overrideHP = null,bool inNight = false)
        {
            var tower = GameObject.Instantiate(gm.tower, pos, Quaternion.identity).GetComponent<Tower>();
            tower.entityGroup = entityGroup;
            tower.towerID = towerID;
            if (overrideHP != null)
            {
                var maxHp = overrideHP.Invoke(tower.towerMaxHp);
                tower.towerMaxHp = maxHp;
            }

            if (gm.gameMode == GameMode.SRCS)
            {
                tower.updateTranslate();
            }
            tower.towerHp = tower.towerMaxHp;
            tower.trueHpBarInner.sprite = ImageManager.Instance.hpBarImages[entityGroup.isFriend() ? 4 : 5];

            var currentPos = tower.transform.position;
            currentPos.x += tower.offsetX; currentPos.y += tower.offsetY;

            tower.putEntity(currentPos, attachEntityType);
            if (inNight)
            {
                foreach (var sp in tower.gameObject.getAllSR())
                {
                    sp.material = BridgeManager.Instance.inDarkMaterial;
                }
            }
            return tower;
        }

        public static Home newHome(this GameManager gm, Vector3 pos, EntityGroup entityGroup, HomeNpcType homeNpcType, Func<float, float> overrideHP = null)
        {
            var home = GameObject.Instantiate(gm.home, pos, Quaternion.identity).GetComponent<Home>();
            home.entityGroup = entityGroup;
            home.transitionToDisable();
            home.hpBar.sprite = ImageManager.Instance.hpBarImages[entityGroup.isFriend() ? 8 : 9];
            home.hpBarInner.sprite = ImageManager.Instance.hpBarImages[entityGroup.isFriend() ? 6 : 7];
            home.hpBar.GetComponent<RectTransform>().localScale *= 1.5f;
            if (overrideHP != null)
            {
                var maxHp = overrideHP.Invoke(home.maxHitpoint);
                home.maxHitpoint = maxHp;
            }

            if(gm.gameMode == GameMode.SRCS)
            {
                home.updateHomeTranslate();
                home.hitpoint = home.maxHitpoint;
            }

            if(homeNpcType != HomeNpcType.none)
            {
                var homeNpc = GameObject.Instantiate(gm.getHomeNpcPrefab(homeNpcType));
                homeNpc.home = home;

                homeNpc.transform.position = new Vector3(entityGroup == EntityGroup.friend ? -8.46f : 8.538f, -3.83f, 0);
                homeNpc.transform.localScale = new Vector3(entityGroup == EntityGroup.friend ? -0.45f : 0.45f, 0.45f, 0.45f);
                homeNpc.target.GetComponent<SpriteRenderer>().sprite = homeNpc.target.GetComponent<Home_Target>().aimSprite[entityGroup == EntityGroup.enemy ? 1 : 0];

                home.npc = homeNpc;
                home.npc.gameObject.SetActive(false);
            }
            return home;
        }
        /// <summary>
        /// 改变背景
        /// </summary>
        /// <param name="backGround"></param>
        public static void changeBackground(BackGround backGround)
        {
            var objs = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            foreach (var bg in objs)
            {
                if(bg.GetComponent<BackGround>() != null)
                {
                    BridgeManager.Destroy(bg);
                }
            }
            var newBg = BridgeManager.Instantiate(backGround);
            if(GameManager.Instance != null) GameManager.Instance.BG = newBg;
        }
        /// <summary>
        /// 根据Type改变背景
        /// </summary>
        /// <param name="backGroundType"></param>
        public static void changeBackground(BackgroundType backGroundType)
        {
            var backGround = findBackgroundByType(backGroundType);
            changeBackground(backGround);
        }
        /// <summary>
        /// 根据Type找到背景预制体
        /// </summary>
        /// <param name="backGroundType"></param>
        /// <returns></returns>
        public static BackGround findBackgroundByType(BackgroundType backGroundType)
        {
            foreach(var bg in BridgeManager.Instance.backGroundPrefabs)
            {
                if(bg.backgroundType == backGroundType)
                {
                    return bg;
                }
            }
            return null;
        }
        /// <summary>
        /// 迷你游戏:根据Type找到特定的食物
        /// </summary>
        /// <param name="foodType"></param>
        /// <returns></returns>
        public static FatDaveFood findDaveFoodByType(FatDaveFoodType foodType)
        {
            foreach (var food in BridgeManager.Instance.daveFoodPrefabs)
            {
                if (food.foodType == foodType)
                {
                    return food;
                }
            }
            return null;
        }
        /// <summary>
        /// 显示关卡提示并且准备开始游戏
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="tips1"></param>
        /// <param name="tips2"></param>
        /// <param name="bgm"></param>
        /// <param name="chapter"></param>
        /// <param name="level"></param>
        /// <param name="startSunCount"></param>
        /// <param name="maxSunCount"></param>
        /// <param name="sunAddSpeedTime"></param>
        /// <param name="waitMusic"></param>
        /// <param name="gameMode"></param>
        public static void showTargetDialogAndStartGame(this DialogManager dm, string tips1, string tips2, Musics bgm, int chapter, int level, float startSunCount = 7,float maxSunCount = 10,float sunAddSpeedTime = 1,bool waitMusic = true,GameMode gameMode = GameMode.Normal)
        {
            var gm = GameManager.Instance;
            var sun = SunManager.Instance;
            var tm = TestManager.Instance;

            //准备游戏
            if (waitMusic) Musics.等待音乐.play(true);
            dm.npcBox.SetActive(false);
            dm.gaoshi.SetActive(true);
            dm.tishiText1.text = tips1;
            dm.tishiText2.text = tips2;
            dm.gaoshi.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -14f), tm.快进开始游戏前木牌动画 ? 0.01f : 1f).OnComplete(() => {
                //测试的时候可以改成0.1s 
                DOVirtual.DelayedCall(tm.快进开始游戏前木牌动画 ? 0.01f : 2f, () =>
                {
                    dm.gaoshi.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 960f), tm.快进开始游戏前木牌动画 ? 0.01f : 1f).OnComplete(() => {
                        gm.setTower(chapter,level);
                        var waitTime = 0f;

                        //特殊等待设置
                        if(level == 8 && chapter == 1)
                        {
                            gm.summonDaveBoss();
                            waitTime = 2.1f;
                        }
                        else if(inNight())
                        {
                            waitTime = 2;
                            var mask = gm.BG.blacknightMask;
                            if (chapter == 2)
                            {
                                //关卡越往后,可见范围越小
                                mask.initialScale /= (float)((level - 1) * 0.2 + 1);
                            }

                            mask.transform.DOScale(mask.initialScale, waitTime).SetEase(Ease.OutQuad);
                        }

                        // 「准备 好 开始」动画
                        DOVirtual.DelayedCall(waitTime, () =>
                        {
                            if (!tm.取消准备好开始动画)
                            {
                                GameObject.Instantiate(dm.zbhks, new Vector3(0, 0, 0), Quaternion.identity);
                            }
                            DOVirtual.DelayedCall(tm.取消准备好开始动画 ? 0.01f : 1.9f, () =>
                            {
                                bgm.play(true);

                                sun.addTime = sunAddSpeedTime;
                                sun.maxSunPoint = maxSunCount;

                                gm.startSunPoint = startSunCount;
                                gm.gameStart(chapter, level);
                            });
                        });
                    });
                });
            });
        }
        /// <summary>
        /// 检测物体内部所有物体的SpriteRenderer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static SpriteRenderer[] getAllSR(this GameObject obj)
        {
            SpriteRenderer[] sr = obj.GetComponentsInChildren<SpriteRenderer>(true);
            if (obj.transform.childCount != 0)
            {
                List<SpriteRenderer> sr2 = sr.ToList();
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    sr2.AddRange(getAllSR(obj.transform.GetChild(i).gameObject).ToList());
                }
                return sr2.ToArray();
            }
            else return sr;
        }
        /// <summary>
        /// 找到所有Cell
        /// </summary>
        /// <returns></returns>
        public static List<Cell> findAllCells()
        {
            var objs = GameObject.FindGameObjectsWithTag("Cell");
            List<Cell> cells = new List<Cell>();
            foreach(var obj in objs)
            {
                if (obj == null || obj.GetComponent<Cell>() == null) continue;
                var cell = obj.GetComponent<Cell>();
                cells.Add(cell);
            }
            return cells;
        }
        /// <summary>
        /// 根据阵营找到所有cell
        /// </summary>
        /// <returns></returns>
        public static List<Cell> findAllCells(EntityGroup group)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Cell");
            List<Cell> cells = new List<Cell>();
            foreach (GameObject obj in objs)
            {
                if (obj == null || obj.GetComponent<Cell>() == null) continue;
                Cell cell = obj.GetComponent<Cell>();

                if (group == EntityGroup.friend)
                {
                    if (cell.cellArea != 1) continue;
                }
                else
                {
                    if (cell.cellArea != 2 && cell.cellArea != 3) continue;
                }

                cells.Add(cell);
            }
            return cells;
        }
        /// <summary>
        /// 根据Area找到所有Cell
        /// </summary>
        /// <returns></returns>
        public static List<Cell> findAllCellsByArea(int cellArea)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Cell");
            List<Cell> cells = new List<Cell>();
            foreach (GameObject obj in objs)
            {
                if (obj == null || obj.GetComponent<Cell>() == null) continue;
                Cell cell = obj.GetComponent<Cell>();
                if (cell.cellArea != cellArea) continue;

                cells.Add(cell);
            }
            return cells;
        }
        /// <summary>
        /// 通过Id找到格子
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        public static Cell findCellById(int cellId)
        {
            foreach (var cell in findAllCells())
            {
                if (cellId == cell.ID) return cell;
            }
            return null;
        }
        /// <summary>
        /// 找到所有实体 不包括Tower的实体和Home
        /// </summary>
        /// <returns></returns>
        public static List<Entity> findAllEntities()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Entity");
            List<Entity> entities = new List<Entity>();
            foreach(GameObject obj in objs)
            {
                if (obj.GetComponent<Entity>() == null) continue;
                Entity entity = obj.GetComponent<Entity>();
                if (entity.hasParent) continue;
                entities.Add(entity);
            }
            return entities;
        }
        /// <summary>
        /// 获取该队伍全部实体 不包括Tower和Home
        /// </summary>
        /// <returns></returns>
        public static List<Entity> findAllEntitiesByGroup(EntityGroup group)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Entity");
            List<Entity> entities = new List<Entity>();
            foreach (GameObject obj in objs)
            {
                if (obj.GetComponent<Entity>() == null) continue;
                Entity entity = obj.GetComponent<Entity>();
                if (entity.hasParent || entity.entityGroup != group) continue;
                entities.Add(entity);
            }
            return entities;
        }
        /// <summary>
        /// 找到所有Tower 和 Home
        /// </summary>
        /// <returns></returns>
        public static List<GameObject> findAllTowerAndHome()
        {
            var objs1 = GameObject.FindGameObjectsWithTag("Home");
            var objs = new List<GameObject>();
            foreach (var obj in objs1)
            {
                objs.Add(obj);
            }
            var objs2 = GameObject.FindGameObjectsWithTag("Tower");
            foreach (var obj in objs2)
            {
                objs.Add(obj);
            }
            return objs;
        }
        /// <summary>
        /// 给礼物(卡牌等物品)
        /// </summary>
        /// <param name="item"></param>
        public static void gift(Item item, Vector3 pos, bool win = false)
        {
            var gift = GameObject.Instantiate(item, pos, Quaternion.identity);
            gift.isGameWin = win;
        }
        /// <summary>
        /// 给金币
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        public static void giveCoinBag(Item item, int count, Vector3 pos, bool win = false)
        {
            Sounds.coin_click.play();
            var gift = GameObject.Instantiate(item, pos, Quaternion.identity);
            gift.GetComponent<CoinBag>().addCoinCount = count;
            gift.isGameWin = win;
        }
        /// <summary>
        /// 掉落金币
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="time">次数</param>
        public static void summonCoin(Vector3 pos,int time)
        {
            for (int i = 0; i < time; i++)
            {
                GameObject.Instantiate(CoinManager.Instance.coinPrefebs[0], pos, Quaternion.identity);
            }
        }
        /// <summary>
        /// 获取实体碰撞箱位置
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Vector3 getEntityBoxColliderPos(this Entity entity)
        {
            if (entity == null) return Vector3.zero;
            Vector2 colliderPos2 = entity.boxCollider.bounds.center;
            Vector3 currentPos = new Vector3(colliderPos2.x, colliderPos2.y, entity.transform.position.z);
            return currentPos;
        }
        /// <summary>
        /// 检查端口是否可用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool IsPortAvailable(int port)
        {
            try
            {
                // 创建一个TcpListener来绑定到指定端口
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                listener.Stop();
                return true; // 端口可用
            }
            catch (SocketException)
            {
                return false; // 端口被占用
            }
        }
        /// <summary>
        /// 通过ID寻找实体
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Entity findEntityByID(int ID)
        {
            foreach(Entity entity in findAllEntities())
            {
                if (entity.GetInstanceID() == ID) return entity;
            }
            return null;
        }
        /// <summary>
        /// 检测实体是否越界,现在只写了X坐标
        /// </summary>
        /// <param name="entity"></param>
        public static void checkAndFixEntityPos(Entity entity)
        {
            Vector3 pos = entity.transform.position;
            if(pos.x < -8) pos.x = 8;
            if(pos.x > 8) pos.x = 8;
            entity.transform.position = pos;
        }
        /// <summary>
        /// 通过ID寻找格子
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Cell getCellById(int id)
        {
            foreach (var cell in GameObject.FindObjectsOfType<Cell>())
            {
                if (cell.ID == id)
                {
                    return cell;
                }
            }
            return null;
        }
        /// <summary>
        /// 计算相对格子ID
        /// </summary>
        /// <param name="cellId"></param>
        /// <returns></returns>
        public static int getRelativeCellId(int cellId)
        {
            return 59 - cellId;
        }
        /// <summary>
        /// 给实体赋予ID 多人游戏
        /// </summary>
        /// <returns></returns>
        public static int createEntityId()
        {
            int ID;
            HashSet<int> existingIds = new HashSet<int>();
            foreach (Entity entity in findAllEntities())
            {
                if (entity != null)
                {
                    existingIds.Add(entity.entityID);
                }
            }
            do
            {
                ID = UnityEngine.Random.Range(0, 9999999);
            } while (existingIds.Contains(ID));

            return ID;
        }
        /// <summary>
        /// 通过ID寻找实体 包括Tower的植物和Home 多人游戏
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Entity findEntityByIDMultiGame(int ID)
        {
            //Entities
            foreach (Entity entity in findAllEntities())
            {
                if (entity.entityID == ID) return entity;
            }
            //Towers And Homes
            foreach (GameObject obj in findAllTowerAndHome())
            {
                Entity entity = null;
                if (obj.tag == "Home")
                {
                    entity = obj.GetComponent<Entity>();
                }
                else if(obj.tag == "Tower")
                {
                    entity = obj.transform.GetChild(2).GetComponent<Entity>();
                }
                if (entity == null) continue;
                if (entity.entityID == ID) return entity;
            }
            return null;
        }
        /// <summary>
        /// 让放置卡牌的实体默认为动画第一帧 beta
        /// </summary>
        public static void fixAnimator(this Entity entity)
        {
            entity.anim.enabled = true;
            DOVirtual.DelayedCall(0.2f, () =>
            {
                entity.anim.enabled = false;
            });
        }
        /// <summary>
        /// 显示自定义对话框
        /// </summary>
        /// <param name="showText">显示的文字</param>
        /// <param name="showTextSize">显示文字大小</param>
        /// <param name="showTextColor">显示文字颜色</param>
        /// <param name="showTextButton">按钮文字</param>
        /// <param name="showTextSizeButton">按钮文字大小</param>
        /// <param name="showTextColorButton">按钮文字颜色</param>
        public static void showDialog(string showText,float showTextSize,Color32 showTextColor, string showTextButton, float showTextSizeButton, Color32 showTextColorButton)
        {
            var d = DialogWarning.Instance;
            if (d == null)
            {
                Debug.Log("null dialog class");
                return;
            }
            d.showText = showText;
            d.fontSize = showTextSize;
            d.textColor = showTextColor;

            d.textColorButton = showTextColorButton;
            d.fontSizeButton = showTextSizeButton;
            d.showTextButton = showTextButton;
            d.openDialog();
        }
        /// <summary>
        /// 在列表里面寻找卡牌掉落物
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static NewCard findGiftCard(this Item[] cards, EntityType entityType)
        {
            if (cards == null) return null;
            foreach (Item item in cards)
            {
                if (item.GetComponent<NewCard>() == null) continue;

                var card = item.GetComponent<NewCard>();

                if (card == null) continue;

                if (card.addCardPrefeb.entityType == entityType) return card;
            }
            return null;
        }
        /// <summary>
        /// 根据Type在总列表里面找特效
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static AreaEffect findEffectByType(AreaEffectType type)
        {
            foreach (var eff in BridgeManager.Instance.areaEffectPrefabs)
            {
                if (eff == null) continue;
                if(eff.type == type) return eff;
            }
            return null;
        }
        /// <summary>
        /// 该法术生成器生成的法术是否对自己有害
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isHarmfulAreaEffectSpawner(EntityType type)
        {
            return type == EntityType.FireballSpawner
                || type == EntityType.SnowBottleSpawner
                || type == EntityType.PoisonBottleSpawner;
        }
        /// <summary>
        /// 法术是否对自己有害
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool isHarmfulAreaEffect(AreaEffectType type)
        {
            return type == AreaEffectType.Fireball
                || type == AreaEffectType.LeavingSnow
                || type == AreaEffectType.PoisonLiquid;
        }
        /// <summary>
        /// 是否在晚上(地图)
        /// </summary>
        /// <returns></returns>
        public static bool inNight()
        {
            return GameManager.Instance.BG.backgroundType == BackgroundType.Night;
        }
        /// <summary>
        /// 是否属于益智时刻的游戏?
        /// </summary>
        /// <param name="gameMode"></param>
        /// <returns></returns>
        public static bool isMiniGame(this GameMode gameMode)
        {
            return gameMode == GameMode.MiniGame_TETF || gameMode == GameMode.MiniGame_WGWE || gameMode == GameMode.MiniGame_BS;
        }
        /// <summary>
        /// 生成一个随机卡组
        /// </summary>
        /// <returns></returns>
        public static List<Card> getRandomCardList()
        {
            var bm = BridgeManager.Instance;
            var cards = new List<Card>();
            for (var i = 0; i < 8; i++)
            {
                var card = bm.allCards[UnityEngine.Random.Range(0, bm.allCards.Count)];
                if (cards.Contains(card) || card.cantBeUsedInRandomCardList) i--;
                else cards.Add(card);
            }
            return cards;
        }
    }
}
