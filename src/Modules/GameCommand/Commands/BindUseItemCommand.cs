﻿using SystemModule;
using SystemModule.Data;
using SystemModule.Enums;

namespace CommandModule.Commands
{
    [Command("BindUseItem", "", CommandHelp.GameCommandBindUseItemHelpMsg, 10)]
    public class BindUseItemCommand : GameCommand
    {
        [ExecuteCommand]
        public void Execute(string[] @params, IPlayerActor PlayerActor)
        {
            if (@params == null || @params.Length <= 0)
            {
                return;
            }
            var sHumanName = @params.Length > 0 ? @params[0] : "";
            var sItem = @params.Length > 1 ? @params[1] : "";
            var sType = @params.Length > 2 ? @params[2] : "";
            var sLight = @params.Length > 3 ? @params[3] : "";
            var nBind = -1;
            var nItem = ModuleShare.GetUseItemIdx(sItem);
            if (string.Compare(sType, "帐号", StringComparison.OrdinalIgnoreCase) == 0)
            {
                nBind = 0;
            }
            if (string.Compare(sType, "人物", StringComparison.OrdinalIgnoreCase) == 0)
            {
                nBind = 1;
            }
            if (string.Compare(sType, "IP", StringComparison.OrdinalIgnoreCase) == 0)
            {
                nBind = 2;
            }
            if (string.Compare(sType, "死亡", StringComparison.OrdinalIgnoreCase) == 0)
            {
                nBind = 3;
            }
            var boLight = sLight == "1";
            if (nItem < 0 || nBind < 0 || string.IsNullOrEmpty(sHumanName) || !string.IsNullOrEmpty(sHumanName) && sHumanName[1] == '?')
            {
                PlayerActor.SysMsg(Command.CommandHelp, MsgColor.Red, MsgType.Hint);
                return;
            }
            var mIPlayerActor = ModuleShare.WorldEngine.GetPlayObject(sHumanName);
            if (mIPlayerActor == null)
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.NowNotOnLineOrOnOtherServer, sHumanName), MsgColor.Red, MsgType.Hint);
                return;
            }
            var userItem = mIPlayerActor.UseItems[nItem];
            if (userItem.Index == 0)
            {
                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandBindUseItemNoItemMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                return;
            }
            int nItemIdx = userItem.Index;
            var nMakeIdex = userItem.MakeIndex;
            ItemBind itemBind;
            string sBindName;
            bool boFind;
            switch (nBind)
            {
                case 0:
                    boFind = false;
                    sBindName = mIPlayerActor.UserAccount;
                    HUtil32.EnterCriticalSection(ModuleShare.ItemBindAccount);
                    try
                    {
                        for (var i = 0; i < ModuleShare.ItemBindAccount.Count; i++)
                        {
                            itemBind = ModuleShare.ItemBindAccount[i];
                            if (itemBind.nItemIdx == nItemIdx && itemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            itemBind = new ItemBind();
                            itemBind.nItemIdx = nItemIdx;
                            itemBind.nMakeIdex = nMakeIdex;
                            itemBind.sBindName = sBindName;
                            ModuleShare.ItemBindAccount.Insert(0, itemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(ModuleShare.ItemBindAccount);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    ModuleShare.SaveItemBindAccount();
                    PlayerActor.SysMsg(string.Format("{0}[{1}]IDX[{2}]系列号[{3}]持久[{4}-{5}]，绑定到{6}成功。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), userItem.Index, userItem.MakeIndex, userItem.Dura, userItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    mIPlayerActor.SysMsg(string.Format("你的{0}[{1}]已经绑定到{2}[{3}]上了。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), sType, sBindName), MsgColor.Blue, MsgType.Hint);
                    mIPlayerActor.SendMsg(PlayerActor, Messages.RM_SENDUSEITEMS, 0, 0, 0, 0);
                    break;
                case 1:
                    sBindName = mIPlayerActor.ChrName;
                    boFind = false;
                    HUtil32.EnterCriticalSection(ModuleShare.ItemBindChrName);
                    try
                    {
                        for (var i = 0; i < ModuleShare.ItemBindChrName.Count; i++)
                        {
                            itemBind = ModuleShare.ItemBindChrName[i];
                            if (itemBind.nItemIdx == nItemIdx && itemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            itemBind = new ItemBind();
                            itemBind.nItemIdx = nItemIdx;
                            itemBind.nMakeIdex = nMakeIdex;
                            itemBind.sBindName = sBindName;
                            ModuleShare.ItemBindChrName.Insert(0, itemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(ModuleShare.ItemBindChrName);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    ModuleShare.SaveItemBindChrName();
                    PlayerActor.SysMsg(string.Format("{0}[{1}]IDX[{2}]系列号[{3}]持久[{4}-{5}]，绑定到{6}成功。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), userItem.Index, userItem.MakeIndex, userItem.Dura, userItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    mIPlayerActor.SysMsg(string.Format("你的{0}[{1}]已经绑定到{2}[{3}]上了。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), sType, sBindName), MsgColor.Blue, MsgType.Hint);
                    PlayerActor.SendUpdateItem(userItem);
                    mIPlayerActor.SendMsg(PlayerActor, Messages.RM_SENDUSEITEMS, 0, 0, 0, 0);
                    break;
                case 2:
                    boFind = false;
                    sBindName = mIPlayerActor.LoginIpAddr;
                    HUtil32.EnterCriticalSection(ModuleShare.ItemBindIPaddr);
                    try
                    {
                        for (var i = 0; i < ModuleShare.ItemBindIPaddr.Count; i++)
                        {
                            itemBind = ModuleShare.ItemBindIPaddr[i];
                            if (itemBind.nItemIdx == nItemIdx && itemBind.nMakeIdex == nMakeIdex)
                            {
                                PlayerActor.SysMsg(string.Format(CommandHelp.GameCommandBindUseItemAlreadBindMsg, sHumanName, sItem), MsgColor.Red, MsgType.Hint);
                                boFind = true;
                                break;
                            }
                        }
                        if (!boFind)
                        {
                            itemBind = new ItemBind();
                            itemBind.nItemIdx = nItemIdx;
                            itemBind.nMakeIdex = nMakeIdex;
                            itemBind.sBindName = sBindName;
                            ModuleShare.ItemBindIPaddr.Insert(0, itemBind);
                        }
                    }
                    finally
                    {
                        HUtil32.LeaveCriticalSection(ModuleShare.ItemBindIPaddr);
                    }
                    if (boFind)
                    {
                        return;
                    }
                    ModuleShare.SaveItemBindIPaddr();
                    PlayerActor.SysMsg(string.Format("{0}[{1}]IDX[{2}]系列号[{3}]持久[{4}-{5}]，绑定到{6}成功。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), userItem.Index, userItem.MakeIndex, userItem.Dura, userItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    mIPlayerActor.SysMsg(string.Format("你的{0}[{1}]已经绑定到{2}[{3}]上了。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), sType, sBindName), MsgColor.Blue, MsgType.Hint);
                    PlayerActor.SendUpdateItem(userItem);
                    mIPlayerActor.SendMsg(PlayerActor, Messages.RM_SENDUSEITEMS, 0, 0, 0, 0);
                    break;
                case 3:// 人物装备死亡不爆绑定
                    sBindName = PlayerActor.ChrName;
                    for (var i = 0; i < ModuleShare.ItemBindDieNoDropName.Count; i++)
                    {
                        //ItemBind = Settings.g_ItemBindDieNoDropName[i];
                        //if ((ItemBind.nItemIdx == nItemIdx) && (ItemBind.sBindName == sBindName))
                        //{
                        //    this.SysMsg(string.Format(Settings.GameCommandBindUseItemAlreadBindMsg, new string[] { sHumanName, sItem }), MsgColor.c_Red, MsgType.t_Hint);
                        //    return;
                        //}
                    }
                    itemBind = new ItemBind
                    {
                        nItemIdx = nItemIdx,
                        nMakeIdex = 0,
                        sBindName = sBindName
                    };
                    //Settings.g_ItemBindDieNoDropName.InsertText(0, ItemBind);
                    //M2Share.SaveItemBindDieNoDropName();// 保存人物装备死亡不爆列表
                    mIPlayerActor.SysMsg(string.Format("{0}[{1}]IDX[{2}]系列号[{3}]持久[{4}-{5}]，死亡不爆绑定到{6}成功。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), userItem.Index, userItem.MakeIndex, userItem.Dura, userItem.DuraMax, sBindName), MsgColor.Blue, MsgType.Hint);
                    PlayerActor.SysMsg(string.Format("您的{0}[{1}]已经绑定到{2}[{3}]上了。", ModuleShare.GetUseItemName(nItem), ModuleShare.ItemSystem.GetStdItemName(userItem.Index), sType, sBindName), MsgColor.Blue, MsgType.Hint);
                    break;
            }
        }
    }
}