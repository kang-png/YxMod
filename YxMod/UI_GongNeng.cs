using Multiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class UI_GongNeng
{
    public static bool wujiasi_KaiGuan;
    public static bool dingdian_KaiGuan ;
    public static bool ifg_KaiGuan;
    public static bool up_KaiGuan;
    public static bool wupengzhuang_KaiGuan;
    public static bool kejiquanxian_KaiGuan;
    public static bool liaotiankuangquanxian_KaiGuan;
    public static bool feitianxitong_KaiGuan;
    public static bool shanxianxitong_KaiGuan;
    public static bool guajianxitong_KaiGuan;
    public static bool chuansongxitong_KaiGuan;
    public static bool yulexitong_KaiGuan;
    public static bool jifeixitong_KaiGuan;
    public static bool qianshouxitong_KaiGuan;
    public static bool chaojitiao_KaiGuan;


    public static void CreatUI()//创建菜单功能区
    {
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("爬墙代码", ref CheatCodes.climbCheat, false, CheatClimb);
        UI.CreatAnNiu_AnXia("漂浮代码", ref CheatCodes.throwCheat, false, CheatThrow);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("无假死", ref wujiasi_KaiGuan, false, WuJiaSi);
        UI.CreatAnNiu_AnXia("无碰撞系统", ref wupengzhuang_KaiGuan, false, WuPengZhuang);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("定点系统", ref dingdian_KaiGuan, false, DingDian);
        UI.CreatAnNiu_AnXia("Y击飞系统", ref jifeixitong_KaiGuan, false, JiFeiXiTong);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("UP", ref up_KaiGuan, false, Up);
        UI.CreatAnNiu_AnXia("IFG", ref ifg_KaiGuan, false, Ifg);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("客机权限", ref kejiquanxian_KaiGuan, false, KeJiQuanXian);
        UI.CreatAnNiu_AnXia("聊天框权限", ref liaotiankuangquanxian_KaiGuan, false, LiaoTianKuangQuanXian);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("飞天系统", ref feitianxitong_KaiGuan, false, FeiTianXiTong);
        UI.CreatAnNiu_AnXia("闪现系统", ref shanxianxitong_KaiGuan, false, ShanXianXiTong);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("传送系统", ref chuansongxitong_KaiGuan, false, ChuanSongXiTong);
        UI.CreatAnNiu_AnXia("挂件系统", ref guajianxitong_KaiGuan, false, GuaJianXiTong);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("娱乐系统", ref yulexitong_KaiGuan, false, YuLeXiTong);
        UI.CreatAnNiu_AnXia("挂机提醒", ref UI_SheZhi.guajitixing, false, UI_SheZhi.SetGuaJiTiXing);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        UI.CreatAnNiu_AnXia("牵手系统", ref qianshouxitong_KaiGuan, false, QianShouXiTong);
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }

    public static void ChuShiHua()
    {
        CheatCodes.climbCheat = PlayerPrefs.GetInt("cheatClimb", 1) > 0;
        CheatCodes.throwCheat = PlayerPrefs.GetInt("cheatThrow", 0) > 0;
        wujiasi_KaiGuan = PlayerPrefs.GetInt("wujiasi_KaiGuan", 1) > 0;
        wupengzhuang_KaiGuan = PlayerPrefs.GetInt("wupengzhuang_KaiGuan", 0) > 0;
        dingdian_KaiGuan = PlayerPrefs.GetInt("dingdian_KaiGuan", 1) > 0;
        jifeixitong_KaiGuan = PlayerPrefs.GetInt("jifeixitong_KaiGuan", 1) > 0;
        up_KaiGuan = PlayerPrefs.GetInt("up_KaiGuan", 1) > 0;
        ifg_KaiGuan = PlayerPrefs.GetInt("ifg_KaiGuan", 1) > 0;
        kejiquanxian_KaiGuan = PlayerPrefs.GetInt("kejiquanxian_KaiGuan", 1) > 0;
        liaotiankuangquanxian_KaiGuan = PlayerPrefs.GetInt("liaotiankuangquanxian_KaiGuan", 0) > 0;
        feitianxitong_KaiGuan = PlayerPrefs.GetInt("feitianxitong_KaiGuan", 1) > 0;
        shanxianxitong_KaiGuan = PlayerPrefs.GetInt("shanxianxitong_KaiGuan", 1) > 0;
        chuansongxitong_KaiGuan = PlayerPrefs.GetInt("chuansongxitong_KaiGuan", 1) > 0;
        guajianxitong_KaiGuan = PlayerPrefs.GetInt("guajianxitong_KaiGuan", 1) > 0;
        yulexitong_KaiGuan = PlayerPrefs.GetInt("yulexitong_KaiGuan", 1) > 0;
        qianshouxitong_KaiGuan = PlayerPrefs.GetInt("qianshouxitong_KaiGuan", 1) > 0;
        chaojitiao_KaiGuan = PlayerPrefs.GetInt("chaojitiao_KaiGuan", 1) > 0;

    }


    private static void CheatClimb()
    {
        string str = $"爬墙代码已{(CheatCodes.climbCheat? "打开":"关闭")}";
        PlayerPrefs.SetInt("cheatClimb", CheatCodes.climbCheat ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void CheatThrow()
    {
        string str = $"漂浮代码已{(CheatCodes.throwCheat ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("cheatThrow", CheatCodes.throwCheat ? 1 : 0);

        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void WuJiaSi()
    {
        string str = $"无假死系统已{(wujiasi_KaiGuan ? "打开" : "关闭")}";

        PlayerPrefs.SetInt("wujiasi_KaiGuan", wujiasi_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void DingDian()
    {
        string str = $"定点系统已{(dingdian_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("dingdian_KaiGuan", dingdian_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void JiFeiXiTong()
    {
        string str = $"Y键击飞系统已{(jifeixitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("jifeixitong_KaiGuan", jifeixitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void Up()
    {
        string str = $"up系统已{(up_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("up_KaiGuan", up_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void Ifg()
    {
        string str = $"ifg系统已{(ifg_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("ifg_KaiGuan", ifg_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void WuPengZhuang()
    {
        string str = $"无碰撞系统已{(wupengzhuang_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("wupengzhuang_KaiGuan", wupengzhuang_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void KeJiQuanXian()
    {
        string str = $"客机权限已{(kejiquanxian_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("kejiquanxian_KaiGuan", kejiquanxian_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    private static void LiaoTianKuangQuanXian()
    {
        string str = $"聊天框权限已{(liaotiankuangquanxian_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("liaotiankuangquanxian_KaiGuan", liaotiankuangquanxian_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void FeiTianXiTong()
    {
        string str = $"飞天系统已{(feitianxitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("feitianxitong_KaiGuan", feitianxitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void ShanXianXiTong()
    {
        string str = $"闪现系统已{(shanxianxitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("shanxianxitong_KaiGuan", shanxianxitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void ChuanSongXiTong()
    {
        string str = $"传送系统已{(chuansongxitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("chuansongxitong_KaiGuan", chuansongxitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void GuaJianXiTong()
    {
        string str = $"挂件系统已{(guajianxitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("guajianxitong_KaiGuan", guajianxitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void YuLeXiTong()
    {
        string str = $"娱乐系统已{(yulexitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("yulexitong_KaiGuan", yulexitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void QianShouXiTong()
    {
        string str = $"牵手系统已{(qianshouxitong_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("qianshouxitong_KaiGuan", qianshouxitong_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
    public static void ChaoJiTiao()
    {
        string str = $"超级跳已{(chaojitiao_KaiGuan ? "打开" : "关闭")}";
        PlayerPrefs.SetInt("chaojitiao_KaiGuan", chaojitiao_KaiGuan ? 1 : 0);
        if (NetGame.isServer)
        {
            Chat.TiShi(str, TiShiMsgId.XiTongTiShi);
        }
        else if (NetGame.isClient)
        {
            Chat.TiShi(NetGame.instance.local, $"{str}，仅房主有效");
        }
    }
}
