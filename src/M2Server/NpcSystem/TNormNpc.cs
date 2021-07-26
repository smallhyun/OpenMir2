﻿using SystemModule;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SystemModule.Common;

namespace M2Server
{
    public class TNormNpc : TAnimalObject
    {
        public int n54C = 0;
        /// <summary>
        /// 用于标识此NPC是否有效，用于重新加载NPC列表(-1 为无效)
        /// </summary>
        public short m_nFlag = 0;
        public int[] FGotoLable;
        public IList<TScript> m_ScriptList = null;
        public string m_sFilePath = string.Empty;
        /// <summary>
        /// 此NPC是否是隐藏的，不显示在地图中
        /// </summary>
        public bool m_boIsHide = false;
        /// <summary>
        ///  NPC类型为地图任务型的，加载脚本时的脚本文件名为 角色名-地图号.txt
        /// </summary>
        public bool m_boIsQuest = false;
        public string m_sPath = string.Empty;

        private void ActionOfAddNameDateList(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            StringList LoadList;
            bool boFound;
            string sListFileName;
            string sLineText;
            var sHumName = string.Empty;
            var sDate = string.Empty;
            sListFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestActionInfo.sParam1;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {

                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            boFound = false;
            for (var I = 0; I < LoadList.Count; I++)
            {
                sLineText = LoadList[I].Trim();
                sLineText = HUtil32.GetValidStr3(sLineText, ref sHumName, new string[] { " ", "\t" });
                sLineText = HUtil32.GetValidStr3(sLineText, ref sDate, new string[] { " ", "\t" });
                if (sHumName.ToLower().CompareTo(PlayObject.m_sCharName.ToLower()) == 0)
                {
                    LoadList[I] = PlayObject.m_sCharName + "\t" + DateTime.Today.ToString();
                    boFound = true;
                    break;
                }
            }
            if (!boFound)
            {
                LoadList.Add(PlayObject.m_sCharName + "\t" + DateTime.Today.ToString());
            }
            try
            {

                LoadList.SaveToFile(sListFileName);
            }
            catch
            {
                M2Share.MainOutMessage("saving fail.... => " + sListFileName);
            }

            //LoadList.Free;
        }

        private void ActionOfDelNameDateList(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            StringList LoadList;
            string sLineText;
            string sListFileName;
            string sHumName = string.Empty;
            string sDate = string.Empty;
            bool boFound;
            sListFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestActionInfo.sParam1;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {

                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            boFound = false;
            for (I = 0; I < LoadList.Count; I++)
            {
                sLineText = LoadList[I].Trim();
                sLineText = HUtil32.GetValidStr3(sLineText, ref sHumName, new string[] { " ", "\t" });
                sLineText = HUtil32.GetValidStr3(sLineText, ref sDate, new string[] { " ", "\t" });
                if (sHumName.ToLower().CompareTo(PlayObject.m_sCharName.ToLower()) == 0)
                {
                    LoadList.RemoveAt(I);
                    boFound = true;
                    break;
                }
            }
            if (boFound)
            {
                try
                {
                    LoadList.SaveToFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("saving fail.... => " + sListFileName);
                }
            }
            //LoadList.Free;
        }

        private void ActionOfAddSkill(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TMagic Magic;
            TUserMagic UserMagic;
            int nLevel;
            nLevel = HUtil32._MIN(3, HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0));
            Magic = M2Share.UserEngine.FindMagic(QuestActionInfo.sParam1);
            if (Magic != null)
            {
                if (!PlayObject.IsTrainingSkill(Magic.wMagicID))
                {
                    UserMagic = new TUserMagic();
                    UserMagic.MagicInfo = Magic;
                    UserMagic.wMagIdx = Magic.wMagicID;
                    UserMagic.btKey = 0;
                    UserMagic.btLevel = (byte)nLevel;
                    UserMagic.nTranPoint = 0;
                    PlayObject.m_MagicList.Add(UserMagic);
                    PlayObject.SendAddMagic(UserMagic);
                    PlayObject.RecalcAbilitys();
                    if (M2Share.g_Config.boShowScriptActionMsg)
                    {
                        PlayObject.SysMsg(Magic.sMagicName + "练习成功。", TMsgColor.c_Green, TMsgType.t_Hint);
                    }
                }
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_ADDSKILL);
            }
        }

        private void ActionOfAutoAddGameGold(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo, int nPoint, int nTime)
        {
            if (QuestActionInfo.sParam1.ToLower().CompareTo("START".ToLower()) == 0)
            {
                if (nPoint > 0 && nTime > 0)
                {
                    PlayObject.m_nIncGameGold = nPoint;
                    PlayObject.m_dwIncGameGoldTime = nTime * 1000;

                    PlayObject.m_dwIncGameGoldTick = HUtil32.GetTickCount();
                    PlayObject.m_boIncGameGold = true;
                    return;
                }
            }
            if (QuestActionInfo.sParam1.ToLower().CompareTo("STOP".ToLower()) == 0)
            {
                PlayObject.m_boIncGameGold = false;
                return;
            }
            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_AUTOADDGAMEGOLD);
        }

