using GameSvr.Actor;
using GameSvr.Monster;
using GameSvr.Npc;
using GameSvr.Script;
using System.Collections;
using SystemModule.Common;
using SystemModule.Data;

namespace GameSvr.DataSource
{
    public class TDefineInfo
    {
        public string sName;
        public string sText;
    }

    public class TQDDinfo
    {
        public int n00;
        public string s04;
        public ArrayList sList;
    }

    public class LocalDB
    {
        private readonly char[] TextSpitConst = { ' ', '\t' };
        private readonly char[] MonsterSpitConst = { ' ', '/', '\t' };

        public static bool LoadAdminList()
        {
            var sIPaddr = string.Empty;
            var sChrName = string.Empty;
            var sData = string.Empty;
            var sfilename = M2Share.GetEnvirFilePath("AdminList.txt");
            if (!File.Exists(sfilename))
            {
                return false;
            }
            M2Share.WorldEngine.AdminList.Clear();
            using var LoadList = new StringList();
            LoadList.LoadFromFile(sfilename);
            for (var i = 0; i < LoadList.Count; i++)
            {
                var sLineText = LoadList[i];
                var nLv = -1;
                if (sLineText != "" && sLineText[0] != ';')
                {
                    if (sLineText[0] == '*')
                    {
                        nLv = 10;
                    }
                    else if (sLineText[0] == '1')
                    {
                        nLv = 9;
                    }
                    else if (sLineText[0] == '2')
                    {
                        nLv = 8;
                    }
                    else if (sLineText[0] == '3')
                    {
                        nLv = 7;
                    }
                    else if (sLineText[0] == '4')
                    {
                        nLv = 6;
                    }
                    else if (sLineText[0] == '5')
                    {
                        nLv = 5;
                    }
                    else if (sLineText[0] == '6')
                    {
                        nLv = 4;
                    }
                    else if (sLineText[0] == '7')
                    {
                        nLv = 3;
                    }
                    else if (sLineText[0] == '8')
                    {
                        nLv = 2;
                    }
                    else if (sLineText[0] == '9')
                    {
                        nLv = 1;
                    }
                    if (nLv > 0)
                    {
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sData, new[] { "/", "\\", " ", "\t" });
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sChrName, new[] { "/", "\\", " ", "\t" });
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sIPaddr, new[] { "/", "\\", " ", "\t" });
                        if (string.IsNullOrEmpty(sChrName) || sIPaddr == "")
                        {
                            continue;
                        }
                        var AdminInfo = new AdminInfo
                        {
                            nLv = nLv,
                            sChrName = sChrName,
                            sIPaddr = sIPaddr
                        };
                        M2Share.WorldEngine.AdminList.Add(AdminInfo);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 读取守卫配置
        /// </summary>
        public static void LoadGuardList()
        {
            try
            {
                var monName = string.Empty;
                var mapName = string.Empty;
                var cX = string.Empty;
                var cY = string.Empty;
                var direction = string.Empty;
                var sFileName = M2Share.GetEnvirFilePath("GuardList.txt");
                if (File.Exists(sFileName))
                {
                    var guardList = new StringList();
                    guardList.LoadFromFile(sFileName);
                    for (var i = 0; i < guardList.Count; i++)
                    {
                        var sLine = guardList[i];
                        if (!string.IsNullOrEmpty(sLine) && sLine[0] != ';')
                        {
                            sLine = HUtil32.GetValidStrCap(sLine, ref monName, ' ');
                            if (!string.IsNullOrEmpty(monName) && monName[0] == '\"')
                            {
                                HUtil32.ArrestStringEx(monName, "\"", "\"", ref monName);
                            }
                            sLine = HUtil32.GetValidStr3(sLine, ref mapName, ' ');
                            sLine = HUtil32.GetValidStr3(sLine, ref cX, new[] { ' ', ',' });
                            sLine = HUtil32.GetValidStr3(sLine, ref cY, new[] { ' ', ',', ':' });
                            sLine = HUtil32.GetValidStr3(sLine, ref direction, new[] { ' ', ':' });
                            if (!string.IsNullOrEmpty(monName) && !string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(direction))
                            {
                                var guard = M2Share.WorldEngine.RegenMonsterByName(mapName, HUtil32.StrToInt16(cX, 0), HUtil32.StrToInt16(cY, 0), monName);
                                if (guard != null)
                                {
                                    guard.Direction = (byte)HUtil32.StrToInt(direction, 0);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                M2Share.Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// 读取物品合成配置
        /// </summary>
        public void LoadMakeItem()
        {
            var sSubName = string.Empty;
            var sItemName = string.Empty;
            IList<MakeItem> List28 = null;
            var sFileName = M2Share.GetEnvirFilePath("MakeItem.txt");
            if (File.Exists(sFileName))
            {
                using var LoadList = new StringList();
                LoadList.LoadFromFile(sFileName);
                for (var i = 0; i < LoadList.Count; i++)
                {
                    var sLine = LoadList[i].Trim();
                    if (string.IsNullOrEmpty(sLine) || sLine.StartsWith(";"))
                    {
                        continue;
                    }
                    if (sLine.StartsWith("["))
                    {
                        if (List28 != null)
                        {
                            M2Share.MakeItemList.Add(sItemName, List28);
                        }
                        List28 = new List<MakeItem>();
                        HUtil32.ArrestStringEx(sLine, "[", "]", ref sItemName);
                    }
                    else
                    {
                        if (List28 != null)
                        {
                            sLine = HUtil32.GetValidStr3(sLine, ref sSubName, TextSpitConst);
                            var nItemCount = HUtil32.StrToInt(sLine.Trim(), 1);
                            List28.Add(new MakeItem() { ItemName = sSubName, ItemCount = nItemCount });
                        }
                    }
                }
                if (List28 != null)
                {
                    M2Share.MakeItemList.Add(sItemName, List28);
                }
            }
        }

        private static void QFunctionNPC()
        {
            try
            {
                var sScriptFile = M2Share.GetEnvirFilePath(ScriptConst.sMarket_Def, "QFunction-0.txt");
                var sScritpDir = M2Share.GetEnvirFilePath(ScriptConst.sMarket_Def);
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    using var SaveList = new StringList();
                    SaveList.Add(";此脚为功能脚本，用于实现各种与脚本有关的功能");
                    SaveList.SaveToFile(sScriptFile);
                }
                if (File.Exists(sScriptFile))
                {
                    M2Share.FunctionNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "QFunction",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = ScriptConst.sMarket_Def,
                        ScriptName = "QFunction",
                        IsHide = true,
                        IsQuest = false
                    };
                    M2Share.WorldEngine.AddMerchant(M2Share.FunctionNPC);
                }
                else
                {
                    M2Share.FunctionNPC = null;
                }
            }
            catch
            {
                M2Share.FunctionNPC = null;
            }
        }

        private static void QMangeNPC()
        {
            try
            {
                var sScriptFile = M2Share.GetEnvirFilePath("MapQuest_def", "QManage.txt");
                var sScritpDir = M2Share.GetEnvirFilePath("MapQuest_def");
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    var sShowFile = HUtil32.ReplaceChar(sScriptFile, '\\', '/');
                    var SaveList = new StringList();
                    SaveList.Add(";此脚为登录脚本，人物每次登录时都会执行此脚本，所有人物初始设置都可以放在此脚本中。");
                    SaveList.Add(";修改脚本内容，可用@ReloadManage命令重新加载该脚本，不须重启程序。");
                    SaveList.Add("[@Login]");
                    SaveList.Add("#if");
                    SaveList.Add("#act");
                    SaveList.Add(";设置10倍杀怪经验");
                    SaveList.Add(";CANGETEXP 1 10");
                    SaveList.Add("#say");
                    SaveList.Add("游戏登录脚本运行成功，欢迎进入本游戏!!!\\ \\");
                    SaveList.Add("<关闭/@exit> \\ \\");
                    SaveList.Add("登录脚本文件位于: \\");
                    SaveList.Add(sShowFile + '\\');
                    SaveList.Add("脚本内容请自行按自己的要求修改。");
                    SaveList.SaveToFile(sScriptFile);
                    SaveList = null;
                }
                if (File.Exists(sScriptFile))
                {
                    M2Share.ManageNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "QManage",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = "MapQuest_def",
                        IsHide = true,
                        IsQuest = false
                    };
                    M2Share.WorldEngine.QuestNpcList.Add(M2Share.ManageNPC);
                }
                else
                {
                    M2Share.ManageNPC = null;
                }
            }
            catch
            {
                M2Share.ManageNPC = null;
            }
        }

        private static void RobotNPC()
        {
            try
            {
                var sScriptFile = M2Share.GetEnvirFilePath("Robot_def", "RobotManage.txt");
                var sScritpDir = M2Share.GetEnvirFilePath("Robot_def");
                if (!Directory.Exists(sScritpDir))
                {
                    Directory.CreateDirectory(sScritpDir);
                }
                if (!File.Exists(sScriptFile))
                {
                    var tSaveList = new StringList();
                    tSaveList.Add(";此脚为机器人专用脚本，用于机器人处理功能用的脚本。");
                    tSaveList.SaveToFile(sScriptFile);
                    tSaveList = null;
                }
                if (File.Exists(sScriptFile))
                {
                    M2Share.RobotNPC = new Merchant
                    {
                        MapName = "0",
                        CurrX = 0,
                        CurrY = 0,
                        ChrName = "RobotManage",
                        NpcFlag = 0,
                        Appr = 0,
                        FilePath = "Robot_def",
                        IsHide = true,
                        IsQuest = false
                    };
                    M2Share.WorldEngine.QuestNpcList.Add(M2Share.RobotNPC);
                }
                else
                {
                    M2Share.RobotNPC = null;
                }
            }
            catch
            {
                M2Share.RobotNPC = null;
            }
        }

        /// <summary>
        /// 读取地图任务配置
        /// </summary>
        /// <returns></returns>
        public int LoadMapQuest()
        {
            var result = 1;
            var sMap = string.Empty;
            var s1C = string.Empty;
            var s20 = string.Empty;
            var sMonName = string.Empty;
            var sItem = string.Empty;
            var sQuest = string.Empty;
            var s30 = string.Empty;
            var s34 = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("MapQuest.txt");
            if (File.Exists(sFileName))
            {
                var tMapQuestList = new StringList();
                tMapQuestList.LoadFromFile(sFileName);
                for (var i = 0; i < tMapQuestList.Count; i++)
                {
                    var tStr = tMapQuestList[i];
                    if (!string.IsNullOrEmpty(tStr) && tStr[0] != ';')
                    {
                        tStr = HUtil32.GetValidStr3(tStr, ref sMap, TextSpitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s1C, TextSpitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s20, TextSpitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref sMonName, TextSpitConst);
                        if (!string.IsNullOrEmpty(sMonName) && sMonName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sMonName, "\"", "\"", ref sMonName);
                        }
                        tStr = HUtil32.GetValidStr3(tStr, ref sItem, TextSpitConst);
                        if (!string.IsNullOrEmpty(sItem) && sItem[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sItem, "\"", "\"", ref sItem);
                        }
                        tStr = HUtil32.GetValidStr3(tStr, ref sQuest, TextSpitConst);
                        tStr = HUtil32.GetValidStr3(tStr, ref s30, TextSpitConst);
                        if (!string.IsNullOrEmpty(sMap) && !string.IsNullOrEmpty(sMonName) && !string.IsNullOrEmpty(sQuest))
                        {
                            var Map = M2Share.MapMgr.FindMap(sMap);
                            if (Map != null)
                            {
                                HUtil32.ArrestStringEx(s1C, "[", "]", ref s34);
                                var n38 = HUtil32.StrToInt(s34, 0);
                                var n3C = HUtil32.StrToInt(s20, 0);
                                var boGrouped = HUtil32.CompareLStr(s30, "GROUP");
                                if (!Map.CreateQuest(n38, n3C, sMonName, sItem, sQuest, boGrouped))
                                {
                                    result = -i;
                                }
                            }
                            else
                            {
                                result = -i;
                            }
                        }
                        else
                        {
                            result = -i;
                        }
                    }
                }
            }
            QMangeNPC();
            QFunctionNPC();
            RobotNPC();
            return result;
        }

        /// <summary>
        /// 读取交易商人配置
        /// </summary>
        public void LoadMerchant()
        {
            var sScript = string.Empty;
            var sMapName = string.Empty;
            var sX = string.Empty;
            var sY = string.Empty;
            var sName = string.Empty;
            var sFlag = string.Empty;
            var sAppr = string.Empty;
            var sIsCalste = string.Empty;
            var sCanMove = string.Empty;
            var sMoveTime = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("Merchant.txt");
            if (File.Exists(sFileName))
            {
                var tMerchantList = new StringList();
                tMerchantList.LoadFromFile(sFileName);
                for (var i = 0; i < tMerchantList.Count; i++)
                {
                    var sLineText = tMerchantList[i].Trim();
                    if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                    {
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sScript, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sMapName, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sX, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sY, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sName, TextSpitConst);
                        if (!string.IsNullOrEmpty(sName) && sName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sName, "\"", "\"", ref sName);
                        }
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sFlag, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sAppr, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sIsCalste, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sCanMove, TextSpitConst);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sMoveTime, TextSpitConst);
                        if (!string.IsNullOrEmpty(sScript) && !string.IsNullOrEmpty(sMapName) && !string.IsNullOrEmpty(sAppr))
                        {
                            Merchant merchantNpc = new()
                            {
                                ScriptName = sScript,
                                MapName = sMapName,
                                CurrX = HUtil32.StrToInt16(sX, 0),
                                CurrY = HUtil32.StrToInt16(sY, 0),
                                ChrName = sName,
                                NpcFlag = HUtil32.StrToInt16(sFlag, 0),
                                Appr = (ushort)HUtil32.StrToInt(sAppr, 0),
                                MoveTime = HUtil32.StrToInt(sMoveTime, 0)
                            };
                            if (HUtil32.StrToInt(sIsCalste, 0) != 0)
                            {
                                merchantNpc.CastleMerchant = true;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && merchantNpc.MoveTime > 0)
                            {
                                merchantNpc.BoCanMove = true;
                            }
                            M2Share.WorldEngine.AddMerchant(merchantNpc);
                        }
                    }
                }
            }
        }

