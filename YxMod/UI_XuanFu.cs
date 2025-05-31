using Multiplayer;
using UnityEngine;

internal class UI_XuanFu
{
    private static Vector2 wuti_scrollPosition;
    public static int humanID = -1;
    public static int yuan_humanID = 1;
    public static int left = UI_Main.xuanfuUI_left;
    public static int top = UI_Main.xuanfuUI_top;

    public static void CreatUI()//创建菜单功能区
    {
        float kehuquHeight = UI_Main.xuanfuUI_gao - UI_Windows.biaotiUiheight;
        float kehuquWeith = UI_Main.xuanfuUI_kuan;// - UI_Windows.biankuangsize * 2;

        Rect humanlistRect = new Rect(0, 0, kehuquWeith, kehuquHeight);
        UI.CreatUiBox(humanlistRect, UI_Windows.gongnengquTexture);
        GUILayout.BeginArea(humanlistRect);
        UI.CreatAnNiu("取消悬浮", false, QuXiaoXuanFu);
        GUILayout.Space(10);

        wuti_scrollPosition = GUILayout.BeginScrollView(wuti_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

        int i = UI.CreatHumanList(humanID,false);
        if (humanID != i)
        {
            humanID = i;
            XuanFuYu(yuan_humanID-1,humanID);
            humanID = -1;

        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    public static void XuanFuYu(int yuanhuman,int inthuman)//悬浮于 human编号
    {
        if (inthuman == yuanhuman)
        {
            Chat.TiShi(NetGame.instance.local, "不能当自己的头部挂件");
            return;
        }
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.guajianxitong_KaiGuan)
            {
                UI_GongNeng.guajianxitong_KaiGuan = true;
                UI_GongNeng.GuaJianXiTong();
            }
            YxMod.SetGuaJian( Human.all[inthuman], Human.all[yuanhuman]);
        }
        else if (NetGame.isClient && YxMod.YxModServer)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("xfy"),$"{yuanhuman}|{inthuman}");//传送至
        }
        UI_Main.ShowXuanFuUI = false;
        UI_CaiDan.JiXu();
    }
    public static void QuXiaoXuanFu()//取消悬浮
    {
        if (NetGame.isServer)
        {
            //YxMod.QuXiaoGuaJian(NetGame.instance.server.players[0].human);
            YxMod.QuXiaoGuaJian(Human.all[yuan_humanID-1]);
        }
        else if (NetGame.isClient && YxMod.YxModServer)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qxxf"), $"{yuan_humanID - 1}");//取消挂件
        }
        UI_Main.ShowXuanFuUI = false;
        UI_CaiDan.JiXu();
    }
}