        // SETAUTOGETEXP 时间 点数 是否安全区 地图号
        private void ActionOfAutoGetExp(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nTime;
            int nPoint;
            bool boIsSafeZone;
            string sMap;
            TEnvirnoment Envir;
            Envir = null;
            nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            boIsSafeZone = QuestActionInfo.sParam3[1] == '1';
            sMap = QuestActionInfo.sParam4;
            if (sMap != "")
            {
                Envir = M2Share.g_MapManager.FindMap(sMap);
            }
            if ((nTime <= 0) || (nPoint <= 0) || ((sMap != "") && (Envir == null)))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SETAUTOGETEXP);
                return;
            }
            PlayObject.m_boAutoGetExpInSafeZone = boIsSafeZone;
            PlayObject.m_AutoGetExpEnvir = Envir;
            PlayObject.m_nAutoGetExpTime = nTime * 1000;
            PlayObject.m_nAutoGetExpPoint = nPoint;
        }

        /// <summary>
        /// 增加挂机
        /// </summary>
        /// <param name="PlayObject"></param>
        /// <param name="QuestActionInfo"></param>
        private void ActionOfOffLine(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nTime;
            int nPoint;
            int nKickOffLine;
            string sOffLineStartMsg;
            sOffLineStartMsg = "系统已经为你开启了脱机泡点功能，你现在可以下线了……";
            PlayObject.m_DefMsg = grobal2.MakeDefaultMsg(grobal2.SM_SYSMESSAGE, PlayObject.ObjectId, HUtil32.MakeWord(M2Share.g_Config.btCustMsgFColor, M2Share.g_Config.btCustMsgBColor), 0, 1);
            PlayObject.SendSocket(PlayObject.m_DefMsg, EDcode.EncodeString(sOffLineStartMsg));
            nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam1, 5);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, 500);
            nKickOffLine = HUtil32.Str_ToInt(QuestActionInfo.sParam3, 1440 * 15);
            PlayObject.m_boAutoGetExpInSafeZone = true;
            PlayObject.m_AutoGetExpEnvir = PlayObject.m_PEnvir;
            PlayObject.m_nAutoGetExpTime = nTime * 1000;
            PlayObject.m_nAutoGetExpPoint = nPoint;
            PlayObject.m_boOffLineFlag = true;

            PlayObject.m_dwKickOffLineTick = HUtil32.GetTickCount() + (nKickOffLine * 60 * 1000);
            IdSrvClient.Instance.SendHumanLogOutMsgA(PlayObject.m_sUserID, PlayObject.m_nSessionID);
            PlayObject.SendDefMessage(grobal2.SM_OUTOFCONNECTION, 0, 0, 0, 0, "");
        }

        private void ActionOfAutoSubGameGold(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo, int nPoint, int nTime)
        {
            if (QuestActionInfo.sParam1.ToLower().CompareTo("START".ToLower()) == 0)
            {
                if ((nPoint > 0) && (nTime > 0))
                {
                    PlayObject.m_nDecGameGold = nPoint;
                    PlayObject.m_dwDecGameGoldTime = nTime * 1000;
                    PlayObject.m_dwDecGameGoldTick = 0;
                    PlayObject.m_boDecGameGold = true;
                    return;
                }
            } 
            if (QuestActionInfo.sParam1.ToLower().CompareTo("STOP".ToLower()) == 0)
            {
                PlayObject.m_boDecGameGold = false;
                return;
            }
            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_AUTOSUBGAMEGOLD);
        }

        private void ActionOfChangeCreditPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            char cMethod;
            int nCreditPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nCreditPoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CREDITPOINT);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nCreditPoint >= 0)
                    {
                        if (nCreditPoint > byte.MaxValue)
                        {
                            PlayObject.m_btCreditPoint = byte.MaxValue;
                        }
                        else
                        {
                            PlayObject.m_btCreditPoint = (byte)nCreditPoint;
                        }
                    }
                    break;
                case '-':
                    if (PlayObject.m_btCreditPoint > (byte)nCreditPoint)
                    {
                        PlayObject.m_btCreditPoint -= (byte)nCreditPoint;
                    }
                    else
                    {
                        PlayObject.m_btCreditPoint = 0;
                    }
                    break;
                case '+':
                    if (PlayObject.m_btCreditPoint + (byte)nCreditPoint > byte.MaxValue)
                    {
                        PlayObject.m_btCreditPoint = byte.MaxValue;
                    }
                    else
                    {
                        PlayObject.m_btCreditPoint += (byte)nCreditPoint;
                    }
                    break;
                default:
                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CREDITPOINT);
                    return;
            }
        }

        private void ActionOfChangeExp(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nExp;
            char cMethod;
            int dwInt;
            nExp = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nExp < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEEXP);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nExp >= 0)
                    {
                        PlayObject.m_Abil.Exp = nExp;
                        dwInt = nExp;
                    }
                    break;
                case '-':
                    if (PlayObject.m_Abil.Exp > nExp)
                    {
                        PlayObject.m_Abil.Exp -= nExp;
                    }
                    else
                    {
                        PlayObject.m_Abil.Exp = 0;
                    }
                    break;
                case '+':
                    if (PlayObject.m_Abil.Exp >= nExp)
                    {
                        if ((PlayObject.m_Abil.Exp - nExp) > (int.MaxValue - PlayObject.m_Abil.Exp))
                        {
                            dwInt = int.MaxValue - PlayObject.m_Abil.Exp;
                        }
                        else
                        {
                            dwInt = nExp;
                        }
                    }
                    else
                    {
                        if ((nExp - PlayObject.m_Abil.Exp) > (int.MaxValue - nExp))
                        {
                            dwInt = int.MaxValue - nExp;
                        }
                        else
                        {
                            dwInt = nExp;
                        }
                    }
                    PlayObject.m_Abil.Exp += dwInt;
                    // PlayObject.GetExp(dwInt);
                    PlayObject.SendMsg(PlayObject, grobal2.RM_WINEXP, 0, dwInt, 0, 0, "");
                    break;
            }
        }

        private void ActionOfChangeHairStyle(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nHair;
            nHair = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if ((QuestActionInfo.sParam1 != "") && (nHair >= 0))
            {
                PlayObject.m_btHair = (byte)nHair;
                PlayObject.FeatureChanged();
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_HAIRSTYLE);
            }
        }

        private void ActionOfChangeJob(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nJob;
            nJob = -1;
            if (HUtil32.CompareLStr(QuestActionInfo.sParam1, M2Share.sWarrior, M2Share.sWarrior.Length))
            {
                nJob = M2Share.jWarr;
            }
            if (HUtil32.CompareLStr(QuestActionInfo.sParam1, M2Share.sWizard, M2Share.sWizard.Length))
            {
                nJob = M2Share.jWizard;
            }
            if (HUtil32.CompareLStr(QuestActionInfo.sParam1, M2Share.sTaos, M2Share.sTaos.Length))
            {
                nJob = M2Share.jTaos;
            }
            if (nJob < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEJOB);
                return;
            }
            if (PlayObject.m_btJob != nJob)
            {
                PlayObject.m_btJob = (byte)nJob;
                // 
                // PlayObject.RecalcLevelAbilitys();
                // PlayObject.RecalcAbilitys();
                // 
                PlayObject.HasLevelUp(0);
            }
        }

        private void ActionOfChangeLevel(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            bool boChgOK;
            int nLevel;
            int nLv;
            int nOldLevel;
            char cMethod;
            boChgOK = false;
            nOldLevel = PlayObject.m_Abil.Level;
            nLevel = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGELEVEL);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if ((nLevel > 0) && (nLevel <= grobal2.MAXLEVEL))
                    {
                        PlayObject.m_Abil.Level = (ushort)nLevel;
                        boChgOK = true;
                    }
                    break;
                case '-':
                    nLv = HUtil32._MAX(0, PlayObject.m_Abil.Level - nLevel);
                    nLv = HUtil32._MIN(grobal2.MAXLEVEL, nLv);
                    PlayObject.m_Abil.Level = (ushort)nLv;
                    boChgOK = true;
                    break;
                case '+':
                    nLv = HUtil32._MAX(0, PlayObject.m_Abil.Level + nLevel);
                    nLv = HUtil32._MIN(grobal2.MAXLEVEL, nLv);
                    PlayObject.m_Abil.Level = (ushort)nLv;
                    boChgOK = true;
                    break;
            }
            if (boChgOK)
            {
                PlayObject.HasLevelUp(nOldLevel);
            }
        }

        private void ActionOfChangePkPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nPKPoint;
            int nPoint;
            int nOldPKLevel;
            char cMethod;
            nOldPKLevel = PlayObject.PKLevel();
            nPKPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nPKPoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEPKPOINT);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nPKPoint >= 0)
                    {
                        PlayObject.m_nPkPoint = nPKPoint;
                    }
                    break;
                case '-':
                    nPoint = HUtil32._MAX(0, PlayObject.m_nPkPoint - nPKPoint);
                    PlayObject.m_nPkPoint = nPoint;
                    break;
                case '+':
                    nPoint = HUtil32._MAX(0, PlayObject.m_nPkPoint + nPKPoint);
                    PlayObject.m_nPkPoint = nPoint;
                    break;
            }
            if (nOldPKLevel != PlayObject.PKLevel())
            {
                PlayObject.RefNameColor();
            }
        }

        private void ActionOfClearMapMon(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            ArrayList MonList;
            TBaseObject Mon;
            MonList = new ArrayList();
            M2Share.UserEngine.GetMapMonster(M2Share.g_MapManager.FindMap(QuestActionInfo.sParam1), MonList);
            for (var i = 0; i < MonList.Count; i++)
            {
                Mon = (TBaseObject)MonList[i];
                if (Mon.m_Master != null)
                {
                    continue;
                }
                if (M2Share.GetNoClearMonList(Mon.m_sCharName))
                {
                    continue;
                }
                Mon.m_boNoItem = true;
                Mon.MakeGhost();
            }
            //MonList.Free;
        }

        private void ActionOfClearNameList(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            StringList LoadList;
            string sListFileName;
            sListFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestActionInfo.sParam1;
            LoadList = new StringList();
            LoadList.Clear();
            try
            {
                LoadList.SaveToFile(sListFileName);
            }
            catch
            {
                M2Share.MainOutMessage("saving fail.... => " + sListFileName);
            }
            //LoadList.Free;
        }

        private void ActionOfClearSkill(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            TUserMagic UserMagic;
            for (I = PlayObject.m_MagicList.Count - 1; I >= 0; I--)
            {
                UserMagic = PlayObject.m_MagicList[I];
                PlayObject.SendDelMagic(UserMagic);

                Dispose(UserMagic);
                PlayObject.m_MagicList.RemoveAt(I);
            }
            PlayObject.RecalcAbilitys();
        }

        private void ActionOfDelNoJobSkill(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            TUserMagic UserMagic;
            for (I = PlayObject.m_MagicList.Count - 1; I >= 0; I--)
            {
                UserMagic = PlayObject.m_MagicList[I];
                if (UserMagic.MagicInfo.btJob != PlayObject.m_btJob)
                {
                    PlayObject.SendDelMagic(UserMagic);

                    Dispose(UserMagic);
                    PlayObject.m_MagicList.RemoveAt(I);
                }
            }
        }

        private void ActionOfDelSkill(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            string sMagicName;
            TMagic Magic;
            TUserMagic UserMagic;
            sMagicName = QuestActionInfo.sParam1;
            Magic = M2Share.UserEngine.FindMagic(sMagicName);
            if (Magic == null)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_DELSKILL);
                return;
            }
            for (I = 0; I < PlayObject.m_MagicList.Count; I++)
            {
                UserMagic = PlayObject.m_MagicList[I];
                if (UserMagic.MagicInfo == Magic)
                {
                    PlayObject.m_MagicList.RemoveAt(I);
                    PlayObject.SendDelMagic(UserMagic);

                    Dispose(UserMagic);
                    PlayObject.RecalcAbilitys();
                    break;
                }
            }
        }

        private void ActionOfGameGold(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nGameGold;
            int nOldGameGold;
            char cMethod;
            nOldGameGold = PlayObject.m_nGameGold;
            nGameGold = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nGameGold < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_GAMEGOLD);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nGameGold >= 0)
                    {
                        PlayObject.m_nGameGold = nGameGold;
                    }
                    break;
                case '-':
                    nGameGold = HUtil32._MAX(0, PlayObject.m_nGameGold - nGameGold);
                    PlayObject.m_nGameGold = nGameGold;
                    break;
                case '+':
                    nGameGold = HUtil32._MAX(0, PlayObject.m_nGameGold + nGameGold);
                    PlayObject.m_nGameGold = nGameGold;
                    break;
            }
            if (M2Share.g_boGameLogGameGold)
            {
                M2Share.AddGameDataLog(format(M2Share.g_sGameLogMsg1, grobal2.LOG_GAMEGOLD, PlayObject.m_sMapName, PlayObject.m_nCurrX, PlayObject.m_nCurrY, PlayObject.m_sCharName, M2Share.g_Config.sGameGoldName, nGameGold, cMethod, this.m_sCharName));
            }
            if (nOldGameGold != PlayObject.m_nGameGold)
            {
                PlayObject.GameGoldChanged();
            }
        }

        private void ActionOfGamePoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nGamePoint;
            int nOldGamePoint;
            char cMethod;
            nOldGamePoint = PlayObject.m_nGamePoint;
            nGamePoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nGamePoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_GAMEPOINT);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nGamePoint >= 0)
                    {
                        PlayObject.m_nGamePoint = nGamePoint;
                    }
                    break;
                case '-':
                    nGamePoint = HUtil32._MAX(0, PlayObject.m_nGamePoint - nGamePoint);
                    PlayObject.m_nGamePoint = nGamePoint;
                    break;
                case '+':
                    nGamePoint = HUtil32._MAX(0, PlayObject.m_nGamePoint + nGamePoint);
                    PlayObject.m_nGamePoint = nGamePoint;
                    break;
            }
            if (M2Share.g_boGameLogGamePoint)
            {
                M2Share.AddGameDataLog(format(M2Share.g_sGameLogMsg1, new object[] { grobal2.LOG_GAMEPOINT, PlayObject.m_sMapName, PlayObject.m_nCurrX, PlayObject.m_nCurrY, PlayObject.m_sCharName, M2Share.g_Config.sGamePointName, nGamePoint, cMethod, this.m_sCharName }));
            }
            if (nOldGamePoint != PlayObject.m_nGamePoint)
            {
                PlayObject.GameGoldChanged();
            }
        }

        private void ActionOfGetMarry(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TBaseObject PoseBaseObject;
            PoseBaseObject = PlayObject.GetPoseCreate();
            if ((PoseBaseObject != null) && (PoseBaseObject.m_btRaceServer == grobal2.RC_PLAYOBJECT) && (PoseBaseObject.m_btGender != PlayObject.m_btGender))
            {
                PlayObject.m_sDearName = PoseBaseObject.m_sCharName;
                PlayObject.RefShowName();
                PoseBaseObject.RefShowName();
            }
            else
            {
                GotoLable(PlayObject, "@MarryError", false);
            }
        }

        private void ActionOfGetMaster(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TBaseObject PoseBaseObject;
            PoseBaseObject = PlayObject.GetPoseCreate();
            if ((PoseBaseObject != null) && (PoseBaseObject.m_btRaceServer == grobal2.RC_PLAYOBJECT) && (PoseBaseObject.m_btGender != PlayObject.m_btGender))
            {
                PlayObject.m_sMasterName = PoseBaseObject.m_sCharName;
                PlayObject.RefShowName();
                PoseBaseObject.RefShowName();
            }
            else
            {
                GotoLable(PlayObject, "@MasterError", false);
            }
        }

        private void ActionOfLineMsg(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sMsg;
            sMsg = GetLineVariableText(PlayObject, QuestActionInfo.sParam2);
            sMsg = sMsg.Replace("%s", PlayObject.m_sCharName);
            sMsg = sMsg.Replace("%d", this.m_sCharName);
            switch (QuestActionInfo.nParam1)
            {
                case 0:
                    M2Share.UserEngine.SendBroadCastMsg(sMsg, TMsgType.t_System);
                    break;
                case 1:
                    M2Share.UserEngine.SendBroadCastMsg("(*) " + sMsg, TMsgType.t_System);
                    break;
                case 2:
                    M2Share.UserEngine.SendBroadCastMsg('[' + this.m_sCharName + ']' + sMsg, TMsgType.t_System);
                    break;
                case 3:
                    M2Share.UserEngine.SendBroadCastMsg('[' + PlayObject.m_sCharName + ']' + sMsg, TMsgType.t_System);
                    break;
                case 4:
                    this.ProcessSayMsg(sMsg);
                    break;
                case 5:
                    PlayObject.SysMsg(sMsg, TMsgColor.c_Red, TMsgType.t_Say);
                    break;
                case 6:
                    PlayObject.SysMsg(sMsg, TMsgColor.c_Green, TMsgType.t_Say);
                    break;
                case 7:
                    PlayObject.SysMsg(sMsg, TMsgColor.c_Blue, TMsgType.t_Say);
                    break;
                case 8:
                    PlayObject.SendGroupText(sMsg);
                    break;
                case 9:
                    if (PlayObject.m_MyGuild != null)
                    {
                        PlayObject.m_MyGuild.SendGuildMsg(sMsg);
                        M2Share.UserEngine.SendServerGroupMsg(grobal2.SS_208, M2Share.nServerIndex, PlayObject.m_MyGuild.sGuildName + "/" + PlayObject.m_sCharName + "/" + sMsg);
                    }
                    break;
                default:
                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSENDMSG);
                    break;
            }
        }

        private void ActionOfMapTing(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
        }

        private void ActionOfMarry(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TPlayObject PoseHuman;
            string sSayMsg;
            if (PlayObject.m_sDearName != "")
            {
                return;
            }
            PoseHuman = (TPlayObject)PlayObject.GetPoseCreate();
            if (PoseHuman == null)
            {
                GotoLable(PlayObject, "@MarryCheckDir", false);
                return;
            }
            if (QuestActionInfo.sParam1 == "")
            {
                if (PoseHuman.m_btRaceServer != grobal2.RC_PLAYOBJECT)
                {
                    GotoLable(PlayObject, "@HumanTypeErr", false);
                    return;
                }
                if (PoseHuman.GetPoseCreate() == PlayObject)
                {
                    if (PlayObject.m_btGender != PoseHuman.m_btGender)
                    {
                        GotoLable(PlayObject, "@StartMarry", false);
                        GotoLable(PoseHuman, "@StartMarry", false);
                        if ((PlayObject.m_btGender == ObjBase.gMan) && (PoseHuman.m_btGender == ObjBase.gWoMan))
                        {
                            sSayMsg = M2Share.g_sStartMarryManMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                            sSayMsg = M2Share.g_sStartMarryManAskQuestionMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                        }
                        else if ((PlayObject.m_btGender == ObjBase.gWoMan) && (PoseHuman.m_btGender == ObjBase.gMan))
                        {
                            sSayMsg = M2Share.g_sStartMarryWoManMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                            sSayMsg = M2Share.g_sStartMarryWoManAskQuestionMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                        }
                        PlayObject.m_boStartMarry = true;
                        PoseHuman.m_boStartMarry = true;
                    }
                    else
                    {
                        GotoLable(PoseHuman, "@MarrySexErr", false);
                        GotoLable(PlayObject, "@MarrySexErr", false);
                    }
                }
                else
                {
                    GotoLable(PlayObject, "@MarryDirErr", false);
                    GotoLable(PoseHuman, "@MarryCheckDir", false);
                }
                return;
            }
            // sREQUESTMARRY
            if (QuestActionInfo.sParam1.ToLower().CompareTo("REQUESTMARRY".ToLower()) == 0)
            {
                if (PlayObject.m_boStartMarry && PoseHuman.m_boStartMarry)
                {
                    if ((PlayObject.m_btGender == ObjBase.gMan) && (PoseHuman.m_btGender == ObjBase.gWoMan))
                    {
                        sSayMsg = M2Share.g_sMarryManAnswerQuestionMsg.Replace("%n", this.m_sCharName);
                        sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                        sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                        M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                        sSayMsg = M2Share.g_sMarryManAskQuestionMsg.Replace("%n", this.m_sCharName);
                        sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                        sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                        M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                        GotoLable(PlayObject, "@WateMarry", false);
                        GotoLable(PoseHuman, "@RevMarry", false);
                    }
                }
                return;
            }
            // sRESPONSEMARRY
            if (QuestActionInfo.sParam1.ToLower().CompareTo("RESPONSEMARRY".ToLower()) == 0)
            {
                if ((PlayObject.m_btGender == ObjBase.gWoMan) && (PoseHuman.m_btGender == ObjBase.gMan))
                {
                    if (QuestActionInfo.sParam2.ToLower().CompareTo("OK".ToLower()) == 0)
                    {
                        if (PlayObject.m_boStartMarry && PoseHuman.m_boStartMarry)
                        {
                            sSayMsg = M2Share.g_sMarryWoManAnswerQuestionMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                            sSayMsg = M2Share.g_sMarryWoManGetMarryMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                            GotoLable(PlayObject, "@EndMarry", false);
                            GotoLable(PoseHuman, "@EndMarry", false);
                            PlayObject.m_boStartMarry = false;
                            PoseHuman.m_boStartMarry = false;
                            PlayObject.m_sDearName = PoseHuman.m_sCharName;
                            PlayObject.m_DearHuman = PoseHuman;
                            PoseHuman.m_sDearName = PlayObject.m_sCharName;
                            PoseHuman.m_DearHuman = PlayObject;
                            PlayObject.RefShowName();
                            PoseHuman.RefShowName();
                        }
                    }
                    else
                    {
                        if (PlayObject.m_boStartMarry && PoseHuman.m_boStartMarry)
                        {
                            GotoLable(PlayObject, "@EndMarryFail", false);
                            GotoLable(PoseHuman, "@EndMarryFail", false);
                            PlayObject.m_boStartMarry = false;
                            PoseHuman.m_boStartMarry = false;
                            sSayMsg = M2Share.g_sMarryWoManDenyMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                            sSayMsg = M2Share.g_sMarryWoManCancelMsg.Replace("%n", this.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%s", PlayObject.m_sCharName);
                            sSayMsg = sSayMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sSayMsg, TMsgType.t_Say);
                        }
                    }
                }
                return;
            }
        }

        private void ActionOfMaster(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TPlayObject PoseHuman;
            if (PlayObject.m_sMasterName != "")
            {
                return;
            }
            PoseHuman = (TPlayObject)PlayObject.GetPoseCreate();
            if (PoseHuman == null)
            {
                GotoLable(PlayObject, "@MasterCheckDir", false);
                return;
            }
            if (QuestActionInfo.sParam1 == "")
            {
                if (PoseHuman.m_btRaceServer != grobal2.RC_PLAYOBJECT)
                {
                    GotoLable(PlayObject, "@HumanTypeErr", false);
                    return;
                }
                if (PoseHuman.GetPoseCreate() == PlayObject)
                {
                    GotoLable(PlayObject, "@StartGetMaster", false);
                    GotoLable(PoseHuman, "@StartMaster", false);
                    PlayObject.m_boStartMaster = true;
                    PoseHuman.m_boStartMaster = true;
                }
                else
                {
                    GotoLable(PlayObject, "@MasterDirErr", false);
                    GotoLable(PoseHuman, "@MasterCheckDir", false);
                }
                return;
            }
            if (QuestActionInfo.sParam1.ToLower().CompareTo("REQUESTMASTER".ToLower()) == 0)
            {
                if (PlayObject.m_boStartMaster && PoseHuman.m_boStartMaster)
                {
                    PlayObject.m_PoseBaseObject = PoseHuman;
                    PoseHuman.m_PoseBaseObject = PlayObject;
                    GotoLable(PlayObject, "@WateMaster", false);
                    GotoLable(PoseHuman, "@RevMaster", false);
                }
                return;
            }
            if (QuestActionInfo.sParam1.ToLower().CompareTo("RESPONSEMASTER".ToLower()) == 0)
            {
                if (QuestActionInfo.sParam2.ToLower().CompareTo("OK".ToLower()) == 0)
                {
                    if ((PlayObject.m_PoseBaseObject == PoseHuman) && (PoseHuman.m_PoseBaseObject == PlayObject))
                    {
                        if (PlayObject.m_boStartMaster && PoseHuman.m_boStartMaster)
                        {
                            GotoLable(PlayObject, "@EndMaster", false);
                            GotoLable(PoseHuman, "@EndMaster", false);
                            PlayObject.m_boStartMaster = false;
                            PoseHuman.m_boStartMaster = false;
                            if (PlayObject.m_sMasterName == "")
                            {
                                PlayObject.m_sMasterName = PoseHuman.m_sCharName;
                                PlayObject.m_boMaster = true;
                            }
                            PlayObject.m_MasterList.Add(PoseHuman);
                            PoseHuman.m_sMasterName = PlayObject.m_sCharName;
                            PoseHuman.m_boMaster = false;
                            PlayObject.RefShowName();
                            PoseHuman.RefShowName();
                        }
                    }
                }
                else
                {
                    if (PlayObject.m_boStartMaster && PoseHuman.m_boStartMaster)
                    {
                        GotoLable(PlayObject, "@EndMasterFail", false);
                        GotoLable(PoseHuman, "@EndMasterFail", false);
                        PlayObject.m_boStartMaster = false;
                        PoseHuman.m_boStartMaster = false;
                    }
                }
                return;
            }
        }

        private void ActionOfMessageBox(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.SendMsg(this, grobal2.RM_MENU_OK, 0, PlayObject.ObjectId, 0, 0, GetLineVariableText(PlayObject, QuestActionInfo.sParam1));
        }

        private void ActionOfMission(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            if ((QuestActionInfo.sParam1 != "") && (QuestActionInfo.nParam2 > 0) && (QuestActionInfo.nParam3 > 0))
            {
                M2Share.g_sMissionMap = QuestActionInfo.sParam1;
                M2Share.g_nMissionX = (short)QuestActionInfo.nParam2;
                M2Share.g_nMissionY = (short)QuestActionInfo.nParam3;
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_MISSION);
            }
        }

        // MOBFIREBURN MAP X Y TYPE TIME POINT
        private void ActionOfMobFireBurn(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sMap;
            int nX;
            int nY;
            int nType;
            int nTime;
            int nPoint;
            TFireBurnEvent FireBurnEvent;
            TEnvirnoment Envir;
            TEnvirnoment OldEnvir;
            sMap = QuestActionInfo.sParam1;
            nX = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            nY = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            nType = HUtil32.Str_ToInt(QuestActionInfo.sParam4, -1);
            nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam5, -1);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam6, -1);
            if ((sMap == "") || (nX < 0) || (nY < 0) || (nType < 0) || (nTime < 0) || (nPoint < 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_MOBFIREBURN);
                return;
            }
            Envir = M2Share.g_MapManager.FindMap(sMap);
            if (Envir != null)
            {
                OldEnvir = PlayObject.m_PEnvir;
                PlayObject.m_PEnvir = Envir;
                FireBurnEvent = new TFireBurnEvent(PlayObject, nX, nY, nType, nTime * 1000, nPoint);
                M2Share.EventManager.AddEvent(FireBurnEvent);
                PlayObject.m_PEnvir = OldEnvir;
                return;
            }
            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_MOBFIREBURN);
        }

        private void ActionOfMobPlace(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo, int nX, int nY, int nCount, int nRange)
        {
            short nRandX;
            short nRandY;
            TBaseObject Mon;
            for (var I = 0; I < nCount; I++)
            {
                nRandX = (short)(M2Share.RandomNumber.Random(nRange * 2 + 1) + (nX - nRange));
                nRandY = (short)(M2Share.RandomNumber.Random(nRange * 2 + 1) + (nY - nRange));
                Mon = M2Share.UserEngine.RegenMonsterByName(M2Share.g_sMissionMap, nRandX, nRandY, QuestActionInfo.sParam1);
                if (Mon != null)
                {
                    Mon.m_boMission = true;
                    Mon.m_nMissionX = M2Share.g_nMissionX;
                    Mon.m_nMissionY = M2Share.g_nMissionY;
                }
                else
                {
                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_MOBPLACE);
                    break;
                }
            }
        }

        private void ActionOfRecallGroupMembers(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
        }

        private void ActionOfSetRankLevelName(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sRankLevelName;
            sRankLevelName = QuestActionInfo.sParam1;
            if (sRankLevelName == "")
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SKILLLEVEL);
                return;
            }
            PlayObject.m_sRankLevelName = sRankLevelName;
            PlayObject.RefShowName();
        }

        private void ActionOfSetScriptFlag(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            bool boFlag;
            int nWhere;
            nWhere = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            boFlag = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1) == 1;
            switch (nWhere)
            {
                case 0:
                    PlayObject.m_boSendMsgFlag = boFlag;
                    break;
                case 1:
                    PlayObject.m_boChangeItemNameFlag = boFlag;
                    break;
                default:
                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SETSCRIPTFLAG);
                    break;
            }
        }

        private void ActionOfSkillLevel(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            TMagic Magic;
            TUserMagic UserMagic;
            int nLevel;
            char cMethod;
            nLevel = HUtil32.Str_ToInt(QuestActionInfo.sParam3, 0);
            if (nLevel < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SKILLLEVEL);
                return;
            }
            cMethod = QuestActionInfo.sParam2[1];
            Magic = M2Share.UserEngine.FindMagic(QuestActionInfo.sParam1);
            if (Magic != null)
            {
                for (I = 0; I < PlayObject.m_MagicList.Count; I++)
                {
                    UserMagic = PlayObject.m_MagicList[I];
                    if (UserMagic.MagicInfo == Magic)
                    {
                        switch (cMethod)
                        {
                            case '=':
                                if (nLevel >= 0)
                                {
                                    nLevel = HUtil32._MAX(3, nLevel);
                                    UserMagic.btLevel = (byte)nLevel;
                                }
                                break;
                            case '-':
                                if (UserMagic.btLevel >= nLevel)
                                {
                                    UserMagic.btLevel -= (byte)nLevel;
                                }
                                else
                                {
                                    UserMagic.btLevel = 0;
                                }
                                break;
                            case '+':
                                if (UserMagic.btLevel + nLevel <= 3)
                                {
                                    UserMagic.btLevel += (byte)nLevel;
                                }
                                else
                                {
                                    UserMagic.btLevel = 3;
                                }
                                break;
                        }
                        PlayObject.SendDelayMsg(PlayObject, grobal2.RM_MAGIC_LVEXP, 0, UserMagic.MagicInfo.wMagicID, UserMagic.btLevel, UserMagic.nTranPoint, "", 100);
                        break;
                    }
                }
            }
        }

        private void ActionOfTakeCastleGold(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nGold;
            nGold = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if ((nGold < 0) || (this.m_Castle == null))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_TAKECASTLEGOLD);
                return;
            }
            if (nGold <= this.m_Castle.m_nTotalGold)
            {
                this.m_Castle.m_nTotalGold -= nGold;
            }
            else
            {
                this.m_Castle.m_nTotalGold = 0;
            }
        }

        private void ActionOfUnMarry(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TPlayObject PoseHuman;
            if (PlayObject.m_sDearName == "")
            {
                GotoLable(PlayObject, "@ExeMarryFail", false);
                return;
            }
            PoseHuman = (TPlayObject)PlayObject.GetPoseCreate();
            if (PoseHuman == null)
            {
                GotoLable(PlayObject, "@UnMarryCheckDir", false);
            }
            if (PoseHuman != null)
            {
                if (QuestActionInfo.sParam1 == "")
                {
                    if (PoseHuman.m_btRaceServer != grobal2.RC_PLAYOBJECT)
                    {
                        GotoLable(PlayObject, "@UnMarryTypeErr", false);
                        return;
                    }
                    if (PoseHuman.GetPoseCreate() == PlayObject)
                    {
                        // and (PosHum.AddInfo.sDearName = Hum.sName)
                        if (PlayObject.m_sDearName == PoseHuman.m_sCharName)
                        {
                            GotoLable(PlayObject, "@StartUnMarry", false);
                            GotoLable(PoseHuman, "@StartUnMarry", false);
                            return;
                        }
                    }
                }
            }
            // sREQUESTUNMARRY
            if (QuestActionInfo.sParam1.ToLower().CompareTo("REQUESTUNMARRY".ToLower()) == 0)
            {
                if (QuestActionInfo.sParam2 == "")
                {
                    if (PoseHuman != null)
                    {
                        PlayObject.m_boStartUnMarry = true;
                        if (PlayObject.m_boStartUnMarry && PoseHuman.m_boStartUnMarry)
                        {
                            // sUnMarryMsg8
                            // sMarryMsg0
                            // sUnMarryMsg9
                            M2Share.UserEngine.SendBroadCastMsg('[' + this.m_sCharName + "]: " + "我宣布" + PoseHuman.m_sCharName + ' ' + '与' + PlayObject.m_sCharName + ' ' + ' ' + "正式脱离夫妻关系。", TMsgType.t_Say);
                            PlayObject.m_sDearName = "";
                            PoseHuman.m_sDearName = "";
                            PlayObject.m_btMarryCount++;
                            PoseHuman.m_btMarryCount++;
                            PlayObject.m_boStartUnMarry = false;
                            PoseHuman.m_boStartUnMarry = false;
                            PlayObject.RefShowName();
                            PoseHuman.RefShowName();
                            GotoLable(PlayObject, "@UnMarryEnd", false);
                            GotoLable(PoseHuman, "@UnMarryEnd", false);
                        }
                        else
                        {
                            GotoLable(PlayObject, "@WateUnMarry", false);
                            // GotoLable(PoseHuman,'@RevUnMarry',False);
                        }
                    }
                    return;
                }
                else
                {
                    // 强行离婚
                    if (QuestActionInfo.sParam2.ToLower().CompareTo("FORCE".ToLower()) == 0)
                    {
                        M2Share.UserEngine.SendBroadCastMsg('[' + this.m_sCharName + "]: " + "我宣布" + PlayObject.m_sCharName + ' ' + '与' + PlayObject.m_sDearName + ' ' + ' ' + "已经正式脱离夫妻关系！！！", TMsgType.t_Say);
                        PoseHuman = M2Share.UserEngine.GetPlayObject(PlayObject.m_sDearName);
                        if (PoseHuman != null)
                        {
                            PoseHuman.m_sDearName = "";
                            PoseHuman.m_btMarryCount++;
                            PoseHuman.RefShowName();
                        }
                        else
                        {
                            //sUnMarryFileName = M2Share.g_Config.sEnvirDir + "UnMarry.txt";
                            //LoadList = new StringList();
                            //if (File.Exists(sUnMarryFileName))
                            //{
                            //    LoadList.LoadFromFile(sUnMarryFileName);
                            //}
                            //LoadList.Add(PlayObject.m_sDearName);
                            //LoadList.SaveToFile(sUnMarryFileName);
                            //LoadList.Free;
                        }
                        PlayObject.m_sDearName = "";
                        PlayObject.m_btMarryCount++;
                        GotoLable(PlayObject, "@UnMarryEnd", false);
                        PlayObject.RefShowName();
                    }
                    return;
                }
            }
        }

        public virtual void ClearScript()
        {
            //for (I = 0; I < m_ScriptList.Count; I ++ )
            //{
            //    Script = m_ScriptList[I];
            //    for (II = 0; II < Script.RecordList.Count; II ++ )
            //    {
            //        SayingRecord = Script.RecordList[II];
            //        for (III = 0; III < SayingRecord.ProcedureList.Count; III ++ )
            //        {
            //            SayingProcedure = SayingRecord.ProcedureList[III];
            //            for (IIII = 0; IIII < SayingProcedure.ConditionList.Count; IIII ++ )
            //            {
            //                Dispose(((SayingProcedure.ConditionList[IIII]) as TQuestConditionInfo));
            //            }
            //            for (IIII = 0; IIII < SayingProcedure.ActionList.Count; IIII ++ )
            //            {
            //                Dispose(((SayingProcedure.ActionList[IIII]) as TQuestActionInfo));
            //            }
            //            for (IIII = 0; IIII < SayingProcedure.ElseActionList.Count; IIII ++ )
            //            {
            //                Dispose(((SayingProcedure.ElseActionList[IIII]) as TQuestActionInfo));
            //            }
            //            //SayingProcedure.ConditionList.Free;
            //            //SayingProcedure.ActionList.Free;
            //            //SayingProcedure.ElseActionList.Free;
            //            Dispose(SayingProcedure);
            //        }
            //        //SayingRecord.ProcedureList.Free;
            //        Dispose(SayingRecord);
            //    }
            //    //Script.RecordList.Free;
            //    Dispose(Script);
            //}
            m_ScriptList.Clear();
        }

        public virtual void Click(TPlayObject PlayObject)
        {
            PlayObject.m_nScriptGotoCount = 0;
            PlayObject.m_sScriptGoBackLable = "";
            PlayObject.m_sScriptCurrLable = "";
            GotoLable(PlayObject, "@main", false);
        }

        private bool ConditionOfCheckAccountIPList(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            StringList LoadList;
            string sCharName;
            string sCharAccount;
            string sCharIPaddr;
            string sLine;
            string sName = string.Empty;
            string sIPaddr;
            result = false;
            try
            {
                sCharName = PlayObject.m_sCharName;
                sCharAccount = PlayObject.m_sUserID;
                sCharIPaddr = PlayObject.m_sIPaddr;
                LoadList = new StringList();
                if (File.Exists(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1))
                {

                    LoadList.LoadFromFile(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1);
                    for (I = 0; I < LoadList.Count; I++)
                    {
                        sLine = LoadList[I];
                        if (sLine[1] == ';')
                        {
                            continue;
                        }
                        sIPaddr = HUtil32.GetValidStr3(sLine, ref sName, new string[] { " ", "/", "\t" });
                        sIPaddr = sIPaddr.Trim();
                        if ((sName == sCharAccount) && (sIPaddr == sCharIPaddr))
                        {
                            result = true;
                            break;
                        }
                    }
                }
                else
                {
                    ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKACCOUNTIPLIST);
                }
            }
            finally
            {
                //LoadList.Free;
            }
            return result;
        }

        private bool ConditionOfCheckBagSize(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nSize;
            result = false;
            nSize = QuestConditionInfo.nParam1;
            if ((nSize <= 0) || (nSize > grobal2.MAXBAGITEM))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKBAGSIZE);
                return result;
            }
            if (PlayObject.m_ItemList.Count + nSize <= grobal2.MAXBAGITEM)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckBonusPoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nTotlePoint;
            char cMethod;
            result = false;
            nTotlePoint = this.m_BonusAbil.DC + this.m_BonusAbil.MC + this.m_BonusAbil.SC + this.m_BonusAbil.AC + this.m_BonusAbil.MAC + this.m_BonusAbil.HP + this.m_BonusAbil.MP + this.m_BonusAbil.Hit + this.m_BonusAbil.Speed + this.m_BonusAbil.X2;
            nTotlePoint = nTotlePoint + this.m_nBonusPoint;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nTotlePoint == QuestConditionInfo.nParam2)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nTotlePoint > QuestConditionInfo.nParam2)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nTotlePoint < QuestConditionInfo.nParam2)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nTotlePoint >= QuestConditionInfo.nParam2)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        public bool ConditionOfCheckHP_CheckHigh(TPlayObject PlayObject, char cMethodMax, int nMax)
        {
            bool result;
            result = false;
            switch (cMethodMax)
            {
                case '=':
                    if (PlayObject.m_WAbil.MaxHP == nMax)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_WAbil.MaxHP > nMax)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_WAbil.MaxHP < nMax)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_WAbil.MaxHP >= nMax)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckHP(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethodMin;
            char cMethodMax;
            int nMin;
            int nMax;
            result = false;
            cMethodMin = QuestConditionInfo.sParam1[1];
            cMethodMax = QuestConditionInfo.sParam1[3];
            nMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nMax = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((nMin < 0) || (nMax < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKHP);
                return result;
            }
            switch (cMethodMin)
            {
                case '=':
                    if (this.m_WAbil.HP == nMin)
                    {
                        result = ConditionOfCheckHP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '>':
                    if (PlayObject.m_WAbil.HP > nMin)
                    {
                        result = ConditionOfCheckHP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '<':
                    if (PlayObject.m_WAbil.HP < nMin)
                    {
                        result = ConditionOfCheckHP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                default:
                    if (PlayObject.m_WAbil.HP >= nMin)
                    {
                        result = ConditionOfCheckHP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
            }
            return result;
        }

        public bool ConditionOfCheckMP_CheckHigh(TPlayObject PlayObject, char cMethodMax, int nMax)
        {
            bool result;
            result = false;
            switch (cMethodMax)
            {
                case '=':
                    if (PlayObject.m_WAbil.MaxMP == nMax)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_WAbil.MaxMP > nMax)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_WAbil.MaxMP < nMax)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_WAbil.MaxMP >= nMax)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMP(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethodMin;
            char cMethodMax;
            int nMin;
            int nMax;
            result = false;
            cMethodMin = QuestConditionInfo.sParam1[1];
            cMethodMax = QuestConditionInfo.sParam1[3];
            nMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nMax = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((nMin < 0) || (nMax < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMP);
                return result;
            }
            switch (cMethodMin)
            {
                case '=':
                    if (this.m_WAbil.MP == nMin)
                    {
                        result = ConditionOfCheckMP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '>':
                    if (PlayObject.m_WAbil.MP > nMin)
                    {
                        result = ConditionOfCheckMP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '<':
                    if (PlayObject.m_WAbil.MP < nMin)
                    {
                        result = ConditionOfCheckMP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                default:
                    if (PlayObject.m_WAbil.MP >= nMin)
                    {
                        result = ConditionOfCheckMP_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
            }
            return result;
        }

        public bool ConditionOfCheckDC_CheckHigh(TPlayObject PlayObject, char cMethodMax, int nMax)
        {
            bool result;
            result = false;
            switch (cMethodMax)
            {
                case '=':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.DC) == nMax)
                    {
                        result = true;
                    }
                    break;
                case '>':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.DC) > nMax)
                    {
                        result = true;
                    }
                    break;
                case '<':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.DC) < nMax)
                    {
                        result = true;
                    }
                    break;
                default:

                    if (HUtil32.HiWord(PlayObject.m_WAbil.DC) >= nMax)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckDC(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethodMin;
            char cMethodMax;
            int nMin;
            int nMax;
            result = false;
            cMethodMin = QuestConditionInfo.sParam1[1];
            cMethodMax = QuestConditionInfo.sParam1[3];
            nMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nMax = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((nMin < 0) || (nMax < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKDC);
                return result;
            }
            switch (cMethodMin)
            {
                case '=':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.DC) == nMin)
                    {
                        result = ConditionOfCheckDC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '>':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.DC) > nMin)
                    {
                        result = ConditionOfCheckDC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '<':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.DC) < nMin)
                    {
                        result = ConditionOfCheckDC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                default:

                    if (HUtil32.LoWord(PlayObject.m_WAbil.DC) >= nMin)
                    {
                        result = ConditionOfCheckDC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
            }
            result = false;
            return result;
        }

        public bool ConditionOfCheckMC_CheckHigh(TPlayObject PlayObject, char cMethodMax, int nMax)
        {
            bool result;
            result = false;
            switch (cMethodMax)
            {
                case '=':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.MC) == nMax)
                    {
                        result = true;
                    }
                    break;
                case '>':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.MC) > nMax)
                    {
                        result = true;
                    }
                    break;
                case '<':

                    if (HUtil32.HiWord(PlayObject.m_WAbil.MC) < nMax)
                    {
                        result = true;
                    }
                    break;
                default:

                    if (HUtil32.HiWord(PlayObject.m_WAbil.MC) >= nMax)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMC(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethodMin;
            char cMethodMax;
            int nMin;
            int nMax;
            result = false;
            cMethodMin = QuestConditionInfo.sParam1[1];
            cMethodMax = QuestConditionInfo.sParam1[3];
            nMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nMax = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((nMin < 0) || (nMax < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMC);
                return result;
            }
            switch (cMethodMin)
            {
                case '=':
                    if (HUtil32.LoWord(PlayObject.m_WAbil.MC) == nMin)
                    {
                        result = ConditionOfCheckMC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '>':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.MC) > nMin)
                    {
                        result = ConditionOfCheckMC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '<':
                    if (HUtil32.LoWord(PlayObject.m_WAbil.MC) < nMin)
                    {
                        result = ConditionOfCheckMC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                default:
                    if (HUtil32.LoWord(PlayObject.m_WAbil.MC) >= nMin)
                    {
                        result = ConditionOfCheckMC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
            }
            return result;
        }

        public bool ConditionOfCheckSC_CheckHigh(TPlayObject PlayObject, char cMethodMax, int nMax)
        {
            bool result;
            result = false;
            switch (cMethodMax)
            {
                case '=':
                    if (HUtil32.HiWord(PlayObject.m_WAbil.SC) == nMax)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (HUtil32.HiWord(PlayObject.m_WAbil.SC) > nMax)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (HUtil32.HiWord(PlayObject.m_WAbil.SC) < nMax)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (HUtil32.HiWord(PlayObject.m_WAbil.SC) >= nMax)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckSC(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethodMin;
            char cMethodMax;
            int nMin;
            int nMax;
            result = false;
            cMethodMin = QuestConditionInfo.sParam1[1];
            cMethodMax = QuestConditionInfo.sParam1[3];
            nMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nMax = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((nMin < 0) || (nMax < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKSC);
                return result;
            }
            switch (cMethodMin)
            {
                case '=':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.SC) == nMin)
                    {
                        result = ConditionOfCheckSC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '>':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.SC) > nMin)
                    {
                        result = ConditionOfCheckSC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                case '<':

                    if (HUtil32.LoWord(PlayObject.m_WAbil.SC) < nMin)
                    {
                        result = ConditionOfCheckSC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
                default:

                    if (HUtil32.LoWord(PlayObject.m_WAbil.SC) >= nMin)
                    {
                        result = ConditionOfCheckSC_CheckHigh(PlayObject, cMethodMax, nMax);
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckExp(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int dwExp;
            result = false;
            dwExp = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, 0);
            if (dwExp == 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKEXP);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_Abil.Exp == dwExp)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_Abil.Exp > dwExp)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_Abil.Exp < dwExp)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_Abil.Exp >= dwExp)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckFlourishPoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nPoint;
            TGuild Guild;
            result = false;
            nPoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nPoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKFLOURISHPOINT);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (Guild.nFlourishing == nPoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (Guild.nFlourishing > nPoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (Guild.nFlourishing < nPoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (Guild.nFlourishing >= nPoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckChiefItemCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nCount;
            TGuild Guild;
            result = false;
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKFLOURISHPOINT);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (Guild.nChiefItemCount == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (Guild.nChiefItemCount > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (Guild.nChiefItemCount < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (Guild.nChiefItemCount >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckGuildAuraePoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nPoint;
            TGuild Guild;
            result = false;
            nPoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nPoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKAURAEPOINT);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (Guild.nAurae == nPoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (Guild.nAurae > nPoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (Guild.nAurae < nPoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (Guild.nAurae >= nPoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckGuildBuildPoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nPoint;
            TGuild Guild;
            result = false;
            nPoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nPoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKBUILDPOINT);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (Guild.nBuildPoint == nPoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (Guild.nBuildPoint > nPoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (Guild.nBuildPoint < nPoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (Guild.nBuildPoint >= nPoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckStabilityPoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nPoint;
            TGuild Guild;
            result = false;
            nPoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nPoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKSTABILITYPOINT);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (Guild.nStability == nPoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (Guild.nStability > nPoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (Guild.nStability < nPoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (Guild.nStability >= nPoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckGameGold(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nGameGold;
            result = false;
            nGameGold = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nGameGold < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKGAMEGOLD);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_nGameGold == nGameGold)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_nGameGold > nGameGold)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_nGameGold < nGameGold)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_nGameGold >= nGameGold)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckGamePoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nGamePoint;
            result = false;
            nGamePoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nGamePoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKGAMEPOINT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_nGamePoint == nGamePoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_nGamePoint > nGamePoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_nGamePoint < nGamePoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_nGamePoint >= nGamePoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckGroupCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nCount;
            result = false;
            if (PlayObject.m_GroupOwner == null)
            {
                return result;
            }
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKGROUPCOUNT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_GroupOwner.m_GroupMembers.Count == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_GroupOwner.m_GroupMembers.Count > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_GroupOwner.m_GroupMembers.Count < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_GroupOwner.m_GroupMembers.Count >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfIsHigh(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMode;
            result = false;
            if (QuestConditionInfo.sParam1 == "")
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISHIGH);
                return result;
            }
            cMode = QuestConditionInfo.sParam1[1];
            switch (cMode)
            {
                case 'L':
                    result = M2Share.g_HighLevelHuman == PlayObject;
                    break;
                case 'P':
                    result = M2Share.g_HighPKPointHuman == PlayObject;
                    break;
                case 'D':
                    result = M2Share.g_HighDCHuman == PlayObject;
                    break;
                case 'M':
                    result = M2Share.g_HighMCHuman == PlayObject;
                    break;
                case 'S':
                    result = M2Share.g_HighSCHuman == PlayObject;
                    break;
                default:
                    ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISHIGH);
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckHaveGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            // Result:=PlayObject.m_MyGuild = nil;
            result = PlayObject.m_MyGuild != null;
            // 01-16 更正检查结果反了

            return result;
        }

        private bool ConditionOfCheckInMapRange(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            string sMapName;
            int nX;
            int nY;
            int nRange;
            result = false;
            sMapName = QuestConditionInfo.sParam1;
            nX = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nY = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            nRange = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            if ((sMapName == "") || (nX < 0) || (nY < 0) || (nRange < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKINMAPRANGE);
                return result;
            }
            if (PlayObject.m_sMapName.ToLower().CompareTo(sMapName.ToLower()) != 0)
            {
                return result;
            }
            if ((Math.Abs(PlayObject.m_nCurrX - nX) <= nRange) && (Math.Abs(PlayObject.m_nCurrY - nY) <= nRange))
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckIsAttackGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (this.m_Castle == null)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISATTACKGUILD);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            result = this.m_Castle.IsAttackGuild(PlayObject.m_MyGuild);
            return result;
        }

        private bool ConditionOfCheckCastleChageDay(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nDay;
            char cMethod;
            int nChangeDay;
            result = false;
            nDay = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if ((nDay < 0) || (this.m_Castle == null))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CASTLECHANGEDAY);
                return result;
            }
            nChangeDay = HUtil32.GetDayCount(DateTime.Now, this.m_Castle.m_ChangeDate);
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nChangeDay == nDay)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nChangeDay > nDay)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nChangeDay < nDay)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nChangeDay >= nDay)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckCastleWarDay(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nDay;
            char cMethod;
            int nWarDay;
            result = false;
            nDay = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if ((nDay < 0) || (this.m_Castle == null))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CASTLEWARDAY);
                return result;
            }
            nWarDay = HUtil32.GetDayCount(DateTime.Now, this.m_Castle.m_WarDate);
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nWarDay == nDay)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nWarDay > nDay)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nWarDay < nDay)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nWarDay >= nDay)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckCastleDoorStatus(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nDay;
            int nDoorStatus;
            TCastleDoor CastleDoor;
            result = false;
            nDay = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            nDoorStatus = -1;
            if (QuestConditionInfo.sParam1.ToLower().CompareTo("损坏".ToLower()) == 0)
            {
                nDoorStatus = 0;
            }
            if (QuestConditionInfo.sParam1.ToLower().CompareTo("开启".ToLower()) == 0)
            {
                nDoorStatus = 1;
            }
            if (QuestConditionInfo.sParam1.ToLower().CompareTo("关闭".ToLower()) == 0)
            {
                nDoorStatus = 2;
            }
            if ((nDay < 0) || (this.m_Castle == null) || (nDoorStatus < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKCASTLEDOOR);
                return result;
            }
            CastleDoor = (TCastleDoor)this.m_Castle.m_MainDoor.BaseObject;
            switch (nDoorStatus)
            {
                case 0:
                    if (CastleDoor.m_boDeath)
                    {
                        result = true;
                    }
                    break;
                case 1:
                    if (CastleDoor.m_boOpened)
                    {
                        result = true;
                    }
                    break;
                case 2:
                    if (!CastleDoor.m_boOpened)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckIsAttackAllyGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (this.m_Castle == null)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISATTACKALLYGUILD);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            result = this.m_Castle.IsAttackAllyGuild(PlayObject.m_MyGuild);
            return result;
        }

        private bool ConditionOfCheckIsDefenseAllyGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (this.m_Castle == null)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISDEFENSEALLYGUILD);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            result = this.m_Castle.IsDefenseAllyGuild(PlayObject.m_MyGuild);
            return result;
        }

        private bool ConditionOfCheckIsDefenseGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (this.m_Castle == null)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ISDEFENSEGUILD);
                return result;
            }
            if (PlayObject.m_MyGuild == null)
            {
                return result;
            }
            result = this.m_Castle.IsDefenseGuild(PlayObject.m_MyGuild);
            return result;
        }

        private bool ConditionOfCheckIsCastleaGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            // if (PlayObject.m_MyGuild <> nil) and (UserCastle.m_MasterGuild = PlayObject.m_MyGuild) then
            if (M2Share.CastleManager.IsCastleMember(PlayObject) != null)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckIsCastleMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            // if PlayObject.IsGuildMaster and (UserCastle.m_MasterGuild = PlayObject.m_MyGuild) then
            if (PlayObject.IsGuildMaster() && (M2Share.CastleManager.IsCastleMember(PlayObject) != null))
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckIsGuildMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = PlayObject.IsGuildMaster();
            return result;
        }

        private bool ConditionOfCheckIsMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if ((PlayObject.m_sMasterName != "") && PlayObject.m_boMaster)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckListCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result = false;
            return result;
        }

        private bool ConditionOfCheckItemAddValue(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            int nWhere;
            int nAddAllValue;
            int nAddValue;
            TUserItem UserItem;
            char cMethod;
            result = false;
            nWhere = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, -1);
            cMethod = QuestConditionInfo.sParam2[1];
            nAddValue = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            if (!(nWhere >= PlayObject.m_UseItems.GetLowerBound(0) && nWhere <= PlayObject.m_UseItems.GetUpperBound(0)) || (nAddValue < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKITEMADDVALUE);
                return result;
            }
            UserItem = PlayObject.m_UseItems[nWhere];
            if (UserItem.wIndex == 0)
            {
                return result;
            }
            nAddAllValue = 0;
            for (I = UserItem.btValue.GetLowerBound(0); I <= UserItem.btValue.GetUpperBound(0); I++)
            {
                nAddAllValue += UserItem.btValue[I];
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nAddAllValue == nAddValue)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nAddAllValue > nAddValue)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nAddAllValue < nAddValue)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nAddAllValue >= nAddValue)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckItemType(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nWhere;
            int nType;
            TUserItem UserItem;
            TItem Stditem;
            result = false;
            nWhere = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, -1);
            nType = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (!(nWhere >= PlayObject.m_UseItems.GetLowerBound(0) && nWhere <= PlayObject.m_UseItems.GetUpperBound(0)))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKITEMTYPE);
                return result;
            }
            UserItem = PlayObject.m_UseItems[nWhere];
            if (UserItem.wIndex == 0)
            {
                return result;
            }
            Stditem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
            if ((Stditem != null) && (Stditem.StdMode == nType))
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckLevelEx(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nLevel;
            char cMethod;
            result = false;
            nLevel = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKLEVELEX);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_Abil.Level == nLevel)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_Abil.Level > nLevel)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_Abil.Level < nLevel)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_Abil.Level >= nLevel)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckNameListPostion(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            StringList LoadList;
            string sCharName;
            int nNamePostion;
            int nPostion;
            string sLine;
            result = false;
            nNamePostion = -1;
            try
            {
                sCharName = PlayObject.m_sCharName;
                LoadList = new StringList();
                if (File.Exists(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1))
                {

                    LoadList.LoadFromFile(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1);
                    for (I = 0; I < LoadList.Count; I++)
                    {
                        sLine = LoadList[I].Trim();
                        if (sLine[1] == ';')
                        {
                            continue;
                        }
                        if (sLine.ToLower().CompareTo(sCharName.ToLower()) == 0)
                        {
                            nNamePostion = I;
                            break;
                        }
                    }
                }
                else
                {
                    ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKNAMELISTPOSITION);
                }
            }
            finally
            {

                //LoadList.Free;
            }
            nPostion = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nPostion < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKNAMELISTPOSITION);
                return result;
            }
            if (nNamePostion >= nPostion)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckMarry(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (PlayObject.m_sDearName != "")
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckMarryCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nCount;
            char cMethod;
            result = false;
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMARRYCOUNT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_btMarryCount == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_btMarryCount > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_btMarryCount < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_btMarryCount >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if ((PlayObject.m_sMasterName != "") && (!PlayObject.m_boMaster))
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckMemBerLevel(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nLevel;
            char cMethod;
            result = false;
            nLevel = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMEMBERLEVEL);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_nMemberLevel == nLevel)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_nMemberLevel > nLevel)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_nMemberLevel < nLevel)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_nMemberLevel >= nLevel)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMemberType(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nType;
            char cMethod;
            result = false;
            nType = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nType < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMEMBERTYPE);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_nMemberType == nType)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_nMemberType > nType)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_nMemberType < nType)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_nMemberType >= nType)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckNameIPList(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            StringList LoadList;
            string sCharName;
            string sCharAccount;
            string sCharIPaddr;
            string sLine;
            string sName = string.Empty;
            string sIPaddr;
            result = false;
            try
            {
                sCharName = PlayObject.m_sCharName;
                sCharAccount = PlayObject.m_sUserID;
                sCharIPaddr = PlayObject.m_sIPaddr;
                LoadList = new StringList();
                if (File.Exists(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1))
                {

                    LoadList.LoadFromFile(M2Share.g_Config.sEnvirDir + QuestConditionInfo.sParam1);
                    for (I = 0; I < LoadList.Count; I++)
                    {
                        sLine = LoadList[I];
                        if (sLine[1] == ';')
                        {
                            continue;
                        }
                        sIPaddr = HUtil32.GetValidStr3(sLine, ref sName, new string[] { " ", "/", "\t" });
                        sIPaddr = sIPaddr.Trim();
                        if ((sName == sCharName) && (sIPaddr == sCharIPaddr))
                        {
                            result = true;
                            break;
                        }
                    }
                }
                else
                {
                    ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKNAMEIPLIST);
                }
            }
            finally
            {
                //LoadList.Free;
            }
            return result;
        }

        private bool ConditionOfCheckPoseDir(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            result = false;
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.GetPoseCreate() == PlayObject) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                switch (QuestConditionInfo.nParam1)
                {
                    case 1:
                        if (PoseHuman.m_btGender == PlayObject.m_btGender)
                        {
                            result = true;
                        }
                        break;
                    case 2:
                        // 要求相同性别
                        if (PoseHuman.m_btGender != PlayObject.m_btGender)
                        {
                            result = true;
                        }
                        break;
                    default:
                        // 要求不同性别
                        result = true;
                        break;
                        // 无参数时不判别性别
                }
            }
            return result;
        }

        private bool ConditionOfCheckPoseGender(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            byte btSex;
            result = false;
            btSex = 0;
            if (QuestConditionInfo.sParam1.ToLower().CompareTo("MAN".ToLower()) == 0)
            {
                btSex = 0;
            }
            else if (QuestConditionInfo.sParam1.ToLower().CompareTo("男".ToLower()) == 0)
            {
                btSex = 0;
            }
            else if (QuestConditionInfo.sParam1.ToLower().CompareTo("WOMAN".ToLower()) == 0)
            {
                btSex = 1;
            }
            else if (QuestConditionInfo.sParam1.ToLower().CompareTo("女".ToLower()) == 0)
            {
                btSex = 1;
            }
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                if (PoseHuman.m_btGender == btSex)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ConditionOfCheckPoseIsMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            result = false;
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                if ((((TPlayObject)PoseHuman).m_sMasterName != "") && ((TPlayObject)PoseHuman).m_boMaster)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ConditionOfCheckPoseLevel(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nLevel;
            TBaseObject PoseHuman;
            char cMethod;
            result = false;
            nLevel = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKPOSELEVEL);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                switch (cMethod)
                {
                    case '=':
                        if (PoseHuman.m_Abil.Level == nLevel)
                        {
                            result = true;
                        }
                        break;
                    case '>':
                        if (PoseHuman.m_Abil.Level > nLevel)
                        {
                            result = true;
                        }
                        break;
                    case '<':
                        if (PoseHuman.m_Abil.Level < nLevel)
                        {
                            result = true;
                        }
                        break;
                    default:
                        if (PoseHuman.m_Abil.Level >= nLevel)
                        {
                            result = true;
                        }
                        break;
                }
            }
            return result;
        }

        private bool ConditionOfCheckPoseMarry(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            result = false;
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                if (((TPlayObject)PoseHuman).m_sDearName != "")
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ConditionOfCheckPoseMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            result = false;
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                if ((((TPlayObject)PoseHuman).m_sMasterName != "") && !((TPlayObject)PoseHuman).m_boMaster)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ConditionOfCheckServerName(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (QuestConditionInfo.sParam1 == M2Share.g_Config.sServerName)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckSlaveCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nCount;
            char cMethod;
            result = false;
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKSLAVECOUNT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_SlaveList.Count == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_SlaveList.Count > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_SlaveList.Count < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_SlaveList.Count >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMap(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            if (QuestConditionInfo.sParam1 == PlayObject.m_sMapName)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private bool ConditionOfCheckPos(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nX;
            int nY;
            nX = QuestConditionInfo.nParam2;
            nY = QuestConditionInfo.nParam3;
            if ((QuestConditionInfo.sParam1 == PlayObject.m_sMapName) && (nX == PlayObject.m_nCurrX) && (nY == PlayObject.m_nCurrY))
            {
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private bool ConditionOfReviveSlave(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result = false;
            int I;
            int resultc;
            string s18;
            System.IO.FileInfo myFile;
            StringList LoadList;
            string sFileName = string.Empty;
            string SLineText = string.Empty;
            string Petname = string.Empty;
            string lvl = string.Empty;
            string lvlexp = string.Empty;
            resultc = -1;
            sFileName = M2Share.g_Config.sEnvirDir + "PetData\\" + PlayObject.m_sCharName + ".txt";
            if (File.Exists(sFileName))
            {
                LoadList = new StringList();
                // Templist:=TStringList.Create;

                LoadList.LoadFromFile(sFileName);
                if (PlayObject.m_btJob == M2Share.jTaos)
                {
                }
                else
                {
                }
                for (I = 0; I < LoadList.Count; I++)
                {
                    s18 = LoadList[I].Trim();
                    if ((s18 != "") && (s18[1] != ';'))
                    {
                        s18 = HUtil32.GetValidStr3(s18, ref Petname, "/");
                        s18 = HUtil32.GetValidStr3(s18, ref lvl, "/");
                        s18 = HUtil32.GetValidStr3(s18, ref lvlexp, "/");
                        // PlayObject.ReviveSlave(PetName,str_ToInt(lvl,0),str_ToInt(lvlexp,0),nslavecount,10 * 24 * 60 * 60);
                        resultc = I;
                    }
                }
                if (LoadList.Count > 0)
                {
                    result = true;
                    myFile = new FileInfo(sFileName);
                    StreamWriter _W_0 = myFile.CreateText();
                    _W_0.Close();
                }
            }
            return result;
        }

        private bool ConditionOfCheckMagicLvl(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            TUserMagic UserMagic;
            result = false;
            for (I = 0; I < PlayObject.m_MagicList.Count; I++)
            {
                UserMagic = PlayObject.m_MagicList[I];
                if (UserMagic.MagicInfo.sMagicName.ToLower().CompareTo(QuestConditionInfo.sParam1.ToLower()) == 0)
                {
                    if (UserMagic.btLevel == QuestConditionInfo.nParam2)
                    {
                        result = true;
                    }
                    break;
                }
            }
            return result;
        }

        private bool ConditionOfCheckGroupClass(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            int nCount;
            int nJob;
            char cMethod;
            TPlayObject PlayObjectEx;
            result = false;
            nJob = -1;
            nCount = 0;
            if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sWarrior, M2Share.sWarrior.Length))
            {
                nJob = M2Share.jWarr;
            }
            if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sWizard, M2Share.sWizard.Length))
            {
                nJob = M2Share.jWizard;
            }
            if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sTaos, M2Share.sTaos.Length))
            {
                nJob = M2Share.jTaos;
            }
            if (nJob < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHANGEJOB);
                return result;
            }
            if (PlayObject.m_GroupOwner != null)
            {
                for (I = 0; I < PlayObject.m_GroupMembers.Count; I++)
                {

                    PlayObjectEx = PlayObject.m_GroupMembers[I];
                    if (PlayObjectEx.m_btJob == nJob)
                    {
                        nCount++;
                    }
                }
            }
            cMethod = QuestConditionInfo.sParam2[1];
            switch (cMethod)
            {
                case '=':
                    if (nCount == QuestConditionInfo.nParam3)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nCount > QuestConditionInfo.nParam3)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nCount < QuestConditionInfo.nParam3)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nCount >= QuestConditionInfo.nParam3)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        public TNormNpc() : base()
        {
            this.m_boSuperMan = true;
            this.m_btRaceServer = grobal2.RC_NPC;
            this.m_nLight = 2;
            this.m_btAntiPoison = 99;
            this.m_ScriptList = new List<TScript>();
            this.m_boStickMode = true;
            this.m_sFilePath = "";
            this.m_boIsHide = false;
            this.m_boIsQuest = true;
            this.FGotoLable = new int[100];
        }

        ~TNormNpc()
        {
            ClearScript();
            // 
            // for I := 0 to ScriptList.Count - 1 do begin
            // Dispose(pTScript(ScriptList.Items[I]));
            // end;
            // 

            //m_ScriptList.Free;
            // base.Destroy();
        }
        private void ExeAction(TPlayObject PlayObject, string sParam1, string sParam2, string sParam3, int nParam1, int nParam2, int nParam3)
        {
            int nInt1;
            int dwInt;
            // ================================================
            // 更改人物当前经验值
            // EXEACTION CHANGEEXP 0 经验数  设置为指定经验值
            // EXEACTION CHANGEEXP 1 经验数  增加指定经验
            // EXEACTION CHANGEEXP 2 经验数  减少指定经验
            // ================================================
            if (sParam1.ToLower().CompareTo("CHANGEEXP".ToLower()) == 0)
            {
                nInt1 = HUtil32.Str_ToInt(sParam2, -1);
                switch (nInt1)
                {
                    case 0:
                        if (nParam3 >= 0)
                        {
                            PlayObject.m_Abil.Exp = nParam3;
                            PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        }
                        break;
                    case 1:
                        if (PlayObject.m_Abil.Exp >= nParam3)
                        {
                            if ((PlayObject.m_Abil.Exp - nParam3) > (int.MaxValue - PlayObject.m_Abil.Exp))
                            {
                                dwInt = int.MaxValue - PlayObject.m_Abil.Exp;
                            }
                            else
                            {
                                dwInt = nParam3;
                            }
                        }
                        else
                        {
                            if ((nParam3 - PlayObject.m_Abil.Exp) > (int.MaxValue - nParam3))
                            {
                                dwInt = int.MaxValue - nParam3;
                            }
                            else
                            {
                                dwInt = nParam3;
                            }
                        }
                        PlayObject.m_Abil.Exp += dwInt;
                        PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        break;
                    case 2:
                        if (PlayObject.m_Abil.Exp > nParam3)
                        {
                            PlayObject.m_Abil.Exp -= nParam3;
                        }
                        else
                        {
                            PlayObject.m_Abil.Exp = 0;
                        }
                        PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        break;
                }
                PlayObject.SysMsg("您当前经验点数为: " + PlayObject.m_Abil.Exp.ToString() + '/' + PlayObject.m_Abil.MaxExp.ToString(), TMsgColor.c_Green, TMsgType.t_Hint);
                return;
            }
            // ================================================
            // 更改人物当前等级
            // EXEACTION CHANGELEVEL 0 等级数  设置为指定等级
            // EXEACTION CHANGELEVEL 1 等级数  增加指定等级
            // EXEACTION CHANGELEVEL 2 等级数  减少指定等级
            // ================================================
            if (sParam1.ToLower().CompareTo("CHANGELEVEL".ToLower()) == 0)
            {
                nInt1 = HUtil32.Str_ToInt(sParam2, -1);
                switch (nInt1)
                {
                    case 0:
                        if (nParam3 >= 0)
                        {
                            PlayObject.m_Abil.Level = (ushort)nParam3;
                            PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        }
                        break;
                    case 1:
                        if (PlayObject.m_Abil.Level >= nParam3)
                        {
                            if ((PlayObject.m_Abil.Level - nParam3) > (short.MaxValue - PlayObject.m_Abil.Level))
                            {
                                dwInt = short.MaxValue - PlayObject.m_Abil.Level;
                            }
                            else
                            {
                                dwInt = nParam3;
                            }
                        }
                        else
                        {
                            if ((nParam3 - PlayObject.m_Abil.Level) > (int.MaxValue - nParam3))
                            {
                                dwInt = int.MaxValue - nParam3;
                            }
                            else
                            {
                                dwInt = nParam3;
                            }
                        }
                        PlayObject.m_Abil.Level += (ushort)dwInt;
                        PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        break;
                    case 2:
                        if (PlayObject.m_Abil.Level > nParam3)
                        {
                            PlayObject.m_Abil.Level -= (ushort)nParam3;
                        }
                        else
                        {
                            PlayObject.m_Abil.Level = 0;
                        }
                        PlayObject.HasLevelUp(PlayObject.m_Abil.Level - 1);
                        break;
                }
                PlayObject.SysMsg("您当前等级为: " + PlayObject.m_Abil.Level.ToString(), TMsgColor.c_Green, TMsgType.t_Hint);
                return;
            }
            // ================================================
            // 杀死人物
            // EXEACTION KILL 0 人物死亡,不显示凶手信息
            // EXEACTION KILL 1 人物死亡不掉物品,不显示凶手信息
            // EXEACTION KILL 2 人物死亡,显示凶手信息为NPC
            // EXEACTION KILL 3 人物死亡不掉物品,显示凶手信息为NPC
            // ================================================
            if (sParam1.ToLower().CompareTo("KILL".ToLower()) == 0)
            {
                nInt1 = HUtil32.Str_ToInt(sParam2, -1);
                switch (nInt1)
                {
                    case 1:
                        PlayObject.m_boNoItem = true;
                        PlayObject.Die();
                        break;
                    case 2:
                        PlayObject.SetLastHiter(this);
                        PlayObject.Die();
                        break;
                    case 3:
                        PlayObject.m_boNoItem = true;
                        PlayObject.SetLastHiter(this);
                        PlayObject.Die();
                        break;
                    default:
                        PlayObject.Die();
                        break;
                }
                return;
            }
            // ================================================
            // 踢人物下线
            // EXEACTION KICK
            // ================================================
            if (sParam1.ToLower().CompareTo("KICK".ToLower()) == 0)
            {
                PlayObject.m_boKickFlag = true;
                return;
            }
            // ==============================================================================

        }

        // FFE9
        public string GetLineVariableText(TPlayObject PlayObject, string sMsg)
        {
            string result;
            int nC;
            string s10 = string.Empty;
            nC = 0;
            while (true)
            {
                if (HUtil32.TagCount(sMsg, '>') < 1)
                {
                    break;
                }
                HUtil32.ArrestStringEx(sMsg, '<', '>', ref s10);
                GetVariableText(PlayObject, ref sMsg, s10);
                nC++;
                if (nC >= 101)
                {
                    break;
                }
            }
            result = sMsg;
            return result;
        }

        // FFEA
        public virtual void GetVariableText(TPlayObject PlayObject, ref string sMsg, string sVariable)
        {
            string sText = string.Empty;
            string s14 = string.Empty;
            int I;
            int n18;
            TDynamicVar DynamicVar;
            bool boFoundVar;
            // 全局信息
            if (sVariable == "$SERVERNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$SERVERNAME>", M2Share.g_Config.sServerName);
                return;
            }
            if (sVariable == "$SERVERIP")
            {
                sMsg = sub_49ADB8(sMsg, "<$SERVERIP>", M2Share.g_Config.sServerIPaddr);
                return;
            }
            if (sVariable == "$WEBSITE")
            {
                sMsg = sub_49ADB8(sMsg, "<$WEBSITE>", M2Share.g_Config.sWebSite);
                return;
            }
            if (sVariable == "$BBSSITE")
            {
                sMsg = sub_49ADB8(sMsg, "<$BBSSITE>", M2Share.g_Config.sBbsSite);
                return;
            }
            if (sVariable == "$CLIENTDOWNLOAD")
            {
                sMsg = sub_49ADB8(sMsg, "<$CLIENTDOWNLOAD>", M2Share.g_Config.sClientDownload);
                return;
            }
            if (sVariable == "$QQ")
            {
                sMsg = sub_49ADB8(sMsg, "<$QQ>", M2Share.g_Config.sQQ);
                return;
            }
            if (sVariable == "$PHONE")
            {
                sMsg = sub_49ADB8(sMsg, "<$PHONE>", M2Share.g_Config.sPhone);
                return;
            }
            if (sVariable == "$BANKACCOUNT0")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT0>", M2Share.g_Config.sBankAccount0);
                return;
            }
            if (sVariable == "$BANKACCOUNT1")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT1>", M2Share.g_Config.sBankAccount1);
                return;
            }
            if (sVariable == "$BANKACCOUNT2")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT2>", M2Share.g_Config.sBankAccount2);
                return;
            }
            if (sVariable == "$BANKACCOUNT3")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT3>", M2Share.g_Config.sBankAccount3);
                return;
            }
            if (sVariable == "$BANKACCOUNT4")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT4>", M2Share.g_Config.sBankAccount4);
                return;
            }
            if (sVariable == "$BANKACCOUNT5")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT5>", M2Share.g_Config.sBankAccount5);
                return;
            }
            if (sVariable == "$BANKACCOUNT6")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT6>", M2Share.g_Config.sBankAccount6);
                return;
            }
            if (sVariable == "$BANKACCOUNT7")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT7>", M2Share.g_Config.sBankAccount7);
                return;
            }
            if (sVariable == "$BANKACCOUNT8")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT8>", M2Share.g_Config.sBankAccount8);
                return;
            }
            if (sVariable == "$BANKACCOUNT9")
            {
                sMsg = sub_49ADB8(sMsg, "<$BANKACCOUNT9>", M2Share.g_Config.sBankAccount9);
                return;
            }
            if (sVariable == "$GAMEGOLDNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$GAMEGOLDNAME>", M2Share.g_Config.sGameGoldName);
                return;
            }
            if (sVariable == "$GAMEPOINTNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$GAMEPOINTNAME>", M2Share.g_Config.sGamePointName);
                return;
            }
            if (sVariable == "$USERCOUNT")
            {
                sText = M2Share.UserEngine.PlayObjectCount.ToString();
                sMsg = sub_49ADB8(sMsg, "<$USERCOUNT>", sText);
                return;
            }
            if (sVariable == "$MACRUNTIME")
            {

                sText = (HUtil32.GetTickCount() / (24 * 60 * 60 * 1000)).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MACRUNTIME>", sText);
                return;
            }
            if (sVariable == "$SERVERRUNTIME")
            {
                //nSecond = (HUtil32.GetTickCount() - M2Share.g_dwStartTick) / 1000;
                //wHour = nSecond / 3600;
                //wMinute = (nSecond / 60) % 60;
                //wSecond = nSecond % 60;
                //sText = format("%d:%d:%d", new short[] {wHour, wMinute, wSecond});
                sMsg = sub_49ADB8(sMsg, "<$SERVERRUNTIME>", sText);
                return;
            }
            if (sVariable == "$DATETIME")
            {
                // sText:=DateTimeToStr(Now);
                sText = DateTime.Now.ToString("dddddd,dddd,hh:mm:nn");
                sMsg = sub_49ADB8(sMsg, "<$DATETIME>", sText);
                return;
            }
            if (sVariable == "$HIGHLEVELINFO")
            {
                if (M2Share.g_HighLevelHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighLevelHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHLEVELINFO>", sText);
                return;
            }
            if (sVariable == "$HIGHPKINFO")
            {
                if (M2Share.g_HighPKPointHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighPKPointHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHPKINFO>", sText);
                return;
            }
            if (sVariable == "$HIGHDCINFO")
            {
                if (M2Share.g_HighDCHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighDCHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHDCINFO>", sText);
                return;
            }
            if (sVariable == "$HIGHMCINFO")
            {
                if (M2Share.g_HighMCHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighMCHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHMCINFO>", sText);
                return;
            }
            if (sVariable == "$HIGHSCINFO")
            {
                if (M2Share.g_HighSCHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighSCHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHSCINFO>", sText);
                return;
            }
            if (sVariable == "$HIGHONLINEINFO")
            {
                if (M2Share.g_HighOnlineHuman != null)
                {
                    sText = ((TPlayObject)M2Share.g_HighOnlineHuman).GetMyInfo();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$HIGHONLINEINFO>", sText);
                return;
            }
            // 个人信息
            if (sVariable == "$RANDOMNO")
            {
                sMsg = sub_49ADB8(sMsg, "<$RANDOMNO>", PlayObject.m_sRandomNo);
                return;
            }
            if (sVariable == "$RELEVEL")
            {
                sText = PlayObject.m_btReLevel.ToString();
                sMsg = sub_49ADB8(sMsg, "<$RELEVEL>", sText);
                return;
            }
            if (sVariable == "$HUMANSHOWNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$HUMANSHOWNAME>", PlayObject.GetShowName());
                return;
            }
            if (sVariable == "$MONKILLER")
            {
                if (PlayObject.m_LastHiter != null)
                {
                    if (PlayObject.m_LastHiter.m_btRaceServer != grobal2.RC_PLAYOBJECT)
                    {
                        sMsg = sub_49ADB8(sMsg, "<$MONKILLER>", PlayObject.m_LastHiter.m_sCharName);
                    }
                }
                else
                {
                    sMsg = sub_49ADB8(sMsg, "<$MONKILLER>", "未知");
                }
                return;
            }
            if (sVariable == "$KILLER")
            {
                if (PlayObject.m_LastHiter != null)
                {
                    if (PlayObject.m_LastHiter.m_btRaceServer == grobal2.RC_PLAYOBJECT)
                    {
                        sMsg = sub_49ADB8(sMsg, "<$KILLER>", PlayObject.m_LastHiter.m_sCharName);
                    }
                }
                else
                {
                    sMsg = sub_49ADB8(sMsg, "<$KILLER>", "未知");
                }
                return;
            }
            if (sVariable == "$USERNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$USERNAME>", PlayObject.m_sCharName);
                return;
            }
            if (sVariable == "$GUILDNAME")
            {
                if (PlayObject.m_MyGuild != null)
                {
                    sMsg = sub_49ADB8(sMsg, "<$GUILDNAME>", PlayObject.m_MyGuild.sGuildName);
                }
                else
                {
                    sMsg = "无";
                }
                return;
            }
            if (sVariable == "$RANKNAME")
            {
                sMsg = sub_49ADB8(sMsg, "<$RANKNAME>", PlayObject.m_sGuildRankName);
                return;
            }
            if (sVariable == "$LEVEL")
            {
                sText = PlayObject.m_Abil.Level.ToString();
                sMsg = sub_49ADB8(sMsg, "<$LEVEL>", sText);
                return;
            }
            if (sVariable == "$HP")
            {
                sText = PlayObject.m_WAbil.HP.ToString();
                sMsg = sub_49ADB8(sMsg, "<$HP>", sText);
                return;
            }
            if (sVariable == "$MAXHP")
            {
                sText = PlayObject.m_WAbil.MaxHP.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXHP>", sText);
                return;
            }
            if (sVariable == "$MP")
            {
                sText = PlayObject.m_WAbil.MP.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MP>", sText);
                return;
            }
            if (sVariable == "$MAXMP")
            {
                sText = PlayObject.m_WAbil.MaxMP.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXMP>", sText);
                return;
            }
            if (sVariable == "$AC")
            {

                sText = HUtil32.LoWord(PlayObject.m_WAbil.AC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$AC>", sText);
                return;
            }
            if (sVariable == "$MAXAC")
            {

                sText = HUtil32.HiWord(PlayObject.m_WAbil.AC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXAC>", sText);
                return;
            }
            if (sVariable == "$MAC")
            {

                sText = HUtil32.LoWord(PlayObject.m_WAbil.MAC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAC>", sText);
                return;
            }
            if (sVariable == "$MAXMAC")
            {

                sText = HUtil32.HiWord(PlayObject.m_WAbil.MAC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXMAC>", sText);
                return;
            }
            if (sVariable == "$DC")
            {

                sText = HUtil32.LoWord(PlayObject.m_WAbil.DC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$DC>", sText);
                return;
            }
            if (sVariable == "$MAXDC")
            {

                sText = HUtil32.HiWord(PlayObject.m_WAbil.DC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXDC>", sText);
                return;
            }
            if (sVariable == "$MC")
            {

                sText = HUtil32.LoWord(PlayObject.m_WAbil.MC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MC>", sText);
                return;
            }
            if (sVariable == "$MAXMC")
            {

                sText = HUtil32.HiWord(PlayObject.m_WAbil.MC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXMC>", sText);
                return;
            }
            if (sVariable == "$SC")
            {

                sText = HUtil32.LoWord(PlayObject.m_WAbil.SC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$SC>", sText);
                return;
            }
            if (sVariable == "$MAXSC")
            {

                sText = HUtil32.HiWord(PlayObject.m_WAbil.SC).ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXSC>", sText);
                return;
            }
            if (sVariable == "$EXP")
            {
                sText = PlayObject.m_Abil.Exp.ToString();
                sMsg = sub_49ADB8(sMsg, "<$EXP>", sText);
                return;
            }
            if (sVariable == "$MAXEXP")
            {
                sText = PlayObject.m_Abil.MaxExp.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXEXP>", sText);
                return;
            }
            if (sVariable == "$PKPOINT")
            {
                sText = PlayObject.m_nPkPoint.ToString();
                sMsg = sub_49ADB8(sMsg, "<$PKPOINT>", sText);
                return;
            }
            if (sVariable == "$CREDITPOINT")
            {
                sText = PlayObject.m_btCreditPoint.ToString();
                sMsg = sub_49ADB8(sMsg, "<$CREDITPOINT>", sText);
                return;
            }
            if (sVariable == "$HW")
            {
                sText = PlayObject.m_WAbil.HandWeight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$HW>", sText);
                return;
            }
            if (sVariable == "$MAXHW")
            {
                sText = PlayObject.m_WAbil.MaxHandWeight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXHW>", sText);
                return;
            }
            if (sVariable == "$BW")
            {
                sText = PlayObject.m_WAbil.Weight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$BW>", sText);
                return;
            }
            if (sVariable == "$MAXBW")
            {
                sText = PlayObject.m_WAbil.MaxWeight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXBW>", sText);
                return;
            }
            if (sVariable == "$WW")
            {
                sText = PlayObject.m_WAbil.WearWeight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$WW>", sText);
                return;
            }
            if (sVariable == "$MAXWW")
            {
                sText = PlayObject.m_WAbil.MaxWearWeight.ToString();
                sMsg = sub_49ADB8(sMsg, "<$MAXWW>", sText);
                return;
            }
            if (sVariable == "$GOLDCOUNT")
            {
                sText = PlayObject.m_nGold.ToString() + '/' + PlayObject.m_nGoldMax.ToString();
                sMsg = sub_49ADB8(sMsg, "<$GOLDCOUNT>", sText);
                return;
            }
            if (sVariable == "$GAMEGOLD")
            {
                sText = PlayObject.m_nGameGold.ToString();
                sMsg = sub_49ADB8(sMsg, "<$GAMEGOLD>", sText);
                return;
            }
            if (sVariable == "$GAMEPOINT")
            {
                sText = PlayObject.m_nGamePoint.ToString();
                sMsg = sub_49ADB8(sMsg, "<$GAMEPOINT>", sText);
                return;
            }
            if (sVariable == "$HUNGER")
            {
                sText = PlayObject.GetMyStatus().ToString();
                sMsg = sub_49ADB8(sMsg, "<$HUNGER>", sText);
                return;
            }
            if (sVariable == "$LOGINTIME")
            {
                sText = PlayObject.m_dLogonTime.ToString();
                sMsg = sub_49ADB8(sMsg, "<$LOGINTIME>", sText);
                return;
            }
            if (sVariable == "$LOGINLONG")
            {
                sText = ((HUtil32.GetTickCount() - PlayObject.m_dwLogonTick) / 60000).ToString() + "分钟";
                sMsg = sub_49ADB8(sMsg, "<$LOGINLONG>", sText);
                return;
            }
            if (sVariable == "$DRESS")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_DRESS].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$DRESS>", sText);
                return;
            }
            else if (sVariable == "$WEAPON")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_WEAPON].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$WEAPON>", sText);
                return;
            }
            else if (sVariable == "$RIGHTHAND")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_RIGHTHAND].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$RIGHTHAND>", sText);
                return;
            }
            else if (sVariable == "$HELMET")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_HELMET].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$HELMET>", sText);
                return;
            }
            else if (sVariable == "$NECKLACE")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_NECKLACE].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$NECKLACE>", sText);
                return;
            }
            else if (sVariable == "$RING_R")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_RINGR].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$RING_R>", sText);
                return;
            }
            else if (sVariable == "$RING_L")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_RINGL].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$RING_L>", sText);
                return;
            }
            else if (sVariable == "$ARMRING_R")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_ARMRINGR].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$ARMRING_R>", sText);
                return;
            }
            else if (sVariable == "$ARMRING_L")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_ARMRINGL].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$ARMRING_L>", sText);
                return;
            }
            else if (sVariable == "$BUJUK")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BUJUK].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$BUJUK>", sText);
                return;
            }
            else if (sVariable == "$BELT")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BELT].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$BELT>", sText);
                return;
            }
            else if (sVariable == "$BOOTS")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BOOTS].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$BOOTS>", sText);
                return;
            }
            else if (sVariable == "$CHARM")
            {
                sText = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_CHARM].wIndex);
                sMsg = sub_49ADB8(sMsg, "<$CHARM>", sText);
                return;
            }
            else if (sVariable == "$IPADDR")
            {
                sText = PlayObject.m_sIPaddr;
                sMsg = sub_49ADB8(sMsg, "<$IPADDR>", sText);
                return;
            }
            else if (sVariable == "$IPLOCAL")
            {
                sText = PlayObject.m_sIPLocal;
                // GetIPLocal(PlayObject.m_sIPaddr);
                sMsg = sub_49ADB8(sMsg, "<$IPLOCAL>", sText);
                return;
            }
            else if (sVariable == "$GUILDBUILDPOINT")
            {
                if (PlayObject.m_MyGuild == null)
                {
                    sText = "无";
                }
                else
                {
                    sText = PlayObject.m_MyGuild.nBuildPoint.ToString();
                }
                sMsg = sub_49ADB8(sMsg, "<$GUILDBUILDPOINT>", sText);
                return;
            }
            else if (sVariable == "$GUILDAURAEPOINT")
            {
                if (PlayObject.m_MyGuild == null)
                {
                    sText = "无";
                }
                else
                {
                    sText = PlayObject.m_MyGuild.nAurae.ToString();
                }
                sMsg = sub_49ADB8(sMsg, "<$GUILDAURAEPOINT>", sText);
                return;
            }
            else if (sVariable == "$GUILDSTABILITYPOINT")
            {
                if (PlayObject.m_MyGuild == null)
                {
                    sText = "无";
                }
                else
                {
                    sText = PlayObject.m_MyGuild.nStability.ToString();
                }
                sMsg = sub_49ADB8(sMsg, "<$GUILDSTABILITYPOINT>", sText);
                return;
            }
            if (sVariable == "$GUILDFLOURISHPOINT")
            {
                if (PlayObject.m_MyGuild == null)
                {
                    sText = "无";
                }
                else
                {
                    sText = PlayObject.m_MyGuild.nFlourishing.ToString();
                }
                sMsg = sub_49ADB8(sMsg, "<$GUILDFLOURISHPOINT>", sText);
                return;
            }
            // 其它信息
            if (sVariable == "$REQUESTCASTLEWARITEM")
            {
                sText = M2Share.g_Config.sZumaPiece;
                sMsg = sub_49ADB8(sMsg, "<$REQUESTCASTLEWARITEM>", sText);
                return;
            }
            if (sVariable == "$REQUESTCASTLEWARDAY")
            {
                sText = M2Share.g_Config.sZumaPiece;
                sMsg = sub_49ADB8(sMsg, "<$REQUESTCASTLEWARDAY>", sText);
                return;
            }
            if (sVariable == "$REQUESTBUILDGUILDITEM")
            {
                sText = M2Share.g_Config.sWomaHorn;
                sMsg = sub_49ADB8(sMsg, "<$REQUESTBUILDGUILDITEM>", sText);
                return;
            }
            if (sVariable == "$OWNERGUILD")
            {
                if (this.m_Castle != null)
                {
                    sText = this.m_Castle.m_sOwnGuild;
                    if (sText == "")
                    {
                        sText = "游戏管理";
                    }
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$OWNERGUILD>", sText);
                return;
            }
            // 0049AF32
            if (sVariable == "$CASTLENAME")
            {
                if (this.m_Castle != null)
                {
                    sText = this.m_Castle.m_sName;
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$CASTLENAME>", sText);
                return;
            }
            if (sVariable == "$LORD")
            {
                if (this.m_Castle != null)
                {
                    if (this.m_Castle.m_MasterGuild != null)
                    {
                        sText = this.m_Castle.m_MasterGuild.GetChiefName();
                    }
                    else
                    {
                        sText = "管理员";
                    }
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$LORD>", sText);
                return;
            }
            // 0049AF32
            if (sVariable == "$GUILDWARFEE")
            {
                sMsg = sub_49ADB8(sMsg, "<$GUILDWARFEE>", M2Share.g_Config.nGuildWarPrice.ToString());
                return;
            }
            if (sVariable == "$BUILDGUILDFEE")
            {
                sMsg = sub_49ADB8(sMsg, "<$BUILDGUILDFEE>", M2Share.g_Config.nBuildGuildPrice.ToString());
                return;
            }
            if (sVariable == "$CASTLEWARDATE")
            {
                if (this.m_Castle == null)
                {
                    this.m_Castle = M2Share.CastleManager.GetCastle(0);
                }
                if (this.m_Castle != null)
                {
                    if (!this.m_Castle.m_boUnderWar)
                    {
                        sText = this.m_Castle.GetWarDate();
                        if (sText != "")
                        {
                            sMsg = sub_49ADB8(sMsg, "<$CASTLEWARDATE>", sText);
                        }
                        else
                        {
                            sMsg = "Well I guess there may be no wall conquest war in the mean time .\\ \\<back/@main>";
                        }
                    }
                    else
                    {
                        sMsg = "Now is on wall conquest war.\\ \\<back/@main>";
                    }
                }
                else
                {
                    sText = "????";
                }
                return;
            }
            if (sVariable == "$LISTOFWAR")
            {
                if (this.m_Castle != null)
                {
                    sText = this.m_Castle.GetAttackWarList();
                }
                else
                {
                    sText = "????";
                }
                if (sText != "")
                {
                    sMsg = sub_49ADB8(sMsg, "<$LISTOFWAR>", sText);
                }
                else
                {
                    sMsg = "We have no schedule...\\ \\<back/@main>";
                }
                return;
            }
            if (sVariable == "$CASTLECHANGEDATE")
            {
                if (this.m_Castle != null)
                {
                    sText = this.m_Castle.m_ChangeDate.ToString();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$CASTLECHANGEDATE>", sText);
                return;
            }
            if (sVariable == "$CASTLEWARLASTDATE")
            {
                if (this.m_Castle != null)
                {
                    sText = this.m_Castle.m_WarDate.ToString();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$CASTLEWARLASTDATE>", sText);
                return;
            }
            if (sVariable == "$CASTLEGETDAYS")
            {
                if (this.m_Castle != null)
                {
                    sText = HUtil32.GetDayCount(DateTime.Now, this.m_Castle.m_ChangeDate).ToString();
                }
                else
                {
                    sText = "????";
                }
                sMsg = sub_49ADB8(sMsg, "<$CASTLEGETDAYS>", sText);
                return;
            }
            if (sVariable == "$CMD_DATE")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_DATE>", M2Share.g_GameCommand.DATA.sCmd);
                return;
            }
            if (sVariable == "$CMD_ALLOWMSG")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_ALLOWMSG>", M2Share.g_GameCommand.ALLOWMSG.sCmd);
                return;
            }
            if (sVariable == "$CMD_LETSHOUT")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_LETSHOUT>", M2Share.g_GameCommand.LETSHOUT.sCmd);
                return;
            }
            if (sVariable == "$CMD_LETTRADE")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_LETTRADE>", M2Share.g_GameCommand.LETTRADE.sCmd);
                return;
            }
            if (sVariable == "$CMD_LETGUILD")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_LETGUILD>", M2Share.g_GameCommand.LETGUILD.sCmd);
                return;
            }
            if (sVariable == "$CMD_ENDGUILD")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_ENDGUILD>", M2Share.g_GameCommand.ENDGUILD.sCmd);
                return;
            }
            if (sVariable == "$CMD_BANGUILDCHAT")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_BANGUILDCHAT>", M2Share.g_GameCommand.BANGUILDCHAT.sCmd);
                return;
            }
            if (sVariable == "$CMD_AUTHALLY")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_AUTHALLY>", M2Share.g_GameCommand.AUTHALLY.sCmd);
                return;
            }
            if (sVariable == "$CMD_AUTH")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_AUTH>", M2Share.g_GameCommand.AUTH.sCmd);
                return;
            }
            if (sVariable == "$CMD_AUTHCANCEL")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_AUTHCANCEL>", M2Share.g_GameCommand.AUTHCANCEL.sCmd);
                return;
            }
            if (sVariable == "$CMD_USERMOVE")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_USERMOVE>", M2Share.g_GameCommand.USERMOVE.sCmd);
                return;
            }
            if (sVariable == "$CMD_SEARCHING")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_SEARCHING>", M2Share.g_GameCommand.SEARCHING.sCmd);
                return;
            }
            if (sVariable == "$CMD_ALLOWGROUPCALL")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_ALLOWGROUPCALL>", M2Share.g_GameCommand.ALLOWGROUPCALL.sCmd);
                return;
            }
            if (sVariable == "$CMD_GROUPRECALLL")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_GROUPRECALLL>", M2Share.g_GameCommand.GROUPRECALLL.sCmd);
                return;
            }
            if (sVariable == "$CMD_ATTACKMODE")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_ATTACKMODE>", M2Share.g_GameCommand.ATTACKMODE.sCmd);
                return;
            }
            if (sVariable == "$CMD_REST")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_REST>", M2Share.g_GameCommand.REST.sCmd);
                return;
            }
            if (sVariable == "$CMD_STORAGESETPASSWORD")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_STORAGESETPASSWORD>", M2Share.g_GameCommand.SETPASSWORD.sCmd);
                return;
            }
            if (sVariable == "$CMD_STORAGECHGPASSWORD")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_STORAGECHGPASSWORD>", M2Share.g_GameCommand.CHGPASSWORD.sCmd);
                return;
            }
            if (sVariable == "$CMD_STORAGELOCK")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_STORAGELOCK>", M2Share.g_GameCommand.__LOCK.sCmd);
                return;
            }
            if (sVariable == "$CMD_STORAGEUNLOCK")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_STORAGEUNLOCK>", M2Share.g_GameCommand.UNLOCKSTORAGE.sCmd);
                return;
            }
            if (sVariable == "$CMD_UNLOCK")
            {
                sMsg = sub_49ADB8(sMsg, "<$CMD_UNLOCK>", M2Share.g_GameCommand.UNLOCK.sCmd);
                return;
            }
            if (HUtil32.CompareLStr(sVariable, "$HUMAN(", "$HUMAN(".Length))
            {
                HUtil32.ArrestStringEx(sVariable, '(', ')', ref s14);
                boFoundVar = false;
                for (I = 0; I < PlayObject.m_DynamicVarList.Count; I++)
                {
                    DynamicVar = PlayObject.m_DynamicVarList[I];
                    if (DynamicVar.sName.ToLower().CompareTo(s14.ToLower()) == 0)
                    {
                        switch (DynamicVar.VarType)
                        {
                            case TVarType.VInteger:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.nInternet.ToString());
                                boFoundVar = true;
                                break;
                            case TVarType.VString:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.sString);
                                boFoundVar = true;
                                break;
                        }
                        break;
                    }
                }
                if (!boFoundVar)
                {
                    sMsg = "??";
                }
                return;
            }
            if (HUtil32.CompareLStr(sVariable, "$GUILD(", "$GUILD(".Length))
            {
                if (PlayObject.m_MyGuild == null)
                {
                    return;
                }
                HUtil32.ArrestStringEx(sVariable, '(', ')', ref s14);
                boFoundVar = false;
                for (I = 0; I < PlayObject.m_MyGuild.m_DynamicVarList.Count; I++)
                {
                    DynamicVar = PlayObject.m_MyGuild.m_DynamicVarList[I];
                    if (DynamicVar.sName.ToLower().CompareTo(s14.ToLower()) == 0)
                    {
                        switch (DynamicVar.VarType)
                        {
                            case TVarType.VInteger:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.nInternet.ToString());
                                boFoundVar = true;
                                break;
                            case TVarType.VString:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.sString);
                                boFoundVar = true;
                                break;
                        }
                        break;
                    }
                }
                if (!boFoundVar)
                {
                    sMsg = "??";
                }
                return;
            }
            if (HUtil32.CompareLStr(sVariable, "$GLOBAL(", "$GLOBAL(".Length))
            {
                HUtil32.ArrestStringEx(sVariable, '(', ')', ref s14);
                boFoundVar = false;
                for (I = 0; I < M2Share.g_DynamicVarList.Count; I++)
                {
                    DynamicVar = M2Share.g_DynamicVarList[I];
                    if (DynamicVar.sName.ToLower().CompareTo(s14.ToLower()) == 0)
                    {
                        switch (DynamicVar.VarType)
                        {
                            case TVarType.VInteger:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.nInternet.ToString());
                                boFoundVar = true;
                                break;
                            case TVarType.VString:
                                sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', DynamicVar.sString);
                                boFoundVar = true;
                                break;
                        }
                        break;
                    }
                }
                if (!boFoundVar)
                {
                    sMsg = "??";
                }
                return;
            }
            if (HUtil32.CompareLStr(sVariable, "$STR(", "$STR(".Length))
            {
                HUtil32.ArrestStringEx(sVariable, '(', ')', ref s14);
                n18 = M2Share.GetValNameNo(s14);
                if (n18 >= 0)
                {
                    switch (n18)
                    {
                        // Modify the A .. B: 0 .. 9
                        case 0:
                            sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', PlayObject.m_nVal[n18].ToString());
                            break;
                        // Modify the A .. B: 100 .. 119
                        case 100:
                            sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', M2Share.g_Config.GlobalVal[n18 - 100].ToString());
                            break;
                        // Modify the A .. B: 200 .. 209
                        case 200:
                            sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', PlayObject.m_DyVal[n18 - 200].ToString());
                            break;
                        // Modify the A .. B: 300 .. 399
                        case 300:
                            sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', PlayObject.m_nMval[n18 - 300].ToString());
                            break;
                        // Modify the A .. B: 400 .. 499
                        case 400:
                            sMsg = sub_49ADB8(sMsg, '<' + sVariable + '>', M2Share.g_Config.GlobaDyMval[n18 - 400].ToString());
                            break;
                    }
                }
            }
        }

        public bool GotoLable_CheckQuestStatus(TPlayObject PlayObject, TScript ScriptInfo)
        {
            bool result = true;
            int I;
            if (!ScriptInfo.boQuest)
            {
                return result;
            }
            I = 0;
            while (true)
            {
                if ((ScriptInfo.QuestInfo[I].nRandRage > 0) && (M2Share.RandomNumber.Random(ScriptInfo.QuestInfo[I].nRandRage) != 0))
                {
                    result = false;
                    break;
                }
                if (PlayObject.GetQuestFalgStatus(ScriptInfo.QuestInfo[I].wFlag) != ScriptInfo.QuestInfo[I].btValue)
                {
                    result = false;
                    break;
                }
                I++;
                if (I >= 10)
                {
                    break;
                }
            }
            // while

            return result;
        }

        public TUserItem GotoLable_CheckItemW(TPlayObject PlayObject, string sItemType, int nParam)
        {
            TUserItem result = null;
            int nCount = 0;
            if (HUtil32.CompareLStr(sItemType, "[NECKLACE]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_NECKLACE].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_NECKLACE];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[RING]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_RINGL].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_RINGL];
                }
                if (PlayObject.m_UseItems[grobal2.U_RINGR].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_RINGR];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[ARMRING]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_ARMRINGL].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_ARMRINGL];
                }
                if (PlayObject.m_UseItems[grobal2.U_ARMRINGR].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_ARMRINGR];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[WEAPON]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_WEAPON].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_WEAPON];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[HELMET]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_HELMET].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_HELMET];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[BUJUK]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BUJUK].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_BUJUK];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[BELT]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BELT].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_BELT];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[BOOTS]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BOOTS].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_BOOTS];
                }
                return result;
            }
            if (HUtil32.CompareLStr(sItemType, "[CHARM]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_CHARM].wIndex > 0)
                {
                    result = PlayObject.m_UseItems[grobal2.U_CHARM];
                }
                return result;
            }
            result = PlayObject.sub_4C4CD4(sItemType, ref nCount);
            if (nCount < nParam)
            {
                result = null;
            }
            return result;
        }

        public bool GotoLable_CheckStringList(string sHumName, string sListFileName)
        {
            bool result;
            StringList LoadList;
            result = false;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            if (File.Exists(sListFileName))
            {
                LoadList = new StringList();
                try
                {
                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
                for (var i = 0; i < LoadList.Count; i++)
                {
                    if (LoadList[i].Trim().ToLower().CompareTo(sHumName.ToLower()) == 0)
                    {
                        result = true;
                        break;
                    }
                }
                //LoadList.Free;
            }
            else
            {
                M2Share.MainOutMessage("file not found => " + sListFileName);
            }
            return result;
        }

        public void GotoLable_QuestCheckCondition_SetVal(TPlayObject PlayObject, string sIndex, int nCount)
        {
            int n14;
            n14 = M2Share.GetValNameNo(sIndex);
            // 获得索引
            if (n14 >= 0)
            {
                switch (n14)
                {
                    // 根据不同的索引进行赋值
                    // Modify the A .. B: 0 .. 9
                    case 0:
                        PlayObject.m_nVal[n14] = nCount;
                        break;
                    // Modify the A .. B: 100 .. 119
                    case 100:
                        M2Share.g_Config.GlobalVal[n14 - 100] = nCount;
                        break;
                    // Modify the A .. B: 200 .. 209
                    case 200:
                        PlayObject.m_DyVal[n14 - 200] = nCount;
                        break;
                    // Modify the A .. B: 300 .. 399
                    case 300:
                        PlayObject.m_nMval[n14 - 300] = nCount;
                        break;
                    // Modify the A .. B: 400 .. 499
                    case 400:
                        M2Share.g_Config.GlobaDyMval[n14 - 400] = (short)nCount;
                        break;
                    // Modify the A .. B: 500 .. 599
                    case 500:
                        PlayObject.m_nSval[n14 - 600] = nCount.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        public bool GotoLable_QuestCheckCondition_CheckDieMon(TPlayObject PlayObject, string MonName)
        {
            bool result = false;
            if (MonName == "")
            {
                result = true;
            }
            if ((PlayObject.m_LastHiter != null) && (PlayObject.m_LastHiter.m_sCharName == MonName))
            {
                result = true;
            }
            return result;
        }

        public bool GotoLable_QuestCheckCondition_CheckKillMon(TPlayObject PlayObject, string MonName)
        {
            bool result = false;
            if (MonName == "")
            {
                result = true;
            }
            if ((PlayObject.m_TargetCret != null) && (PlayObject.m_TargetCret.m_sCharName == MonName))
            {
                result = true;
            }
            return result;
        }

        public bool GotoLable_QuestCheckCondition_CheckRandomNo(TPlayObject PlayObject, string sNumber)
        {
            bool result = false;
            if (PlayObject.m_sRandomNo == sNumber)
            {
                result = true;
            }
            return result;
        }

        public bool GotoLable_QuestCheckCondition_CheckUserDateType(TPlayObject PlayObject, string charName, string sListFileName, string sDay, string param1, string param2)
        {
            bool result;
            int nDay;
            int UseDay;
            int LastDay;
            DateTime nnday;
            int i;
            StringList LoadList;
            string sText = string.Empty;
            string Name = string.Empty;
            string ssDay = string.Empty;
            result = false;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            LoadList = new StringList();
            try
            {
                if (File.Exists(sListFileName))
                {
                    try
                    {

                        LoadList.LoadFromFile(sListFileName);
                    }
                    catch
                    {
                        M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                    }
                }
                nDay = HUtil32.Str_ToInt(sDay, 0);
                for (i = 0; i < LoadList.Count; i++)
                {
                    sText = LoadList[i].Trim();
                    sText = HUtil32.GetValidStrCap(sText, ref Name, new string[] { " ", "\t" });
                    Name = Name.Trim();
                    if (charName == Name)
                    {
                        ssDay = sText.Trim();
                        nnday = HUtil32.Str_ToDate(ssDay);
                        UseDay = HUtil32.Round(DateTime.Today.ToOADate() - nnday.ToOADate());
                        LastDay = nDay - UseDay;
                        if (LastDay < 0)
                        {
                            result = true;
                            LastDay = 0;
                        }
                        GotoLable_QuestCheckCondition_SetVal(PlayObject, param1, UseDay);
                        GotoLable_QuestCheckCondition_SetVal(PlayObject, param2, LastDay);
                        return result;
                    }
                }
            }
            finally
            {
                //LoadList.Free;
            }
            return result;
        }

        public bool GotoLable_QuestCheckCondition(TPlayObject PlayObject, IList<TQuestConditionInfo> ConditionList, ref string sC, ref TUserItem UserItem)
        {
            bool result = true;
            TQuestConditionInfo QuestConditionInfo;
            int n10 = 0;
            int n14 = 0;
            int n18 = 0;
            int n1C = 0;
            int nMaxDura = 0;
            int nDura = 0;
            int Hour = 0;
            int Min = 0;
            int Sec = 0;
            int MSec = 0;
            TEnvirnoment Envir;
            TItem StdItem;
            for (var i = 0; i < ConditionList.Count; i++)
            {
                QuestConditionInfo = ConditionList[i];
                switch (QuestConditionInfo.nCmdCode)
                {
                    case M2Share.nCHECKUSERDATE:
                        result = GotoLable_QuestCheckCondition_CheckUserDateType(PlayObject, PlayObject.m_sCharName, m_sPath + QuestConditionInfo.sParam1, QuestConditionInfo.sParam3, QuestConditionInfo.sParam4, QuestConditionInfo.sParam5);
                        break;
                    case M2Share.nSC_CHECKRANDOMNO:
                        Console.WriteLine("TODO nSC_CHECKRANDOMNO...");
                        //result = GotoLable_QuestCheckCondition_CheckRandomNo(PlayObject, sMsg);
                        break;
                    case M2Share.nCheckDiemon:
                        result = GotoLable_QuestCheckCondition_CheckDieMon(PlayObject, QuestConditionInfo.sParam1);
                        break;
                    case M2Share.ncheckkillplaymon:
                        result = GotoLable_QuestCheckCondition_CheckKillMon(PlayObject, QuestConditionInfo.sParam1);
                        break;
                    case M2Share.nCHECK:
                        n14 = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, 0);
                        n18 = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, 0);
                        n10 = PlayObject.GetQuestFalgStatus(n14);
                        if (n10 == 0)
                        {
                            if (n18 != 0)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            if (n18 == 0)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nRANDOM:
                        if (M2Share.RandomNumber.Random(QuestConditionInfo.nParam1) != 0)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nGENDER:
                        if (QuestConditionInfo.sParam1.ToLower().CompareTo(M2Share.sMAN.ToLower()) == 0)
                        {
                            if (PlayObject.m_btGender != ObjBase.gMan)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            if (PlayObject.m_btGender != ObjBase.gWoMan)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nDAYTIME:
                        if (QuestConditionInfo.sParam1.ToLower().CompareTo(M2Share.sSUNRAISE.ToLower()) == 0)
                        {
                            if (M2Share.g_nGameTime != 0)
                            {
                                result = false;
                            }
                        }
                        if (QuestConditionInfo.sParam1.ToLower().CompareTo(M2Share.sDAY.ToLower()) == 0)
                        {
                            if (M2Share.g_nGameTime != 1)
                            {
                                result = false;
                            }
                        }
                        if (QuestConditionInfo.sParam1.ToLower().CompareTo(M2Share.sSUNSET.ToLower()) == 0)
                        {
                            if (M2Share.g_nGameTime != 2)
                            {
                                result = false;
                            }
                        }
                        if (QuestConditionInfo.sParam1.ToLower().CompareTo(M2Share.sNIGHT.ToLower()) == 0)
                        {
                            if (M2Share.g_nGameTime != 3)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nCHECKOPEN:
                        n14 = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, 0);
                        n18 = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, 0);
                        n10 = PlayObject.GetQuestUnitOpenStatus(n14);
                        if (n10 == 0)
                        {
                            if (n18 != 0)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            if (n18 == 0)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nCHECKUNIT:
                        n14 = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, 0);
                        n18 = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, 0);
                        n10 = PlayObject.GetQuestUnitStatus(n14);
                        if (n10 == 0)
                        {
                            if (n18 != 0)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            if (n18 == 0)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nCHECKLEVEL:
                        if (PlayObject.m_Abil.Level < QuestConditionInfo.nParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKJOB:
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sWarrior, M2Share.sWarrior.Length))
                        {
                            if (PlayObject.m_btJob != M2Share.jWarr)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sWizard, M2Share.sWizard.Length))
                        {
                            if (PlayObject.m_btJob != M2Share.jWizard)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sTaos, M2Share.sTaos.Length))
                        {
                            if (PlayObject.m_btJob != M2Share.jTaos)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nCHECKBBCOUNT:
                        if (PlayObject.m_SlaveList.Count < QuestConditionInfo.nParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKCREDITPOINT:
                        break;
                    case M2Share.nCHECKITEM:
                        UserItem = PlayObject.QuestCheckItem(QuestConditionInfo.sParam1, ref n1C, ref nMaxDura, ref nDura);
                        if (n1C < QuestConditionInfo.nParam2)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKITEMW:
                        UserItem = GotoLable_CheckItemW(PlayObject, QuestConditionInfo.sParam1, QuestConditionInfo.nParam2);
                        if (UserItem == null)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKGOLD:
                        if (PlayObject.m_nGold < QuestConditionInfo.nParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nISTAKEITEM:
                        if (sC != QuestConditionInfo.sParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKDURA:
                        UserItem = PlayObject.QuestCheckItem(QuestConditionInfo.sParam1, ref n1C, ref nMaxDura, ref nDura);
                        if (HUtil32.Round(nDura / 1000) < QuestConditionInfo.nParam2)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKDURAEVA:
                        UserItem = PlayObject.QuestCheckItem(QuestConditionInfo.sParam1, ref n1C, ref nMaxDura, ref nDura);
                        if (n1C > 0)
                        {
                            if (HUtil32.Round(nMaxDura / n1C / 1000) < QuestConditionInfo.nParam2)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nDAYOFWEEK:
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sSUN, M2Share.sSUN.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 1)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sMON, M2Share.sMON.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 2)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sTUE, M2Share.sTUE.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 3)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sWED, M2Share.sWED.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 4)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sTHU, M2Share.sTHU.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 5)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sFRI, M2Share.sFRI.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 6)
                            {
                                result = false;
                            }
                        }
                        if (HUtil32.CompareLStr(QuestConditionInfo.sParam1, M2Share.sSAT, M2Share.sSAT.Length))
                        {
                            if ((int)DateTime.Now.DayOfWeek != 7)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nHOUR:
                        if ((QuestConditionInfo.nParam1 != 0) && (QuestConditionInfo.nParam2 == 0))
                        {
                            QuestConditionInfo.nParam2 = QuestConditionInfo.nParam1;
                        }
                        Hour = DateTime.Now.Hour;
                        Min = DateTime.Now.Minute;
                        Sec = DateTime.Now.Second;
                        MSec = DateTime.Now.Millisecond;
                        if ((Hour < QuestConditionInfo.nParam1) || (Hour > QuestConditionInfo.nParam2))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nMIN:
                        if ((QuestConditionInfo.nParam1 != 0) && (QuestConditionInfo.nParam2 == 0))
                        {
                            QuestConditionInfo.nParam2 = QuestConditionInfo.nParam1;
                        }
                        Hour = DateTime.Now.Hour;
                        Min = DateTime.Now.Minute;
                        Sec = DateTime.Now.Second;
                        MSec = DateTime.Now.Millisecond;
                        if ((Min < QuestConditionInfo.nParam1) || (Min > QuestConditionInfo.nParam2))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKPKPOINT:
                        if (PlayObject.PKLevel() < QuestConditionInfo.nParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKLUCKYPOINT:
                        if (PlayObject.m_nBodyLuckLevel < QuestConditionInfo.nParam1)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKMONMAP:
                        Envir = M2Share.g_MapManager.FindMap(QuestConditionInfo.sParam1);
                        if (Envir != null)
                        {
                            if (M2Share.UserEngine.GetMapMonster(Envir, null) < QuestConditionInfo.nParam2)
                            {
                                result = false;
                            }
                        }
                        break;
                    case M2Share.nCHECKMONAREA:
                        break;
                    case M2Share.nCHECKHUM:
                        // 0049C4CB
                        if (M2Share.UserEngine.GetMapHuman(QuestConditionInfo.sParam1) < QuestConditionInfo.nParam2)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKBAGGAGE:
                        if (PlayObject.IsEnoughBag())
                        {
                            if (QuestConditionInfo.sParam1 != "")
                            {
                                result = false;
                                StdItem = M2Share.UserEngine.GetStdItem(QuestConditionInfo.sParam1);
                                if (StdItem != null)
                                {
                                    if (PlayObject.IsAddWeightAvailable(StdItem.Weight))
                                    {
                                        result = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKNAMELIST:
                        if (!GotoLable_CheckStringList(PlayObject.m_sCharName, m_sPath + QuestConditionInfo.sParam1))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKACCOUNTLIST:
                        if (!GotoLable_CheckStringList(PlayObject.m_sUserID, m_sPath + QuestConditionInfo.sParam1))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nCHECKIPLIST:
                        if (!GotoLable_CheckStringList(PlayObject.m_sIPaddr, m_sPath + QuestConditionInfo.sParam1))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nEQUAL:
                        // 0049C5AC
                        n10 = M2Share.GetValNameNo(QuestConditionInfo.sParam1);
                        if (n10 >= 0)
                        {
                            switch (n10)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    if (PlayObject.m_nVal[n10] != QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    if (M2Share.g_Config.GlobalVal[n10 - 100] != QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    if (PlayObject.m_DyVal[n10 - 200] != QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    if (PlayObject.m_nMval[n10 - 300] != QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    if (M2Share.g_Config.GlobaDyMval[n10 - 400] != QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                            }
                            // case
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nLARGE:
                        // 0049C658
                        n10 = M2Share.GetValNameNo(QuestConditionInfo.sParam1);
                        if (n10 >= 0)
                        {
                            switch (n10)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    if (PlayObject.m_nVal[n10] <= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    if (M2Share.g_Config.GlobalVal[n10 - 100] <= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    if (PlayObject.m_DyVal[n10 - 200] <= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    if (PlayObject.m_nMval[n10 - 300] <= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    if (M2Share.g_Config.GlobaDyMval[n10 - 400] <= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                            }
                            // case
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSMALL:
                        // 0049C704
                        n10 = M2Share.GetValNameNo(QuestConditionInfo.sParam1);
                        if (n10 >= 0)
                        {
                            switch (n10)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    if (PlayObject.m_nVal[n10] >= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    if (M2Share.g_Config.GlobalVal[n10 - 100] >= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    if (PlayObject.m_DyVal[n10 - 200] >= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    if (PlayObject.m_nMval[n10 - 300] >= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    if (M2Share.g_Config.GlobaDyMval[n10 - 400] >= QuestConditionInfo.nParam2)
                                    {
                                        result = false;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISSYSOP:
                        if (!(PlayObject.m_btPermission >= 4))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISADMIN:
                        if (!(PlayObject.m_btPermission >= 6))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGROUPCOUNT:
                        if (!ConditionOfCheckGroupCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSEDIR:
                        if (!ConditionOfCheckPoseDir(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSELEVEL:
                        if (!ConditionOfCheckPoseLevel(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSEGENDER:
                        if (!ConditionOfCheckPoseGender(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKLEVELEX:
                        if (!ConditionOfCheckLevelEx(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKBONUSPOINT:
                        if (!ConditionOfCheckBonusPoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMARRY:
                        if (!ConditionOfCheckMarry(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSEMARRY:
                        if (!ConditionOfCheckPoseMarry(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMARRYCOUNT:
                        if (!ConditionOfCheckMarryCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMASTER:
                        if (!ConditionOfCheckMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_HAVEMASTER:
                        if (!ConditionOfHaveMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSEMASTER:
                        if (!ConditionOfCheckPoseMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_POSEHAVEMASTER:
                        if (!ConditionOfPoseHaveMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKISMASTER:
                        if (!ConditionOfCheckIsMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_HASGUILD:
                        if (!ConditionOfCheckHaveGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISGUILDMASTER:
                        if (!ConditionOfCheckIsGuildMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKCASTLEMASTER:
                        if (!ConditionOfCheckIsCastleMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISCASTLEGUILD:
                        if (!ConditionOfCheckIsCastleaGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISATTACKGUILD:
                        if (!ConditionOfCheckIsAttackGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISDEFENSEGUILD:
                        if (!ConditionOfCheckIsDefenseGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKCASTLEDOOR:
                        if (!ConditionOfCheckCastleDoorStatus(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISATTACKALLYGUILD:
                        if (!ConditionOfCheckIsAttackAllyGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISDEFENSEALLYGUILD:
                        if (!ConditionOfCheckIsDefenseAllyGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOSEISMASTER:
                        if (!ConditionOfCheckPoseIsMaster(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKNAMEIPLIST:
                        if (!ConditionOfCheckNameIPList(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKACCOUNTIPLIST:
                        if (!ConditionOfCheckAccountIPList(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSLAVECOUNT:
                        if (!ConditionOfCheckSlaveCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISNEWHUMAN:
                        if (!PlayObject.m_boNewHuman)
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMEMBERTYPE:
                        if (!ConditionOfCheckMemberType(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMEMBERLEVEL:
                        if (!ConditionOfCheckMemBerLevel(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGAMEGOLD:
                        if (!ConditionOfCheckGameGold(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGAMEPOINT:
                        if (!ConditionOfCheckGamePoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKNAMELISTPOSITION:
                        if (!ConditionOfCheckNameListPostion(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGUILDLIST:
                        // nSC_CHECKGUILDLIST:     if not ConditionOfCheckGuildList(PlayObject,QuestConditionInfo) then Result:=False;
                        if (PlayObject.m_MyGuild != null)
                        {
                            if (!GotoLable_CheckStringList(PlayObject.m_MyGuild.sGuildName, m_sPath + QuestConditionInfo.sParam1))
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKRENEWLEVEL:
                        if (!ConditionOfCheckReNewLevel(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSLAVELEVEL:
                        if (!ConditionOfCheckSlaveLevel(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSLAVENAME:
                        if (!ConditionOfCheckSlaveName(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKCREDITPOINT:
                        if (!ConditionOfCheckCreditPoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKOFGUILD:
                        if (!ConditionOfCheckOfGuild(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPAYMENT:
                        if (!ConditionOfCheckPayMent(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKUSEITEM:
                        if (!ConditionOfCheckUseItem(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKBAGSIZE:
                        if (!ConditionOfCheckBagSize(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKLISTCOUNT:
                        if (!ConditionOfCheckListCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKDC:
                        if (!ConditionOfCheckDC(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMC:
                        if (!ConditionOfCheckMC(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSC:
                        if (!ConditionOfCheckSC(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKHP:
                        if (!ConditionOfCheckHP(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMP:
                        if (!ConditionOfCheckMP(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKITEMTYPE:
                        if (!ConditionOfCheckItemType(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKEXP:
                        if (!ConditionOfCheckExp(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKCASTLEGOLD:
                        if (!ConditionOfCheckCastleGold(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_PASSWORDERRORCOUNT:
                        if (!ConditionOfCheckPasswordErrorCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISLOCKPASSWORD:
                        if (!ConditionOfIsLockPassword(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISLOCKSTORAGE:
                        if (!ConditionOfIsLockStorage(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKBUILDPOINT:
                        if (!ConditionOfCheckGuildBuildPoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKAURAEPOINT:
                        if (!ConditionOfCheckGuildAuraePoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSTABILITYPOINT:
                        if (!ConditionOfCheckStabilityPoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKFLOURISHPOINT:
                        if (!ConditionOfCheckFlourishPoint(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKCONTRIBUTION:
                        if (!ConditionOfCheckContribution(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKRANGEMONCOUNT:
                        if (!ConditionOfCheckRangeMonCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKITEMADDVALUE:
                        if (!ConditionOfCheckItemAddValue(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKINMAPRANGE:
                        if (!ConditionOfCheckInMapRange(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CASTLECHANGEDAY:
                        if (!ConditionOfCheckCastleChageDay(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CASTLEWARDAY:
                        if (!ConditionOfCheckCastleWarDay(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ONLINELONGMIN:
                        if (!ConditionOfCheckOnlineLongMin(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGUILDCHIEFITEMCOUNT:
                        if (!ConditionOfCheckChiefItemCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKNAMEDATELIST:
                        if (!ConditionOfCheckNameDateList(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMAPHUMANCOUNT:
                        if (!ConditionOfCheckMapHumanCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMAPMONCOUNT:
                        if (!ConditionOfCheckMapMonCount(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKVAR:
                        if (!ConditionOfCheckVar(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKSERVERNAME:
                        if (!ConditionOfCheckServerName(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKISONMAP:
                        if (!ConditionOfCheckIsOnMap(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_KILLBYHUM:
                        if ((PlayObject.m_LastHiter != null) && (PlayObject.m_LastHiter.m_btRaceServer != grobal2.RC_PLAYOBJECT))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_KILLBYMON:
                        if ((PlayObject.m_LastHiter != null) && (PlayObject.m_LastHiter.m_btRaceServer == grobal2.RC_PLAYOBJECT))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKINSAFEZONE:
                        if (!PlayObject.InSafeZone())
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMAP:
                        if (!ConditionOfCheckMap(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKPOS:
                        if (!ConditionOfCheckPos(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_REVIVESLAVE:
                        if (!ConditionOfReviveSlave(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKMAGICLVL:
                        if (!ConditionOfCheckMagicLvl(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_CHECKGROUPCLASS:
                        if (!ConditionOfCheckGroupClass(PlayObject, QuestConditionInfo))
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISGROUPMASTER:
                        if (PlayObject.m_GroupOwner != null)
                        {
                            if (PlayObject.m_GroupOwner != PlayObject)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                    case M2Share.nSC_ISHIGH:
                        result = ConditionOfIsHigh(PlayObject, QuestConditionInfo);
                        break;
                }
                if (!result)
                {
                    break;
                }
            }
            return result;
        }

        public bool GotoLable_JmpToLable(TPlayObject PlayObject, string sLabel)
        {
            bool result = false;
            PlayObject.m_nScriptGotoCount++;
            if (PlayObject.m_nScriptGotoCount > M2Share.g_Config.nScriptGotoCountLimit)
            {
                return result;
            }
            GotoLable(PlayObject, sLabel, false);
            result = true;
            return result;
        }

        public void GotoLable_GoToQuest(TPlayObject PlayObject, int nQuest)
        {
            TScript Script;
            for (var i = 0; i < m_ScriptList.Count; i++)
            {
                Script = m_ScriptList[i];
                if (Script.nQuest == nQuest)
                {
                    PlayObject.m_Script = Script;
                    PlayObject.m_NPC = this;
                    GotoLable(PlayObject, M2Share.sMAIN, false);
                    break;
                }
            }
        }

        public void GotoLable_AddUseDateList(string sHumName, string sListFileName)
        {
            StringList LoadList;
            string s10 = string.Empty;
            string sText;
            bool bo15;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {
                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            bo15 = false;
            for (var i = 0; i < LoadList.Count; i++)
            {
                sText = LoadList[i].Trim();
                sText = HUtil32.GetValidStrCap(sText, ref s10, new string[] { " ", "\t" });
                if (sHumName.ToLower().CompareTo(s10.ToLower()) == 0)
                {
                    bo15 = true;
                    break;
                }
            }
            if (!bo15)
            {
                s10 = string.Format("%s    %s", new string[] { sHumName, DateTime.Today.ToString() });
                LoadList.Add(s10);
                try
                {
                    LoadList.SaveToFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("saving fail.... => " + sListFileName);
                }
            }
            //LoadList.Free;
        }

        public void GotoLable_AddList(string sHumName, string sListFileName)
        {
            StringList LoadList;
            string s10 = string.Empty;
            bool bo15;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {
                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            bo15 = false;
            for (var I = 0; I < LoadList.Count; I++)
            {
                s10 = LoadList[I].Trim();
                if (sHumName.ToLower().CompareTo(s10.ToLower()) == 0)
                {
                    bo15 = true;
                    break;
                }
            }
            if (!bo15)
            {
                LoadList.Add(sHumName);
                try
                {
                    LoadList.SaveToFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("saving fail.... => " + sListFileName);
                }
            }
            // LoadList.Free;
        }

        public void GotoLable_DELUseDateList(string sHumName, string sListFileName)
        {
            StringList LoadList;
            string s10 = string.Empty;
            string sText;
            bool bo15;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {
                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            bo15 = false;
            for (var i = 0; i < LoadList.Count; i++)
            {
                sText = LoadList[i].Trim();
                sText = HUtil32.GetValidStrCap(sText, ref s10, new string[] { " ", "\t" });
                if (sHumName.ToLower().CompareTo(s10.ToLower()) == 0)
                {
                    bo15 = true;
                    LoadList.RemoveAt(i);
                    break;
                }
            }
            if (bo15)
            {
                try
                {
                    LoadList.SaveToFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("saving fail.... => " + sListFileName);
                }
            }
            //LoadList.Free;
        }

        public void GotoLable_DelList(string sHumName, string sListFileName)
        {
            StringList LoadList;
            string s10 = string.Empty;
            bool bo15;
            sListFileName = M2Share.g_Config.sEnvirDir + sListFileName;
            LoadList = new StringList();
            if (File.Exists(sListFileName))
            {
                try
                {
                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
            }
            bo15 = false;
            for (var I = 0; I < LoadList.Count; I++)
            {
                s10 = LoadList[I].Trim();
                if (sHumName.ToLower().CompareTo(s10.ToLower()) == 0)
                {
                    LoadList.RemoveAt(I);
                    bo15 = true;
                    break;
                }
            }
            if (bo15)
            {
                try
                {
                    LoadList.SaveToFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("saving fail.... => " + sListFileName);
                }
            }
            // LoadList.Free;
        }

        public void GotoLable_TakeItem(TPlayObject PlayObject, string sItemName, int nItemCount, ref string sC)
        {
            TUserItem UserItem;
            TItem StdItem;
            if (sItemName.ToLower().CompareTo(grobal2.sSTRING_GOLDNAME.ToLower()) == 0)
            {
                PlayObject.DecGold(nItemCount);
                PlayObject.GoldChanged();
                if (M2Share.g_boGameLogGold)
                {
                    M2Share.AddGameDataLog("10" + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + grobal2.sSTRING_GOLDNAME + "\t" + nItemCount.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                }
                return;
            }
            for (var i = PlayObject.m_ItemList.Count - 1; i >= 0; i--)
            {
                if (nItemCount <= 0)
                {
                    break;
                }
                UserItem = PlayObject.m_ItemList[i];
                if (M2Share.UserEngine.GetStdItemName(UserItem.wIndex).ToLower().CompareTo(sItemName.ToLower()) == 0)
                {
                    StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                    if (StdItem.NeedIdentify == 1)
                    {
                        M2Share.AddGameDataLog("10" + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + sItemName + "\t" + UserItem.MakeIndex.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                    }
                    PlayObject.SendDelItems(UserItem);
                    sC = M2Share.UserEngine.GetStdItemName(UserItem.wIndex);
                    Dispose(UserItem);
                    PlayObject.m_ItemList.RemoveAt(i);
                    nItemCount -= 1;
                }
            }
        }

        public void GotoLable_GiveItem(TPlayObject PlayObject, string sItemName, int nItemCount)
        {
            // 0049D1D0
            int I;
            TUserItem UserItem;
            TItem StdItem;
            if (sItemName.ToLower().CompareTo(grobal2.sSTRING_GOLDNAME.ToLower()) == 0)
            {
                PlayObject.IncGold(nItemCount);
                PlayObject.GoldChanged();
                // 0049D2FE
                if (M2Share.g_boGameLogGold)
                {
                    M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + grobal2.sSTRING_GOLDNAME + "\t" + nItemCount.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                }
                return;
            }
            if (M2Share.UserEngine.GetStdItemIdx(sItemName) > 0)
            {
                // if nItemCount > 50 then nItemCount:=50;//11.22 限制数量大小
                if (!(nItemCount >= 1 && nItemCount <= 50))
                {
                    nItemCount = 1;
                }
                // 12.28 改上一条
                for (I = 0; I < nItemCount; I++)
                {
                    // nItemCount 为0时出死循环
                    if (PlayObject.IsEnoughBag())
                    {
                        UserItem = new TUserItem();
                        if (M2Share.UserEngine.CopyToUserItemFromName(sItemName, ref UserItem))
                        {
                            PlayObject.m_ItemList.Add(UserItem);
                            PlayObject.SendAddItem(UserItem);
                            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                            // 0049D46B
                            if (StdItem.NeedIdentify == 1)
                            {
                                M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + sItemName + "\t" + UserItem.MakeIndex.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                            }
                        }
                        else
                        {

                            Dispose(UserItem);
                        }
                    }
                    else
                    {
                        UserItem = new TUserItem();
                        if (M2Share.UserEngine.CopyToUserItemFromName(sItemName, ref UserItem))
                        {
                            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                            // 0049D5A5
                            if (StdItem.NeedIdentify == 1)
                            {
                                M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + sItemName + "\t" + UserItem.MakeIndex.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                            }
                            PlayObject.DropItemDown(UserItem, 3, false, PlayObject, null);
                        }

                        Dispose(UserItem);
                    }
                }
            }
        }

        public void GotoLable_TakeWItem(TPlayObject PlayObject, string sItemName, int nItemCount)
        {
            int I;
            string sName = string.Empty;
            string sC = string.Empty;
            if (HUtil32.CompareLStr(sItemName, "[NECKLACE]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_NECKLACE].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_NECKLACE]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_NECKLACE].wIndex);
                    PlayObject.m_UseItems[grobal2.U_NECKLACE].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[RING]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_RINGL].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_RINGL]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_RINGL].wIndex);
                    PlayObject.m_UseItems[grobal2.U_RINGL].wIndex = 0;
                    return;
                }
                if (PlayObject.m_UseItems[grobal2.U_RINGR].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_RINGR]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_RINGR].wIndex);
                    PlayObject.m_UseItems[grobal2.U_RINGR].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[ARMRING]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_ARMRINGL].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_ARMRINGL]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_ARMRINGL].wIndex);
                    PlayObject.m_UseItems[grobal2.U_ARMRINGL].wIndex = 0;
                    return;
                }
                if (PlayObject.m_UseItems[grobal2.U_ARMRINGR].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_ARMRINGR]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_ARMRINGR].wIndex);
                    PlayObject.m_UseItems[grobal2.U_ARMRINGR].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[WEAPON]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_WEAPON].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_WEAPON]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_WEAPON].wIndex);
                    PlayObject.m_UseItems[grobal2.U_WEAPON].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[HELMET]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_HELMET].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_HELMET]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_HELMET].wIndex);
                    PlayObject.m_UseItems[grobal2.U_HELMET].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[DRESS]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_DRESS].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_DRESS]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_DRESS].wIndex);
                    PlayObject.m_UseItems[grobal2.U_DRESS].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[U_BUJUK]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BUJUK].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_BUJUK]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BUJUK].wIndex);
                    PlayObject.m_UseItems[grobal2.U_BUJUK].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[U_BELT]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BELT].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_BELT]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BELT].wIndex);
                    PlayObject.m_UseItems[grobal2.U_BELT].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[U_BOOTS]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_BOOTS].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_BOOTS]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_BOOTS].wIndex);
                    PlayObject.m_UseItems[grobal2.U_BOOTS].wIndex = 0;
                    return;
                }
            }
            if (HUtil32.CompareLStr(sItemName, "[U_CHARM]", 4))
            {
                if (PlayObject.m_UseItems[grobal2.U_CHARM].wIndex > 0)
                {
                    PlayObject.SendDelItems(PlayObject.m_UseItems[grobal2.U_CHARM]);
                    sC = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[grobal2.U_CHARM].wIndex);
                    PlayObject.m_UseItems[grobal2.U_CHARM].wIndex = 0;
                    return;
                }
            }
            for (I = PlayObject.m_UseItems.GetLowerBound(0); I <= PlayObject.m_UseItems.GetUpperBound(0); I++)
            {
                if (nItemCount <= 0)
                {
                    return;
                }
                if (PlayObject.m_UseItems[I].wIndex > 0)
                {
                    sName = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[I].wIndex);
                    if (sName.ToLower().CompareTo(sItemName.ToLower()) == 0)
                    {
                        PlayObject.SendDelItems(PlayObject.m_UseItems[I]);
                        PlayObject.m_UseItems[I].wIndex = 0;
                        nItemCount -= 1;
                    }
                }
            }
        }

        public bool GotoLable_QuestActionProcess(TPlayObject PlayObject, IList<TQuestActionInfo> ActionList, ref string sC, ref TUserItem UserItem)
        {
            bool result;
            int II;
            TQuestActionInfo QuestActionInfo;
            int n14;
            int n18;
            int n1C;
            int n28;
            int n2C;
            int n20X;
            int n24Y;
            int n34;
            int n38;
            int n3C;
            int n40;
            string s4C = string.Empty;
            string s50 = string.Empty;
            string s34 = string.Empty;
            string s44 = string.Empty;
            string s48 = string.Empty;
            TEnvirnoment Envir;
            ArrayList List58;
            TPlayObject User;
            result = true;
            n18 = 0;
            n34 = 0;
            n38 = 0;
            n3C = 0;
            n40 = 0;
            for (var I = 0; I < ActionList.Count; I++)
            {
                QuestActionInfo = ActionList[I];
                switch (QuestActionInfo.nCmdCode)
                {
                    case M2Share.nSET:
                        n28 = HUtil32.Str_ToInt(QuestActionInfo.sParam1, 0);
                        n2C = HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0);
                        PlayObject.SetQuestFlagStatus(n28, n2C);
                        break;
                    case M2Share.nTAKE:
                        GotoLable_TakeItem(PlayObject, QuestActionInfo.sParam1, QuestActionInfo.nParam2, ref sC);
                        break;
                    case M2Share.nSC_GIVE:
                        // nGIVE: GiveItem(QuestActionInfo.sParam1,QuestActionInfo.nParam2);
                        ActionOfGiveItem(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nTAKEW:
                        GotoLable_TakeWItem(PlayObject, QuestActionInfo.sParam1, QuestActionInfo.nParam2);
                        break;
                    case M2Share.nCLOSE:
                        PlayObject.SendMsg(this, grobal2.RM_MERCHANTDLGCLOSE, 0, this.ObjectId, 0, 0, "");
                        break;
                    case M2Share.nRESET:
                        for (II = 0; II < QuestActionInfo.nParam2; II++)
                        {
                            PlayObject.SetQuestFlagStatus(QuestActionInfo.nParam1 + II, 0);
                        }
                        break;
                    case M2Share.nSETOPEN:
                        n28 = HUtil32.Str_ToInt(QuestActionInfo.sParam1, 0);
                        n2C = HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0);
                        PlayObject.SetQuestUnitOpenStatus(n28, n2C);
                        break;
                    case M2Share.nSETUNIT:
                        n28 = HUtil32.Str_ToInt(QuestActionInfo.sParam1, 0);
                        n2C = HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0);
                        PlayObject.SetQuestUnitStatus(n28, n2C);
                        break;
                    case M2Share.nRESETUNIT:
                        for (II = 0; II < QuestActionInfo.nParam2; II++)
                        {
                            PlayObject.SetQuestUnitStatus(QuestActionInfo.nParam1 + II, 0);
                        }
                        break;
                    case M2Share.nBREAK:
                        result = false;
                        break;
                    case M2Share.nTIMERECALL:
                        PlayObject.m_boTimeRecall = true;
                        PlayObject.m_sMoveMap = PlayObject.m_sMapName;
                        PlayObject.m_nMoveX = PlayObject.m_nCurrX;
                        PlayObject.m_nMoveY = PlayObject.m_nCurrY;

                        PlayObject.m_dwTimeRecallTick = HUtil32.GetTickCount() + (QuestActionInfo.nParam1 * 60 * 1000);
                        break;
                    case M2Share.nSC_PARAM1:
                        n34 = QuestActionInfo.nParam1;
                        s44 = QuestActionInfo.sParam1;
                        break;
                    case M2Share.nSC_PARAM2:
                        n38 = QuestActionInfo.nParam1;
                        s48 = QuestActionInfo.sParam1;
                        break;
                    case M2Share.nSC_PARAM3:
                        n3C = QuestActionInfo.nParam1;
                        s4C = QuestActionInfo.sParam1;
                        break;
                    case M2Share.nSC_PARAM4:
                        n40 = QuestActionInfo.nParam1;
                        s50 = QuestActionInfo.sParam1;
                        break;
                    case M2Share.nSC_EXEACTION:
                        n40 = QuestActionInfo.nParam1;
                        s50 = QuestActionInfo.sParam1;
                        ExeAction(PlayObject, QuestActionInfo.sParam1, QuestActionInfo.sParam2, QuestActionInfo.sParam3, QuestActionInfo.nParam1, QuestActionInfo.nParam2, QuestActionInfo.nParam3);
                        break;
                    case M2Share.nMAPMOVE:
                        PlayObject.SendRefMsg(grobal2.RM_SPACEMOVE_FIRE, 0, 0, 0, 0, "");
                        PlayObject.SpaceMove(QuestActionInfo.sParam1, (short)QuestActionInfo.nParam2, (short)QuestActionInfo.nParam3, 0);
                        //bo11 = true;
                        break;
                    case M2Share.nMAP:
                        PlayObject.SendRefMsg(grobal2.RM_SPACEMOVE_FIRE, 0, 0, 0, 0, "");
                        PlayObject.MapRandomMove(QuestActionInfo.sParam1, 0);
                        //bo11 = true;
                        break;
                    case M2Share.nTAKECHECKITEM:
                        if (UserItem != null)
                        {
                            PlayObject.QuestTakeCheckItem(UserItem);
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sTAKECHECKITEM);
                        }
                        break;
                    case M2Share.nMONGEN:
                        for (II = 0; II < QuestActionInfo.nParam2; II++)
                        {
                            n20X = M2Share.RandomNumber.Random(QuestActionInfo.nParam3 * 2 + 1) + (n38 - QuestActionInfo.nParam3);
                            n24Y = M2Share.RandomNumber.Random(QuestActionInfo.nParam3 * 2 + 1) + (n3C - QuestActionInfo.nParam3);
                            M2Share.UserEngine.RegenMonsterByName(s44, (short)n20X, (short)n24Y, QuestActionInfo.sParam1);
                        }
                        break;
                    case M2Share.nMONCLEAR:
                        List58 = new ArrayList();
                        M2Share.UserEngine.GetMapMonster(M2Share.g_MapManager.FindMap(QuestActionInfo.sParam1), List58);
                        for (II = 0; II < List58.Count; II++)
                        {
                            ((TBaseObject)List58[II]).m_boNoItem = true;
                            ((TBaseObject)List58[II]).m_WAbil.HP = 0;
                        }
                        // for
                        //List58.Free;
                        break;
                    case M2Share.nMOV:
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            if (HUtil32.RangeInDefined(n14, 0, 10))
                            {
                                PlayObject.m_nVal[n14] = QuestActionInfo.nParam2;
                            }
                            else if (HUtil32.RangeInDefined(n14, 100, 120))
                            {
                                M2Share.g_Config.GlobalVal[n14 - 100] = QuestActionInfo.nParam2;
                            }
                            else if (HUtil32.RangeInDefined(n14, 200, 210))
                            {
                                PlayObject.m_DyVal[n14 - 200] = QuestActionInfo.nParam2;
                            }
                            else if (HUtil32.RangeInDefined(n14, 300, 400))
                            {
                                PlayObject.m_nMval[n14 - 300] = QuestActionInfo.nParam2;
                            }
                            else if (HUtil32.RangeInDefined(n14, 400, 500))
                            {
                                M2Share.g_Config.GlobaDyMval[n14 - 400] = (short)QuestActionInfo.nParam2;
                            }
                            else
                            {
                                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sMOV);
                                break;
                            }
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sMOV);
                        }
                        break;
                    case M2Share.nINC:
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_nVal[n14] += QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_nVal[n14]++;
                                    }
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        M2Share.g_Config.GlobalVal[n14 - 100] += QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        M2Share.g_Config.GlobalVal[n14 - 100]++;
                                    }
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_DyVal[n14 - 200] += QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_DyVal[n14 - 200]++;
                                    }
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_nMval[n14 - 300] += QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_nMval[n14 - 300]++;
                                    }
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        M2Share.g_Config.GlobaDyMval[n14 - 400] += (short)QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        M2Share.g_Config.GlobaDyMval[n14 - 400]++;
                                    }
                                    break;
                                default:
                                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sINC);
                                    break;
                            }
                            // case
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sINC);
                        }
                        break;
                    case M2Share.nDEC:
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_nVal[n14] -= QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_nVal[n14] -= 1;
                                    }
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        M2Share.g_Config.GlobalVal[n14 - 100] -= QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        M2Share.g_Config.GlobalVal[n14 - 100] -= 1;
                                    }
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_DyVal[n14 - 200] -= QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_DyVal[n14 - 200] -= 1;
                                    }
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        PlayObject.m_nMval[n14 - 300] -= QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        PlayObject.m_nMval[n14 - 300] -= 1;
                                    }
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    if (QuestActionInfo.nParam2 > 1)
                                    {
                                        M2Share.g_Config.GlobaDyMval[n14 - 400] -= (short)QuestActionInfo.nParam2;
                                    }
                                    else
                                    {
                                        M2Share.g_Config.GlobaDyMval[n14 - 400] -= 1;
                                    }
                                    break;
                                default:
                                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sDEC);
                                    break;
                            }
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sDEC);
                        }
                        break;
                    case M2Share.nSUM:
                        n18 = 0;
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    n18 = PlayObject.m_nVal[n14];
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    n18 = M2Share.g_Config.GlobalVal[n14 - 100];
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    n18 = PlayObject.m_DyVal[n14 - 200];
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    n18 = PlayObject.m_nMval[n14 - 300];
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    n18 = M2Share.g_Config.GlobaDyMval[n14 - 400];
                                    break;
                                default:
                                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSUM);
                                    break;
                            }
                            // case
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSUM);
                        }
                        n1C = 0;
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam2);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    n1C = PlayObject.m_nVal[n14];
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    n1C = M2Share.g_Config.GlobalVal[n14 - 100];
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    n1C = PlayObject.m_DyVal[n14 - 200];
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    n1C = PlayObject.m_nMval[n14 - 300];
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    n1C = M2Share.g_Config.GlobaDyMval[n14 - 400];
                                    break;
                                default:
                                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSUM);
                                    break;
                            }
                        }
                        else
                        {
                            // ScriptActionError(PlayObject,'',QuestActionInfo,sSUM);
                        }
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    PlayObject.m_nVal[9] = PlayObject.m_nVal[9] + n18 + n1C;
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    M2Share.g_Config.GlobalVal[9] = M2Share.g_Config.GlobalVal[9] + n18 + n1C;
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    PlayObject.m_DyVal[9] = PlayObject.m_DyVal[9] + n18 + n1C;
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    PlayObject.m_nMval[99] = PlayObject.m_nMval[99] + n18 + n1C;
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    M2Share.g_Config.GlobaDyMval[99] = (short)(M2Share.g_Config.GlobaDyMval[99] + n18 + n1C);
                                    break;
                            }
                        }
                        break;
                    case M2Share.nBREAKTIMERECALL:
                        PlayObject.m_boTimeRecall = false;
                        break;
                    case M2Share.nCHANGEMODE:
                        switch (QuestActionInfo.nParam1)
                        {
                            case 1:
                                PlayObject.CmdChangeAdminMode("", 10, "", HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0) == 1);
                                break;
                            case 2:
                                PlayObject.CmdChangeSuperManMode("", 10, "", HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0) == 1);
                                break;
                            case 3:
                                PlayObject.CmdChangeObMode("", 10, "", HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0) == 1);
                                break;
                            default:
                                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sCHANGEMODE);
                                break;
                        }
                        break;
                    case M2Share.nPKPOINT:
                        if (QuestActionInfo.nParam1 == 0)
                        {
                            PlayObject.m_nPkPoint = 0;
                        }
                        else
                        {
                            if (QuestActionInfo.nParam1 < 0)
                            {
                                if ((PlayObject.m_nPkPoint + QuestActionInfo.nParam1) >= 0)
                                {
                                    PlayObject.m_nPkPoint += QuestActionInfo.nParam1;
                                }
                                else
                                {
                                    PlayObject.m_nPkPoint = 0;
                                }
                            }
                            else
                            {
                                if ((PlayObject.m_nPkPoint + QuestActionInfo.nParam1) > 10000)
                                {
                                    PlayObject.m_nPkPoint = 10000;
                                }
                                else
                                {
                                    PlayObject.m_nPkPoint += QuestActionInfo.nParam1;
                                }
                            }
                        }
                        PlayObject.RefNameColor();
                        break;
                    case M2Share.nCHANGEXP:
                        break;
                    case M2Share.nSC_RECALLMOB:
                        ActionOfRecallmob(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nKICK:
                        PlayObject.m_boReconnection = true;
                        PlayObject.m_boSoftClose = true;
                        break;
                    case M2Share.nMOVR:
                        n14 = M2Share.GetValNameNo(QuestActionInfo.sParam1);
                        if (n14 >= 0)
                        {
                            switch (n14)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    PlayObject.m_nVal[n14] = M2Share.RandomNumber.Random(QuestActionInfo.nParam2);
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    M2Share.g_Config.GlobalVal[n14 - 100] = M2Share.RandomNumber.Random(QuestActionInfo.nParam2);
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    PlayObject.m_DyVal[n14 - 200] = M2Share.RandomNumber.Random(QuestActionInfo.nParam2);
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    PlayObject.m_nMval[n14 - 300] = M2Share.RandomNumber.Random(QuestActionInfo.nParam2);
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    M2Share.g_Config.GlobaDyMval[n14 - 400] = (short)M2Share.RandomNumber.Random(QuestActionInfo.nParam2);
                                    break;
                                default:
                                    ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sMOVR);
                                    break;
                            }
                            // case
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sMOVR);
                        }
                        break;
                    case M2Share.nEXCHANGEMAP:
                        Envir = M2Share.g_MapManager.FindMap(QuestActionInfo.sParam1);
                        if (Envir != null)
                        {
                            List58 = new ArrayList();
                            M2Share.UserEngine.GetMapRageHuman(Envir, 0, 0, 1000, List58);
                            if (List58.Count > 0)
                            {
                                User = (TPlayObject)List58[0];
                                User.MapRandomMove(this.m_sMapName, 0);
                            }

                            //List58.Free;
                            PlayObject.MapRandomMove(QuestActionInfo.sParam1, 0);
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sEXCHANGEMAP);
                        }
                        break;
                    case M2Share.nRECALLMAP:
                        Envir = M2Share.g_MapManager.FindMap(QuestActionInfo.sParam1);
                        if (Envir != null)
                        {
                            List58 = new ArrayList();
                            M2Share.UserEngine.GetMapRageHuman(Envir, 0, 0, 1000, List58);
                            for (II = 0; II < List58.Count; II++)
                            {
                                User = (TPlayObject)List58[II];
                                User.MapRandomMove(this.m_sMapName, 0);
                                if (II > 20)
                                {
                                    break;
                                }
                            }
                            //List58.Free;
                        }
                        else
                        {
                            ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sRECALLMAP);
                        }
                        break;
                    case M2Share.nADDBATCH:
                        //List1C.Add(QuestActionInfo.sParam1, ((n18) as Object));
                        break;
                    case M2Share.nBATCHDELAY:
                        n18 = QuestActionInfo.nParam1 * 1000;
                        break;
                    case M2Share.nBATCHMOVE:
                        //for (II = 0; II < List1C.Count; II ++ )
                        //{
                        //    PlayObject.SendDelayMsg(this, grobal2.RM_10155, 0, 0, 0, 0, List1C[II], ((int)List1C.Values[II]) + n20);
                        //    n20 += ((int)List1C.Values[II]);
                        //}
                        break;
                    case M2Share.nPLAYDICE:
                        PlayObject.m_sPlayDiceLabel = QuestActionInfo.sParam2;
                        PlayObject.SendMsg(this, grobal2.RM_PLAYDICE, (short)QuestActionInfo.nParam1, HUtil32.MakeLong(HUtil32.MakeWord(PlayObject.m_DyVal[0], PlayObject.m_DyVal[1]), HUtil32.MakeWord(PlayObject.m_DyVal[2], PlayObject.m_DyVal[3])), HUtil32.MakeLong(HUtil32.MakeWord(PlayObject.m_DyVal[4], PlayObject.m_DyVal[5]), HUtil32.MakeWord(PlayObject.m_DyVal[6], PlayObject.m_DyVal[7])), HUtil32.MakeLong(HUtil32.MakeWord(PlayObject.m_DyVal[8], PlayObject.m_DyVal[9]), 0), QuestActionInfo.sParam2);
                        //bo11 = true;
                        break;
                    case M2Share.nADDNAMELIST:
                        GotoLable_AddList(PlayObject.m_sCharName, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nDELNAMELIST:
                        GotoLable_DelList(PlayObject.m_sCharName, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nADDUSERDATE:
                        GotoLable_AddUseDateList(PlayObject.m_sCharName, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nDELUSERDATE:
                        GotoLable_DELUseDateList(PlayObject.m_sCharName, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nADDGUILDLIST:
                        if (PlayObject.m_MyGuild != null)
                        {
                            GotoLable_AddList(PlayObject.m_MyGuild.sGuildName, m_sPath + QuestActionInfo.sParam1);
                        }
                        break;
                    case M2Share.nDELGUILDLIST:
                        if (PlayObject.m_MyGuild != null)
                        {
                            GotoLable_DelList(PlayObject.m_MyGuild.sGuildName, m_sPath + QuestActionInfo.sParam1);
                        }
                        break;
                    case M2Share.nSC_LINEMSG:
                    case M2Share.nSENDMSG:
                        ActionOfLineMsg(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nADDACCOUNTLIST:
                        GotoLable_AddList(PlayObject.m_sUserID, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nDELACCOUNTLIST:
                        GotoLable_DelList(PlayObject.m_sUserID, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nADDIPLIST:
                        GotoLable_AddList(PlayObject.m_sIPaddr, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nDELIPLIST:
                        GotoLable_DelList(PlayObject.m_sIPaddr, m_sPath + QuestActionInfo.sParam1);
                        break;
                    case M2Share.nGOQUEST:
                        GotoLable_GoToQuest(PlayObject, QuestActionInfo.nParam1);
                        break;
                    case M2Share.nENDQUEST:
                        PlayObject.m_Script = null;
                        break;
                    case M2Share.nGOTO:
                        if (!GotoLable_JmpToLable(PlayObject, QuestActionInfo.sParam1))
                        {
                            // ScriptActionError(PlayObject,'',QuestActionInfo,sGOTO);
                            M2Share.MainOutMessage("[脚本死循环] NPC:" + this.m_sCharName + " 位置:" + this.m_sMapName + '(' + this.m_nCurrX.ToString() + ':' + this.m_nCurrY.ToString() + ')' + " 命令:" + M2Share.sGOTO + ' ' + QuestActionInfo.sParam1);
                            result = false;
                            return result;
                        }
                        break;
                    case M2Share.nSC_HAIRCOLOR:
                        break;
                    case M2Share.nSC_WEARCOLOR:
                        break;
                    case M2Share.nSC_HAIRSTYLE:
                        ActionOfChangeHairStyle(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MONRECALL:
                        break;
                    case M2Share.nSC_HORSECALL:
                        break;
                    case M2Share.nSC_HAIRRNDCOL:
                        break;
                    case M2Share.nSC_KILLHORSE:
                        break;
                    case M2Share.nSC_RANDSETDAILYQUEST:
                        break;
                    case M2Share.nSC_RECALLGROUPMEMBERS:
                        ActionOfRecallGroupMembers(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARNAMELIST:
                        ActionOfClearNameList(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MAPTING:
                        ActionOfMapTing(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGELEVEL:
                        ActionOfChangeLevel(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MARRY:
                        ActionOfMarry(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MASTER:
                        ActionOfMaster(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_UNMASTER:
                        ActionOfUnMaster(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_UNMARRY:
                        ActionOfUnMarry(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GETMARRY:
                        ActionOfGetMarry(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GETMASTER:
                        ActionOfGetMaster(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARSKILL:
                        ActionOfClearSkill(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELNOJOBSKILL:
                        ActionOfDelNoJobSkill(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELSKILL:
                        ActionOfDelSkill(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_ADDSKILL:
                        ActionOfAddSkill(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SKILLLEVEL:
                        ActionOfSkillLevel(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEPKPOINT:
                        ActionOfChangePkPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEEXP:
                        ActionOfChangeExp(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEJOB:
                        ActionOfChangeJob(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MISSION:
                        ActionOfMission(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MOBPLACE:
                        ActionOfMobPlace(PlayObject, QuestActionInfo, n34, n38, n3C, n40);
                        break;
                    case M2Share.nSC_SETMEMBERTYPE:
                        ActionOfSetMemberType(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETMEMBERLEVEL:
                        ActionOfSetMemberLevel(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GAMEGOLD:
                        // nSC_SETMEMBERTYPE:   PlayObject.m_nMemberType:=Str_ToInt(QuestActionInfo.sParam1,0);
                        // nSC_SETMEMBERLEVEL:  PlayObject.m_nMemberType:=Str_ToInt(QuestActionInfo.sParam1,0);
                        ActionOfGameGold(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GAMEPOINT:
                        ActionOfGamePoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_OffLine:
                        ActionOfOffLine(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_AUTOADDGAMEGOLD:
                        // 增加挂机
                        ActionOfAutoAddGameGold(PlayObject, QuestActionInfo, n34, n38);
                        break;
                    case M2Share.nSC_AUTOSUBGAMEGOLD:
                        ActionOfAutoSubGameGold(PlayObject, QuestActionInfo, n34, n38);
                        break;
                    case M2Share.nSC_CHANGENAMECOLOR:
                        ActionOfChangeNameColor(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARPASSWORD:
                        ActionOfClearPassword(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_RENEWLEVEL:
                        ActionOfReNewLevel(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_KILLSLAVE:
                        ActionOfKillSlave(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEGENDER:
                        ActionOfChangeGender(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_KILLMONEXPRATE:
                        ActionOfKillMonExpRate(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_POWERRATE:
                        ActionOfPowerRate(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEMODE:
                        ActionOfChangeMode(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CHANGEPERMISSION:
                        ActionOfChangePerMission(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_KILL:
                        ActionOfKill(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_KICK:
                        ActionOfKick(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_BONUSPOINT:
                        ActionOfBonusPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_RESTRENEWLEVEL:
                        ActionOfRestReNewLevel(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELMARRY:
                        ActionOfDelMarry(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELMASTER:
                        ActionOfDelMaster(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CREDITPOINT:
                        ActionOfChangeCreditPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARNEEDITEMS:
                        ActionOfClearNeedItems(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARMAEKITEMS:
                        ActionOfClearMakeItems(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETSENDMSGFLAG:
                        PlayObject.m_boSendMsgFlag = true;
                        break;
                    case M2Share.nSC_UPGRADEITEMS:
                        ActionOfUpgradeItems(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_UPGRADEITEMSEX:
                        ActionOfUpgradeItemsEx(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MONGENEX:
                        ActionOfMonGenEx(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARMAPMON:
                        ActionOfClearMapMon(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETMAPMODE:
                        ActionOfSetMapMode(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_PKZONE:
                        ActionOfPkZone(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_RESTBONUSPOINT:
                        ActionOfRestBonusPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_TAKECASTLEGOLD:
                        ActionOfTakeCastleGold(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_HUMANHP:
                        ActionOfHumanHP(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_HUMANMP:
                        ActionOfHumanMP(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_BUILDPOINT:
                        ActionOfGuildBuildPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELAYGOTO:
                        ActionOfDelayCall(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_AURAEPOINT:
                        ActionOfGuildAuraePoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_STABILITYPOINT:
                        ActionOfGuildstabilityPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_FLOURISHPOINT:
                        ActionOfGuildFlourishPoint(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_OPENMAGICBOX:
                        ActionOfOpenMagicBox(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETRANKLEVELNAME:
                        ActionOfSetRankLevelName(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GMEXECUTE:
                        ActionOfGmExecute(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GUILDCHIEFITEMCOUNT:
                        ActionOfGuildChiefItemCount(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_ADDNAMEDATELIST:
                        ActionOfAddNameDateList(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_DELNAMEDATELIST:
                        ActionOfDelNameDateList(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MOBFIREBURN:
                        ActionOfMobFireBurn(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_MESSAGEBOX:
                        ActionOfMessageBox(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETSCRIPTFLAG:
                        ActionOfSetScriptFlag(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SETAUTOGETEXP:
                        ActionOfAutoGetExp(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_VAR:
                        ActionOfVar(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_LOADVAR:
                        ActionOfLoadVar(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_SAVEVAR:
                        ActionOfSaveVar(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CALCVAR:
                        ActionOfCalcVar(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GUILDRECALL:
                        ActionOfGuildRecall(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GROUPADDLIST:
                        ActionOfGroupAddList(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_CLEARLIST:
                        ActionOfClearList(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GROUPRECALL:
                        ActionOfGroupRecall(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_GROUPMOVEMAP:
                        ActionOfGroupMoveMap(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_REPAIRALL:
                        ActionOfRepairAllItem(PlayObject, QuestActionInfo);
                        break;
                    case M2Share.nSC_QUERYBAGITEMS:// 刷新包裹
                        if ((HUtil32.GetTickCount() - PlayObject.m_dwQueryBagItemsTick) > M2Share.g_Config.dwQueryBagItemsTick)
                        {
                            PlayObject.m_dwQueryBagItemsTick = HUtil32.GetTickCount();
                            PlayObject.ClientQueryBagItems();
                        }
                        else
                        {
                            PlayObject.SysMsg(M2Share.g_sQUERYBAGITEMS, TMsgColor.c_Red, TMsgType.t_Hint);
                        }
                        break;
                    case M2Share.nSC_SETRANDOMNO:
                        while (true)
                        {
                            n2C = M2Share.RandomNumber.Random(999999);
                            if ((n2C >= 1000) && (n2C.ToString() != PlayObject.m_sRandomNo))
                            {
                                PlayObject.m_sRandomNo = n2C.ToString();
                                break;
                            }
                        }
                        break;
                }
            }
            return result;
        }

        public void GotoLable_SendMerChantSayMsg(TPlayObject PlayObject, string sMsg, bool boFlag)
        {
            string s10 = string.Empty;
            string s14 = sMsg;
            int nC = 0;
            while (true)
            {
                if (HUtil32.TagCount(s14, '>') < 1)
                {
                    break;
                }
                s14 = HUtil32.ArrestStringEx(s14, '<', '>', ref s10);
                GetVariableText(PlayObject, ref sMsg, s10);
                nC++;
                if (nC >= 101)
                {
                    break;
                }
            }
            PlayObject.GetScriptLabel(sMsg);
            if (boFlag)
            {
                PlayObject.SendFirstMsg(this, grobal2.RM_MERCHANTSAY, 0, 0, 0, 0, this.m_sCharName + '/' + sMsg);
            }
            else
            {
                PlayObject.SendMsg(this, grobal2.RM_MERCHANTSAY, 0, 0, 0, 0, this.m_sCharName + '/' + sMsg);
            }
        }

        public void GotoLable(TPlayObject PlayObject, string sLabel, bool boExtJmp, string sMsg)
        {
            bool bo11;
            string sSendMsg;
            TScript Script = null;
            TScript Script3C = null;
            TSayingRecord SayingRecord;
            TSayingProcedure SayingProcedure;
            TUserItem UserItem = null;
            string sC = string.Empty;
            if (PlayObject.m_NPC != this)
            {
                PlayObject.m_NPC = null;
                PlayObject.m_Script = null;
                //FillChar(PlayObject.m_nVal, sizeof(PlayObject.m_nVal), '\0');
            }
            if (sLabel.ToLower().CompareTo("@main".ToLower()) == 0)
            {
                for (var i = 0; i < m_ScriptList.Count; i++)
                {
                    Script3C = m_ScriptList[i];
                    if (Script3C.RecordList.TryGetValue(sLabel.ToLower(), out SayingRecord))
                    {
                        Script = Script3C;
                        PlayObject.m_Script = Script;
                        PlayObject.m_NPC = this;
                        break;
                    }
                    if (Script != null)
                    {
                        break;
                    }
                }
            }
            if (Script == null)
            {
                if (PlayObject.m_Script != null)
                {
                    for (var i = m_ScriptList.Count - 1; i >= 0; i--)
                    {
                        if (m_ScriptList[i] == PlayObject.m_Script)
                        {
                            Script = m_ScriptList[i];
                        }
                    }
                }
                if (Script == null)
                {
                    for (var i = m_ScriptList.Count - 1; i >= 0; i--)
                    {
                        if (GotoLable_CheckQuestStatus(PlayObject, m_ScriptList[i]))
                        {
                            Script = m_ScriptList[i];
                            PlayObject.m_Script = Script;
                            PlayObject.m_NPC = this;
                        }
                    }
                }
            }
            // 跳转到指定示签，执行
            if (Script != null)
            {
                if (Script.RecordList.TryGetValue(sLabel.ToLower(), out SayingRecord))
                {
                    if (boExtJmp && SayingRecord.boExtJmp == false)
                    {
                        return;
                    }
                    sSendMsg = "";
                    for (var i = 0; i < SayingRecord.ProcedureList.Count; i++)
                    {
                        SayingProcedure = SayingRecord.ProcedureList[i];
                        bo11 = false;
                        if (GotoLable_QuestCheckCondition(PlayObject, SayingProcedure.ConditionList, ref sC, ref UserItem))
                        {
                            sSendMsg = sSendMsg + SayingProcedure.sSayMsg;
                            if (!GotoLable_QuestActionProcess(PlayObject, SayingProcedure.ActionList, ref sC, ref UserItem))
                            {
                                break;
                            }
                            if (bo11)
                            {
                                GotoLable_SendMerChantSayMsg(PlayObject, sSendMsg, true);
                            }
                        }
                        else
                        {
                            sSendMsg = sSendMsg + SayingProcedure.sElseSayMsg;
                            if (!GotoLable_QuestActionProcess(PlayObject, SayingProcedure.ElseActionList, ref sC, ref UserItem))
                            {
                                break;
                            }
                            if (bo11)
                            {
                                GotoLable_SendMerChantSayMsg(PlayObject, sSendMsg, true);
                            }
                        }
                    }
                    if (sSendMsg != "")
                    {
                        GotoLable_SendMerChantSayMsg(PlayObject, sSendMsg, false);
                    }
                }
            }
        }

        public void GotoLable(TPlayObject PlayObject, string sLabel, bool boExtJmp)
        {
            GotoLable(PlayObject, sLabel, boExtJmp, "");
        }

        public void LoadNPCScript()
        {
            string s08;
            if (m_boIsQuest)
            {
                m_sPath = M2Share.sNpc_def;
                s08 = this.m_sCharName + '-' + this.m_sMapName;
                M2Share.ScriptSystem.LoadNpcScript(this, m_sFilePath, s08);
            }
            else
            {
                m_sPath = m_sFilePath;
                M2Share.ScriptSystem.LoadNpcScript(this, m_sFilePath, this.m_sCharName);
            }
        }

        public override bool Operate(TProcessMessage ProcessMsg)
        {
            return base.Operate(ProcessMsg);
        }

        public override void Run()
        {
            if (this.m_Master != null)// 不允许召唤为宝宝
            {
                this.m_Master = null;
            }
            base.Run();
        }

        private void ScriptActionError(TPlayObject PlayObject, string sErrMsg, TQuestActionInfo QuestActionInfo, string sCmd)
        {
            string sMsg;
            const string sOutMessage = "[脚本错误] %s 脚本命令:%s NPC名称:%s 地图:%s(%d:%d) 参数1:%s 参数2:%s 参数3:%s 参数4:%s 参数5:%s 参数6:%s";
            sMsg = format(sOutMessage, new object[] { sErrMsg, sCmd, this.m_sCharName, this.m_sMapName, this.m_nCurrX, this.m_nCurrY, QuestActionInfo.sParam1, QuestActionInfo.sParam2, QuestActionInfo.sParam3, QuestActionInfo.sParam4, QuestActionInfo.sParam5, QuestActionInfo.sParam6 });
            M2Share.MainOutMessage(sMsg);
        }

        private void ScriptConditionError(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo, string sCmd)
        {
            string sMsg;
            sMsg = "Cmd:" + sCmd + " NPC名称:" + this.m_sCharName + " 地图:" + this.m_sMapName + " 座标:" + this.m_nCurrX.ToString() + ':' + this.m_nCurrY.ToString() + " 参数1:" + QuestConditionInfo.sParam1 + " 参数2:" + QuestConditionInfo.sParam2 + " 参数3:" + QuestConditionInfo.sParam3 + " 参数4:" + QuestConditionInfo.sParam4 + " 参数5:" + QuestConditionInfo.sParam5;
            M2Share.MainOutMessage("[脚本参数不正确] " + sMsg);
        }

        public void SendMsgToUser(TPlayObject PlayObject, string sMsg)
        {
            PlayObject.SendMsg(this, grobal2.RM_MERCHANTSAY, 0, 0, 0, 0, this.m_sCharName + '/' + sMsg);
        }

        public string sub_49ADB8(string sMsg, string sStr, string sText)
        {
            string result;
            string s14;
            string s18;
            int n10 = sMsg.IndexOf(sStr);
            if (n10 > 0)
            {
                s14 = sMsg.Substring(0, n10 - 1);
                s18 = sMsg.Substring(sStr.Length + n10, sMsg.Length - (sStr.Length + n10));
                result = s14 + sText + s18;
            }
            else
            {
                result = sMsg;
            }
            return result;
        }

        public virtual void UserSelect(TPlayObject PlayObject, string sData)
        {
            string sLabel = string.Empty;
            PlayObject.m_nScriptGotoCount = 0;
            if ((sData != "") && (sData[0] == '@'))// 处理脚本命令 @back 返回上级标签内容
            {
                HUtil32.GetValidStr3(sData, ref sLabel, new char[] { '\r' });
                if (PlayObject.m_sScriptCurrLable != sLabel)
                {
                    if (sLabel != M2Share.sBACK)
                    {
                        PlayObject.m_sScriptGoBackLable = PlayObject.m_sScriptCurrLable;
                        PlayObject.m_sScriptCurrLable = sLabel;
                    }
                    else
                    {
                        if (PlayObject.m_sScriptCurrLable != "")
                        {
                            PlayObject.m_sScriptCurrLable = "";
                        }
                        else
                        {
                            PlayObject.m_sScriptGoBackLable = "";
                        }
                    }
                }
            }
        }

        private void ActionOfChangeNameColor(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nColor = QuestActionInfo.nParam1;
            if ((nColor < 0) || (nColor > 255))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGENAMECOLOR);
                return;
            }
            PlayObject.m_btNameColor = (byte)nColor;
            PlayObject.RefNameColor();
        }

        private void ActionOfClearPassword(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_sStoragePwd = "";
            PlayObject.m_boPasswordLocked = false;
        }

        // 挂机的
        // RECALLMOB 怪物名称 等级 叛变时间 变色(0,1) 固定颜色(1 - 7)
        // 变色为0 时固定颜色才起作用
        private void ActionOfRecallmob(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TBaseObject Mon;
            if (QuestActionInfo.nParam3 <= 1)
            {
                Mon = PlayObject.MakeSlave(QuestActionInfo.sParam1, 3, HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0), 100, 10 * 24 * 60 * 60);
            }
            else
            {
                Mon = PlayObject.MakeSlave(QuestActionInfo.sParam1, 3, HUtil32.Str_ToInt(QuestActionInfo.sParam2, 0), 100, QuestActionInfo.nParam3 * 60);
            }
            if (Mon != null)
            {
                if ((QuestActionInfo.sParam4 != "") && (QuestActionInfo.sParam4[1] == '1'))
                {
                    Mon.m_boAutoChangeColor = true;
                }
                else if (QuestActionInfo.nParam5 > 0)
                {
                    Mon.m_boFixColor = true;
                    Mon.m_nFixColorIdx = QuestActionInfo.nParam5 - 1;
                }
            }
        }

        private void ActionOfReNewLevel(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nReLevel = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            int nLevel = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            int nBounsuPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            if ((nReLevel < 0) || (nLevel < 0) || (nBounsuPoint < 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_RENEWLEVEL);
                return;
            }
            if ((PlayObject.m_btReLevel + nReLevel) <= 255)
            {
                PlayObject.m_btReLevel += (byte)nReLevel;
                if (nLevel > 0)
                {
                    PlayObject.m_Abil.Level = (ushort)nLevel;
                }
                if (M2Share.g_Config.boReNewLevelClearExp)
                {
                    PlayObject.m_Abil.Exp = 0;
                }
                PlayObject.m_nBonusPoint += nBounsuPoint;
                PlayObject.SendMsg(PlayObject, grobal2.RM_ADJUST_BONUS, 0, 0, 0, 0, "");
                PlayObject.HasLevelUp(0);
                PlayObject.RefShowName();
            }
        }

        private void ActionOfChangeGender(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nGender;
            nGender = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if (!new ArrayList(new int[] { 0, 1 }).Contains(nGender))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEGENDER);
                return;
            }
            PlayObject.m_btGender = (byte)nGender;
            PlayObject.FeatureChanged();
        }

        private void ActionOfKillSlave(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TBaseObject Slave;
            for (var I = 0; I < PlayObject.m_SlaveList.Count; I++)
            {
                Slave = PlayObject.m_SlaveList[I];
                Slave.m_WAbil.HP = 0;
            }
        }

        private void ActionOfKillMonExpRate(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nRate = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            int nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if ((nRate < 0) || (nTime < 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_KILLMONEXPRATE);
                return;
            }
            PlayObject.m_nKillMonExpRate = nRate;
            // PlayObject.m_dwKillMonExpRateTime:=_MIN(High(Word),nTime);
            PlayObject.m_dwKillMonExpRateTime = nTime;
            if (M2Share.g_Config.boShowScriptActionMsg)
            {
                PlayObject.SysMsg(format(M2Share.g_sChangeKillMonExpRateMsg, new object[] { PlayObject.m_nKillMonExpRate / 100, PlayObject.m_dwKillMonExpRateTime }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfMonGenEx(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sMapName = QuestActionInfo.sParam1;
            int nMapX = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            int nMapY = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            string sMonName = QuestActionInfo.sParam4;
            int nRange = QuestActionInfo.nParam5;
            int nCount = QuestActionInfo.nParam6;
            if ((sMapName == "") || (nMapX <= 0) || (nMapY <= 0) || (sMapName == "") || (nRange <= 0) || (nCount <= 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_MONGENEX);
                return;
            }
            for (var I = 0; I < nCount; I++)
            {
                int nRandX = M2Share.RandomNumber.Random(nRange * 2 + 1) + (nMapX - nRange);
                int nRandY = M2Share.RandomNumber.Random(nRange * 2 + 1) + (nMapY - nRange);
                if (M2Share.UserEngine.RegenMonsterByName(sMapName, (short)nRandX, (short)nRandY, sMonName) == null)
                {
                    // ScriptActionError(PlayObject,'',QuestActionInfo,sSC_MONGENEX);
                    break;
                }
            }
        }

        private void ActionOfOpenMagicBox(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TBaseObject Monster;
            short nX = 0;
            short nY = 0;
            string sMonName = QuestActionInfo.sParam1;
            if (sMonName == "")
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_OPENMAGICBOX);
                return;
            }
            PlayObject.GetFrontPosition(ref nX, ref nY);
            Monster = M2Share.UserEngine.RegenMonsterByName(PlayObject.m_PEnvir.sMapName, nX, nY, sMonName);
            if (Monster == null)
            {
                return;
            }
            Monster.Die();
        }

        private void ActionOfPkZone(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nX;
            int nY;
            TFireBurnEvent FireBurnEvent;
            int nMinX;
            int nMaxX;
            int nMinY;
            int nMaxY;
            int nRange;
            int nType;
            int nTime;
            int nPoint;
            nRange = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            nType = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam4, -1);
            if ((nRange < 0) || (nType < 0) || (nTime < 0) || (nPoint < 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_PKZONE);
                return;
            }
            nMinX = this.m_nCurrX - nRange;
            nMaxX = this.m_nCurrX + nRange;
            nMinY = this.m_nCurrY - nRange;
            nMaxY = this.m_nCurrY + nRange;
            for (nX = nMinX; nX <= nMaxX; nX++)
            {
                for (nY = nMinY; nY <= nMaxY; nY++)
                {
                    if (((nX < nMaxX) && (nY == nMinY)) || ((nY < nMaxY) && (nX == nMinX)) || (nX == nMaxX) || (nY == nMaxY))
                    {
                        FireBurnEvent = new TFireBurnEvent(PlayObject, nX, nY, nType, nTime * 1000, nPoint);
                        M2Share.EventManager.AddEvent(FireBurnEvent);
                    }
                }
            }
        }

        private void ActionOfPowerRate(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nRate;
            int nTime;
            nRate = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            nTime = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if ((nRate < 0) || (nTime < 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_POWERRATE);
                return;
            }
            PlayObject.m_nPowerRate = nRate;
            // PlayObject.m_dwPowerRateTime:=_MIN(High(Word),nTime);
            PlayObject.m_dwPowerRateTime = nTime;
            if (M2Share.g_Config.boShowScriptActionMsg)
            {
                PlayObject.SysMsg(format(M2Share.g_sChangePowerRateMsg, new object[] { PlayObject.m_nPowerRate / 100, PlayObject.m_dwPowerRateTime }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfChangeMode(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nMode;
            bool boOpen;
            nMode = QuestActionInfo.nParam1;
            boOpen = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1) == 1;
            if (nMode >= 1 && nMode <= 3)
            {
                switch (nMode)
                {
                    case 1:
                        PlayObject.m_boAdminMode = boOpen;
                        if (PlayObject.m_boAdminMode)
                        {
                            PlayObject.SysMsg(M2Share.sGameMasterMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        else
                        {
                            PlayObject.SysMsg(M2Share.sReleaseGameMasterMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        break;
                    case 2:
                        PlayObject.m_boSuperMan = boOpen;
                        if (PlayObject.m_boSuperMan)
                        {
                            PlayObject.SysMsg(M2Share.sSupermanMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        else
                        {
                            PlayObject.SysMsg(M2Share.sReleaseSupermanMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        break;
                    case 3:
                        PlayObject.m_boObMode = boOpen;
                        if (PlayObject.m_boObMode)
                        {
                            PlayObject.SysMsg(M2Share.sObserverMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        else
                        {
                            PlayObject.SysMsg(M2Share.g_sReleaseObserverMode, TMsgColor.c_Green, TMsgType.t_Hint);
                        }
                        break;
                }
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEMODE);
            }
        }

        private void ActionOfChangePerMission(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nPermission;
            nPermission = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if (nPermission >= 0 && nPermission <= 10)
            {
                PlayObject.m_btPermission = (byte)nPermission;
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CHANGEPERMISSION);
                return;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sChangePermissionMsg, new byte[] { PlayObject.m_btPermission }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfGiveItem(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            TUserItem UserItem;
            TItem StdItem;
            string sItemName = string.Empty;
            int nItemCount;
            sItemName = QuestActionInfo.sParam1;
            nItemCount = QuestActionInfo.nParam2;
            if ((sItemName == "") || (nItemCount <= 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_GIVE);
                return;
            }
            if (sItemName.ToLower().CompareTo(grobal2.sSTRING_GOLDNAME.ToLower()) == 0)
            {
                PlayObject.IncGold(nItemCount);
                PlayObject.GoldChanged();
                // 0049D2FE
                if (M2Share.g_boGameLogGold)
                {
                    M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + grobal2.sSTRING_GOLDNAME + "\t" + nItemCount.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                }
                return;
            }
            if (M2Share.UserEngine.GetStdItemIdx(sItemName) > 0)
            {
                // if nItemCount > 50 then nItemCount:=50;//11.22 限制数量大小
                if (!(nItemCount >= 1 && nItemCount <= 50))
                {
                    nItemCount = 1;
                }
                // 12.28 改上一条
                for (I = 0; I < nItemCount; I++)
                {
                    // nItemCount 为0时出死循环
                    if (PlayObject.IsEnoughBag())
                    {
                        UserItem = new TUserItem();
                        if (M2Share.UserEngine.CopyToUserItemFromName(sItemName, ref UserItem))
                        {
                            PlayObject.m_ItemList.Add(UserItem);
                            PlayObject.SendAddItem(UserItem);
                            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                            if (StdItem.NeedIdentify == 1)
                            {
                                M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + sItemName + "\t" + UserItem.MakeIndex.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                            }
                        }
                        else
                        {
                            Dispose(UserItem);
                        }
                    }
                    else
                    {
                        UserItem = new TUserItem();
                        if (M2Share.UserEngine.CopyToUserItemFromName(sItemName, ref UserItem))
                        {
                            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                            if (StdItem.NeedIdentify == 1)
                            {
                                M2Share.AddGameDataLog('9' + "\t" + PlayObject.m_sMapName + "\t" + PlayObject.m_nCurrX.ToString() + "\t" + PlayObject.m_nCurrY.ToString() + "\t" + PlayObject.m_sCharName + "\t" + sItemName + "\t" + UserItem.MakeIndex.ToString() + "\t" + '1' + "\t" + this.m_sCharName);
                            }
                            PlayObject.DropItemDown(UserItem, 3, false, PlayObject, null);
                        }
                        Dispose(UserItem);
                    }
                }
            }
        }

        private void ActionOfGmExecute(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sParam1 = QuestActionInfo.sParam1;
            string sParam2 = QuestActionInfo.sParam2;
            string sParam3 = QuestActionInfo.sParam3;
            string sParam4 = QuestActionInfo.sParam4;
            string sParam5 = QuestActionInfo.sParam5;
            string sParam6 = QuestActionInfo.sParam6;
            if (sParam2.ToLower().CompareTo("Self".ToLower()) == 0)
            {
                sParam2 = PlayObject.m_sCharName;
            }
            string sData = format("@{0} {1} {2} {3} {4} {5}", new string[] { sParam1, sParam2, sParam3, sParam4, sParam5, sParam6 });
            byte btOldPermission = PlayObject.m_btPermission;
            try
            {
                PlayObject.m_btPermission = 10;
                PlayObject.ProcessUserLineMsg(sData);
            }
            finally
            {
                PlayObject.m_btPermission = btOldPermission;
            }
        }

        private void ActionOfGuildAuraePoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            char cMethod;
            TGuild Guild;
            int nAuraePoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nAuraePoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_AURAEPOINT);
                return;
            }
            if (PlayObject.m_MyGuild == null)
            {
                PlayObject.SysMsg(M2Share.g_sScriptGuildAuraePointNoGuild, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    Guild.nAurae = nAuraePoint;
                    break;
                case '-':
                    if (Guild.nAurae >= nAuraePoint)
                    {
                        Guild.nAurae = Guild.nAurae - nAuraePoint;
                    }
                    else
                    {
                        Guild.nAurae = 0;
                    }
                    break;
                case '+':
                    if ((int.MaxValue - Guild.nAurae) >= nAuraePoint)
                    {
                        Guild.nAurae = Guild.nAurae + nAuraePoint;
                    }
                    else
                    {
                        Guild.nAurae = int.MaxValue;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptGuildAuraePointMsg, new int[] { Guild.nAurae }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfGuildBuildPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nBuildPoint;
            char cMethod;
            TGuild Guild;
            nBuildPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nBuildPoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_BUILDPOINT);
                return;
            }
            if (PlayObject.m_MyGuild == null)
            {
                PlayObject.SysMsg(M2Share.g_sScriptGuildBuildPointNoGuild, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    Guild.nBuildPoint = nBuildPoint;
                    break;
                case '-':
                    if (Guild.nBuildPoint >= nBuildPoint)
                    {
                        Guild.nBuildPoint = Guild.nBuildPoint - nBuildPoint;
                    }
                    else
                    {
                        Guild.nBuildPoint = 0;
                    }
                    break;
                case '+':
                    if ((int.MaxValue - Guild.nBuildPoint) >= nBuildPoint)
                    {
                        Guild.nBuildPoint = Guild.nBuildPoint + nBuildPoint;
                    }
                    else
                    {
                        Guild.nBuildPoint = int.MaxValue;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptGuildBuildPointMsg, new int[] { Guild.nBuildPoint }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfGuildChiefItemCount(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nItemCount;
            char cMethod;
            TGuild Guild;
            nItemCount = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nItemCount < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_GUILDCHIEFITEMCOUNT);
                return;
            }
            if (PlayObject.m_MyGuild == null)
            {
                PlayObject.SysMsg(M2Share.g_sScriptGuildFlourishPointNoGuild, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    Guild.nChiefItemCount = nItemCount;
                    break;
                case '-':
                    if (Guild.nChiefItemCount >= nItemCount)
                    {
                        Guild.nChiefItemCount = Guild.nChiefItemCount - nItemCount;
                    }
                    else
                    {
                        Guild.nChiefItemCount = 0;
                    }
                    break;
                case '+':
                    if ((int.MaxValue - Guild.nChiefItemCount) >= nItemCount)
                    {
                        Guild.nChiefItemCount = Guild.nChiefItemCount + nItemCount;
                    }
                    else
                    {
                        Guild.nChiefItemCount = int.MaxValue;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptChiefItemCountMsg, new int[] { Guild.nChiefItemCount }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfGuildFlourishPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nFlourishPoint;
            char cMethod;
            TGuild Guild;
            nFlourishPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nFlourishPoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_FLOURISHPOINT);
                return;
            }
            if (PlayObject.m_MyGuild == null)
            {
                PlayObject.SysMsg(M2Share.g_sScriptGuildFlourishPointNoGuild, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    Guild.nFlourishing = nFlourishPoint;
                    break;
                case '-':
                    if (Guild.nFlourishing >= nFlourishPoint)
                    {
                        Guild.nFlourishing = Guild.nFlourishing - nFlourishPoint;
                    }
                    else
                    {
                        Guild.nFlourishing = 0;
                    }
                    break;
                case '+':
                    if ((int.MaxValue - Guild.nFlourishing) >= nFlourishPoint)
                    {
                        Guild.nFlourishing = Guild.nFlourishing + nFlourishPoint;
                    }
                    else
                    {
                        Guild.nFlourishing = int.MaxValue;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptGuildFlourishPointMsg, new int[] { Guild.nFlourishing }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfGuildstabilityPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nStabilityPoint;
            char cMethod;
            TGuild Guild;
            nStabilityPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nStabilityPoint < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_STABILITYPOINT);
                return;
            }
            if (PlayObject.m_MyGuild == null)
            {
                PlayObject.SysMsg(M2Share.g_sScriptGuildStabilityPointNoGuild, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            Guild = PlayObject.m_MyGuild;
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    Guild.nStability = nStabilityPoint;
                    break;
                case '-':
                    if (Guild.nStability >= nStabilityPoint)
                    {
                        Guild.nStability = Guild.nStability - nStabilityPoint;
                    }
                    else
                    {
                        Guild.nStability = 0;
                    }
                    break;
                case '+':
                    if ((int.MaxValue - Guild.nStability) >= nStabilityPoint)
                    {
                        Guild.nStability = Guild.nStability + nStabilityPoint;
                    }
                    else
                    {
                        Guild.nStability = int.MaxValue;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptGuildStabilityPointMsg, new int[] { Guild.nStability }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfHumanHP(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nHP;
            char cMethod;
            nHP = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nHP < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_HUMANHP);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    PlayObject.m_WAbil.HP = (ushort)nHP;
                    break;
                case '-':
                    if (PlayObject.m_WAbil.HP >= nHP)
                    {
                        PlayObject.m_WAbil.HP -= (ushort)nHP;
                    }
                    else
                    {
                        PlayObject.m_WAbil.HP = 0;
                    }
                    break;
                case '+':
                    PlayObject.m_WAbil.HP += (ushort)nHP;
                    if (PlayObject.m_WAbil.HP > PlayObject.m_WAbil.MaxHP)
                    {
                        PlayObject.m_WAbil.HP = PlayObject.m_WAbil.MaxHP;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptChangeHumanHPMsg, new ushort[] { PlayObject.m_WAbil.MaxHP }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfHumanMP(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nMP;
            char cMethod;
            nMP = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nMP < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_HUMANMP);
                return;
            }
            cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    PlayObject.m_WAbil.MP = (ushort)nMP;
                    break;
                case '-':
                    if (PlayObject.m_WAbil.MP >= nMP)
                    {
                        PlayObject.m_WAbil.MP -= (ushort)nMP;
                    }
                    else
                    {
                        PlayObject.m_WAbil.MP = 0;
                    }
                    break;
                case '+':
                    PlayObject.m_WAbil.MP += (ushort)nMP;
                    if (PlayObject.m_WAbil.MP > PlayObject.m_WAbil.MaxMP)
                    {
                        PlayObject.m_WAbil.MP = PlayObject.m_WAbil.MaxMP;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sScriptChangeHumanMPMsg, new ushort[] { PlayObject.m_WAbil.MaxMP }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfKick(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_boKickFlag = true;
        }

        private void ActionOfKill(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nMode;
            nMode = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if (nMode >= 0 && nMode <= 3)
            {
                switch (nMode)
                {
                    case 1:
                        PlayObject.m_boNoItem = true;
                        PlayObject.Die();
                        break;
                    case 2:
                        PlayObject.SetLastHiter(this);
                        PlayObject.Die();
                        break;
                    case 3:
                        PlayObject.m_boNoItem = true;
                        PlayObject.SetLastHiter(this);
                        PlayObject.Die();
                        break;
                    default:
                        PlayObject.Die();
                        break;
                }
            }
            else
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_KILL);
            }
        }

        private void ActionOfBonusPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nBonusPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if ((nBonusPoint < 0) || (nBonusPoint > 10000))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_BONUSPOINT);
                return;
            }
            char cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    //FillChar(PlayObject.m_BonusAbil, sizeof(TNakedAbility), '\0');
                    PlayObject.HasLevelUp(0);
                    PlayObject.m_nBonusPoint = nBonusPoint;
                    PlayObject.SendMsg(PlayObject, grobal2.RM_ADJUST_BONUS, 0, 0, 0, 0, "");
                    break;
                case '-':
                    break;
                case '+':
                    PlayObject.m_nBonusPoint += nBonusPoint;
                    PlayObject.SendMsg(PlayObject, grobal2.RM_ADJUST_BONUS, 0, 0, 0, 0, "");
                    break;
            }
        }

        private void ActionOfDelMarry(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_sDearName = "";
            PlayObject.RefShowName();
        }

        private void ActionOfDelMaster(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_sMasterName = "";
            PlayObject.RefShowName();
        }

        private void ActionOfRestBonusPoint(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nTotleUsePoint;
            nTotleUsePoint = PlayObject.m_BonusAbil.DC + PlayObject.m_BonusAbil.MC + PlayObject.m_BonusAbil.SC + PlayObject.m_BonusAbil.AC + PlayObject.m_BonusAbil.MAC + PlayObject.m_BonusAbil.HP + PlayObject.m_BonusAbil.MP + PlayObject.m_BonusAbil.Hit + PlayObject.m_BonusAbil.Speed + PlayObject.m_BonusAbil.X2;
            //FillChar(PlayObject.m_BonusAbil, sizeof(TNakedAbility), '\0');
            PlayObject.m_nBonusPoint += nTotleUsePoint;
            PlayObject.SendMsg(PlayObject, grobal2.RM_ADJUST_BONUS, 0, 0, 0, 0, "");
            PlayObject.HasLevelUp(0);
            PlayObject.SysMsg("分配点数已复位！！！", TMsgColor.c_Red, TMsgType.t_Hint);
        }

        private void ActionOfRestReNewLevel(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_btReLevel = 0;
            PlayObject.HasLevelUp(0);
        }

        private void ActionOfSetMapMode(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sMapName = QuestActionInfo.sParam1;
            string sMapMode = QuestActionInfo.sParam2;
            string sParam1 = QuestActionInfo.sParam3;
            string sParam2 = QuestActionInfo.sParam4;
            TEnvirnoment Envir = M2Share.g_MapManager.FindMap(sMapName);
            if ((Envir == null) || (sMapMode == ""))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SETMAPMODE);
                return;
            }
            if (sMapMode.ToLower().CompareTo("SAFE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boSAFE = true;
                }
                else
                {
                    Envir.Flag.boSAFE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("DARK".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boDarkness = true;
                }
                else
                {
                    Envir.Flag.boDarkness = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("FIGHT".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boFightZone = true;
                }
                else
                {
                    Envir.Flag.boFightZone = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("FIGHT3".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boFight3Zone = true;
                }
                else
                {
                    Envir.Flag.boFight3Zone = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("DAY".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boDayLight = true;
                }
                else
                {
                    Envir.Flag.boDayLight = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("QUIZ".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boQUIZ = true;
                }
                else
                {
                    Envir.Flag.boQUIZ = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NORECONNECT".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNORECONNECT = true;
                    Envir.Flag.sNoReConnectMap = sParam1;
                }
                else
                {
                    Envir.Flag.boNORECONNECT = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("MUSIC".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boMUSIC = true;
                    Envir.Flag.nMUSICID = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boMUSIC = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("EXPRATE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boEXPRATE = true;
                    Envir.Flag.nEXPRATE = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boEXPRATE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("PKWINLEVEL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boPKWINLEVEL = true;
                    Envir.Flag.nPKWINLEVEL = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boPKWINLEVEL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("PKWINEXP".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boPKWINEXP = true;
                    Envir.Flag.nPKWINEXP = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boPKWINEXP = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("PKLOSTLEVEL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boPKLOSTLEVEL = true;
                    Envir.Flag.nPKLOSTLEVEL = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boPKLOSTLEVEL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("PKLOSTEXP".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boPKLOSTEXP = true;
                    Envir.Flag.nPKLOSTEXP = HUtil32.Str_ToInt(sParam1, -1);
                }
                else
                {
                    Envir.Flag.boPKLOSTEXP = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("DECHP".ToLower()) == 0)
            {
                if ((sParam1 != "") && (sParam2 != ""))
                {
                    Envir.Flag.boDECHP = true;
                    Envir.Flag.nDECHPTIME = HUtil32.Str_ToInt(sParam1, -1);
                    Envir.Flag.nDECHPPOINT = HUtil32.Str_ToInt(sParam2, -1);
                }
                else
                {
                    Envir.Flag.boDECHP = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("DECGAMEGOLD".ToLower()) == 0)
            {
                if ((sParam1 != "") && (sParam2 != ""))
                {
                    Envir.Flag.boDECGAMEGOLD = true;
                    Envir.Flag.nDECGAMEGOLDTIME = HUtil32.Str_ToInt(sParam1, -1);
                    Envir.Flag.nDECGAMEGOLD = HUtil32.Str_ToInt(sParam2, -1);
                }
                else
                {
                    Envir.Flag.boDECGAMEGOLD = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("RUNHUMAN".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boRUNHUMAN = true;
                }
                else
                {
                    Envir.Flag.boRUNHUMAN = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("RUNMON".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boRUNMON = true;
                }
                else
                {
                    Envir.Flag.boRUNMON = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NEEDHOLE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNEEDHOLE = true;
                }
                else
                {
                    Envir.Flag.boNEEDHOLE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NORECALL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNORECALL = true;
                }
                else
                {
                    Envir.Flag.boNORECALL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOGUILDRECALL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOGUILDRECALL = true;
                }
                else
                {
                    Envir.Flag.boNOGUILDRECALL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NODEARRECALL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNODEARRECALL = true;
                }
                else
                {
                    Envir.Flag.boNODEARRECALL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOMASTERRECALL".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOMASTERRECALL = true;
                }
                else
                {
                    Envir.Flag.boNOMASTERRECALL = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NORANDOMMOVE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNORANDOMMOVE = true;
                }
                else
                {
                    Envir.Flag.boNORANDOMMOVE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NODRUG".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNODRUG = true;
                }
                else
                {
                    Envir.Flag.boNODRUG = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("MINE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boMINE = true;
                }
                else
                {
                    Envir.Flag.boMINE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("MINE2".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boMINE2 = true;
                }
                else
                {
                    Envir.Flag.boMINE2 = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOTHROWITEM".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOTHROWITEM = true;
                }
                else
                {
                    Envir.Flag.boNOTHROWITEM = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NODROPITEM".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNODROPITEM = true;
                }
                else
                {
                    Envir.Flag.boNODROPITEM = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOPOSITIONMOVE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOPOSITIONMOVE = true;
                }
                else
                {
                    Envir.Flag.boNOPOSITIONMOVE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOHORSE".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOHORSE = true;
                }
                else
                {
                    Envir.Flag.boNOHORSE = false;
                }
            }
            else if (sMapMode.ToLower().CompareTo("NOCHAT".ToLower()) == 0)
            {
                if (sParam1 != "")
                {
                    Envir.Flag.boNOCHAT = true;
                }
                else
                {
                    Envir.Flag.boNOCHAT = false;
                }
            }
        }

        private void ActionOfSetMemberLevel(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nLevel = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SETMEMBERLEVEL);
                return;
            }
            char cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    PlayObject.m_nMemberLevel = nLevel;
                    break;
                case '-':
                    PlayObject.m_nMemberLevel -= nLevel;
                    if (PlayObject.m_nMemberLevel < 0)
                    {
                        PlayObject.m_nMemberLevel = 0;
                    }
                    break;
                case '+':
                    PlayObject.m_nMemberLevel += nLevel;
                    if (PlayObject.m_nMemberLevel > 65535)
                    {
                        PlayObject.m_nMemberLevel = 65535;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sChangeMemberLevelMsg, new int[] { PlayObject.m_nMemberLevel }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private void ActionOfSetMemberType(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nType = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            if (nType < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SETMEMBERTYPE);
                return;
            }
            char cMethod = QuestActionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    PlayObject.m_nMemberType = nType;
                    break;
                case '-':
                    PlayObject.m_nMemberType -= nType;
                    if (PlayObject.m_nMemberType < 0)
                    {
                        PlayObject.m_nMemberType = 0;
                    }
                    break;
                case '+':
                    PlayObject.m_nMemberType += nType;
                    if (PlayObject.m_nMemberType > 65535)
                    {
                        PlayObject.m_nMemberType = 65535;
                    }
                    break;
            }
            if (M2Share.g_Config.boShowScriptActionMsg)
            {

                PlayObject.SysMsg(format(M2Share.g_sChangeMemberTypeMsg, new int[] { PlayObject.m_nMemberType }), TMsgColor.c_Green, TMsgType.t_Hint);
            }
        }

        private bool ConditionOfCheckRangeMonCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            TBaseObject BaseObject;
            bool result = false;
            string sMapName = QuestConditionInfo.sParam1;
            int nX = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            int nY = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            int nRange = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, -1);
            char cMethod = QuestConditionInfo.sParam5[1];
            int nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam6, -1);
            TEnvirnoment Envir = M2Share.g_MapManager.FindMap(sMapName);
            if ((Envir == null) || (nX < 0) || (nY < 0) || (nRange < 0) || (nCount < 0))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKRANGEMONCOUNT);
                return result;
            }
            IList<TBaseObject> MonList = new List<TBaseObject>();
            int nMapRangeCount = Envir.GetRangeBaseObject(nX, nY, nRange, true, MonList);
            for (var i = MonList.Count - 1; i >= 0; i--)
            {
                BaseObject = MonList[i];
                if ((BaseObject.m_btRaceServer < grobal2.RC_ANIMAL) || (BaseObject.m_btRaceServer == grobal2.RC_ARCHERGUARD) || (BaseObject.m_Master != null))
                {
                    MonList.RemoveAt(i);
                }
            }
            nMapRangeCount = MonList.Count;
            switch (cMethod)
            {
                case '=':
                    if (nMapRangeCount == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nMapRangeCount > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nMapRangeCount < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nMapRangeCount >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckReNewLevel(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result = false;
            int nLevel = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKLEVELEX);
                return result;
            }
            char cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_btReLevel == nLevel)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_btReLevel > nLevel)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_btReLevel < nLevel)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_btReLevel >= nLevel)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckSlaveLevel(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            char cMethod;
            TBaseObject BaseObject;
            int nSlaveLevel;
            bool result = false;
            int nLevel = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nLevel < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKLEVELEX);
                return result;
            }
            nSlaveLevel = -1;
            for (var I = 0; I < PlayObject.m_SlaveList.Count; I++)
            {
                BaseObject = PlayObject.m_SlaveList[I];
                if (BaseObject.m_Abil.Level > nSlaveLevel)
                {
                    nSlaveLevel = BaseObject.m_Abil.Level;
                }
            }
            if (nSlaveLevel < 0)
            {
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nSlaveLevel == nLevel)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nSlaveLevel > nLevel)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nSlaveLevel < nLevel)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nSlaveLevel >= nLevel)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckUseItem(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result = false;
            int nWhere = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, -1);
            if ((nWhere < 0) || (nWhere > PlayObject.m_UseItems.GetUpperBound(0)))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKUSEITEM);
                return result;
            }
            if (PlayObject.m_UseItems[nWhere].wIndex > 0)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckVar(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            string sType;
            string sVarName;
            string sVarValue;
            int nVarValue;
            string sName = string.Empty;
            string sMethod;
            char cMethod;
            TDynamicVar DynamicVar;
            bool boFoundVar = false;
            IList<TDynamicVar> DynamicVarList;
            result = false;
            sType = QuestConditionInfo.sParam1;
            sVarName = QuestConditionInfo.sParam2;
            sMethod = QuestConditionInfo.sParam3;
            nVarValue = HUtil32.Str_ToInt(QuestConditionInfo.sParam4, 0);
            sVarValue = QuestConditionInfo.sParam4;
            if ((sType == "") || (sVarName == "") || (sMethod == ""))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKVAR);
                return result;
            }
            cMethod = sMethod[1];
            DynamicVarList = GetDynamicVarList(PlayObject, sType, ref sName);
            if (DynamicVarList == null)
            {
                // ,format(sVarTypeError,[sType])
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKVAR);
                return result;
            }
            for (var i = 0; i < DynamicVarList.Count; i++)
            {
                DynamicVar = DynamicVarList[i];
                if (DynamicVar.sName.ToLower().CompareTo(sVarName.ToLower()) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.VInteger:
                            switch (cMethod)
                            {
                                case '=':
                                    if (DynamicVar.nInternet == nVarValue)
                                    {
                                        result = true;
                                    }
                                    break;
                                case '>':
                                    if (DynamicVar.nInternet > nVarValue)
                                    {
                                        result = true;
                                    }
                                    break;
                                case '<':
                                    if (DynamicVar.nInternet < nVarValue)
                                    {
                                        result = true;
                                    }
                                    break;
                                default:
                                    if (DynamicVar.nInternet >= nVarValue)
                                    {
                                        result = true;
                                    }
                                    break;
                            }
                            break;
                        case TVarType.VString:
                            break;
                    }
                    boFoundVar = true;
                    break;
                }
            }
            if (!boFoundVar)
            {
                // format(sVarFound,[sVarName,sType]),
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKVAR);
            }
            return result;
        }

        private bool ConditionOfHaveMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (PlayObject.m_sMasterName != "")
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfPoseHaveMaster(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            TBaseObject PoseHuman;
            result = false;
            PoseHuman = PlayObject.GetPoseCreate();
            if ((PoseHuman != null) && (PoseHuman.m_btRaceServer == grobal2.RC_PLAYOBJECT))
            {
                if (((TPlayObject)PoseHuman).m_sMasterName != "")
                {
                    result = true;
                }
            }
            return result;
        }

        private void ActionOfUnMaster(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TPlayObject PoseHuman;
            string sMsg;
            if (PlayObject.m_sMasterName == "")
            {
                GotoLable(PlayObject, "@ExeMasterFail", false);
                return;
            }
            PoseHuman = (TPlayObject)PlayObject.GetPoseCreate();
            if (PoseHuman == null)
            {
                GotoLable(PlayObject, "@UnMasterCheckDir", false);
            }
            if (PoseHuman != null)
            {
                if (QuestActionInfo.sParam1 == "")
                {
                    if (PoseHuman.m_btRaceServer != grobal2.RC_PLAYOBJECT)
                    {
                        GotoLable(PlayObject, "@UnMasterTypeErr", false);
                        return;
                    }
                    if (PoseHuman.GetPoseCreate() == PlayObject)
                    {
                        if (PlayObject.m_sMasterName == PoseHuman.m_sCharName)
                        {
                            if (PlayObject.m_boMaster)
                            {
                                GotoLable(PlayObject, "@UnIsMaster", false);
                                return;
                            }
                            if (PlayObject.m_sMasterName != PoseHuman.m_sCharName)
                            {
                                GotoLable(PlayObject, "@UnMasterError", false);
                                return;
                            }
                            GotoLable(PlayObject, "@StartUnMaster", false);
                            GotoLable(PoseHuman, "@WateUnMaster", false);
                            return;
                        }
                    }
                }
            }
            // sREQUESTUNMARRY
            if (QuestActionInfo.sParam1.ToLower().CompareTo("REQUESTUNMASTER".ToLower()) == 0)
            {
                if (QuestActionInfo.sParam2 == "")
                {
                    if (PoseHuman != null)
                    {
                        PlayObject.m_boStartUnMaster = true;
                        if (PlayObject.m_boStartUnMaster && PoseHuman.m_boStartUnMaster)
                        {
                            sMsg = M2Share.g_sNPCSayUnMasterOKMsg.Replace("%n", this.m_sCharName);
                            sMsg = sMsg.Replace("%s", PlayObject.m_sCharName);
                            sMsg = sMsg.Replace("%d", PoseHuman.m_sCharName);
                            M2Share.UserEngine.SendBroadCastMsg(sMsg, TMsgType.t_Say);
                            PlayObject.m_sMasterName = "";
                            PoseHuman.m_sMasterName = "";
                            PlayObject.m_boStartUnMaster = false;
                            PoseHuman.m_boStartUnMaster = false;
                            PlayObject.RefShowName();
                            PoseHuman.RefShowName();
                            GotoLable(PlayObject, "@UnMasterEnd", false);
                            GotoLable(PoseHuman, "@UnMasterEnd", false);
                        }
                        else
                        {
                            GotoLable(PlayObject, "@WateUnMaster", false);
                            GotoLable(PoseHuman, "@RevUnMaster", false);
                        }
                    }
                    return;
                }
                else
                {
                    // 强行出师
                    if (QuestActionInfo.sParam2.ToLower().CompareTo("FORCE".ToLower()) == 0)
                    {
                        sMsg = M2Share.g_sNPCSayForceUnMasterMsg.Replace("%n", this.m_sCharName);
                        sMsg = sMsg.Replace("%s", PlayObject.m_sCharName);
                        sMsg = sMsg.Replace("%d", PlayObject.m_sMasterName);
                        M2Share.UserEngine.SendBroadCastMsg(sMsg, TMsgType.t_Say);
                        PoseHuman = M2Share.UserEngine.GetPlayObject(PlayObject.m_sMasterName);
                        if (PoseHuman != null)
                        {
                            PoseHuman.m_sMasterName = "";
                            PoseHuman.RefShowName();
                        }
                        else
                        {
                            try
                            {
                                M2Share.g_UnForceMasterList.Add(PlayObject.m_sMasterName);
                                M2Share.SaveUnForceMasterList();
                            }
                            finally
                            {
                            }
                        }
                        PlayObject.m_sMasterName = "";
                        GotoLable(PlayObject, "@UnMasterEnd", false);
                        PlayObject.RefShowName();
                    }
                    return;
                }
            }
        }

        private bool ConditionOfCheckCastleGold(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nGold;
            result = false;
            nGold = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if ((nGold < 0) || (this.m_Castle == null))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKCASTLEGOLD);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (this.m_Castle.m_nTotalGold == nGold)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (this.m_Castle.m_nTotalGold > nGold)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (this.m_Castle.m_nTotalGold < nGold)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (this.m_Castle.m_nTotalGold >= nGold)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckContribution(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nContribution;
            char cMethod;
            result = false;
            nContribution = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nContribution < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKCONTRIBUTION);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_wContribution == nContribution)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_wContribution > nContribution)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_wContribution < nContribution)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_wContribution >= nContribution)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckCreditPoint(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nCreditPoint;
            char cMethod;
            result = false;
            nCreditPoint = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nCreditPoint < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKCREDITPOINT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_btCreditPoint == nCreditPoint)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_btCreditPoint > nCreditPoint)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_btCreditPoint < nCreditPoint)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_btCreditPoint >= nCreditPoint)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private void ActionOfClearNeedItems(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            int nNeed;
            TUserItem UserItem;
            TItem StdItem;
            nNeed = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            if (nNeed < 0)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CLEARNEEDITEMS);
                return;
            }
            for (I = PlayObject.m_ItemList.Count - 1; I >= 0; I--)
            {
                UserItem = PlayObject.m_ItemList[I];
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if ((StdItem != null) && (StdItem.Need == nNeed))
                {
                    PlayObject.SendDelItems(UserItem);

                    Dispose(UserItem);
                    PlayObject.m_ItemList.RemoveAt(I);
                }
            }
            for (I = PlayObject.m_StorageItemList.Count - 1; I >= 0; I--)
            {
                UserItem = PlayObject.m_StorageItemList[I];
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if ((StdItem != null) && (StdItem.Need == nNeed))
                {

                    Dispose(UserItem);
                    PlayObject.m_StorageItemList.RemoveAt(I);
                }
            }
        }

        private void ActionOfClearMakeItems(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            int nMakeIndex;
            string sItemName = string.Empty;
            TUserItem UserItem;
            TItem StdItem;
            bool boMatchName;
            sItemName = QuestActionInfo.sParam1;
            nMakeIndex = QuestActionInfo.nParam2;
            boMatchName = QuestActionInfo.sParam3 == "1";
            if ((sItemName == "") || (nMakeIndex <= 0))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CLEARMAKEITEMS);
                return;
            }
            for (I = PlayObject.m_ItemList.Count - 1; I >= 0; I--)
            {
                UserItem = PlayObject.m_ItemList[I];
                if (UserItem.MakeIndex != nMakeIndex)
                {
                    continue;
                }
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if (!boMatchName || ((StdItem != null) && (StdItem.Name.ToLower().CompareTo(sItemName.ToLower()) == 0)))
                {
                    PlayObject.SendDelItems(UserItem);

                    Dispose(UserItem);
                    PlayObject.m_ItemList.RemoveAt(I);
                }
            }
            for (I = PlayObject.m_StorageItemList.Count - 1; I >= 0; I--)
            {
                UserItem = PlayObject.m_ItemList[I];
                if (UserItem.MakeIndex != nMakeIndex)
                {
                    continue;
                }
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if (!boMatchName || ((StdItem != null) && (StdItem.Name.ToLower().CompareTo(sItemName.ToLower()) == 0)))
                {

                    Dispose(UserItem);
                    PlayObject.m_StorageItemList.RemoveAt(I);
                }
            }
            for (I = PlayObject.m_UseItems.GetLowerBound(0); I <= PlayObject.m_UseItems.GetUpperBound(0); I++)
            {
                UserItem = PlayObject.m_UseItems[I];
                if (UserItem.MakeIndex != nMakeIndex)
                {
                    continue;
                }
                StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
                if (!boMatchName || ((StdItem != null) && (StdItem.Name.ToLower().CompareTo(sItemName.ToLower()) == 0)))
                {
                    UserItem.wIndex = 0;
                }
            }
        }

        public virtual void SendCustemMsg(TPlayObject PlayObject, string sMsg)
        {
            if (!M2Share.g_Config.boSendCustemMsg)
            {
                PlayObject.SysMsg(M2Share.g_sSendCustMsgCanNotUseNowMsg, TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            if (PlayObject.m_boSendMsgFlag)
            {
                PlayObject.m_boSendMsgFlag = false;
                M2Share.UserEngine.SendBroadCastMsg(PlayObject.m_sCharName + ": " + sMsg, TMsgType.t_Cust);
            }
            else
            {
            }
        }

        private bool ConditionOfCheckOfGuild(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = false;
            if (QuestConditionInfo.sParam1 == "")
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKOFGUILD);
                return result;
            }
            if (PlayObject.m_MyGuild != null)
            {
                if (PlayObject.m_MyGuild.sGuildName.ToLower().CompareTo(QuestConditionInfo.sParam1.ToLower()) == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        private bool ConditionOfCheckOnlineLongMin(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            char cMethod;
            int nOnlineMin;
            int nOnlineTime;
            result = false;
            nOnlineMin = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nOnlineMin < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_ONLINELONGMIN);
                return result;
            }
            nOnlineTime = (HUtil32.GetTickCount() - PlayObject.m_dwLogonTick) / 60000;
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (nOnlineTime == nOnlineMin)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nOnlineTime > nOnlineMin)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nOnlineTime < nOnlineMin)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nOnlineTime >= nOnlineMin)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckPasswordErrorCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nErrorCount;
            char cMethod;
            result = false;
            nErrorCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam2, -1);
            if (nErrorCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_PASSWORDERRORCOUNT);
                return result;
            }
            cMethod = QuestConditionInfo.sParam1[1];
            switch (cMethod)
            {
                case '=':
                    if (PlayObject.m_btPwdFailCount == nErrorCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (PlayObject.m_btPwdFailCount > nErrorCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (PlayObject.m_btPwdFailCount < nErrorCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (PlayObject.m_btPwdFailCount >= nErrorCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfIsLockPassword(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = PlayObject.m_boPasswordLocked;
            return result;
        }

        private bool ConditionOfIsLockStorage(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            result = !PlayObject.m_boCanGetBackItem;
            return result;
        }

        private bool ConditionOfCheckPayMent(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nPayMent;
            result = false;
            nPayMent = HUtil32.Str_ToInt(QuestConditionInfo.sParam1, -1);
            if (nPayMent < 1)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKPAYMENT);
                return result;
            }
            if (PlayObject.m_nPayMent == nPayMent)
            {
                result = true;
            }
            return result;
        }

        private bool ConditionOfCheckSlaveName(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            string sSlaveName;
            TBaseObject BaseObject;
            result = false;
            sSlaveName = QuestConditionInfo.sParam1;
            if (sSlaveName == "")
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKSLAVENAME);
                return result;
            }
            for (I = 0; I < PlayObject.m_SlaveList.Count; I++)
            {
                BaseObject = PlayObject.m_SlaveList[I];
                if (sSlaveName.ToLower().CompareTo(BaseObject.m_sCharName.ToLower()) == 0)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private void ActionOfUpgradeItems(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nRate;
            int nWhere;
            int nValType;
            int nPoint;
            int nAddPoint;
            TUserItem UserItem;
            TItem StdItem;
            nWhere = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            nRate = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            if ((nWhere < 0) || (nWhere > PlayObject.m_UseItems.GetUpperBound(0)) || (nRate < 0) || (nPoint < 0) || (nPoint > 255))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_UPGRADEITEMS);
                return;
            }
            UserItem = PlayObject.m_UseItems[nWhere];
            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
            if ((UserItem.wIndex <= 0) || (StdItem == null))
            {
                PlayObject.SysMsg("你身上没有戴指定物品！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            nRate = M2Share.RandomNumber.Random(nRate);
            nPoint = M2Share.RandomNumber.Random(nPoint);
            nValType = M2Share.RandomNumber.Random(14);
            if (nRate != 0)
            {
                PlayObject.SysMsg("装备升级失败！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            if (nValType == 14)
            {
                nAddPoint = nPoint * 1000;
                if (UserItem.DuraMax + nAddPoint > ushort.MaxValue)
                {
                    nAddPoint = ushort.MaxValue - UserItem.DuraMax;
                }
                UserItem.DuraMax = (ushort)(UserItem.DuraMax + nAddPoint);
            }
            else
            {
                nAddPoint = nPoint;
                if (UserItem.btValue[nValType] + nAddPoint > byte.MaxValue)
                {
                    nAddPoint = byte.MaxValue - UserItem.btValue[nValType];
                }
                UserItem.btValue[nValType] = (byte)(UserItem.btValue[nValType] + nAddPoint);
            }
            PlayObject.SendUpdateItem(UserItem);
            PlayObject.SysMsg("装备升级成功", TMsgColor.c_Green, TMsgType.t_Hint);
            PlayObject.SysMsg(StdItem.Name + ": " + UserItem.Dura + '/' + UserItem.DuraMax.ToString() + '/' + UserItem.btValue[0].ToString() + '/' + UserItem.btValue[1].ToString() + '/' + UserItem.btValue[2].ToString() + '/' + UserItem.btValue[3].ToString() + '/' + UserItem.btValue[4].ToString() + '/' + UserItem.btValue[5].ToString() + '/' + UserItem.btValue[6].ToString() + '/' + UserItem.btValue[7].ToString() + '/' + UserItem.btValue[8].ToString() + '/' + UserItem.btValue[9].ToString() + '/' + UserItem.btValue[10].ToString() + '/' + UserItem.btValue[11].ToString() + '/' + UserItem.btValue[12].ToString() + '/' + UserItem.btValue[13].ToString(), TMsgColor.c_Blue, TMsgType.t_Hint);
        }

        private void ActionOfUpgradeItemsEx(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int nRate;
            int nWhere;
            int nValType;
            int nPoint;
            int nAddPoint;
            TUserItem UserItem;
            TItem StdItem;
            int nUpgradeItemStatus;
            int nRatePoint;
            nWhere = HUtil32.Str_ToInt(QuestActionInfo.sParam1, -1);
            nValType = HUtil32.Str_ToInt(QuestActionInfo.sParam2, -1);
            nRate = HUtil32.Str_ToInt(QuestActionInfo.sParam3, -1);
            nPoint = HUtil32.Str_ToInt(QuestActionInfo.sParam4, -1);
            nUpgradeItemStatus = HUtil32.Str_ToInt(QuestActionInfo.sParam5, -1);
            if ((nValType < 0) || (nValType > 14) || (nWhere < 0) || (nWhere > PlayObject.m_UseItems.GetUpperBound(0)) || (nRate < 0) || (nPoint < 0) || (nPoint > 255))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_UPGRADEITEMSEX);
                return;
            }
            UserItem = PlayObject.m_UseItems[nWhere];
            StdItem = M2Share.UserEngine.GetStdItem(UserItem.wIndex);
            if ((UserItem.wIndex <= 0) || (StdItem == null))
            {
                PlayObject.SysMsg("你身上没有戴指定物品！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                return;
            }
            nRatePoint = M2Share.RandomNumber.Random(nRate * 10);
            nPoint = HUtil32._MAX(1, M2Share.RandomNumber.Random(nPoint));
            if (!(nRatePoint >= 0 && nRatePoint <= 10))
            {
                switch (nUpgradeItemStatus)
                {
                    case 0:
                        PlayObject.SysMsg("装备升级未成功！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                        break;
                    case 1:
                        PlayObject.SendDelItems(UserItem);
                        UserItem.wIndex = 0;
                        PlayObject.SysMsg("装备破碎！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                        break;
                    case 2:
                        PlayObject.SysMsg("装备升级失败，装备属性恢复默认！！！", TMsgColor.c_Red, TMsgType.t_Hint);
                        if (nValType != 14)
                        {
                            UserItem.btValue[nValType] = 0;
                        }
                        break;
                }
                return;
            }
            if (nValType == 14)
            {
                nAddPoint = nPoint * 1000;
                if (UserItem.DuraMax + nAddPoint > ushort.MaxValue)
                {
                    nAddPoint = ushort.MaxValue - UserItem.DuraMax;
                }
                UserItem.DuraMax = (ushort)(UserItem.DuraMax + nAddPoint);
            }
            else
            {
                nAddPoint = nPoint;
                if (UserItem.btValue[nValType] + nAddPoint > byte.MaxValue)
                {
                    nAddPoint = byte.MaxValue - UserItem.btValue[nValType];
                }
                UserItem.btValue[nValType] = (byte)(UserItem.btValue[nValType] + nAddPoint);
            }
            PlayObject.SendUpdateItem(UserItem);
            PlayObject.SysMsg("装备升级成功", TMsgColor.c_Green, TMsgType.t_Hint);
            PlayObject.SysMsg(StdItem.Name + ": " + UserItem.Dura + '/' + UserItem.DuraMax.ToString() + '-' + UserItem.btValue[0].ToString() + '/' + UserItem.btValue[1].ToString() + '/' + UserItem.btValue[2].ToString() + '/' + UserItem.btValue[3].ToString() + '/' + UserItem.btValue[4].ToString() + '/' + UserItem.btValue[5].ToString() + '/' + UserItem.btValue[6].ToString() + '/' + UserItem.btValue[7].ToString() + '/' + UserItem.btValue[8].ToString() + '/' + UserItem.btValue[9].ToString() + '/' + UserItem.btValue[10].ToString() + '/' + UserItem.btValue[11].ToString() + '/' + UserItem.btValue[12].ToString() + '/' + UserItem.btValue[13].ToString(), TMsgColor.c_Blue, TMsgType.t_Hint);
        }

        // 声明变量
        // VAR 数据类型(Integer String) 类型(HUMAN GUILD GLOBAL) 变量值
        private void ActionOfVar(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            string sType;
            TVarType VarType;
            string sVarName;
            string sVarValue;
            int nVarValue;
            string sName = string.Empty;
            TDynamicVar DynamicVar;
            bool boFoundVar;
            IList<TDynamicVar> DynamicVarList;
            const string sVarFound = "变量%s已存在，变量类型:%s";
            const string sVarTypeError = "变量类型错误，错误类型:%s 当前支持类型(HUMAN、GUILD、GLOBAL)";
            sType = QuestActionInfo.sParam2;
            sVarName = QuestActionInfo.sParam3;
            sVarValue = QuestActionInfo.sParam4;
            nVarValue = HUtil32.Str_ToInt(QuestActionInfo.sParam4, 0);
            VarType = TVarType.vNone;
            if (QuestActionInfo.sParam1.ToLower().CompareTo("Integer".ToLower()) == 0)
            {
                VarType = TVarType.VInteger;
            }
            if (QuestActionInfo.sParam1.ToLower().CompareTo("String".ToLower()) == 0)
            {
                VarType = TVarType.VString;
            }
            if ((sType == "") || (sVarName == "") || (VarType == TVarType.vNone))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_VAR);
                return;
            }
            DynamicVar = new TDynamicVar();
            DynamicVar.sName = sVarName;
            DynamicVar.VarType = VarType;
            DynamicVar.nInternet = nVarValue;
            DynamicVar.sString = sVarValue;
            boFoundVar = false;
            DynamicVarList = GetDynamicVarList(PlayObject, sType, ref sName);
            if (DynamicVarList == null)
            {

                Dispose(DynamicVar);

                ScriptActionError(PlayObject, format(sVarTypeError, new string[] { sType }), QuestActionInfo, M2Share.sSC_VAR);
                return;
            }
            for (I = 0; I < DynamicVarList.Count; I++)
            {
                if (DynamicVarList[I].sName.ToLower().CompareTo(sVarName.ToLower()) == 0)
                {
                    boFoundVar = true;
                    break;
                }
            }
            if (!boFoundVar)
            {
                DynamicVarList.Add(DynamicVar);
            }
            else
            {

                ScriptActionError(PlayObject, format(sVarFound, new string[] { sVarName, sType }), QuestActionInfo, M2Share.sSC_VAR);
            }
        }

        // 读取变量值
        // LOADVAR 变量类型 变量名 文件名
        private void ActionOfLoadVar(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            int I;
            string sType;
            string sVarName;
            string sFileName;
            string sName = string.Empty;
            TDynamicVar DynamicVar = null;
            bool boFoundVar;
            IList<TDynamicVar> DynamicVarList;
            IniFile IniFile;
            const string sVarFound = "变量%s不存在，变量类型:%s";
            const string sVarTypeError = "变量类型错误，错误类型:%s 当前支持类型(HUMAN、GUILD、GLOBAL)";
            sType = QuestActionInfo.sParam1;
            sVarName = QuestActionInfo.sParam2;
            sFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestActionInfo.sParam3;
            if ((sType == "") || (sVarName == "") || !File.Exists(sFileName))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_LOADVAR);
                return;
            }
            boFoundVar = false;
            DynamicVarList = GetDynamicVarList(PlayObject, sType, ref sName);
            if (DynamicVarList == null)
            {
                Dispose(DynamicVar);
                ScriptActionError(PlayObject, format(sVarTypeError, new string[] { sType }), QuestActionInfo, M2Share.sSC_VAR);
                return;
            }
            IniFile = new IniFile(sFileName);
            for (I = 0; I < DynamicVarList.Count; I++)
            {
                DynamicVar = DynamicVarList[I];
                if (DynamicVar.sName.ToLower().CompareTo(sVarName.ToLower()) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.VInteger:

                            DynamicVar.nInternet = IniFile.ReadInteger(sName, DynamicVar.sName, 0);
                            break;
                        case TVarType.VString:

                            DynamicVar.sString = IniFile.ReadString(sName, DynamicVar.sName, "");
                            break;
                    }
                    boFoundVar = true;
                    break;
                }
            }
            if (!boFoundVar)
            {
                ScriptActionError(PlayObject, format(sVarFound, new string[] { sVarName, sType }), QuestActionInfo, M2Share.sSC_LOADVAR);
            }
            //IniFile.Free;
        }

        // 保存变量值
        // SAVEVAR 变量类型 变量名 文件名
        private void ActionOfSaveVar(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sName = string.Empty;
            TDynamicVar DynamicVar = null;
            bool boFoundVar;
            IList<TDynamicVar> DynamicVarList;
            IniFile IniFile;
            const string sVarFound = "变量%s不存在，变量类型:%s";
            const string sVarTypeError = "变量类型错误，错误类型:%s 当前支持类型(HUMAN、GUILD、GLOBAL)";
            string sType = QuestActionInfo.sParam1;
            string sVarName = QuestActionInfo.sParam2;
            string sFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestActionInfo.sParam3;
            if ((sType == "") || (sVarName == "") || !File.Exists(sFileName))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_SAVEVAR);
                return;
            }
            boFoundVar = false;
            DynamicVarList = GetDynamicVarList(PlayObject, sType, ref sName);
            if (DynamicVarList == null)
            {
                Dispose(DynamicVar);
                ScriptActionError(PlayObject, format(sVarTypeError, new string[] { sType }), QuestActionInfo, M2Share.sSC_VAR);
                return;
            }
            IniFile = new IniFile(sFileName);
            for (var I = 0; I < DynamicVarList.Count; I++)
            {
                DynamicVar = DynamicVarList[I];
                if (DynamicVar.sName.ToLower().CompareTo(sVarName.ToLower()) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.VInteger:

                            IniFile.WriteInteger(sName, DynamicVar.sName, DynamicVar.nInternet);
                            break;
                        case TVarType.VString:

                            IniFile.WriteString(sName, DynamicVar.sName, DynamicVar.sString);
                            break;
                    }
                    boFoundVar = true;
                    break;
                }
            }
            if (!boFoundVar)
            {
                ScriptActionError(PlayObject, format(sVarFound, new string[] { sVarName, sType }), QuestActionInfo, M2Share.sSC_SAVEVAR);
            }
            //IniFile.Free;
        }

        private void ActionOfDelayCall(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            PlayObject.m_nDelayCall = HUtil32._MAX(1, QuestActionInfo.nParam1);
            PlayObject.m_sDelayCallLabel = QuestActionInfo.sParam2;
            PlayObject.m_dwDelayCallTick = HUtil32.GetTickCount();
            PlayObject.m_boDelayCall = true;
            PlayObject.m_DelayCallNPC = this.ObjectId;
        }

        // 对变量进行运算(+、-、*、/)
        private void ActionOfCalcVar(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sName = string.Empty;
            TDynamicVar DynamicVar = null;
            IList<TDynamicVar> DynamicVarList;
            const string sVarFound = "变量%s不存在，变量类型:%s";
            const string sVarTypeError = "变量类型错误，错误类型:%s 当前支持类型(HUMAN、GUILD、GLOBAL)";
            string sType = QuestActionInfo.sParam1;
            string sVarName = QuestActionInfo.sParam2;
            string sMethod = QuestActionInfo.sParam3;
            string sVarValue = QuestActionInfo.sParam4;
            int nVarValue = HUtil32.Str_ToInt(QuestActionInfo.sParam4, 0);
            if ((sType == "") || (sVarName == "") || (sMethod == ""))
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_CALCVAR);
                return;
            }
            bool boFoundVar = false;
            char cMethod = sMethod[1];
            DynamicVarList = GetDynamicVarList(PlayObject, sType, ref sName);
            if (DynamicVarList == null)
            {
                Dispose(DynamicVar);
                ScriptActionError(PlayObject, format(sVarTypeError, new string[] { sType }), QuestActionInfo, M2Share.sSC_CALCVAR);
                return;
            }
            for (var i = 0; i < DynamicVarList.Count; i++)
            {
                DynamicVar = DynamicVarList[i];
                if (DynamicVar.sName.ToLower().CompareTo(sVarName.ToLower()) == 0)
                {
                    switch (DynamicVar.VarType)
                    {
                        case TVarType.VInteger:
                            switch (cMethod)
                            {
                                case '=':
                                    DynamicVar.nInternet = nVarValue;
                                    break;
                                case '+':
                                    DynamicVar.nInternet = DynamicVar.nInternet + nVarValue;
                                    break;
                                case '-':
                                    DynamicVar.nInternet = DynamicVar.nInternet - nVarValue;
                                    break;
                                case '*':
                                    DynamicVar.nInternet = DynamicVar.nInternet * nVarValue;
                                    break;
                                case '/':
                                    DynamicVar.nInternet = DynamicVar.nInternet / nVarValue;
                                    break;
                            }
                            break;
                        case TVarType.VString:
                            break;
                    }
                    boFoundVar = true;
                    break;
                }
            }
            if (!boFoundVar)
            {
                ScriptActionError(PlayObject, format(sVarFound, new string[] { sVarName, sType }), QuestActionInfo, M2Share.sSC_CALCVAR);
            }
        }

        private void ActionOfGuildRecall(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            if (PlayObject.m_MyGuild != null)
            {
                // PlayObject.GuildRecall('GuildRecall','');
            }
        }

        private void ActionOfGroupAddList(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string ffile = QuestActionInfo.sParam1;
            if (PlayObject.m_GroupOwner != null)
            {
                for (var I = 0; I < PlayObject.m_GroupMembers.Count; I++)
                {
                    PlayObject = PlayObject.m_GroupMembers[I];
                    // AddListEx(PlayObject.m_sCharName,ffile);
                }
            }
        }

        // if DeleteFile(fileName)
        private void ActionOfClearList(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string ffile;
            ffile = M2Share.g_Config.sEnvirDir + QuestActionInfo.sParam1;
            if (File.Exists(ffile))
            {
                //myFile = new FileInfo(ffile);
                //_W_0 = myFile.CreateText();
                //_W_0.Close();
            }
        }

        private void ActionOfGroupRecall(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            if (PlayObject.m_GroupOwner != null)
            {
                // PlayObject.GroupRecall('GroupRecall');
            }
        }

        // 脚本特修身上所有装备命令
        private void ActionOfRepairAllItem(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            string sUserItemName;
            bool boIsHasItem = false;
            for (var i = PlayObject.m_UseItems.GetLowerBound(0); i <= PlayObject.m_UseItems.GetUpperBound(0); i++)
            {
                if (PlayObject.m_UseItems[i].wIndex <= 0)
                {
                    continue;
                }
                sUserItemName = M2Share.UserEngine.GetStdItemName(PlayObject.m_UseItems[i].wIndex);
                if (!(i != grobal2.U_CHARM))
                {
                    PlayObject.SysMsg(sUserItemName + " 禁止修理...", TMsgColor.c_Red, TMsgType.t_Hint);
                    continue;
                }
                PlayObject.m_UseItems[i].Dura = PlayObject.m_UseItems[i].DuraMax;
                PlayObject.SendMsg(this, grobal2.RM_DURACHANGE, (short)i, PlayObject.m_UseItems[i].Dura, PlayObject.m_UseItems[i].DuraMax, 0, "");
                boIsHasItem = true;
            }
            if (boIsHasItem)
            {
                PlayObject.SysMsg("您身上的的装备修复成功了...", TMsgColor.c_Blue, TMsgType.t_Hint);
            }
        }

        private void ActionOfGroupMoveMap(TPlayObject PlayObject, TQuestActionInfo QuestActionInfo)
        {
            TPlayObject PlayObjectEx;
            TEnvirnoment Envir;
            bool boFlag = false;
            if (PlayObject.m_GroupOwner != null)
            {
                Envir = M2Share.g_MapManager.FindMap(QuestActionInfo.sParam1);
                if (Envir != null)
                {
                    if (Envir.CanWalk(QuestActionInfo.nParam2, QuestActionInfo.nParam3, true))
                    {
                        for (var i = 0; i < PlayObject.m_GroupMembers.Count; i++)
                        {
                            PlayObjectEx = PlayObject.m_GroupMembers[i];
                            PlayObjectEx.SpaceMove(QuestActionInfo.sParam1, (short)QuestActionInfo.nParam2, (short)QuestActionInfo.nParam3, 0);
                        }
                        boFlag = true;
                    }
                }
            }
            if (!boFlag)
            {
                ScriptActionError(PlayObject, "", QuestActionInfo, M2Share.sSC_GROUPMOVEMAP);
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            this.m_Castle = M2Share.CastleManager.InCastleWarArea(this);
        }

        private bool ConditionOfCheckNameDateList(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int I;
            StringList LoadList;
            string sListFileName;
            string sLineText;
            string sHumName = string.Empty;
            string sDate = string.Empty;
            bool boDeleteExprie;
            bool boNoCompareHumanName;
            DateTime dOldDate = DateTime.Now;
            char cMethod;
            int nValNo;
            int nValNoDay;
            int nDayCount;
            int nDay;
            result = false;
            nDayCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            nValNo = M2Share.GetValNameNo(QuestConditionInfo.sParam4);
            nValNoDay = M2Share.GetValNameNo(QuestConditionInfo.sParam5);
            boDeleteExprie = QuestConditionInfo.sParam6.ToLower().CompareTo("清理".ToLower()) == 0;
            boNoCompareHumanName = QuestConditionInfo.sParam6.ToLower().CompareTo("1".ToLower()) == 0;
            cMethod = QuestConditionInfo.sParam2[1];
            if (nDayCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKNAMEDATELIST);
                return result;
            }
            sListFileName = M2Share.g_Config.sEnvirDir + m_sPath + QuestConditionInfo.sParam1;
            if (File.Exists(sListFileName))
            {
                LoadList = new StringList();
                try
                {

                    LoadList.LoadFromFile(sListFileName);
                }
                catch
                {
                    M2Share.MainOutMessage("loading fail.... => " + sListFileName);
                }
                for (I = 0; I < LoadList.Count; I++)
                {
                    sLineText = LoadList[I].Trim();
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sHumName, new string[] { " ", "\t" });
                    sLineText = HUtil32.GetValidStr3(sLineText, ref sDate, new string[] { " ", "\t" });
                    if ((sHumName.ToLower().CompareTo(PlayObject.m_sCharName.ToLower()) == 0) || boNoCompareHumanName)
                    {
                        nDay = int.MaxValue;
                        //if (TryStrToDateTime(sDate, dOldDate))
                        //{
                        //    nDay = HUtil32.GetDayCount(DateTime.Now, dOldDate);
                        //}
                        switch (cMethod)
                        {
                            case '=':
                                if (nDay == nDayCount)
                                {
                                    result = true;
                                }
                                break;
                            case '>':
                                if (nDay > nDayCount)
                                {
                                    result = true;
                                }
                                break;
                            case '<':
                                if (nDay < nDayCount)
                                {
                                    result = true;
                                }
                                break;
                            default:
                                if (nDay >= nDayCount)
                                {
                                    result = true;
                                }
                                break;
                        }
                        if (nValNo >= 0)
                        {
                            switch (nValNo)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    PlayObject.m_nVal[nValNo] = nDay;
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    M2Share.g_Config.GlobalVal[nValNo - 100] = nDay;
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    PlayObject.m_DyVal[nValNo - 200] = nDay;
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    PlayObject.m_nMval[nValNo - 300] = nDay;
                                    break;
                                // Modify the A .. B: 400 .. 499
                                case 400:
                                    M2Share.g_Config.GlobaDyMval[nValNo - 400] = (short)nDay;
                                    break;
                            }
                        }
                        if (nValNoDay >= 0)
                        {
                            switch (nValNoDay)
                            {
                                // Modify the A .. B: 0 .. 9
                                case 0:
                                    PlayObject.m_nVal[nValNoDay] = nDayCount - nDay;
                                    break;
                                // Modify the A .. B: 100 .. 119
                                case 100:
                                    M2Share.g_Config.GlobalVal[nValNoDay - 100] = nDayCount - nDay;
                                    break;
                                // Modify the A .. B: 200 .. 209
                                case 200:
                                    PlayObject.m_DyVal[nValNoDay - 200] = nDayCount - nDay;
                                    break;
                                // Modify the A .. B: 300 .. 399
                                case 300:
                                    PlayObject.m_nMval[nValNoDay - 300] = nDayCount - nDay;
                                    break;
                            }
                        }
                        if (!result)
                        {
                            if (boDeleteExprie)
                            {
                                LoadList.RemoveAt(I);
                                try
                                {

                                    LoadList.SaveToFile(sListFileName);
                                }
                                catch
                                {
                                    M2Share.MainOutMessage("Save fail.... => " + sListFileName);
                                }
                            }
                        }
                        break;
                    }
                }

                //LoadList.Free;
            }
            else
            {
                M2Share.MainOutMessage("file not found => " + sListFileName);
            }
            return result;
        }

        // CHECKMAPHUMANCOUNT MAP = COUNT
        private bool ConditionOfCheckMapHumanCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nCount;
            int nHumanCount;
            char cMethod;
            result = false;
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            if (nCount < 0)
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMAPHUMANCOUNT);
                return result;
            }
            nHumanCount = M2Share.UserEngine.GetMapHuman(QuestConditionInfo.sParam1);
            cMethod = QuestConditionInfo.sParam2[1];
            switch (cMethod)
            {
                case '=':
                    if (nHumanCount == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nHumanCount > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nHumanCount < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nHumanCount >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckMapMonCount(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result;
            int nCount;
            int nMonCount;
            char cMethod;
            TEnvirnoment Envir;
            result = false;
            nCount = HUtil32.Str_ToInt(QuestConditionInfo.sParam3, -1);
            Envir = M2Share.g_MapManager.FindMap(QuestConditionInfo.sParam1);
            if ((nCount < 0) || (Envir == null))
            {
                ScriptConditionError(PlayObject, QuestConditionInfo, M2Share.sSC_CHECKMAPMONCOUNT);
                return result;
            }
            nMonCount = M2Share.UserEngine.GetMapMonster(Envir, null);
            cMethod = QuestConditionInfo.sParam2[1];
            switch (cMethod)
            {
                case '=':
                    if (nMonCount == nCount)
                    {
                        result = true;
                    }
                    break;
                case '>':
                    if (nMonCount > nCount)
                    {
                        result = true;
                    }
                    break;
                case '<':
                    if (nMonCount < nCount)
                    {
                        result = true;
                    }
                    break;
                default:
                    if (nMonCount >= nCount)
                    {
                        result = true;
                    }
                    break;
            }
            return result;
        }

        private bool ConditionOfCheckIsOnMap(TPlayObject PlayObject, TQuestConditionInfo QuestConditionInfo)
        {
            bool result = false;
            if ((PlayObject.m_sMapFileName == QuestConditionInfo.sParam1) || (PlayObject.m_sMapName == QuestConditionInfo.sParam1))
            {
                result = true;
            }
            return result;
        }

        private IList<TDynamicVar> GetDynamicVarList(TPlayObject PlayObject, string sType, ref string sName)
        {
            IList<TDynamicVar> result = null;
            if (HUtil32.CompareLStr(sType, "HUMAN", "HUMAN".Length))
            {
                result = PlayObject.m_DynamicVarList;
                sName = PlayObject.m_sCharName;
            }
            else if (HUtil32.CompareLStr(sType, "GUILD", "GUILD".Length))
            {
                if (PlayObject.m_MyGuild == null)
                {
                    return result;
                }
                result = PlayObject.m_MyGuild.m_DynamicVarList;
                sName = PlayObject.m_MyGuild.sGuildName;
            }
            else if (HUtil32.CompareLStr(sType, "GLOBAL", "GLOBAL".Length))
            {
                result = M2Share.g_DynamicVarList;
                sName = "GLOBAL";
            }
            return result;
        }
    }
}