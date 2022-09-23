﻿namespace GameSvr.Command
{
    public static class GameCommandConst
    {
        public const string GameLogMsg1 = "{0}\09{1}\09{2}\09{3}\09{4}\09{5}\09{6}\09{7}\09{8}";
        public const string HumanDieEvent = "人物死亡事件";
        public const string HitOverSpeed = "[攻击超速] {0} 间隔:{1} 数量:{2}";
        public const string RunOverSpeed = "[跑步超速] {0} 间隔:{1} 数量:{2}";
        public const string WalkOverSpeed = "[行走超速] {0} 间隔:{1} 数量:{2}";
        public const string SpellOverSpeed = "[魔法超速] {0} 间隔:{1} 数量:{2}";
        public const string BunOverSpeed = "[游戏超速] {0} 间隔:{1} 数量:{2}";
        public const string GameCommandPermissionTooLow = "权限不够!!!";
        public const string GameCommandParamUnKnow = "命令格式: @{0} {1}";
        public const string GameCommandMoveHelpMsg = "地图号";
        public const string GameCommandPositionMoveHelpMsg = "地图号 座标X 座标Y";
        public const string GameCommandPositionMoveCanotMoveToMap = "无法移动到地图: {0} X:{1} Y:{2}";
        public const string GameCommandInfoHelpMsg = "人物名称";
        public const string NowNotOnLineOrOnOtherServer = "{0} 现在不在线，或在其它服务器上!!!";
        public const string GameCommandMobCountHelpMsg = "地图号";
        public const string GameCommandMobCountMapNotFound = "指定的地图不存在!!!";
        public const string GameCommandMobCountMonsterCount = "怪物数量：{0}";
        public const string GameCommandHumanCountHelpMsg = "地图号";
        public const string GameCommandKickHumanHelpMsg = "人物名称";
        public const string GameCommandTingHelpMsg = "人物名称";
        public const string GameCommandSuperTingHelpMsg = "人物名称 范围(0-10)";
        public const string GameCommandMapMoveHelpMsg = "源地图  目标地图";
        public const string GameCommandMapMoveMapNotFound = "地图{0}不存在!!!";
        public const string GameCommandShutupHelpMsg = "人物名称  时间长度(分钟)";
        public const string GameCommandShutupHumanMsg = "{0} 已被禁言{1}分钟";
        public const string GameCommandGamePointHelpMsg = "人物名称 控制符(+,-,=) 游戏点数(1-100000000)";
        public const string GameCommandGamePointHumanMsg = "你的游戏点已增加{0}点，当前总点数为{1}点。";
        public const string GameCommandGamePointGMMsg = "{0}的游戏点已增加{1}点，当前总点数为{2}点。";
        public const string GameCommandCreditPointHelpMsg = "人物名称 控制符(+,-,=) 声望点数(0-255)";
        public const string GameCommandCreditPointHumanMsg = "你的声望点已增加{0}点，当前总声望点数为{1}点。";
        public const string GameCommandCreditPointGMMsg = "{0}的声望点已增加{1}点，当前总声望点数为{2}点。";
        public const string GameCommandGameGoldHelpMsg = " 人物名称 控制符(+,-,=) 游戏币(1-200000000)";
        public const string GameCommandGameGoldHumanMsg = "你的{0}已增加{1}，当前拥有{2}{3}。";
        public const string GameCommandGameGoldGMMsg = "{0}的{1}已增加{2}，当前拥有{3}{4}。";
        public const string GameCommandMapInfoMsg = "地图名称: {0}({1})";
        public const string GameCommandMapInfoSizeMsg = "地图大小: X({0}) Y({1})";
        public const string GameCommandShutupReleaseHelpMsg = "人物名称";
        public const string GameCommandShutupReleaseCanSendMsg = "你已经恢复聊天功能!!!";
        public const string GameCommandShutupReleaseHumanCanSendMsg = "{0} 已经恢复聊天。";
        public const string GameCommandShutupListIsNullMsg = "禁言列表为空!!!";
        public const string GameCommandLevelConsoleMsg = "[等级调整] {0} ({1} -> {2})";
        public const string GameCommandSbkGoldHelpMsg = "城堡名称 控制符(=、-、+) 金币数(1-100000000)";
        public const string GameCommandSbkGoldCastleNotFoundMsg = "城堡{0}未找到!!!";
        public const string GameCommandSbkGoldShowMsg = "{0}的金币数为: {1} 今天收入: {2}";
        public const string GameCommandRecallHelpMsg = "人物名称";
        public const string GameCommandReGotoHelpMsg = "人物名称";
        public const string GameCommandShowHumanFlagHelpMsg = "人物名称 标识号";
        public const string GameCommandShowHumanFlagONMsg = "{0}: [{1}] = ON";
        public const string GameCommandShowHumanFlagOFFMsg = "{0}: [{1}] = OFF";
        public const string GameCommandShowHumanUnitHelpMsg = "人物名称 单元号";
        public const string GameCommandShowHumanUnitONMsg = "{0}: [{1}] = ON";
        public const string GameCommandShowHumanUnitOFFMsg = "{0}: [{1}] = OFF";
        public const string GameCommandMobHelpMsg = "怪物名称 数量 等级";
        public const string GameCommandMobMsg = "怪物名称不正确或其它未问题!!!";
        public const string GameCommandMobNpcHelpMsg = "NPC名称 脚本文件名 外形(数字) 属沙城(0,1)";
        public const string GameCommandNpcScriptHelpMsg = "？？？？";
        public const string GameCommandDelNpcMsg = "命令使用方法不正确，必须与NPC面对面，才能使用此命令!!!";
        public const string GameCommandRecallMobHelpMsg = "怪物名称 数量 等级";
        public const string GameCommandLuckPointHelpMsg = "人物名称 控制符 幸运点数";
        public const string GameCommandLuckPointMsg = "{0} 的幸运点数为:{1}/{2} 幸运值为:{3}";
        public const string GameCommandLotteryTicketMsg = "已中彩票数:{0} 未中彩票数:{1} 一等奖:{2} 二等奖:{3} 三等奖:{4} 四等奖:{5} 五等奖:{6} 六等奖:{7} ";
        public const string GameCommandReloadGuildHelpMsg = "行会名称";
        public const string GameCommandReloadGuildOnMasterserver = "此命令只能在主游戏服务器上执行!!!";
        public const string GameCommandReloadGuildNotFoundGuildMsg = "未找到行会{0}!!!";
        public const string GameCommandReloadGuildSuccessMsg = "行会{0}重加载成功...";
        public const string GameCommandReloadLineNoticeSuccessMsg = "重新加载公告设置信息完成。";
        public const string GameCommandReloadLineNoticeFailMsg = "重新加载公告设置信息失败!!!";
        public const string GameCommandFreePKHelpMsg = "人物名称";
        public const string GameCommandFreePKHumanMsg = "你的PK值已经被清除...";
        public const string GameCommandFreePKMsg = "{0}的PK值已经被清除...";
        public const string GameCommandPKPointHelpMsg = "人物名称";
        public const string GameCommandPKPointMsg = "{0}的PK点数为:{1}";
        public const string GameCommandIncPkPointHelpMsg = "人物名称 PK点数";
        public const string GameCommandIncPkPointAddPointMsg = "{0}的PK值已增加%d点...";
        public const string GameCommandIncPkPointDecPointMsg = "{0}的PK值已减少%d点...";
        public const string GameCommandHumanLocalHelpMsg = "人物名称";
        public const string GameCommandHumanLocalMsg = "{0}来自:{1}";
        public const string GameCommandPrvMsgHelpMsg = "人物名称";
        public const string GameCommandPrvMsgUnLimitMsg = "{0} 已从禁止私聊列表中删除...";
        public const string GameCommandPrvMsgLimitMsg = "{0} 已被加入禁止私聊列表...";
        public const string GamecommandMakeHelpMsg = " 物品名称  数量";
        public const string GamecommandMakeItemNameOrPerMissionNot = "输入的物品名称不正确，或权限不够!!!";
        public const string GamecommandMakeInCastleWarRange = "攻城区域，禁止使用此功能!!!";
        public const string GamecommandMakeInSafeZoneRange = "非安全区，禁止使用此功能!!!";
        public const string GamecommandMakeItemNameNotFound = "{0} 物品名称不正确!!!";
        public const string GamecommandSuperMakeHelpMsg = "身上没指定物品!!!";
        public const string GameCommandViewWhisperHelpMsg = " 人物名称";
        public const string GameCommandViewWhisperMsg1 = "已停止侦听{0}的私聊信息...";
        public const string GameCommandViewWhisperMsg2 = "正在侦听{0}的私聊信息...";
        public const string GameCommandReAliveHelpMsg = " 人物名称";
        public const string GameCommandReAliveMsg = "{0} 已获重生.";
        public const string GameCommandChangeJobHelpMsg = " 人物名称 职业类型(Warr Wizard Taos)";
        public const string GameCommandChangeJobMsg = "{0} 的职业更改成功。";
        public const string GameCommandChangeJobHumanMsg = "职业更改成功。";
        public const string GameCommandTestGetBagItemsHelpMsg = "(用于测试升级武器方面参数)";
        public const string GameCommandShowUseItemInfoHelpMsg = "人物名称";
        public const string GameCommandBindUseItemHelpMsg = "人物名称 物品类型 绑定方法";
        public const string GameCommandBindUseItemNoItemMsg = "{0}的{1}没有戴物品!!!";
        public const string GameCommandBindUseItemAlreadBindMsg = "{0}的{1}上的物品早已绑定过了!!!";
        public const string GameCommandMobFireBurnHelpMsg = "命令格式: {0} {1} {2} {3} {4} {5} {6}";
        public const string GameCommandMobFireBurnMapNotFountMsg = "地图{0} 不存在";
        public static string GetSellOffGlod = "{0} {1}增加";
        public static string EnableDearRecall = "允许夫妻传送!!!";
        public static string DisableDearRecall = "禁止夫妻传送!!!";
        public static string EnableMasterRecall = "允许师徒传送!!!";
        public static string DisableMasterRecall = "禁止师徒传送!!!";
        public static string NowCurrDateTime = "当前日期时间: ";
        public static string EnableHearWhisper = "[允许私聊]";
        public static string DisableHearWhisper = "[禁止私聊]";
        public static string EnableShoutMsg = "[允许群聊]";
        public static string DisableShoutMsg = "[禁止群聊]";
        public static string EnableDealMsg = "[允许交易]";
        public static string DisableDealMsg = "[禁止交易]";
        public static string EnableGuildChat = "[允许行会聊天]";
        public static string DisableGuildChat = "[禁止行会聊天]";
        public static string EnableJoinGuild = "[允许加入行会]";
        public static string DisableJoinGuild = "[禁止加入行会]";
        public static string EnableAuthAllyGuild = "[允许行会联盟]";
        public static string DisableAuthAllyGuild = "[禁止行会联盟]";
        public static string EnableGroupRecall = "[允许天地合一]";
        public static string DisableGroupRecall = "[禁止天地合一]";
        public static string EnableGuildRecall = "[允许行会合一]";
        public static string DisableGuildRecall = "[禁止行会合一]";
        public static string PleaseInputPassword = "请输入密码:";
        public static string TheMapDisableMove = "地图{0}({1})不允许传送!!!";
        public static string TheMapNotFound = "{0} 此地图号不存在!!!";

    }
}
