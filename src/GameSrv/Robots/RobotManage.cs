﻿using NLog;
using SystemModule.Common;

namespace GameSrv.Robots
{
    public class RobotManage
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IList<RobotObject> RobotHumanList;
        private readonly Thread _roBotThread;

        public RobotManage()
        {
            RobotHumanList = new List<RobotObject>();
            LoadRobot();
            _roBotThread = new Thread(Run) { IsBackground = true };
        }

        ~RobotManage()
        {
            UnLoadRobot();
        }

        public void Start()
        {
            _roBotThread.Start();
            _logger.Info("脚本机器人线程启动...");
        }

        public void Stop()
        {
            _roBotThread.Interrupt();
            _logger.Info("脚本机器人线程停止...");
        }

        private void LoadRobot()
        {
            var sRobotName = string.Empty;
            var sScriptFileName = string.Empty;
            var sFileName = M2Share.GetEnvirFilePath("Robot.txt");
            if (!File.Exists(sFileName)) return;
            using var LoadList = new StringList();
            LoadList.LoadFromFile(sFileName);
            for (var i = 0; i < LoadList.Count; i++)
            {
                var sLineText = LoadList[i];
                if (string.IsNullOrEmpty(sLineText) || sLineText[0] == ';') continue;
                sLineText = HUtil32.GetValidStr3(sLineText, ref sRobotName, new[] { ' ', '/', '\t' });
                sLineText = HUtil32.GetValidStr3(sLineText, ref sScriptFileName, new[] { ' ', '/', '\t' });
                if (string.IsNullOrEmpty(sRobotName) || string.IsNullOrEmpty(sScriptFileName)) continue;
                var robotHuman = new RobotObject();
                robotHuman.ChrName = sRobotName;
                robotHuman.ScriptFileName = sScriptFileName;
                robotHuman.LoadScript();
                RobotHumanList.Add(robotHuman);
            }
        }

        public void ReLoadRobot()
        {
            UnLoadRobot();
            LoadRobot();
        }

        private void Run()
        {
            const string sExceptionMsg = "[Exception] TRobotManage::Run";
            while (M2Share.StartReady)
            {
                try
                {
                    for (var i = RobotHumanList.Count - 1; i >= 0; i--)
                    {
                        RobotHumanList[i].Run();
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(sExceptionMsg);
                    _logger.Error(e.Message);
                }
                Thread.Sleep(20);
            }
        }

        private void UnLoadRobot()
        {
            for (var i = 0; i < RobotHumanList.Count; i++)
            {
                RobotHumanList[i] = null;
            }
            RobotHumanList.Clear();
        }
    }
}