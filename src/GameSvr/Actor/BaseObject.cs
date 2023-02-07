﻿using GameSvr.Castle;
using GameSvr.Event;
using GameSvr.Items;
using GameSvr.Maps;
using GameSvr.Monster;
using GameSvr.Monster.Monsters;
using GameSvr.Player;
using SystemModule.Consts;
using SystemModule.Data;
using SystemModule.Enums;
using SystemModule.Packets.ClientPackets;

namespace GameSvr.Actor
{
    public partial class BaseObject : ActorEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ChrName;
        /// <summary>
        /// 所在座标X
        /// </summary>
        public short CurrX;
        /// <summary>
        /// 所在座标Y
        /// </summary>
        public short CurrY;
        /// <summary>
        /// 所在方向
        /// </summary>
        public byte Direction;
        /// <summary>
        /// 所在地图名称
        /// </summary>
        public string MapName;
        /// <summary>
        /// 地图文件名称
        /// </summary>
        public string MapFileName;
        /// <summary>
        /// 人物金币数
        /// </summary>
        public int Gold;
        /// <summary>
        /// 状态值
        /// </summary>
        public int CharStatus;
        protected int CharStatusEx;
        public MonGenInfo MonGen;
        /// <summary>
        /// 骑马
        /// </summary>
        public bool OnHorse;
        public ushort IncHealth;
        public ushort IncSpell;
        public ushort IncHealing;
        /// <summary>
        /// 怪物经验值
        /// </summary>
        public int FightExp = 0;
        /// <summary>
        /// 基本属性
        /// </summary>
        public Ability Abil;
        /// <summary>
        /// 角色属性
        /// </summary>
        public Ability WAbil;
        /// <summary>
        /// 附加属性
        /// </summary>
        internal AddAbility AddAbil;
        /// <summary>
        /// 视觉范围大小
        /// </summary>
        public byte ViewRange;
        /// <summary>
        /// 状态属性值结束时间
        /// 0-绿毒(减HP) 1-红毒(减MP) 2-防、魔防为0(唯我独尊3级) 3-不能跑动(中蛛网)
        /// 4-不能移动(中战连击) 5-麻痹(石化) 6-减血，被连击技能万剑归宗击中后掉血
        /// 7-冰冻(不能跑动，不能魔法) 8-隐身 9-防御力(神圣战甲术) 10-魔御力(幽灵盾) 11-魔法盾
        /// </summary>
        internal ushort[] StatusTimeArr;
        /// <summary>
        /// 状态持续的开始时间
        /// </summary>
        internal int[] StatusArrTick;
        /// <summary>
        /// 外观代码
        /// </summary>
        public ushort Appr;
        /// <summary>
        /// 种族
        /// </summary>
        public byte Race;
        /// <summary>
        /// 在地图上的类型
        /// </summary>
        public CellType MapCell;
        /// <summary>
        /// 角色外形
        /// </summary>
        public byte RaceImg;
        /// <summary>
        /// 人物攻击准确度
        /// </summary>
        public byte HitPoint;
        /// <summary>
        /// 额外攻击伤害(攻杀)
        /// </summary>
        internal ushort HitPlus;
        /// <summary>
        /// 双倍攻击伤害(烈火专用)
        /// </summary>
        internal ushort HitDouble;
        public ushort HealthRecover;
        public ushort SpellRecover;
        public byte AntiPoison;
        public ushort PoisonRecover;
        public ushort AntiMagic;
        /// <summary>
        /// 人物的幸运值
        /// </summary>
        public byte Luck;
        public byte PerHealth;
        public byte PerHealing;
        public byte PerSpell;
        /// <summary>
        /// 增加血量的间隔
        /// </summary>
        public int IncHealthSpellTick;
        /// <summary>
        /// 中绿毒降HP点数
        /// </summary>
        private byte GreenPoisoningPoint;
        /// <summary>
        /// 敏捷度
        /// </summary>
        public byte SpeedPoint;
        /// <summary>
        /// 攻击速度
        /// </summary>
        protected ushort HitSpeed;
        /// <summary>
        /// 不死系,1-为不死系
        /// </summary>
        public byte LifeAttrib;
        /// <summary>
        /// 否可以看到隐身人物(视线范围) 
        /// </summary>
        public byte CoolEyeCode = 0;
        /// <summary>
        /// 是否可以看到隐身人物
        /// </summary>
        public bool CoolEye;
        /// <summary>
        /// 是否被召唤(主人)
        /// </summary>
        public BaseObject Master;
        /// <summary>
        /// 是否属下(玩家召唤出来才为True)
        /// </summary>
        public bool IsSlave;
        /// <summary>
        /// 怪物叛变时间
        /// </summary>
        public int MasterRoyaltyTick;
        public int MasterTick;
        /// <summary>
        /// 杀怪计数
        /// </summary>
        public int KillMonCount;
        /// <summary>
        /// 宝宝等级(1-7)
        /// </summary>
        public byte SlaveExpLevel;
        /// <summary>
        /// 召唤等级
        /// </summary>
        public byte SlaveMakeLevel;
        /// <summary>
        /// 下属列表
        /// </summary>        
        internal IList<BaseObject> SlaveList;
        /// <summary>
        /// 宝宝攻击状态(休息/攻击)
        /// </summary>
        public bool SlaveRelax = false;
        /// <summary>
        /// 人物名字的颜色
        /// </summary>        
        public byte NameColor;
        /// <summary>
        /// 亮度
        /// </summary>
        public byte Light;
        /// <summary>
        /// 行会占争范围
        /// </summary>
        public bool GuildWarArea;
        /// <summary>
        /// 所属城堡
        /// </summary>
        public UserCastle Castle;
        /// <summary>
        /// 无敌模式
        /// </summary>
        public bool SuperMan;
        /// <summary>
        /// 不进入火墙
        /// </summary>
        public bool BoFearFire;
        /// <summary>
        /// 是否是动物
        /// </summary>
        public bool Animal;
        /// <summary>
        /// 死亡是否不掉物品
        /// </summary>
        public bool NoItem;
        /// <summary>
        /// 隐身模式
        /// </summary>
        public bool FixedHideMode;
        /// <summary>
        /// 不能冲撞模式(即敌人不能使用野蛮冲撞技能攻击)
        /// </summary>
        public bool StickMode;
        /// <summary>
        /// 被打到是否减慢行走速度,等级小于50的怪 F-减慢 T-不减慢
        /// </summary>
        public bool RushMode;
        /// <summary>
        /// 非攻击模式 F-可攻击 T-不攻击
        /// </summary>
        public bool NoAttackMode;
        public bool NoTame;
        /// <summary>
        /// 尸体
        /// </summary>
        public bool Skeleton;
        /// <summary>
        /// 肉的品质
        /// </summary>
        public ushort MeatQuality;
        /// <summary>
        /// 身体坚韧性
        /// </summary>
        public byte BodyLeathery;
        /// <summary>
        /// 不能走动模式(困魔咒)
        /// </summary>
        public bool HolySeize;
        /// <summary>
        /// 不能走动间隔(困魔咒)
        /// </summary>
        public int HolySeizeTick;
        /// <summary>
        /// 不能走动时长(困魔咒)
        /// </summary>
        public int HolySeizeInterval;
        /// <summary>
        /// 狂暴模式
        /// </summary>
        public bool CrazyMode;
        /// <summary>
        /// 狂暴间隔
        /// </summary>
        private int CrazyModeTick;
        /// <summary>
        /// 狂暴时常
        /// </summary>
        private int CrazyModeInterval;
        /// <summary>
        /// 心灵启示
        /// </summary>
        public bool ShowHp;
        /// <summary>
        /// 心灵启示检查时间
        /// </summary>
        public int ShowHpTick = 0;
        /// <summary>
        /// 心灵启示有效时长
        /// </summary>
        public int ShowHpInterval = 0;
        public Envirnoment Envir;
        /// <summary>
        /// 尸体清除
        /// </summary>
        public bool Ghost;
        /// <summary>
        /// 尸体清除间隔
        /// </summary>
        public int GhostTick;
        /// <summary>
        /// 死亡
        /// </summary>
        public bool Death;
        /// <summary>
        /// 死亡间隔
        /// </summary>
        public int DeathTick;
        public bool Invisible;
        /// <summary>
        /// 是否可以复活
        /// </summary>
        public bool CanReAlive;
        /// <summary>
        /// 复活时间
        /// </summary>
        public int ReAliveTick = 0;
        /// <summary>
        /// 怪物所拿的武器
        /// </summary>
        public byte MonsterWeapon = 0;
        /// <summary>
        /// 弯腰间隔
        /// </summary>
        public int StruckTick = 0;
        /// <summary>
        /// 刷新消息
        /// </summary>
        protected bool WantRefMsg;
        /// <summary>
        /// 增加到地图是否成功
        /// </summary>
        public bool AddtoMapSuccess;
        /// <summary>
        /// 换地图时，跑走不考虑坐标
        /// </summary>
        internal bool SpaceMoved;
        protected byte AttackSkillCount;
        protected byte AttackSkillPointCount;
        public bool Mission;
        public short MissionX = 0;
        public short MissionY = 0;
        /// <summary>
        /// 隐身戒指
        /// </summary>
        public bool HideMode;
        /// <summary>
        /// 石像化
        /// </summary>
        public bool StoneMode;
        /// <summary>
        /// 魔法隐身了
        /// </summary>
        public bool Transparent;
        /// <summary>
        /// 管理模式
        /// </summary>
        public bool AdminMode;
        /// <summary>
        /// 隐身模式（GM模式）
        /// </summary>
        public bool ObMode;
        /// <summary>
        /// 复活戒指使用间隔计数
        /// </summary>
        private int RevivalTick = 0;
        /// <summary>
        /// 死亡是不是掉装备
        /// </summary>
        public bool NoDropUseItem = false;
        /// <summary>
        /// 力量物品值
        /// </summary>
        protected byte PowerItem = 0;
        /// <summary>
        /// 视觉搜索时间间隔
        /// </summary>
        public int SearchTime;
        /// <summary>
        /// 视觉搜索间隔
        /// </summary>
        public int SearchTick;
        /// <summary>
        /// 上次运行时间
        /// </summary>
        public int RunTick;
        /// <summary>
        /// 运行时间
        /// </summary>
        public int RunTime;
        /// <summary>
        /// 特别指定为 此类型  加血间隔
        /// </summary>
        protected int HealthTick;
        protected int SpellTick;
        public BaseObject TargetCret;
        public int TargetFocusTick = 0;
        /// <summary>
        /// 人物被对方杀害时对方对象
        /// </summary>
        public BaseObject LastHiter;
        public int LastHiterTick;
        public BaseObject ExpHitter;
        protected int ExpHitterTick;
        protected int MapMoveTick;
        /// <summary>
        /// 中毒处理间隔时间
        /// </summary>
        protected int PoisoningTick;
        protected int VerifyTick;
        /// <summary>
        /// 怪物叛变时间间隔
        /// </summary>
        protected int CheckRoyaltyTick;
        /// <summary>
        /// 恢复血量和魔法间隔
        /// </summary>
        protected int AutoRecoveryTick;
        protected readonly PriorityQueue<SendMessage, int> MsgQueue;
        protected readonly IList<BaseObject> VisibleHumanList;
        protected readonly IList<VisibleMapItem> VisibleItems;
        protected readonly IList<EventInfo> VisibleEvents;
        /// <summary>
        /// 是否在可视范围内有人物,及宝宝
        /// </summary>
        public bool IsVisibleActive;
        /// <summary>
        /// 可见精灵列表
        /// </summary>
        public readonly IList<VisibleBaseObject> VisibleActors;
        /// <summary>
        /// 物品列表
        /// </summary>
        public IList<UserItem> ItemList;
        /// <summary>
        /// 身上物品
        /// </summary>
        public UserItem[] UseItems;
        public IList<MonsterSayMsg> SayMsgList;
        private int SendRefMsgTick;
        /// <summary>
        /// 攻击间隔
        /// </summary>
        protected int AttackTick = 0;
        /// <summary>
        /// 走路间隔
        /// </summary>
        protected int WalkTick;
        /// <summary>
        /// 走路速度
        /// </summary>
        public int WalkSpeed;
        /// <summary>
        /// 下次攻击时间
        /// </summary>
        public int NextHitTime;
        protected UserMagic[] MagicArr;
        /// <summary>
        /// 是否刷新在地图上信息
        /// </summary>
        protected bool DenyRefStatus;
        /// <summary>
        /// 是否增加地图计数
        /// </summary>
        public bool AddToMaped;
        /// <summary>
        /// 是否从地图中删除计数
        /// </summary>
        public bool DelFormMaped = false;
        public bool AutoChangeColor;
        protected int AutoChangeColorTick;
        protected byte AutoChangeIdx;
        /// <summary>
        /// 固定颜色
        /// </summary>
        public bool FixColor;
        public byte FixColorIdx;
        protected int FixStatus;
        /// <summary>
        /// 快速麻痹，受攻击后麻痹立即消失
        /// </summary>
        protected bool FastParalysis;
        public bool NastyMode;
        /// <summary>
        /// 是否机器人
        /// </summary>
        public bool IsRobot;

        protected BaseObject()
        {
            Ghost = false;
            GhostTick = 0;
            Death = false;
            DeathTick = 0;
            SendRefMsgTick = HUtil32.GetTickCount();
            Direction = 4;
            Race = ActorRace.Animal;
            RaceImg = 0;
            Gold = 0;
            Appr = 0;
            ViewRange = 5;
            Light = 0;
            NameColor = 255;
            HitPlus = 0;
            HitDouble = 0;
            BoFearFire = false;
            HitPoint = 5;
            SpeedPoint = 15;
            HitSpeed = 0;
            LifeAttrib = 0;
            AntiPoison = 0;
            PoisonRecover = 0;
            HealthRecover = 0;
            SpellRecover = 0;
            AntiMagic = 0;
            Luck = 0;
            IncSpell = 0;
            IncHealth = 0;
            IncHealing = 0;
            PerHealth = 5;
            PerHealing = 5;
            PerSpell = 5;
            IncHealthSpellTick = HUtil32.GetTickCount();
            GreenPoisoningPoint = 0;
            CharStatus = 0;
            CharStatusEx = 0;
            StatusTimeArr = new ushort[15];
            StatusArrTick = new int[15];
            GuildWarArea = false;
            SuperMan = false;
            Skeleton = false;
            RushMode = false;
            HolySeize = false;
            CrazyMode = false;
            ShowHp = false;
            Animal = false;
            NoItem = false;
            BodyLeathery = 50;
            FixedHideMode = false;
            StickMode = false;
            NoAttackMode = false;
            NoTame = false;
            AddAbil = new AddAbility();
            MsgQueue = new PriorityQueue<SendMessage, int>();
            VisibleHumanList = new List<BaseObject>();
            VisibleActors = new List<VisibleBaseObject>();
            VisibleItems = new List<VisibleMapItem>();
            VisibleEvents = new List<EventInfo>();
            ItemList = new List<UserItem>();
            IsVisibleActive = false;
            UseItems = new UserItem[13];
            Castle = null;
            Master = null;
            KillMonCount = 0;
            SlaveExpLevel = 0;
            SlaveList = new List<BaseObject>();
            Abil = new Ability();
            Abil = new Ability
            {
                Level = 1,
                AC = 0,
                MAC = 0,
                DC = (ushort)HUtil32.MakeLong(1, 4),
                MC = (ushort)HUtil32.MakeLong(1, 2),
                SC = (ushort)HUtil32.MakeLong(1, 2),
                HP = 15,
                MP = 15,
                MaxHP = 15,
                MaxMP = 15,
                Exp = 0,
                MaxExp = 50,
                Weight = 0,
                MaxWeight = 100
            };
            WantRefMsg = false;
            Mission = false;
            HideMode = false;
            StoneMode = false;
            CoolEye = false;
            Transparent = false;
            AdminMode = false;
            ObMode = false;
            RunTick = HUtil32.GetTickCount() + M2Share.RandomNumber.Random(1500);
            RunTime = 250;
            SearchTime = M2Share.RandomNumber.Random(2000) + 2000;
            SearchTick = HUtil32.GetTickCount();
            PoisoningTick = HUtil32.GetTickCount();
            VerifyTick = HUtil32.GetTickCount();
            CheckRoyaltyTick = HUtil32.GetTickCount();
            AutoRecoveryTick = HUtil32.GetTickCount();
            MapMoveTick = HUtil32.GetTickCount();
            MasterTick = 0;
            WalkSpeed = 1400;
            NextHitTime = 2000;
            HealthTick = 0;
            SpellTick = 0;
            TargetCret = null;
            LastHiter = null;
            ExpHitter = null;
            SayMsgList = null;
            DenyRefStatus = false;
            AddToMaped = true;
            AutoChangeColor = false;
            AutoChangeColorTick = HUtil32.GetTickCount();
            AutoChangeIdx = 0;
            FixColor = false;
            FixColorIdx = 0;
            FixStatus = -1;
            FastParalysis = false;
            NastyMode = false;
            MagicArr = new UserMagic[50];
            M2Share.ActorMgr.Add(this);
        }

