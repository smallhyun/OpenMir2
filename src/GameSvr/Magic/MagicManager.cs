﻿using GameSvr.Actor;
using GameSvr.Event.Events;
using GameSvr.Player;
using SystemModule;
using SystemModule.Consts;
using SystemModule.Data;
using SystemModule.Enums;
using SystemModule.Packets.ClientPackets;

namespace GameSvr.Magic
{
    public class MagicManager
    {
        /// <summary>
        /// 宠物叛变时间
        /// </summary>
        private const int DwRoyaltySec = 10 * 24 * 60 * 60;
        
        private static int MagPushArround(BaseObject playObject, int nPushLevel)
        {
            var result = 0;
            for (var i = 0; i < playObject.VisibleActors.Count; i++)
            {
                var baseObject = playObject.VisibleActors[i].BaseObject;
                if (Math.Abs(playObject.CurrX - baseObject.CurrX) <= 1 && Math.Abs(playObject.CurrY - baseObject.CurrY) <= 1)
                {
                    if (!baseObject.Death && baseObject != playObject)
                    {
                        if (playObject.Abil.Level > baseObject.Abil.Level && !baseObject.StickMode)
                        {
                            var levelgap = playObject.Abil.Level - baseObject.Abil.Level;
                            if (M2Share.RandomNumber.Random(20) < 6 + nPushLevel * 3 + levelgap)
                            {
                                if (playObject.IsProperTarget(baseObject))
                                {
                                    var push = 1 + HUtil32._MAX(0, nPushLevel - 1) + M2Share.RandomNumber.Random(2);
                                    var nDir = M2Share.GetNextDirection(playObject.CurrX, playObject.CurrY, baseObject.CurrX, baseObject.CurrY);
                                    baseObject.CharPushed(nDir, push);
                                    result++;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static bool MagBigHealing(PlayObject playObject, int nPower, int nX, int nY)
        {
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            playObject.GetMapBaseObjects(playObject.Envir, nX, nY, 1, baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var baseObject = baseObjectList[i];
                if (playObject.IsProperFriend(baseObject))
                {
                    if (baseObject.WAbil.HP < baseObject.WAbil.MaxHP)
                    {
                        baseObject.SendDelayMsg(playObject, Grobal2.RM_MAGHEALING, 0, nPower, 0, 0, "", 800);
                        result = true;
                    }
                    if (playObject.AbilSeeHealGauge)
                    {
                        playObject.SendMsg(baseObject, Grobal2.RM_10414, 0, 0, 0, 0, "");
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 是否战士技能
        /// </summary>
        /// <returns></returns>
        public static bool IsWarrSkill(int wMagIdx)
        {
            var result = false;
            switch (wMagIdx)
            {
                case MagicConst.SKILL_ONESWORD:
                case MagicConst.SKILL_ILKWANG:
                case MagicConst.SKILL_YEDO:
                case MagicConst.SKILL_ERGUM:
                case MagicConst.SKILL_BANWOL:
                case MagicConst.SKILL_FIRESWORD:
                case MagicConst.SKILL_MOOTEBO:
                case MagicConst.SKILL_CROSSMOON:
                case MagicConst.SKILL_TWINBLADE:
                    result = true;
                    break;
            }
            return result;
        }

        private static ushort MPow(UserMagic userMagic)
        {
            return (ushort)(userMagic.Magic.Power + M2Share.RandomNumber.Random(userMagic.Magic.MaxPower - userMagic.Magic.Power));
        }

        private static ushort GetPower(UserMagic userMagic, ushort nPower)
        {
            return (ushort)(HUtil32.Round(nPower / (userMagic.Magic.TrainLv + 1) * (userMagic.Level + 1)) + userMagic.Magic.DefPower +
                            M2Share.RandomNumber.Random(userMagic.Magic.DefMaxPower - userMagic.Magic.DefPower));
        }

        private static ushort GetPower13(UserMagic userMagic, int nInt)
        {
            var d10 = nInt / 3.0;
            var d18 = nInt - d10;
            var result = (ushort)HUtil32.Round(d18 / (userMagic.Magic.TrainLv + 1) * (userMagic.Level + 1) + d10 + (userMagic.Magic.DefPower + M2Share.RandomNumber.Random(userMagic.Magic.DefMaxPower - userMagic.Magic.DefPower)));
            return result;
        }

        private static ushort GetRPow(int wInt)
        {
            ushort result;
            if (HUtil32.HiByte(wInt) > HUtil32.LoByte(wInt))
            {
                result = (ushort)(M2Share.RandomNumber.Random(HUtil32.HiByte(wInt) - HUtil32.LoByte(wInt) + 1) + HUtil32.LoByte(wInt));
            }
            else
            {
                result = HUtil32.LoByte(wInt);
            }
            return result;
        }

        public static void DoSpell_sub_4934B4(PlayObject playObject)
        {
            if (playObject.UseItems[Grobal2.U_ARMRINGL].Dura < 100)
            {
                playObject.UseItems[Grobal2.U_ARMRINGL].Dura = 0;
                playObject.SendDelItems(playObject.UseItems[Grobal2.U_ARMRINGL]);
                playObject.UseItems[Grobal2.U_ARMRINGL].Index = 0;
            }
        }

        public static bool DoSpell(PlayObject playObject, UserMagic userMagic, short nTargetX, short nTargetY, BaseObject targetObject)
        {
            short n14 = 0;
            short n18 = 0;
            byte n1C;
            short nAmuletIdx = 0;
            if (IsWarrSkill(userMagic.MagIdx))
            {
                return false;
            }
            if ((Math.Abs(playObject.CurrX - nTargetX) > M2Share.Config.MagicAttackRage) || (Math.Abs(playObject.CurrY - nTargetY) > M2Share.Config.MagicAttackRage))
            {
                return false;
            }
            playObject.SendRefMsg(Grobal2.RM_SPELL, userMagic.Magic.Effect, nTargetX, nTargetY, userMagic.Magic.MagicId, "");
            if (targetObject != null && targetObject.Death)
            {
                return false;
            }
            var boTrain = false;
            var boSpellFail = false;
            var boSpellFire = true;
            if (playObject.SoftVersionDateEx == 0 && playObject.MDwClientTick == 0)
            {
                return false;
            }
            ushort nPower;
            switch (userMagic.Magic.MagicId)
            {
                case MagicConst.SKILL_FIREBALL:
                case MagicConst.SKILL_FIREBALL2:
                    if (playObject.MagCanHitTarget(playObject.CurrX, playObject.CurrY, targetObject))
                    {
                        if (playObject.IsProperTarget(targetObject))
                        {
                            if (targetObject.AntiMagic <= M2Share.RandomNumber.Random(10) && Math.Abs(targetObject.CurrX - nTargetX) <= 1 && Math.Abs(targetObject.CurrY - nTargetY) <= 1)
                            {
                                nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                                playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 600);
                                if (targetObject.Race >= ActorRace.Animal)
                                {
                                    boTrain = true;
                                }
                            }
                            else
                            {
                                targetObject = null;
                            }
                        }
                        else
                        {
                            targetObject = null;
                        }
                    }
                    else
                    {
                        targetObject = null;
                    }
                    break;
                case MagicConst.SKILL_HEALLING:
                    if (targetObject == null)
                    {
                        targetObject = playObject;
                        nTargetX = playObject.CurrX;
                        nTargetY = playObject.CurrY;
                    }
                    if (playObject.IsProperFriend(targetObject))
                    {
                        nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.SC) * 2, (HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC)) * 2 + 1);
                        if (targetObject.WAbil.HP < targetObject.WAbil.MaxHP)
                        {
                            targetObject.SendDelayMsg(playObject, Grobal2.RM_MAGHEALING, 0, nPower, 0, 0, "", 800);
                            boTrain = true;
                        }
                        if (playObject.AbilSeeHealGauge)
                        {
                            playObject.SendMsg(targetObject, Grobal2.RM_10414, 0, 0, 0, 0, "");
                        }
                    }
                    break;
                case MagicConst.SKILL_AMYOUNSUL:
                    boSpellFail = true;
                    if (playObject.IsProperTarget(targetObject))
                    {
                        if (MagicBase.CheckAmulet(playObject, 1, 2, ref nAmuletIdx))
                        {
                            var stdItem = M2Share.WorldEngine.GetStdItem(playObject.UseItems[nAmuletIdx].Index);
                            if (stdItem != null)
                            {
                                MagicBase.UseAmulet(playObject, 1, 2, ref nAmuletIdx);
                                if (M2Share.RandomNumber.Random(targetObject.AntiPoison + 7) <= 6)
                                {
                                    switch (stdItem.Shape)
                                    {
                                        case 1:
                                            nPower = (ushort)(GetPower13(userMagic, 40) + GetRPow(playObject.WAbil.SC) * 2);// 中毒类型 - 绿毒
                                            targetObject.SendDelayMsg(playObject, Grobal2.RM_POISON, PoisonState.DECHEALTH, nPower, playObject.ActorId, HUtil32.Round(userMagic.Level / 3 * (nPower / M2Share.Config.AmyOunsulPoint)), "", 1000);
                                            break;
                                        case 2:
                                            nPower = (ushort)(GetPower13(userMagic, 30) + GetRPow(playObject.WAbil.SC) * 2);// 中毒类型 - 红毒
                                            targetObject.SendDelayMsg(playObject, Grobal2.RM_POISON, PoisonState.DAMAGEARMOR, nPower, playObject.ActorId, HUtil32.Round(userMagic.Level / 3 * (nPower / M2Share.Config.AmyOunsulPoint)), "", 1000);
                                            break;
                                    }
                                    if (targetObject.Race == ActorRace.Play || targetObject.Race >= ActorRace.Animal)
                                    {
                                        boTrain = true;
                                    }
                                }
                                playObject.SetTargetCreat(targetObject);
                                boSpellFail = false;
                            }
                        }
                    }
                    break;
                case MagicConst.SKILL_FIREWIND:
                    if (MagPushArround(playObject, userMagic.Level) > 0)
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_FIRE:
                    n1C = M2Share.GetNextDirection(playObject.CurrX, playObject.CurrY, nTargetX, nTargetY);
                    if (playObject.Envir.GetNextPosition(playObject.CurrX, playObject.CurrY, n1C, 1, ref n14, ref n18))
                    {
                        playObject.Envir.GetNextPosition(playObject.CurrX, playObject.CurrY, n1C, 5, ref nTargetX, ref nTargetY);
                        nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                        if (playObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, false) > 0)
                        {
                            boTrain = true;
                        }
                    }
                    break;
                case MagicConst.SKILL_SHOOTLIGHTEN:
                    n1C = M2Share.GetNextDirection(playObject.CurrX, playObject.CurrY, nTargetX, nTargetY);
                    if (playObject.Envir.GetNextPosition(playObject.CurrX, playObject.CurrY, n1C, 1, ref n14, ref n18))
                    {
                        playObject.Envir.GetNextPosition(playObject.CurrX, playObject.CurrY, n1C, 8, ref nTargetX, ref nTargetY);
                        nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), (ushort)(HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1));
                        if (playObject.MagPassThroughMagic(n14, n18, nTargetX, nTargetY, n1C, nPower, true) > 0)
                        {
                            boTrain = true;
                        }
                    }
                    break;
                case MagicConst.SKILL_LIGHTENING:
                    if (playObject.IsProperTarget(targetObject))
                    {
                        if (M2Share.RandomNumber.Random(10) >= targetObject.AntiMagic)
                        {
                            nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                            if (targetObject.LifeAttrib == Grobal2.LA_UNDEAD)
                            {
                                nPower = (ushort)HUtil32.Round(nPower * 1.5);
                            }
                            playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 600);
                            if (targetObject.Race >= ActorRace.Animal)
                            {
                                boTrain = true;
                            }
                        }
                        else
                        {
                            targetObject = null;
                        }
                    }
                    else
                    {
                        targetObject = null;
                    }
                    break;
                case MagicConst.SKILL_FIRECHARM:
                case MagicConst.SKILL_HANGMAJINBUB:
                case MagicConst.SKILL_DEJIWONHO:
                case MagicConst.SKILL_HOLYSHIELD:
                case MagicConst.SKILL_SKELLETON:
                case MagicConst.SKILL_CLOAK:
                case MagicConst.SKILL_BIGCLOAK:
                    boSpellFail = true;
                    if (MagicBase.CheckAmulet(playObject, 1, 1, ref nAmuletIdx))
                    {
                        MagicBase.UseAmulet(playObject, 1, 1, ref nAmuletIdx);
                        switch (userMagic.Magic.MagicId)
                        {
                            case MagicConst.SKILL_FIRECHARM:
                                if (playObject.MagCanHitTarget(playObject.CurrX, playObject.CurrY, targetObject))
                                {
                                    if (playObject.IsProperTarget(targetObject))
                                    {
                                        if (M2Share.RandomNumber.Random(10) >= targetObject.AntiMagic)
                                        {
                                            if (Math.Abs(targetObject.CurrX - nTargetX) <= 1 && Math.Abs(targetObject.CurrY - nTargetY) <= 1)
                                            {
                                                nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.SC), HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC) + 1);
                                                playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 1200);
                                                if (targetObject.Race >= ActorRace.Animal)
                                                {
                                                    boTrain = true;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    targetObject = null;
                                }
                                break;
                            case MagicConst.SKILL_HANGMAJINBUB:
                                nPower = playObject.GetAttackPower(GetPower13(userMagic, 60) + HUtil32.LoByte(playObject.WAbil.SC) * 10, HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC) + 1);
                                if (playObject.MagMakeDefenceArea(nTargetX, nTargetY, 3, nPower, 1) > 0)
                                {
                                    boTrain = true;
                                }
                                break;
                            case MagicConst.SKILL_DEJIWONHO:
                                nPower = playObject.GetAttackPower(GetPower13(userMagic, 60) + HUtil32.LoByte(playObject.WAbil.SC) * 10, HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC) + 1);
                                if (playObject.MagMakeDefenceArea(nTargetX, nTargetY, 3, nPower, 0) > 0)
                                {
                                    boTrain = true;
                                }
                                break;
                            case MagicConst.SKILL_HOLYSHIELD:
                                if (MagMakeHolyCurtain(playObject, GetPower13(userMagic, 40) + GetRPow(playObject.WAbil.SC) * 3, nTargetX, nTargetY) > 0)
                                {
                                    boTrain = true;
                                }
                                break;
                            case MagicConst.SKILL_SKELLETON:
                                if (MagMakeSlave(playObject, userMagic))
                                {
                                    boTrain = true;
                                }
                                break;
                            case MagicConst.SKILL_CLOAK:
                                if (MagMakePrivateTransparent(playObject, (ushort)(GetPower13(userMagic, 30) + GetRPow(playObject.WAbil.SC) * 3)))
                                {
                                    boTrain = true;
                                }
                                break;
                            case MagicConst.SKILL_BIGCLOAK:
                                if (MagMakeGroupTransparent(playObject, nTargetX, nTargetY, GetPower13(userMagic, 30) + GetRPow(playObject.WAbil.SC) * 3))
                                {
                                    boTrain = true;
                                }
                                break;
                        }
                        boSpellFail = false;
                    }
                    break;
                case MagicConst.SKILL_TAMMING:
                    if (playObject.IsProperTarget(targetObject))
                    {
                        if (MagTamming(playObject, targetObject, nTargetX, nTargetY, userMagic.Level))
                        {
                            boTrain = true;
                        }
                    }
                    break;
                case MagicConst.SKILL_SPACEMOVE:
                    var targerActors = targetObject == null ? 0 : targetObject.ActorId;
                    playObject.SendRefMsg(Grobal2.RM_MAGICFIRE, 0, HUtil32.MakeWord(userMagic.Magic.EffectType, userMagic.Magic.Effect), HUtil32.MakeLong(nTargetX, nTargetY), targerActors, "");
                    boSpellFire = false;
                    if (MagSaceMove(playObject, userMagic.Level))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_EARTHFIRE:
                    if (MagMakeFireCross(playObject, playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1), (ushort)(GetPower(userMagic, 10) + (GetRPow(playObject.WAbil.MC) >> 1)), nTargetX, nTargetY))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_FIREBOOM:
                    if (MagBigExplosion(playObject, playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1), nTargetX, nTargetY, M2Share.Config.FireBoomRage))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_LIGHTFLOWER:
                    if (MagElecBlizzard(playObject, playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC),
                        HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1)))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_SHOWHP:
                    if (targetObject != null && !targetObject.ShowHp)
                    {
                        if (M2Share.RandomNumber.Random(6) <= userMagic.Level + 3)
                        {
                            targetObject.ShowHpTick = HUtil32.GetTickCount();
                            targetObject.ShowHpInterval = GetPower13(userMagic, GetRPow(playObject.WAbil.SC) * 2 + 30) * 1000;
                            targetObject.SendDelayMsg(targetObject, Grobal2.RM_DOOPENHEALTH, 0, 0, 0, 0, "", 1500);
                            boTrain = true;
                        }
                    }
                    break;
                case MagicConst.SKILL_BIGHEALLING:
                    nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.SC) * 2, (HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC)) * 2 + 1);
                    if (MagBigHealing(playObject, nPower, nTargetX, nTargetY))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_SINSU:
                    boSpellFail = true;
                    if (MagicBase.CheckAmulet(playObject, 5, 1, ref nAmuletIdx))
                    {
                        MagicBase.UseAmulet(playObject, 5, 1, ref nAmuletIdx);
                        if (MagMakeSinSuSlave(playObject, userMagic))
                        {
                            boTrain = true;
                        }
                        boSpellFail = false;
                    }
                    break;
                case MagicConst.SKILL_ANGEL:
                    boSpellFail = true;
                    if (MagicBase.CheckAmulet(playObject, 2, 1, ref nAmuletIdx))
                    {
                        MagicBase.UseAmulet(playObject, 2, 1, ref nAmuletIdx);
                        if (MagMakeAngelSlave(playObject, userMagic))
                        {
                            boTrain = true;
                        }
                        boSpellFail = false;
                    }
                    break;
                case MagicConst.SKILL_SHIELD:
                    if (playObject.MagBubbleDefenceUp(userMagic.Level, GetPower(userMagic, (ushort)(GetRPow(playObject.WAbil.MC) + 15))))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_KILLUNDEAD:
                    if (playObject.IsProperTarget(targetObject))
                    {
                        if (MagTurnUndead(playObject, targetObject, nTargetX, nTargetY, userMagic.Level))
                        {
                            boTrain = true;
                        }
                    }
                    break;
                case MagicConst.SKILL_SNOWWIND:
                    if (MagBigExplosion(playObject, playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1), nTargetX, nTargetY, M2Share.Config.SnowWindRange))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_UNAMYOUNSUL:
                    if (targetObject == null)
                    {
                        targetObject = playObject;
                        nTargetX = playObject.CurrX;
                        nTargetY = playObject.CurrY;
                    }
                    if (playObject.IsProperFriend(targetObject))
                    {
                        if (M2Share.RandomNumber.Random(7) - (userMagic.Level + 1) < 0)
                        {
                            if (targetObject.StatusArr[PoisonState.DECHEALTH] != 0)
                            {
                                targetObject.StatusArr[PoisonState.DECHEALTH] = 1;
                                boTrain = true;
                            }
                            if (targetObject.StatusArr[PoisonState.DAMAGEARMOR] != 0)
                            {
                                targetObject.StatusArr[PoisonState.DAMAGEARMOR] = 1;
                                boTrain = true;
                            }
                            if (targetObject.StatusArr[PoisonState.STONE] != 0)
                            {
                                targetObject.StatusArr[PoisonState.STONE] = 1;
                                boTrain = true;
                            }
                        }
                    }
                    break;
                case MagicConst.SKILL_WINDTEBO:
                    if (MagWindTebo(playObject, userMagic))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_MABE:
                    nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                    if (MabMabe(playObject, targetObject, nPower, userMagic.Level, nTargetX, nTargetY))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_GROUPLIGHTENING:
                    if (MagGroupLightening(playObject, userMagic, nTargetX, nTargetY, targetObject, ref boSpellFire))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_GROUPAMYOUNSUL:
                    if (MagGroupAmyounsul(playObject, userMagic, nTargetX, nTargetY, targetObject))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_GROUPDEDING:
                    if (MagGroupDeDing(playObject, userMagic, nTargetX, nTargetY, targetObject))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_43:
                    break;
                case MagicConst.SKILL_44:
                    if (MagHbFireBall(playObject, userMagic, nTargetX, nTargetY, ref targetObject))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_45:
                    if (playObject.IsProperTarget(targetObject))
                    {
                        if (M2Share.RandomNumber.Random(10) >= targetObject.AntiMagic)
                        {
                            nPower = playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                            if (targetObject.LifeAttrib == Grobal2.LA_UNDEAD)
                            {
                                nPower = (ushort)HUtil32.Round(nPower * 1.5);
                            }
                            playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 600);
                            if (targetObject.Race >= ActorRace.Animal)
                            {
                                boTrain = true;
                            }
                        }
                        else
                        {
                            targetObject = null;
                        }
                    }
                    else
                    {
                        targetObject = null;
                    }
                    break;
                case MagicConst.SKILL_46:
                    if (MagMakeClone(playObject, userMagic))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_47:
                    if (MagBigExplosion(playObject, playObject.GetAttackPower(GetPower(userMagic, MPow(userMagic)) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1), nTargetX, nTargetY, M2Share.Config.FireBoomRage))
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_ENERGYREPULSOR:
                    if (MagPushArround(playObject, userMagic.Level) > 0)
                    {
                        boTrain = true;
                    }
                    break;
                case MagicConst.SKILL_49:
                    boTrain = true;
                    break;
                case MagicConst.SKILL_UENHANCER:
                    boSpellFail = true;
                    if (targetObject == null)
                    {
                        targetObject = playObject;
                        nTargetX = playObject.CurrX;
                        nTargetY = playObject.CurrY;
                    }
                    if (playObject.IsProperFriend(targetObject))
                    {
                        if (MagicBase.CheckAmulet(playObject, 1, 1, ref nAmuletIdx))
                        {
                            MagicBase.UseAmulet(playObject, 1, 1, ref nAmuletIdx);
                            nPower = (ushort)(userMagic.Level + 1 + M2Share.RandomNumber.Random(userMagic.Level));
                            n14 = (short)playObject.GetAttackPower(GetPower13(userMagic, 60) + HUtil32.LoByte(playObject.WAbil.SC) * 10, HUtil32.HiByte(playObject.WAbil.SC) - HUtil32.LoByte(playObject.WAbil.SC) + 1);
                            if (targetObject.AttPowerUp(nPower, n14))
                            {
                                boTrain = true;
                            }
                            boSpellFail = false;
                        }
                    }
                    break;
                case MagicConst.SKILL_51:// 灵魂召唤术
                    boTrain = true;
                    break;
                case MagicConst.SKILL_52:// 诅咒术
                    boTrain = true;
                    break;
                case MagicConst.SKILL_53:// 灵魂召唤术
                    boTrain = true;
                    break;
                case MagicConst.SKILL_54:// 诅咒术
                    boTrain = true;
                    break;
            }
            if (boSpellFail)
            {
                return false;
            }
            if (boSpellFire)
            {
                if (targetObject == null)
                {
                    playObject.SendRefMsg(Grobal2.RM_MAGICFIRE, 0, HUtil32.MakeWord(userMagic.Magic.EffectType, userMagic.Magic.Effect), HUtil32.MakeLong(nTargetX, nTargetY), 0, "");
                }
                else
                {
                    playObject.SendRefMsg(Grobal2.RM_MAGICFIRE, 0, HUtil32.MakeWord(userMagic.Magic.EffectType, userMagic.Magic.Effect), HUtil32.MakeLong(nTargetX, nTargetY), targetObject.ActorId, "");
                }
            }
            if (userMagic.Level < 3 && boTrain)
            {
                if (userMagic.Magic.TrainLevel[userMagic.Level] <= playObject.Abil.Level)
                {
                    playObject.TrainSkill(userMagic, M2Share.RandomNumber.Random(3) + 1);
                    if (!playObject.CheckMagicLevelup(userMagic))
                    {
                        playObject.SendDelayMsg(playObject, Grobal2.RM_MAGIC_LVEXP, 0, userMagic.Magic.MagicId, userMagic.Level, userMagic.TranPoint, "", 1000);
                    }
                }
            }
            return true;
        }

        public static bool MagMakePrivateTransparent(BaseObject baseObject, ushort nHTime)
        {
            if (baseObject.StatusArr[PoisonState.STATE_TRANSPARENT] > 0)
            {
                return false;
            }
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            baseObject.GetMapBaseObjects(baseObject.Envir, baseObject.CurrX, baseObject.CurrY, 9, baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var targetBaseObject = baseObjectList[i];
                if (targetBaseObject.Race >= ActorRace.Animal && targetBaseObject.TargetCret == baseObject)
                {
                    if (Math.Abs(targetBaseObject.CurrX - baseObject.CurrX) > 1 || Math.Abs(targetBaseObject.CurrY - baseObject.CurrY) > 1 || M2Share.RandomNumber.Random(2) == 0)
                    {
                        targetBaseObject.TargetCret = null;
                    }
                }
            }
            baseObjectList.Clear();
            baseObject.StatusArr[PoisonState.STATE_TRANSPARENT] = nHTime;
            baseObject.CharStatus = baseObject.GetCharStatus();
            baseObject.StatusChanged();
            baseObject.HideMode = true;
            baseObject.Transparent = true;
            return true;
        }

        /// <summary>
        /// 诱惑之光
        /// </summary>
        private static bool MagTamming(BaseObject baseObject, BaseObject targetBaseObject, int nTargetX, int nTargetY, int nMagicLevel)
        {
            var result = false;
            if (targetBaseObject.Race != ActorRace.Play && M2Share.RandomNumber.Random(4 - nMagicLevel) == 0)
            {
                targetBaseObject.TargetCret = null;
                if (targetBaseObject.Master == baseObject)
                {
                    targetBaseObject.OpenHolySeizeMode((nMagicLevel * 5 + 10) * 1000);
                    result = true;
                }
                else
                {
                    if (M2Share.RandomNumber.Random(2) == 0)
                    {
                        if (targetBaseObject.Abil.Level <= baseObject.Abil.Level + 2)
                        {
                            if (M2Share.RandomNumber.Random(3) == 0)
                            {
                                if (M2Share.RandomNumber.Random(baseObject.Abil.Level + 20 + nMagicLevel * 5) > targetBaseObject.Abil.Level + M2Share.Config.MagTammingTargetLevel)
                                {
                                    if (!targetBaseObject.NoTame && targetBaseObject.LifeAttrib != Grobal2.LA_UNDEAD && targetBaseObject.Abil.Level < M2Share.Config.MagTammingLevel && baseObject.SlaveList.Count < M2Share.Config.MagTammingCount)
                                    {
                                        int n14 = targetBaseObject.Abil.MaxHP / M2Share.Config.MagTammingHPRate;
                                        if (n14 <= 2)
                                        {
                                            n14 = 2;
                                        }
                                        else
                                        {
                                            n14 += n14;
                                        }
                                        if (targetBaseObject.Master != baseObject && M2Share.RandomNumber.Random(n14) == 0)
                                        {
                                            targetBaseObject.BreakCrazyMode();
                                            if (targetBaseObject.Master != null)
                                            {
                                                targetBaseObject.WAbil.HP = (ushort)(targetBaseObject.WAbil.HP / 10);
                                            }
                                            if (targetBaseObject.CanReAlive && targetBaseObject.Master == null)
                                            {
                                                targetBaseObject.CanReAlive = false;
                                                if (targetBaseObject.MonGen != null)
                                                {
                                                    if (targetBaseObject.MonGen.ActiveCount > 0)
                                                    {
                                                        targetBaseObject.MonGen.ActiveCount--;
                                                    }
                                                    else
                                                    {
                                                        targetBaseObject.MonGen = null;
                                                    }
                                                }
                                            }
                                            targetBaseObject.Master = baseObject;
                                            targetBaseObject.MasterRoyaltyTick = (M2Share.RandomNumber.Random(baseObject.Abil.Level * 2) + (nMagicLevel << 2) * 5 + 20) * 60 * 1000 + HUtil32.GetTickCount();
                                            targetBaseObject.SlaveMakeLevel = (byte)nMagicLevel;
                                            if (targetBaseObject.MasterTick == 0)
                                            {
                                                targetBaseObject.MasterTick = HUtil32.GetTickCount();
                                            }
                                            targetBaseObject.BreakHolySeizeMode();
                                            if (1500 - nMagicLevel * 200 < targetBaseObject.WalkSpeed)
                                            {
                                                targetBaseObject.WalkSpeed = 1500 - nMagicLevel * 200;
                                            }
                                            if (2000 - nMagicLevel * 200 < targetBaseObject.NextHitTime)
                                            {
                                                targetBaseObject.NextHitTime = 2000 - nMagicLevel * 200;
                                            }
                                            targetBaseObject.RefShowName();
                                            baseObject.SlaveList.Add(targetBaseObject);
                                        }
                                        else
                                        {
                                            if (M2Share.RandomNumber.Random(14) == 0)
                                            {
                                                targetBaseObject.WAbil.HP = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (targetBaseObject.LifeAttrib == Grobal2.LA_UNDEAD && M2Share.RandomNumber.Random(20) == 0)
                                        {
                                            targetBaseObject.WAbil.HP = 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (targetBaseObject.LifeAttrib != Grobal2.LA_UNDEAD && M2Share.RandomNumber.Random(20) == 0)
                                    {
                                        targetBaseObject.OpenCrazyMode(M2Share.RandomNumber.Random(20) + 10);
                                    }
                                }
                            }
                            else
                            {
                                if (targetBaseObject.LifeAttrib != Grobal2.LA_UNDEAD)
                                {
                                    targetBaseObject.OpenCrazyMode(M2Share.RandomNumber.Random(20) + 10);// 变红
                                }
                            }
                        }
                    }
                    else
                    {
                        targetBaseObject.OpenHolySeizeMode((nMagicLevel * 5 + 10) * 1000);
                    }
                    result = true;
                }
            }
            else
            {
                if (M2Share.RandomNumber.Random(2) == 0)
                {
                    result = true;
                }
            }
            return result;
        }

        private static bool MagTurnUndead(BaseObject baseObject, BaseObject targetBaseObject, int nTargetX, int nTargetY, int nLevel)
        {
            var result = false;
            if (targetBaseObject.SuperMan || targetBaseObject.LifeAttrib != Grobal2.LA_UNDEAD)
            {
                return result;
            }
            ((AnimalObject)targetBaseObject).Struck(baseObject);
            if (targetBaseObject.TargetCret == null)
            {
                ((AnimalObject)targetBaseObject).MBoRunAwayMode = true;
                ((AnimalObject)targetBaseObject).MDwRunAwayStart = HUtil32.GetTickCount();
                ((AnimalObject)targetBaseObject).MDwRunAwayTime = 10 * 1000;
            }
            baseObject.SetTargetCreat(targetBaseObject);
            if (M2Share.RandomNumber.Random(2) + (baseObject.Abil.Level - 1) > targetBaseObject.Abil.Level)
            {
                if (targetBaseObject.Abil.Level < M2Share.Config.MagTurnUndeadLevel)
                {
                    var n14 = baseObject.Abil.Level - targetBaseObject.Abil.Level;
                    if (M2Share.RandomNumber.Random(100) < (nLevel << 3) - nLevel + 15 + n14)
                    {
                        targetBaseObject.SetLastHiter(baseObject);
                        targetBaseObject.WAbil.HP = 0;
                        result = true;
                    }
                }
            }
            return result;
        }

        private static bool MagWindTebo(PlayObject playObject, UserMagic userMagic)
        {
            var result = false;
            var poseBaseObject = playObject.GetPoseCreate();
            if (poseBaseObject != null && poseBaseObject != playObject && !poseBaseObject.Death && !poseBaseObject.Ghost && playObject.IsProperTarget(poseBaseObject) && !poseBaseObject.StickMode)
            {
                if (Math.Abs(playObject.CurrX - poseBaseObject.CurrX) <= 1 && Math.Abs(playObject.CurrY - poseBaseObject.CurrY) <= 1 && playObject.Abil.Level > poseBaseObject.Abil.Level)
                {
                    if (M2Share.RandomNumber.Random(20) < userMagic.Level * 6 + 6 + (playObject.Abil.Level - poseBaseObject.Abil.Level))
                    {
                        poseBaseObject.CharPushed(M2Share.GetNextDirection(playObject.CurrX, playObject.CurrY, poseBaseObject.CurrX, poseBaseObject.CurrY), HUtil32._MAX(0, userMagic.Level - 1) + 1);
                        result = true;
                    }
                }
            }
            return result;
        }

        private static bool MagSaceMove(PlayObject playObject, int nLevel)
        {
            var result = false;
            if (M2Share.RandomNumber.Random(11) < nLevel * 2 + 4)
            {
                playObject.SendRefMsg(Grobal2.RM_SPACEMOVE_FIRE2, 0, 0, 0, 0, "");
                var envir = playObject.Envir;
                playObject.MapRandomMove(playObject.HomeMap, 1);
                if (envir != playObject.Envir && playObject.Race == ActorRace.Play)
                {
                    playObject.BoTimeRecall = false;
                }
                result = true;
            }
            return result;
        }

        private static bool MagGroupAmyounsul(PlayObject playObject, UserMagic userMagic, int nTargetX, int nTargetY, BaseObject targetBaseObject)
        {
            short nAmuletIdx = 0;
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            playObject.GetMapBaseObjects(playObject.Envir, nTargetX, nTargetY, HUtil32._MAX(1, userMagic.Level), baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var baseObject = baseObjectList[i];
                if (baseObject.Death || baseObject.Ghost || playObject == baseObject)
                {
                    continue;
                }
                if (playObject.IsProperTarget(baseObject))
                {
                    if (MagicBase.CheckAmulet(playObject, 1, 2, ref nAmuletIdx))
                    {
                        var stdItem = M2Share.WorldEngine.GetStdItem(playObject.UseItems[nAmuletIdx].Index);
                        if (stdItem != null)
                        {
                            MagicBase.UseAmulet(playObject, 1, 2, ref nAmuletIdx);
                            if (M2Share.RandomNumber.Random(baseObject.AntiPoison + 7) <= 6)
                            {
                                int nPower;
                                switch (stdItem.Shape)
                                {
                                    case 1:
                                        nPower = MagicBase.GetPower13(40, userMagic) + MagicBase.GetRPow(playObject.WAbil.SC) * 2;// 中毒类型 - 绿毒
                                        baseObject.SendDelayMsg(playObject, Grobal2.RM_POISON, PoisonState.DECHEALTH, nPower, playObject.ActorId, HUtil32.Round(userMagic.Level / 3 * (nPower / M2Share.Config.AmyOunsulPoint)), "", 1000);
                                        break;
                                    case 2:
                                        nPower = MagicBase.GetPower13(30, userMagic) + MagicBase.GetRPow(playObject.WAbil.SC) * 2;// 中毒类型 - 红毒
                                        baseObject.SendDelayMsg(playObject, Grobal2.RM_POISON, PoisonState.DAMAGEARMOR, nPower, playObject.ActorId, HUtil32.Round(userMagic.Level / 3 * (nPower / M2Share.Config.AmyOunsulPoint)), "", 1000);
                                        break;
                                }
                                if (baseObject.Race == ActorRace.Play || baseObject.Race >= ActorRace.Animal)
                                {
                                    result = true;
                                }
                            }
                        }
                        playObject.SetTargetCreat(baseObject);
                    }
                }
            }
            baseObjectList.Clear();
            return result;
        }

        private static bool MagGroupDeDing(PlayObject playObject, UserMagic userMagic, int nTargetX, int nTargetY, BaseObject targetBaseObject)
        {
            BaseObject baseObject;
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            playObject.GetMapBaseObjects(playObject.Envir, nTargetX, nTargetY, HUtil32._MAX(1, userMagic.Level), baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                baseObject = baseObjectList[i];
                if (baseObject.Death || baseObject.Ghost || playObject == baseObject)
                {
                    continue;
                }
                if (playObject.IsProperTarget(baseObject))
                {
                    var nPower = playObject.GetAttackPower(HUtil32.LoByte(playObject.WAbil.DC), HUtil32.HiByte(playObject.WAbil.DC) - HUtil32.LoByte(playObject.WAbil.DC));
                    if (M2Share.RandomNumber.Random(baseObject.SpeedPoint) >= playObject.HitPoint)
                    {
                        nPower = 0;
                    }
                    if (nPower > 0)
                    {
                        nPower = baseObject.GetHitStruckDamage(playObject, nPower);
                    }
                    if (nPower > 0)
                    {
                        baseObject.StruckDamage(nPower);
                        playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(baseObject.CurrX, baseObject.CurrY), 1, baseObject.ActorId, "", 200);
                    }
                    if (baseObject.Race >= ActorRace.Animal)
                    {
                        result = true;
                    }
                }
                playObject.SendRefMsg(Grobal2.RM_10205, 0, baseObject.CurrX, baseObject.CurrY, 1, "");
            }
            baseObjectList.Clear();
            return result;
        }

        private static bool MagGroupLightening(PlayObject playObject, UserMagic userMagic, short nTargetX, short nTargetY, BaseObject targetBaseObject, ref bool boSpellFire)
        {
            var result = false;
            boSpellFire = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            playObject.GetMapBaseObjects(playObject.Envir, nTargetX, nTargetY, HUtil32._MAX(1, userMagic.Level), baseObjectList);
            playObject.SendRefMsg(Grobal2.RM_MAGICFIRE, 0, HUtil32.MakeWord(userMagic.Magic.EffectType, userMagic.Magic.Effect), HUtil32.MakeLong(nTargetX, nTargetY), targetBaseObject.ActorId, "");
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var baseObject = baseObjectList[i];
                if (baseObject.Death || baseObject.Ghost || playObject == baseObject)
                {
                    continue;
                }
                if (playObject.IsProperTarget(baseObject))
                {
                    if (M2Share.RandomNumber.Random(10) >= baseObject.AntiMagic)
                    {
                        var nPower = playObject.GetAttackPower(MagicBase.GetPower(MagicBase.MPow(userMagic), userMagic) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
                        if (baseObject.LifeAttrib == Grobal2.LA_UNDEAD)
                        {
                            nPower = (ushort)HUtil32.Round(nPower * 1.5);
                        }
                        playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(baseObject.CurrX, baseObject.CurrY), 2, baseObject.ActorId, "", 600);
                        if (baseObject.Race >= ActorRace.Animal)
                        {
                            result = true;
                        }
                    }
                    if (baseObject.CurrX != nTargetX || baseObject.CurrY != nTargetY)
                    {
                        playObject.SendRefMsg(Grobal2.RM_10205, 0, baseObject.CurrX, baseObject.CurrY, 4, "");
                    }
                }
            }
            baseObjectList.Clear();
            return result;
        }

        private static bool MagHbFireBall(PlayObject playObject, UserMagic userMagic, short nTargetX, short nTargetY, ref BaseObject targetBaseObject)
        {
            var result = false;
            if (!playObject.MagCanHitTarget(playObject.CurrX, playObject.CurrY, targetBaseObject))
            {
                targetBaseObject = null;
                return false;
            }
            if (!playObject.IsProperTarget(targetBaseObject))
            {
                targetBaseObject = null;
                return false;
            }
            if (targetBaseObject.AntiMagic > M2Share.RandomNumber.Random(10) || Math.Abs(targetBaseObject.CurrX - nTargetX) > 1 || Math.Abs(targetBaseObject.CurrY - nTargetY) > 1)
            {
                targetBaseObject = null;
                return false;
            }
            var nPower = playObject.GetAttackPower(MagicBase.GetPower(MagicBase.MPow(userMagic), userMagic) + HUtil32.LoByte(playObject.WAbil.MC), HUtil32.HiByte(playObject.WAbil.MC) - HUtil32.LoByte(playObject.WAbil.MC) + 1);
            playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetBaseObject.ActorId, "", 600);
            if (targetBaseObject.Race >= ActorRace.Animal)
            {
                result = true;
            }
            if (playObject.Abil.Level > targetBaseObject.Abil.Level && !targetBaseObject.StickMode)
            {
                var levelgap = playObject.Abil.Level - targetBaseObject.Abil.Level;
                if (M2Share.RandomNumber.Random(20) < 6 + userMagic.Level * 3 + levelgap)
                {
                    var push = M2Share.RandomNumber.Random(userMagic.Level) - 1;
                    if (push > 0)
                    {
                        byte nDir = M2Share.GetNextDirection(playObject.CurrX, playObject.CurrY, targetBaseObject.CurrX, targetBaseObject.CurrY);
                        playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYPUSHED, nDir, HUtil32.MakeLong(nTargetX, nTargetY), push, targetBaseObject.ActorId, "", 600);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 火墙
        /// </summary>
        /// <returns></returns>
        private static bool MagMakeFireCross(PlayObject playObject, int nDamage, ushort time, short nX, short nY)
        {
            const string sDisableInSafeZoneFireCross = "安全区不允许使用...";
            if (M2Share.Config.DisableInSafeZoneFireCross && playObject.InSafeZone(playObject.Envir, nX, nY))
            {
                playObject.SysMsg(sDisableInSafeZoneFireCross, MsgColor.Red, MsgType.Notice);
                return false;
            }
            if (playObject.Envir.GetEvent(nX, nY - 1) == null)
            {
                FireBurnEvent fireBurnEvent = new FireBurnEvent(playObject, nX, (short)(nY - 1), Grobal2.ET_FIRE, time * 1000, nDamage);
                M2Share.EventMgr.AddEvent(fireBurnEvent);
            }
            if (playObject.Envir.GetEvent(nX - 1, nY) == null)
            {
                FireBurnEvent fireBurnEvent = new FireBurnEvent(playObject, (short)(nX - 1), nY, Grobal2.ET_FIRE, time * 1000, nDamage);
                M2Share.EventMgr.AddEvent(fireBurnEvent);
            }
            if (playObject.Envir.GetEvent(nX, nY) == null)
            {
                FireBurnEvent fireBurnEvent = new FireBurnEvent(playObject, nX, nY, Grobal2.ET_FIRE, time * 1000, nDamage);
                M2Share.EventMgr.AddEvent(fireBurnEvent);
            }
            if (playObject.Envir.GetEvent(nX + 1, nY) == null)
            {
                FireBurnEvent fireBurnEvent = new FireBurnEvent(playObject, (short)(nX + 1), nY, Grobal2.ET_FIRE, time * 1000, nDamage);
                M2Share.EventMgr.AddEvent(fireBurnEvent);
            }
            if (playObject.Envir.GetEvent(nX, nY + 1) == null)
            {
                FireBurnEvent fireBurnEvent = new FireBurnEvent(playObject, nX, (short)(nY + 1), Grobal2.ET_FIRE, time * 1000, nDamage);
                M2Share.EventMgr.AddEvent(fireBurnEvent);
            }
            return true;
        }

        private static bool MagBigExplosion(BaseObject baseObject, int nPower, int nX, int nY, int nRage)
        {
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            baseObject.GetMapBaseObjects(baseObject.Envir, nX, nY, nRage, baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var targetBaseObject = baseObjectList[i];
                if (baseObject.IsProperTarget(targetBaseObject))
                {
                    baseObject.SetTargetCreat(targetBaseObject);
                    targetBaseObject.SendMsg(baseObject, Grobal2.RM_MAGSTRUCK, 0, nPower, 0, 0, "");
                    result = true;
                }
            }
            baseObjectList.Clear();
            return result;
        }

        private static bool MagElecBlizzard(BaseObject baseObject, int nPower)
        {
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            baseObject.GetMapBaseObjects(baseObject.Envir, baseObject.CurrX, baseObject.CurrY, M2Share.Config.ElecBlizzardRange, baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var targetBaseObject = baseObjectList[i];
                int nPowerPoint;
                if (targetBaseObject.LifeAttrib != Grobal2.LA_UNDEAD)
                {
                    nPowerPoint = nPower / 10;
                }
                else
                {
                    nPowerPoint = nPower;
                }
                if (baseObject.IsProperTarget(targetBaseObject))
                {
                    targetBaseObject.SendMsg(baseObject, Grobal2.RM_MAGSTRUCK, 0, nPowerPoint, 0, 0, "");
                    result = true;
                }
            }
            baseObjectList.Clear();
            return result;
        }

        private static int MagMakeHolyCurtain(BaseObject baseObject, int nPower, short nX, short nY)
        {
            var result = 0;
            if (baseObject.Envir.CanWalk(nX, nY, true))
            {
                IList<BaseObject> baseObjectList = new List<BaseObject>();
                MagicEvent magicEvent = null;
                baseObject.GetMapBaseObjects(baseObject.Envir, nX, nY, 1, baseObjectList);
                
                for (var i = 0; i < baseObjectList.Count; i++)
                {
                    var targetBaseObject = baseObjectList[i];
                    if (targetBaseObject.Race >= ActorRace.Animal && M2Share.RandomNumber.Random(4) + (baseObject.Abil.Level - 1) > targetBaseObject.Abil.Level && targetBaseObject.Master == null)
                    {
                        targetBaseObject.OpenHolySeizeMode(nPower * 1000);
                        if (magicEvent == null)
                        {
                            magicEvent = new MagicEvent
                            {
                                BaseObjectList = new List<BaseObject>(),
                                dwStartTick = HUtil32.GetTickCount(),
                                dwTime = nPower * 1000
                            };
                        }
                        magicEvent.BaseObjectList.Add(targetBaseObject);
                        result++;
                    }
                    else
                    {
                        result = 0;
                    }
                }

                if (result > 0 && magicEvent != null)
                {
                    var holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX - 1), (short)(nY - 2), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[0] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX + 1), (short)(nY - 2), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[1] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX - 2), (short)(nY - 1), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[2] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX + 2), (short)(nY - 1), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[3] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX - 2), (short)(nY + 1), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[4] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX + 2), (short)(nY + 1), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[5] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX - 1), (short)(nY + 2), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[6] = holyCurtainEvent;
                    holyCurtainEvent = new HolyCurtainEvent(baseObject.Envir, (short)(nX + 1), (short)(nY + 2), Grobal2.ET_HOLYCURTAIN, nPower * 1000);
                    M2Share.EventMgr.AddEvent(holyCurtainEvent);
                    magicEvent.Events[7] = holyCurtainEvent;
                    M2Share.WorldEngine.MagicEventList.Add(magicEvent);
                }
                else
                {
                    if (magicEvent == null) return result;
                    magicEvent.BaseObjectList = null;
                }
            }
            return result;
        }

        private static bool MagMakeGroupTransparent(BaseObject baseObject, int nX, int nY, int nHTime)
        {
            var result = false;
            IList<BaseObject> baseObjectList = new List<BaseObject>();
            baseObject.GetMapBaseObjects(baseObject.Envir, nX, nY, 1, baseObjectList);
            for (var i = 0; i < baseObjectList.Count; i++)
            {
                var targetBaseObject = baseObjectList[i];
                if (baseObject.IsProperFriend(targetBaseObject))
                {
                    if (targetBaseObject.StatusArr[PoisonState.STATE_TRANSPARENT] == 0)
                    {
                        targetBaseObject.SendDelayMsg(targetBaseObject, Grobal2.RM_TRANSPARENT, 0, nHTime, 0, 0, "", 800);
                        result = true;
                    }
                }
            }
            baseObjectList.Clear();
            return result;
        }

        private static bool MabMabe(PlayObject playObject, BaseObject targetObject, int nPower, int nLevel, short nTargetX, short nTargetY)
        {
            var result = false;
            if (playObject.MagCanHitTarget(playObject.CurrX, playObject.CurrY, targetObject))
            {
                if (playObject.IsProperTarget(targetObject))
                {
                    if (targetObject.AntiMagic <= M2Share.RandomNumber.Random(10) && Math.Abs(targetObject.CurrX - nTargetX) <= 1 && Math.Abs(targetObject.CurrY - nTargetY) <= 1)
                    {
                        playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower / 3, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 600);
                        if (M2Share.RandomNumber.Random(2) + (playObject.Abil.Level - 1) > targetObject.Abil.Level)
                        {
                            var nLv = playObject.Abil.Level - targetObject.Abil.Level;
                            if (M2Share.RandomNumber.Random(M2Share.Config.MabMabeHitRandRate) < HUtil32._MAX(M2Share.Config.MabMabeHitMinLvLimit, nLevel * 8 - nLevel + 15 + nLv))
                            {
                                if (M2Share.RandomNumber.Random(M2Share.Config.MabMabeHitSucessRate) < nLevel * 2 + 4)
                                {
                                    if (targetObject.Race == ActorRace.Play)
                                    {
                                        playObject.SetPkFlag(targetObject);
                                        playObject.SetTargetCreat(targetObject);
                                    }
                                    targetObject.SetLastHiter(playObject);
                                    nPower = targetObject.GetMagStruckDamage(playObject, (ushort)nPower);
                                    playObject.SendDelayMsg(playObject, Grobal2.RM_DELAYMAGIC, nPower, HUtil32.MakeLong(nTargetX, nTargetY), 2, targetObject.ActorId, "", 600);
                                    if (targetObject.Race == ActorRace.Play && !(targetObject as PlayObject).UnParalysis)
                                    {
                                        targetObject.SendDelayMsg(playObject, Grobal2.RM_POISON, PoisonState.STONE, nPower / M2Share.Config.MabMabeHitMabeTimeRate + M2Share.RandomNumber.Random(nLevel), playObject.ActorId, nLevel, "", 650); // 中毒类型 - 麻痹
                                    }
                                    result = true;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static bool MagMakeSinSuSlave(PlayObject playObject, UserMagic userMagic)
        {
            var result = false;
            if (!playObject.CheckServerMakeSlave())
            {
                var sMonName = M2Share.Config.Dragon;
                int nExpLevel = userMagic.Level;
                var nCount = M2Share.Config.DragonCount;
                for (var i = 0; i < M2Share.Config.DragonArray.Length; i++)
                {
                    if (M2Share.Config.DragonArray[i].nHumLevel == 0)
                    {
                        break;
                    }
                    if (playObject.Abil.Level >= M2Share.Config.DragonArray[i].nHumLevel)
                    {
                        sMonName = M2Share.Config.DragonArray[i].sMonName;
                        nExpLevel = M2Share.Config.DragonArray[i].nLevel;
                        nCount = M2Share.Config.DragonArray[i].nCount;
                    }
                }
                if (playObject.MakeSlave(sMonName, userMagic.Level, nExpLevel, nCount, DwRoyaltySec) != null)
                {
                    result = true;
                }
                else
                {
                    playObject.RecallSlave(sMonName);
                }
            }
            return result;
        }

        private static bool MagMakeSlave(PlayObject playObject, UserMagic userMagic)
        {
            var result = false;
            if (!playObject.CheckServerMakeSlave())
            {
                var sMonName = M2Share.Config.Skeleton;
                int nExpLevel = userMagic.Level;
                var nCount = M2Share.Config.SkeletonCount;
                for (var i = 0; i < M2Share.Config.SkeletonArray.Length; i++)
                {
                    if (M2Share.Config.SkeletonArray[i].nHumLevel == 0)
                    {
                        break;
                    }
                    if (playObject.Abil.Level >= M2Share.Config.SkeletonArray[i].nHumLevel)
                    {
                        sMonName = M2Share.Config.SkeletonArray[i].sMonName;
                        nExpLevel = M2Share.Config.SkeletonArray[i].nLevel;
                        nCount = M2Share.Config.SkeletonArray[i].nCount;
                    }
                }
                if (playObject.MakeSlave(sMonName, userMagic.Level, nExpLevel, nCount, DwRoyaltySec) != null)
                {
                    result = true;
                }
            }
            return result;
        }

        private static bool MagMakeClone(PlayObject playObject, UserMagic userMagic)
        {
            new PlayCloneObject(playObject);
            return true;
        }

        private static bool MagMakeAngelSlave(PlayObject playObject, UserMagic userMagic)
        {
            var result = false;
            if (!playObject.CheckServerMakeSlave())
            {
                var sMonName = M2Share.Config.Angel;
                if (playObject.MakeSlave(sMonName, userMagic.Level, userMagic.Level, 1, DwRoyaltySec) != null)
                {
                    result = true;
                }
            }
            return result;
        }
    }
}