using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static readonly Encoding encoding = new UTF8Encoding(false);
    private static string dataPath = Application.isMobilePlatform ? Application.persistentDataPath + "data" : (Application.isEditor ?
    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".cpzEditor" : Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".cpz") + Path.DirectorySeparatorChar + "data.json";
    public static DataManager Instance;
    public MyData data;
    public MyData newPlayerData;//初始化的数据
    public MyData testData;//测试数据
    private string savedNewPlayerData;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        if (TestManager.Instance.不读取编辑器专用存档)
        {
            dataPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + Path.DirectorySeparatorChar + ".cpz" + Path.DirectorySeparatorChar + "data.json";
        }
        newPlayerData.beforeSave();
        savedNewPlayerData = JsonUtility.ToJson(newPlayerData);
        newPlayerData.afterLoad();
        loadPlayerData();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete) && TestManager.Instance.Delete键重置存档) // 按下 Delete 删除数据文件 (测试)
        {
            resetPlayerData();
        }
    }
    public void addCard(Card card)
    {
        data.cardBag.Add(card);
        savePlayerData();
    }
    /// <summary>
    /// 存档
    /// </summary>
    public void savePlayerData()
    {
        data.beforeSave();
        string json = JsonUtility.ToJson(data);
        var file = new FileInfo(dataPath);
        if (!file.Directory.Exists) file.Directory.Create();
        File.WriteAllTextAsync(dataPath, json, encoding);
    }
    /// <summary>
    /// 加载存档
    /// </summary>
    public void loadPlayerData()
    {
        string json = File.Exists(dataPath) ? File.ReadAllText(dataPath, encoding) : null;
        data = json == null ? null : JsonUtility.FromJson<MyData>(json);
        if (data == null)
        {
            print("未找到存档，正在新建...");
            resetPlayerData();
        }
        else
        {
            if (!data.havePlayedSCRS)
            {
                data.sunPointSRCS = 0;
                data.allEntitiesSRCS.Clear();
                data.allTowersSRCS.Clear();
                savePlayerData();
            }
        }
        data.afterLoad();
        autoFixData();
    }
    /// <summary>
    /// 重置玩家存档
    /// </summary>
    public void resetPlayerData()
    {
        data = JsonUtility.FromJson<MyData>(savedNewPlayerData);
        savePlayerData();
    }
    /// <summary>
    /// 自动修复异常数据
    /// </summary>
    private void autoFixData()
    {
        var type = typeof(MyData);
        var saveFlag = false;
        foreach (var field in type.GetFields())
        {
            void setField(object v)
            {
                field.SetValue(data, v);
                saveFlag = true;
            }
            var value = field.GetValue(data);
            var intRange = field.GetCustomAttribute<IntRangeAttribute>();
            if (intRange != null && value is int)
            {
                if ((int)value < intRange.min) setField(intRange.min);
                if ((int)value > intRange.max) setField(intRange.max);
            }
            var floatRange = field.GetCustomAttribute<FloatRangeAttribute>();
            if (floatRange != null && value is float)
            {
                if ((float)value < floatRange.min) setField(floatRange.min);
                if ((float)value > floatRange.max) setField(floatRange.max);
            }
            var doubleRange = field.GetCustomAttribute<DoubleRangeAttribute>();
            if (doubleRange != null && value is double)
            {
                if ((double)value < doubleRange.min) setField(doubleRange.min);
                if ((double)value > doubleRange.max) setField(doubleRange.max);
            }
        }
        if (saveFlag) savePlayerData();
    }

}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class IntRangeAttribute : Attribute
{
    public int min;
    public int max;
    public IntRangeAttribute(int min = int.MinValue, int max = int.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class FloatRangeAttribute : Attribute
{
    public float min;
    public float max;
    public FloatRangeAttribute(float min = float.MinValue, float max = float.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class DoubleRangeAttribute : Attribute
{
    public double min;
    public double max;
    public DoubleRangeAttribute(double min = double.MinValue, double max = double.MaxValue)
    {
        this.min = min;
        this.max = max;
    }
}
