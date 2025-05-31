using HumanAPI;
using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Voronoi2;



public class UI_Main : MonoBehaviour
{
    public static Texture2D mainUiBoxTexture = new Texture2D(1, 1);
    public static Texture2D anniuTexture = new Texture2D(1, 1);
    public static Texture2D anniuTexture2 = new Texture2D(1, 1);
    public static Texture2D closeTexture = new Texture2D(1, 1);

    
    public static bool ShowUI = true;
    public static bool ShowCaiDanUI;
    public static bool ShowGongNengUI;
    public static bool ShowWanJiaUI;
    public static bool ShowWuTiUI;
    //public static bool ShowYuLeUI;
    public static bool ShowSheZhiUI;
    public static bool ShowChuanSongUI;
    public static bool ShowXuanFuUI;
    public static bool ShowQianShouUI;

    public static bool ShowCaiDanUI2 = true;
    public static bool ShowGongNengUI2;
    public static bool ShowWanJiaUI2;
    public static bool ShowWuTiUI2;
    public static bool ShowYuLeUI2;
    public static bool ShowSheZhiUI2;
    public static bool ShowChuanSongUI2;
    public static bool ShowXuanFuUI2;
    public static bool ShowQianShouUI2;

    public static bool ShowShuBiao;

    private static int mainUI_kuan = 800;
    private static int mainUI_gao = 35;
    private static int mainUI_left = (Screen.width - mainUI_kuan) / 2;
    private static int mainUI_top = 0;

    private static int zhuangtaiUI_kuan = mainUI_kuan;
    private static int zhuangtaiUI_gao = mainUI_gao;

    private static int mainUI_juli = 20;//主界面下方空白
    private static int zhuangtaiUI_juli = 20;//状态栏下方空白
    private static int chuangtiUI_juli = 5;//窗体之间缝隙

    private static int caidanUI_kuan = 150;
    private static int caidanUI_gao = Screen.height - mainUI_gao - zhuangtaiUI_gao - mainUI_juli - zhuangtaiUI_juli;//550
    private static int caidanUI_left = mainUI_left;
    private static int caidanUI_top = mainUI_top + mainUI_gao + mainUI_juli;

    private static int gongnengUI_kuan = 200;//长度
    private static int gongnengUI_gao = caidanUI_gao / 2 - chuangtiUI_juli;//400;//高度
    private static int gongnengUI_left = mainUI_left + mainUI_kuan - gongnengUI_kuan;//长度
    private static int gongnengUI_top = caidanUI_top;//400;//高度

    public static int wutiUI_kuan = gongnengUI_kuan;
    public static int wutiUI_gao = gongnengUI_gao + chuangtiUI_juli;
    public static int wutiUI_left = gongnengUI_left;
    public static int wutiUI_top = gongnengUI_top + gongnengUI_gao + chuangtiUI_juli;

    public static int wanjiaUI_kuan = mainUI_kuan - caidanUI_kuan - gongnengUI_kuan - chuangtiUI_juli * 2;//400
    public static int wanjiaUI_gao = caidanUI_gao / 2 - chuangtiUI_juli;//350
    public static int wanjiaUI_left = caidanUI_left + caidanUI_kuan + chuangtiUI_juli;//400
    public static int wanjiaUI_top = caidanUI_top;//350

    public static int shezhiUI_kuan = wanjiaUI_kuan;//长度
    public static int shezhiUI_gao = wanjiaUI_gao + chuangtiUI_juli;//高度
    public static int shezhiUI_left = wanjiaUI_left;
    public static int shezhiUI_top = wanjiaUI_top + wanjiaUI_gao + chuangtiUI_juli;

    public static int chuansongUI_kuan = 200;
    public static int chuansongUI_gao =  caidanUI_gao / 2 - chuangtiUI_juli;//400
    public static int chuansongUI_left = caidanUI_left + caidanUI_kuan + chuangtiUI_juli;//caidanUI_left - chuansongUI_kuan - chuangtiUI_juli;
    public static int chuansongUI_top = caidanUI_top;

