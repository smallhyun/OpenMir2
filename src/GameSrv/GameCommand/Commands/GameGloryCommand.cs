﻿using GameSrv.Player;

namespace GameSrv.GameCommand.Commands {
    [Command("GameGlory", "调整玩家灵符", 10)]
    public class GameGloryCommand : GameCommand {
        [ExecuteCommand]
        public void Execute(string[] @params, PlayObject playObject) {
            //string sHumanName = @Params.Length > 0 ? @Params[1] : "";
            //string sCtr = @Params.Length > 1 ? @Params[2] : "";
            //int nGameGlory = @Params.Length > 2 ? HUtil32.StrToInt(@Params[3]) : 0;
            //var Ctr = '1';
            //if ((PlayObject.m_btPermission < this.Attributes.nPermissionMin))
            //{
            //    PlayObject.SysMsg(Settings.GameCommandPermissionTooLow, MsgColor.c_Red, MsgType.t_Hint); // 权限不够
            //    return;
            //}

            //if ((sCtr != ""))
            //{
            //    Ctr = sCtr[0];
            //}

            //if ((string.IsNullOrEmpty(sHumanName)) || !(new ArrayList(new string[] { "=", "+", "-" }).Contains(Ctr)) ||
            //    (nGameGlory < 0) || (nGameGlory > 255) || ((!string.IsNullOrEmpty(sHumanName)) && (sHumanName[0] == '?')))
            //{
            //    {
            //        PlayObject.SysMsg(
            //            string.Format(Settings.GameCommandParamUnKnow, this.Attributes.Name,
            //                Settings.GameCommandGameGloryHelpMsg), MsgColor.c_Red, MsgType.t_Hint);
            //        return;
            //    }

            //    var TargerObject = M2Share.WorldEngine.GePlayObject(sHumanName);
            //    if (TargerObject == null)
            //    {
            //        PlayObject.SysMsg(string.Format(Settings.NowNotOnLineOrOnOtherServer, new string[] { sHumanName }),
            //            MsgColor.c_Red, MsgType.t_Hint);
            //        return;
            //    }

            //    switch (sCtr[0])
            //    {
            //        case '=':
            //            TargerObject.m_btGameGlory = (byte)nGameGlory;
            //            break;
            //        case '+':
            //            TargerObject.m_btGameGlory += (byte)nGameGlory;
            //            break;
            //        case '-':
            //            TargerObject.m_btGameGlory -= (byte)nGameGlory;
            //            break;
            //    }

            //    if (Settings.g_boGameLogGameGlory)
            //    {
            //        M2Share.ItemEventSource.AddGameLog(string.Format(Settings.GameLogMsg1, M2Share.LOG_GameGlory,
            //            TargerObject.MapName, TargerObject.CurrX, TargerObject.CurrY, TargerObject.m_sChrName,
            //            Settings.Config.sGameGlory, TargerObject.m_btGameGlory,
            //            sCtr[1] + "(" + (nGameGlory).ToString() + ")", PlayObject.m_sChrName));
            //    }

            //    TargerObject.GameGloryChanged();
            //    TargerObject.SysMsg(
            //        string.Format(Settings.GameCommandGameGirdHumanMsg, Settings.Config.sGameGlory, nGameGlory,
            //            TargerObject.m_btGameGlory, Settings.Config.sGameGlory), MsgColor.c_Green, MsgType.t_Hint);
            //    PlayObject.SysMsg(
            //        string.Format(Settings.GameCommandGameGirdGMMsg, sHumanName, Settings.Config.sGameGlory,
            //            nGameGlory, TargerObject.m_btGameGlory, Settings.Config.sGameGlory), MsgColor.c_Green,
            //        MsgType.t_Hint);
            //}
        }
    }
}