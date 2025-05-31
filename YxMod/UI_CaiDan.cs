using HumanAPI;
using Multiplayer;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SteelSeriesHFF;



internal class UI_CaiDan
{
    private static Vector2 scrollPosition;
    public static float gaodu;

    public static bool TouDeng;
    public static GameObject TouDengObject;
    public static bool QuanJuDeng;
    public static bool ZhenJiaTiChu;
    public static bool JieMiMoShi;
    public static bool ZhaoBuTongMoShi;
    public static bool JieSuoChengJiu;
    public static bool HuaBing;
    public static int chuansongzhiTop;
    public static int xuanfuyuTop;
    public static int qianshouTop;


    public static List<GameObject> 碰撞真渲染无 = new List<GameObject>();
    public static List<GameObject> 碰撞真渲染假 = new List<GameObject>();
    public static List<GameObject> 渲染真碰撞无 = new List<GameObject>();
    public static List<GameObject> 渲染真碰撞假 = new List<GameObject>();
    public static List<GameObject> 渲染真碰撞真触发真 = new List<GameObject>();
    public static List<GameObject> 渲染真碰撞真图层非碰撞图层 = new List<GameObject>();
    public static GameObject boxGameObject;
    public static GameObject sphereGameObject;



    public static void CreatUI()//创建菜单功能区
    {
        gaodu= 0;
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true)); //开始滚动视图区域
        UI.CreatAnNiu("继续", false, JiXu);
        gaodu += UI.buttonHeight;
        if (NetGame.isServer || (NetGame.isClient))
        {
            UI.CreatAnNiu("载点(Ctrl+F)", false, ZaiRuCunDangDian_CaiDan);
            gaodu += UI.buttonHeight;
        }

        if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
        {
            UI.CreatAnNiu("重新开始", false, ChongXinKaiShi_CaiDan);
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("上一关(PgUp)", false, ShangYiGuan_CaiDan);
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("下一关(PgDn)", false, XiaYiGuan_CaiDan);
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("重置物品(F3)", false, ChongZhiWuPin_CaiDan);
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("召集(F2)", false, JiHe_CaiDan);
            gaodu += UI.buttonHeight;
        }

        if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer))
        {
            UI.CreatAnNiu("传送至(C)",  false,ChuanSongZhi);
            chuansongzhiTop = (int)gaodu;
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("悬浮于(X)", false, XuanFuYu);
            xuanfuyuTop = (int)gaodu;
            gaodu += UI.buttonHeight;
            UI.CreatAnNiu("牵手(Z)", false, QianShou);
            qianshouTop = (int)gaodu;
            gaodu += UI.buttonHeight;
        }
        if (NetGame.isServer || (NetGame.isClient))
        {
            UI.CreatAnNiu("邀请好友", false, YaoQingHaoYou);
            gaodu += UI.buttonHeight;
        }
        //if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
        //{
        //    UI.CreatAnNiu("换图", false);
        //gaodu += UI.buttonHeight;
        //}
        if (NetGame.isServer && App.state == AppSate.ServerPlayLevel)
        {
            UI.CreatAnNiu("返回大厅", false,FanHuiDaTing);
            gaodu += UI.buttonHeight;
        }
        else if ((NetGame.isServer && App.state == AppSate.ServerLobby) || NetGame.isClient)//在大厅
        {
            UI.CreatAnNiu("离开游戏", false, LiKai);
            gaodu += UI.buttonHeight;
        }

        GUILayout.Space(10);
        gaodu += 10;
        if (NetGame.isServer || (NetGame.isClient))
        {
            GUILayout.BeginHorizontal();
            UI.CreatAnNiu("分身+1", false, AddFenShen);
            UI.CreatAnNiu("分身-1", false, JianFenShen);
            gaodu += UI.buttonHeight;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            UI.CreatAnNiu("平滑+1", false, AddSmooth);
            UI.CreatAnNiu("平滑-1", false, JianSmooth);
            gaodu += UI.buttonHeight;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            UI.CreatAnNiu_AnXia("头灯", ref TouDeng, false, OnTouDeng_CaiDan);
            UI.CreatAnNiu_AnXia("全局灯", ref QuanJuDeng, false, OnQuanJuDeng_CaiDan);
            gaodu += UI.buttonHeight;
            GUILayout.EndHorizontal();
            UI.CreatAnNiu_AnXia("真假剔除",ref ZhenJiaTiChu, false, ZhenJiaTiChu_Mod);  //////public void TrueandFalseModels()
            gaodu += UI.buttonHeight;
            //UI.CreatAnNiu_AnXia("解密模式", ref JieMiMoShi, false);
            //UI.CreatAnNiu_AnXia("找不同模式", ref ZhaoBuTongMoShi, false);
            UI.CreatAnNiu_AnXia("解锁Steam成就", ref JieSuoChengJiu, false, JieSuoChengJiu_CaiDan);
            gaodu += UI.buttonHeight;
        }
        if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian))
        {
            UI.CreatAnNiu_AnXia("滑冰图", ref HuaBing, false, HuaBingTu);
            gaodu += UI.buttonHeight;
            //UI.CreatAnNiu_AnXia("蹦迪图", ref BengDi, false);
            GUILayout.Space(10);
            gaodu += 10;
        }


        UI.CreatAnNiu("定点设置>>", false,CaiDan_DingDianSheZhi);
        gaodu += UI.buttonHeight;
        UI.CreatAnNiu("开房设置>>", false, CaiDan_KaiFangSheZhi);
        gaodu += UI.buttonHeight;
        UI.CreatAnNiu("聊天设置>>", false, CaiDan_LiaoTianSheZhi);
        gaodu += UI.buttonHeight;
        UI.CreatAnNiu("UI显示设置>>", false, CaiDan_UISheZhi);
        gaodu += UI.buttonHeight;
        UI.CreatAnNiu("游戏设置>>", false,CaiDan_YouXiSheZhi);
        gaodu += UI.buttonHeight;
        UI.CreatAnNiu("YxMod设置>>", false,CaiDan_YxModSheZhi);
        gaodu += UI.buttonHeight;
        GUILayout.EndScrollView();

        //GUILayout.FlexibleSpace();

    }

    private static void CaiDan_DingDianSheZhi()
    {
        UI_SheZhi.shezhiID = 0;
        UI_Main.ShowSheZhiUI=true;
    }
    private static void CaiDan_KaiFangSheZhi()
    {
        UI_SheZhi.shezhiID = 1;
        UI_Main.ShowSheZhiUI = true;
    }
    private static void CaiDan_LiaoTianSheZhi()
    {
        UI_SheZhi.shezhiID = 2;
        UI_Main.ShowSheZhiUI = true;
    }
    private static void CaiDan_UISheZhi()
    {
        UI_SheZhi.shezhiID = 3;
        UI_Main.ShowSheZhiUI = true;
    }
    private static void CaiDan_YouXiSheZhi()
    {
        UI_SheZhi.shezhiID = 4;
        UI_Main.ShowSheZhiUI = true;
    }
    private static void CaiDan_YxModSheZhi()
    {
        UI_SheZhi.shezhiID = 5;
        UI_Main.ShowSheZhiUI = true;
    }

    public static void Update()
    {
        if(!NetGame.isServer && !NetGame.isClient)
        {
            return;
        }
        if(TouDeng && TouDengObject != null)
        {
            TouDengObject.transform.position = GameObject.Find("Listener").transform.position + new Vector3(0f, 1f, 0f);
        }
        
    }
    public static void JiXu()
    {
        UI_Main.XianShiCaiDan(false);
    }
    public static void ZaiRuCunDangDian_CaiDan()
    {
        JiXu();
        ZaiRuCunDangDian();
    }
    public static void ZaiRuCunDangDian()
    {
        if (NetGame.isServer)
        {
            Game.instance.Respawn(NetGame.instance.local.players[0].human, Vector3.zero);
            //Game.instance.RestartCheckpoint();//全部
        }
        else if (NetGame.isClient)
        {
            NetGame.instance.SendRequestRespawn();
            Game.instance.Resume();
        }
    }
    private static void ChongXinKaiShi_CaiDan()
    {
        JiXu();
        ChongXinKaiShi();
    }
    public static void ChongXinKaiShi()
    {
        if (NetGame.isServer)
        {
            Game.instance.RestartLevel();
            Game.instance.Resume();
            Chat.TiShi($"玩家 {NetGame.instance.local.name} 重新开始了游戏");
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("cxks"));//重新开始
            
        }
        
    }
    private static void ShangYiGuan_CaiDan()
    {
        JiXu();
        ShangYiGuan();
    }
    public static void ShangYiGuan()
    {
        if (NetGame.isServer)
        {
            YxMod.ShangYiGuan();
            Chat.TiShi($"玩家 {NetGame.instance.local.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("syg"));//上一关
        }
    }

    private static void XiaYiGuan_CaiDan()
    {
        JiXu();
        XiaYiGuan();
    }
    public static void XiaYiGuan()
    {
        if (NetGame.isServer)
        {
            YxMod.XiaYiGuan();
            Chat.TiShi($"玩家 {NetGame.instance.local.name} 使游戏进入到第 {Game.instance.currentCheckpointNumber + 1} 关");
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("xyg"));//下一关
        }
        
    }
    private static void ChongZhiWuPin_CaiDan()
    {
        JiXu();
        ChongZhiWuPin();
    }
    public static void ChongZhiWuPin()
    {
        if (NetGame.isServer)
        {
            YxMod.ChongZhiWuPin();
            Chat.TiShi($"玩家 {NetGame.instance.local.name} 重置了所有物品");
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("czwp"));
            
        }
        
    }
    private static void JiHe_CaiDan()
    {
        JiXu();
        JiHe();
    }
    public static void JiHe()
    {
        if (NetGame.isServer)
        {
            Human human = NetGame.instance.local.players[0].human;
            YxMod.JiHe(human);
            Chat.TiShi($"玩家 {NetGame.instance.local.name} 召集了所有玩家");
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("jihe"));//集合
            
        }
        
    }
    private static void ChuanSongZhi()
    {
        UI_Main.ShowChuanSongUI =true;
        if (UI_Main.ShowChuanSongUI)
        {
            UI_Main.ShowXuanFuUI = UI_Main.ShowQianShouUI = false;

            UI_Main.chuansongUiRect.yMin = UI_Main.caidanUiRect.yMin + chuansongzhiTop + 35;

            if (NetGame.isServer)
            {
                UI_ChuanSong.yuan_humanID = 1;
            }
            else if (NetGame.isClient)
            {
                UI_ChuanSong.yuan_humanID = UI_WanJia.GetClientHumanID();
            }

        }
    }
    private static void XuanFuYu()
    {
        UI_Main.ShowXuanFuUI=true;
        if (UI_Main.ShowXuanFuUI)
        {
            UI_Main.ShowChuanSongUI = UI_Main.ShowQianShouUI = false;
            UI_Main.xuanfuUiRect.yMin = UI_Main.caidanUiRect.yMin + xuanfuyuTop + 35;
            if (NetGame.isServer)
            {
                UI_XuanFu.yuan_humanID = 1;
            }
            else if (NetGame.isClient)
            {
                UI_XuanFu.yuan_humanID = UI_WanJia.GetClientHumanID();
            }
        }
    }
    private static void QianShou()
    {
        UI_Main.ShowQianShouUI = true;
        if (UI_Main.ShowQianShouUI)
        {
            UI_Main.ShowChuanSongUI = UI_Main.ShowXuanFuUI = false;
            UI_Main.qianshouUiRect.yMin = UI_Main.caidanUiRect.yMin + qianshouTop + 35;
            if (NetGame.isServer)
            {
                UI_QianShou.yuan_humanID = 1;
            }
            else if (NetGame.isClient)
            {
                UI_QianShou.yuan_humanID = UI_WanJia.GetClientHumanID();
            }
        }
    }
    private static void YaoQingHaoYou()
    {
        JiXu();
        NetGame.instance.transport.SendInvite();
    }
    private static void LiKai()
    {
        //App.instance.TuiChu(false);
        JiXu();
        LiKaiYouXi();
    }
    public static void LiKaiYouXi()
    {
        if (App.state == AppSate.ClientLoadLevel || App.state == AppSate.ClientLoadLobby || App.state == AppSate.ServerLoadLevel || App.state == AppSate.ServerLoadLobby)
        {
            NetGame.instance.LeaveGame();
            Game.instance.singleRun = false;
            SteamUGC.StopPlaytimeTrackingForAllItems();
            Game.instance.UnloadLevel();
            //EnterMenu();
            MenuSystem.instance.ShowMainMenu();
            return;
        }
        if (App.state == AppSate.PlayLevel || App.state == AppSate.ClientPlayLevel || App.state == AppSate.ServerPlayLevel || App.state == AppSate.ClientWaitServerLoad)
        {
            App.instance.PauseLeave(instantLeave: true);
        }
        if (App.isClient)
        {
            App.instance.LeaveLobby();
        }
        else if (App.isServer)
        {
            App.instance.StopServer();
        }

        //            NetGame.instance.LeaveGame();
        //            EnterMenu();
        //            MenuSystem.instance.ShowMainMenu();
    }
    private static void FanHuiDaTing()
    {
        //App.instance.TuiChu(true);
        JiXu();
        LiKaiYouXi();
    }
    private static void AddFenShen()
    {
        YxMod.Add_Local_Player("1");
    }
    private static void JianFenShen()
    {
        YxMod.Remove_Local_Player("1");
    }
    private static void AddSmooth()
    {
        OnCameraSmooth(((Options.cameraSmoothing / 20) + 1.0).ToString());
    }
    private static void JianSmooth()
    {
        OnCameraSmooth(((Options.cameraSmoothing / 20) - 1.0).ToString());
    }
    private static void OnCameraSmooth(string param)
    {
        if (!string.IsNullOrEmpty(param))
        {
            param = param.ToLowerInvariant();
            if (float.TryParse(param, out var result))
            {
                Options.cameraSmoothing = (int)(result * 20f);
            }
            else if ("off".Equals(param))
            {
                Options.cameraSmoothing = 0;
            }
            else if ("on".Equals(param))
            {
                Options.cameraSmoothing = 20;
            }
            Chat.TiShi(NetGame.instance.local , "smooth变更为" + Options.cameraSmoothing / 20);
        }
    }
    private static void OnTouDeng_CaiDan()
    {
        OnTouDeng(TouDeng);
    }

    private static void OnTouDeng(bool DaKai = true)///头顶灯光源
    {
        TouDengObject = GameObject.Find("TouDeng");
        if (DaKai)
        {
            if(TouDengObject == null)
            {
                TouDengObject = new GameObject("TouDeng");
                TouDengObject.AddComponent<Light>();
                TouDengObject.GetComponent<Light>().range = 20f;
                TouDengObject.GetComponent<Light>().intensity = 1f;
            }
            //TouDengObject.transform.position = GameObject.Find("Listener").transform.position + new Vector3(0f, 1f, 0f);
            return;
        }
        //gameObject.transform.position = new Vector3(0f, 0f, 0f);
        UnityEngine.Object.Destroy(TouDengObject);
    }
    private static void OnQuanJuDeng_CaiDan()
    {
        OnQuanJuDeng(QuanJuDeng);
    }
    private static void OnQuanJuDeng(bool DaKai = true)///全局灯光源
    {
        GameObject gameObject = GameObject.Find("QuanJuDeng");
        if (DaKai)
        {
            if (gameObject == null)
            {
                gameObject = new GameObject("QuanJuDeng");
                gameObject.AddComponent<Light>();
                gameObject.GetComponent<Light>().type = LightType.Directional;
                gameObject.transform.rotation = Quaternion.Euler(60f, 30f, 0f);
            }
            return;
        }
        UnityEngine.Object.Destroy(gameObject);
    }
    public static void ZhenJiaTiChu_Mod()///真假剔除
    {
        if (ZhenJiaTiChu)
        {
            if (碰撞真渲染假.Count <= 0 && 碰撞真渲染无.Count <= 0 && 渲染真碰撞假.Count <= 0 && 渲染真碰撞无.Count <= 0 && 渲染真碰撞真触发真.Count <= 0 && 渲染真碰撞真图层非碰撞图层.Count <= 0)
            {
                //ZhenJiaTiChu=true;
                MeshRenderer[] componentsInChildren = (UnityEngine.Object.FindObjectOfType<BuiltinLevel>()).gameObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer meshRenderer in componentsInChildren)
                {
                    if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>() == null)
                    {
                        meshRenderer.enabled = false;
                        渲染真碰撞无.Add(meshRenderer.gameObject);
                    }
                    else if (meshRenderer.enabled && !meshRenderer.gameObject.GetComponent<Collider>().enabled)
                    {
                        meshRenderer.enabled = false;
                        渲染真碰撞假.Add(meshRenderer.gameObject);
                    }
                    else if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>().enabled && meshRenderer.gameObject.GetComponent<Collider>().isTrigger)
                    {
                        meshRenderer.enabled = false;
                        渲染真碰撞真触发真.Add(meshRenderer.gameObject);
                    }
                    else if (meshRenderer.enabled && meshRenderer.gameObject.GetComponent<Collider>().enabled && !meshRenderer.gameObject.GetComponent<Collider>().isTrigger && !GameObjectLayerTest(meshRenderer.gameObject))
                    {
                        meshRenderer.enabled = false;
                        渲染真碰撞真触发真.Add(meshRenderer.gameObject);
                    }
                }
                Collider[] componentsInChildren2 = UnityEngine.Object.FindObjectOfType<BuiltinLevel>().gameObject.GetComponentsInChildren<Collider>(includeInactive: true);
                foreach (Collider collider in componentsInChildren2)
                {
                    if (!collider.enabled || collider.isTrigger || !GameObjectLayerTest(collider.gameObject))
                    {
                        continue;
                    }
                    if (collider.gameObject.GetComponent<MeshRenderer>() != null)
                    {
                        if (!collider.GetComponent<MeshRenderer>().enabled)
                        {
                            collider.GetComponent<MeshRenderer>().enabled = true;
                            碰撞真渲染假.Add(collider.gameObject);
                        }
                        continue;
                    }
                    if (collider.gameObject.GetComponent<MeshRenderer>() == null)
                    {
                        collider.gameObject.AddComponent<MeshRenderer>();
                    }
                    if (collider.gameObject.GetComponent<MeshFilter>() == null)
                    {
                        collider.gameObject.AddComponent<MeshFilter>();
                    }
                    if (collider as MeshCollider)
                    {
                        collider.gameObject.GetComponent<MeshFilter>().sharedMesh = ((MeshCollider)collider).sharedMesh;
                    }
                    if (collider as BoxCollider)
                    {
                        if (boxGameObject == null)
                        {
                            boxGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        }
                        collider.gameObject.GetComponent<MeshFilter>().sharedMesh = boxGameObject.GetComponent<MeshFilter>().sharedMesh;
                    }
                    if (collider as SphereCollider)
                    {
                        if (sphereGameObject == null)
                        {
                            sphereGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        }
                        collider.gameObject.GetComponent<MeshFilter>().sharedMesh = sphereGameObject.GetComponent<MeshFilter>().sharedMesh;
                    }
                    碰撞真渲染无.Add(collider.gameObject);
                }
                //Debug.Log(string.Concat(new object[12]
                //{
                //"碰撞真渲染假=", 碰撞真渲染假.Count, " 碰撞真渲染无=", 碰撞真渲染无.Count, " 渲染真碰撞假=", 渲染真碰撞假.Count, " 渲染真碰撞无=", 渲染真碰撞无.Count, " 渲染真碰撞真触发真=", 渲染真碰撞真触发真.Count,
                //" 渲染真碰撞真图层非碰撞图层=", 渲染真碰撞真图层非碰撞图层.Count
                //}));
            }
            Chat.TiShi(NetGame.instance.local, "真假剔除已打开");
            return;
        }
        //ZhenJiaTiChu = false;
        Chat.TiShi(NetGame.instance.local, "真假剔除已关闭");
        foreach (GameObject item in 碰撞真渲染假)
        {
            try
            {
                item.GetComponent<MeshRenderer>().enabled = false;
            }
            catch
            {
                break;
            }
        }
        碰撞真渲染假.Clear();
        foreach (GameObject item2 in 碰撞真渲染无)
        {
            try
            {
                item2.GetComponent<MeshRenderer>().enabled = false;
            }
            catch
            {
                break;
            }
        }
        碰撞真渲染无.Clear();
        foreach (GameObject item3 in 渲染真碰撞假)
        {
            try
            {
                item3.GetComponent<MeshRenderer>().enabled = true;
            }
            catch
            {
                break;
            }
        }
        渲染真碰撞假.Clear();
        foreach (GameObject item4 in 渲染真碰撞无)
        {
            try
            {
                item4.GetComponent<MeshRenderer>().enabled = true;
            }
            catch
            {
                break;
            }
        }
        渲染真碰撞无.Clear();
        foreach (GameObject item5 in 渲染真碰撞真触发真)
        {
            try
            {
                item5.GetComponent<MeshRenderer>().enabled = true;
            }
            catch
            {
                break;
            }
        }
        渲染真碰撞真触发真.Clear();
        foreach (GameObject item6 in 渲染真碰撞真图层非碰撞图层)
        {
            try
            {
                item6.GetComponent<MeshRenderer>().enabled = true;
            }
            catch
            {
                break;
            }
        }
        渲染真碰撞真图层非碰撞图层.Clear();
        //Debug.Log(string.Concat(new object[12]
        //{
        //    "碰撞真渲染假=", 碰撞真渲染假.Count, " 碰撞真渲染无=", 碰撞真渲染无.Count, " 渲染真碰撞假=", 渲染真碰撞假.Count, " 渲染真碰撞无=", 渲染真碰撞无.Count, " 渲染真碰撞真触发真=", 渲染真碰撞真触发真.Count,
        //    " 渲染真碰撞真图层非碰撞图层=", 渲染真碰撞真图层非碰撞图层.Count
        //}));
    }
    public static bool GameObjectLayerTest(GameObject gameObject)
    {
        if (gameObject.layer != 1 && gameObject.layer != 1 && gameObject.layer != 4 && gameObject.layer != 8 && gameObject.layer != 11 && gameObject.layer != 12 && gameObject.layer != 15 && gameObject.layer != 17)
        {
            return gameObject.layer != 19;
        }
        return false;
    }
    private static void JieSuoChengJiu_CaiDan()
    {
        if(JieSuoChengJiu)
        {
            unlock();
        }
        else
        {
            StatsAndAchievements.ResetAchievements();
            Chat.TiShi(NetGame.instance.local, "成就已重置");
        }
    }
    public static void unlock()//解锁成就
    {
        IEnumerator enumerator = Enum.GetValues(Type.GetTypeFromHandle(typeof(Achievement).TypeHandle)).GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                StatsAndAchievements.UnlockAchievement((Achievement)enumerator.Current);
            }
        }
        finally
        {
            if (enumerator is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
    private static void HuaBingTu()
    {
        if (NetGame.isServer)
        {
            HuaBingTu_Fun(HuaBing);
            if(HuaBing)
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把地图变成了滑冰蹦迪地图");
            }
            else
            {
                Chat.TiShi($"玩家 {NetGame.instance.local.name} 把地图恢复了原状");
            }
        }
        else if (NetGame.isClient && YxMod.YxModServer && YxMod.KeJiQuanXian)
        {
            Chat.SendYxModMsgClient(Chat.YxModMsgStr("huabing"));//集合
        }
    }

    public static void HuaBingTu_Fun(bool DaKai=true)
    {
        GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject gameObject in array)
        {
            if (!(Vector3.Distance(gameObject.transform.position, NetGame.instance.local.players[0].human.transform.position) < 1000f) || !(gameObject.GetComponentInParent<Human>() == null))
            {
                continue;
            }
            Collider component = gameObject.GetComponent<Collider>();
            try
            {
                if (component != null)
                {
                    if (!DaKai)
                    {
                        component.material.bounceCombine = PhysicMaterialCombine.Minimum;
                        component.material.bounciness = 0f;
                        component.material.frictionCombine = PhysicMaterialCombine.Average;
                        component.material.staticFriction = 0.5f;
                        component.material.dynamicFriction = 0.5f;
                        component.sharedMaterial.staticFriction = 0.5f;
                        component.sharedMaterial.dynamicFriction = 0.5f;
                    }
                    else
                    {
                        component.material.bounceCombine = PhysicMaterialCombine.Maximum;
                        component.material.bounciness = 1f;
                        component.material.frictionCombine = PhysicMaterialCombine.Minimum;
                        component.material.staticFriction = 0f;
                        component.material.dynamicFriction = 0f;
                        component.sharedMaterial.staticFriction = 0f;
                        component.sharedMaterial.dynamicFriction = 0f;
                    }
                }
            }
            catch
            {
            }
        }
    }
}