    public static int xuanfuUI_kuan = chuansongUI_kuan;//200
    public static int xuanfuUI_gao = chuansongUI_gao;//400
    public static int xuanfuUI_left = caidanUI_left + caidanUI_kuan + chuangtiUI_juli; //chuansongUI_left;
    public static int xuanfuUI_top = chuansongUI_top + chuansongUI_gao + chuangtiUI_juli;

    public static int qianshouUI_kuan = chuansongUI_kuan;
    public static int qianshouUI_gao = chuansongUI_gao;//400
    public static int qianshouUI_left = chuansongUI_left + chuansongUI_kuan + chuangtiUI_juli;//caidanUI_left - chuansongUI_kuan - chuangtiUI_juli;
    public static int qianshouUI_top = caidanUI_top;

    public static Rect mainUiRect = new Rect(mainUI_left, mainUI_top, mainUI_kuan, mainUI_gao); // 主窗口尺寸
    public static Rect caidanUiRect = new Rect(caidanUI_left, caidanUI_top, caidanUI_kuan, caidanUI_gao);
    public static Rect gongnengUiRect = new Rect(gongnengUI_left, gongnengUI_top, gongnengUI_kuan, gongnengUI_gao);

    public static Rect wanjiaUiRect = new Rect(wanjiaUI_left, wanjiaUI_top, wanjiaUI_kuan, wanjiaUI_gao);
    public static Rect shezhiUiRect = new Rect(shezhiUI_left, shezhiUI_top, shezhiUI_kuan, shezhiUI_gao);

    public static Rect wutiUiRect = new Rect(wutiUI_left, wutiUI_top, wutiUI_kuan, wutiUI_gao);

    public static Rect chuansongUiRect = new Rect(chuansongUI_left, chuansongUI_top, chuansongUI_kuan, chuansongUI_gao);
    public static Rect xuanfuUiRect = new Rect(xuanfuUI_left, xuanfuUI_top, xuanfuUI_kuan, xuanfuUI_gao);
    public static Rect qianshouUiRect = new Rect(qianshouUI_left, qianshouUI_top, qianshouUI_kuan, qianshouUI_gao);

    public UI_Windows caidan = new UI_Windows("菜单(Tab)", caidanUiRect);
    public UI_Windows gongneng = new UI_Windows("全局功能开关(K)", gongnengUiRect);
    public UI_Windows wanjia = new UI_Windows("玩家控制(P)", wanjiaUiRect);
    public UI_Windows wuti = new UI_Windows("物体控制(I)", wutiUiRect);
    public UI_Windows shezhi = new UI_Windows("设置(O)", shezhiUiRect);

    public UI_Windows chuansong = new UI_Windows("传送至(C)", chuansongUiRect);
    public UI_Windows xuanfu = new UI_Windows("悬浮于(X)", xuanfuUiRect);
    public UI_Windows qianshou = new UI_Windows("牵手(Z)", qianshouUiRect);
    void Awake()
    {
        // 使用Debug.Log()方法来将文本输出到控制台
        //Debug.Log("Hello, world!");
        Debug.Log("YxMod 加载成功!");

    }

    // 在所有插件全部启动完成后会调用Start()方法，执行顺序在Awake()后面；
    void Start()
    {

        mainUiBoxTexture.SetPixel(0, 0, new Color32(100, 100, 100, 255));//UI背景色
        mainUiBoxTexture.Apply();

        closeTexture.SetPixel(0, 0, new Color32(0xea, 0x84, 0x7b, 255));//关闭
        closeTexture.Apply();
        UI.UI_ChuShiHua();
        UI_SheZhi.ChuShiHua();
        UI_GongNeng.ChuShiHua();
        YanSe.GetHumanHwnd();//获取游戏Hwnd
        
    }

