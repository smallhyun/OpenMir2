﻿using GameSvr.Player;
using SystemModule.Data;

namespace GameSvr.Command.Commands
{
    [GameCommand("Letguild", "退出公会", "", 0)]
    public class LetGuildCommand : BaseCommond
    {
        [DefaultCommand]
        public void Letguild(PlayObject playObject)
        {
            playObject.AllowGuild = !playObject.AllowGuild;
            if (playObject.AllowGuild)
            {
                playObject.SysMsg(GameCommandConst.EnableJoinGuild, MsgColor.Green, MsgType.Hint);
            }
            else
            {
                playObject.SysMsg(GameCommandConst.DisableJoinGuild, MsgColor.Green, MsgType.Hint);
            }
            return;
        }
    }
}