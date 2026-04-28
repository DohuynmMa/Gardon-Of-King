using System;

public enum Musics
{
    主菜单BGM = 0,
    等待音乐 = 1,
    失败音乐 = 2,
    胜利音乐 = 3,
    第一章bgm = 4,
    花园bgm = 5,
    戴夫BOSS一阶段 = 6,
    戴夫BOSS二阶段 = 7,
    UltimateBattle = 8,
    恭喜发财 = 9,
    第二章bgm = 10,
    Loonboon = 11,
}

public enum Voices
{
    戴夫短1 = 0,
    戴夫短2 = 1,
    戴夫短3 = 2,
    戴夫长1 = 3,
    戴夫长2 = 4,
    戴夫长3 = 5,
    戴夫长4 = 6,
    戴夫长5 = 7,
    戴夫长6 = 8,
    戴夫叫1 = 9,
    戴夫叫2 = 10,
    戴夫发神经 = 11,
    国王笑1 = 12,
    国王笑2 = 13,
    国王笑3 = 14,
    国王笑4 = 15,
    国王笑5 = 16,
    国王笑6 = 17
}

public enum Sounds
{
    rollin = 0,
    paper = 1,
    tap = 2,
    准备好开始 = 3,
    coin_click = 4,
    prize_click = 5,
    使用铲子 = 6,
    僵尸丢手 = 7,
    僵尸吃 = 8,
    ZombiesAreComing = 9,
    pause = 10,
    种植物 = 11,
    SeedLift = 12,
    哥布林戳人音效 = 13,
    砸瘪 = 14,
    丢小鬼音效 = 15,
    巨人僵尸趋势 = 16,
    点击阳光 = 17,
    子弹打中普通 = 18,
    子弹打中塑料1 = 19,
    子弹打中塑料2 = 20,
    子弹打铁1 = 21,
    子弹打铁2 = 22,
    防御塔爆炸 = 23,
    僵尸叫1 = 24,
    僵尸叫2 = 25,
    僵尸叫3 = 26,
    僵尸叫4 = 27,
    僵尸叫5 = 28,
    僵尸叫6 = 29,
    gulp = 30,
    bleep = 31,
    buzzer = 32,
    ButtonClick = 33,
    GraaveButton = 34,
    僵尸倒地 = 35,
    砸碎玻璃 = 36,
    狙击枪发射 = 37,
    狙击枪上膛 = 38,
    explosion = 39,
    zamboni = 40,
    挖土 = 41,
    出土 = 42,
    植物shoot = 43,
    无音效 = 44,
    樱桃炸弹爆炸 = 45,
    戴夫狙击技能的滴滴声 = 46,
    狂暴药水声 = 47,
    手雷拉保险 = 48,
    寒冰豌豆发射 = 49,
    冰冻音效 = 50,
    激光蓄力发射 = 51,
    玉米 = 52,
    黄油 = 53,
    西瓜破烂 = 54,
    报纸掉落 = 55,
    二爷发火1 = 56,
    二爷发火2 = 57,
    化妆僵尸发火 = 58,
    水晶破碎 = 59,
    水晶破碎2 = 60,
    铲子挖土 = 61,
    watering = 62,
    bitClick = 63,
    放置皮卡 = 64,
    皮卡攻击 = 65,
    皮卡击中 = 66,
    皮卡掉手 = 67,
    割伤 = 68,
    皮卡走路 = 69,
    放置骷髅 = 70,
    骷髅死亡 = 71,
    骷髅攻击 = 72,
    火球释放 = 73,
    火球爆炸 = 74,
    持续冰冻 = 75,
    玻璃瓶损坏 = 76,
    治疗药水 = 77,
    毒药法术 = 78,
    皮卡巨人放置 = 79,
    皮卡巨人走路 = 80,
    皮卡巨人切换形态 = 81,
    皮卡巨人攻击 = 82,
    挥刀 = 83,
    女巫生成 = 84,
    女巫攻击 = 85,
    女巫召唤 = 86,
    弹 = 87,
    吃 = 88,
    打嗝 = 89
}

public static class AudioHelper
{
    public static void normalZombieShout()
    {
        new[]
        {
            Sounds.僵尸叫1, Sounds.僵尸叫2, Sounds.僵尸叫3,
            Sounds.僵尸叫4, Sounds.僵尸叫5, Sounds.僵尸叫6,
        }
        .random().playWithPitch(UnityEngine.Random.Range(0.9f, 1.1f));
    }
    public static void paperZombieAngry()
    {
        new[]
        {
            Sounds.二爷发火1, Sounds.二爷发火2,
        }
        .random().playWithPitch(UnityEngine.Random.Range(0.9f, 1.1f));
    }

    public static T random<T>() where T : Enum
    {
        var values = typeof(T).GetEnumValues();
        return (T) values.random();
    }

    public static object random(this Array array)
    {
        var index = UnityEngine.Random.Range(0, array.Length);
        return array.GetValue(index);
    }

    public static T random<T>(this T[] array)
    {
        var index = UnityEngine.Random.Range(0, array.Length);
        return array[index];
    }
    public static void play(this Musics sound, bool loop)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playMusic(id, loop);
    }
    public static void play(this Voices sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playVoice(id);
    }
    public static void play(this Sounds sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id);
    }
    public static void play(this Sounds sound,float time)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id,time);
    }
    public static void play(this Sounds sound, float time,float speed)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSounds(id, time,speed);
    }
    public static void playWithPitch(this Sounds sound, float pitch,float volume)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id, pitch,volume);
    }
    public static void playWithPitch(this Sounds sound, float pitch)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id, pitch);
    }
    public static void playWithPitch(this Sounds sound)
    {
        var id = Convert.ToInt32(sound);
        SoundsManager.playSoundsWithPitch(id);
    }
}
