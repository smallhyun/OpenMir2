﻿using GameSvr.Player;
using SystemModule.Data;
using SystemModule.Enums;

namespace GameSvr.GameCommand.Commands
{
    /// <summary>
    /// 将指定玩家添加到禁止人物列表
    /// </summary>
    [Command("DenyChrNameLogon", "将指定玩家添加到禁止人物列表", "人物名称 是否永久封(0,1)", 10)]
    public class DenyChrNameLogonCommand : GameCommand
    {
        [ExecuteCommand]
        public void DenyChrNameLogon(string[] @Params, PlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            string sChrName = @Params.Length > 0 ? @Params[0] : "";
            string sFixDeny = @Params.Length > 1 ? @Params[1] : "";
            if (sChrName == "")
            {
                PlayObject.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            try
            {
                if (sFixDeny != "" && sFixDeny[0] == '1')
                {
                    //Settings.g_DenyChrNameList.Add(sChrName, ((1) as Object));
                    M2Share.SaveDenyChrNameList();
                    PlayObject.SysMsg(sChrName + "已加入禁止人物列表", MsgColor.Green, MsgType.Hint);
                }
                else
                {
                    //Settings.g_DenyChrNameList.Add(sChrName, ((0) as Object));
                    PlayObject.SysMsg(sChrName + "已加入临时禁止人物列表", MsgColor.Green, MsgType.Hint);
                }
            }
            finally
            {
            }
        }
    }
}