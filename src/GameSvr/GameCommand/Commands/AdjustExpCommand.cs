﻿using GameSvr.Player;

namespace GameSvr.GameCommand.Commands
{
    /// <summary>
    /// 暂时不清楚干啥的
    /// </summary>
    [Command("AdjustExp", "", 10)]
    public class AdjustExpCommand : GameCommand
    {
        [ExecuteCommand]
        public static void AdjustExp(string[] @Params, PlayObject PlayObject)
        {
            if (PlayObject.Permission < 6)
            {
                return;
            }
        }
    }
}