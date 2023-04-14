﻿namespace GameGate
{
    public class TableDef
    {
        // 1~12
        // 13~24
        // 25~36
        // 37~48
        // 49~60
        // 61~72
        // 73~84
        // 85~96
        // 97~99
        // 100..111
        // 112~119
        // 120~128
        public static readonly bool[] MaigicDelayArray = { true, true, true, true, true, true, true, false, true, true, true, true, false, true, true, true, true, true, true, true, true, true, true, true, true, false, false, false, true, true, true, true, true, true, true, false, true, true, true, true, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false };
        // 小火球
        // 治愈术
        // 基本剑术
        // 精神力战法
        // 大火球
        // 施毒术
        // 地狱火焰
        // 疾电雷光
        // 雷电术
        // 12
        // 灵魂道府
        // 火墙
        // 爆裂火焰
        // 地狱雷光
        // 圣言术
        // 冰咆哮
        // 火焰冰
        // 狮子吼 - 群体雷电术
        // 群体施毒术
        // 彻地钉
        // 40
        // 狮子吼
        // 寒冰掌
        // 灭天火
        // 火龙烈焰
        // 37~48
        // 净化术
        // 无极真气
        // 51
        // 52
        // 53
        // 54
        // 55
        // 逐日剑法
        // 噬血术
        // 流星火雨
        // 59
        // 60
        // 61
        // 62
        // 63
        // 64
        // 65
        // 66
        // 67
        // 71擒龙手
        // 84~95
        // 96..99
        // 100~111
        // 112
        public static readonly bool[] MaigicAttackArray = { false, true, false, false, false, true, true, false, false, true, true, true, false, true, false, false, false, false, false, false, false, false, true, true, true, false, false, false, false, false, false, false, true, true, false, false, true, true, true, true, false, true, false, false, true, true, false, true, false, false, false, true, true, true, true, false, false, true, true, false, false, false, false, false, false, false, false, false, false, false, false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true };
        // 120~128
        // 魔法的延迟表
        public static int[] MAIGIC_DELAY_TIME_LIST_DEF;
        // 01 小火球
        // 02 治疗术
        // 03 初级剑法
        // 04 精神战法
        // 05 大火球
        // 06 施毒术
        // 07 攻杀剑法
        // 08 抗拒火环
        // 09 地狱火
        // 10 疾光电影
        // 11 雷电术
        // 12 刺杀剑术
        // 13 灵魂火符
        // 14 幽灵盾
        // 15 神圣战甲术
        // 16 困魔咒
        // 17 召唤骷髅
        // 18 隐身术
        // 19 集体隐身术
        // 20 诱惑之光
        // 21 瞬息移动
        // 22 火墙
        // 23 爆裂火焰
        // 24 地狱雷光
        // 25 半月弯刀
        // 26 烈火剑法
        // 27 野蛮冲撞
        // 28 心灵启示
        // 29 群体治愈术
        // 30 召唤神兽
        // 31 魔法盾
        // 32 圣言术
        // 33 冰咆哮
        // 34 解毒术
        // 35 老狮子吼
        // 36 火焰冰
        // 37 群体雷电术
        // 38 群体施毒术
        // 39 彻地钉
        // 40 双龙斩
        // 41 狮子吼
        // 42 龙影剑法
        // 43 雷霆剑法
        // 44 寒冰掌
        // 45 灭天火
        // 46 召唤英雄
        // 47 火龙烈焰
        // 48 气功波
        // 49 净化术
        // 50 无极真气
        // 51 飓风破
        // 52 诅咒术
        // 53 血咒
        // 54 骷髅咒
        // 55
        // 56 逐日剑法
        // 57 噬血术
        // 58 流星火雨
        // 59
        // 60 破魂斩
        // 61 劈星斩
        // 62 雷霆一击
        // 63 噬魂沼泽
        // 64 末日审判
        // 65 火龙气焰
        // 66 英雄开天斩
        // 67
        // 68
        // 69
        // 70 心灵召唤
        // 71 英雄擒龙手
        // 72
        // 73
        // 74 英雄分身术
        // 75 英雄护体神盾
        /// <summary>
        /// 魔法的延迟表
        /// </summary>
        public static readonly int[] MaigicDelayTimeList = { 60000, 1110 + 60, 1110 + 40, 1110, 1110, 1110 + 60, 1110 + 40, 1110, 1110 + 30, 1110 + 60, 1110 + 100, 1110 + 100, 1110, 1110 + 60, 1110 + 40, 1110 + 40, 1110 + 50, 1110 + 50, 1110 + 50, 1110 + 50, 1110 + 60, 1110 + 50, 1110 + 120, 1110 + 60, 1110 + 60, 1110, 1110, 1110, 1110 + 40, 1110 + 40, 1110 + 120, 1110, 1320 - 90, 1260 - 90, 1240 - 90, 1260 - 90, 1260 - 90, 1320 - 90, 1320 - 90, 1320 - 90, 1110, 1230 - 90, 1110, 1110, 1260 - 90, 1260 - 90, 1260 - 90, 1260 - 90, 1230 - 90, 1240 - 90, 1230 - 90, 1240 - 90, 1240 - 90, 1240 - 90, 1260 - 90, 1260 - 90, 1110, 1260 - 90, 1320 - 90, 1300, 200, 200, 200, 200, 200, 200, 1500 - 90, 1800 - 90, 1110, 1110, 1700 - 90, 1800 - 90, 1110, 1110, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 350, 1300, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
        public static readonly string[] MaigicNameList = { "", "火球术", "治愈术", "基本剑术", "精神力战法", "大火球", "施毒术", "攻杀剑术", "抗拒火环", "地狱火", "疾光电影", "雷电术", "刺杀剑术", "灵魂火符", "幽灵盾", "神圣战甲术", "困魔咒", "召唤骷髅", "隐身术", "集体隐身术", "诱惑之光", "瞬息移动", "火墙", "爆裂火焰", "地狱雷光", "半月弯刀", "烈火剑法", "野蛮冲撞", "心灵启示", "群体治疗术", "召唤神兽", "魔法盾", "圣言术", "冰咆哮", "解毒术", "老狮子吼", "火焰冰", "群体雷电术", "群体施毒术", "彻地钉", "双龙斩", "狮子吼", "龙影剑法", "雷霆剑法", "寒冰掌", "灭天火", "召唤英雄", "火龙烈焰", "气功波", "净化术", "无极真气", "飓风破", "诅咒术", "血咒", "骷髅咒", "", "逐日剑法", "噬血术", "流星火雨", "", "破魂斩", "劈星斩", "雷霆一击", "噬魂沼泽", "末日审判", "火龙气焰", "开天斩", "神秘解读", "唯我独尊", "", "英雄出击", "擒龙手", "", "", "", "护体神盾", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "追心刺", "三绝杀", "断岳斩", "横扫千军", "凤舞祭", "惊雷爆", "冰天雪地", "双龙破", "虎啸诀", "八卦掌", "三焰咒", "万箭归宗", "旋转风火轮", "断空斩", "倚天辟地", "血魂一击(战)", "血魂一击(法)", "血魂一击(道)", "", "", "", "", "", "", "", "", "", "" };

        public void Initialization()
        {
            MAIGIC_DELAY_TIME_LIST_DEF = MaigicDelayTimeList;
        }
    }
}