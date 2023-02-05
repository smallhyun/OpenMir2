﻿using GameSvr.Player;
using SystemModule.Data;
using SystemModule.Enums;

namespace GameSvr.GameCommand.Commands
{
    /// <summary>
    /// 设置怪物行动速度
    /// </summary>
    [Command("ChangeZenFastStep", "设置怪物行动速度", "速度", 10)]
    public class ChangeZenFastStepCommand : GameCommand
    {
        [ExecuteCommand]
        public void ChangeZenFastStep(string[] @Params, PlayObject PlayObject)
        {
            if (@Params == null)
            {
                return;
            }
            string sFastStep = @Params.Length > 0 ? @Params[0] : "";
            int nFastStep = HUtil32.StrToInt(sFastStep, -1);
            if (sFastStep == "" || nFastStep < 1 || sFastStep != "")
            {
                PlayObject.SysMsg("设置怪物行动速度。", MsgColor.Red, MsgType.Hint);
                PlayObject.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            M2Share.Config.ZenFastStep = nFastStep;
            PlayObject.SysMsg($"怪物行动速度: {nFastStep}", MsgColor.Green, MsgType.Hint);
        }
    }
}