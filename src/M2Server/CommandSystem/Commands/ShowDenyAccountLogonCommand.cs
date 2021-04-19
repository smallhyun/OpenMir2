﻿using SystemModule;
using M2Server.CommandSystem;

namespace M2Server
{
    [GameCommand("ShowDenyAccountLogon", "", 10)]
    public class ShowDenyAccountLogonCommand : BaseCommond
    {
        [DefaultCommand]
        public void ShowDenyAccountLogon(string[] @Params, TPlayObject PlayObject)
        {
            if (PlayObject.m_btPermission < 6)
            {
                return;
            }
            try
            {
                if (M2Share.g_DenyAccountList.Count <= 0)
                {
                    PlayObject.SysMsg("禁止登录帐号列表为空。", TMsgColor.c_Green, TMsgType.t_Hint);
                    return;
                }
                for (var i = 0; i < M2Share.g_DenyAccountList.Count; i++)
                {
                    //PlayObject.SysMsg(M2Share.g_DenyAccountList[i], TMsgColor.c_Green, TMsgType.t_Hint);
                }
            }
            finally
            {
            }
        }
    }
}