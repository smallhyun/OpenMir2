﻿using GameSvr.Command;
using SystemModule.Common;

namespace GameSvr.Conf
{
    public class GameCmdConf : IniFile
    {
        public GameCmdConf(string fileName) : base(fileName)
        {
            Load();
        }

        /// <summary>
        /// 读取自定义命令配置
        /// </summary>
        public void LoadConfig()
        {
            int nLoadInteger;
            string LoadString = ReadString("Command", "Date", "");
            if (LoadString == "")
            {
                WriteString("Command", "Date", M2Share.GameCommand.Data.CommandName);
            }
            else
            {
                M2Share.GameCommand.Data.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Date", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Date", M2Share.GameCommand.Data.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Data.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "PrvMsg", "");
            if (LoadString == "")
            {
                WriteString("Command", "PrvMsg", M2Share.GameCommand.Prvmsg.CommandName);
            }
            else
            {
                M2Share.GameCommand.Prvmsg.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "PrvMsg", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "PrvMsg", M2Share.GameCommand.Prvmsg.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Prvmsg.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AllowMsg", "");
            if (LoadString == "")
            {
                WriteString("Command", "AllowMsg", M2Share.GameCommand.AllowMsg.CommandName);
            }
            else
            {
                M2Share.GameCommand.AllowMsg.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AllowMsg", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AllowMsg", M2Share.GameCommand.AllowMsg.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AllowMsg.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "LetShout", "");
            if (LoadString == "")
            {
                WriteString("Command", "LetShout", M2Share.GameCommand.Letshout.CommandName);
            }
            else
            {
                M2Share.GameCommand.Letshout.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "LetTrade", "");
            if (LoadString == "")
            {
                WriteString("Command", "LetTrade", M2Share.GameCommand.LetTrade.CommandName);
            }
            else
            {
                M2Share.GameCommand.LetTrade.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "LetGuild", "");
            if (LoadString == "")
            {
                WriteString("Command", "LetGuild", M2Share.GameCommand.Letguild.CommandName);
            }
            else
            {
                M2Share.GameCommand.Letguild.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "EndGuild", "");
            if (LoadString == "")
            {
                WriteString("Command", "EndGuild", M2Share.GameCommand.Endguild.CommandName);
            }
            else
            {
                M2Share.GameCommand.Endguild.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "BanGuildChat", "");
            if (LoadString == "")
            {
                WriteString("Command", "BanGuildChat", M2Share.GameCommand.BanGuildChat.CommandName);
            }
            else
            {
                M2Share.GameCommand.BanGuildChat.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AuthAlly", "");
            if (LoadString == "")
            {
                WriteString("Command", "AuthAlly", M2Share.GameCommand.Authally.CommandName);
            }
            else
            {
                M2Share.GameCommand.Authally.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "Auth", "");
            if (LoadString == "")
            {
                WriteString("Command", "Auth", M2Share.GameCommand.Auth.CommandName);
            }
            else
            {
                M2Share.GameCommand.Auth.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AuthCancel", "");
            if (LoadString == "")
            {
                WriteString("Command", "AuthCancel", M2Share.GameCommand.AuthCancel.CommandName);
            }
            else
            {
                M2Share.GameCommand.AuthCancel.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "ViewDiary", "");
            if (LoadString == "")
            {
                WriteString("Command", "ViewDiary", M2Share.GameCommand.Diary.CommandName);
            }
            else
            {
                M2Share.GameCommand.Diary.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "UserMove", "");
            if (LoadString == "")
            {
                WriteString("Command", "UserMove", M2Share.GameCommand.UserMove.CommandName);
            }
            else
            {
                M2Share.GameCommand.UserMove.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "Searching", "");
            if (LoadString == "")
            {
                WriteString("Command", "Searching", M2Share.GameCommand.Searching.CommandName);
            }
            else
            {
                M2Share.GameCommand.Searching.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AllowGroupCall", "");
            if (LoadString == "")
            {
                WriteString("Command", "AllowGroupCall", M2Share.GameCommand.AllowGroupCall.CommandName);
            }
            else
            {
                M2Share.GameCommand.AllowGroupCall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "GroupCall", "");
            if (LoadString == "")
            {
                WriteString("Command", "GroupCall", M2Share.GameCommand.GroupRecalll.CommandName);
            }
            else
            {
                M2Share.GameCommand.GroupRecalll.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AllowGuildReCall", "");
            if (LoadString == "")
            {
                WriteString("Command", "AllowGuildReCall", M2Share.GameCommand.AllowGuildRecall.CommandName);
            }
            else
            {
                M2Share.GameCommand.AllowGuildRecall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "GuildReCall", "");
            if (LoadString == "")
            {
                WriteString("Command", "GuildReCall", M2Share.GameCommand.GuildRecalll.CommandName);
            }
            else
            {
                M2Share.GameCommand.GuildRecalll.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "StorageUnLock", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageUnLock", M2Share.GameCommand.UnlockStorage.CommandName);
            }
            else
            {
                M2Share.GameCommand.UnlockStorage.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "PasswordUnLock", "");
            if (LoadString == "")
            {
                WriteString("Command", "PasswordUnLock", M2Share.GameCommand.Unlock.CommandName);
            }
            else
            {
                M2Share.GameCommand.Unlock.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "StorageLock", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageLock", M2Share.GameCommand.Lock.CommandName);
            }
            else
            {
                M2Share.GameCommand.Lock.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "StorageSetPassword", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageSetPassword", M2Share.GameCommand.SetPassword.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetPassword.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "PasswordLock", "");
            if (LoadString == "")
            {
                WriteString("Command", "PasswordLock", M2Share.GameCommand.PasswordLock.CommandName);
            }
            else
            {
                M2Share.GameCommand.PasswordLock.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "StorageChgPassword", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageChgPassword", M2Share.GameCommand.ChgPassword.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChgPassword.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "StorageClearPassword", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageClearPassword", M2Share.GameCommand.ClrPassword.CommandName);
            }
            else
            {
                M2Share.GameCommand.ClrPassword.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "StorageClearPassword", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "StorageClearPassword", M2Share.GameCommand.ClrPassword.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ClrPassword.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "StorageUserClearPassword", "");
            if (LoadString == "")
            {
                WriteString("Command", "StorageUserClearPassword", M2Share.GameCommand.UnPassword.CommandName);
            }
            else
            {
                M2Share.GameCommand.UnPassword.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "MemberFunc", "");
            if (LoadString == "")
            {
                WriteString("Command", "MemberFunc", M2Share.GameCommand.MemberFunction.CommandName);
            }
            else
            {
                M2Share.GameCommand.MemberFunction.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "MemberFuncEx", "");
            if (LoadString == "")
            {
                WriteString("Command", "MemberFuncEx", M2Share.GameCommand.MemberFunctioneX.CommandName);
            }
            else
            {
                M2Share.GameCommand.MemberFunctioneX.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "Dear", "");
            if (LoadString == "")
            {
                WriteString("Command", "Dear", M2Share.GameCommand.Dear.CommandName);
            }
            else
            {
                M2Share.GameCommand.Dear.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "Master", "");
            if (LoadString == "")
            {
                WriteString("Command", "Master", M2Share.GameCommand.Master.CommandName);
            }
            else
            {
                M2Share.GameCommand.Master.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "DearRecall", "");
            if (LoadString == "")
            {
                WriteString("Command", "DearRecall", M2Share.GameCommand.DearRecall.CommandName);
            }
            else
            {
                M2Share.GameCommand.DearRecall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "MasterRecall", "");
            if (LoadString == "")
            {
                WriteString("Command", "MasterRecall", M2Share.GameCommand.MasteRecall.CommandName);
            }
            else
            {
                M2Share.GameCommand.MasteRecall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AllowDearRecall", "");
            if (LoadString == "")
            {
                WriteString("Command", "AllowDearRecall", M2Share.GameCommand.AllowDearRcall.CommandName);
            }
            else
            {
                M2Share.GameCommand.AllowDearRcall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AllowMasterRecall", "");
            if (LoadString == "")
            {
                WriteString("Command", "AllowMasterRecall", M2Share.GameCommand.AllowMasterRecall.CommandName);
            }
            else
            {
                M2Share.GameCommand.AllowMasterRecall.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "AttackMode", "");
            if (LoadString == "")
            {
                WriteString("Command", "AttackMode", M2Share.GameCommand.AttackMode.CommandName);
            }
            else
            {
                M2Share.GameCommand.AttackMode.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "Rest", "");
            if (LoadString == "")
            {
                WriteString("Command", "Rest", M2Share.GameCommand.Rest.CommandName);
            }
            else
            {
                M2Share.GameCommand.Rest.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "TakeOnHorse", "");
            if (LoadString == "")
            {
                WriteString("Command", "TakeOnHorse", M2Share.GameCommand.TakeonHorse.CommandName);
            }
            else
            {
                M2Share.GameCommand.TakeonHorse.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "TakeOffHorse", "");
            if (LoadString == "")
            {
                WriteString("Command", "TakeOffHorse", M2Share.GameCommand.TakeofHorse.CommandName);
            }
            else
            {
                M2Share.GameCommand.TakeofHorse.CommandName = LoadString;
            }
            LoadString = ReadString("Command", "HumanLocal", "");
            if (LoadString == "")
            {
                WriteString("Command", "HumanLocal", M2Share.GameCommand.HumanLocal.CommandName);
            }
            else
            {
                M2Share.GameCommand.HumanLocal.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "HumanLocal", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "HumanLocal", M2Share.GameCommand.HumanLocal.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.HumanLocal.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Move", "");
            if (LoadString == "")
            {
                WriteString("Command", "Move", M2Share.GameCommand.Move.CommandName);
            }
            else
            {
                M2Share.GameCommand.Move.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MoveMin", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MoveMin", M2Share.GameCommand.Move.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Move.nPerMissionMin = nLoadInteger;
            }
            nLoadInteger = ReadInteger("Permission", "MoveMax", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MoveMax", M2Share.GameCommand.Move.nPerMissionMax);
            }
            else
            {
                M2Share.GameCommand.Move.nPerMissionMax = nLoadInteger;
            }
            LoadString = ReadString("Command", "PositionMove", "");
            if (LoadString == "")
            {
                WriteString("Command", "PositionMove", M2Share.GameCommand.PositionMove.CommandName);
            }
            else
            {
                M2Share.GameCommand.PositionMove.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "PositionMoveMin", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "PositionMoveMin", M2Share.GameCommand.PositionMove.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.PositionMove.nPerMissionMin = nLoadInteger;
            }
            nLoadInteger = ReadInteger("Permission", "PositionMoveMax", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "PositionMoveMax", M2Share.GameCommand.PositionMove.nPerMissionMax);
            }
            else
            {
                M2Share.GameCommand.PositionMove.nPerMissionMax = nLoadInteger;
            }
            LoadString = ReadString("Command", "Info", "");
            if (LoadString == "")
            {
                WriteString("Command", "Info", M2Share.GameCommand.Info.CommandName);
            }
            else
            {
                M2Share.GameCommand.Info.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Info", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Info", M2Share.GameCommand.Info.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Info.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MobLevel", "");
            if (LoadString == "")
            {
                WriteString("Command", "MobLevel", M2Share.GameCommand.MobLevel.CommandName);
            }
            else
            {
                M2Share.GameCommand.MobLevel.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MobLevel", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MobLevel", M2Share.GameCommand.MobLevel.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MobLevel.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MobCount", "");
            if (LoadString == "")
            {
                WriteString("Command", "MobCount", M2Share.GameCommand.MobCount.CommandName);
            }
            else
            {
                M2Share.GameCommand.MobCount.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MobCount", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MobCount", M2Share.GameCommand.MobCount.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MobCount.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "HumanCount", "");
            if (LoadString == "")
            {
                WriteString("Command", "HumanCount", M2Share.GameCommand.HumanCount.CommandName);
            }
            else
            {
                M2Share.GameCommand.HumanCount.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "HumanCount", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "HumanCount", M2Share.GameCommand.HumanCount.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.HumanCount.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Map", "");
            if (LoadString == "")
            {
                WriteString("Command", "Map", M2Share.GameCommand.Map.CommandName);
            }
            else
            {
                M2Share.GameCommand.Map.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Map", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Map", M2Share.GameCommand.Map.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Map.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Kick", "");
            if (LoadString == "")
            {
                WriteString("Command", "Kick", M2Share.GameCommand.Kick.CommandName);
            }
            else
            {
                M2Share.GameCommand.Kick.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Kick", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Kick", M2Share.GameCommand.Kick.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Kick.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Ting", "");
            if (LoadString == "")
            {
                WriteString("Command", "Ting", M2Share.GameCommand.Ting.CommandName);
            }
            else
            {
                M2Share.GameCommand.Ting.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Ting", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Ting", M2Share.GameCommand.Ting.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Ting.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SuperTing", "");
            if (LoadString == "")
            {
                WriteString("Command", "SuperTing", M2Share.GameCommand.Superting.CommandName);
            }
            else
            {
                M2Share.GameCommand.Superting.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SuperTing", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SuperTing", M2Share.GameCommand.Superting.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Superting.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MapMove", "");
            if (LoadString == "")
            {
                WriteString("Command", "MapMove", M2Share.GameCommand.MapMove.CommandName);
            }
            else
            {
                M2Share.GameCommand.MapMove.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MapMove", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MapMove", M2Share.GameCommand.MapMove.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MapMove.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Shutup", "");
            if (LoadString == "")
            {
                WriteString("Command", "Shutup", M2Share.GameCommand.ShutUp.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShutUp.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Shutup", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Shutup", M2Share.GameCommand.ShutUp.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShutUp.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReleaseShutup", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReleaseShutup", M2Share.GameCommand.ReleaseShutup.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReleaseShutup.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReleaseShutup", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReleaseShutup", M2Share.GameCommand.ReleaseShutup.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReleaseShutup.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShutupList", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShutupList", M2Share.GameCommand.ShutupList.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShutupList.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShutupList", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShutupList", M2Share.GameCommand.ShutupList.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShutupList.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "GameMaster", "");
            if (LoadString == "")
            {
                WriteString("Command", "GameMaster", M2Share.GameCommand.GameMaster.CommandName);
            }
            else
            {
                M2Share.GameCommand.GameMaster.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "GameMaster", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "GameMaster", M2Share.GameCommand.GameMaster.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.GameMaster.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ObServer", "");
            if (LoadString == "")
            {
                WriteString("Command", "ObServer", M2Share.GameCommand.ObServer.CommandName);
            }
            else
            {
                M2Share.GameCommand.ObServer.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ObServer", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ObServer", M2Share.GameCommand.ObServer.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ObServer.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SuperMan", "");
            if (LoadString == "")
            {
                WriteString("Command", "SuperMan", M2Share.GameCommand.SueprMan.CommandName);
            }
            else
            {
                M2Share.GameCommand.SueprMan.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SuperMan", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SuperMan", M2Share.GameCommand.SueprMan.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SueprMan.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Level", "");
            if (LoadString == "")
            {
                WriteString("Command", "Level", M2Share.GameCommand.Level.CommandName);
            }
            else
            {
                M2Share.GameCommand.Level.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Level", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Level", M2Share.GameCommand.Level.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Level.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SabukWallGold", "");
            if (LoadString == "")
            {
                WriteString("Command", "SabukWallGold", M2Share.GameCommand.SabukwallGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.SabukwallGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SabukWallGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SabukWallGold", M2Share.GameCommand.SabukwallGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SabukwallGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Recall", "");
            if (LoadString == "")
            {
                WriteString("Command", "Recall", M2Share.GameCommand.Recall.CommandName);
            }
            else
            {
                M2Share.GameCommand.Recall.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Recall", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Recall", M2Share.GameCommand.Recall.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Recall.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReGoto", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReGoto", M2Share.GameCommand.Regoto.CommandName);
            }
            else
            {
                M2Share.GameCommand.Regoto.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReGoto", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReGoto", M2Share.GameCommand.Regoto.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Regoto.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Flag", "");
            if (LoadString == "")
            {
                WriteString("Command", "Flag", M2Share.GameCommand.Showflag.CommandName);
            }
            else
            {
                M2Share.GameCommand.Showflag.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Flag", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Flag", M2Share.GameCommand.Showflag.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Showflag.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShowOpen", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShowOpen", M2Share.GameCommand.ShowOpen.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowOpen.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShowOpen", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShowOpen", M2Share.GameCommand.ShowOpen.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowOpen.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShowUnit", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShowUnit", M2Share.GameCommand.ShowUnit.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowUnit.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShowUnit", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShowUnit", M2Share.GameCommand.ShowUnit.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowUnit.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Attack", "");
            if (LoadString == "")
            {
                WriteString("Command", "Attack", M2Share.GameCommand.Attack.CommandName);
            }
            else
            {
                M2Share.GameCommand.Attack.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Attack", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Attack", M2Share.GameCommand.Attack.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Attack.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Mob", "");
            if (LoadString == "")
            {
                WriteString("Command", "Mob", M2Share.GameCommand.Mob.CommandName);
            }
            else
            {
                M2Share.GameCommand.Mob.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Mob", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Mob", M2Share.GameCommand.Mob.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Mob.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MobNpc", "");
            if (LoadString == "")
            {
                WriteString("Command", "MobNpc", M2Share.GameCommand.MobNpc.CommandName);
            }
            else
            {
                M2Share.GameCommand.MobNpc.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MobNpc", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MobNpc", M2Share.GameCommand.MobNpc.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MobNpc.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelNpc", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelNpc", M2Share.GameCommand.DeleteNpc.CommandName);
            }
            else
            {
                M2Share.GameCommand.DeleteNpc.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelNpc", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelNpc", M2Share.GameCommand.DeleteNpc.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DeleteNpc.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "NpcScript", "");
            if (LoadString == "")
            {
                WriteString("Command", "NpcScript", M2Share.GameCommand.NpcScript.CommandName);
            }
            else
            {
                M2Share.GameCommand.NpcScript.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "NpcScript", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "NpcScript", M2Share.GameCommand.NpcScript.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.NpcScript.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "RecallMob", "");
            if (LoadString == "")
            {
                WriteString("Command", "RecallMob", M2Share.GameCommand.RecallMob.CommandName);
            }
            else
            {
                M2Share.GameCommand.RecallMob.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "RecallMob", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "RecallMob", M2Share.GameCommand.RecallMob.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.RecallMob.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "LuckPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "LuckPoint", M2Share.GameCommand.LuckyPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.LuckyPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "LuckPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "LuckPoint", M2Share.GameCommand.LuckyPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.LuckyPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "LotteryTicket", "");
            if (LoadString == "")
            {
                WriteString("Command", "LotteryTicket", M2Share.GameCommand.LotteryTicket.CommandName);
            }
            else
            {
                M2Share.GameCommand.LotteryTicket.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "LotteryTicket", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "LotteryTicket", M2Share.GameCommand.LotteryTicket.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.LotteryTicket.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadGuild", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadGuild", M2Share.GameCommand.ReloadGuild.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadGuild.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadGuild", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadGuild", M2Share.GameCommand.ReloadGuild.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadGuild.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadLineNotice", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadLineNotice", M2Share.GameCommand.ReloadLineNotice.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadLineNotice.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadLineNotice", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadLineNotice", M2Share.GameCommand.ReloadLineNotice.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadLineNotice.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadAbuse", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadAbuse", M2Share.GameCommand.ReloadAbuse.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadAbuse.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadAbuse", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadAbuse", M2Share.GameCommand.ReloadAbuse.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadAbuse.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "BackStep", "");
            if (LoadString == "")
            {
                WriteString("Command", "BackStep", M2Share.GameCommand.BackStep.CommandName);
            }
            else
            {
                M2Share.GameCommand.BackStep.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "BackStep", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "BackStep", M2Share.GameCommand.BackStep.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.BackStep.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Ball", "");
            if (LoadString == "")
            {
                WriteString("Command", "Ball", M2Share.GameCommand.Ball.CommandName);
            }
            else
            {
                M2Share.GameCommand.Ball.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Ball", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Ball", M2Share.GameCommand.Ball.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Ball.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "FreePenalty", "");
            if (LoadString == "")
            {
                WriteString("Command", "FreePenalty", M2Share.GameCommand.FreePenalty.CommandName);
            }
            else
            {
                M2Share.GameCommand.FreePenalty.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "FreePenalty", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "FreePenalty", M2Share.GameCommand.FreePenalty.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.FreePenalty.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "PkPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "PkPoint", M2Share.GameCommand.PkPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.PkPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "PkPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "PkPoint", M2Share.GameCommand.PkPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.PkPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "IncPkPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "IncPkPoint", M2Share.GameCommand.Incpkpoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.Incpkpoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "IncPkPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "IncPkPoint", M2Share.GameCommand.Incpkpoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Incpkpoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeLuck", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeLuck", M2Share.GameCommand.ChangeLuck.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeLuck.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeLuck", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeLuck", M2Share.GameCommand.ChangeLuck.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeLuck.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Hunger", "");
            if (LoadString == "")
            {
                WriteString("Command", "Hunger", M2Share.GameCommand.Hunger.CommandName);
            }
            else
            {
                M2Share.GameCommand.Hunger.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Hunger", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Hunger", M2Share.GameCommand.Hunger.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Hunger.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Hair", "");
            if (LoadString == "")
            {
                WriteString("Command", "Hair", M2Share.GameCommand.Hair.CommandName);
            }
            else
            {
                M2Share.GameCommand.Hair.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Hair", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Hair", M2Share.GameCommand.Hair.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Hair.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Training", "");
            if (LoadString == "")
            {
                WriteString("Command", "Training", M2Share.GameCommand.Training.CommandName);
            }
            else
            {
                M2Share.GameCommand.Training.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Training", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Training", M2Share.GameCommand.Training.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Training.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DeleteSkill", "");
            if (LoadString == "")
            {
                WriteString("Command", "DeleteSkill", M2Share.GameCommand.DeleteSkill.CommandName);
            }
            else
            {
                M2Share.GameCommand.DeleteSkill.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DeleteSkill", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DeleteSkill", M2Share.GameCommand.DeleteSkill.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DeleteSkill.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeJob", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeJob", M2Share.GameCommand.ChangeJob.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeJob.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeJob", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeJob", M2Share.GameCommand.ChangeJob.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeJob.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeGender", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeGender", M2Share.GameCommand.ChangeGender.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeGender.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeGender", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeGender", M2Share.GameCommand.ChangeGender.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeGender.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "NameColor", "");
            if (LoadString == "")
            {
                WriteString("Command", "NameColor", M2Share.GameCommand.Namecolor.CommandName);
            }
            else
            {
                M2Share.GameCommand.Namecolor.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "NameColor", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "NameColor", M2Share.GameCommand.Namecolor.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Namecolor.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Mission", "");
            if (LoadString == "")
            {
                WriteString("Command", "Mission", M2Share.GameCommand.Mission.CommandName);
            }
            else
            {
                M2Share.GameCommand.Mission.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Mission", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Mission", M2Share.GameCommand.Mission.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Mission.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MobPlace", "");
            if (LoadString == "")
            {
                WriteString("Command", "MobPlace", M2Share.GameCommand.MobPlace.CommandName);
            }
            else
            {
                M2Share.GameCommand.MobPlace.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MobPlace", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MobPlace", M2Share.GameCommand.MobPlace.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MobPlace.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Transparecy", "");
            if (LoadString == "")
            {
                WriteString("Command", "Transparecy", M2Share.GameCommand.Transparecy.CommandName);
            }
            else
            {
                M2Share.GameCommand.Transparecy.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Transparecy", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Transparecy", M2Share.GameCommand.Transparecy.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Transparecy.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DeleteItem", "");
            if (LoadString == "")
            {
                WriteString("Command", "DeleteItem", M2Share.GameCommand.DeleteItem.CommandName);
            }
            else
            {
                M2Share.GameCommand.DeleteItem.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DeleteItem", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DeleteItem", M2Share.GameCommand.DeleteItem.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DeleteItem.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Level0", "");
            if (LoadString == "")
            {
                WriteString("Command", "Level0", M2Share.GameCommand.Level.CommandName);
            }
            else
            {
                M2Share.GameCommand.Level.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Level0", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Level0", M2Share.GameCommand.Level.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Level.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ClearMission", "");
            if (LoadString == "")
            {
                WriteString("Command", "ClearMission", M2Share.GameCommand.ClearMission.CommandName);
            }
            else
            {
                M2Share.GameCommand.ClearMission.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ClearMission", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ClearMission", M2Share.GameCommand.ClearMission.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ClearMission.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SetFlag", "");
            if (LoadString == "")
            {
                WriteString("Command", "SetFlag", M2Share.GameCommand.SetFlag.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetFlag.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SetFlag", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SetFlag", M2Share.GameCommand.SetFlag.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SetFlag.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SetOpen", "");
            if (LoadString == "")
            {
                WriteString("Command", "SetOpen", M2Share.GameCommand.SetOpen.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetOpen.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SetOpen", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SetOpen", M2Share.GameCommand.SetOpen.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SetOpen.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SetUnit", "");
            if (LoadString == "")
            {
                WriteString("Command", "SetUnit", M2Share.GameCommand.SetUnit.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetUnit.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SetUnit", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SetUnit", M2Share.GameCommand.SetUnit.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SetUnit.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReConnection", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReConnection", M2Share.GameCommand.Reconnection.CommandName);
            }
            else
            {
                M2Share.GameCommand.Reconnection.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReConnection", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReConnection", M2Share.GameCommand.Reconnection.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Reconnection.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DisableFilter", "");
            if (LoadString == "")
            {
                WriteString("Command", "DisableFilter", M2Share.GameCommand.DisableFilter.CommandName);
            }
            else
            {
                M2Share.GameCommand.DisableFilter.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DisableFilter", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DisableFilter", M2Share.GameCommand.DisableFilter.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DisableFilter.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeUserFull", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeUserFull", M2Share.GameCommand.ChguserFull.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChguserFull.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeUserFull", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeUserFull", M2Share.GameCommand.ChguserFull.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChguserFull.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeZenFastStep", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeZenFastStep", M2Share.GameCommand.ChgZenFastStep.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChgZenFastStep.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeZenFastStep", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeZenFastStep", M2Share.GameCommand.ChgZenFastStep.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChgZenFastStep.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ContestPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "ContestPoint", M2Share.GameCommand.ContestPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.ContestPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ContestPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ContestPoint", M2Share.GameCommand.ContestPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ContestPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "StartContest", "");
            if (LoadString == "")
            {
                WriteString("Command", "StartContest", M2Share.GameCommand.StartContest.CommandName);
            }
            else
            {
                M2Share.GameCommand.StartContest.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "StartContest", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "StartContest", M2Share.GameCommand.StartContest.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.StartContest.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "EndContest", "");
            if (LoadString == "")
            {
                WriteString("Command", "EndContest", M2Share.GameCommand.EndContest.CommandName);
            }
            else
            {
                M2Share.GameCommand.EndContest.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "EndContest", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "EndContest", M2Share.GameCommand.EndContest.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.EndContest.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Announcement", "");
            if (LoadString == "")
            {
                WriteString("Command", "Announcement", M2Share.GameCommand.Announcement.CommandName);
            }
            else
            {
                M2Share.GameCommand.Announcement.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Announcement", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Announcement", M2Share.GameCommand.Announcement.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Announcement.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "OXQuizRoom", "");
            if (LoadString == "")
            {
                WriteString("Command", "OXQuizRoom", M2Share.GameCommand.Oxquizroom.CommandName);
            }
            else
            {
                M2Share.GameCommand.Oxquizroom.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "OXQuizRoom", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "OXQuizRoom", M2Share.GameCommand.Oxquizroom.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Oxquizroom.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Gsa", "");
            if (LoadString == "")
            {
                WriteString("Command", "Gsa", M2Share.GameCommand.Gsa.CommandName);
            }
            else
            {
                M2Share.GameCommand.Gsa.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Gsa", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Gsa", M2Share.GameCommand.Gsa.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Gsa.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeItemName", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeItemName", M2Share.GameCommand.ChangeItemName.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeItemName.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeItemName", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeItemName", M2Share.GameCommand.ChangeItemName.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeItemName.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DisableSendMsg", "");
            if (LoadString == "")
            {
                WriteString("Command", "DisableSendMsg", M2Share.GameCommand.DisableSendMsg.CommandName);
            }
            else
            {
                M2Share.GameCommand.DisableSendMsg.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DisableSendMsg", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DisableSendMsg", M2Share.GameCommand.DisableSendMsg.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DisableSendMsg.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "EnableSendMsg", "");
            if (LoadString == "")
            {
                WriteString("Command", "EnableSendMsg", M2Share.GameCommand.EnableSendMsg.CommandName);
            }
            else
            {
                M2Share.GameCommand.EnableSendMsg.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "EnableSendMsg", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "EnableSendMsg", M2Share.GameCommand.EnableSendMsg.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.EnableSendMsg.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DisableSendMsgList", "");
            if (LoadString == "")
            {
                WriteString("Command", "DisableSendMsgList", M2Share.GameCommand.DisableSendMsgList.CommandName);
            }
            else
            {
                M2Share.GameCommand.DisableSendMsgList.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DisableSendMsgList", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DisableSendMsgList", M2Share.GameCommand.DisableSendMsgList.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DisableSendMsgList.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Kill", "");
            if (LoadString == "")
            {
                WriteString("Command", "Kill", M2Share.GameCommand.Kill.CommandName);
            }
            else
            {
                M2Share.GameCommand.Kill.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Kill", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Kill", M2Share.GameCommand.Kill.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Kill.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Make", "");
            if (LoadString == "")
            {
                WriteString("Command", "Make", M2Share.GameCommand.Make.CommandName);
            }
            else
            {
                M2Share.GameCommand.Make.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MakeMin", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MakeMin", M2Share.GameCommand.Make.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Make.nPerMissionMin = nLoadInteger;
            }
            nLoadInteger = ReadInteger("Permission", "MakeMax", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MakeMax", M2Share.GameCommand.Make.nPerMissionMax);
            }
            else
            {
                M2Share.GameCommand.Make.nPerMissionMax = nLoadInteger;
            }
            LoadString = ReadString("Command", "SuperMake", "");
            if (LoadString == "")
            {
                WriteString("Command", "SuperMake", M2Share.GameCommand.Smake.CommandName);
            }
            else
            {
                M2Share.GameCommand.Smake.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SuperMake", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SuperMake", M2Share.GameCommand.Smake.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Smake.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "BonusPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "BonusPoint", M2Share.GameCommand.BonusPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.BonusPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "BonusPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "BonusPoint", M2Share.GameCommand.BonusPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.BonusPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelBonuPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelBonuPoint", M2Share.GameCommand.DelBonusPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelBonusPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelBonuPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelBonuPoint", M2Share.GameCommand.DelBonusPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelBonusPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "RestBonuPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "RestBonuPoint", M2Share.GameCommand.Restbonuspoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.Restbonuspoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "RestBonuPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "RestBonuPoint", M2Share.GameCommand.Restbonuspoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Restbonuspoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "FireBurn", "");
            if (LoadString == "")
            {
                WriteString("Command", "FireBurn", M2Share.GameCommand.FireBurn.CommandName);
            }
            else
            {
                M2Share.GameCommand.FireBurn.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "FireBurn", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "FireBurn", M2Share.GameCommand.FireBurn.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.FireBurn.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "TestStatus", "");
            if (LoadString == "")
            {
                WriteString("Command", "TestStatus", M2Share.GameCommand.TestStatus.CommandName);
            }
            else
            {
                M2Share.GameCommand.TestStatus.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "TestStatus", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "TestStatus", M2Share.GameCommand.TestStatus.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.TestStatus.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelGold", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelGold", M2Share.GameCommand.DelGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelGold", M2Share.GameCommand.DelGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AddGold", "");
            if (LoadString == "")
            {
                WriteString("Command", "AddGold", M2Share.GameCommand.AddGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.AddGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AddGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AddGold", M2Share.GameCommand.AddGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AddGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelGameGold", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelGameGold", M2Share.GameCommand.DelGameGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelGameGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelGameGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelGameGold", M2Share.GameCommand.DelGameGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelGameGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AddGamePoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "AddGamePoint", M2Share.GameCommand.AddGameGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.AddGameGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AddGameGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AddGameGold", M2Share.GameCommand.AddGameGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AddGameGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "GameGold", "");
            if (LoadString == "")
            {
                WriteString("Command", "GameGold", M2Share.GameCommand.GameGold.CommandName);
            }
            else
            {
                M2Share.GameCommand.GameGold.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "GameGold", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "GameGold", M2Share.GameCommand.GameGold.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.GameGold.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "GamePoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "GamePoint", M2Share.GameCommand.GamePoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.GamePoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "GamePoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "GamePoint", M2Share.GameCommand.GamePoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.GamePoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "CreditPoint", "");
            if (LoadString == "")
            {
                WriteString("Command", "CreditPoint", M2Share.GameCommand.CreditPoint.CommandName);
            }
            else
            {
                M2Share.GameCommand.CreditPoint.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "CreditPoint", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "CreditPoint", M2Share.GameCommand.CreditPoint.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.CreditPoint.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "TestGoldChange", "");
            if (LoadString == "")
            {
                WriteString("Command", "TestGoldChange", M2Share.GameCommand.Testgoldchange.CommandName);
            }
            else
            {
                M2Share.GameCommand.Testgoldchange.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "TestGoldChange", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "TestGoldChange", M2Share.GameCommand.Testgoldchange.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Testgoldchange.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "RefineWeapon", "");
            if (LoadString == "")
            {
                WriteString("Command", "RefineWeapon", M2Share.GameCommand.RefineWeapon.CommandName);
            }
            else
            {
                M2Share.GameCommand.RefineWeapon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "RefineWeapon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "RefineWeapon", M2Share.GameCommand.RefineWeapon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.RefineWeapon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadAdmin", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadAdmin", M2Share.GameCommand.ReloadAdmin.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadAdmin.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadAdmin", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadAdmin", M2Share.GameCommand.ReloadAdmin.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadAdmin.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadNpc", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadNpc", M2Share.GameCommand.ReloadNpc.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadNpc.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadNpc", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadNpc", M2Share.GameCommand.ReloadNpc.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadNpc.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadManage", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadManage", M2Share.GameCommand.ReloadManage.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadManage.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadManage", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadManage", M2Share.GameCommand.ReloadManage.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadManage.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadRobotManage", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadRobotManage", M2Share.GameCommand.ReloadRobotManage.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadRobotManage.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadRobotManage", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadRobotManage", M2Share.GameCommand.ReloadRobotManage.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadRobotManage.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadRobot", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadRobot", M2Share.GameCommand.ReloadRobot.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadRobot.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadRobot", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadRobot", M2Share.GameCommand.ReloadRobot.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadRobot.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadMonitems", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadMonitems", M2Share.GameCommand.ReloadMonItems.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadMonItems.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadMonitems", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadMonitems", M2Share.GameCommand.ReloadMonItems.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadMonItems.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadDiary", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadDiary", M2Share.GameCommand.Reloaddiary.CommandName);
            }
            else
            {
                M2Share.GameCommand.Reloaddiary.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadDiary", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadDiary", M2Share.GameCommand.Reloaddiary.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Reloaddiary.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadItemDB", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadItemDB", M2Share.GameCommand.Reloaditemdb.CommandName);
            }
            else
            {
                M2Share.GameCommand.Reloaditemdb.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadItemDB", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadItemDB", M2Share.GameCommand.Reloaditemdb.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Reloaditemdb.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadMagicDB", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadMagicDB", M2Share.GameCommand.ReloadMagicDb.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadMagicDb.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadMagicDB", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadMagicDB", M2Share.GameCommand.ReloadMagicDb.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadMagicDb.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadMonsterDB", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadMonsterDB", M2Share.GameCommand.Reloadmonsterdb.CommandName);
            }
            else
            {
                M2Share.GameCommand.Reloadmonsterdb.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadMonsterDB", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadMonsterDB", M2Share.GameCommand.Reloadmonsterdb.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Reloadmonsterdb.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReAlive", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReAlive", M2Share.GameCommand.ReaLive.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReaLive.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReAlive", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReAlive", M2Share.GameCommand.ReaLive.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReaLive.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AdjuestTLevel", "");
            if (LoadString == "")
            {
                WriteString("Command", "AdjuestTLevel", M2Share.GameCommand.AdjuestLevel.CommandName);
            }
            else
            {
                M2Share.GameCommand.AdjuestLevel.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AdjuestTLevel", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AdjuestTLevel", M2Share.GameCommand.AdjuestLevel.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AdjuestLevel.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AdjuestExp", "");
            if (LoadString == "")
            {
                WriteString("Command", "AdjuestExp", M2Share.GameCommand.AdjuestExp.CommandName);
            }
            else
            {
                M2Share.GameCommand.AdjuestExp.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AdjuestExp", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AdjuestExp", M2Share.GameCommand.AdjuestExp.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AdjuestExp.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AddGuild", "");
            if (LoadString == "")
            {
                WriteString("Command", "AddGuild", M2Share.GameCommand.AddGuild.CommandName);
            }
            else
            {
                M2Share.GameCommand.AddGuild.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AddGuild", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AddGuild", M2Share.GameCommand.AddGuild.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.AddGuild.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelGuild", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelGuild", M2Share.GameCommand.DelGuild.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelGuild.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelGuild", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelGuild", M2Share.GameCommand.DelGuild.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelGuild.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeSabukLord", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeSabukLord", M2Share.GameCommand.ChangeSabukLord.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeSabukLord.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeSabukLord", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeSabukLord", M2Share.GameCommand.ChangeSabukLord.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeSabukLord.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ForcedWallConQuestWar", "");
            if (LoadString == "")
            {
                WriteString("Command", "ForcedWallConQuestWar", M2Share.GameCommand.ForcedWallConQuestWar.CommandName);
            }
            else
            {
                M2Share.GameCommand.ForcedWallConQuestWar.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ForcedWallConQuestWar", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ForcedWallConQuestWar", M2Share.GameCommand.ForcedWallConQuestWar.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ForcedWallConQuestWar.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AddToItemEvent", "");
            if (LoadString == "")
            {
                WriteString("Command", "AddToItemEvent", M2Share.GameCommand.Addtoitemevent.CommandName);
            }
            else
            {
                M2Share.GameCommand.Addtoitemevent.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AddToItemEvent", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AddToItemEvent", M2Share.GameCommand.Addtoitemevent.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Addtoitemevent.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AddToItemEventAspieces", "");
            if (LoadString == "")
            {
                WriteString("Command", "AddToItemEventAspieces", M2Share.GameCommand.Addtoitemeventaspieces.CommandName);
            }
            else
            {
                M2Share.GameCommand.Addtoitemeventaspieces.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AddToItemEventAspieces", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AddToItemEventAspieces", M2Share.GameCommand.Addtoitemeventaspieces.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Addtoitemeventaspieces.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ItemEventList", "");
            if (LoadString == "")
            {
                WriteString("Command", "ItemEventList", M2Share.GameCommand.Itemeventlist.CommandName);
            }
            else
            {
                M2Share.GameCommand.Itemeventlist.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ItemEventList", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ItemEventList", M2Share.GameCommand.Itemeventlist.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Itemeventlist.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "StartIngGiftNO", "");
            if (LoadString == "")
            {
                WriteString("Command", "StartIngGiftNO", M2Share.GameCommand.Startinggiftno.CommandName);
            }
            else
            {
                M2Share.GameCommand.Startinggiftno.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "StartIngGiftNO", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "StartIngGiftNO", M2Share.GameCommand.Startinggiftno.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Startinggiftno.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DeleteAllItemEvent", "");
            if (LoadString == "")
            {
                WriteString("Command", "DeleteAllItemEvent", M2Share.GameCommand.Deleteallitemevent.CommandName);
            }
            else
            {
                M2Share.GameCommand.Deleteallitemevent.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DeleteAllItemEvent", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DeleteAllItemEvent", M2Share.GameCommand.Deleteallitemevent.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Deleteallitemevent.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "StartItemEvent", "");
            if (LoadString == "")
            {
                WriteString("Command", "StartItemEvent", M2Share.GameCommand.Startitemevent.CommandName);
            }
            else
            {
                M2Share.GameCommand.Startitemevent.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "StartItemEvent", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "StartItemEvent", M2Share.GameCommand.Startitemevent.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Startitemevent.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ItemEventTerm", "");
            if (LoadString == "")
            {
                WriteString("Command", "ItemEventTerm", M2Share.GameCommand.Itemeventterm.CommandName);
            }
            else
            {
                M2Share.GameCommand.Itemeventterm.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ItemEventTerm", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ItemEventTerm", M2Share.GameCommand.Itemeventterm.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Itemeventterm.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "AdjuestTestLevel", "");
            if (LoadString == "")
            {
                WriteString("Command", "AdjuestTestLevel", M2Share.GameCommand.Adjuesttestlevel.CommandName);
            }
            else
            {
                M2Share.GameCommand.Adjuesttestlevel.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "AdjuestTestLevel", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "AdjuestTestLevel", M2Share.GameCommand.Adjuesttestlevel.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Adjuesttestlevel.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "OpTraining", "");
            if (LoadString == "")
            {
                WriteString("Command", "OpTraining", M2Share.GameCommand.TrainingSkill.CommandName);
            }
            else
            {
                M2Share.GameCommand.TrainingSkill.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "OpTraining", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "OpTraining", M2Share.GameCommand.TrainingSkill.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.TrainingSkill.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "OpDeleteSkill", "");
            if (LoadString == "")
            {
                WriteString("Command", "OpDeleteSkill", M2Share.GameCommand.Opdeleteskill.CommandName);
            }
            else
            {
                M2Share.GameCommand.Opdeleteskill.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "OpDeleteSkill", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "OpDeleteSkill", M2Share.GameCommand.Opdeleteskill.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Opdeleteskill.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeWeaponDura", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeWeaponDura", M2Share.GameCommand.Changeweapondura.CommandName);
            }
            else
            {
                M2Share.GameCommand.Changeweapondura.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeWeaponDura", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeWeaponDura", M2Share.GameCommand.Changeweapondura.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Changeweapondura.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReloadGuildAll", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReloadGuildAll", M2Share.GameCommand.ReloadGuildAll.CommandName);
            }
            else
            {
                M2Share.GameCommand.ReloadGuildAll.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReloadGuildAll", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReloadGuildAll", M2Share.GameCommand.ReloadGuildAll.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ReloadGuildAll.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Who", "");
            if (LoadString == "")
            {
                WriteString("Command", "Who", M2Share.GameCommand.Who.CommandName);
            }
            else
            {
                M2Share.GameCommand.Who.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Who", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Who", M2Share.GameCommand.Who.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Who.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "Total", "");
            if (LoadString == "")
            {
                WriteString("Command", "Total", M2Share.GameCommand.Total.CommandName);
            }
            else
            {
                M2Share.GameCommand.Total.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "Total", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "Total", M2Share.GameCommand.Total.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Total.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "TestGa", "");
            if (LoadString == "")
            {
                WriteString("Command", "TestGa", M2Share.GameCommand.Testga.CommandName);
            }
            else
            {
                M2Share.GameCommand.Testga.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "TestGa", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "TestGa", M2Share.GameCommand.Testga.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Testga.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "MapInfo", "");
            if (LoadString == "")
            {
                WriteString("Command", "MapInfo", M2Share.GameCommand.MapInfo.CommandName);
            }
            else
            {
                M2Share.GameCommand.MapInfo.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "MapInfo", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "MapInfo", M2Share.GameCommand.MapInfo.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.MapInfo.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SbkDoor", "");
            if (LoadString == "")
            {
                WriteString("Command", "SbkDoor", M2Share.GameCommand.SbkDoor.CommandName);
            }
            else
            {
                M2Share.GameCommand.SbkDoor.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SbkDoor", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SbkDoor", M2Share.GameCommand.SbkDoor.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SbkDoor.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeDearName", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeDearName", M2Share.GameCommand.ChangeDearName.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeDearName.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeDearName", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeDearName", M2Share.GameCommand.ChangeDearName.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeDearName.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ChangeMasterName", "");
            if (LoadString == "")
            {
                WriteString("Command", "ChangeMasterrName", M2Share.GameCommand.ChangeMasterName.CommandName);
            }
            else
            {
                M2Share.GameCommand.ChangeMasterName.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ChangeMasterName", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ChangeMasterName", M2Share.GameCommand.ChangeMasterName.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ChangeMasterName.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "StartQuest", "");
            if (LoadString == "")
            {
                WriteString("Command", "StartQuest", M2Share.GameCommand.StartQuest.CommandName);
            }
            else
            {
                M2Share.GameCommand.StartQuest.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "StartQuest", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "StartQuest", M2Share.GameCommand.StartQuest.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.StartQuest.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SetPermission", "");
            if (LoadString == "")
            {
                WriteString("Command", "SetPermission", M2Share.GameCommand.SetperMission.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetperMission.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SetPermission", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SetPermission", M2Share.GameCommand.SetperMission.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SetperMission.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ClearMon", "");
            if (LoadString == "")
            {
                WriteString("Command", "ClearMon", M2Share.GameCommand.ClearMon.CommandName);
            }
            else
            {
                M2Share.GameCommand.ClearMon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ClearMon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ClearMon", M2Share.GameCommand.ClearMon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ClearMon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ReNewLevel", "");
            if (LoadString == "")
            {
                WriteString("Command", "ReNewLevel", M2Share.GameCommand.RenewLevel.CommandName);
            }
            else
            {
                M2Share.GameCommand.RenewLevel.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ReNewLevel", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ReNewLevel", M2Share.GameCommand.RenewLevel.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.RenewLevel.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DenyIPaddrLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DenyIPaddrLogon", M2Share.GameCommand.DenyipLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DenyipLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DenyIPaddrLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DenyIPaddrLogon", M2Share.GameCommand.DenyipLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DenyipLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DenyAccountLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DenyAccountLogon", M2Share.GameCommand.DenyAccountLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DenyAccountLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DenyAccountLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DenyAccountLogon", M2Share.GameCommand.DenyAccountLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DenyAccountLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DenyCharNameLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DenyCharNameLogon", M2Share.GameCommand.DenyCharNameLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DenyCharNameLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DenyCharNameLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DenyCharNameLogon", M2Share.GameCommand.DenyCharNameLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DenyCharNameLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelDenyIPLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelDenyIPLogon", M2Share.GameCommand.DelDenyIpLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelDenyIpLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelDenyIPLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelDenyIPLogon", M2Share.GameCommand.DelDenyIpLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelDenyIpLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelDenyAccountLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelDenyAccountLogon", M2Share.GameCommand.DelDenyAccountLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelDenyAccountLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelDenyAccountLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelDenyAccountLogon", M2Share.GameCommand.DelDenyAccountLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelDenyAccountLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "DelDenyCharNameLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "DelDenyCharNameLogon", M2Share.GameCommand.DelDenyCharNameLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.DelDenyCharNameLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "DelDenyCharNameLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "DelDenyCharNameLogon", M2Share.GameCommand.DelDenyCharNameLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.DelDenyCharNameLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShowDenyIPLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShowDenyIPLogon", M2Share.GameCommand.ShowDenyIpLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowDenyIpLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShowDenyIPLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShowDenyIPLogon", M2Share.GameCommand.ShowDenyIpLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowDenyIpLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShowDenyAccountLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShowDenyAccountLogon", M2Share.GameCommand.ShowDenyAccountLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowDenyAccountLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShowDenyAccountLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShowDenyAccountLogon", M2Share.GameCommand.ShowDenyAccountLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowDenyAccountLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShowDenyCharNameLogon", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShowDenyCharNameLogon", M2Share.GameCommand.ShowDenyCharNameLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowDenyCharNameLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShowDenyCharNameLogon", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShowDenyCharNameLogon", M2Share.GameCommand.ShowDenyCharNameLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowDenyCharNameLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ViewWhisper", "");
            if (LoadString == "")
            {
                WriteString("Command", "ViewWhisper", M2Share.GameCommand.ViewWhisper.CommandName);
            }
            else
            {
                M2Share.GameCommand.ViewWhisper.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ViewWhisper", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ViewWhisper", M2Share.GameCommand.ViewWhisper.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ViewWhisper.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SpiritStart", "");
            if (LoadString == "")
            {
                WriteString("Command", "SpiritStart", M2Share.GameCommand.Spirit.CommandName);
            }
            else
            {
                M2Share.GameCommand.Spirit.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SpiritStart", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SpiritStart", M2Share.GameCommand.Spirit.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.Spirit.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SpiritStop", "");
            if (LoadString == "")
            {
                WriteString("Command", "SpiritStop", M2Share.GameCommand.SpiritStop.CommandName);
            }
            else
            {
                M2Share.GameCommand.SpiritStop.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SpiritStop", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SpiritStop", M2Share.GameCommand.SpiritStop.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SpiritStop.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "SetMapMode", "");
            if (LoadString == "")
            {
                WriteString("Command", "SetMapMode", M2Share.GameCommand.SetMapMode.CommandName);
            }
            else
            {
                M2Share.GameCommand.SetMapMode.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "SetMapMode", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "SetMapMode", M2Share.GameCommand.SetMapMode.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.SetMapMode.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ShoweMapMode", "");
            if (LoadString == "")
            {
                WriteString("Command", "ShoweMapMode", M2Share.GameCommand.ShowMapMode.CommandName);
            }
            else
            {
                M2Share.GameCommand.ShowMapMode.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ShoweMapMode", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ShoweMapMode", M2Share.GameCommand.ShowMapMode.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ShowMapMode.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "ClearBag", "");
            if (LoadString == "")
            {
                WriteString("Command", "ClearBag", M2Share.GameCommand.ClearBag.CommandName);
            }
            else
            {
                M2Share.GameCommand.ClearBag.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "ClearBag", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "ClearBag", M2Share.GameCommand.ClearBag.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.ClearBag.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "LockLogin", "");
            if (LoadString == "")
            {
                WriteString("Command", "LockLogin", M2Share.GameCommand.LockLogon.CommandName);
            }
            else
            {
                M2Share.GameCommand.LockLogon.CommandName = LoadString;
            }
            nLoadInteger = ReadInteger("Permission", "LockLogin", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "LockLogin", M2Share.GameCommand.LockLogon.nPerMissionMin);
            }
            else
            {
                M2Share.GameCommand.LockLogon.nPerMissionMin = nLoadInteger;
            }
            LoadString = ReadString("Command", "GMRedMsgCmd", "");
            if (LoadString == "")
            {
                WriteString("Command", "GMRedMsgCmd", M2Share.g_GMRedMsgCmd);
            }
            else
            {
                M2Share.g_GMRedMsgCmd = LoadString[0];
            }
            nLoadInteger = ReadInteger("Permission", "GMRedMsgCmd", -1);
            if (nLoadInteger < 0)
            {
                WriteInteger("Permission", "GMRedMsgCmd", M2Share.g_nGMREDMSGCMD);
            }
            else
            {
                M2Share.g_nGMREDMSGCMD = nLoadInteger;
            }

            foreach (var item in M2Share.GameCommand.GetType().GetFields())
            {
                M2Share.CustomCommands.Add((GameCmd)item.GetValue(M2Share.GameCommand));
            }
        }
    }
}