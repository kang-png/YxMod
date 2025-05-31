using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




public class UI_Windows
{
    public static Texture2D WindowsTexture = new Texture2D(1, 1);//Box窗体 背景色
    private Texture2D yincangTexture = new Texture2D(1, 1);
    public static Texture2D gongnengquTexture = new Texture2D(1, 1);
    public static Texture2D gongnengquTexture2 = new Texture2D(1, 1);
    public static int biaotiUiheight = 35;
    public static int biaotiUiweith;
    public static int biankuangsize = 5;
    private bool isDragging = false; // 添加一个标志来跟踪是否正在拖动窗口
    private Vector2 dragStartMousePosition; // 存储鼠标按下时的位置

    private Rect rect;
    private string biaoti;

    // 带参数的构造函数
    public UI_Windows(string biaoti = "标题", Rect rect = new Rect())
    {
        this.biaoti = biaoti;
        this.rect = rect;
        //Debug.Log(this.biaoti);
        //Debug.Log(this.rect.width);
        WindowsTexture.SetPixel(0, 0, new Color32(50, 50, 50, 250));//UI背景色
        WindowsTexture.Apply();

        yincangTexture.SetPixel(0, 0, new Color32(150, 150, 150, 255));//关闭
        yincangTexture.Apply();

        gongnengquTexture.SetPixel(0, 0, new Color32(90, 90, 90, 255));//功能区
        gongnengquTexture.Apply();
        gongnengquTexture2.SetPixel(0, 0, new Color32(110, 110, 110, 255));//功能区
        gongnengquTexture2.Apply();
    }
    public void CreatWindowsUi(UiState state)//创建菜单界面
    {
        if (state == UiState.CaiDan)
        {
            rect.height = UI_CaiDan.gaodu + biaotiUiheight + biankuangsize;
            //Debug.Log("UI_CaiDan.gaodu : " + UI_CaiDan.gaodu);
        }
        //else if(state==UiState.ChuanSong)
        //{
        //    rect.yMin = UI_Main.caidanUiRect.yMin + UI_CaiDan.chuansongzhiTop + biaotiUiheight ;
        //}
        //else if(state == UiState.XuanFu)
        //{
        //    rect.yMin = UI_Main.caidanUiRect.yMin + UI_CaiDan.xuanfuyuTop + biaotiUiheight;
        //}
        UI.CreatUiBox(rect, WindowsTexture);
        GUILayout.BeginArea(rect);

        CreatBiaoTiLanUI(state);
        CreatKeHuQuUIs(state);

        GUILayout.EndArea();

        TuoDong();
    }

    private void CreatBiaoTiLanUI(UiState state)//创建界面标题栏
    {
        biaotiUiweith = (int)rect.width;
        Rect biaotiLanRect = new Rect(0, 0, biaotiUiweith, biaotiUiheight);
        GUILayout.BeginArea(biaotiLanRect);

        GUILayout.BeginHorizontal();
        GUILayout.Space(10);
        CreatBiaoTiUI();
        GUILayout.FlexibleSpace();

        CreatGuanBiAnNiuUIs(state);

        GUILayout.Space(5);
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void CreatKeHuQuUIs(UiState state)//创建界面功能区
    {
        Rect kehuquRect = new Rect(biankuangsize, biaotiUiheight, rect.width - biankuangsize * 2, rect.height - biaotiUiheight - biankuangsize);

        UI.CreatUiBox(kehuquRect, gongnengquTexture);
        GUILayout.BeginArea(kehuquRect);

        switch (state)
        {
            case UiState.CaiDan:
                UI_CaiDan.CreatUI();
                break;
            case UiState.GongNengKaiGuan:
                UI_GongNeng.CreatUI();
                break;
            case UiState.WanJiaKongZhi:
                UI_WanJia.CreatUI();
                break;
            case UiState.WuTiKongZhi:
                UI_WuTi.CreatUI();
                break;
            case UiState.SheZhi:
                UI_SheZhi.CreatUI();
                break;
            case UiState.ChuanSong:
                UI_ChuanSong.CreatUI();
                break;
            case UiState.XuanFu:
                UI_XuanFu.CreatUI();
                break;
            case UiState.QianShou:
                UI_QianShou.CreatUI();
                break;
        }

        GUILayout.EndArea();
    }
    private void CreatBiaoTiUI()//创建界面标题
    {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"<b><size=16>{biaoti}</size></b>");
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }
    private void CreatGuanBiAnNiuUIs(UiState state)//隐藏按钮
    {
        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.normal.background = yincangTexture; // 按钮的背景纹理
        //styleButton.active.background = anniuTexture2; // 假设按下时的纹理
        styleButton.hover.background = styleButton.normal.background;
        styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
        styleButton.hover.textColor = Color.white;
        styleButton.alignment = TextAnchor.MiddleCenter;
        styleButton.fontSize = 16;

        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("━", styleButton, GUILayout.Width(25)))//✖
        {
            CreatGuanBiAnNiuUI(state);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();

    }
    private void CreatGuanBiAnNiuUI(UiState state)//点击最小化按钮
    {
        switch (state)
        {
            case UiState.CaiDan:
                UI_Main.ShowCaiDanUI = !UI_Main.ShowCaiDanUI;
                break;
            case UiState.GongNengKaiGuan:
                UI_Main.ShowGongNengUI = !UI_Main.ShowGongNengUI;
                break;
            case UiState.WanJiaKongZhi:
                UI_Main.ShowWanJiaUI = !UI_Main.ShowWanJiaUI;
                break;
            case UiState.WuTiKongZhi:
                UI_Main.ShowWuTiUI = !UI_Main.ShowWuTiUI;
                break;
            case UiState.SheZhi:
                UI_Main.ShowSheZhiUI = !UI_Main.ShowSheZhiUI;
                break;
            case UiState.ChuanSong:
                UI_Main.ShowChuanSongUI = !UI_Main.ShowChuanSongUI;
                break;
            case UiState.XuanFu:
                UI_Main.ShowXuanFuUI = !UI_Main.ShowXuanFuUI;
                break;
            case UiState.QianShou:
                UI_Main.ShowQianShouUI = !UI_Main.ShowQianShouUI;
                break;
        }
    }
    private void TuoDong()//拖动Box窗体
    {
        // 处理窗口拖动
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            isDragging = true;
            dragStartMousePosition = Event.current.mousePosition;
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            isDragging = false; // 当鼠标松开时，重置拖动状态
        }
        else if (isDragging && Event.current.type == EventType.MouseDrag)
        {
            rect.position += Event.current.mousePosition - dragStartMousePosition; // 更新窗口位置
            dragStartMousePosition = Event.current.mousePosition; // 更新拖动起始位置
            Event.current.Use();
        }
    }
    private Rect ClampRect(Rect rectToClamp, Rect bounds)//Box窗体出界检测
    {
        return new Rect(
            Mathf.Clamp(rectToClamp.xMin, bounds.xMin, bounds.xMax - rectToClamp.width),
            Mathf.Clamp(rectToClamp.yMin, bounds.yMin, bounds.yMax - rectToClamp.height),
            rectToClamp.width,
            rectToClamp.height
        );
    }

}
