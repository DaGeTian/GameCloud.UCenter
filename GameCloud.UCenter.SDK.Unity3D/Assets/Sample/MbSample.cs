// Copyright(c) Cragon.All rights reserved.

namespace GameCloud.UCenter.SDK.Sample
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using GameCloud.Unity.Common;
    using GameCloud.UCenter.SDK.Unity;

    public class EcSampleListener : IEcEngineListener
    {
        //---------------------------------------------------------------------
        public void init(EntityMgr entity_mgr, Entity et_root)
        {
            entity_mgr.regComponent<ClientSampleApp<DefSampleApp>>();
            entity_mgr.regComponent<ClientUCenterSDK<DefUCenterSDK>>();

            entity_mgr.regEntityDef<EtSampleApp>();
            entity_mgr.regEntityDef<EtUCenterSDK>();
        }

        //---------------------------------------------------------------------
        public void release()
        {
        }
    }

    public class MbSample : MonoBehaviour
    {
        //---------------------------------------------------------------------
        static MbSample mMbMain;
        EcEngine mEngine;

        //---------------------------------------------------------------------
        public List<string> ListInfo { get; set; }

        //---------------------------------------------------------------------
        static public MbSample Instance
        {
            get { return mMbMain; }
        }

        //----------------------------------------------------------------------
        void Awake()
        {
            mMbMain = this;
        }

        //----------------------------------------------------------------------
        void Start()
        {
            // 初始化系统参数
            {
                Application.runInBackground = true;
                Time.fixedDeltaTime = 0.05f;
                Application.targetFrameRate = 30;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }

            // 初始化日志
            {
                EbLog.NoteCallback = Debug.Log;
                EbLog.WarningCallback = Debug.LogWarning;
                EbLog.ErrorCallback = Debug.LogError;
            }

            EbLog.Note("MbSample.Start()");

            ListInfo = new List<string>();

            if (mEngine == null)
            {
                EcEngineSettings settings;
                settings.ProjectName = "EcSample";
                settings.RootEntityType = "EtRoot";
                mEngine = new EcEngine(ref settings, new EcSampleListener());
            }

            // 创建EtSampleApp
            EntityMgr.Instance.createEntity<EtSampleApp>(null, EcEngine.Instance.EtNode);
        }

        //---------------------------------------------------------------------
        void Update()
        {
            if (mEngine != null)
            {
                mEngine.update(Time.deltaTime);
            }

            // 检测是否请求退出
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        //---------------------------------------------------------------------
        void OnDestroy()
        {
            _destory();
        }

        //----------------------------------------------------------------------
        void OnApplicationQuit()
        {
            _destory();
        }

        //----------------------------------------------------------------------
        void OnApplicationFocus(bool focusStatus)
        {
        }

        //----------------------------------------------------------------------
        void OnGUI()
        {
            // 文本显示
            int height = 20;
            int index = 0;
            foreach (var i in ListInfo)
            {
                GUI.Label(new Rect(10, 10 + index * height, 800, 600), i);
                index++;
            }

            // 第一个文字按钮
            //GUI.color = Color.yellow;  //按钮文字颜色  
            //GUI.backgroundColor = Color.red; //按钮背景颜色

            //if (GUI.Button(new Rect(50, 250, 200, 30), "Button1"))
            //{
            //    info = "按下了Button1";
            //}

            //// 第二个图片按钮
            //GUI.color = Color.white;  //按钮文字颜色  
            //GUI.backgroundColor = Color.green; //按钮背景颜色

            //if (GUI.Button(new Rect(50, 300, 128, 64), buttonTexture))
            //{
            //    info = "按下了Button2";
            //}

            //// 持续按下的按钮
            //if (GUI.RepeatButton(new Rect(50, 400, 200, 30), "按钮按下中"))
            //{
            //    info = "按钮按下中的时间：" + repeatTime;
            //    repeatTime++;
            //}
        }

        //----------------------------------------------------------------------
        void _destory()
        {
            if (mEngine == null) return;

            if (mEngine != null)
            {
                mEngine.close();
                mEngine = null;
            }

            Screen.sleepTimeout = SleepTimeout.SystemSetting;

            EbLog.Note("MbSample._destory()");
        }
    }
}