        private static void LoadMapGen(StringList MonGenList, string sFileName)
        {
            var sFileDir = M2Share.GetEnvirFilePath("MonGen");
            if (!Directory.Exists(sFileDir))
            {
                Directory.CreateDirectory(sFileDir);
            }
            var sFilePatchName = sFileDir + sFileName;
            if (!File.Exists(sFilePatchName)) return;
            using var LoadList = new StringList();
            LoadList.LoadFromFile(sFilePatchName);
            for (var i = 0; i < LoadList.Count; i++)
            {
                MonGenList.Add(LoadList[i]);
            }
        }

        /// <summary>
        /// 读取怪物刷新配置信息
        /// </summary>
        /// <returns></returns>
        public int LoadMonGen(out int mongenCount)
        {
            var sLineText = string.Empty;
            var sData = string.Empty;
            int i;
            var result = 0;
            mongenCount = 0;
            var sFileName = M2Share.GetEnvirFilePath("MonGen.txt");
            if (File.Exists(sFileName))
            {
                using var LoadList = new StringList();
                LoadList.LoadFromFile(sFileName);
                i = 0;
                while (true)
                {
                    if (i >= LoadList.Count)
                    {
                        break;
                    }
                    if (HUtil32.CompareLStr("loadgen", LoadList[i]))
                    {
                        var sMapGenFile = HUtil32.GetValidStr3(LoadList[i], ref sLineText, TextSpitConst);
                        LoadList.RemoveAt(i);
                        if (!string.IsNullOrEmpty(sMapGenFile))
                        {
                            LoadMapGen(LoadList, sMapGenFile);
                        }
                    }
                    i++;
                }
                MonGenInfo MonGenInfo = null;
                for (i = 0; i < LoadList.Count; i++)
                {
                    sLineText = LoadList[i];
                    if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                    {
                        MonGenInfo = new MonGenInfo();
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.MapName = sData;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.X = HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.Y = HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStrCap(sLineText, ref sData, TextSpitConst);
                        if (!string.IsNullOrEmpty(sData) && sData[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sData, "\"", "\"", ref sData);
                        }
                        MonGenInfo.MonName = sData;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.Range = HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.Count = HUtil32.StrToInt(sData, 0);
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.ZenTime = HUtil32.StrToInt(sData, -1) * 60 * 1000;
                        sLineText = HUtil32.GetValidStr3(sLineText, ref sData, TextSpitConst);
                        MonGenInfo.MissionGenRate = HUtil32.StrToInt(sData, 0);// 集中座标刷新机率 1 -100
                        if (!string.IsNullOrEmpty(MonGenInfo.MapName) && !string.IsNullOrEmpty(MonGenInfo.MonName) && MonGenInfo.ZenTime > 0 && M2Share.MapMgr.GetMapInfo(M2Share.ServerIndex, MonGenInfo.MapName) != null)
                        {
                            MonGenInfo.CertList = new List<BaseObject>();
                            MonGenInfo.Envir = M2Share.MapMgr.FindMap(MonGenInfo.MapName);
                            if (MonGenInfo.Envir != null)
                            {
                                M2Share.WorldEngine.MonGenList.Add(MonGenInfo);
                            }
                            else
                            {
                                MonGenInfo = null;
                            }
                        }
                    }
                }
                MonGenInfo = new MonGenInfo
                {
                    CertList = new List<BaseObject>(),
                    Envir = null
                };
                if (M2Share.WorldEngine.MonGenInfoThreadMap.ContainsKey(0))
                {
                    M2Share.WorldEngine.MonGenInfoThreadMap[0].Add(MonGenInfo);
                }
                else
                {
                    M2Share.WorldEngine.MonGenInfoThreadMap.Add(0, new List<MonGenInfo>() { MonGenInfo });
                }
                result = 1;
                mongenCount = M2Share.WorldEngine.MonGenList.Sum(x => x.Count);
            }
            return result;
        }

