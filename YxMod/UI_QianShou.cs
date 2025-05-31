using Multiplayer;
using UnityEngine;

internal class UI_QianShou
{
    private static Vector2 wuti_scrollPosition;
    public static int humanID = -1;
    public static int yuan_humanID = 1;
    public static int left = UI_Main.qianshouUI_left;
    public static int top = UI_Main.qianshouUI_top;

    public static void CreatUI()//创建菜单功能区
    {
        float kehuquHeight = UI_Main.qianshouUI_gao - UI_Windows.biaotiUiheight;
        float kehuquWeith = UI_Main.qianshouUI_kuan;// - UI_Windows.biankuangsize * 2;

        Rect humanlistRect = new Rect(0, 0, kehuquWeith, kehuquHeight);
        UI.CreatUiBox(humanlistRect, UI_Windows.gongnengquTexture);
        GUILayout.BeginArea(humanlistRect);
        UI.CreatAnNiu("取消牵手", false, QuXiaoQianShou);

        GUILayout.Space(10);

        wuti_scrollPosition = GUILayout.BeginScrollView(wuti_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

        int i = UI.CreatHumanList(humanID,false);
        if (humanID != i)
        {
            humanID = i;
            QianShou(yuan_humanID-1,humanID);
            humanID = -1;

        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    public static void QianShou(int yuanhuman,int inthuman)//牵手 human编号
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.qianshouxitong_KaiGuan)
            {
                UI_GongNeng.qianshouxitong_KaiGuan = true;
                UI_GongNeng.QianShouXiTong();
            }
            YxMod.SetQianShou(Human.all[yuanhuman], Human.all[inthuman]);
        }
        else if (NetGame.isClient && YxMod.YxModServer)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qs"),$"{yuanhuman}|{inthuman}");//
        }
        UI_Main.ShowQianShouUI = false;
        UI_CaiDan.JiXu();
    }
    public static void QuXiaoQianShou()//取消
    {
        if (NetGame.isServer)
        {
            YxMod.QuXiaoQianShou(Human.all[yuan_humanID - 1]);
        }
        else if (NetGame.isClient && YxMod.YxModServer)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qxqs"), $"{yuan_humanID-1}");//取消挂件
        }
        UI_Main.ShowQianShouUI = false;
        UI_CaiDan.JiXu();
    }
}
