﻿using GameSvr.Player;
using SystemModule.Data;

namespace GameSvr.Command.Commands
{
    [GameCommand("TakeOnHorse", "", 10)]
    public class TakeOnHorseCommand : BaseCommond
    {
        [DefaultCommand]
        public void TakeOnHorse(PlayObject PlayObject)
        {
            if (PlayObject.OnHorse)
            {
                return;
            }
            if (PlayObject.HorseType == 0)
            {
                PlayObject.SysMsg("骑马必须先戴上马牌!!!", MsgColor.Red, MsgType.Hint);
                return;
            }
            PlayObject.OnHorse = true;
            PlayObject.FeatureChanged();
            if (PlayObject.OnHorse)
            {
                M2Share.g_FunctionNPC.GotoLable(PlayObject, "@OnHorse", false);
            }
        }
    }
}