        /// <summary>
        /// 获取物品掉落位置
        /// </summary>
        /// <returns></returns>
        private bool GetDropPosition(short nOrgX, short nOrgY, int nRange, ref short pX, ref short pY)
        {
            bool result = false;
            int nItemCount = 0;
            int n24 = 999;
            short n28 = 0;
            short n2C = 0;
            for (var i = 0; i < nRange; i++)
            {
                for (var ii = -i; ii <= i; ii++)
                {
                    for (var iii = -i; iii <= i; iii++)
                    {
                        pX = (short)(nOrgX + iii - 1);
                        pY = (short)(nOrgY + ii - 1);
                        if (Envir.GetItemEx(pX, pY, ref nItemCount) == 0)
                        {
                            if (Envir.Bo2C)
                            {
                                result = true;
                                break;
                            }
                        }
                        else
                        {
                            if (Envir.Bo2C && n24 > nItemCount)
                            {
                                n24 = nItemCount;
                                n28 = pX;
                                n2C = pY;
                            }
                        }
                    }
                    if (result)
                    {
                        break;
                    }
                }
                if (result)
                {
                    break;
                }
            }
            if (!result)
            {
                if (n24 < 8)
                {
                    pX = n28;
                    pY = n2C;
                }
                else
                {
                    pX = nOrgX;
                    pY = nOrgY;
                }
            }
            return result;
        }

        public bool DropItemDown(UserItem userItem, int nScatterRange, bool boDieDrop, int itemOfCreat, int dropCreat)
        {
            if (userItem == null)
            {
                return false;
            }
            bool result = false;
            short dx = 0;
            short dy = 0;
            StdItem stdItem = M2Share.WorldEngine.GetStdItem(userItem.Index);
            if (stdItem != null)
            {
                if (stdItem.StdMode == 40)
                {
                    ushort idura = userItem.Dura;
                    idura = (ushort)(idura - 2000);
                    if (idura <= 0)
                    {
                        idura = 0;
                    }
                    userItem.Dura = idura;
                }
                MapItem mapItem = new MapItem
                {
                    UserItem = new UserItem(userItem),
                    Name = CustomItem.GetItemName(userItem),// 取自定义物品名称
                    Looks = stdItem.Looks
                };
                if (stdItem.StdMode == 45)
                {
                    mapItem.Looks = (ushort)M2Share.GetRandomLook(mapItem.Looks, stdItem.Shape);
                }
                mapItem.AniCount = stdItem.AniCount;
                mapItem.Reserved = 0;
                mapItem.Count = 1;
                mapItem.OfBaseObject = itemOfCreat;
                mapItem.CanPickUpTick = HUtil32.GetTickCount();
                mapItem.DropBaseObject = dropCreat;
                GetDropPosition(CurrX, CurrY, nScatterRange, ref dx, ref dy);
                MapItem pr = (MapItem)Envir.AddToMap(dx, dy, CellType.Item, mapItem);
                if (pr == mapItem)
                {
                    SendRefMsg(Messages.RM_ITEMSHOW, mapItem.Looks, mapItem.ActorId, dx, dy, mapItem.Name);
                    int logcap;
                    if (boDieDrop)
                    {
                        logcap = 15;
                    }
                    else
                    {
                        logcap = 7;
                    }
                    if (!M2Share.IsCheapStuff(stdItem.StdMode))
                    {
                        if (stdItem.NeedIdentify == 1)
                        {
                            M2Share.EventSource.AddEventLog(logcap, MapName + "\t" + CurrX + "\t" + CurrY + "\t" + ChrName + "\t" + stdItem.Name + "\t" + userItem.MakeIndex + "\t" +
                                                                    HUtil32.BoolToIntStr(Race == ActorRace.Play) + "\t" + '0');
                        }
                    }
                    result = true;
                }
            }
            return result;
        }

        public void GoldChanged()
        {
            if (Race == ActorRace.Play)
            {
                SendUpdateMsg(this, Messages.RM_GOLDCHANGED, 0, 0, 0, 0, "");
            }
        }

        public void GameGoldChanged()
        {
            if (Race == ActorRace.Play)
            {
                SendUpdateMsg(this, Messages.RM_GAMEGOLDCHANGED, 0, 0, 0, 0, "");
            }
        }

        protected bool WalkTo(byte btDir, bool boFlag)
        {
            short n20 = 0;
            short n24 = 0;
            bool bo29;
            const string sExceptionMsg = "[Exception] TBaseObject::WalkTo";
            bool result = false;
            if (HolySeize)
            {
                return false;
            }
            try
            {
                var oldX = CurrX;
                var oldY = CurrY;
                Direction = btDir;
                short newX = 0;
                short newY = 0;
                switch (btDir)
                {
                    case Grobal2.DR_UP:
                        newX = CurrX;
                        newY = (short)(CurrY - 1);
                        break;
                    case Grobal2.DR_UPRIGHT:
                        newX = (short)(CurrX + 1);
                        newY = (short)(CurrY - 1);
                        break;
                    case Grobal2.DR_RIGHT:
                        newX = (short)(CurrX + 1);
                        newY = CurrY;
                        break;
                    case Grobal2.DR_DOWNRIGHT:
                        newX = (short)(CurrX + 1);
                        newY = (short)(CurrY + 1);
                        break;
                    case Grobal2.DR_DOWN:
                        newX = CurrX;
                        newY = (short)(CurrY + 1);
                        break;
                    case Grobal2.DR_DOWNLEFT:
                        newX = (short)(CurrX - 1);
                        newY = (short)(CurrY + 1);
                        break;
                    case Grobal2.DR_LEFT:
                        newX = (short)(CurrX - 1);
                        newY = CurrY;
                        break;
                    case Grobal2.DR_UPLEFT:
                        newX = (short)(CurrX - 1);
                        newY = (short)(CurrY - 1);
                        break;
                }
                if (newX >= 0 && Envir.Width - 1 >= newX && newY >= 0 && Envir.Height - 1 >= newY)
                {
                    bo29 = true;
                    if (BoFearFire && !Envir.CanSafeWalk(newX, newY))
                    {
                        bo29 = false;
                    }
                    if (Master != null)
                    {
                        Master.Envir.GetNextPosition(Master.CurrX, Master.CurrY, Master.Direction, 1, ref n20, ref n24);
                        if (newX == n20 && newY == n24)
                        {
                            bo29 = false;
                        }
                    }
                    if (bo29)
                    {
                        if (Envir.MoveToMovingObject(CurrX, CurrY, this, newX, newY, boFlag) > 0)
                        {
                            CurrX = newX;
                            CurrY = newY;
                        }
                    }
                }
                if (CurrX != oldX || CurrY != oldY)
                {
                    if (Walk(Messages.RM_WALK))
                    {
                        if (Transparent && HideMode)
                        {
                            StatusTimeArr[PoisonState.STATETRANSPARENT] = 1;
                        }
                        result = true;
                    }
                    else
                    {
                        Envir.DeleteFromMap(CurrX, CurrY, MapCell, this);
                        CurrX = oldX;
                        CurrY = oldY;
                        Envir.AddToMap(CurrX, CurrY, MapCell, this);
                    }
                }
            }
            catch (Exception ex)
            {
                M2Share.Logger.Error(sExceptionMsg);
                M2Share.Logger.Error(ex.StackTrace);
            }
            return result;
        }

        protected void HealthSpellChanged()
        {
            if (Race == ActorRace.Play)
            {
                SendUpdateMsg(this, Messages.RM_HEALTHSPELLCHANGED, 0, 0, 0, 0, "");
            }

            if (ShowHp)
            {
                SendRefMsg(Messages.RM_HEALTHSPELLCHANGED, 0, 0, 0, 0, "");
            }
        }

        internal int CalcGetExp(int nLevel, int nExp)
        {
            int result;
            if (M2Share.Config.HighLevelKillMonFixExp || (Abil.Level < (nLevel + 10)))
            {
                result = nExp;
            }
            else
            {
                result = nExp - HUtil32.Round(nExp / 15 * (Abil.Level - (nLevel + 10)));
            }
            if (result <= 0)
            {
                result = 1;
            }
            return result;
        }

        public void RefNameColor()
        {
            SendRefMsg(Messages.RM_CHANGENAMECOLOR, 0, 0, 0, 0, "");
        }

        private int GainSlaveUpKillCount()
        {
            int tCount;
            if (SlaveExpLevel < Grobal2.SlaveMaxLevel - 2)
            {
                tCount = M2Share.Config.MonUpLvNeedKillCount[SlaveExpLevel];
            }
            else
            {
                tCount = 0;
            }
            return (Abil.Level * M2Share.Config.MonUpLvRate) - Abil.Level + M2Share.Config.MonUpLvNeedKillBase + tCount;
        }

        private void GainSlaveExp(byte nLevel)
        {
            KillMonCount += nLevel;
            if (GainSlaveUpKillCount() < KillMonCount)
            {
                KillMonCount -= GainSlaveUpKillCount();
                if (SlaveExpLevel < (SlaveMakeLevel * 2 + 1))
                {
                    SlaveExpLevel++;
                    RecalcAbilitys();
                    RefNameColor();
                }
            }
        }

        protected bool DropGoldDown(int nGold, bool boFalg, int goldOfCreat, int dropGoldCreat)
        {
            bool result = false;
            short nX = 0;
            short nY = 0;
            int s20;
            MapItem mapItem = new MapItem
            {
                Name = Grobal2.StringGoldName,
                Count = nGold,
                Looks = M2Share.GetGoldShape(nGold),
                OfBaseObject = goldOfCreat,
                CanPickUpTick = HUtil32.GetTickCount(),
                DropBaseObject = dropGoldCreat
            };
            GetDropPosition(CurrX, CurrY, 3, ref nX, ref nY);
            MapItem mapItemA = (MapItem)Envir.AddToMap(nX, nY, CellType.Item, mapItem);
            if (mapItemA != null)
            {
                if (mapItemA.ActorId != mapItem.ActorId)
                {
                    mapItem = mapItemA;
                }
                SendRefMsg(Messages.RM_ITEMSHOW, mapItem.Looks, mapItem.ActorId, nX, nY, mapItem.Name);
                if (Race == ActorRace.Play)
                {
                    if (boFalg)
                    {
                        s20 = 15;
                    }
                    else
                    {
                        s20 = 7;
                    }
                    if (M2Share.GameLogGold)
                    {
                        M2Share.EventSource.AddEventLog(s20, MapName + "\t" + CurrX + "\t" + CurrY + "\t" + ChrName + "\t" + Grobal2.StringGoldName + "\t" + nGold + "\t" + HUtil32.BoolToIntStr(Race == ActorRace.Play) + "\t" + '0');
                    }
                }
                result = true;
            }
            return result;
        }

        internal int GetGuildRelation(PlayObject play, PlayObject target)
        {
            GuildWarArea = false;
            if ((play.MyGuild == null) || (target.MyGuild == null))
            {
                return 0;
            }
            if (play.InSafeArea() || target.InSafeArea())
            {
                return 0;
            }
            if (play.MyGuild.GuildWarList.Count <= 0)
            {
                return 0;
            }
            GuildWarArea = true;
            int result = 0;
            if (play.MyGuild.IsWarGuild(target.MyGuild) && target.MyGuild.IsWarGuild(play.MyGuild))
            {
                result = 2;
            }
            if (play.MyGuild == target.MyGuild)
            {
                result = 1;
            }
            if (play.MyGuild.IsAllyGuild(target.MyGuild) && target.MyGuild.IsAllyGuild(play.MyGuild))
            {
                result = 3;
            }
            return result;
        }

        internal void MakeWeaponUnlock()
        {
            if (UseItems[Grobal2.U_WEAPON] == null)
            {
                return;
            }
            if (UseItems[Grobal2.U_WEAPON].Index <= 0)
            {
                return;
            }
            if (UseItems[Grobal2.U_WEAPON].Desc[3] > 0)
            {
                UseItems[Grobal2.U_WEAPON].Desc[3] -= 1;
                SysMsg(Settings.TheWeaponIsCursed, MsgColor.Red, MsgType.Hint);
            }
            else
            {
                if (UseItems[Grobal2.U_WEAPON].Desc[4] < 10)
                {
                    UseItems[Grobal2.U_WEAPON].Desc[4]++;
                    SysMsg(Settings.TheWeaponIsCursed, MsgColor.Red, MsgType.Hint);
                }
            }
            if (Race == ActorRace.Play)
            {
                RecalcAbilitys();
                SendMsg(this, Messages.RM_ABILITY, 0, 0, 0, 0, "");
                SendMsg(this, Messages.RM_SUBABILITY, 0, 0, 0, 0, "");
            }
        }

        public ushort GetAttackPower(int nBasePower, int nPower)
        {
            int result;
            if (nPower < 0)
            {
                nPower = 0;
            }
            if (Luck > 0)
            {
                if (M2Share.RandomNumber.Random(10 - HUtil32._MIN(9, Luck)) == 0)
                {
                    result = nBasePower + nPower;
                }
                else
                {
                    result = nBasePower + M2Share.RandomNumber.Random(nPower + 1);
                }
            }
            else
            {
                result = nBasePower + M2Share.RandomNumber.Random(nPower + 1);
                if (Luck <= 0)
                {
                    if (M2Share.RandomNumber.Random(10 - HUtil32._MAX(0, -Luck)) == 0)
                    {
                        result = nBasePower;
                    }
                }
            }
            if (Race == ActorRace.Play)
            {
                PlayObject playObject = (PlayObject)this;
                result = HUtil32.Round(result * (playObject.PowerRate / 100));
                if (playObject.BoPowerItem)
                {
                    result = HUtil32.Round(PowerItem * result);
                }
            }
            if (AutoChangeColor)
            {
                result = result * AutoChangeIdx + 1;
            }
            if (FixColor)
            {
                result = result * FixColorIdx + 1;
            }
            return (ushort)result;
        }

        /// <summary>
        /// 减少生命值
        /// </summary>
        /// <param name="nDamage"></param>
        internal void DamageHealth(ushort nDamage)
        {
            if ((LastHiter == null) || ((LastHiter.Race == ActorRace.Play) && !((PlayObject)LastHiter).UnMagicShield))
            {
                if (Race == ActorRace.Play && ((PlayObject)this).MagicShield && (nDamage > 0) && (WAbil.MP > 0))
                {
                    int nSpdam = HUtil32.Round(nDamage * 1.5);
                    if (WAbil.MP >= nSpdam)
                    {
                        WAbil.MP = (ushort)(WAbil.MP - nSpdam);
                        nSpdam = 0;
                    }
                    else
                    {
                        nSpdam = nSpdam - WAbil.MP;
                        WAbil.MP = 0;
                    }
                    nDamage = (ushort)HUtil32.Round(nSpdam / 1.5);
                    HealthSpellChanged();
                }
            }
            if (nDamage > 0)
            {
                if ((WAbil.HP - nDamage) > 0)
                {
                    WAbil.HP = (ushort)(WAbil.HP - nDamage);
                }
                else
                {
                    WAbil.HP = 0;
                }
            }
            else
            {
                if ((WAbil.HP - nDamage) < WAbil.MaxHP)
                {
                    WAbil.HP = (ushort)(WAbil.HP - nDamage);
                }
                else
                {
                    WAbil.HP = WAbil.MaxHP;
                }
            }
        }

