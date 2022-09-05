﻿using GameSvr.Actor;
using GameSvr.Player;
using SystemModule.Data;

namespace GameSvr.Command.Commands
{
    /// <summary>
    /// 显示你屏幕上你近处所有怪与人的详细情况
    /// </summary>
    [GameCommand("MobLevel", "显示你屏幕上你近处所有怪与人的详细情况", 10)]
    public class MobLevelCommand : BaseCommond
    {
        [DefaultCommand]
        public void MobLevel(PlayObject PlayObject)
        {
            IList<BaseObject> BaseObjectList = new List<BaseObject>();
            PlayObject.Envir.GetRangeBaseObject(PlayObject.CurrX, PlayObject.CurrY, 2, true, BaseObjectList);
            for (var i = 0; i < BaseObjectList.Count; i++)
            {
                PlayObject.SysMsg(BaseObjectList[i].GeTBaseObjectInfo(), MsgColor.Green, MsgType.Hint);
            }
            BaseObjectList.Clear();
            BaseObjectList = null;
        }
    }
}