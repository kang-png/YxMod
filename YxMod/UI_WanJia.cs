using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



internal class UI_WanJia
{
    private static Vector2 wanjia_scrollPosition;
    private static Vector2 thing_scrollPosition;


    private static int wanjialist_kuan = 150;
    public static int humanID = 99;

    public static bool allkejiquanxian = UI_SheZhi.morenkejiquanxian;
    public static bool allliaotiankuangquanxian = true;
    public static bool alldingdian = true;
    public static bool allwujiasi = true;
    public static bool allwupengzhuang;
    public static bool allfeitian;
    public static bool allchaoren;
    public static bool allshanxian;
    public static bool alldongjie;
    public static bool allbengdi;
    public static bool allsanjitiao;
    public static bool alldiantun;
    public static bool allqiqiu;

    public static bool allqiqiuxifa;
    public static bool alldaoli;
    public static bool allzhuanquan;
    public static bool alltuoluo;

    public static bool allketouguai;
    public static bool alldiaosigui;
    public static bool allpangxie;
    public static bool allqianshui;

    public static bool alltuique;
    public static bool alltuiguai;
    public static bool allchaichu;
    public static bool allkongqipao;
    public static bool allbanshen;
    public static bool allchaojitiao;

    public static Human human;

    public static void CreatUI()//创建菜单功能区
    {
        float kehuquHeight = UI_Main.wanjiaUI_gao - UI_Windows.biaotiUiheight;
        float kehuquWeith = UI_Main.wanjiaUI_kuan - UI_Windows.biankuangsize * 2;
        Rect wanjialist = new Rect(0, 0, wanjialist_kuan, kehuquHeight);
        UI.CreatUiBox(wanjialist, UI_Windows.gongnengquTexture2);

        Rect thinglist = new Rect(wanjialist_kuan + UI_Windows.biankuangsize, 0, kehuquWeith - wanjialist_kuan - UI_Windows.biankuangsize, kehuquHeight);
        UI.CreatUiBox(thinglist, UI_Windows.gongnengquTexture2);

        GUILayout.BeginArea(wanjialist);
        wanjia_scrollPosition = GUILayout.BeginScrollView(wanjia_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

        int i = UI.CreatHumanList(humanID);
        humanID = i;

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(thinglist);
        thing_scrollPosition = GUILayout.BeginScrollView(thing_scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域

        if (humanID == 0)
        {
            if (NetGame.isServer)
            {
                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("客机权限", ref allkejiquanxian, false, KeJiQuanXian);
                UI.CreatAnNiu_AnXia("聊天框权限", ref allliaotiankuangquanxian, false, AllLiaoTianKuangQuanXian);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("个人定点", ref alldingdian, false, GeRenDingDian);
                UI.CreatAnNiu_AnXia("无假死", ref allwujiasi, false, WuJiaSi);
                UI.CreatAnNiu_AnXia("无碰撞", ref allwupengzhuang, false, WuPengZhuang);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("飞天", ref allfeitian, false, FeiTian);
                UI.CreatAnNiu_AnXia("超人", ref allchaoren, false, ChaoRen);
                UI.CreatAnNiu_AnXia("闪现", ref allshanxian, false, ShanXian);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("冻结", ref alldongjie, false, DongJie);
                UI.CreatAnNiu_AnXia("半身不遂", ref allbanshen, false, BanShen);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("蹦迪", ref allbengdi, false, BengDi);
                UI.CreatAnNiu_AnXia("三级跳", ref allsanjitiao, false, SanJiTiao);
                UI.CreatAnNiu_AnXia("电臀", ref alldiantun, false, DianTun);
                UI.CreatAnNiu_AnXia("气球", ref allqiqiu, false, QiQiu);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("气球戏法", ref allqiqiuxifa, false, QiQiuXiFa);
                UI.CreatAnNiu_AnXia("倒立", ref alldaoli, false, DaoLi);
                UI.CreatAnNiu_AnXia("转圈圈", ref allzhuanquan, false, ZhuanQuan);
                UI.CreatAnNiu_AnXia("陀螺", ref alltuoluo, false, TuoLuo);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("磕头怪", ref allketouguai, false, KeTouGuai);
                UI.CreatAnNiu_AnXia("吊死鬼", ref alldiaosigui, false, DiaoSiGui);
                UI.CreatAnNiu_AnXia("螃蟹", ref allpangxie, false, PangXie);
                UI.CreatAnNiu_AnXia("潜水", ref allqianshui, false, QianShui);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("腿瘸", ref alltuique, false, TuiQue);
                UI.CreatAnNiu_AnXia("腿拐", ref alltuiguai, false, TuiGuai);
                UI.CreatAnNiu_AnXia("拆除", ref allchaichu, false, ChaiChu);
                UI.CreatAnNiu_AnXia("空气炮", ref allkongqipao, false, KongQiPao);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("超级跳", ref allchaojitiao, false, chaojitiao);
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            human = YxMod.ChaHumanId($"{humanID}");
            if (human == null)
            {
                humanID = 0;
                return;
            }
            GUILayout.BeginHorizontal();
            if ((NetGame.isServer && humanID != 1) || (NetGame.isClient && !KeJiZiJi())) //不是自己
            {
                UI.CreatAnNiu("个人资料", false, GeRenZiLiao);
                UI.CreatAnNiu("加好友", false, JiaHaoYou);
            }
            if ((NetGame.isServer && humanID != 1) || (NetGame.isClient && !KeJiZiJi() && humanID != 1)) //不是自己/房主/
            {
                UI.CreatAnNiu("踢出房间", false, TiChuFangJian);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (NetGame.isServer)
            {
                if (human.isClient && humanID != 1)
                {
                    UI.CreatAnNiu_AnXia("客机权限", ref human.kejiquanxian, false, KeJiQuanXian);
                }
                UI.CreatAnNiu_AnXia("聊天框权限", ref human.liaotiankuangquanxian, false);
            }
            GUILayout.EndHorizontal();

            if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))) //自己
            {
                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("个人定点", ref human.dingdian.kaiguan, false, GeRenDingDian);
                UI.CreatAnNiu_AnXia("无假死", ref human.wujiasi, false, WuJiaSi);
                UI.CreatAnNiu_AnXia("无碰撞", ref human.wupengzhuang, false, WuPengZhuang);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("飞天", ref human.feitian, false, FeiTian);
                UI.CreatAnNiu_AnXia("超人", ref human.chaoren, false, ChaoRen);
                UI.CreatAnNiu_AnXia("闪现", ref human.shanxian, false, ShanXian);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu("传送至>>", false, ChuanSongZhi);
                UI.CreatAnNiu("悬浮于>>", false, XuanFuYu);
                UI.CreatAnNiu("牵手>>", false, QianShou);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("冻结", ref human.dongjie, false, DongJie);
                UI.CreatAnNiu_AnXia("半身不遂", ref human.banshen, false, BanShen);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("蹦迪", ref human.bengdi, false, BengDi);
                UI.CreatAnNiu_AnXia("三级跳", ref human.sanjitiao, false, SanJiTiao);
                UI.CreatAnNiu_AnXia("电臀", ref human.diantun, false, DianTun);
                UI.CreatAnNiu_AnXia("气球", ref human.qiqiu, false, QiQiu);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("气球戏法", ref human.qiqiuxifa, false, QiQiuXiFa);
                UI.CreatAnNiu_AnXia("倒立", ref human.daoli, false, DaoLi);
                UI.CreatAnNiu_AnXia("转圈圈", ref human.zhuanquan, false, ZhuanQuan);
                UI.CreatAnNiu_AnXia("陀螺", ref human.tuoluo, false, TuoLuo);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("磕头怪", ref human.ketouguai, false, KeTouGuai);
                UI.CreatAnNiu_AnXia("吊死鬼", ref human.diaosigui, false, DiaoSiGui);
                UI.CreatAnNiu_AnXia("螃蟹", ref human.pangxie, false, PangXie);
                UI.CreatAnNiu_AnXia("潜水", ref human.qianshui, false, QianShui);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("腿瘸", ref human.tuique, false, TuiQue);
                UI.CreatAnNiu_AnXia("腿拐", ref human.tuiguai, false, TuiGuai);
                UI.CreatAnNiu_AnXia("拆除", ref human.chaichu, false, ChaiChu);
                UI.CreatAnNiu_AnXia("空气炮", ref human.kongqipao, false, KongQiPao);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                UI.CreatAnNiu_AnXia("超级跳", ref human.chaojitiao, false, chaojitiao);
                GUILayout.EndHorizontal();
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
    private static bool KeJiZiJi()
    {
        return humanID == GetClientHumanID();//(int)NetGame.instance.local.hostId;
    }
    public static int GetClientHumanID()
    {
        int id = 0;
        if (NetGame.isClient)
        {
            List<NetHost> players = NetGame.instance.readyclients;//客户机名

            for (int j = 0; j < players.Count; j++)
            {
                if (players[j].hostId == NetGame.instance.local.hostId)
                {
                    id = j + 1 + 1;
                    break;
                }
            }
        }
        return id;
    }
    private static void ChuanSongZhi()
    {
        UI_Main.ShowChuanSongUI = true;
        if (UI_Main.ShowChuanSongUI)
        {
            UI_Main.ShowXuanFuUI = UI_Main.ShowQianShouUI = false;
            UI_Main.chuansongUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 100;
            UI_Main.chuansongUiRect.yMin = UI_Main.wanjiaUiRect.yMin ;
        }

        UI_ChuanSong.yuan_humanID = humanID;
        
    }

    private static void XuanFuYu()//悬浮于
    {

        UI_Main.ShowXuanFuUI = true;
        if (UI_Main.ShowXuanFuUI)
        {
            UI_Main.ShowChuanSongUI = UI_Main.ShowQianShouUI = false;
            UI_Main.xuanfuUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 150;
            UI_Main.xuanfuUiRect.yMin = UI_Main.wanjiaUiRect.yMin;
        }
        UI_XuanFu.yuan_humanID = humanID;
    }
    private static void QianShou()//牵手
    {

        UI_Main.ShowQianShouUI = true;
        if (UI_Main.ShowQianShouUI)
        {
            UI_Main.ShowChuanSongUI = UI_Main.ShowXuanFuUI = false;
            UI_Main.qianshouUiRect.xMin = UI_Main.wanjiaUiRect.xMin + 150;
            UI_Main.qianshouUiRect.yMin = UI_Main.wanjiaUiRect.yMin;
        }
        UI_QianShou.yuan_humanID = humanID;
    }
    private static void GeRenZiLiao()
    {
        ulong ulSteamID = 0UL;
        ulong.TryParse(human.player.skinUserId, out ulSteamID);
        SteamFriends.ActivateGameOverlayToUser("steamid", new CSteamID(ulSteamID));
    }
    private static void JiaHaoYou()
    {
        ulong ulSteamID2 = 0UL;
        ulong.TryParse(human.player.skinUserId, out ulSteamID2);
        SteamFriends.ActivateGameOverlayToUser("friendadd", new CSteamID(ulSteamID2));
    }
    private static void TiChuFangJian()//有效果
    {
        TiChuFangJian(human);
    }
    public static void TiChuFangJian(Human human0)//有效果
    {
        if (NetGame.isServer)
        {
            NetGame.instance.Kick(human0.player.host);
            NetGame.instance.OnDisconnect(human0.player.host.connection, false);
        }
        else if (NetGame.isClient)
        {
            if (YxMod.YxModServer)//如果房主有文件 并且有权限
            {
                if (YxMod.KeJiQuanXian)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("kick"), $"{humanID-1}");
                }
            }
            else
            {
                //客机强踢人
                //Debug.Log("开始尝试强踢");
                ulong result3 = 0uL;
                if (ulong.TryParse(human0.player.skinUserId, out result3))
                {
                    NetHost host = new NetHost(new CSteamID(result3), string.Empty);
                    for (uint num3 = 0u; num3 < 12; num3++)
                    {
                        NetStream netStream = NetGame.BeginMessage(NetMsgId.RemoveHost);
                        try
                        {
                            netStream.WriteNetId(num3);
                            NetGame.instance.SendReliable(host, netStream);
                        }
                        finally
                        {
                            if (netStream != null)
                            {
                                netStream = netStream.Release();
                            }
                        }
                    }
                    Chat.TiShi(NetGame.instance.local, $"尝试强踢玩家 {human0.player.host.name}");
                }
                else
                {
                    //Debug.Log($"强踢失败! {human.player.host.name} skinUserId = {human.player.skinUserId}");
                }
            }
        }
    }
    private static void KeJiQuanXian()//客机权限
    {
        if (NetGame.isServer)
        {
            if(humanID ==0)
            {
                for(int i = 0; i < Human.all.Count;i++ )
                {
                    if (Human.all[i].isClient)
                    {
                        Human.all[i].kejiquanxian = allkejiquanxian ;
                        Chat.SendYxModMsgServer(Human.all[i].player.host, Chat.YxModMsgStr("kejiquanxian"), human.kejiquanxian ? "1" : "0");
                    }

                    Chat.TiShi($"所有客机 的 客机权限 已 {(allkejiquanxian ? "打开" : "关闭")}");
                }
            }
            else
            {
                Chat.SendYxModMsgServer(human.player.host, Chat.YxModMsgStr("kejiquanxian"), human.kejiquanxian ? "1" : "0");

            }
        }


    }

    private static void AllLiaoTianKuangQuanXian()
    {
        if (NetGame.isServer)
        {
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].liaotiankuangquanxian = allliaotiankuangquanxian ;
                }
                Chat.TiShi($"所有玩家 的 聊天框权限 已 {(allliaotiankuangquanxian ? "打开" : "关闭")}");
            }
        }
    }


    private static void GeRenDingDian()//个人定点
    {

        if (NetGame.isServer)
        {
            if(!UI_GongNeng.dingdian_KaiGuan)
            {

                UI_GongNeng.dingdian_KaiGuan = true;
                UI_GongNeng. DingDian();
            }

            if(humanID==0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].dingdian.kaiguan = alldingdian ;
                }
                Chat.TiShi($"所有玩家 的 个人定点 已 {(alldingdian ? "打开" : "关闭")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.dingdian.kaiguan ? "打开" : "关闭")}了 {human.player.host.name} 的个人定点");
            }
            
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("gerendingdian"), $"{humanID-1}");
        }
    }
    private static void WuJiaSi()//无假死
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.wujiasi_KaiGuan)
            {
                UI_GongNeng.wujiasi_KaiGuan = true;
                UI_GongNeng.WuJiaSi();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].wujiasi = allwujiasi;
                }
                Chat.TiShi($"所有玩家 的 无假死 已 {(allwujiasi ? "打开" : "关闭")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.wujiasi ? "打开" : "关闭")}了 {human.player.host.name} 的无假死");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("wujiasi"), $"{humanID - 1}");
        }
    }
    private static void WuPengZhuang()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.wupengzhuang_KaiGuan)
            {
                UI_GongNeng.wupengzhuang_KaiGuan = true;
                UI_GongNeng.WuPengZhuang();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].wupengzhuang = allwupengzhuang;
                }
                Chat.TiShi($"所有玩家 的 无碰撞 已 {(allwupengzhuang ? "打开" : "关闭")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.wupengzhuang ? "打开" : "关闭")}了 {human.player.host.name} 的无碰撞");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("wupengzhuang"), $"{humanID - 1}");
        }
    }
    public static void FeiTian()//飞天
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.feitianxitong_KaiGuan)
            {
                UI_GongNeng.feitianxitong_KaiGuan = true;
                UI_GongNeng.FeiTianXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].feitian = allfeitian;
                    YxMod.SetFeiTian(Human.all[i]);
                    if (Human.all[i].feitian)
                    {
                        Chat.TiShi(Human.all[i].player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                    }
                }
                Chat.TiShi($"所有玩家 都 {(allfeitian ? "学会了" : "忘记了")} 飞天");
            }
            else
            {
                YxMod.SetFeiTian(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.feitian ? "赋予" : "取消了")} {human.player.host.name} 飞天能力");
                if (human.feitian)
                {
                    Chat.TiShi(human.player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                }
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("feitian"), $"{humanID - 1}");
        }
    }
    private static void ChaoRen()//超人
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.feitianxitong_KaiGuan)
            {
                UI_GongNeng.feitianxitong_KaiGuan = true;
                UI_GongNeng.FeiTianXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].chaoren = allchaoren;
                }
                Chat.TiShi($"所有玩家 都 {(allchaoren ? "变成了 超人" : "恢复为人类")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.chaoren ? "赋予" : "取消了")} {human.player.host.name} 超人能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaoren"), $"{humanID - 1}");
        }
    }
    private static void ShanXian()//闪现
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.shanxianxitong_KaiGuan)
            {
                UI_GongNeng.shanxianxitong_KaiGuan = true;
                UI_GongNeng.ShanXianXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].shanxian = allshanxian;
                }
                Chat.TiShi($"所有玩家 都 {(allshanxian ? "学会了" : "忘记了")} 闪现");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.shanxian ? "赋予" : "取消了")} {human.player.host.name} 闪现能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("shanxian"), $"{humanID - 1}");
        }
    }

    private static void DongJie()//冻结
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    
                    Human.all[i].dongjie = alldongjie;
                    YxMod.DongJie(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都被 {(alldongjie ? "冻结" : "解冻")} 了");
            }
            else
            {
                YxMod.DongJie(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.dongjie ? "冻结了" : "解冻了")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("dongjie"), $"{humanID - 1}");
        }
    }
    private static void BanShen()//半身不遂
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].banshen = allbanshen;
                    YxMod.BanShen(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(allbanshen ? "学会" : "忘记")} 了 半身不遂");
            }
            else
            {
                YxMod.BanShen(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.banshen ? "赋予了" : "取消了")} {human.player.host.name} 半身不遂");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("banshen"), $"{humanID - 1}");
        }
    }

    private static void BengDi()//蹦迪
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {

                    Human.all[i].bengdi = allbengdi;
                    YxMod.BengDi(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(allbengdi ? "学会了" : "忘记了")} 蹦迪");
            }
            else
            {
                YxMod.BengDi(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.bengdi ? "赋予" : "取消了")} {human.player.host.name} 蹦迪能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("bengdi"), $"{humanID - 1}");
        }
    }
    private static void SanJiTiao()//三级跳
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].sanjitiao = allsanjitiao;
                }
                Chat.TiShi($"所有玩家 都 {(allsanjitiao ? "学会了" : "忘记了")} 三级跳");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.sanjitiao ? "赋予" : "取消了")} {human.player.host.name} 三级跳能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("sanjitiao"), $"{humanID - 1}");
        }
    }
    private static void DianTun()//电臀
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].diantun = alldiantun;
                    YxMod.DianTun(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(alldiantun ? "学会了" : "忘记了")} 电臀");
            }
            else
            {
                YxMod.DianTun(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.diantun ? "赋予" : "取消了")} {human.player.host.name} 电臀能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("diantun"), $"{humanID - 1}");
        }
    }
    private static void QiQiu()//气球
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {

                    Human.all[i].qiqiu = allqiqiu;
                    YxMod.QiQiu(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(allqiqiu ? "变成了 气球" : "恢复了")}");
            }
            else
            {
                YxMod.QiQiu(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.qiqiu ? "变成了气球" : "恢复正常")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qiqiu"), $"{humanID - 1}");
        }
    }
    private static void QiQiuXiFa()//气球戏法
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].qiqiuxifa = allqiqiuxifa;
                }
                Chat.TiShi($"所有玩家 都 {(allqiqiuxifa ? "学会了" : "忘记了")} 左键抓住物品可以起飞的气球戏法");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.qiqiuxifa ? "赋予" : "取消了")} {human.player.host.name} 左键抓住物品可以起飞的气球戏法能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qiqiuxifa"), $"{humanID - 1}");
        }
    }
    private static void DaoLi()//倒立
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].daoli = alldaoli;
                    YxMod.DaoLi(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(alldaoli ? "学会了" : "忘记了")} 双手抓地使用倒立");
            }
            else
            {
                YxMod.DaoLi(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.daoli ? "赋予" : "取消了")} {human.player.host.name} 双手抓地使用倒立能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("daoli"), $"{humanID - 1}");
        }
    }
    private static void ZhuanQuan()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].zhuanquan = allzhuanquan;
                }
                Chat.TiShi($"所有玩家 都 {(allzhuanquan ? "学会了" : "忘记了")} 按住空格可以转圈圈");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.zhuanquan ? "赋予" : "取消了")} {human.player.host.name} 按住空格可以转圈圈能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("zhuanquan"), $"{humanID - 1}");
        }
    }
    private static void TuoLuo()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].tuoluo = alltuoluo;
                }
                Chat.TiShi($"所有玩家 都 {(alltuoluo ? "变成了 小陀螺" : "恢复了")} ");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.tuoluo ? "变成了小陀螺" : "恢复正常")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuoluo"), $"{humanID - 1}");
        }
    }
    private static void KeTouGuai()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].ketouguai = allketouguai;
                }
                Chat.TiShi($"所有玩家 都 {(allketouguai ? "变成了 磕头怪" : "恢复了")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.ketouguai ? "变成了磕头怪" : "恢复正常")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("ketouguai"), $"{humanID - 1}");
        }
    }
    private static void DiaoSiGui()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].diaosigui = alldiaosigui;
                    YxMod.DiaoSiGui(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(alldiaosigui ? "变成了 吊死鬼" : "恢复了")}");
            }
            else
            {
                YxMod.DiaoSiGui(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.diaosigui ? "变成了吊死鬼" : "恢复正常")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("diaosigui"), $"{humanID - 1}");
        }
    }
    private static void PangXie()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].pangxie = allpangxie;
                }
                Chat.TiShi($"所有玩家 都 {(allpangxie ? "变成了 大闸蟹" : "恢复了")}");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把 {human.player.host.name} {(human.pangxie ? "变成了大闸蟹" : "恢复正常")}");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("pangxie"), $"{humanID - 1}");
        }
    }
    private static void QianShui()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].qianshui = allqianshui;
                }
                Chat.TiShi($"所有玩家 都 {(allqianshui ? "学会了" : "忘记了")} 潜水");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.qianshui ? "赋予" : "取消了")} {human.player.host.name} 潜水能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("qianshui"), $"{humanID - 1}");
        }
    }
    private static void TuiQue()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }

            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].tuique = alltuique;
                    YxMod.TuiQue(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(alltuique ? "瘸了一条腿" : "被治好了")}");
            }
            else
            {
                YxMod.TuiQue(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.tuique ? "打瘸了" : "治好了")} {human.player.host.name} 一条腿");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuique"), $"{humanID - 1}");
        }
    }
    private static void TuiGuai()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].tuiguai = alltuiguai;
                    YxMod.TuiGuai(Human.all[i]);
                }
                Chat.TiShi($"所有玩家 都 {(alltuiguai ? "获得了" : "丢掉了")} 拐杖");
            }
            else
            {
                YxMod.TuiGuai(human);
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.tuiguai ? "送给" : "收回了")} {human.player.host.name} 一支拐杖");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("tuiguai"), $"{humanID - 1}");
        }
    }
    private static void ChaiChu()
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].chaichu = allchaichu;
                    if (Human.all[i].chaichu)
                    {
                        Chat.TiShi(Human.all[i].player.host, "开启拆除,左手抓住目标，即可拆卸。");
                    }
                }
                Chat.TiShi($"所有玩家 都 {(allchaichu ? "学会了" : "忘记了")} 拆除");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.chaichu ? "赋予" : "取消了")} {human.player.host.name} 拆除能力");
                if (human.chaichu)
                {
                    Chat.TiShi(human.player.host, "开启拆除,左手抓住目标，即可拆卸。");
                }
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaichu"), $"{humanID - 1}");
        }
    }
    private static void KongQiPao()//空气炮
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0)// 全员控制
            {
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].kongqipao = allkongqipao;
                    if (Human.all[i].kongqipao)
                    {
                        Chat.TiShi(Human.all[i].player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                    }
                }
                Chat.TiShi($"所有玩家 都 {(allkongqipao ? "获得了" : "丢掉了")} 空气炮");
            }
            else// 单个玩家控制
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.kongqipao ? "赋予" : "取消了")} {human.player.host.name} 空气炮能力");
                if (human.kongqipao)
                {
                    Chat.TiShi(human.player.host, "长按 鼠标左键 向前方打出空气炮，被击中的物体会被击飞。");
                }
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("kongqipao"), $"{humanID - 1}");
        }
    }

    private static void chaojitiao()//超级跳
    {
        if (NetGame.isServer)
        {
            if (!UI_GongNeng.yulexitong_KaiGuan)
            {
                UI_GongNeng.yulexitong_KaiGuan = true;
                UI_GongNeng.YuLeXiTong();
            }
            if (humanID == 0) // 全员控制
            {
                YxMod.SuperJumpEnabled = allchaojitiao; // 同步全局状态
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].chaojitiao = allchaojitiao;
                    YxMod.chaojitiao(Human.all[i]); // 为每个玩家应用超级跳效果
                }
                Chat.TiShi($"所有玩家 都 {(allchaojitiao ? "学会了" : "忘记了")} 超级跳");
            }
            else // 单个玩家控制
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} {(human.chaojitiao ? "赋予" : "取消了")} {human.player.host.name} 超级跳能力");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && (YxMod.KeJiQuanXian || KeJiZiJi()))
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("chaojitiao"), $"{humanID - 1}");
        }
    }
}
