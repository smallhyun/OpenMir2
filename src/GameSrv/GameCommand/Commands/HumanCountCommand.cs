﻿using GameSrv.Player;
using SystemModule.Enums;

namespace GameSrv.GameCommand.Commands
{
    /// <summary>
    /// 取指定地图玩家数量
    /// </summary>
    [Command("HumanCount", "取指定地图玩家数量", CommandHelp.GameCommandHumanCountHelpMsg, 10)]
    public class HumanCountCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @Params, PlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            string sMapName = @Params.Length > 0 ? @Params[0] : "";
            if (string.IsNullOrEmpty(sMapName))
            {
                PlayObject.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            Maps.Envirnoment Envir = M2Share.MapMgr.FindMap(sMapName);
            if (Envir == null)
            {
                PlayObject.SysMsg(CommandHelp.GameCommandMobCountMapNotFound, MsgColor.Red, MsgType.Hint);
                return;
            }
            PlayObject.SysMsg(string.Format(CommandHelp.GameCommandMobCountMonsterCount, M2Share.WorldEngine.GetMapHuman(sMapName)), MsgColor.Green, MsgType.Hint);
        }
    }
}