        public static byte GetBackDir(int nDir)
        {
            byte result = 0;
            switch (nDir)
            {
                case Grobal2.DR_UP:
                    result = Grobal2.DR_DOWN;
                    break;
                case Grobal2.DR_DOWN:
                    result = Grobal2.DR_UP;
                    break;
                case Grobal2.DR_LEFT:
                    result = Grobal2.DR_RIGHT;
                    break;
                case Grobal2.DR_RIGHT:
                    result = Grobal2.DR_LEFT;
                    break;
                case Grobal2.DR_UPLEFT:
                    result = Grobal2.DR_DOWNRIGHT;
                    break;
                case Grobal2.DR_UPRIGHT:
                    result = Grobal2.DR_DOWNLEFT;
                    break;
                case Grobal2.DR_DOWNLEFT:
                    result = Grobal2.DR_UPRIGHT;
                    break;
                case Grobal2.DR_DOWNRIGHT:
                    result = Grobal2.DR_UPLEFT;
                    break;
            }

            return result;
        }

        public int CharPushed(byte nDir, int nPushCount)
        {
            short nx = 0;
            short ny = 0;
            int result = 0;
            byte olddir = Direction;
            Direction = nDir;
            byte nBackDir = GetBackDir(nDir);
            for (int i = 0; i < nPushCount; i++)
            {
                GetFrontPosition(ref nx, ref ny);
                if (Envir.CanWalk(nx, ny, false))
                {
                    if (Envir.MoveToMovingObject(CurrX, CurrY, this, nx, ny, false) > 0)
                    {
                        CurrX = nx;
                        CurrY = ny;
                        SendRefMsg(Messages.RM_PUSH, nBackDir, CurrX, CurrY, 0, "");
                        result++;
                        if (Race >= ActorRace.Animal)
                        {
                            WalkTick = WalkTick + 800;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            Direction = nBackDir;
            if (result == 0)
            {
                Direction = olddir;
            }
            return result;
        }

        public int MagPassThroughMagic(short sx, short sy, short tx, short ty, byte nDir, int magPwr, bool undeadAttack)
        {
            int tcount = 0;
            for (int i = 0; i < 12; i++)
            {
                BaseObject baseObject = (BaseObject)Envir.GetMovingObject(sx, sy, true);
                if (baseObject != null)
                {
                    if (IsProperTarget(baseObject))
                    {
                        if (M2Share.RandomNumber.Random(10) >= baseObject.AntiMagic)
                        {
                            if (undeadAttack)
                            {
                                magPwr = HUtil32.Round(magPwr * 1.5);
                            }
                            baseObject.SendDelayMsg(this, Messages.RM_MAGSTRUCK, 0, magPwr, 0, 0, "", 600);
                            tcount++;
                        }
                    }
                }
                if (!((Math.Abs(sx - tx) <= 0) && (Math.Abs(sy - ty) <= 0)))
                {
                    nDir = M2Share.GetNextDirection(sx, sy, tx, ty);
                    if (!Envir.GetNextPosition(sx, sy, nDir, 1, ref sx, ref sy))
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return tcount;
        }

        private void BreakOpenHealth()
        {
            if (ShowHp)
            {
                ShowHp = false;
                CharStatusEx = CharStatusEx ^ PoisonState.OPENHEATH;
                CharStatus = GetCharStatus();
                SendRefMsg(Messages.RM_CLOSEHEALTH, 0, 0, 0, 0, "");
            }
        }

        private void MakeOpenHealth()
        {
            ShowHp = true;
            CharStatusEx = CharStatusEx | PoisonState.OPENHEATH;
            CharStatus = GetCharStatus();
            SendRefMsg(Messages.RM_OPENHEALTH, 0, WAbil.HP, WAbil.MaxHP, 0, "");
        }

        protected void IncHealthSpell(int nHp, int nMp)
        {
            if ((nHp < 0) || (nMp < 0))
            {
                return;
            }
            if ((WAbil.HP + nHp) >= WAbil.MaxHP)
            {
                WAbil.HP = WAbil.MaxHP;
            }
            else
            {
                WAbil.HP += (ushort)nHp;
            }
            if ((WAbil.MP + nMp) >= WAbil.MaxMP)
            {
                WAbil.MP = WAbil.MaxMP;
            }
            else
            {
                WAbil.MP += (ushort)nMp;
            }
            HealthSpellChanged();
        }

        private void ItemDamageRevivalRing()
        {
            for (int i = 0; i < UseItems.Length; i++)
            {
                if (UseItems[i] != null && UseItems[i].Index > 0)
                {
                    StdItem pSItem = M2Share.WorldEngine.GetStdItem(UseItems[i].Index);
                    if (pSItem != null)
                    {
                        if (M2Share.ItemDamageRevivalMap.Contains(pSItem.Shape) || (((i == Grobal2.U_WEAPON) || (i == Grobal2.U_RIGHTHAND)) && M2Share.ItemDamageRevivalMap.Contains(pSItem.AniCount)))
                        {
                            ushort nDura = UseItems[i].Dura;
                            ushort tDura = (ushort)HUtil32.Round(nDura / 1000.0);
                            nDura -= 1000;
                            if (nDura <= 0)
                            {
                                nDura = 0;
                                UseItems[i].Dura = nDura;
                                if (Race == ActorRace.Play)
                                {
                                    ((PlayObject)this).SendDelItems(UseItems[i]);
                                }
                                UseItems[i].Index = 0;
                                RecalcAbilitys();
                            }
                            else
                            {
                                UseItems[i].Dura = nDura;
                            }
                            if (tDura != HUtil32.Round(nDura / 1000.0)) // 1.03
                            {
                                SendMsg(this, Messages.RM_DURACHANGE, i, nDura, UseItems[i].DuraMax, 0, "");
                            }
                        }
                    }
                }
            }
        }

        public bool GetFrontPosition(ref short nX, ref short nY)
        {
            Envirnoment envir = Envir;
            nX = CurrX;
            nY = CurrY;
            switch (Direction)
            {
                case Grobal2.DR_UP:
                    if (nY > 0)
                    {
                        nY -= 1;
                    }
                    break;
                case Grobal2.DR_UPRIGHT:
                    if ((nX < (envir.Width - 1)) && (nY > 0))
                    {
                        nX++;
                        nY -= 1;
                    }
                    break;
                case Grobal2.DR_RIGHT:
                    if (nX < (envir.Width - 1))
                    {
                        nX++;
                    }
                    break;
                case Grobal2.DR_DOWNRIGHT:
                    if ((nX < (envir.Width - 1)) && (nY < (envir.Height - 1)))
                    {
                        nX++;
                        nY++;
                    }
                    break;
                case Grobal2.DR_DOWN:
                    if (nY < (envir.Height - 1))
                    {
                        nY++;
                    }
                    break;
                case Grobal2.DR_DOWNLEFT:
                    if ((nX > 0) && (nY < (envir.Height - 1)))
                    {
                        nX -= 1;
                        nY++;
                    }
                    break;
                case Grobal2.DR_LEFT:
                    if (nX > 0)
                    {
                        nX -= 1;
                    }
                    break;
                case Grobal2.DR_UPLEFT:
                    if ((nX > 0) && (nY > 0))
                    {
                        nX -= 1;
                        nY -= 1;
                    }
                    break;
            }
            return true;
        }

        private static bool SpaceMoveGetRandXY(Envirnoment envir, ref short nX, ref short nY)
        {
            int n14;
            short n18;
            int n1C;
            bool result = false;
            if (envir.Width < 80)
            {
                n18 = 3;
            }
            else
            {
                n18 = 10;
            }
            if (envir.Height < 150)
            {
                if (envir.Height < 50)
                {
                    n1C = 2;
                }
                else
                {
                    n1C = 15;
                }
            }
            else
            {
                n1C = 50;
            }
            n14 = 0;
            while (true)
            {
                if (envir.CanWalk(nX, nY, true))
                {
                    result = true;
                    break;
                }

                if (nX < (envir.Width - n1C - 1))
                {
                    nX += n18;
                }
                else
                {
                    nX = (short)M2Share.RandomNumber.Random(envir.Width);
                    if (nY < (envir.Height - n1C - 1))
                    {
                        nY += n18;
                    }
                    else
                    {
                        nY = (short)M2Share.RandomNumber.Random(envir.Height);
                    }
                }
                n14++;
                if (n14 >= 201)
                {
                    break;
                }
            }
            return result;
        }

        public void SpaceMove(string sMap, short nX, short nY, int nInt)
        {
            Envirnoment envir = M2Share.MapMgr.FindMap(sMap);
            if (envir != null)
            {
                if (M2Share.ServerIndex == envir.ServerIndex)
                {
                    Envirnoment oldEnvir = Envir;
                    int nOldX = CurrX;
                    int nOldY = CurrY;
                    bool moveSuccess = false;
                    Envir.DeleteFromMap(CurrX, CurrY, MapCell, this);
                    VisibleHumanList.Clear();
                    for (int i = 0; i < VisibleItems.Count; i++)
                    {
                        VisibleItems[i] = null;
                    }
                    VisibleItems.Clear();
                    for (int i = 0; i < VisibleActors.Count; i++)
                    {
                        VisibleActors[i] = null;
                    }
                    VisibleActors.Clear();
                    VisibleEvents.Clear();
                    Envir = envir;
                    MapName = envir.MapName;
                    MapFileName = envir.MapFileName;
                    CurrX = nX;
                    CurrY = nY;
                    if (SpaceMoveGetRandXY(Envir, ref CurrX, ref CurrY))
                    {
                        Envir.AddToMap(CurrX, CurrY, MapCell, this);
                        SendMsg(this, Messages.RM_CLEAROBJECTS, 0, 0, 0, 0, "");
                        SendMsg(this, Messages.RM_CHANGEMAP, 0, 0, 0, 0, MapFileName);
                        if (nInt == 1)
                        {
                            SendRefMsg(Messages.RM_SPACEMOVE_SHOW2, Direction, CurrX, CurrY, 0, "");
                        }
                        else
                        {
                            SendRefMsg(Messages.RM_SPACEMOVE_SHOW, Direction, CurrX, CurrY, 0, "");
                        }
                        MapMoveTick = HUtil32.GetTickCount();
                        SpaceMoved = true;
                        moveSuccess = true;
                    }
                    if (!moveSuccess)
                    {
                        Envir = oldEnvir;
                        CurrX = (short)nOldX;
                        CurrY = (short)nOldY;
                        Envir.AddToMap(CurrX, CurrY, MapCell, this);
                    }
                    OnEnvirnomentChanged();
                }
                else
                {
                    if (SpaceMoveGetRandXY(envir, ref nX, ref nY))
                    {
                        if (Race == ActorRace.Play)
                        {
                            DisappearA();
                            SpaceMoved = true;
                            ((PlayObject)this).ChangeSpaceMove(envir, nX, nY);
                        }
                        else
                        {
                            KickException();
                        }
                    }
                }
            }
        }

        public void RefShowName()
        {
            SendRefMsg(Messages.RM_USERNAME, 0, 0, 0, 0, GetShowName());
        }

        public BaseObject MakeSlave(string sMonName, int nMakeLevel, int nExpLevel, int nMaxMob, int dwRoyaltySec)
        {
            if (SlaveList.Count < nMaxMob)
            {
                short nX = 0;
                short nY = 0;
                GetFrontPosition(ref nX, ref nY);
                BaseObject monObj = M2Share.WorldEngine.RegenMonsterByName(Envir.MapName, nX, nY, sMonName);
                if (monObj != null)
                {
                    monObj.Master = this;
                    monObj.IsSlave = true;
                    monObj.MasterRoyaltyTick = HUtil32.GetTickCount() + (dwRoyaltySec * 1000);
                    monObj.SlaveMakeLevel = (byte)nMakeLevel;
                    monObj.SlaveExpLevel = (byte)nExpLevel;
                    monObj.RecalcAbilitys();
                    if (monObj.WAbil.HP < monObj.WAbil.MaxHP)
                    {
                        monObj.WAbil.HP = (ushort)(monObj.WAbil.HP + (monObj.WAbil.MaxHP - monObj.WAbil.HP) / 2);
                    }
                    monObj.RefNameColor();
                    SlaveList.Add(monObj);
                    return monObj;
                }
            }
            return null;
        }

        /// <summary>
        /// 地图随机移动
        /// </summary>
        public void MapRandomMove(string sMapName, int nInt)
        {
            int nEgdey;
            Envirnoment envir = M2Share.MapMgr.FindMap(sMapName);
            if (envir != null)
            {
                if (envir.Height < 150)
                {
                    if (envir.Height < 30)
                    {
                        nEgdey = 2;
                    }
                    else
                    {
                        nEgdey = 20;
                    }
                }
                else
                {
                    nEgdey = 50;
                }

                short nX = (short)(M2Share.RandomNumber.Random(envir.Width - nEgdey - 1) + nEgdey);
                short nY = (short)(M2Share.RandomNumber.Random(envir.Height - nEgdey - 1) + nEgdey);
                SpaceMove(sMapName, nX, nY, nInt);
            }
        }

        public bool AddItemToBag(UserItem userItem)
        {
            if (ItemList.Count >= Grobal2.MaxBagItem)
                return false;
            ItemList.Add(userItem);
            WeightChanged();
            return true;
        }

        public BaseObject GetPoseCreate()
        {
            short nX = 0;
            short nY = 0;
            if (GetFrontPosition(ref nX, ref nY))
            {
                return (BaseObject)Envir.GetMovingObject(nX, nY, true);
            }
            return null;
        }

        protected bool GetAttackDir(BaseObject baseObject, ref byte btDir)
        {
            bool result = false;
            if ((CurrX - 1 <= baseObject.CurrX) && (CurrX + 1 >= baseObject.CurrX) &&
                (CurrY - 1 <= baseObject.CurrY) && (CurrY + 1 >= baseObject.CurrY) &&
                ((CurrX != baseObject.CurrX) || (CurrY != baseObject.CurrY)))
            {
                result = true;
                if (((CurrX - 1) == baseObject.CurrX) && (CurrY == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_LEFT;
                    return true;
                }
                if (((CurrX + 1) == baseObject.CurrX) && (CurrY == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_RIGHT;
                    return true;
                }
                if ((CurrX == baseObject.CurrX) && ((CurrY - 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_UP;
                    return true;
                }
                if ((CurrX == baseObject.CurrX) && ((CurrY + 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_DOWN;
                    return true;
                }
                if (((CurrX - 1) == baseObject.CurrX) && ((CurrY - 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_UPLEFT;
                    return true;
                }
                if (((CurrX + 1) == baseObject.CurrX) && ((CurrY - 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_UPRIGHT;
                    return true;
                }
                if (((CurrX - 1) == baseObject.CurrX) && ((CurrY + 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_DOWNLEFT;
                    return true;
                }
                if (((CurrX + 1) == baseObject.CurrX) && ((CurrY + 1) == baseObject.CurrY))
                {
                    btDir = Grobal2.DR_DOWNRIGHT;
                    return true;
                }
                btDir = 0;
            }
            return result;
        }

        protected bool GetAttackDir(BaseObject baseObject, int nRange, ref byte btDir)
        {
            short nX = 0;
            short nY = 0;
            btDir = M2Share.GetNextDirection(CurrX, CurrY, baseObject.CurrX, baseObject.CurrY);
            if (Envir.GetNextPosition(CurrX, CurrY, btDir, nRange, ref nX, ref nY))
            {
                return baseObject == (BaseObject)Envir.GetMovingObject(nX, nY, true);
            }
            return false;
        }

        protected bool TargetInSpitRange(BaseObject baseObject, ref byte btDir)
        {
            bool result = false;
            if ((Math.Abs(baseObject.CurrX - CurrX) <= 2) && (Math.Abs(baseObject.CurrY - CurrY) <= 2))
            {
                int nX = baseObject.CurrX - CurrX;
                int nY = baseObject.CurrY - CurrY;
                if ((Math.Abs(nX) <= 1) && (Math.Abs(nY) <= 1))
                {
                    GetAttackDir(baseObject, ref btDir);
                    return true;
                }
                nX += 2;
                nY += 2;
                if ((nX >= 0) && (nX <= 4) && (nY >= 0) && (nY <= 4))
                {
                    btDir = M2Share.GetNextDirection(CurrX, CurrY, baseObject.CurrX, baseObject.CurrY);
                    if (M2Share.Config.SpitMap[btDir, nY, nX] == 1)
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 计算包裹物品总重量
        /// </summary>
        protected ushort RecalcBagWeight()
        {
            ushort result = 0;
            for (int i = 0; i < ItemList.Count; i++)
            {
                StdItem stdItem = M2Share.WorldEngine.GetStdItem(ItemList[i].Index);
                if (stdItem != null)
                {
                    result += stdItem.Weight;
                }
            }
            return result;
        }

        private bool AddToMap()
        {
            object point = Envir.AddToMap(CurrX, CurrY, MapCell, this);
            bool result = point != null;
            if (!FixedHideMode)
            {
                SendRefMsg(Messages.RM_TURN, Direction, CurrX, CurrY, 0, "");
            }
            return result;
        }

        /// <summary>
        /// 计算施法魔法值
        /// </summary>
        internal static ushort GetMagicSpell(UserMagic userMagic)
        {
            return (ushort)HUtil32.Round(userMagic.Magic.Spell / (userMagic.Magic.TrainLv + 1) * (userMagic.Level + 1));
        }

        /// <summary>
        /// 减少魔法值
        /// </summary>
        protected void DamageSpell(ushort nSpellPoint)
        {
            if (nSpellPoint > 0)
            {
                if ((WAbil.MP - nSpellPoint) > 0)
                {
                    WAbil.MP -= nSpellPoint;
                }
                else
                {
                    WAbil.MP = 0;
                }
            }
            else
            {
                if ((WAbil.MP - nSpellPoint) < WAbil.MaxMP)
                {
                    WAbil.MP -= nSpellPoint;
                }
                else
                {
                    WAbil.MP = WAbil.MaxMP;
                }
            }
        }

        /// <summary>
        /// 减少武器持久值
        /// </summary>
        protected void DoDamageWeapon(ushort nWeaponDamage)
        {
            if (UseItems[Grobal2.U_WEAPON] == null || UseItems[Grobal2.U_WEAPON].Index <= 0)
            {
                return;
            }
            ushort nDura = UseItems[Grobal2.U_WEAPON].Dura;
            int nDuraPoint = HUtil32.Round(nDura / 1.03);
            nDura -= nWeaponDamage;
            if (nDura <= 0)
            {
                nDura = 0;
                UseItems[Grobal2.U_WEAPON].Dura = nDura;
                if (Race == ActorRace.Play)
                {
                    ((PlayObject)this).SendDelItems(UseItems[Grobal2.U_WEAPON]);
                    StdItem stdItem = M2Share.WorldEngine.GetStdItem(UseItems[Grobal2.U_WEAPON].Index);
                    if (stdItem.NeedIdentify == 1)
                    {
                        M2Share.EventSource.AddEventLog(3, MapName + "\t" + CurrX + "\t" + CurrY + "\t" + ChrName + "\t" + stdItem.Name + "\t" +
                                                           UseItems[Grobal2.U_WEAPON].MakeIndex + "\t" + HUtil32.BoolToIntStr(Race == ActorRace.Play) + "\t" + '0');
                    }
                }
                UseItems[Grobal2.U_WEAPON].Index = 0;
                SendMsg(this, Messages.RM_DURACHANGE, Grobal2.U_WEAPON, nDura, UseItems[Grobal2.U_WEAPON].DuraMax, 0, "");
            }
            else
            {
                UseItems[Grobal2.U_WEAPON].Dura = nDura;
            }
            if ((ushort)Math.Abs((nDura / 1.03)) != nDuraPoint)
            {
                SendMsg(this, Messages.RM_DURACHANGE, Grobal2.U_WEAPON, UseItems[Grobal2.U_WEAPON].Dura, UseItems[Grobal2.U_WEAPON].DuraMax, 0, "");
            }
        }

        protected virtual byte GetChrColor(BaseObject baseObject)
        {
            if (baseObject.Race == ActorRace.NPC) //增加NPC名字颜色单独控制
            {
                return M2Share.Config.NpcNameColor;
            }
            if (baseObject.CrazyMode)
            {
                return 0xF9;
            }
            if (baseObject.HolySeize)
            {
                return 0x7D;
            }
            if (baseObject.IsSlave && baseObject.SlaveExpLevel <= Grobal2.SlaveMaxLevel)
            {
                return M2Share.Config.SlaveColor[baseObject.SlaveExpLevel];
            }
            return baseObject.GetNameColor();
        }

        public static int GetLevelExp(int nLevel)
        {
            int result;
            if (nLevel <= Grobal2.MaxLevel)
            {
                result = M2Share.Config.NeedExps[nLevel];
            }
            else
            {
                result = M2Share.Config.NeedExps[M2Share.Config.NeedExps.Length];
            }
            return result;
        }

        protected virtual byte GetNameColor()
        {
            return NameColor;
        }

        public void HearMsg(string sMsg)
        {
            if (!string.IsNullOrEmpty(sMsg))
            {
                SendMsg(null, Messages.RM_HEAR, 0, M2Share.Config.btHearMsgFColor, M2Share.Config.btHearMsgBColor, 0, sMsg);
            }
        }

        protected bool InSafeArea()
        {
            if (Envir == null)
            {
                return false;
            }
            bool result = Envir.Flag.SafeArea;
            if (result)
            {
                return true;
            }
            for (int i = 0; i < M2Share.StartPointList.Count; i++)
            {
                if (M2Share.StartPointList[i].MapName == Envir.MapName)
                {
                    if (M2Share.StartPointList[i] != null)
                    {
                        int cX = M2Share.StartPointList[i].CurrX;
                        int cY = M2Share.StartPointList[i].CurrY;
                        if ((Math.Abs(CurrX - cX) <= 60) && (Math.Abs(CurrY - cY) <= 60))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        private void MonsterRecalcAbilitys()
        {
            WAbil.DC = (ushort)HUtil32.MakeLong(HUtil32.LoWord(WAbil.DC), HUtil32.HiWord(WAbil.DC));
            int maxHp = 0;
            if ((Race == ActorRace.MonsterWhiteskeleton) || (Race == ActorRace.MonsterElfmonster) || (Race == ActorRace.MonsterElfwarrior))
            {
                WAbil.DC = (ushort)HUtil32.MakeLong(HUtil32.LoWord(WAbil.DC), (ushort)HUtil32.Round((SlaveExpLevel * 0.1 + 0.3) * 3.0 * SlaveExpLevel + HUtil32.HiWord(WAbil.DC)));
                maxHp = maxHp + HUtil32.Round((SlaveExpLevel * 0.1 + 0.3) * WAbil.MaxHP) * SlaveExpLevel;
                maxHp = maxHp + WAbil.MaxHP;
                if (SlaveExpLevel > 0)
                {
                    WAbil.MaxHP = (ushort)maxHp;
                }
                else
                {
                    WAbil.MaxHP = WAbil.MaxHP;
                }
            }
            else
            {
                maxHp = WAbil.MaxHP;
                WAbil.DC = (ushort)HUtil32.MakeLong(HUtil32.LoWord(WAbil.DC), (ushort)HUtil32.Round(SlaveExpLevel * 2 + HUtil32.HiWord(WAbil.DC)));
                maxHp = maxHp + HUtil32.Round(WAbil.MaxHP * 0.15) * SlaveExpLevel;
                WAbil.MaxHP = (ushort)HUtil32._MIN(HUtil32.Round(WAbil.MaxHP + SlaveExpLevel * 60), maxHp);
            }
        }

        /// <summary>
        /// 发送优先级消息
        /// </summary>
        public void SendPriorityMsg(BaseObject baseObject, int wIdent, int wParam, int nParam1, int nParam2, int nParam3, string sMsg = "", MessagePriority Priority = MessagePriority.Normal)
        {
            try
            {
                HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
                if (!Ghost)
                {
                    SendMessage sendMessage = new SendMessage
                    {
                        wIdent = wIdent,
                        wParam = wParam,
                        nParam1 = nParam1,
                        nParam2 = nParam2,
                        nParam3 = nParam3,
                        DeliveryTime = 0,
                        BaseObject = baseObject.ActorId,
                        LateDelivery = false,
                        Buff = sMsg
                    };
                    MsgQueue.Enqueue(sendMessage, (byte)Priority);
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
        }

        public void SendMsg(BaseObject baseObject, int wIdent, int wParam, int nParam1, int nParam2, int nParam3,
            string sMsg)
        {
            try
            {
                HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
                if (!Ghost)
                {
                    SendMessage sendMessage = new SendMessage
                    {
                        wIdent = wIdent,
                        wParam = wParam,
                        nParam1 = nParam1,
                        nParam2 = nParam2,
                        nParam3 = nParam3,
                        DeliveryTime = 0,
                        BaseObject = baseObject.ActorId,
                        LateDelivery = false,
                        Buff = sMsg
                    };
                    MsgQueue.Enqueue(sendMessage, wIdent);
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
        }

        /// <summary>
        /// 发送延时消息
        /// </summary>
        public void SendDelayMsg(BaseObject baseObject, int wIdent, int wParam, int lParam1, int lParam2, int lParam3, string sMsg, int dwDelay)
        {
            try
            {
                HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
                if (!Ghost)
                {
                    SendMessage sendMessage = new SendMessage
                    {
                        wIdent = wIdent,
                        wParam = wParam,
                        nParam1 = lParam1,
                        nParam2 = lParam2,
                        nParam3 = lParam3,
                        DeliveryTime = HUtil32.GetTickCount() + dwDelay,
                        BaseObject = baseObject.ActorId,
                        LateDelivery = true,
                        Buff = sMsg
                    };
                    MsgQueue.Enqueue(sendMessage, wIdent);
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
        }

        /// <summary>
        /// 发送延时消息
        /// </summary>
        public void SendDelayMsg(int baseObject, short wIdent, int wParam, int lParam1, int lParam2, int lParam3, string sMsg, int dwDelay)
        {
            try
            {
                HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
                if (!Ghost)
                {
                    SendMessage sendMessage = new SendMessage
                    {
                        wIdent = wIdent,
                        wParam = wParam,
                        nParam1 = lParam1,
                        nParam2 = lParam2,
                        nParam3 = lParam3,
                        DeliveryTime = HUtil32.GetTickCount() + dwDelay,
                        LateDelivery = true,
                        Buff = sMsg
                    };
                    sendMessage.BaseObject = baseObject == Messages.RM_STRUCK ? Messages.RM_STRUCK : baseObject;
                    MsgQueue.Enqueue(sendMessage, wIdent);
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
        }

        internal void SendUpdateDelayMsg(BaseObject baseObject, short wIdent, short wParam, int lParam1, int lParam2,
            int lParam3, string sMsg, int dwDelay)
        {
            int i;
            HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
            try
            {
                i = 0;
                while (true)
                {
                    if (MsgQueue.Count <= i)
                    {
                        break;
                    }
                    if (MsgQueue.TryPeek(out SendMessage sendMessage, out int priority))
                    {
                        if ((sendMessage.wIdent == wIdent) && (sendMessage.nParam1 == lParam1))
                        {
                            MsgQueue.TryDequeue(out sendMessage, out priority);
                            Dispose(sendMessage);
                        }
                    }
                    i++;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
            SendDelayMsg(baseObject.ActorId, wIdent, wParam, lParam1, lParam2, lParam3, sMsg, dwDelay);
        }

        public void SendUpdateMsg(BaseObject baseObject, int wIdent, int wParam, int lParam1, int lParam2, int lParam3,
            string sMsg)
        {
            int i;
            try
            {
                HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
                i = 0;
                while (true)
                {
                    if (MsgQueue.Count <= i)
                    {
                        break;
                    }
                    if (MsgQueue.TryPeek(out SendMessage sendMessage, out int priority))
                    {
                        if (sendMessage.wIdent == wIdent)
                        {
                            MsgQueue.TryDequeue(out sendMessage, out priority);
                            Dispose(sendMessage);
                        }
                    }
                    i++;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
            SendMsg(baseObject, wIdent, wParam, lParam1, lParam2, lParam3, sMsg);
        }

        public void SendActionMsg(BaseObject baseObject, int wIdent, int wParam, int lParam1, int lParam2, int lParam3,
            string sMsg)
        {
            int i;
            HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
            try
            {
                i = 0;
                while (true)
                {
                    if (MsgQueue.Count <= i)
                    {
                        break;
                    }
                    if (MsgQueue.TryPeek(out SendMessage sendMessage, out int priority))
                    {
                        if ((sendMessage.wIdent == Messages.CM_TURN) || (sendMessage.wIdent == Messages.CM_WALK) ||
                            (sendMessage.wIdent == Messages.CM_SITDOWN) || (sendMessage.wIdent == Messages.CM_HORSERUN) ||
                            (sendMessage.wIdent == Messages.CM_RUN) || (sendMessage.wIdent == Messages.CM_HIT) ||
                            (sendMessage.wIdent == Messages.CM_HEAVYHIT) || (sendMessage.wIdent == Messages.CM_BIGHIT) ||
                            (sendMessage.wIdent == Messages.CM_POWERHIT) || (sendMessage.wIdent == Messages.CM_LONGHIT) ||
                            (sendMessage.wIdent == Messages.CM_WIDEHIT) || (sendMessage.wIdent == Messages.CM_FIREHIT))
                        {
                            MsgQueue.TryDequeue(out sendMessage, out priority);
                            Dispose(sendMessage);
                        }
                    }
                    i++;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
            SendMsg(baseObject, wIdent, wParam, lParam1, lParam2, lParam3, sMsg);
        }

        protected virtual bool GetMessage(out ProcessMessage msg)
        {
            bool result = false;
            int count = MsgQueue.Count;
            HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
            msg = null;
            try
            {
                while (count > 0)
                {
                    if (MsgQueue.TryDequeue(out SendMessage sendMessage, out int priority))
                    {
                        if ((sendMessage.DeliveryTime > 0) && (HUtil32.GetTickCount() < sendMessage.DeliveryTime)) //延时消息
                        {
                            count--;
                            MsgQueue.Enqueue(sendMessage, sendMessage.wIdent);
                            continue;
                        }
                        msg = new ProcessMessage();
                        msg.wIdent = sendMessage.wIdent;
                        msg.wParam = sendMessage.wParam;
                        msg.nParam1 = sendMessage.nParam1;
                        msg.nParam2 = sendMessage.nParam2;
                        msg.nParam3 = sendMessage.nParam3;
                        if (sendMessage.BaseObject > 0)
                        {
                            msg.BaseObject = sendMessage.BaseObject;
                        }
                        msg.DeliveryTime = sendMessage.DeliveryTime;
                        msg.LateDelivery = sendMessage.LateDelivery;
                        msg.Msg = sendMessage.Buff;
                        result = true;
                    }
                    break;
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
            return result;
        }

        public static bool GetMapBaseObjects(Envirnoment envir, int nX, int nY, int nRage, IList<BaseObject> rList)
        {
            const string sExceptionMsg = "[Exception] TBaseObject::GetMapBaseObjects";
            if (rList == null)
            {
                return false;
            }
            try
            {
                int nStartX = nX - nRage;
                int nEndX = nX + nRage;
                int nStartY = nY - nRage;
                int nEndY = nY + nRage;
                for (int x = nStartX; x <= nEndX; x++)
                {
                    for (int y = nStartY; y <= nEndY; y++)
                    {
                        bool cellSuccess = false;
                        MapCellInfo cellInfo = envir.GetCellInfo(x, y, ref cellSuccess);
                        if (cellSuccess && cellInfo.IsAvailable)
                        {
                            for (int i = 0; i < cellInfo.Count; i++)
                            {
                                CellObject cellObject = cellInfo.ObjList[i];
                                if (cellObject != null && cellObject.ActorObject)
                                {
                                    BaseObject baseObject = M2Share.ActorMgr.Get(cellObject.CellObjId);
                                    if (baseObject != null && !baseObject.Death && !baseObject.Ghost)
                                    {
                                        rList.Add(baseObject);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                M2Share.Logger.Error(sExceptionMsg);
            }
            return true;
        }

        public void SendRefMsg(int wIdent, int wParam, int nParam1, int nParam2, int nParam3, string sMsg)
        {
            const string sExceptionMsg = "[Exception] TBaseObject::SendRefMsg Name = {0}";
            if (Envir == null)
            {
                M2Share.Logger.Error(ChrName + " SendRefMsg nil PEnvir ");
                return;
            }
            if (ObMode || FixedHideMode)
            {
                SendMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg); // 如果隐身模式则只发送信息给自己
                return;
            }
            HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
            try
            {
                BaseObject baseObject;
                if (((HUtil32.GetTickCount() - SendRefMsgTick) >= 500) || (VisibleHumanList.Count == 0))
                {
                    SendRefMsgTick = HUtil32.GetTickCount();
                    VisibleHumanList.Clear();
                    short nLx = (short)(CurrX - M2Share.Config.SendRefMsgRange); // 12
                    short nHx = (short)(CurrX + M2Share.Config.SendRefMsgRange); // 12
                    short nLy = (short)(CurrY - M2Share.Config.SendRefMsgRange); // 12
                    short nHy = (short)(CurrY + M2Share.Config.SendRefMsgRange); // 12
                    for (short nCx = nLx; nCx <= nHx; nCx++)
                    {
                        for (short nCy = nLy; nCy <= nHy; nCy++)
                        {
                            bool cellSuccess = false;
                            MapCellInfo cellInfo = Envir.GetCellInfo(nCx, nCy, ref cellSuccess);
                            if (cellSuccess)
                            {
                                if (cellInfo.IsAvailable)
                                {
                                    for (int i = 0; i < cellInfo.Count; i++)
                                    {
                                        CellObject cellObject = cellInfo.ObjList[i];
                                        if (cellObject != null)
                                        {
                                            if (cellObject.ActorObject)
                                            {
                                                if ((HUtil32.GetTickCount() - cellObject.AddTime) >= 60 * 1000)
                                                {
                                                    cellInfo.Remove(cellObject);
                                                    if (cellInfo.Count <= 0)
                                                    {
                                                        cellInfo.Dispose();
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        baseObject = M2Share.ActorMgr.Get(cellObject.CellObjId);
                                                        if ((baseObject != null) && !baseObject.Ghost)
                                                        {
                                                            if (baseObject.Race == ActorRace.Play)
                                                            {
                                                                baseObject.SendMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
                                                                VisibleHumanList.Add(baseObject);
                                                            }
                                                            else if (baseObject.WantRefMsg)
                                                            {
                                                                if ((wIdent == Messages.RM_STRUCK) || (wIdent == Messages.RM_HEAR) || (wIdent == Messages.RM_DEATH))
                                                                {
                                                                    baseObject.SendMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
                                                                    VisibleHumanList.Add(baseObject);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        cellInfo.Remove(cellObject);
                                                        if (cellInfo.Count <= 0)
                                                        {
                                                            cellInfo.Dispose();
                                                        }
                                                        M2Share.Logger.Error(Format(sExceptionMsg, ChrName));
                                                        M2Share.Logger.Error(e.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return;
                }

                for (int nC = 0; nC < VisibleHumanList.Count; nC++)
                {
                    baseObject = VisibleHumanList[nC];
                    if (baseObject.Ghost)
                    {
                        continue;
                    }
                    if ((baseObject.Envir == Envir) && (Math.Abs(baseObject.CurrX - CurrX) < 11) && (Math.Abs(baseObject.CurrY - CurrY) < 11))
                    {
                        if (baseObject.Race == ActorRace.Play)
                        {
                            baseObject.SendMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
                        }
                        else if (baseObject.WantRefMsg)
                        {
                            if ((wIdent == Messages.RM_STRUCK) || (wIdent == Messages.RM_HEAR) || (wIdent == Messages.RM_DEATH))
                            {
                                baseObject.SendMsg(this, wIdent, wParam, nParam1, nParam2, nParam3, sMsg);
                            }
                        }
                    }
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
        }

        public int GetFeatureToLong()
        {
            return GetFeature(null);
        }

        public virtual int GetFeature(BaseObject baseObject)
        {
            return M2Share.MakeMonsterFeature(RaceImg, MonsterWeapon, Appr);
        }

        public int GetCharStatus()
        {
            //0x80000000 指十六进制值，转成二进制则为10000000000000000000000000000000 然后Shr右移
            //例：I为3,右移3位，得到二进制值：10000000000000000000000000000
            //    I为6,右移6位，得到二进制值: 10000000000000000000000000
            //or 代表运算, 需要两个运算数，即两个数的位运算，只有其中一个是1就返回1; 都是0才返回0
            //and 表示 当对应位均为1时返回1，其余为0
            //从上面算法得到，最终 nStatus得到是1,
            int nStatus = 0;
            for (int i = 0; i < StatusTimeArr.Length; i++)
            {
                if (StatusTimeArr[i] > 0)
                {
                    nStatus = (int)(nStatus | (0x80000000 >> i));
                }
            }
            return nStatus | (CharStatusEx & 0x0000FFFF);
        }

        public void AbilCopyToWAbil()
        {
            WAbil = (Ability)Abil.Clone();
        }

        public virtual void Initialize()
        {
            AbilCopyToWAbil();
            AddtoMapSuccess = true;
            if (Envir.CanWalk(CurrX, CurrY, true) && AddToMap())
            {
                AddtoMapSuccess = false;
            }
            CharStatus = GetCharStatus();
        }

        /// <summary>
        /// 取怪物说话信息列表
        /// </summary>
        internal void LoadSayMsg()
        {
            for (int i = 0; i < M2Share.MonSayMsgList.Count; i++)
            {
                if (M2Share.MonSayMsgList.TryGetValue(ChrName, out SayMsgList))
                {
                    break;
                }
            }
        }

        public virtual void Disappear()
        {

        }

        public void FeatureChanged()
        {
            SendRefMsg(Messages.RM_FEATURECHANGED, GetFeatureEx(), GetFeatureToLong(), 0, 0, "");
        }

        public virtual ushort GetFeatureEx()
        {
            return 0;
        }

        public void StatusChanged()
        {
            SendRefMsg(Messages.RM_CHARSTATUSCHANGED, HitSpeed, CharStatus, 0, 0, "");
        }

        protected void DisappearA()
        {
            Envir.DeleteFromMap(CurrX, CurrY, MapCell, this);
            SendRefMsg(Messages.RM_DISAPPEAR, 0, 0, 0, 0, "");
        }

        protected void KickException()
        {
            if (Race == ActorRace.Play)
            {
                MapName = M2Share.Config.HomeMap;
                CurrX = M2Share.Config.HomeX;
                CurrY = M2Share.Config.HomeY;
                ((PlayObject)this).BoEmergencyClose = true;
            }
            else
            {
                Death = true;
                DeathTick = HUtil32.GetTickCount();
                MakeGhost();
            }
        }

        protected bool Walk(int nIdent)
        {
            const string sExceptionMsg = "[Exception] TBaseObject::Walk {0} {1} {2}:{3}";
            bool result = true;
            if (Envir == null)
            {
                M2Share.Logger.Error("Walk nil PEnvir");
                return true;
            }
            try
            {
                bool cellSuccess = false;
                MapCellInfo cellInfo = Envir.GetCellInfo(CurrX, CurrY, ref cellSuccess);
                if (cellSuccess && cellInfo.IsAvailable)
                {
                    for (int i = 0; i < cellInfo.Count; i++)
                    {
                        CellObject cellObject = cellInfo.ObjList[i];
                        if (cellObject == null)
                        {
                            continue;
                        }
                        switch (cellObject.CellType)
                        {
                            case CellType.Route:
                                GateObject gateObj = (GateObject)M2Share.CellObjectMgr.Get(cellObject.CellObjId);
                                if (gateObj != null)
                                {
                                    if (Race == ActorRace.Play)
                                    {
                                        if (Envir.ArroundDoorOpened(CurrX, CurrY))
                                        {
                                            if ((!gateObj.Envir.Flag.boNEEDHOLE) || (M2Share.EventMgr.GetEvent(Envir, CurrX, CurrY, Grobal2.ET_DIGOUTZOMBI) != null))
                                            {
                                                if (M2Share.ServerIndex == gateObj.Envir.ServerIndex)
                                                {
                                                    if (!EnterAnotherMap(gateObj.Envir, gateObj.X, gateObj.Y))
                                                    {
                                                        result = false;
                                                    }
                                                }
                                                else
                                                {
                                                    DisappearA();
                                                    SpaceMoved = true;
                                                    ((PlayObject)this).ChangeSpaceMove(gateObj.Envir, gateObj.X, gateObj.Y);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result = false;
                                    }
                                }
                                break;
                            case CellType.Event:
                                {
                                    EventInfo mapEvent = null;
                                    EventInfo owinEvent = (EventInfo)M2Share.CellObjectMgr.Get(cellObject.CellObjId);
                                    if (owinEvent.OwnBaseObject != null)
                                    {
                                        mapEvent = (EventInfo)M2Share.CellObjectMgr.Get(cellObject.CellObjId);
                                    }
                                    if (mapEvent != null)
                                    {
                                        if (mapEvent.OwnBaseObject.IsProperTarget(this))
                                        {
                                            SendMsg(mapEvent.OwnBaseObject, Messages.RM_MAGSTRUCK_MINE, 0, mapEvent.Damage, 0, 0, "");
                                        }
                                    }
                                    break;
                                }
                            case CellType.MapEvent:
                                break;
                            case CellType.Door:
                                break;
                            case CellType.Roon:
                                break;
                        }
                    }
                }
                if (result)
                {
                    SendRefMsg(nIdent, Direction, CurrX, CurrY, 0, "");
                }
            }
            catch (Exception e)
            {
                M2Share.Logger.Error(Format(sExceptionMsg, ChrName, MapName, CurrX, CurrY));
                M2Share.Logger.Error(e.Message);
            }
            return result;
        }

        /// <summary>
        /// 切换地图
        /// </summary>
        private bool EnterAnotherMap(Envirnoment envir, short nDMapX, short nDMapY)
        {
            bool result = false;
            const string sExceptionMsg = "[Exception] TBaseObject::EnterAnotherMap";
            try
            {
                if (Abil.Level < envir.EnterLevel)
                {
                    SysMsg($"需要 {envir.Flag.RequestLevel - 1} 级以上才能进入 {envir.MapDesc}", MsgColor.Red, MsgType.Hint);
                    return false;
                }
                if (envir.QuestNpc != null)
                {
                    envir.QuestNpc.Click((PlayObject)this);
                }
                if (envir.Flag.NeedSetonFlag >= 0)
                {
                    if (((PlayObject)this).GetQuestFalgStatus(envir.Flag.NeedSetonFlag) != envir.Flag.NeedOnOff)
                    {
                        return false;
                    }
                }
                bool cellSuccess = false;
                envir.GetCellInfo(nDMapX, nDMapY, ref cellSuccess);
                if (!cellSuccess)
                {
                    return false;
                }
                UserCastle castle = M2Share.CastleMgr.IsCastlePalaceEnvir(envir);
                if ((castle != null) && (Race == ActorRace.Play))
                {
                    if (!castle.CheckInPalace(CurrX, CurrY))
                    {
                        return false;
                    }
                }
                if (envir.Flag.NoHorse)
                {
                    OnHorse = false;
                }
                Envirnoment oldEnvir = Envir;
                short nOldX = CurrX;
                short nOldY = CurrY;
                DisappearA();
                VisibleHumanList.Clear();
                for (int i = 0; i < VisibleItems.Count; i++)
                {
                    VisibleItems[i] = null;
                }
                VisibleItems.Clear();
                VisibleEvents.Clear();
                for (int i = 0; i < VisibleActors.Count; i++)
                {
                    VisibleActors[i] = null;
                }
                VisibleActors.Clear();
                SendMsg(this, Messages.RM_CLEAROBJECTS, 0, 0, 0, 0, "");
                Envir = envir;
                MapName = envir.MapName;
                MapFileName = envir.MapFileName;
                CurrX = nDMapX;
                CurrY = nDMapY;
                SendMsg(this, Messages.RM_CHANGEMAP, 0, 0, 0, 0, envir.MapFileName);
                if (AddToMap())
                {
                    MapMoveTick = HUtil32.GetTickCount();
                    SpaceMoved = true;
                    result = true;
                }
                else
                {
                    Envir = oldEnvir;
                    CurrX = nOldX;
                    CurrY = nOldY;
                    Envir.AddToMap(CurrX, CurrY, MapCell, this);
                }
                OnEnvirnomentChanged();
                if (Race == ActorRace.Play) // 复位泡点，及金币，时间
                {
                    ((PlayObject)this).IncGamePointTick = HUtil32.GetTickCount();
                    ((PlayObject)this).IncGameGoldTick = HUtil32.GetTickCount();
                    ((PlayObject)this).AutoGetExpTick = HUtil32.GetTickCount();
                }
                if (Envir.Flag.Fight3Zone && (Envir.Flag.Fight3Zone != oldEnvir.Flag.Fight3Zone))
                {
                    RefShowName();
                }
            }
            catch
            {
                M2Share.Logger.Error(sExceptionMsg);
            }
            return result;
        }

        protected void TurnTo(byte nDir)
        {
            Direction = nDir;
            SendRefMsg(Messages.RM_TURN, nDir, CurrX, CurrY, 0, "");
        }

        public void SysMsg(string sMsg, MsgColor msgColor, MsgType msgType)
        {
            if (M2Share.Config.ShowPreFixMsg)
            {
                switch (msgType)
                {
                    case MsgType.Mon:
                        sMsg = M2Share.Config.MonSayMsgPreFix + sMsg;
                        break;
                    case MsgType.Hint:
                        sMsg = M2Share.Config.HintMsgPreFix + sMsg;
                        break;
                    case MsgType.GameManger:
                        sMsg = M2Share.Config.GameManagerRedMsgPreFix + sMsg;
                        break;
                    case MsgType.System:
                        sMsg = M2Share.Config.SysMsgPreFix + sMsg;
                        break;
                    case MsgType.Cust:
                        sMsg = M2Share.Config.CustMsgPreFix + sMsg;
                        break;
                    case MsgType.Castle:
                        sMsg = M2Share.Config.CastleMsgPreFix + sMsg;
                        break;
                }
            }
            if (msgType == MsgType.Notice) // 公告
            {
                string str = string.Empty;
                string fColor = string.Empty;
                string bColor = string.Empty;
                string nTime = string.Empty;
                switch (sMsg[0])
                {
                    case '[':// 顶部滚动公告
                        {
                            sMsg = HUtil32.ArrestStringEx(sMsg, "[", "]", ref str);
                            bColor = HUtil32.GetValidStrCap(str, ref fColor, ',');
                            if (M2Share.Config.ShowPreFixMsg)
                            {
                                sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                            }
                            SendMsg(this, Messages.RM_MOVEMESSAGE, 0, HUtil32.StrToInt(fColor, 255), HUtil32.StrToInt(bColor, 255), 0, sMsg);
                            break;
                        }
                    case '<':// 聊天框彩色公告
                        {
                            sMsg = HUtil32.ArrestStringEx(sMsg, "<", ">", ref str);
                            bColor = HUtil32.GetValidStrCap(str, ref fColor, ',');
                            if (M2Share.Config.ShowPreFixMsg)
                            {
                                sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                            }
                            SendMsg(this, Messages.RM_SYSMESSAGE, 0, HUtil32.StrToInt(fColor, 255), HUtil32.StrToInt(bColor, 255), 0, sMsg);
                            break;
                        }
                    case '{': // 屏幕居中公告
                        {
                            sMsg = HUtil32.ArrestStringEx(sMsg, "{", "}", ref str);
                            str = HUtil32.GetValidStrCap(str, ref fColor, ',');
                            str = HUtil32.GetValidStrCap(str, ref bColor, ',');
                            str = HUtil32.GetValidStrCap(str, ref nTime, ',');
                            if (M2Share.Config.ShowPreFixMsg)
                            {
                                sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                            }
                            SendMsg(this, Messages.RM_MOVEMESSAGE, 1, HUtil32.StrToInt(fColor, 255), HUtil32.StrToInt(bColor, 255), HUtil32.StrToInt(nTime, 0), sMsg);
                            break;
                        }
                    default:
                        switch (msgColor)
                        {
                            case MsgColor.Red: // 控制公告的颜色
                                if (M2Share.Config.ShowPreFixMsg)
                                {
                                    sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                                }
                                SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.RedMsgFColor, M2Share.Config.RedMsgBColor, 0, sMsg);
                                break;
                            case MsgColor.Green:
                                if (M2Share.Config.ShowPreFixMsg)
                                {
                                    sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                                }
                                SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.GreenMsgFColor, M2Share.Config.GreenMsgBColor, 0, sMsg);
                                break;
                            case MsgColor.Blue:
                                if (M2Share.Config.ShowPreFixMsg)
                                {
                                    sMsg = M2Share.Config.LineNoticePreFix + sMsg;
                                }
                                SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.BlueMsgFColor, M2Share.Config.BlueMsgBColor, 0, sMsg);
                                break;
                        }
                        break;
                }
            }
            else
            {
                switch (msgColor)
                {
                    case MsgColor.Green:
                        SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.GreenMsgFColor, M2Share.Config.GreenMsgBColor, 0, sMsg);
                        break;
                    case MsgColor.Blue:
                        SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.BlueMsgFColor, M2Share.Config.BlueMsgBColor, 0, sMsg);
                        break;
                    default:
                        if (msgType == MsgType.Cust)
                        {
                            SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.CustMsgFColor, M2Share.Config.CustMsgBColor, 0, sMsg);
                        }
                        else
                        {
                            SendMsg(this, Messages.RM_SYSMESSAGE, 0, M2Share.Config.RedMsgFColor, M2Share.Config.RedMsgBColor, 0, sMsg);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 怪物说话
        /// </summary>
        protected void MonsterSayMsg(BaseObject attackBaseObject, MonStatus monStatus)
        {
            if (!M2Share.Config.MonSayMsg)
            {
                return;
            }
            if (Race == ActorRace.Play)
            {
                return;
            }
            if (SayMsgList == null)
            {
                return;
            }
            if (attackBaseObject == null)
            {
                return;
            }
            string sAttackName;
            if ((attackBaseObject.Race != ActorRace.Play) && (attackBaseObject.Master == null))
            {
                return;
            }
            if (attackBaseObject.Master != null)
            {
                sAttackName = attackBaseObject.Master.ChrName;
            }
            else
            {
                sAttackName = attackBaseObject.ChrName;
            }
            for (int i = 0; i < SayMsgList.Count; i++)
            {
                MonsterSayMsg monSayMsg = SayMsgList[i];
                string sMsg = monSayMsg.sSayMsg.Replace("%s", M2Share.FilterShowName(ChrName));
                sMsg = sMsg.Replace("%d", sAttackName);
                if ((monSayMsg.State == monStatus) && (M2Share.RandomNumber.Random(monSayMsg.nRate) == 0))
                {
                    if (monStatus == MonStatus.MonGen)
                    {
                        M2Share.WorldEngine.SendBroadCastMsg(sMsg, MsgType.Mon);
                        break;
                    }
                    if (monSayMsg.Color == MsgColor.White)
                    {
                        ProcessSayMsg(sMsg);
                    }
                    else
                    {
                        attackBaseObject.SysMsg(sMsg, monSayMsg.Color, MsgType.Mon);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 设置肉的品质
        /// </summary>
        protected void ApplyMeatQuality()
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                StdItem stdItem = M2Share.WorldEngine.GetStdItem(ItemList[i].Index);
                if (stdItem != null)
                {
                    if (stdItem.StdMode == 40)
                    {
                        ItemList[i].Dura = MeatQuality;
                    }
                }
            }
        }

        protected bool TakeBagItems(BaseObject baseObject)
        {
            bool result = false;
            while (true)
            {
                if (baseObject.ItemList.Count <= 0)
                {
                    break;
                }
                UserItem userItem = baseObject.ItemList[0];
                if (!AddItemToBag(userItem))
                {
                    break;
                }
                if (this is PlayObject)
                {
                    ((PlayObject)this)?.SendAddItem(userItem);
                    result = true;
                }
                baseObject.ItemList.RemoveAt(0);
            }
            return result;
        }

        /// <summary>
        /// 散落金币
        /// </summary>
        /// <param name="goldOfCreat"></param>
        internal void ScatterGolds(int goldOfCreat)
        {
            int I;
            int nGold;
            if (Gold > 0)
            {
                I = 0;
                while (true)
                {
                    if (Gold > M2Share.Config.MonOneDropGoldCount)
                    {
                        nGold = M2Share.Config.MonOneDropGoldCount;
                        Gold = Gold - M2Share.Config.MonOneDropGoldCount;
                    }
                    else
                    {
                        nGold = Gold;
                        Gold = 0;
                    }
                    if (nGold > 0)
                    {
                        if (!DropGoldDown(nGold, true, goldOfCreat, this.ActorId))
                        {
                            Gold = Gold + nGold;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }

                    I++;
                    if (I >= 17)
                    {
                        break;
                    }
                }
                GoldChanged();
            }
        }

        public void SetLastHiter(BaseObject baseObject)
        {
            LastHiter = baseObject;
            LastHiterTick = HUtil32.GetTickCount();
            if (ExpHitter == null)
            {
                ExpHitter = baseObject;
                ExpHitterTick = HUtil32.GetTickCount();
            }
            else
            {
                if (ExpHitter == baseObject)
                {
                    ExpHitterTick = HUtil32.GetTickCount();
                }
            }
        }

        internal static bool IsGoodKilling(BaseObject cert)
        {
            if (cert.Race == ActorRace.Play)
            {
                return ((PlayObject)cert).PvpFlag;
            }
            return false;
        }

        /// <summary>
        /// 是否可以攻击的目标
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsAttackTarget(BaseObject baseObject)
        {
            bool result = false;
            if ((baseObject == null) || (baseObject == this))
            {
                return false;
            }
            if (baseObject.AdminMode || baseObject.StoneMode)
            {
                return false;
            }
            if (Race >= ActorRace.Animal)
            {
                if (Master != null)
                {
                    if ((Master.LastHiter == baseObject) || (Master.ExpHitter == baseObject) || (Master.TargetCret == baseObject))
                    {
                        result = true;
                    }
                    if (baseObject.TargetCret != null)
                    {
                        if ((baseObject.TargetCret == Master) || (baseObject.TargetCret.Master == Master) && (baseObject.Race != ActorRace.Play))
                        {
                            result = true;
                        }
                    }
                    if ((baseObject.TargetCret == this) && (baseObject.Race >= ActorRace.Animal))
                    {
                        result = true;
                    }
                    if (baseObject.Master != null)
                    {
                        if ((baseObject.Master == Master.LastHiter) || (baseObject.Master == Master.TargetCret))
                        {
                            result = true;
                        }
                    }
                    if (baseObject.Master == Master)
                    {
                        result = false;
                    }
                    if (baseObject.HolySeize)
                    {
                        result = false;
                    }
                    if (Master.SlaveRelax)
                    {
                        result = false;
                    }
                    if (baseObject.Race == ActorRace.Play)
                    {
                        if (baseObject.InSafeZone())
                        {
                            result = false;
                        }
                    }
                    BreakCrazyMode();
                }
                else
                {
                    if (baseObject.Race == ActorRace.Play)
                    {
                        result = true;
                    }
                    if ((Race > ActorRace.PeaceNpc) && (Race < ActorRace.Animal))
                    {
                        result = true;
                    }
                    if (baseObject.Master != null)
                    {
                        result = true;
                    }
                }
                if (CrazyMode && ((baseObject.Race == ActorRace.Play) || (baseObject.Race > ActorRace.PeaceNpc)))
                {
                    result = true;
                }
                if (NastyMode && ((baseObject.Race < ActorRace.NPC) || (baseObject.Race > ActorRace.PeaceNpc)))
                {
                    result = true;
                }
                return result;
            }
            return true;
        }

        /// <summary>
        /// 检查对象是否可以被攻击
        /// </summary>
        /// <returns></returns>
        public virtual bool IsProperTarget(BaseObject baseObject)
        {
            return IsAttackTarget(baseObject);
        }

        protected void WeightChanged()
        {
            WAbil.Weight = RecalcBagWeight();
            SendUpdateMsg(this, Messages.RM_WEIGHTCHANGED, 0, 0, 0, 0, "");
        }

        public bool InSafeZone()
        {
            if (Envir == null)
            {
                return true;
            }
            bool result = Envir.Flag.SafeArea;
            if (result)
            {
                return true;
            }
            if ((Envir.MapName != M2Share.Config.RedHomeMap) ||
                (Math.Abs(CurrX - M2Share.Config.RedHomeX) > M2Share.Config.SafeZoneSize) ||
                (Math.Abs(CurrY - M2Share.Config.RedHomeY) > M2Share.Config.SafeZoneSize))
            {
                result = false;
            }
            else
            {
                return true;
            }
            for (int i = 0; i < M2Share.StartPointList.Count; i++)
            {
                if (M2Share.StartPointList[i].MapName == Envir.MapName)
                {
                    if (M2Share.StartPointList[i] != null)
                    {
                        int nSafeX = M2Share.StartPointList[i].CurrX;
                        int nSafeY = M2Share.StartPointList[i].CurrY;
                        if ((Math.Abs(CurrX - nSafeX) <= M2Share.Config.SafeZoneSize) &&
                            (Math.Abs(CurrY - nSafeY) <= M2Share.Config.SafeZoneSize))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public bool InSafeZone(Envirnoment envir, int nX, int nY)
        {
            if (Envir == null)
            {
                return true;
            }
            bool result = Envir.Flag.SafeArea;
            if (result)
            {
                return true;
            }
            if ((envir.MapName != M2Share.Config.RedHomeMap) ||
                (Math.Abs(nX - M2Share.Config.RedHomeX) > M2Share.Config.SafeZoneSize) ||
                (Math.Abs(nY - M2Share.Config.RedHomeY) > M2Share.Config.SafeZoneSize))
            {
                result = false;
            }
            else
            {
                return true;
            }
            for (int i = 0; i < M2Share.StartPointList.Count; i++)
            {
                if (M2Share.StartPointList[i].MapName == envir.MapName)
                {
                    if (M2Share.StartPointList[i] != null)
                    {
                        int nSafeX = M2Share.StartPointList[i].CurrX;
                        int nSafeY = M2Share.StartPointList[i].CurrY;
                        if ((Math.Abs(nX - nSafeX) <= M2Share.Config.SafeZoneSize) &&
                            (Math.Abs(nY - nSafeY) <= M2Share.Config.SafeZoneSize))
                        {
                            result = true;
                        }
                    }
                }
            }
            return result;
        }

        public void OpenHolySeizeMode(int dwInterval)
        {
            HolySeize = true;
            HolySeizeTick = HUtil32.GetTickCount();
            HolySeizeInterval = dwInterval;
            RefNameColor();
        }

        public void BreakHolySeizeMode()
        {
            HolySeize = false;
            RefNameColor();
        }

        public void OpenCrazyMode(int nTime)
        {
            CrazyMode = true;
            CrazyModeTick = HUtil32.GetTickCount();
            CrazyModeInterval = nTime * 1000;
            RefNameColor();
        }

        public void BreakCrazyMode()
        {
            if (CrazyMode)
            {
                CrazyMode = false;
                RefNameColor();
            }
        }

        /// <summary>
        /// 召唤属下
        /// </summary>
        /// <param name="sSlaveName"></param>
        public void RecallSlave(string sSlaveName)
        {
            short nX = 0;
            short nY = 0;
            int nFlag = -1;
            GetFrontPosition(ref nX, ref nY);
            if (sSlaveName == M2Share.Config.Dragon)
            {
                nFlag = 1;
            }
            for (int i = SlaveList.Count - 1; i >= 0; i--)
            {
                if (nFlag == 1)
                {
                    if ((SlaveList[i].ChrName == M2Share.Config.Dragon) || (SlaveList[i].ChrName == M2Share.Config.Dragon1))
                    {
                        SlaveList[i].SpaceMove(Envir.MapName, nX, nY, 1);
                        break;
                    }
                }
                else if (SlaveList[i].ChrName == sSlaveName)
                {
                    SlaveList[i].SpaceMove(Envir.MapName, nX, nY, 1);
                    break;
                }
            }
        }

        public virtual ushort GetHitStruckDamage(BaseObject target, int nDamage)
        {
            int nArmor;
            int nRnd = HUtil32.LoByte(WAbil.AC) + M2Share.RandomNumber.Random(Math.Abs(HUtil32.HiByte(WAbil.AC) - HUtil32.LoByte(WAbil.AC)) + 1);
            if (nRnd > 0)
            {
                nArmor = HUtil32.LoByte(WAbil.AC) + M2Share.RandomNumber.Random(nRnd);
            }
            else
            {
                nArmor = HUtil32.LoByte(WAbil.AC);
            }
            nDamage = HUtil32._MAX(0, nDamage - nArmor);
            if (nDamage > 0)
            {
                if ((LifeAttrib == Grobal2.LA_UNDEAD) && (target != null))
                {
                    nDamage += target.AddAbil.UndeadPower;
                }
            }
            return (ushort)nDamage;
        }

        public virtual ushort GetMagStruckDamage(BaseObject baseObject, ushort nDamage)
        {
            int n14 = HUtil32.LoByte(WAbil.MAC) + M2Share.RandomNumber.Random(Math.Abs(HUtil32.HiByte(WAbil.MAC) - HUtil32.LoByte(WAbil.MAC)) + 1);
            nDamage = (ushort)HUtil32._MAX(0, nDamage - n14);
            if ((LifeAttrib == Grobal2.LA_UNDEAD) && (baseObject != null))
            {
                nDamage += AddAbil.UndeadPower;
            }
            return nDamage;
        }

        public void StruckDamage(ushort nDamage)
        {
            PlayObject playObject;
            StdItem stdItem;
            bool bo19;
            if (nDamage <= 0)
            {
                return;
            }
            if ((Race >= 50) && (LastHiter != null) && (LastHiter.Race == ActorRace.Play)) // 人攻击怪物
            {
                switch (((PlayObject)LastHiter).Job)
                {
                    case PlayJob.Warrior:
                        nDamage = (ushort)(nDamage * M2Share.Config.WarrMon / 10);
                        break;
                    case PlayJob.Wizard:
                        nDamage = (ushort)(nDamage * M2Share.Config.WizardMon / 10);
                        break;
                    case PlayJob.Taoist:
                        nDamage = (ushort)(nDamage * M2Share.Config.TaosMon / 10);
                        break;
                }
            }
            if ((Race == ActorRace.Play) && (LastHiter != null) && (LastHiter.Master != null)) // 怪物攻击人
            {
                nDamage = (ushort)(nDamage * M2Share.Config.MonHum / 10);
            }
            ushort nDam = (ushort)(M2Share.RandomNumber.Random(10) + 5);
            if (StatusTimeArr[PoisonState.DAMAGEARMOR] > 0)
            {
                nDam = (ushort)HUtil32.Round(nDam * (M2Share.Config.PosionDamagarmor / 10)); // 1.2
                nDamage = (ushort)HUtil32.Round(nDamage * (M2Share.Config.PosionDamagarmor / 10)); // 1.2
            }
            bo19 = false;
            ushort nDura;
            int nOldDura;
            if (UseItems[Grobal2.U_DRESS] != null && UseItems[Grobal2.U_DRESS].Index > 0)
            {
                nDura = UseItems[Grobal2.U_DRESS].Dura;
                nOldDura = HUtil32.Round(nDura / 1000);
                nDura -= nDam;
                if (nDura <= 0)
                {
                    if (Race == ActorRace.Play)
                    {
                        playObject = this as PlayObject;
                        playObject.SendDelItems(UseItems[Grobal2.U_DRESS]);
                        stdItem = M2Share.WorldEngine.GetStdItem(UseItems[Grobal2.U_DRESS].Index);
                        if (stdItem.NeedIdentify == 1)
                        {
                            M2Share.EventSource.AddEventLog(3, MapName + "\t" + CurrX + "\t" + CurrY + "\t" +
                                                               ChrName + "\t" + stdItem.Name + "\t" +
                                                               UseItems[Grobal2.U_DRESS].MakeIndex + "\t"
                                                               + HUtil32.BoolToIntStr(Race == ActorRace.Play) +
                                                               "\t" + '0');
                        }
                        UseItems[Grobal2.U_DRESS].Index = 0;
                        FeatureChanged();
                    }
                    UseItems[Grobal2.U_DRESS].Index = 0;
                    UseItems[Grobal2.U_DRESS].Dura = 0;
                    bo19 = true;
                }
                else
                {
                    UseItems[Grobal2.U_DRESS].Dura = (ushort)nDura;
                }

                if (nOldDura != HUtil32.Round(nDura / 1000))
                {
                    SendMsg(this, Messages.RM_DURACHANGE, Grobal2.U_DRESS, nDura, UseItems[Grobal2.U_DRESS].DuraMax, 0, "");
                }
            }

            for (int i = 0; i < UseItems.Length; i++)
            {
                if ((UseItems[i] != null) && (UseItems[i].Index > 0) && (M2Share.RandomNumber.Random(8) == 0))
                {
                    nDura = UseItems[i].Dura;
                    nOldDura = HUtil32.Round(nDura / 1000);
                    nDura -= nDam;
                    if (nDura <= 0)
                    {
                        if (Race == ActorRace.Play)
                        {
                            playObject = this as PlayObject;
                            playObject.SendDelItems(UseItems[i]);
                            stdItem = M2Share.WorldEngine.GetStdItem(UseItems[i].Index);
                            if (stdItem.NeedIdentify == 1)
                            {
                                M2Share.EventSource.AddEventLog(3, MapName + "\t" + CurrX + "\t" + CurrY + "\t" + ChrName + "\t" + stdItem.Name + "\t" +
                                                       UseItems[i].MakeIndex + "\t" + HUtil32.BoolToIntStr(Race == ActorRace.Play) + "\t" + '0');
                            }
                            UseItems[i].Index = 0;
                            FeatureChanged();
                        }
                        UseItems[i].Index = 0;
                        UseItems[i].Dura = 0;
                        bo19 = true;
                    }
                    else
                    {
                        UseItems[i].Dura = (ushort)nDura;
                    }
                    if (nOldDura != HUtil32.Round(nDura / 1000))
                    {
                        SendMsg(this, Messages.RM_DURACHANGE, i, nDura, UseItems[i].DuraMax, 0, "");
                    }
                }
            }
            if (bo19)
            {
                RecalcAbilitys();
            }
            DamageHealth(nDamage);
        }

        public virtual string GetBaseObjectInfo()
        {
            return ChrName + ' ' + "地图:" + MapName + '(' + Envir.MapDesc + ") " + "座标:" + CurrX +
                         '/' + CurrY + ' ' + "等级:" + Abil.Level + ' ' + "经验:" + Abil.Exp + ' ' + "生命值: " + WAbil.HP + '-' + WAbil.MaxHP + ' ' + "魔法值: " + WAbil.MP + '-' +
                         WAbil.MaxMP + ' ' + "攻击力: " + HUtil32.LoByte(WAbil.DC) + '-' +
                         HUtil32.HiByte(WAbil.DC) + ' ' + "魔法力: " + HUtil32.LoByte(WAbil.MC) + '-' + HUtil32.HiByte(WAbil.MC) + ' ' + "道术: " +
                         HUtil32.LoByte(WAbil.SC) + '-' + HUtil32.HiByte(WAbil.SC) + ' ' + "防御力: " + HUtil32.LoByte(WAbil.AC) + '-' + HUtil32.HiByte(WAbil.AC) + ' ' + "魔防力: " +
                         HUtil32.LoByte(WAbil.MAC) + '-' + HUtil32.HiByte(WAbil.MAC) + ' ' + "准确:" + HitPoint + ' ' + "敏捷:" + SpeedPoint;
        }

        public bool GetBackPosition(ref short nX, ref short nY)
        {
            Envirnoment envir = Envir;
            nX = CurrX;
            nY = CurrY;
            switch (Direction)
            {
                case Grobal2.DR_UP:
                    if (nY < (envir.Height - 1))
                    {
                        nY++;
                    }
                    break;
                case Grobal2.DR_DOWN:
                    if (nY > 0)
                    {
                        nY -= 1;
                    }
                    break;
                case Grobal2.DR_LEFT:
                    if (nX < (envir.Width - 1))
                    {
                        nX++;
                    }
                    break;
                case Grobal2.DR_RIGHT:
                    if (nX > 0)
                    {
                        nX -= 1;
                    }
                    break;
                case Grobal2.DR_UPLEFT:
                    if ((nX < (envir.Width - 1)) && (nY < (envir.Height - 1)))
                    {
                        nX++;
                        nY++;
                    }
                    break;
                case Grobal2.DR_UPRIGHT:
                    if ((nX < (envir.Width - 1)) && (nY > 0))
                    {
                        nX -= 1;
                        nY++;
                    }
                    break;
                case Grobal2.DR_DOWNLEFT:
                    if ((nX > 0) && (nY < (envir.Height - 1)))
                    {
                        nX++;
                        nY -= 1;
                    }
                    break;
                case Grobal2.DR_DOWNRIGHT:
                    if ((nX > 0) && (nY > 0))
                    {
                        nX -= 1;
                        nY -= 1;
                    }
                    break;
            }
            return true;
        }

        public bool MakePosion(int nType, ushort nTime, int nPoint)
        {
            if (nType >= Grobal2.MAX_STATUS_ATTRIBUTE)
                return false;
            int nOldCharStatus = CharStatus;
            if (StatusTimeArr[nType] > 0)
            {
                if (StatusTimeArr[nType] < nTime)
                {
                    StatusTimeArr[nType] = nTime;
                }
            }
            else
            {
                StatusTimeArr[nType] = nTime;
            }
            StatusArrTick[nType] = HUtil32.GetTickCount();
            CharStatus = GetCharStatus();
            GreenPoisoningPoint = (byte)nPoint;
            if (nOldCharStatus != CharStatus)
            {
                StatusChanged();
            }
            if (Race == ActorRace.Play)
            {
                SysMsg(Format(Settings.YouPoisoned, nTime, nPoint), MsgColor.Red, MsgType.Hint);
            }
            return true;
        }

        /// <summary>
        /// 检查是否正有跨服数据
        /// </summary>
        /// <returns></returns>
        public bool CheckServerMakeSlave()
        {
            bool result = false;
            HUtil32.EnterCriticalSection(M2Share.ProcessMsgCriticalSection);
            try
            {
                for (int i = 0; i < MsgQueue.Count; i++)
                {
                    if (MsgQueue.TryPeek(out SendMessage sendMessage, out int priority))
                    {
                        if (sendMessage.wIdent == Messages.RM_10401)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            finally
            {
                HUtil32.LeaveCriticalSection(M2Share.ProcessMsgCriticalSection);
            }
            return result;
        }

        protected bool GetRecallXy(short nX, short nY, int nRange, ref short nDx, ref short nDy)
        {
            bool result = false;
            if (Envir.GetMovingObject(nX, nY, true) == null)
            {
                result = true;
                nDx = nX;
                nDy = nY;
            }
            if (!result)
            {
                for (int i = 0; i < nRange; i++)
                {
                    for (int j = -i; j <= i; j++)
                    {
                        for (int k = -i; k <= i; k++)
                        {
                            nDx = (short)(nX + k);
                            nDy = (short)(nY + j);
                            if (Envir.GetMovingObject(nDx, nDy, true) == null)
                            {
                                result = true;
                                break;
                            }
                        }
                        if (result)
                        {
                            break;
                        }
                    }
                    if (result)
                    {
                        break;
                    }
                }
            }
            if (!result)
            {
                nDx = nX;
                nDy = nY;
            }
            return result;
        }

        /// <summary>
        /// 破魔法盾
        /// </summary>
        /// <param name="nInt"></param>
        internal void DamageBubbleDefence(int nInt)
        {
            if (StatusTimeArr[PoisonState.BubbleDefenceUP] > 0)
            {
                if (StatusTimeArr[PoisonState.BubbleDefenceUP] > 3)
                {
                    StatusTimeArr[PoisonState.BubbleDefenceUP] -= 3;
                }
                else
                {
                    StatusTimeArr[PoisonState.BubbleDefenceUP] = 1;
                }
            }
        }

        public bool MagCanHitTarget(short nX, short nY, BaseObject targeBaseObject)
        {
            bool result = false;
            if (targeBaseObject == null)
            {
                return false;
            }
            int n20 = Math.Abs(nX - targeBaseObject.CurrX) + Math.Abs(nY - targeBaseObject.CurrY);
            int n14 = 0;
            while (n14 < 13)
            {
                byte n18 = M2Share.GetNextDirection(nX, nY, targeBaseObject.CurrX, targeBaseObject.CurrY);
                if (Envir.GetNextPosition(nX, nY, n18, 1, ref nX, ref nY) && Envir.IsValidCell(nX, nY))
                {
                    if ((nX == targeBaseObject.CurrX) && (nY == targeBaseObject.CurrY))
                    {
                        result = true;
                        break;
                    }
                    int n1C = Math.Abs(nX - targeBaseObject.CurrX) + Math.Abs(nY - targeBaseObject.CurrY);
                    if (n1C > n20)
                    {
                        result = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
                n14++;
            }
            return result;
        }

        public int MagMakeDefenceArea(int nX, int nY, int nRange, ushort nSec, byte btState)
        {
            int result = 0;
            int nStartX = nX - nRange;
            int nEndX = nX + nRange;
            int nStartY = nY - nRange;
            int nEndY = nY + nRange;
            for (int cX = nStartX; cX <= nEndX; cX++)
            {
                for (int cY = nStartY; cY <= nEndY; cY++)
                {
                    bool cellSuccess = false;
                    MapCellInfo cellInfo = Envir.GetCellInfo(cX, cY, ref cellSuccess);
                    if (cellSuccess && cellInfo.IsAvailable)
                    {
                        for (int k = 0; k < cellInfo.Count; k++)
                        {
                            CellObject cellObject = cellInfo.ObjList[k];
                            if ((cellObject != null) && (cellObject.CellType == CellType.Play || cellObject.CellType == CellType.Monster))
                            {
                                BaseObject baseObject = M2Share.ActorMgr.Get(cellObject.CellObjId);
                                if ((baseObject != null) && (!baseObject.Ghost))
                                {
                                    if (IsProperFriend(baseObject))
                                    {
                                        if (btState == 0)
                                        {
                                            baseObject.DefenceUp(nSec);
                                        }
                                        else
                                        {
                                            baseObject.MagDefenceUp(nSec);
                                        }
                                        result++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private bool DefenceUp(ushort nSec)
        {
            bool result = false;
            if (StatusTimeArr[PoisonState.DefenceUP] > 0)
            {
                if (StatusTimeArr[PoisonState.DefenceUP] < nSec)
                {
                    StatusTimeArr[PoisonState.DefenceUP] = nSec;
                    result = true;
                }
            }
            else
            {
                StatusTimeArr[PoisonState.DefenceUP] = nSec;
                result = true;
            }
            StatusArrTick[PoisonState.DefenceUP] = HUtil32.GetTickCount();
            SysMsg(Format(Settings.DefenceUpTime, nSec), MsgColor.Green, MsgType.Hint);
            RecalcAbilitys();
            SendMsg(this, Messages.RM_ABILITY, 0, 0, 0, 0, "");
            return result;
        }

        public bool AttPowerUp(int nPower, int nTime)
        {
            ((PlayObject)this).ExtraAbil[0] = (ushort)nPower;
            ((PlayObject)this).ExtraAbilTimes[0] = HUtil32.GetTickCount() + nTime * 1000;
            SysMsg(Format(Settings.AttPowerUpTime, nTime / 60, nTime % 60), MsgColor.Green, MsgType.Hint);
            RecalcAbilitys();
            SendMsg(this, Messages.RM_ABILITY, 0, 0, 0, 0, "");
            return true;
        }

        private bool MagDefenceUp(ushort nSec)
        {
            bool result = false;
            if (StatusTimeArr[PoisonState.MagDefenceUP] > 0)
            {
                if (StatusTimeArr[PoisonState.MagDefenceUP] < nSec)
                {
                    StatusTimeArr[PoisonState.MagDefenceUP] = nSec;
                    result = true;
                }
            }
            else
            {
                StatusTimeArr[PoisonState.MagDefenceUP] = nSec;
                result = true;
            }
            StatusArrTick[PoisonState.MagDefenceUP] = HUtil32.GetTickCount();
            SysMsg(Format(Settings.MagDefenceUpTime, nSec), MsgColor.Green, MsgType.Hint);
            RecalcAbilitys();
            SendMsg(this, Messages.RM_ABILITY, 0, 0, 0, 0, "");
            return result;
        }

        public UserItem CheckItemCount(string sItemName, ref int nCount)
        {
            UserItem result = null;
            nCount = 0;
            for (int i = 0; i < UseItems.Length; i++)
            {
                if (UseItems[i] == null)
                {
                    continue;
                }
                string sName = M2Share.WorldEngine.GetStdItemName(UseItems[i].Index);
                if (string.Compare(sName, sItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    result = UseItems[i];
                    nCount++;
                }
            }
            return result;
        }

        public UserItem CheckItems(string sItemName)
        {
            for (int i = 0; i < ItemList.Count; i++)
            {
                UserItem userItem = ItemList[i];
                if (userItem == null)
                {
                    continue;
                }
                if (string.Compare(M2Share.WorldEngine.GetStdItemName(userItem.Index), sItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return userItem;
                }
            }
            return null;
        }

        protected void DelBagItem(int nIndex)
        {
            if ((nIndex < 0) || (nIndex >= ItemList.Count))
            {
                return;
            }
            Dispose(ItemList[nIndex]);
            ItemList.RemoveAt(nIndex);
        }

        public bool DelBagItem(int nItemIndex, string sItemName)
        {
            bool result = false;
            for (int i = 0; i < ItemList.Count; i++)
            {
                UserItem userItem = ItemList[i];
                if ((userItem.MakeIndex == nItemIndex) &&
                    string.Compare(M2Share.WorldEngine.GetStdItemName(userItem.Index), sItemName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Dispose(userItem);
                    ItemList.RemoveAt(i);
                    result = true;
                    break;
                }
            }
            if (result)
            {
                WeightChanged();
            }
            return result;
        }

        public bool CanMove(short nX, short nY, bool boFlag)
        {
            if (Math.Abs(CurrX - nX) <= 1 && Math.Abs(CurrX - nY) <= 1)
            {
                return Envir.CanWalkEx(nX, nY, boFlag);
            }
            return CanRun(nX, nY, boFlag);
        }

        public bool CanMove(short nCurrX, short nCurrY, short nX, short nY, bool boFlag)
        {
            if ((Math.Abs(nCurrX - nX) <= 1) && (Math.Abs(nCurrY - nY) <= 1))
            {
                return Envir.CanWalkEx(nX, nY, boFlag);
            }
            else
            {
                return CanRun(nCurrX, nCurrY, nX, nY, boFlag);
            }
        }

        private bool AdminCanRun()
        {
            if (Race == ActorRace.Play)
            {
                return ((((PlayObject)this).Permission > 9) && M2Share.Config.boGMRunAll);
            }
            return false;
        }

        public bool CanRun(short nCurrX, short nCurrY, short nX, short nY, bool boFlag)
        {
            byte btDir = M2Share.GetNextDirection(nCurrX, nCurrY, nX, nY);
            bool canWalk = (M2Share.Config.DiableHumanRun || AdminCanRun()) || (M2Share.Config.boSafeAreaLimited && InSafeZone());
            switch (btDir)
            {
                case Grobal2.DR_UP:
                    if (nCurrY > 1)
                    {
                        if ((Envir.CanWalkEx(nCurrX, nCurrY - 1, canWalk)) && (Envir.CanWalkEx(nCurrX, nCurrY - 2, canWalk)))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_UPRIGHT:
                    if (nCurrX < Envir.Width - 2 && nCurrY > 1)
                    {
                        if ((Envir.CanWalkEx(nCurrX + 1, nCurrY - 1, canWalk)) && (Envir.CanWalkEx(nCurrX + 2, nCurrY - 2, canWalk)))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_RIGHT:
                    if (nCurrX < Envir.Width - 2)
                    {
                        if (Envir.CanWalkEx(nCurrX + 1, nCurrY, canWalk) && (Envir.CanWalkEx(nCurrX + 2, nCurrY, canWalk)))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_DOWNRIGHT:
                    if ((nCurrX < Envir.Width - 2) && (nCurrY < Envir.Height - 2) && (Envir.CanWalkEx(nCurrX + 1, nCurrY + 1, canWalk) && (Envir.CanWalkEx(nCurrX + 2, nCurrY + 2, canWalk))))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_DOWN:
                    if ((nCurrY < Envir.Height - 2) && (Envir.CanWalkEx(nCurrX, nCurrY + 1, canWalk && (Envir.CanWalkEx(nCurrX, nCurrY + 2, canWalk)))))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_DOWNLEFT:
                    if ((nCurrX > 1) && (nCurrY < Envir.Height - 2) && (Envir.CanWalkEx(nCurrX - 1, nCurrY + 1, canWalk)) && (Envir.CanWalkEx(nCurrX - 2, nCurrY + 2, canWalk)))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_LEFT:
                    if ((nCurrX > 1) && (Envir.CanWalkEx(nCurrX - 1, nCurrY, canWalk)) && (Envir.CanWalkEx(nCurrX - 2, nCurrY, canWalk)))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_UPLEFT:
                    if ((nCurrX > 1) && (nCurrY > 1) && (Envir.CanWalkEx(nCurrX - 1, nCurrY - 1, canWalk)) && (Envir.CanWalkEx(nCurrX - 2, nCurrY - 2, canWalk)))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private bool CanRun(short nX, short nY, bool boFlag)
        {
            byte btDir = M2Share.GetNextDirection(CurrX, CurrY, nX, nY);
            bool canWalk = (M2Share.Config.DiableHumanRun || AdminCanRun()) || (M2Share.Config.boSafeAreaLimited && InSafeZone());
            switch (btDir)
            {
                case Grobal2.DR_UP:
                    if (CurrY > 1)
                    {
                        if ((Envir.CanWalkEx(CurrX, CurrY - 1, canWalk)) && (Envir.CanWalkEx(CurrX, CurrY - 2, canWalk)))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_UPRIGHT:
                    if (CurrX < Envir.Width - 2 && CurrY > 1)
                    {
                        if ((Envir.CanWalkEx(CurrX + 1, CurrY - 1, canWalk)) && (Envir.CanWalkEx(CurrX + 2, CurrY - 2, canWalk)))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_RIGHT:
                    if (CurrX < Envir.Width - 2)
                    {
                        if (Envir.CanWalkEx(CurrX + 1, CurrY, canWalk && (Envir.CanWalkEx(CurrX + 2, CurrY, canWalk))))
                        {
                            return true;
                        }
                    }
                    break;
                case Grobal2.DR_DOWNRIGHT:
                    if ((CurrX < Envir.Width - 2) && (CurrY < Envir.Height - 2) && (Envir.CanWalkEx(CurrX + 1, CurrY + 1, canWalk) && (Envir.CanWalkEx(CurrX + 2, CurrY + 2, canWalk))))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_DOWN:
                    if ((CurrY < Envir.Height - 2)
                        && (Envir.CanWalkEx(CurrX, CurrY + 1, canWalk) && (Envir.CanWalkEx(CurrX, CurrY + 2, canWalk))))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_DOWNLEFT:
                    if ((CurrX > 1) && (CurrY < Envir.Height - 2) && (Envir.CanWalkEx(CurrX - 1, CurrY + 1, canWalk)) && (Envir.CanWalkEx(CurrX - 2, CurrY + 2, canWalk)))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_LEFT:
                    if ((CurrX > 1) && (Envir.CanWalkEx(CurrX - 1, CurrY, canWalk)) && (Envir.CanWalkEx(CurrX - 2, CurrY, canWalk)))
                    {
                        return true;
                    }
                    break;
                case Grobal2.DR_UPLEFT:
                    if ((CurrX > 1) && (CurrY > 1) && (Envir.CanWalkEx(CurrX - 1, CurrY - 1, canWalk)) && (Envir.CanWalkEx(CurrX - 2, CurrY - 2, canWalk)))
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public BaseObject GetMaster()
        {
            if (Race != ActorRace.Play)
            {
                BaseObject masterObject = Master;
                if (masterObject != null)
                {
                    while (true)
                    {
                        if (masterObject.Master != null)
                        {
                            masterObject = masterObject.Master;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return masterObject;
            }
            return null;
        }

        public bool ReAliveEx(MonGenInfo monGen)
        {
            WAbil = Abil;
            Gold = 0;
            NoItem = false;
            StoneMode = false;
            Skeleton = false;
            HolySeize = false;
            CrazyMode = false;
            ShowHp = false;
            FixedHideMode = false;

            if (this is CastleDoor)
            {
                ((CastleDoor)this).IsOpened = false;
                StickMode = true;
            }

            if (this is MagicMonster)
            {
                ((MagicMonster)this).DupMode = false;
            }

            if (this is MagicMonObject)
            {
                ((MagicMonObject)this).UseMagic = false;
            }

            if (this is RockManObject)
            {
                HideMode = false;
            }

            if (this is WallStructure)
            {
                ((WallStructure)this).SetMapFlaged = false;
            }

            if (this is SoccerBall)
            {
                ((SoccerBall)this).N550 = 0;
                ((SoccerBall)this).TargetX = -1;
            }

            if (this is FrostTiger)
            {
                //((TFrostTiger)(this)).m_boApproach = false;
            }

            if (this is CowKingMonster)
            {
                /*((TCowKingMonster)(this)).m_boCowKingMon = true;
                ((TCowKingMonster)(this)).m_nDangerLevel = 0;
                ((TCowKingMonster)(this)).m_boDanger = false;
                ((TCowKingMonster)(this)).m_boCrazy = false;*/
            }

            if (this is DigOutZombi)
            {
                FixedHideMode = true;
            }

            if (this is WhiteSkeleton)
            {
                ((WhiteSkeleton)this).BoIsFirst = true;
                FixedHideMode = true;
            }

            if (this is ScultureMonster)
            {
                FixedHideMode = true;
            }

            if (this is ScultureKingMonster)
            {
                StoneMode = true;
                CharStatusEx = PoisonState.STONEMODE;
            }

            if (this is ElfMonster)
            {
                FixedHideMode = true;
                NoAttackMode = true;
                ((ElfMonster)this).BoIsFirst = true;
            }

            if (this is ElfWarriorMonster)
            {
                FixedHideMode = true;
                ((ElfWarriorMonster)this).BoIsFirst = true;
                ((ElfWarriorMonster)this).UsePoison = false;
            }

            if (this is ElectronicScolpionMon)
            {
                ((ElectronicScolpionMon)this).UseMagic = false;
                //((TElectronicScolpionMon)(this)).m_boApproach = false;
            }

            if (this is DoubleCriticalMonster)
            {
                //((TDoubleCriticalMonster)(this)).m_n7A0 = 0;
            }

            if (this is StickMonster)
            {
                SearchTick = HUtil32.GetTickCount();
                FixedHideMode = true;
                StickMode = true;
            }

            MeatQuality = (ushort)(M2Share.RandomNumber.Random(3500) + 3000);
            //m_nBodyLeathery = m_nPerBodyLeathery;
            //m_nPushedCount = 0;
            //m_nBodyState = 0;

            switch (Race)
            {
                case 51:
                    MeatQuality = (ushort)(M2Share.RandomNumber.Random(3500) + 3000);
                    BodyLeathery = 50;
                    break;
                case 52:
                    if (M2Share.RandomNumber.Random(30) == 0)
                    {
                        MeatQuality = (ushort)(M2Share.RandomNumber.Random(20000) + 10000);
                        BodyLeathery = 150;
                    }
                    else
                    {
                        MeatQuality = (ushort)(M2Share.RandomNumber.Random(8000) + 8000);
                        BodyLeathery = 150;
                    }

                    break;
                case 53:
                    MeatQuality = (ushort)(M2Share.RandomNumber.Random(8000) + 8000);
                    BodyLeathery = 150;
                    break;
                case 54:
                    Animal = true;
                    break;
                case 95:
                    if (M2Share.RandomNumber.Random(2) == 0)
                    {
                        // m_boSafeWalk = true;
                    }

                    break;
                case 96:
                    if (M2Share.RandomNumber.Random(4) == 0)
                    {
                        // m_boSafeWalk = true;
                    }

                    break;
                case 97:
                    if (M2Share.RandomNumber.Random(2) == 0)
                    {
                        // m_boSafeWalk = true;
                    }

                    break;
                case 169:
                    StickMode = false;
                    break;
                case 170:
                    StickMode = true;
                    break;
            }

            UseItems = new UserItem[13];
            for (int i = 0; i < ItemList.Count; i++)
            {
                ItemList[i] = null;
            }
            ItemList.Clear();

            OnEnvirnomentChanged();
            CharStatus = GetCharStatus();
            StatusChanged();
            if (Envir == null)
            {
                return false;
            }

            int nX = monGen.X - monGen.Range + M2Share.RandomNumber.Random(monGen.Range * 2 + 1);
            int nY = monGen.Y - monGen.Range + M2Share.RandomNumber.Random(monGen.Range * 2 + 1);
            bool mBoErrorOnInit = true;
            if (Envir.CanWalk(nX, nY, true))
            {
                CurrX = (short)nX;
                CurrY = (short)nY;
                if (AddToMap())
                {
                    mBoErrorOnInit = false;
                }
            }

            int nRange = 0;
            int nRange2 = 0;
            if (mBoErrorOnInit)
            {
                if (Envir.Width < 50)
                {
                    nRange = 2;
                }
                else
                {
                    nRange = 3;
                }

                if (Envir.Height < 250)
                {
                    if (Envir.Height < 30)
                    {
                        nRange2 = 2;
                    }
                    else
                    {
                        nRange2 = 20;
                    }
                }
                else
                {
                    nRange2 = 50;
                }
            }

            int nC = 0;
            object addObj = null;
            short nX2 = CurrX;
            short nY2 = CurrY;
            while (true)
            {
                if (!Envir.CanWalk(nX, nY, false))
                {
                    if ((Envir.Width - nRange2 - 1) > nX)
                    {
                        nX = nX + nRange;
                    }
                    else
                    {
                        nX = M2Share.RandomNumber.Random(Envir.Width / 2) + nRange2;
                    }

                    if (Envir.Height - nRange2 - 1 > nY)
                    {
                        nY = nY + nRange;
                    }
                    else
                    {
                        nY = M2Share.RandomNumber.Random(Envir.Height / 2) + nRange2;
                    }
                }
                else
                {
                    CurrX = (short)nX;
                    CurrY = (short)nY;
                    addObj = Envir.AddToMap(nX, nY, MapCell, this);
                    break;
                }
                nC++;
                if (nC > 46)
                {
                    break;
                }
            }
            if (addObj == null)
            {
                CurrX = nX2;
                CurrY = nY2;
                Envir.AddToMap(CurrX, CurrY, MapCell, this);
            }
            Abil.HP = Abil.MaxHP;
            Abil.MP = Abil.MaxMP;
            WAbil.HP = WAbil.MaxHP;
            WAbil.MP = WAbil.MaxMP;
            RecalcAbilitys();
            Death = false;
            Invisible = false;
            SendRefMsg(Messages.RM_TURN, Direction, CurrX, CurrY, GetFeatureToLong(), "");
            MonsterSayMsg(null, MonStatus.MonGen);
            return true;
        }

        public void OnEnvirnomentChanged()
        {
            if (CanReAlive)
            {
                if ((MonGen != null) && (MonGen.Envir != Envir))
                {
                    CanReAlive = false;
                    if (MonGen.ActiveCount > 0)
                    {
                        MonGen.ActiveCount--;
                    }
                    MonGen = null;
                }
            }
            //if ((m_PEnvir != null))
            //{
            //    if (m_nLastMapSecret != m_PEnvir.Flag.nSecret)
            //    {
            //        if (m_btRaceServer == ActorRace.Play)
            //        {
            //            if ((m_btRaceServer = ActorRace.Play) && (m_nLastMapSecret != -1))
            //            {
            //                var i = GetFeatureToLong();
            //                var sSENDMSG = string.Empty;
            //                var nSafeX = GetTitleIndex();
            //                if (nSafeX > 0)
            //                {
            //                    var MessageBodyW = new TMessageBodyW();
            //                    MessageBodyW.Param1 = HUtil32.MakeWord(nSafeX, 0);
            //                    MessageBodyW.Param2 = 0;
            //                    MessageBodyW.Tag1 = 0;
            //                    MessageBodyW.Tag2 = 0;
            //                    sSENDMSG = EDcode.EncodeBuffer(@MessageBodyW);
            //                }
            //                ((TPlayObject)(this)).m_DefMsg = Grobal2.MakeDefaultMsg(Messages.SM_FEATURECHANGED, this.ObjectId, HUtil32.LoWord(i), HUtil32.HiWord(i), GetFeatureEx());
            //                ((TPlayObject)(this)).SendSocket(((TPlayObject)(this)).m_DefMsg, sSENDMSG);
            //                ((TPlayObject)(this)).protectedPowerPointChanged();
            //                SendUpdateMsg(this, Messages.RM_USERNAME, 0, 0, 0, 0, GetShowName());
            //            }
            //            HealthSpellChanged();
            //        }
            //        m_nLastMapSecret = m_PEnvir.Flag.nSecret;
            //    }
            //}
            //m_nCurEnvirIdx = -1;
            //m_nCastleEnvirListIdx = -1;
            //m_CurSafeZoneList.Clear();
            //for (int i = 0; i < M2Share.StartPointList.Count; i++)
            //{
            //    var StartPointInfo = M2Share.StartPointList[i];
            //    if (StartPointInfo.m_sMapName == m_PEnvir.sMapName)
            //    {
            //        m_CurSafeZoneList.Add(StartPointInfo);
            //    }
            //}
            //if ((m_btRaceServer == ActorRace.Play) && !((TPlayObject)(this)).m_boOffLineFlag)
            //{
            //   ((TPlayObject)(this)).CheckMapEvent(5, "");
            //}
        }

        protected static void Dispose(object obj)
        {
            obj = null;
        }

        protected static string Format(string str, params object[] par)
        {
            return string.Format(str, par);
        }
    }
}