    // 插件启动后会一直循环执行Update()方法，可用于监听事件或判断键盘按键，执行顺序在Start()后面

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Home))
        {
            if (ShowUI)
            {
                ShowCaiDanUI2 = ShowCaiDanUI; ShowCaiDanUI = false;
                ShowGongNengUI2 = ShowGongNengUI; ShowGongNengUI = false;
                ShowWanJiaUI2 = ShowWanJiaUI; ShowWanJiaUI = false;
                ShowWuTiUI2 = ShowWuTiUI; ShowWuTiUI = false;
                ShowSheZhiUI2 = ShowSheZhiUI; ShowSheZhiUI = false;
                ShowChuanSongUI2 = ShowChuanSongUI; ShowChuanSongUI = false;
                ShowXuanFuUI2 = ShowXuanFuUI; ShowXuanFuUI = false;
                ShowQianShouUI2 = ShowQianShouUI; ShowQianShouUI = false;
                ShowShuBiao = false;
            }
            else
            {
                ShowCaiDanUI = ShowCaiDanUI2;
                ShowGongNengUI = ShowGongNengUI2;
                ShowWanJiaUI = ShowWanJiaUI2;
                ShowWuTiUI = ShowWuTiUI2;
                ShowSheZhiUI = ShowSheZhiUI2;
                ShowChuanSongUI = ShowChuanSongUI2;
                ShowXuanFuUI = ShowXuanFuUI2;
                ShowQianShouUI = ShowQianShouUI2;
                ShowShuBiao = true;
            }
            ShowUI = !ShowUI;
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (ShowCaiDanUI || ShowGongNengUI || ShowWanJiaUI || ShowWuTiUI || ShowSheZhiUI || ShowChuanSongUI || ShowXuanFuUI || ShowQianShouUI)
            {
                XianShiCaiDan(false);
            }
            else
            {
                XianShiCaiDan();
            }
            //ShowCaiDanUI = !ShowCaiDanUI;
        }
        if (Input.GetKeyDown(KeyCode.P) && !NetChat.typing)
        {
            ShowWanJiaUI = !ShowWanJiaUI;
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.O) && !NetChat.typing)
        {
            ShowSheZhiUI = !ShowSheZhiUI;
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.I) && !NetChat.typing)
        {
            ShowWuTiUI = !ShowWuTiUI;
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.K) && !NetChat.typing)
        {
            ShowGongNengUI = !ShowGongNengUI;
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.C) && !NetChat.typing)
        {
            ShowChuanSongUI = !ShowChuanSongUI;
            if (ShowChuanSongUI)
            {
                ShowXuanFuUI = ShowQianShouUI = false;
                if (NetGame.isServer)
                {
                    UI_ChuanSong.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_ChuanSong.yuan_humanID = UI_WanJia.GetClientHumanID();
                }
            }
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.X) && !NetChat.typing)
        {
            ShowXuanFuUI = !ShowXuanFuUI;
            if (ShowXuanFuUI)
            {
                ShowChuanSongUI = ShowQianShouUI = false;
                if (NetGame.isServer)
                {
                    UI_XuanFu.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_XuanFu.yuan_humanID = UI_WanJia.GetClientHumanID();
                }
            }
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Z) && !NetChat.typing)
        {
            ShowQianShouUI = !ShowQianShouUI;
            if (ShowQianShouUI)
            {
                ShowChuanSongUI = ShowXuanFuUI = false;
                if (NetGame.isServer)
                {
                    UI_QianShou.yuan_humanID = 1;
                }
                else if (NetGame.isClient)
                {
                    UI_QianShou.yuan_humanID = UI_WanJia.GetClientHumanID();
                }
            }
            if (!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI)
            {
                ShowShuBiao = false;
            }
        }
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && !NetChat.typing)
        {
            //Debug.Log("按了shift");
            if(!ShowCaiDanUI && !ShowGongNengUI && !ShowWanJiaUI && !ShowWuTiUI && !ShowSheZhiUI && !ShowChuanSongUI && !ShowXuanFuUI && !ShowQianShouUI && UI_SheZhi.shift_xianshishubiao)
            {
                //Debug.Log("显示鼠标");
                ShowShuBiao = !ShowShuBiao;
            }
            
        }


        UI_CaiDan.Update();//更新头顶灯光跟随
    }
    public static void XianShiCaiDan(bool xianshi = true)
    {
        ShowUI = true;
        if (xianshi)
        {
            ShowCaiDanUI = true;
            ShowGongNengUI = ShowGongNengUI2;
            ShowWanJiaUI = ShowWanJiaUI2;
            ShowWuTiUI = ShowWuTiUI2;
            //ShowYuLeUI = ShowYuLeUI2;
            ShowSheZhiUI = ShowSheZhiUI2;
            ShowChuanSongUI = ShowChuanSongUI2;
            ShowXuanFuUI = ShowXuanFuUI2;
            ShowQianShouUI = ShowQianShouUI2;
            ShowShuBiao = true;
        }
        else
        {
            ShowCaiDanUI2 = ShowCaiDanUI; ShowCaiDanUI = false;
            ShowGongNengUI2 = ShowGongNengUI; ShowGongNengUI = false;
            ShowWanJiaUI2 = ShowWanJiaUI; ShowWanJiaUI = false;
            ShowWuTiUI2 = ShowWuTiUI; ShowWuTiUI = false;
            //ShowYuLeUI2 = ShowYuLeUI; ShowYuLeUI = false;
            ShowSheZhiUI2 = ShowSheZhiUI; ShowSheZhiUI = false;
            ShowChuanSongUI2 = ShowChuanSongUI; ShowChuanSongUI = false;
            ShowXuanFuUI2 = ShowXuanFuUI; ShowXuanFuUI = false;
            ShowQianShouUI2 = ShowQianShouUI; ShowQianShouUI = false;
            ShowShuBiao = false;
        }
    }

    // 在插件关闭时会调用OnDestroy()方法
    void OnDestroy()
    {
        //Debug.Log("YxMod被卸载!");
    }


    void OnGUI()
    {
        if(!UI_SheZhi.shift_xianshishubiao)
        {
            ShowShuBiao = false;
        }
        
        if (ShowUI)
        {
            mainUi();//创建主界面
        }
        if (ShowCaiDanUI)
        {
            ShowUI = true; ShowShuBiao = true;
            //UI_Windows.biaotiUiweith = caidanUI_kuan;
            caidan.CreatWindowsUi(UiState.CaiDan);
        }

        if (ShowChuanSongUI || ShowXuanFuUI || ShowQianShouUI)
        {
            if (ShowChuanSongUI)
            {
                ShowUI = true; ShowShuBiao = true;
                chuansong.CreatWindowsUi(UiState.ChuanSong);
                //ShowXuanFuUI = false;
            }
            if (ShowXuanFuUI)
            {
                ShowUI = true; ShowShuBiao = true;
                xuanfu.CreatWindowsUi(UiState.XuanFu);
                //ShowChuanSongUI = false;
            }
            if (ShowQianShouUI)
            {
                ShowUI = true; ShowShuBiao = true;
                qianshou.CreatWindowsUi(UiState.QianShou);
                //ShowChuanSongUI = false;
            }
        }

        else
        {
            if (ShowGongNengUI)
            {
                ShowUI = true; ShowShuBiao = true;
                //UI_Windows.biaotiUiweith = gongnengUI_kuan;
                gongneng.CreatWindowsUi(UiState.GongNengKaiGuan);
            }
            if (ShowWanJiaUI)
            {
                ShowUI = true; ShowShuBiao = true;
                //UI_Windows.biaotiUiweith = wanjiaUI_kuan;
                wanjia.CreatWindowsUi(UiState.WanJiaKongZhi);
            }
            if (ShowWuTiUI)
            {
                ShowUI = true; ShowShuBiao = true;
                //UI_Windows.biaotiUiweith = wutiUI_kuan;
                wuti.CreatWindowsUi(UiState.WuTiKongZhi);
            }
            if (ShowSheZhiUI)
            {
                ShowUI = true; ShowShuBiao = true;
                //UI_Windows.biaotiUiweith = shezhiUI_kuan;
                shezhi.CreatWindowsUi(UiState.SheZhi);
            }
        }



    }

    private void mainUi()//创建主界面
    {
        UI.CreatUiBox(mainUiRect, mainUiBoxTexture);
        //mainUiBox();
        GUILayout.BeginArea(mainUiRect);
        GUILayout.BeginHorizontal();
        GUILayout.Space(10);

        mainUiBox_BiaoTi();
        mainUiBox_Tabs();
        mainUiBox_Close();

        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

    }
    //private void mainUiBox()//创建主界面Box
    //{
    //    GUIStyle myGuiStyle = new GUIStyle(GUI.skin.box)
    //    {
    //        normal = { background = mainUiBoxTexture },
    //    };
    //    GUI.Box(mainUiRect, GUIContent.none, myGuiStyle);
    //}
    private void mainUiBox_BiaoTi()//创建主界面标题
    {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"<b><size=16>YxMod <i><color=grey>{YxMod.BanBenHao:0.0}</color></i></size></b>");
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();
    }

    private void mainUiBox_Tabs()//创建主界面按钮集合
    {
        UI.CreatAnNiu_AnXia("菜单(Tab)", ref ShowCaiDanUI);
        UI.CreatAnNiu_AnXia("玩家控制(P)", ref ShowWanJiaUI);
        UI.CreatAnNiu_AnXia("功能开关(K)", ref ShowGongNengUI);
        UI.CreatAnNiu_AnXia("物体控制(I)", ref ShowWuTiUI);
        //UI.CreatAnNiu_AnXia("娱乐模式(L)", ref ShowYuLeUI);
        UI.CreatAnNiu_AnXia("设置(O)", ref ShowSheZhiUI);
    }



    private void mainUiBox_Close()//关闭按钮
    {
        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.normal.background = closeTexture; // 按钮的背景纹理
        //styleButton.active.background = anniuTexture2; // 假设按下时的纹理
        styleButton.hover.background = styleButton.normal.background;
        styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
        styleButton.hover.textColor = Color.white;
        styleButton.alignment = TextAnchor.MiddleCenter;
        styleButton.fontSize = 16;

        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Home", styleButton, GUILayout.Width(55)))
        {
            ShowCaiDanUI2 = ShowCaiDanUI; ShowCaiDanUI = false;
            ShowGongNengUI2 = ShowGongNengUI; ShowGongNengUI = false;
            ShowWanJiaUI2 = ShowWanJiaUI; ShowWanJiaUI = false;
            ShowWuTiUI2 = ShowWuTiUI; ShowWuTiUI = false;
            //ShowYuLeUI2 = ShowYuLeUI; ShowYuLeUI = false;
            ShowSheZhiUI2 = ShowSheZhiUI; ShowSheZhiUI = false;
            ShowChuanSongUI2 = ShowChuanSongUI; ShowChuanSongUI = false;
            ShowXuanFuUI2 = ShowXuanFuUI; ShowXuanFuUI = false;
            ShowQianShouUI2 = ShowQianShouUI; ShowQianShouUI = false;

            ShowUI = false;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

    }
    //void Update()
    //{
    //    // 确保窗口不会超出屏幕范围
    //    //Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
    //    //windowRect2 = GUI.Window(0, windowRect, DrawWindowContent, "", GUIStyle.none, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
    //    //windowRect2 = GUIUtility.ClampRect(windowRect, screenRect);
    //}




}
