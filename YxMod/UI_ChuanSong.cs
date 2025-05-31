using Multiplayer;
using UnityEngine;

internal class UI_ChuanSong
{
    private static Vector2 wuti_scrollPosition;
    public static int humanID = -1;
    public static int yuan_humanID =1;
    public static int left = UI_Main.chuansongUI_left;
    public static int top = UI_Main.chuansongUI_top;

    public static void CreatUI()//创建菜单功能区
    {
        float kehuquHeight = UI_Main.chuansongUI_gao - UI_Windows.biaotiUiheight;
        float kehuquWeith = UI_Main.chuansongUI_kuan;// - UI_Windows.biankuangsize * 2;

        Rect humanlistRect = new Rect(0, 0, kehuquWeith, kehuquHeight);
        UI.CreatUiBox(humanlistRect, UI_Windows.gongnengquTexture);
        GUILayout.BeginArea(humanlistRect);
        wuti_scrollPosition = GUILayout.BeginScrollView(wuti_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域
        if (NetGame.isServer || NetGame.isClient)
        {
            int i = UI.CreatHumanList(humanID,false);
            if (humanID != i)
            {
                humanID = i;
                ChuanSongZhi(yuan_humanID-1, humanID);
                humanID = -1;

            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    public static void ChuanSongZhi(int yuanhuman, int inthuman)//传送至 human编号
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.chuansongxitong_KaiGuan)
            {
                UI_GongNeng.chuansongxitong_KaiGuan = true;
                UI_GongNeng.ChuanSongXiTong();
            }
            //Human human1 = NetGame.instance.local.players[0].human;
            YxMod.ChuanSong(Human.all[yuanhuman], Human.all[inthuman]);
        }
        else if (NetGame.isClient && YxMod.YxModServer)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("csz"),$"{yuanhuman}|{inthuman}");//传送至
        }
        UI_Main.ShowChuanSongUI = false;
        UI_CaiDan.JiXu();
    }

}