        /// <summary>
        /// 读取怪物物品掉落配置
        /// </summary>
        /// <returns></returns>
        public void LoadMonitems(string MonName, ref IList<MonsterDropItem> ItemList)
        {
            var sData = string.Empty;
            var monFileName = M2Share.GetEnvirFilePath("MonItems", $"{MonName}.txt");
            if (File.Exists(monFileName))
            {
                if (ItemList != null)
                {
                    for (var i = 0; i < ItemList.Count; i++)
                    {
                        ItemList[i] = null;
                    }
                    ItemList.Clear();
                }
                if (ItemList == null)
                {
                    ItemList = new List<MonsterDropItem>();
                }
                using var LoadList = new StringList();
                LoadList.LoadFromFile(monFileName);
                for (var i = 0; i < LoadList.Count; i++)
                {
                    var s28 = LoadList[i];
                    if (!string.IsNullOrEmpty(s28) && s28[0] != ';')
                    {
                        s28 = HUtil32.GetValidStr3(s28, ref sData, MonsterSpitConst);
                        var n18 = HUtil32.StrToInt(sData, -1);
                        s28 = HUtil32.GetValidStr3(s28, ref sData, MonsterSpitConst);
                        var n1C = HUtil32.StrToInt(sData, -1);
                        s28 = HUtil32.GetValidStr3(s28, ref sData, TextSpitConst);
                        if (!string.IsNullOrEmpty(sData))
                        {
                            if (sData[0] == '\"')
                            {
                                HUtil32.ArrestStringEx(sData, "\"", "\"", ref sData);
                            }
                        }
                        var itemName = sData;
                        s28 = HUtil32.GetValidStr3(s28, ref sData, TextSpitConst);
                        var itemCount = HUtil32.StrToInt(sData, 1);
                        if (n18 > 0 && n1C > 0 && !string.IsNullOrEmpty(itemName))
                        {
                            var MonItem = new MonsterDropItem
                            {
                                SelPoint = n18 - 1,
                                MaxPoint = n1C,
                                ItemName = itemName,
                                Count = itemCount
                            };
                            ItemList.Add(MonItem);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取管理NPC配置
        /// </summary>
        public void LoadNpcs()
        {
            var ChrName = string.Empty;
            var type = string.Empty;
            var mapName = string.Empty;
            var cX = string.Empty;
            var cY = string.Empty;
            var flag = string.Empty;
            var appr = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("Npcs.txt");
            if (File.Exists(sFileName))
            {
                using var LoadList = new StringList();
                LoadList.LoadFromFile(sFileName);
                for (var i = 0; i < LoadList.Count; i++)
                {
                    var sData = LoadList[i].Trim();
                    if (!string.IsNullOrEmpty(sData) && sData[0] != ';')
                    {
                        sData = HUtil32.GetValidStrCap(sData, ref ChrName, TextSpitConst);
                        if (!string.IsNullOrEmpty(ChrName) && ChrName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(ChrName, "\"", "\"", ref ChrName);
                        }
                        sData = HUtil32.GetValidStr3(sData, ref type, TextSpitConst);
                        sData = HUtil32.GetValidStr3(sData, ref mapName, TextSpitConst);
                        sData = HUtil32.GetValidStr3(sData, ref cX, TextSpitConst);
                        sData = HUtil32.GetValidStr3(sData, ref cY, TextSpitConst);
                        sData = HUtil32.GetValidStr3(sData, ref flag, TextSpitConst);
                        sData = HUtil32.GetValidStr3(sData, ref appr, TextSpitConst);
                        if (!string.IsNullOrEmpty(ChrName) && !string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(appr))
                        {
                            NormNpc NPC = null;
                            switch (HUtil32.StrToInt(type, 0))
                            {
                                case 0:
                                    NPC = new Merchant();
                                    break;
                                case 1:
                                    NPC = new GuildOfficial();
                                    break;
                                case 2:
                                    NPC = new CastleOfficial();
                                    break;
                            }
                            if (NPC != null)
                            {
                                NPC.MapName = mapName;
                                NPC.CurrX = HUtil32.StrToInt16(cX, 0);
                                NPC.CurrY = HUtil32.StrToInt16(cY, 0);
                                NPC.ChrName = ChrName;
                                NPC.NpcFlag = HUtil32.StrToInt16(flag, 0);
                                NPC.Appr = (ushort)HUtil32.StrToInt(appr, 0);
                                M2Share.WorldEngine.QuestNpcList.Add(NPC);
                            }
                        }
                    }
                }
            }
        }

        private static string LoadQuestDiary_sub_48978C(int nIndex)
        {
            string result;
            if (nIndex >= 1000)
            {
                result = nIndex.ToString();
                return result;
            }
            if (nIndex >= 100)
            {
                result = nIndex.ToString() + '0';
                return result;
            }
            result = nIndex + "00";
            return result;
        }

        public int LoadQuestDiary()
        {
            var result = 1;
            var s18 = string.Empty;
            var s20 = string.Empty;
            var bo2D = false;
            var nC = 1;
            M2Share.QuestDiaryList.Clear();
            while (true)
            {
                IList<TQDDinfo> QDDinfoList = null;
                var sFileName = M2Share.GetEnvirFilePath("QuestDiary", LoadQuestDiary_sub_48978C(nC) + ".txt");
                if (File.Exists(sFileName))
                {
                    s18 = string.Empty;
                    TQDDinfo QDDinfo = null;
                    using var LoadList = new StringList();
                    LoadList.LoadFromFile(sFileName);
                    for (var i = 0; i < LoadList.Count; i++)
                    {
                        var s1C = LoadList[i];
                        if (!string.IsNullOrEmpty(s1C) && s1C[0] != ';')
                        {
                            if (s1C[0] == '[' && s1C.Length > 2)
                            {
                                if (string.IsNullOrEmpty(s18))
                                {
                                    HUtil32.ArrestStringEx(s1C, "[", "]", ref s18);
                                    QDDinfoList = new List<TQDDinfo>();
                                    QDDinfo = new TQDDinfo
                                    {
                                        n00 = nC,
                                        s04 = s18,
                                        sList = new ArrayList()
                                    };
                                    QDDinfoList.Add(QDDinfo);
                                    bo2D = true;
                                }
                                else
                                {
                                    if (s1C[0] != '@')
                                    {
                                        s1C = HUtil32.GetValidStr3(s1C, ref s20, TextSpitConst);
                                        HUtil32.ArrestStringEx(s20, "[", "]", ref s20);
                                        QDDinfo = new TQDDinfo
                                        {
                                            n00 = HUtil32.StrToInt(s20, 0),
                                            s04 = s1C,
                                            sList = new ArrayList()
                                        };
                                        QDDinfoList.Add(QDDinfo);
                                        bo2D = true;
                                    }
                                    else
                                    {
                                        bo2D = false;
                                    }
                                }
                            }
                            else
                            {
                                if (bo2D)
                                {
                                    QDDinfo.sList.Add(s1C);
                                }
                            }
                        }
                    }
                }
                if (QDDinfoList != null)
                {
                    M2Share.QuestDiaryList.Add(QDDinfoList);
                }
                else
                {
                    M2Share.QuestDiaryList.Add(null);
                }
                nC++;
                if (nC >= 105)
                {
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// 读取安全区配置
        /// </summary>
        public void LoadStartPoint()
        {
            var mapName = string.Empty;
            var cX = string.Empty;
            var cY = string.Empty;
            var allSay = string.Empty;
            var range = string.Empty;
            var type = string.Empty;
            var zone = string.Empty;
            var fire = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("StartPoint.txt");
            if (File.Exists(sFileName))
            {
                M2Share.StartPointList.Clear();
                using var LoadList = new StringList();
                LoadList.LoadFromFile(sFileName);
                for (var i = 0; i < LoadList.Count; i++)
                {
                    var sLine = LoadList[i].Trim();
                    if (!string.IsNullOrEmpty(sLine) && sLine[0] != ';')
                    {
                        sLine = HUtil32.GetValidStr3(sLine, ref mapName, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref cX, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref cY, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref allSay, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref range, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref type, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref zone, TextSpitConst);
                        sLine = HUtil32.GetValidStr3(sLine, ref fire, TextSpitConst);
                        if (!string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(cX) && cY != "")
                        {
                            var startPoint = new StartPoint
                            {
                                MapName = mapName,
                                CurrX = HUtil32.StrToInt16(cX, 0),
                                CurrY = HUtil32.StrToInt16(cY, 0),
                                NotAllowSay = Convert.ToBoolean(HUtil32.StrToInt(allSay, 0)),
                                Range = HUtil32.StrToInt(range, 0),
                                Type = HUtil32.StrToInt(type, 0),
                                PkZone = HUtil32.StrToInt(zone, 0),
                                PkFire = HUtil32.StrToInt(fire, 0)
                            };
                            M2Share.StartPointList.Add(startPoint);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取解包物品配置
        /// </summary>
        /// <returns></returns>
        public int LoadUnbindList()
        {
            var result = 0;
            var sData = string.Empty;
            var sItemName = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("UnbindList.txt");
            if (File.Exists(sFileName))
            {
                using var LoadList = new StringList();
                LoadList.LoadFromFile(sFileName);
                for (var i = 0; i < LoadList.Count; i++)
                {
                    var readLine = LoadList[i];
                    if (!string.IsNullOrEmpty(readLine) && readLine[0] != ';')
                    {
                        readLine = HUtil32.GetValidStr3(readLine, ref sData, TextSpitConst);
                        readLine = HUtil32.GetValidStrCap(readLine, ref sItemName, TextSpitConst);
                        if (!string.IsNullOrEmpty(sItemName) && sItemName[0] == '\"')
                        {
                            HUtil32.ArrestStringEx(sItemName, "\"", "\"", ref sItemName);
                        }
                        var n10 = HUtil32.StrToInt(sData, 0);
                        if (n10 > 0)
                        {
                            if (M2Share.UnbindList.ContainsKey(n10))
                            {
                                M2Share.Logger.Warn($"重复解包物品[{sItemName}]...");
                                continue;
                            }
                            M2Share.UnbindList.Add(n10, sItemName);
                        }
                        else
                        {
                            result = -i;// 需要取负数
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static int SaveGoodRecord(Merchant NPC, string sFile)
        {
            var result = -1;
            var sFileName = ".\\Envir\\Market_Saved\\" + sFile + ".sav";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Write | FileShare.ReadWrite);
            //}
            //else
            //{
            //    FileHandle = File.Create(sFileName);
            //}
            //if (FileHandle > 0)
            //{
            //    
            //    FillChar(Header420, sizeof(TGoodFileHeader), '\0');
            //    for (I = 0; I < NPC.m_GoodsList.Count; I ++ )
            //    {
            //        List = ((NPC.m_GoodsList[I]) as ArrayList);
            //        Header420.nItemCount += List.Count;
            //    }
            //    
            //    FileWrite(FileHandle, Header420, sizeof(TGoodFileHeader));
            //    for (I = 0; I < NPC.m_GoodsList.Count; I ++ )
            //    {
            //        List = ((NPC.m_GoodsList[I]) as ArrayList);
            //        for (II = 0; II < List.Count; II ++ )
            //        {
            //            UserItem = List[II];
            //            
            //            FileWrite(FileHandle, UserItem, sizeof(TUserItem));
            //        }
            //    }
            //    result = 1;
            //}
            return result;
        }

        public static int SaveGoodPriceRecord(Merchant NPC, string sFile)
        {
            var result = -1;
            var sFileName = ".\\Envir\\Market_Prices\\" + sFile + ".prc";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Write | FileShare.ReadWrite);
            //}
            //else
            //{
            //    FileHandle = File.Create(sFileName);
            //}
            //if (FileHandle > 0)
            //{
            //    
            //    FillChar(Header420, sizeof(TGoodFileHeader), '\0');
            //    Header420.nItemCount = NPC.m_ItemPriceList.Count;
            //    
            //    FileWrite(FileHandle, Header420, sizeof(TGoodFileHeader));
            //    for (I = 0; I < NPC.m_ItemPriceList.Count; I ++ )
            //    {
            //        ItemPrice = NPC.m_ItemPriceList[I];
            //        
            //        FileWrite(FileHandle, ItemPrice, sizeof(TItemPrice));
            //    }
            //    result = 1;
            //}
            return result;
        }

        public static void ReLoadNpc()
        {

        }

        public void ReLoadMerchants()
        {
            var sScript = string.Empty;
            var sMapName = string.Empty;
            var sX = string.Empty;
            var sY = string.Empty;
            var sChrName = string.Empty;
            var sFlag = string.Empty;
            var sAppr = string.Empty;
            var sCastle = string.Empty;
            var sCanMove = string.Empty;
            var sMoveTime = string.Empty;
            Merchant Merchant;
            var sFileName = M2Share.GetEnvirFilePath("Merchant.txt");
            if (!File.Exists(sFileName))
            {
                return;
            }
            for (var i = 0; i < M2Share.WorldEngine.MerchantList.Count; i++)
            {
                Merchant = M2Share.WorldEngine.MerchantList[i];
                if (Merchant != M2Share.FunctionNPC)
                {
                    Merchant.NpcFlag = -1;
                }
            }
            using var LoadList = new StringList();
            LoadList.LoadFromFile(sFileName);
            for (var i = 0; i < LoadList.Count; i++)
            {
                var sLineText = LoadList[i].Trim();
                if (!string.IsNullOrEmpty(sLineText) && sLineText[0] != ';')
                {
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sScript, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sMapName, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sX, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sY, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sChrName, TextSpitConst);
                    if (sChrName != "" && sChrName[0] == '\"')
                    {
                        HUtil32.ArrestStringEx(sChrName, "\"", "\"", ref sChrName);
                    }
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sFlag, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sAppr, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sCastle, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sCanMove, TextSpitConst);
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sMoveTime, TextSpitConst);
                    var nX = HUtil32.StrToInt(sX, 0);
                    var nY = HUtil32.StrToInt(sY, 0);
                    var boNewNpc = true;
                    for (var j = 0; j < M2Share.WorldEngine.MerchantList.Count; j++)
                    {
                        Merchant = M2Share.WorldEngine.MerchantList[j];
                        if (Merchant.MapName == sMapName && Merchant.CurrX == nX && Merchant.CurrY == nY)
                        {
                            boNewNpc = false;
                            Merchant.ScriptName = sScript;
                            Merchant.ChrName = sChrName;
                            Merchant.NpcFlag = HUtil32.StrToInt16(sFlag, 0);
                            Merchant.Appr = (ushort)HUtil32.StrToInt(sAppr, 0);
                            Merchant.MoveTime = HUtil32.StrToInt(sMoveTime, 0);
                            if (HUtil32.StrToInt(sCastle, 0) != 1)
                            {
                                Merchant.CastleMerchant = true;
                            }
                            else
                            {
                                Merchant.CastleMerchant = false;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && Merchant.MoveTime > 0)
                            {
                                Merchant.BoCanMove = true;
                            }
                            break;
                        }
                    }
                    if (boNewNpc)
                    {
                        Merchant = new Merchant
                        {
                            MapName = sMapName
                        };
                        Merchant.Envir = M2Share.MapMgr.FindMap(Merchant.MapName);
                        if (Merchant.Envir != null)
                        {
                            Merchant.ScriptName = sScript;
                            Merchant.CurrX = (short)nX;
                            Merchant.CurrY = (short)nY;
                            Merchant.ChrName = sChrName;
                            Merchant.NpcFlag = HUtil32.StrToInt16(sFlag, 0);
                            Merchant.Appr = (ushort)HUtil32.StrToInt(sAppr, 0);
                            Merchant.MoveTime = HUtil32.StrToInt(sMoveTime, 0);
                            if (HUtil32.StrToInt(sCastle, 0) != 1)
                            {
                                Merchant.CastleMerchant = true;
                            }
                            else
                            {
                                Merchant.CastleMerchant = false;
                            }
                            if (HUtil32.StrToInt(sCanMove, 0) != 0 && Merchant.MoveTime > 0)
                            {
                                Merchant.BoCanMove = true;
                            }
                            M2Share.WorldEngine.MerchantList.Add(Merchant);
                            Merchant.Initialize();
                        }
                    }
                }
            }
            for (var i = M2Share.WorldEngine.MerchantList.Count - 1; i >= 0; i--)
            {
                Merchant = M2Share.WorldEngine.MerchantList[i];
                if (Merchant.NpcFlag == -1)
                {
                    Merchant.Ghost = true;
                    Merchant.GhostTick = HUtil32.GetTickCount();
                    M2Share.WorldEngine.MerchantList.RemoveAt(i);
                }
            }
        }

        public static int LoadGoodRecord(Merchant NPC, string sFile)
        {
            var result = -1;
            var sFileName = ".\\Envir\\Market_Saved\\" + sFile + ".sav";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode) FileAccess.Read | FileShare.ReadWrite);
            //    List = null;
            //    if (FileHandle > 0)
            //    {
            //        
            //        if (FileRead(FileHandle, Header420, sizeof(TGoodFileHeader)) == sizeof(TGoodFileHeader))
            //        {
            //            for (I = 0; I < Header420.nItemCount; I ++ )
            //            {
            //                UserItem = new TUserItem();
            //                
            //                if (FileRead(FileHandle, UserItem, sizeof(TUserItem)) == sizeof(TUserItem))
            //                {
            //                    if (List == null)
            //                    {
            //                        List = new ArrayList();
            //                        List.Add(UserItem);
            //                    }
            //                    else
            //                    {
            //                        if (((TUserItem)(List[0])).wIndex == UserItem.wIndex)
            //                        {
            //                            List.Add(UserItem);
            //                        }
            //                        else
            //                        {
            //                            NPC.m_GoodsList.Add(List);
            //                            List = new ArrayList();
            //                            List.Add(UserItem);
            //                        }
            //                    }
            //                }
            //            }
            //            if (List != null)
            //            {
            //                NPC.m_GoodsList.Add(List);
            //            }
            //            FileHandle.Close();
            //            result = 1;
            //        }
            //    }
            //}
            return result;
        }

        public static int LoadGoodPriceRecord(Merchant NPC, string sFile)
        {
            var result = -1;
            var sFileName = ".\\Envir\\Market_Prices\\" + sFile + ".prc";
            //if (File.Exists(sFileName))
            //{
            //    FileHandle = File.Open(sFileName, (FileMode)FileAccess.Read | FileShare.ReadWrite);
            //    if (FileHandle > 0)
            //    {
            //        @ Unsupported function or procedure: 'FileRead'
            //        if (FileRead(FileHandle, Header420, sizeof(TGoodFileHeader)) == sizeof(TGoodFileHeader))
            //        {
            //            for (I = 0; I < Header420.nItemCount; I++)
            //            {
            //                ItemPrice = new TItemPrice();
            //                @ Unsupported function or procedure: 'FileRead'
            //                if (FileRead(FileHandle, ItemPrice, sizeof(TItemPrice)) == sizeof(TItemPrice))
            //                {
            //                    NPC.m_ItemPriceList.Add(ItemPrice);
            //                }
            //                else
            //                {
            //                    @ Unsupported function or procedure: 'Dispose'
            //                    Dispose(ItemPrice);
            //                    break;
            //                }
            //            }
            //        }
            //        FileHandle.Close();
            //        result = 1;
            //    }
            //}
            return result;
        }
    }
}