using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



internal class UI_WuTi
{
    private static Vector2 wuti_scrollPosition;
    private static Vector2 thing_scrollPosition;


    private static int wutilist_kuan = 100;
    public static int wutiID = 0;

    public static bool qi;
    public static bool luo;


    public static void CreatUI()//创建菜单功能区
    {

        GUILayout.Space(5);
        UI.CreatFenGeXian();//分割线
        GUILayout.Space(5);
        GUILayout.Label($"<i>空空如也,开发中...</i>", UI.SetLabelStyle_JuZhong(), GUILayout.ExpandWidth(true));



        //float kehuquHeight = UI_Main.wutiUI_gao - UI_Windows.biaotiUiheight;
        //float kehuquWeith = UI_Main.wutiUI_kuan - UI_Windows.biankuangsize * 2;

        //Rect wutilist = new Rect(0, 0, wutilist_kuan, kehuquHeight);
        //UI.CreatUiBox(wutilist, UI_Windows.gongnengquTexture2);
        //GUILayout.BeginArea(wutilist);
        //wuti_scrollPosition = GUILayout.BeginScrollView(wuti_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

        //int i = UI.CreatHumanList(wutiID);
        //if (wutiID != i)
        //{
        //    wutiID = i;
        //    Debug.Log(wutiID);
        //}


        //GUILayout.EndScrollView();
        //GUILayout.EndArea();

        //Rect thinglist = new Rect(wutilist_kuan + UI_Windows.biankuangsize, 0, kehuquWeith - wutilist_kuan - UI_Windows.biankuangsize, kehuquHeight);
        //UI.CreatUiBox(thinglist, UI_Windows.gongnengquTexture2);
        //GUILayout.BeginArea(thinglist);
        //thing_scrollPosition = GUILayout.BeginScrollView(thing_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域
        ////GUILayout.BeginHorizontal();
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        //UI.CreatAnNiu_AnXia("起", ref qi, false);
        ////GUILayout.EndHorizontal();

        ////GUILayout.BeginHorizontal();
        //UI.CreatAnNiu_AnXia("落", ref luo, false);
        //UI.CreatAnNiu_AnXia("落", ref luo, false);
        //UI.CreatAnNiu_AnXia("落", ref luo, false);
        //UI.CreatAnNiu_AnXia("落", ref luo, false);
        //UI.CreatAnNiu_AnXia("落", ref luo, false);
        ////GUILayout.EndHorizontal();


        //GUILayout.EndScrollView();
        //GUILayout.EndArea();

    }

}
