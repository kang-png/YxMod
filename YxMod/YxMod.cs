using HumanAPI;
using Multiplayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
////////修改的内容///////
///Human                             主要功能 <summary>
///NetGame.Awake/OnClientHelo        增加YxMod类,欢迎词/消息处理函数
///NetPlayer.PreFixedUpdate          人物不动/F8自由视角/显示菜单的时候
///
///MenuSystem.BindCursor             显示光标
///NameTag                           一直显示名字


///NetChat                           发言修改
/// OnLobbyCreated                   创建房间信息修改
///
/// App.LevelLoadedClient     服务器版本与此关卡不同
///PlayerManager.OnLocalPlayerAdded  本地玩家加入时  1  2  改为100 200   分身
///MenuCameraEffects.AddHuman/RemoveHuman        分身不分屏
///LevelRepository.OnDownloadThumbnail  去除文件加载失败
///LevelInformationBox.GetNewLevel      关闭大厅下载
///NetTransportSteam.OnLobbyList     显示私密房间
///MultiplayerSelectLobbyMenu.Update F键    单页显示
/// 
/// 
///
/// 
/// 
/// 
/// 
///
/// 
/// </summary>

public class YxMod : MonoBehaviour
{
    public static bool YxModServer=false;
    public static bool KeJiQuanXian=false;
    public static int fenshen_cam;//分身相机
    public static bool SuperJumpEnabled = false; // 超级跳是否启用



    //public static bool YanZheng_OK;
    //public static string YanZheng_Str = "";

    //public static bool KeYong;

    public static float BanBenHao = 2.8f;//当前版本号,每次更新需要手动修改

    //public static float newBanBenHao;
    //public static bool QiangZhiGengXin;
    //public static string dllurl;
    //public static bool downOK;//下载成功
    //public static bool downNO;//下载失败
    //public static long dllSize;
    //public static bool dllSizeOK;//文件大小网络验证

   // public static bool One;

    private static string SubstringBetween(string source, string startMarker, string endMarker)
    {
        // 找到开始标记的位置
        int startIndex = source.IndexOf(startMarker) + startMarker.Length;

        // 如果开始标记未找到，则返回空字符串或抛出异常（根据实际需求）
        if (startIndex == -1)
        {
            return "";
            // 或者 throw new ArgumentException("开始标记未在源字符串中找到");
        }

        // 找到结束标记的位置
        int endIndex = source.IndexOf(endMarker, startIndex);

        // 如果结束标记未找到，则返回空字符串或抛出异常（根据实际需求）
        if (endIndex == -1)
        {
            return "";
            // 或者 throw new ArgumentException("结束标记未在源字符串中找到");
        }

        // 截取并返回中间的子串
        return source.Substring(startIndex, endIndex - startIndex);
    }

    //IEnumerator JianChaGengXin()
    //{
    
    //    //string url = "http://yxmod.ysepan.com";
    //    string url = "https://vip.123pan.cn/1812996731/YxMod/yxmod2.txt";
    //    UnityWebRequest www = UnityWebRequest.Get(url);
    //    yield return www.SendWebRequest();

    //    if (www.isNetworkError || www.isHttpError)
    //    {
    //        Debug.Log("网络失败" + www.error);

    //    }
    //    else
    //    {
    //        Debug.Log("网络成功");
    //        // 请求成功，处理文本内容
    //        string fileContent = www.downloadHandler.text;
    //        //UnityEngine. Debug.Log(fileContent);
    //        string yanzhengstr = SubstringBetween(fileContent, "<!--yxmod", "yxmod-->");
    //        //Debug.Log(yanzhengstr);
    //        string[] array = yanzhengstr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
    //        foreach (string array2 in array)
    //        {

    //            string[] array3 = array2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    //            if (array3[0] == "可用")
    //            {
    //                int num = 0;
    //                if (int.TryParse(array3[1], out num))
    //                {
    //                    KeYong = num > 0;
    //                    //Debug.Log($"可用 {num}");
    //                }
    //            }
    //            else if (array3[0] == "版本号")
    //            {

    //                float num = 0;
    //                if (float.TryParse(array3[1], out num))
    //                {
    //                    newBanBenHao = num;
    //                    //Debug.Log($"版本号 {newBanBenHao}");
    //                }
    //            }
    //            else if (array3[0] == "强制更新")
    //            {
    //                int num = 0;
    //                if (int.TryParse(array3[1], out num))
    //                {
    //                    QiangZhiGengXin = num > 0;
    //                    //Debug.Log($"强制更新 {num}");
    //                }
    //            }
    //            else if (array3[0] == "FileSize")
    //            {
    //                long num = 0;
    //                if (long.TryParse(array3[1], out num))
    //                {
    //                    dllSize = num;
    //                    //Debug.Log($"FileSize {num}");
    //                }
    //            }
    //            else if (array3[0] == "url")
    //            {
    //                dllurl = array3[1];
    //                //Debug.Log($"dllurl {dllurl}");
    //                //Debug.Log($"newBanBenHao {newBanBenHao}   BanBenHao {BanBenHao}");
    //                if (newBanBenHao != BanBenHao)
    //                {
    //                    //下载DLL
    //                    Debug.Log("update dll");
    //                    // 先获取文件大小
    //                    //StartCoroutine(GetExpectedFileSize(dllurl));
    //                    StartCoroutine(DownDll());//访问网络

    //                }
    //                else
    //                {
                        
    //                    string filePath = Application.dataPath + "/Managed/Assembly-CSharp.dll";
    //                    FileInfo fileInfo = new FileInfo(filePath);

    //                    //Debug.Log($"fileInfo.Length {fileInfo.Length}  dllSize {dllSize}");
    //                    if (fileInfo.Length == dllSize)
    //                    {
    //                        dllSizeOK = true;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
    //IEnumerator DownDll()
    //{
    //    //while (true)
    //    //{
    //    using (UnityWebRequest www = UnityWebRequest.Get(dllurl))
    //    {
    //        yield return www.SendWebRequest();

    //        if (www.isNetworkError || www.isHttpError)
    //        {
    //            downNO = true;
    //            Debug.Log("DownDll:错误");
    //        }
    //        else
    //        {
    //            string filePath = Application.dataPath + "/Managed/Assembly-CSharp2.dll";

    //            // 创建或覆盖文件，并写入下载的二进制数据
    //            File.WriteAllBytes(filePath, www.downloadHandler.data);

    //            FileInfo fileInfo = new FileInfo(filePath);

    //            if (fileInfo.Length == dllSize)
    //            {
    //                downOK = true;

    //                // 删除旧的Assembly-CSharp.dll（如果存在）
    //                string originalFilePath = Application.dataPath + "/Managed/Assembly-CSharp.dll";
    //                if (File.Exists(originalFilePath))
    //                {
    //                    File.Delete(originalFilePath);
    //                    //Debug.Log($"已删除旧的Assembly-CSharp.dll");
    //                }

    //                // 将新的文件重命名为Assembly-CSharp.dll
    //                string newFilePath = Application.dataPath + "/Managed/Assembly-CSharp.dll";
    //                File.Move(filePath, newFilePath);
    //                // Debug.Log($"已将Assembly-CSharp2.dll重命名为Assembly-CSharp.dll，新路径：{newFilePath}");

    //                Debug.Log($"更新成功");
    //            }
    //            else
    //            {
    //                downNO = true;
    //                //Debug.Log($"dll文件下载完成但大小不正确，实际大小：{fileInfo.Length} bytes，期望大小：{dllSize} bytes");
    //                if (File.Exists(filePath))
    //                {
    //                    File.Delete(filePath);
    //                    //Debug.Log($"已删除大小不正确的Assembly-CSharp2.dll");
    //                }
    //                Debug.Log($"更新失败,fileInfo.Length {fileInfo.Length}  dllSize {dllSize}");

    //            }

    //        }
    //    }
    //    //}
    //}
    private void Awake()
    {

        //StartCoroutine(JianChaGengXin());//检查更新
        //YanZheng_OK = true;
        NetGame.instance.gameObject.AddComponent<UI_Main>();
        NetGame.instance.gameObject.AddComponent<KeyDisplayUI>();//键盘UI
        NetGame.instance.gameObject.AddComponent<MiniMap>();//小地图
        NetGame.instance.gameObject.AddComponent<FPSCounter>();//FPS


    }
    private void Start()
    {

    }
    //public void OnGUI()
    //{
    //    if (App.state == AppSate.Startup)
    //    {
    //        if (YxMod.YanZheng_Str.Length != 0)
    //        {
    //            GUIStyle style = new GUIStyle(GUI.skin.GetStyle("label"));
    //            style.fontSize = 30;
    //            style.fontStyle = FontStyle.BoldAndItalic;
    //            style.normal.textColor = Color.red;
    //            style.alignment = TextAnchor.MiddleCenter;
    //            Rect rect = new Rect(0, 0, Screen.width, 100);
    //            GUI.Label(rect, YxMod.YanZheng_Str, style);
    //        }
    //    }
    //}
    public void Update()
    {
        //if (!YanZheng_OK)//验证失败
        //{
        //    if (KeYong)
        //    {
        //        if (newBanBenHao == BanBenHao) // && dllSizeOK
        //        {
        //            YanZheng_Str = "";
        //            YanZheng_OK = true;
        //        }
        //        else
        //        {
        //            if (downOK)
        //            {
        //                if (QiangZhiGengXin)
        //                {
        //                    YanZheng_Str = $"YxMod {newBanBenHao} 更新成功,请重启游戏,否则无法使用!";
        //                    YanZheng_OK = false;
        //                }
        //                else
        //                {
        //                    YanZheng_Str = $"YxMod {newBanBenHao} 更新成功,当前使用的是旧版本,重启游戏后新版本生效";
        //                    YanZheng_OK = true;
        //                }
        //            }
        //            else if (downNO)
        //            {
        //                if (QiangZhiGengXin)
        //                {
        //                    YanZheng_Str = $"YxMod {newBanBenHao} 更新失败,请重启游戏再次尝试更新,否则无法使用!";
        //                    YanZheng_OK = false;
        //                }
        //                else
        //                {
        //                    YanZheng_Str = $"YxMod {newBanBenHao} 更新失败,当前使用的是旧版本,重启游戏后重新尝试更新";
        //                    YanZheng_OK = true;
        //                }

        //            }
        //        }
        //    }
        //    else
        //    {
        //        YanZheng_OK = false;
        //    }
        //    return;
        //}


        //if (!One)
        //{
        //    One = true;
        //    //Debug.Log("验证成功");
        //    NetGame.instance.gameObject.AddComponent<UI_Main>();
        //    NetGame.instance.gameObject.AddComponent<KeyDisplayUI>();//键盘UI
        //    NetGame.instance.gameObject.AddComponent<MiniMap>();//小地图
        //    NetGame.instance.gameObject.AddComponent<FPSCounter>();//FPS
        //    //NetGame.instance.gameObject.AddComponent<DingDian>();//FPS
        //}


        if (UI_SheZhi.quchuqidonghuamian)
        {
            UI_SheZhi.Skip_Start();
        }

        DingDian.Update();//个人定点
        KuaiJieJian_Update();//up ifg 快捷键
        if (NetGame.isLocal)
        {
            UI_CaiDan.ZhenJiaTiChu = false;
        }
        if (NetGame.isLocal || NetGame.isServer)  //初始化客机权限
        {
            YxModServer = false;
            KeJiQuanXian = false;


        }
        if (NetGame.isClient)
        {
            try
            {
                NetGame.instance.local.players[0].human.isClient = true;
            }
            catch { }
        }

    }

    private void KuaiJieJian_Update()
    {
        if (Game.GetKeyDown(KeyCode.F7))///软趴趴 创建房间
        {
            Debug.Log("按了F7");
            ChuangJianFangJian();
        }
        if ((!NetGame.isServer && !NetGame.isClient) || NetChat.typing || Shell.visible)
        {
            return;
        }
        if (Game.GetKeyDown(KeyCode.Mouse3) || Input.GetMouseButtonDown(2) || Game.GetKeyDown(KeyCode.U))
        {
            if (NetGame.isServer)
            {
                Up(Human.all[0]);
            }
            else if (NetGame.isClient)
            {
                if(YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("up"));
                }
                else
                {
                    NetGame.instance.SendChatMessage("up");
                }
                
            }

        }
        if (Game.GetKeyDown(KeyCode.Mouse4) || Input.GetMouseButtonDown(3) || Game.GetKeyDown(KeyCode.B))
        {
            if (NetGame.isServer)
            {
                Ifg(Human.all[0]);
            }
            else if (NetGame.isClient)
            {
                if (YxMod.YxModServer)
                {
                    Chat.SendYxModMsgClient(Chat.YxModMsgStr("ifg"));
                }
                else
                {
                    NetGame.instance.SendChatMessage("ifg");
                }
                
            }
        }

        bool Ctrl = Game.GetKey(KeyCode.LeftControl) || Game.GetKey(KeyCode.RightControl);
        if (Ctrl)
        {
            if (Game.GetKeyDown(KeyCode.F))//载入存档点
            {
                UI_CaiDan.ZaiRuCunDangDian();
            }
        }
        if (Game.GetKeyDown(KeyCode.F2))//集合
        {
            UI_CaiDan.JiHe();
        }
        if (Game.GetKeyDown(KeyCode.F3))//重置物品  ///软趴趴跳过启动画面
        {
            UI_CaiDan.ChongZhiWuPin();
        }
        if (Game.GetKeyDown(KeyCode.F4))///软趴趴 全体飞天
        {
            if (NetGame.isServer)
            {
                if (!UI_GongNeng.feitianxitong_KaiGuan)
                {
                    UI_GongNeng.feitianxitong_KaiGuan = true;
                    UI_GongNeng.FeiTianXiTong();
                }
                UI_WanJia.allfeitian = !UI_WanJia.allfeitian;
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].feitian = UI_WanJia.allfeitian;
                    YxMod.SetFeiTian(Human.all[i]);
                    if (Human.all[i].feitian)
                    {
                        Chat.TiShi(Human.all[i].player.host, "普通情况下是正常飞天。按住左键，W，空格，保持两秒，可进入超人状态。");
                    }
                }
                Chat.TiShi($"所有玩家 都 {(UI_WanJia.allfeitian ? "学会了" : "忘记了")} 飞天");
            }
            //Debug.Log("按了F4");
        }
        if (Game.GetKeyDown(KeyCode.F5))///开房
        {
            Debug.Log("按了F5");
        }
        if (Game.GetKeyDown(KeyCode.F6))///软趴趴 全员飞天
        {
            Debug.Log("按了F6");
        }

        // F8自由视角   F9游戏截图
        if (Game.GetKeyDown(KeyCode.F10))///软趴趴  全员闪现   ///我的 全员超人
        {
            if (NetGame.isServer)
            {
                if (!UI_GongNeng.feitianxitong_KaiGuan)
                {
                    UI_GongNeng.feitianxitong_KaiGuan = true;
                    UI_GongNeng.FeiTianXiTong();
                }

                UI_WanJia.allchaoren = !UI_WanJia.allchaoren;
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].chaoren = UI_WanJia.allchaoren;
                }
                Chat.TiShi($"所有玩家 都 {(UI_WanJia.allchaoren ? "变成了 超人" : "恢复为人类")}");
            }
            //Debug.Log("按了F10");
        }
        if (Game.GetKeyDown(KeyCode.F11))///软趴趴  全员三级跳    ///我的  全员闪现
        {
            if (NetGame.isServer)
            {
                if (!UI_GongNeng.shanxianxitong_KaiGuan)
                {
                    UI_GongNeng.shanxianxitong_KaiGuan = true;
                    UI_GongNeng.ShanXianXiTong();
                }
                UI_WanJia.allshanxian = !UI_WanJia.allshanxian;
                for (int i = 0; i < Human.all.Count; i++)
                {
                    Human.all[i].shanxian = UI_WanJia.allshanxian;
                }
                Chat.TiShi($"所有玩家 都 {(UI_WanJia.allshanxian ? "学会了" : "忘记了")} 闪现");
            }
            //Debug.Log("按了F11");
        }
        if (Game.GetKeyDown(KeyCode.K))///软趴趴  控制物体
        {

        }
        if (Game.GetKeyDown(KeyCode.PageUp))///上一关
        {
            UI_CaiDan.ShangYiGuan();
        }
        if (Game.GetKeyDown(KeyCode.PageDown))///下一关
        {
            UI_CaiDan.XiaYiGuan();
        }

        bool Alt = Game.GetKey(KeyCode.LeftAlt) || Game.GetKey(KeyCode.RightAlt);

        /////分身控制

        for (int i = 0; i < 10; i++)
        {
            if (Alt)   //Alt  + 数字  控制分身
            {
                if ((Game.GetKeyDown(KeyCode.Alpha1 + i) || Game.GetKeyDown(KeyCode.Keypad1 + i)))//切换分身
                {
                    Local_Control(string.Concat(i + 1)); //控制分身
                }
            }
            else if (Ctrl)// Ctrl + 数字键   切换分身
            {
                if ((Game.GetKeyDown(KeyCode.Alpha1 + i) || Game.GetKeyDown(KeyCode.Keypad1 + i)))//切换分身
                {
                    Change_Local_Cam(string.Concat(i + 1));//切换主体
                }
            }
            else
            {
                if (Game.GetKey(KeyCode.Alpha1 + i) || Game.GetKey(KeyCode.Keypad1 + i))
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (j != i)
                        {
                            if (Game.GetKeyDown(KeyCode.Alpha1 + j) || Game.GetKeyDown(KeyCode.Keypad1 + j))
                            {
                                if(i < Human.all.Count && j < Human.all.Count)
                                {
                                    //ChuanSong(Human.all[i], Human.all[j]);
                                    UI_ChuanSong. ChuanSongZhi(i,j);
                                }
                            }
                        }
                        //else
                        //{
                        //    if (Game.GetKeyUp(KeyCode.Alpha1 + j) || Game.GetKeyUp(KeyCode.Keypad1 + j))
                        //    {
                        //        if (i < Human.all.Count && j < Human.all.Count)
                        //        {
                        //            // ChuanSong(NetGame.instance.local.players[0].human, Human.all[j]);
                        //            UI_ChuanSong.ChuanSongZhi(0, j);
                        //        }  
                        //    }
                        //}
                    }
                }
                else if(Game.GetKeyDown(KeyCode.Alpha1 + i) || Game.GetKeyDown(KeyCode.Keypad1 + i))
                {
                    if (i < Human.all.Count)
                    {
                        // ChuanSong(NetGame.instance.local.players[0].human, Human.all[j]);
                        UI_ChuanSong.ChuanSongZhi(0, i);
                    }
                }

            }
            //else// 数字 传送
            //{
            //    if ((Game.GetKeyDown(KeyCode.Alpha1 + i) || Game.GetKeyDown(KeyCode.Keypad1 + i)))//切换分身
            //    {
            //        //Change_Local_Cam(string.Concat(i + 1));//切换主体
            //        ChuanSong(NetGame.instance.local.players[0].human, Human.all[i]);
            //    }

            //}




        }
    }
    public static void ChuangJianFangJian()//快速创建房间
    {
        try
        {
            if (NetGame.isLocal)
            {
                if (App.state == AppSate.Startup)
                {
                    StartupExperienceController.instance.SkipStartupExperience(null);
                }
                Game.multiplayerLobbyLevel = Options.multiplayerLobbyLevelStore;
                WorkshopRepository.instance.SetLobbyTitle(Game.multiplayerLobbyLevel);
                NetGame.instance.transport.SetLobbyStatus(status: false);
                RatingMenu.instance.LoadInit();
                Dialogs.HideProgress();
                App.instance.HostGame();
                Debug.Log("创建房间");
                //Chat.TiShi(NetGame.instance.local , "快速创建房间");
                //Custom_Messages(0u, ColorfulSpeek.colorshow("创建房间"), "");
            }
        }
        catch
        {
        }
    }

    public static void Add_Local_Player(string txt) //加分身
    {
        if (string.IsNullOrEmpty(txt))
        {
            return;
        }
        string[] array = txt.Split(new char[]
        {
                ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length != 1)
        {
            return;
        }
        int num = 0;
        if (!int.TryParse(array[0], out num))
        {
            return;
        }
        checked
        {
            if (!NetGame.instance.local.players.Count.Equals(0))
            {
                //Shell.Print(string.Format("尝试添加{0}人，当前{1}人", num, NetGame.instance.local.players.Count));
                //NetChat.Print(string.Format("尝试添加{0}人，当前{1}人", num, NetGame.instance.local.players.Count));
                Chat.TiShi(NetGame.instance.local, $"尝试添加{num}人，当前{NetGame.instance.local.players.Count}人");
                for (int i = 0; i < num; i++)
                {
                    NetGame.instance.AddLocalPlayer();
                }
                return;
            }
            NetGame.instance.AddLocalPlayer();
            NetGame.instance.local.players[0].cameraController.gameCam.gameObject.SetActive(true);
            //Shell.Print(string.Format("尝试添加{0}人，当前玩家{1}人", num, NetGame.instance.local.players.Count));
            //NetChat.Print(string.Format("尝试添加{0}人，当前玩家{1}人", num, NetGame.instance.local.players.Count));
            Chat.TiShi(NetGame.instance.local, $"尝试添加{num}人，当前{NetGame.instance.local.players.Count}人");
        }
    }
    public static void Remove_Local_Player(string txt)//删除分身
    {
        if (NetGame.instance.local.players.Count < 2)
        {
            //NetChat.Print("分身数量为0,不能删除");
            Chat.TiShi(NetGame.instance.local, $"分身数量为0,不能删除");
            return;
        }
        checked
        {
            if (string.IsNullOrEmpty(txt))
            {
                //NetChat.Print("正在全部删除!");
                Chat.TiShi(NetGame.instance.local, $"正在全部删除!");
                for (int i = 1; i < NetGame.instance.local.players.Count; i++)
                {
                    try
                    {
                        NetGame.instance.RemoveLocalPlayer(NetGame.instance.local.players[i]);
                    }
                    catch
                    {
                    }
                }
                if (NetGame.instance.local.players.Count > 1)
                {
                    Remove_Local_Player(txt);
                }
                NetGame.instance.local.players[0].cameraController.gameCam.gameObject.SetActive(true);
                NetGame.instance.local.players[0].human.disableInput = false;
                return;
            }
            string[] array = txt.Split(new char[]
            {
                    ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 1)
            {
                return;
            }
            int num = 0;
            if (!int.TryParse(array[0], out num))
            {
                return;
            }
            if (num > NetGame.instance.local.players.Count)
            {
                //NetChat.Print("超出本地玩家当前数量!当前玩家数量为" + NetGame.instance.local.players.Count);
                Chat.TiShi(NetGame.instance.local, $"超出本地玩家当前数量!当前玩家数量为{NetGame.instance.local.players.Count}");
                return;
            }
            int count = NetGame.instance.local.players.Count;
            for (int j = count - num; j < count; j++)
            {
                NetGame.instance.RemoveLocalPlayer(NetGame.instance.local.players[j]);
            }
            if (NetGame.instance.local.players.Count != 0 && NetGame.instance.local.players.Count <= fenshen_cam + 1)
            {
                NetGame.instance.local.players[0].cameraController.gameCam.gameObject.SetActive(true);
                NetGame.instance.local.players[0].human.disableInput = false;
            }
            //("已经删除{0}人，当前玩家{1}人", num, NetGame.instance.local.players.Count));
            Chat.TiShi(NetGame.instance.local, $"已经删除{num}人，当前玩家{NetGame.instance.local.players.Count}人");
        }
    }
    public static void Change_Local_Cam(string txt)//切换分身
    {
        if (string.IsNullOrEmpty(txt))
        {
            return;
        }
        string[] array = txt.Split(new char[]
        {
                ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length != 1)
        {
            return;
        }
        int num = 0;
        if (!int.TryParse(array[0], out num))
        {
            return;
        }
        checked
        {
            if (num > 0 && num <= NetGame.instance.local.players.Count)
            {
                if (NetGame.instance.local.players.Count >= num)
                {
                    num--;
                    NetGame.instance.local.players[fenshen_cam].cameraController.gameCam.gameObject.SetActive(false);
                    //NetGame.instance.local.players[fenshen_cam].human.disableInput = true;
                    fenshen_cam = num;
                    NetGame.instance.local.players[num].cameraController.gameCam.gameObject.SetActive(true);
                    //NetGame.instance.local.players[num].human.disableInput = false;
                    //(" 转移主体到{0}号", num + 1)));
                    Chat.TiShi(NetGame.instance.local, $"转移主体到{num + 1}号分身");
                }
                return;
            }
            //("玩家编号错误!当前本地玩家总数", NetGame.instance.local.players.Count)));
            Chat.TiShi(NetGame.instance.local, $"分身编号错误!当前本地玩家总数{NetGame.instance.local.players.Count}");
        }
    }
    public static void Local_Control(string txt)
    {
        checked
        {
            if (!string.IsNullOrEmpty(txt))
            {
                string[] array = txt.Split(new char[]
                {
                        ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 1)
                {
                    //NetChat.Print(ColorfulSpeek.colorshow("格式错误!"));
                    Chat.TiShi(NetGame.instance.local, $"格式错误!");
                    return;
                }
                int num = 0;
                if (!int.TryParse(array[0], out num))
                {
                    //NetChat.Print(ColorfulSpeek.colorshow("格式错误!"));
                    Chat.TiShi(NetGame.instance.local, $"格式错误!");
                    return;
                }
                if (num < 0 || num > NetGame.instance.local.players.Count)
                {
                    //NetChat.Print(ColorfulSpeek.colorshow(Host_Func.A_0加A_1("错误玩家编号!当前玩家总数", NetGame.instance.local.players.Count)));
                    Chat.TiShi(NetGame.instance.local, $"分身编号错误!当前本地玩家总数{NetGame.instance.local.players.Count}");
                    return;
                }
                num--;
                if (NetGame.instance.local.players[num].human.disableInput)
                {
                    NetGame.instance.local.players[num].human.disableInput = false;
                    //NetChat.Print(ColorfulSpeek.colorshow(("{0}号玩家已恢复控制!", num + 1)));
                    Chat.TiShi(NetGame.instance.local, $"{num + 1}号分身已被控制!");
                    return;
                }
                NetGame.instance.local.players[num].human.disableInput = true;
                //NetChat.Print(ColorfulSpeek.colorshow(("{0}号分身已取消控制!", num + 1)));
                Chat.TiShi(NetGame.instance.local, $"{num + 1}号分身已取消控制!");
                return;
            }
            else
            {
                bool flag = false;
                for (int i = NetGame.instance.local.players.Count - 1; i >= 0; i--)
                {
                    if (NetGame.instance.local.players[i].human.disableInput)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    for (int j = NetGame.instance.local.players.Count - 1; j >= 0; j--)
                    {
                        NetGame.instance.local.players[j].human.disableInput = true;
                        //NetChat.Print(ColorfulSpeek.colorshow("已取消所有分身控制!"));
                        Chat.TiShi(NetGame.instance.local, $"取消所有分身控制!");
                    }
                    return;
                }
                for (int k = NetGame.instance.local.players.Count - 1; k >= 0; k--)
                {
                    NetGame.instance.local.players[k].human.disableInput = false;
                    //NetChat.Print(ColorfulSpeek.colorshow("已还原所有分身控制!"));
                    Chat.TiShi(NetGame.instance.local, $"控制所有分身!");
                }
                return;
            }
        }
    }//分身控制
    public static void ShangYiGuan()
    {
        int result = Game.instance.currentCheckpointNumber - 1;
        result = result >= 0 ? result : 0;
        if (GameSave.ProgressMoreOrEqual(Game.instance.currentLevelNumber, result) || Game.instance.workshopLevel != null)
        {
            Game.instance.RestartCheckpoint(result, 0);
        }
        //try
        //{
        //    int num2 = Game.instance.currentCheckpointNumber - 1;
        //    Game.instance.RestartCheckpoint(num2, 0);
        //    Shell.Print($"loading checkpoint {num2}");
        //}
        //catch
        //{
        //}
    }
    public static void XiaYiGuan()
    {
        int result = Game.instance.currentCheckpointNumber + 1;
        //Game.currentLevel.checkpoints.Length - 1
        result = result <= Game.currentLevel.checkpoints.Length - 1 ? result : Game.currentLevel.checkpoints.Length - 1;
        if (GameSave.ProgressMoreOrEqual(Game.instance.currentLevelNumber, result) || Game.instance.workshopLevel != null)
        {
            Game.instance.RestartCheckpoint(result, 0);
        }
        //try
        //{
        //    int num2 = Game.instance.currentCheckpointNumber + 1;
        //    Game.instance.RestartCheckpoint(num2, 0);
        //    Shell.Print($"loading checkpoint {num2}");
        //}
        //catch
        //{
        //}
    }
    public static void ChongZhiWuPin()
    {
        if (Game.currentLevel.respawnLocked)
        {
            return;
        }
        Game.instance.currentCheckpointNumber = Mathf.Min(Game.instance.currentCheckpointNumber, Game.currentLevel.checkpoints.Length - 1);
        //Game.instance.currentCheckpointSubObjectives = Game.instance.currentCheckpointSubObjectives;
        if (Game.currentLevel.prerespawn != null)
        {
            Game.currentLevel.prerespawn(Game.instance.currentCheckpointNumber, false);
        }
        Game.currentLevel.Reset(Game.instance.currentCheckpointNumber, Game.instance.currentCheckpointSubObjectives);
        if (NetGame.isServer)
        {
            NetSceneManager.ResetLevel(Game.instance.currentCheckpointNumber, Game.instance.currentCheckpointSubObjectives);
        }
        Game.currentLevel.BeginLevel();
        Game.instance.CheckpointLoaded(Game.instance.currentCheckpointNumber);
        SignalManager.EndReset();
        Game.currentLevel.PostEndReset(Game.instance.currentCheckpointNumber);

    }
    public static void BanShen(Human human)//半身不遂
    {
        if (human == null)
        {
            return;
        }
        if (human.banshen)
        {
            //human.grabManager.grabbedObjects.Add(Human.all[1].gameObject);
            human.ragdoll.partLeftHand.sensor.grabObject = null;
            human.ragdoll.partLeftHand.sensor.grabObject = human.ragdoll.partRightHand.sensor.gameObject;
        }
        else
        {
            human.ragdoll.partLeftHand.sensor.grabObject = null;
        }
    }
    public static void QuXiaoBanShen(Human human)
    {
        if (human == null)
        {
            return;
        }
        if (human.banshen && human.ragdoll.partLeftHand.sensor.grabObject != human.ragdoll.partRightHand.sensor.gameObject)
        {
            human.banshen=false;
        }
    }

    public static void WuJiaSi_Fun(Human human)//无假死
    {
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng.wujiasi_KaiGuan)
        {
            return;
        }
        //Debug.Log(human.state);
        if (human.wujiasi)
        {
            if (human.state == HumanState.Dead || human.state == HumanState.Spawning || human.state == HumanState.Unconscious)
            {
                //Debug.Log("222");
                human.state = HumanState.Idle;
            }
        }
    }
    public static void Up(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.up_KaiGuan)
        {
            Chat.TiShi(human.player.host, "up系统已关闭");
            return;
        }
        human.SetPosition(human.transform.position + 3f * Vector3.up);
        Chat.TiShi($"玩家 {human.player.host.name} 向上跳了3米");

    }
    public static void Ifg(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.ifg_KaiGuan)
        {
            Chat.TiShi(human.player.host, "ifg系统已关闭");
            return;
        }

        for (int i = 0; i < human.rigidbodies.Length; i++)
        {
            human.rigidbodies[i].useGravity = !human.rigidbodies[i].useGravity;
        }
    }
    //public static void JiHe(Human human)//全体集合至
    //{
    //    for (int i = 0; i < Human.all.Count; i++)
    //    {
    //        if (Human.all[i] != human)
    //        {
    //            Human.all[i].SetPosition(human.transform.position + new Vector3(0f, (float)i+1, 0f));
    //        }
    //    }
    //}

    public static void JiHe(Human centerHuman)
    {
        int count = Human.all.Count;
        float angleIncrement = 360f / count; // 计算每个单位的角度增量

        for (int i = 0; i < count; i++)
        {
            Human currentHuman = Human.all[i];

            if (currentHuman != centerHuman)
            {
                // 计算当前位置的角度
                float angle = i * angleIncrement;

                // 将角度转换为弧度
                float radians = angle * Mathf.Deg2Rad;

                // 计算新的位置（以centerHuman为中心）
                Vector3 newPosition = centerHuman.transform.position +
                                      new Vector3(Mathf.Cos(radians), 0.2f, Mathf.Sin(radians)) * 0.5f;

                // 设置新位置
                currentHuman.SetPosition(newPosition);
            }
        }
    }

    public static void WuPengZhuang_Fun(Human human)//无碰撞
    {
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng.wupengzhuang_KaiGuan)
        {
            human.wupengzhuang = false;
            //return;
        }

        foreach (Human human2 in Human.all)
        {
            if (human2 != human)
            {
                if (human.wupengzhuang || human2.wupengzhuang)
                {
                    IgnoreCollision.Ignore(human.transform, human2.transform);
                }
                else
                {
                    IgnoreCollision.Unignore(human.transform, human2.transform);
                }
            }
        }
    }
    public static void ZuoXia_Fun(Human human)//坐下
    {

        if (human.zuoxia)
        {
            human.zuoxiaTime -= Time.fixedDeltaTime;

            human.onGround = true;
            human.jump = false;
            Vector3 b = Quaternion.Euler(0f, 0f, 0f) * Vector3.down;
            human.ragdoll.partHips.rigidbody.AddForce(b * 1500f * 1f, ForceMode.Force);


            if (human.zuoxiaTime <= 0f)
            {
                human.zuoxia = false;
                human.yizuoxia = false;
                human.ragdoll.partBall.collider.enabled = true;
                human.zuoxiaTime = 0f;
            }
        }
    }
    public static void ZuoXia(Human human, bool guixia = false)
    {
        human.zuoxiaTime = 0.1f;////坐下的时间  3秒
        human.zuoxia = true;
        human.state = HumanState.Idle;
        human.ragdoll.partBall.collider.enabled = false;
        if (!human.yizuoxia)
        {
            human.yizuoxia = true;

            Vector3 a = Quaternion.Euler(0f, human.controls.cameraYawAngle, 0f) * Vector3.forward;
            //human.ragdoll.partLeftThigh.rigidbody.AddForce(a * 1000f * 0.1f, ForceMode.Force);
            //human.ragdoll.partRightThigh.rigidbody.AddForce(a * 1000f * 0.1f, ForceMode.Force);
            //human.ragdoll.partLeftLeg.rigidbody.AddForce(a * 1000f * 0.2f, ForceMode.Force);
            //human.ragdoll.partRightLeg.rigidbody.AddForce(a * 1000f * 0.2f, ForceMode.Force);

            human.ragdoll.partLeftFoot.rigidbody.AddForce(a * 1000f * 2f * (guixia ? -1 : 1), ForceMode.Force);//Impulse
            human.ragdoll.partRightFoot.rigidbody.AddForce(a * 1000f * 2f * (guixia ? -1 : 1), ForceMode.Force);

            human.ragdoll.partHips.rigidbody.AddForce(a * 1000f * -4f * (guixia ? -1 : 1), ForceMode.Force);
        }

        human.onGround = true;
        human.jump = false;
        Vector3 b = Quaternion.Euler(0f, 0f, 0f) * Vector3.down;
        human.ragdoll.partHips.rigidbody.AddForce(b * 1500f * 1f, ForceMode.Force);

    }
    public static void YiZiMa(Human human)
    {
        //human.yizima=!human.yizima;
        //if(human.yizima)
        //{
        human.yizima = true;
        human.ragdoll.partBall.collider.enabled = false;
        //return;
        //}
        //human.ragdoll.partBall.collider.enabled = true;
    }
    public static void YiZiMa_Fun(Human human)
    {
        if (human.yizima)
        {
            Vector3 forwardDirection = human.ragdoll.partHead.transform.right;//右
            Vector3 forwardDirection2 = human.ragdoll.partHead.transform.forward;//前

            human.ragdoll.partLeftHand.rigidbody.SafeAddForce(-forwardDirection * 1000f);
            human.ragdoll.partRightHand.rigidbody.SafeAddForce(forwardDirection * 1000f);

            human.ragdoll.partLeftFoot.rigidbody.SafeAddForce(-forwardDirection2 * 1500f);
            human.ragdoll.partRightFoot.rigidbody.SafeAddForce(forwardDirection2 * 1500f);


            human.yizima = false;
            human.ragdoll.partBall.collider.enabled = true;
            //human.ragdoll.partBall.collider.enabled = true;
            //human.ragdoll.partHips .rigidbody.SafeAddForce(-human.ragdoll.partHead.transform.up * 1500f);
        }
    }
    public static void TiTui(Human human)
    {
        if (!human.yititui)
        {
            human.yititui = true;
            human.titui = true;
            Vector3 forward = Quaternion.Euler(0f, human.controls.cameraYawAngle, 0f) * Vector3.forward;
            Vector3 right = Quaternion.Euler(0f, human.controls.cameraYawAngle, 0f) * Vector3.right;
            //human.ragdoll.partLeftThigh.rigidbody.AddForce(forward * 1000f * 1f, ForceMode.Force);//Impulse
            //human.ragdoll.partLeftLeg.rigidbody.AddForce(forward * 1000f * 5f, ForceMode.Force);//Impulse
            human.ragdoll.partLeftFoot.rigidbody.AddForce(forward * 1000f * 16f, ForceMode.Force);//Impulse
            human.ragdoll.partHips.rigidbody.AddForce(right * 1000f * 16f, ForceMode.Force);
            human.ragdoll.partHips.rigidbody.AddForce(right * 1000f * -16f, ForceMode.Force);
            human.ragdoll.partHips.rigidbody.AddForce(forward * 1000f * -10f, ForceMode.Force);
        }
    }
    public static void JiFei_Fun(Human human)//踢到
    {


        if (!human.titui || human == null)
        {
            //Debug.Log($"{human.titui}");
            return;
        }
        if (!UI_GongNeng.jifeixitong_KaiGuan)
        {
            //Chat.TiShi(human.player.host, "Y键击飞系统已关闭");
            return;
        }
        foreach (Rigidbody rigidbody in UnityEngine.Object.FindObjectsOfType(Type.GetTypeFromHandle(typeof(Rigidbody).TypeHandle)))
        {
            JiFei(human, rigidbody);
        }
        human.titui = false;

    }
    private static void JiFei(Human human, Rigidbody targetRigidbody)
    {

        // 获取踢腿部位的碰撞器
        Collider thighCollider = human.ragdoll.partLeftThigh.collider;
        Collider LegCollider = human.ragdoll.partLeftLeg.collider;
        Collider footCollider = human.ragdoll.partLeftFoot.collider;

        if (footCollider == null)
        {
            return;
        }

        // 获取目标刚体的所有碰撞器
        Collider[] targetColliders = targetRigidbody.GetComponents<Collider>();

        foreach (Collider targetCollider in targetColliders)
        {
            if (targetCollider.transform.name == "LightWoodMechLargeCatapult1")
            {
                return;
            }
            // 检测踢腿部位的碰撞器是否与目标刚体的碰撞器相交
            if (thighCollider.bounds.Intersects(targetCollider.bounds) || LegCollider.bounds.Intersects(targetCollider.bounds) || footCollider.bounds.Intersects(targetCollider.bounds))
            {
                // 如果碰撞器相交，执行踢击操作
                Human componentInParent = targetCollider.GetComponentInParent<Human>();
                if (componentInParent != null)
                {
                    if (componentInParent != human)
                    {

                        for (int j = 0; j < componentInParent.rigidbodies.Length; j++)
                        {
                            //componentInParent.ragdoll.partBall.rigidbody.AddForce(targetRigidbody.mass * (human.ragdoll.partHead.transform.forward * 100f + Vector3.up * 50f) * 16.5f / 1, ForceMode.Impulse);
                            componentInParent.rigidbodies[j].AddForce(componentInParent.rigidbodies[j].mass * (human.ragdoll.partHead.transform.forward * 10f + Vector3.up * 5f) * 16.5f / componentInParent.rigidbodies.Length, ForceMode.Impulse);
                        }
                        //Debug.Log($"{targetCollider.transform.name}");
                        break;
                    }
                }
                else
                {
                    //targetRigidbody = targetRigidbody.GetComponentInParent<Rigidbody>();
                    targetRigidbody.AddForce(targetRigidbody.mass * (human.ragdoll.partHead.transform.forward * 10f + Vector3.up * 5f) * 2f, ForceMode.Impulse);
                    //Debug.Log($"{targetCollider.transform.name}");
                    break;
                }
            }
        }
    }
    public static void enThrowing(Human human, bool flag)//去掉各个部位的碰撞器
    {
        human.ragdoll.partHead.collider.enabled = flag;
        human.ragdoll.partChest.collider.enabled = flag;
        human.ragdoll.partWaist.collider.enabled = flag;
        human.ragdoll.partLeftArm.collider.enabled = flag;
        human.ragdoll.partLeftForearm.collider.enabled = flag;
        human.ragdoll.partLeftHand.collider.enabled = flag;
        human.ragdoll.partLeftThigh.collider.enabled = flag;
        human.ragdoll.partLeftLeg.collider.enabled = flag;
        human.ragdoll.partLeftFoot.collider.enabled = flag;
        human.ragdoll.partRightArm.collider.enabled = flag;
        human.ragdoll.partRightForearm.collider.enabled = flag;
        human.ragdoll.partRightHand.collider.enabled = flag;
        human.ragdoll.partRightThigh.collider.enabled = flag;
        human.ragdoll.partRightLeg.collider.enabled = flag;
        human.ragdoll.partRightFoot.collider.enabled = flag;
        human.ragdoll.partBall.collider.enabled = flag;
        human.ragdoll.partHips.collider.enabled = flag;
        Collider[] componentsInChildren = human.GetComponentsInChildren<Collider>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (componentsInChildren[i].name.Equals("LeftHip"))
            {
                componentsInChildren[i].enabled = flag;
            }
            if (componentsInChildren[i].name.Equals("RightHip"))
            {
                componentsInChildren[i].enabled = flag;
            }
        }
    }
    public static void GuaJian_Fun(Human human)
    {
        if (human == null)
        { return; }

        if (!UI_GongNeng.guajianxitong_KaiGuan)
        {
            if (human.ntp || human.ntp_human != null)
            {
                human.ntp = false;

                foreach (Rigidbody rigidbody in human.rigidbodies)
                {
                    rigidbody.position += new Vector3(0f, 1f, 0f);
                }
                human.ntp_Offset = Vector3.zero;
                human.ntp_human = null;
                enThrowing(human, true);
            }
            return;
        }

        if (human.ntp)
        {
            if (human.controls.walkSpeed != 0f && human.ntp_Offset.sqrMagnitude < 25f)
            {
                human.ntp_Offset = human.ntp_Offset + human.controls.walkDirection * 0.1f;
                if (human.ntp_Offset.x > 3f)
                {
                    human.ntp_Offset.x = 3f;
                }
                else if (human.ntp_Offset.x < -3f)
                {
                    human.ntp_Offset.x = -3f;
                }
                if (human.ntp_Offset.z > 3f)
                {
                    human.ntp_Offset.z = 3f;
                }
                else if (human.ntp_Offset.z < -3f)
                {
                    human.ntp_Offset.z = -3f;
                }
            }
            if (human.controls.jump && human.ntp_Offset.y < 5f)
            {
                human.ntp_Offset.y = human.ntp_Offset.y + 0.1f;
            }
            if (human.ntp_Offset.y > 0f && human.controls.unconscious)
            {
                human.ntp_Offset.y = human.ntp_Offset.y - 0.1f;
                if (human.ntp_Offset.y < 0f)
                {
                    human.ntp_Offset.y = 0f;
                }
            }
            if (human.controls.shootingFirework)
            {
                human.ntp_Offset = Vector3.zero;
            }
            if (human.ntp_human != null)
            {
                for (int l = 0; l < human.rigidbodies.Length; l++)
                {
                    human.rigidbodies[l].rotation = human.ntp_human.rigidbodies[l].rotation;
                    Vector3 position = human.ntp_human.rigidbodies[l].position + human.ntp_Offset;
                    position.y += 2f;
                    human.rigidbodies[l].position = position;
                    human.rigidbodies[l].velocity = human.ntp_human.rigidbodies[l].velocity;
                }
                return;
            }
            else
            {
                QuXiaoGuaJian(human);
            }

        }
    }
    public static void SetGuaJian(Human human, Human ntphuman)//当human的头部挂件
    {
        if (human == null || ntphuman == null)
        {
            return;
        }
        if (human == ntphuman)
        {
            Chat.TiShi(ntphuman.player.host, "不能当自己的头部挂件");
            return;
        }

        if (ntphuman.ntp)
        {
            if (ntphuman.ntp_human == human)
            {
                QuXiaoGuaJian(ntphuman);//如果已经是他的挂件了就取消
                return;
            }
            if (human.ntp && human.ntp_human == ntphuman)
            {
                human.ntp_human = ntphuman.ntp_human;
            }
        }
        else if (!ntphuman.ntp)
        {
            if (human.ntp && human.ntp_human == ntphuman)
            {
                human.ntp = false;
                human.ntp_human = null;
                enThrowing(human, true);
            }
        }

        ntphuman.ntp = true;
        ntphuman.ntp_human = human;
        enThrowing(ntphuman, false);
        Chat.TiShi($"玩家 {ntphuman.player.host.name} 悬浮到了 {human.player.host.name} 的头上");
        Chat.TiShi(ntphuman.player.host, $"您成为了 {human.player.host.name} 的头部挂件，跳跃上升，按Y下降，可前后左右移动调整位置。");//，跳跃上升，按Y下降，可前后左右移动调整位置。
    }
    public static void QuXiaoGuaJian(Human human)//human放弃当头部挂件
    {
        if (human == null)
        {
            return;
        }
        if (human.ntp || human.ntp_human != null)
        {
            human.ntp = false;

            foreach (Rigidbody rigidbody in human.rigidbodies)
            {
                rigidbody.position += new Vector3(0f, 1f, 0f);
            }
            human.ntp_Offset = Vector3.zero;
            human.ntp_human = null;
            enThrowing(human, true);
            //Chat.TiShi($"{human.player.host.name} 放弃了 当头部挂件");
        }
    }
    public static Human ChaHumanId(string txt)//查 Human
    {
        try
        {
            if (string.IsNullOrEmpty(txt))
            {
                return null;
            }
            string[] array = txt.Split(new char[]
            {
            ' '
            }, StringSplitOptions.RemoveEmptyEntries);
            if (array.Length != 1)
            {
                return null;
            }
            int num = 0;
            if (int.TryParse(array[0], out num))
            {
                if (num == 1)
                {
                    return NetGame.instance.server.players[NetGame.instance.server.players.Count - 1].human;
                }
                if (num - 2 < NetGame.instance.readyclients.Count)
                {
                    return NetGame.instance.readyclients[num - 2].players[NetGame.instance.readyclients[num - 2].players.Count - 1].human;
                }
            }
            Chat.TiShi(NetGame.instance.local, $"{num}号玩家不存在!");
            return null;
        }
        catch
        {
            //Debug.Log("ChaHumanId 出错了");
            return null;
        }
    }
    public static void ChuanSong(Human human1, Human human2)//human1 传送到 human2
    {
        human1.SetPosition(human2.transform.position + new Vector3(0.5f, 0.2f, 0f));
        Chat.TiShi($"玩家 {human1.player.host.name} 传送到了 {human2.player.host.name} 的身边");
    }
    
    public static void ShanXian_Fun(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng. shanxianxitong_KaiGuan)
        {
            human.shanxian = false;
            return;
        }
        if (human.shanxian)
        {

            if (human.controls.walkSpeed > 0f)
            {
                human.onGround = true;
                human.state = HumanState.Walk;
            }
            else
            {
                human.onGround = true;
                human.state = HumanState.Idle;
            }
            if (human.controls.jump)
            {
                human.state = HumanState.Jump;
                human.onGround = true;
                human.jump = true;
            }

            //float walkForward = human.player.walkForward;
            //float walkRight = human.player.walkRight;
            //bool jump = human.player.jump;
            //bool shooting = human.player.shooting;
            bool playDead = human.player.playDead;
            if (human.controls.walkSpeed != 0f)
            {
                human.SetPosition(human.transform.position + human.player.controls.walkDirection * 0.3f + new Vector3(0f, 0.01f, 0f));
            }
            if (playDead)
            {
                human.SetPosition(human.transform.position + new Vector3(0f, -0.15f, 0f));
            }
        }
    }
    public static void FeiTian_Fun(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.feitianxitong_KaiGuan)
        {
            if (human.feitian)
            {
                Rigidbody[] rigidbodies = human.rigidbodies;
                for (int i = 0; i < rigidbodies.Length; i++)
                {
                    rigidbodies[i].useGravity = true;
                }
                human.feitian = false;
                human.Fly = false;
                human.ResetDrag();
                enThrowing(human, true);
            }
            return;
        }

        if (human.Fly)
        {
            if (human.controls.walkSpeed > 0f)
            {
                human.onGround = true;
                human.state = HumanState.Walk;
            }
            else
            {
                human.onGround = true;
                human.state = HumanState.Idle;
            }
            if (human.controls.jump)
            {
                human.state = HumanState.Jump;
                human.onGround = true;
                human.jump = true;
            }
        }
        if (human.feitian)
        {
            if (human.flowing && human.controls.jump)
            {
                if (human.GetComponent<GrabManager>().grabbedObjects.Count == 0)
                {
                    for (int i = 0; i < human.rigidbodies.Length; i++)
                    {
                        human.rigidbodies[i].AddForce(Vector3.up * 45f, ForceMode.Force);
                        human.rigidbodies[i].drag = 4f;
                        if (human.controls.walkSpeed != 0f)
                        {
                            human.rigidbodies[i].AddForce(human.controls.walkDirection * 300f / 1.75f, ForceMode.Force);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < human.rigidbodies.Length; j++)
                    {
                        human.rigidbodies[j].AddForce(Vector3.up * 300f, ForceMode.Force);
                        human.rigidbodies[j].drag = 4f;
                        if (human.controls.walkSpeed != 0f)
                        {
                            human.rigidbodies[j].AddForce(human.controls.walkDirection * 300f / 1.75f, ForceMode.Force);
                        }
                    }
                }
            }
            if (human.controls.walkSpeed == 0f)
            {
                if (human.flowing)
                {
                    human.state = HumanState.Idle;
                }
                return;
            }
            if (!human.controls.leftGrab && !human.controls.rightGrab)
            {
                if (!human.flowing)
                {
                    human.extend_time_rush = 0f;
                    return;
                }
                if (human.rush)
                {
                    human.rush = false;
                    enThrowing(human, true);
                }
                if (human.GetComponent<GroundManager>().onGround)
                {
                    human.flowing = false;
                    human.Fly = true;
                    enThrowing(human, true);
                    if (!human.rigidbodies[0].useGravity)
                    {
                        for (int k = 0; k < human.rigidbodies.Length; k++)
                        {
                            human.rigidbodies[k].useGravity = true;
                        }
                        human.ResetDrag();
                    }
                    return;
                }
                for (int l = 0; l < human.rigidbodies.Length; l++)
                {
                    human.rigidbodies[l].drag = 4f;
                    human.rigidbodies[l].AddForce(human.controls.walkDirection * human.flowing_speed / 1.75f, ForceMode.Force);
                }
                human.state = HumanState.Idle;
                return;
            }
            else if (human.GetComponent<GrabManager>().grabbedObjects.Count == 0)
            {
                if (!human.rush)
                {
                    human.extend_time_rush += Time.fixedDeltaTime;
                    if (human.extend_time_rush > 2f)
                    {
                        human.rush = true;
                        human.flowing = true;
                        human.extend_time_rush = 0f;
                        human.Fly = false;
                        enThrowing(human, false);
                        if (human.rigidbodies[0].useGravity)
                        {
                            for (int m = 0; m < human.rigidbodies.Length; m++)
                            {
                                human.rigidbodies[m].drag = 4f;
                                human.rigidbodies[m].useGravity = false;
                            }
                        }
                    }
                }
                if (human.flowing)
                {
                    if (human.rush)
                    {
                        human.ReleaseGrab(1f);
                    }
                    Vector3 vector = human.targetDirection * human.flowing_speed;
                    human.ragdoll.partBall.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partChest.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partHead.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partHips.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partLeftArm.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partLeftForearm.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partLeftHand.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partLeftThigh.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partRightArm.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partRightForearm.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partRightThigh.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partWaist.rigidbody.AddForce(vector, ForceMode.Force);
                    human.ragdoll.partBall.rigidbody.drag = 4f;
                    human.ragdoll.partChest.rigidbody.drag = 4f;
                    human.ragdoll.partHead.rigidbody.drag = 4f;
                    human.ragdoll.partHips.rigidbody.drag = 4f;
                    human.ragdoll.partLeftArm.rigidbody.drag = 4f;
                    human.ragdoll.partLeftForearm.rigidbody.drag = 4f;
                    human.ragdoll.partLeftHand.rigidbody.drag = 4f;
                    human.ragdoll.partLeftThigh.rigidbody.drag = 4f;
                    human.ragdoll.partRightArm.rigidbody.drag = 4f;
                    human.ragdoll.partRightForearm.rigidbody.drag = 4f;
                    human.ragdoll.partRightThigh.rigidbody.drag = 4f;
                    human.ragdoll.partWaist.rigidbody.drag = 4f;
                    human.state = HumanState.Fall;
                }
                return;
            }
        }
    }
    public static void SetFeiTian(Human human)
    {
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng.feitianxitong_KaiGuan)
        {
            //Chat.TiShi(human.player.host, $"飞天系统已关闭");
            return;
        }
        //human.supper_man = !human.supper_man;
        if (human.feitian)
        {
            human.flowing_speed = 300f;
            human.Fly = true;
            return;
        }
        Rigidbody[] rigidbodies = human.rigidbodies;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].useGravity = true;
        }
        human.Fly = false;
        human.ResetDrag();
        enThrowing(human, true);
    }
    public static void enShan(Human human, bool shan)
    {
        human.ragdoll.partBall.collider.enabled = !shan;
        human.ragdoll.partChest.collider.enabled = !shan;
        human.ragdoll.partHead.collider.enabled = !shan;
        human.ragdoll.partHips.collider.enabled = !shan;
        human.ragdoll.partLeftArm.collider.enabled = !shan;
        human.ragdoll.partLeftFoot.collider.enabled = !shan;
        human.ragdoll.partLeftForearm.collider.enabled = !shan;
        human.ragdoll.partLeftHand.collider.enabled = !shan;
        human.ragdoll.partLeftLeg.collider.enabled = !shan;
        human.ragdoll.partLeftThigh.collider.enabled = !shan;
        human.ragdoll.partRightArm.collider.enabled = !shan;
        human.ragdoll.partRightFoot.collider.enabled = !shan;
        human.ragdoll.partRightForearm.collider.enabled = !shan;
        human.ragdoll.partRightHand.collider.enabled = !shan;
        human.ragdoll.partRightLeg.collider.enabled = !shan;
        human.ragdoll.partRightThigh.collider.enabled = !shan;
        human.ragdoll.partWaist.collider.enabled = !shan;
        Collider[] componentsInChildren = human.GetComponentsInChildren<Collider>();
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            if (componentsInChildren[i].name.Equals("LeftHip"))
            {
                componentsInChildren[i].enabled = !shan;
            }
            if (componentsInChildren[i].name.Equals("RightHip"))
            {
                componentsInChildren[i].enabled = !shan;
            }
        }
        if (shan)
        {
            Rigidbody[] rigidbodies = human.rigidbodies;
            for (int j = 0; j < rigidbodies.Length; j++)
            {
                rigidbodies[j].drag = 4f;
            }
            return;
        }
        human.overridenDrag = false;
        human.ResetDrag();
    }
    public static void ChaoRen_Fun(Human human)//咸蛋超人
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.feitianxitong_KaiGuan)
        {
            human.chaoren = false;
            return;
        }
        if (human.chaoren)
        {
            if (human.controls.walkSpeed != 0f && human.controls.walkLocalDirection.z >= 0f && human.controls.walkLocalDirection.x == 0f)
            {
                if (human.enshan)
                {
                    human.enshan = true;
                    enShan(human, true);
                    Rigidbody[] rigidbodies = human.rigidbodies;
                    for (int i = 0; i < rigidbodies.Length; i++)
                    {
                        rigidbodies[i].useGravity = false;
                    }
                    for (int j = 0; j < rigidbodies.Length; j++)
                    {
                        rigidbodies[j].velocity = Vector3.zero;
                    }
                }
                Vector3 b = human.ragdoll.partLeftHand.rigidbody.transform.position;
                Vector3 b2 = human.ragdoll.partRightHand.rigidbody.transform.position;
                human.ragdoll.partLeftFoot.rigidbody.AddForce(Human_Looking_Direction(human) * -600f, ForceMode.Force);
                human.ragdoll.partRightHand.rigidbody.AddForce(Human_Looking_Direction(human) * 600f, ForceMode.Force);
                human.ragdoll.partLeftHand.rigidbody.AddForce(Human_Looking_Direction(human) * 600f, ForceMode.Force);
                human.ragdoll.partHead.rigidbody.AddForce(Human_Looking_Direction(human) * 600f, ForceMode.Force);
                human.ragdoll.partRightFoot.rigidbody.AddForce(Human_Looking_Direction(human) * -600f, ForceMode.Force);
                human.state = HumanState.Idle;
                human.SetPosition(human.transform.position + ((human.controls.leftGrab && human.controls.rightGrab) ? 100f : 20f) * Time.fixedDeltaTime * (Human_Looking_Direction(human) + (human.controls.jump ? (2f * Vector3.up) : Vector3.zero)));
                human.ragdoll.partLeftFoot.rigidbody.velocity = (((human.ragdoll.partLeftFoot.rigidbody.velocity / 2f).magnitude > 5f) ? (human.ragdoll.partLeftFoot.rigidbody.velocity / 2f) : Vector3.zero);
                human.ragdoll.partRightFoot.rigidbody.velocity = (((human.ragdoll.partRightFoot.rigidbody.velocity / 2f).magnitude > 5f) ? (human.ragdoll.partRightFoot.rigidbody.velocity / 2f) : Vector3.zero);
                Human human2 = null;
                Rigidbody rigidbody = null;
                if (human.ragdoll.partLeftHand.sensor.grabObject != null)
                {
                    human2 = human.ragdoll.partLeftHand.sensor.grabObject.GetComponentInParent<Human>();
                    rigidbody = human.ragdoll.partLeftHand.sensor.grabObject.GetComponentInParent<Rigidbody>();
                    if (human2 != null)
                    {
                        Vector3 b3 = human.ragdoll.partLeftHand.rigidbody.transform.position - b;
                        human2.SetPosition(human2.transform.position + b3);
                    }
                    else if (rigidbody != null)
                    {
                        Vector3 b4 = human.ragdoll.partLeftHand.rigidbody.transform.position - b;
                        human.ragdoll.partLeftHand.sensor.grabObject.transform.position = human.ragdoll.partLeftHand.sensor.grabObject.transform.position + b4;
                    }
                }
                if (human.ragdoll.partRightHand.sensor.grabObject != null)
                {
                    Human component = human.ragdoll.partRightHand.sensor.grabObject.GetComponent<Human>();
                    Rigidbody component2 = human.ragdoll.partRightHand.sensor.grabObject.GetComponent<Rigidbody>();
                    if (component != null && (human2 == null || !human2.Equals(component)))
                    {
                        Vector3 b5 = human.ragdoll.partRightHand.rigidbody.transform.position - b2;
                        component.SetPosition(component.transform.position + b5);
                        return;
                    }
                    if (component == null && component2 != null && (rigidbody == null || !component2.Equals(rigidbody)))
                    {
                        Vector3 b6 = human.ragdoll.partRightHand.rigidbody.transform.position - b2;
                        human.ragdoll.partRightHand.sensor.grabObject.transform.position = human.ragdoll.partRightHand.sensor.grabObject.transform.position + b6;
                        return;
                    }
                }
            }
            else if (!human.enshan)
            {
                human.enshan = false;
                enShan(human, false);
                Rigidbody[] rigidbodies2 = human.rigidbodies;
                for (int k = 0; k < rigidbodies2.Length; k++)
                {
                    rigidbodies2[k].useGravity = true;
                }
            }
        }
    }
    public static void BengDi(Human human)
    {
        //human.bengdi = !human.bengdi;
        if (human.bengdi)
        {
            Rigidbody[] rigidbodies = human.rigidbodies;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                Collider component = rigidbodies[i].gameObject.GetComponent<Collider>();
                if (component != null)
                {
                    component.material.bounceCombine = PhysicMaterialCombine.Minimum;
                    component.material.frictionCombine = PhysicMaterialCombine.Average;
                    component.material.staticFriction = 0f;
                    component.material.dynamicFriction = 0f;
                    component.material.bounceCombine = PhysicMaterialCombine.Maximum;
                    component.material.bounciness = 1f;
                    component.material.frictionCombine = PhysicMaterialCombine.Minimum;
                    component.sharedMaterial.staticFriction = 0f;
                    component.sharedMaterial.dynamicFriction = 0f;
                }
            }

            return;
        }
        Rigidbody[] rigidbodies2 = human.rigidbodies;
        for (int j = 0; j < rigidbodies2.Length; j++)
        {
            Collider component2 = rigidbodies2[j].gameObject.GetComponent<Collider>();
            if (component2 != null)
            {
                component2.material.staticFriction = 0.5f;
                component2.material.dynamicFriction = 0.5f;
                component2.material.bounciness = 0f;
                component2.sharedMaterial.staticFriction = 0.5f;
                component2.sharedMaterial.dynamicFriction = 0.5f;
            }
        }

    }//蹦迪
    public static void DongJie(Human human)//冻结
    {
        Rigidbody[] rigidbodies = human.rigidbodies;
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (human.dongjie)
            {
                rigidbody.isKinematic = true;
            }
            else
            {
                rigidbody.isKinematic = false;
            }
        }
    }
    public static void DianTun_Fun(Human human)//电臀
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.diantun)
        {
            human.diantun_i++;
            if (human.diantun_i > 7)
            {
                human.diantun_i = 0;
                human.Diantun_Mod = !human.Diantun_Mod;
            }
            //human.ReleaseGrab(5f);

            Vector3 a = Quaternion.Euler(human.controls.targetPitchAngle, human.controls.targetYawAngle, 0f) * Vector3.forward;
            Vector3 b = Quaternion.Euler(Mathf.Clamp(human.controls.targetPitchAngle, -70f, 80f), human.controls.targetYawAngle, 0f) * Vector3.forward;
            if (human.Diantun_Mod)
            {
                human.ragdoll.partHips.rigidbody.AddForce((a + b) * 350f, ForceMode.Force);
            }
            else
            {
                human.ragdoll.partHips.rigidbody.AddForce((a + b) * -350f / 2f, ForceMode.Force);
            }
        }
    }
    public static void DianTun(Human human)
    {

        //human.diantun = !human.diantun;
        if (human.diantun)
        {
            human.ragdoll.partHead.rigidbody.drag = 2000000f;
            //"向您发起多人运动邀请"
            return;
        }

        human.ResetDrag();

        //"玩累了多人运动"
    }
    public static void QiQiu_Fun(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.qiqiu)
        {
            Human grabbedByHuman = human.grabbedByHuman;
            if (grabbedByHuman != null)
            {
                if (grabbedByHuman.ragdoll.partLeftHand.sensor.grabBody != null)
                {
                    grabbedByHuman.ragdoll.partLeftHand.sensor.grabBody.AddForce(Vector3.up.normalized * 700f + grabbedByHuman.controls.walkSpeed * 350f * grabbedByHuman.controls.walkDirection, ForceMode.Force);
                }
                if (grabbedByHuman.ragdoll.partRightHand.sensor.grabBody != null)
                {
                    grabbedByHuman.ragdoll.partRightHand.sensor.grabBody.AddForce(Vector3.up.normalized * 700f + grabbedByHuman.controls.walkSpeed * 350f * grabbedByHuman.controls.walkDirection, ForceMode.Force);
                }
            }
        }
    }
    public static void QiQiu(Human human)
    {
        //human.qiqiu = !human.qiqiu;
        if (human.qiqiu)
        {
            Rigidbody[] rigidbodies = human.rigidbodies;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].useGravity = false;
                rigidbodies[i].drag = 4f;
            }
            return;
        }

        Rigidbody[] rigidbodies2 = human.rigidbodies;
        for (int j = 0; j < rigidbodies2.Length; j++)
        {
            rigidbodies2[j].useGravity = true;
        }
        human.ResetDrag();
    }
    public static void QiQiuXiFa_Fun(Human human)//抓物飞
    {
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.qiqiuxifa)
        {
            float num = 0f;
            for (int j = 0; j < human.rigidbodies.Length; j++)
            {
                num += human.rigidbodies[j].mass;
            }
            List<GameObject> grabbedObjects = GetGrabManager(human).grabbedObjects;
            for (int k = 0; k < grabbedObjects.Count; k++)
            {
                Rigidbody component = grabbedObjects[k].GetComponent<Rigidbody>();
                if (component != null)
                {
                    Vector3 force = -1f * (component.mass + num + 1f) * Physics.gravity;
                    component.SafeAddForce(force, ForceMode.Force);
                }
            }
        }
    }
    public static void KeTouGuai_Fun(Human human)//磕头怪
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.ketouguai)
        {
            human.ragdoll.partHead.transform.localEulerAngles = new Vector3(180f, 0f, 0f) * Time.time * 10f;
        }
    }
    public static void ToggleSuperJump()
    {
        YxMod.SuperJumpEnabled = !YxMod.SuperJumpEnabled;
        Debug.Log($"超级跳已{(YxMod.SuperJumpEnabled ? "启用" : "禁用")}");
    }
    public static void chaojitiao(Human human)//超级跳
    {
        if (human == null || !UI_GongNeng.yulexitong_KaiGuan)
            return;

        // 检查是否在地面上且按下了跳跃键
        bool isGrounded = human.groundDelay <= 0f && human.onGround;
        bool canJump = human.jumpDelay <= 0f;

        // 允许按住跳跃键连续跳跃
        if (isGrounded && canJump && human.controls.jump)
        {
            // 计算达到5米高度需要的初速度
            float jumpForce = Mathf.Sqrt(2f * 9.81f * 5f);

            // 为所有刚体设置向上的速度
            foreach (Rigidbody rb in human.rigidbodies)
            {
                // 先清除垂直速度，保持水平速度
                Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.velocity = horizontalVelocity + Vector3.up * jumpForce;
            }

            // 设置较短的跳跃延迟，允许快速连续跳跃
            human.jumpDelay = 0.1f;
            human.groundDelay = 0.1f;
            human.state = HumanState.Jump;
        }
    }

    private static bool jumpKeyPressedLastFrame = false;


    private static Vector3 GetVerticalDirAccurate(Vector3 _dir)
    {
        return RotateRound(_dir, Vector3.zero, Vector3.up, 90f).normalized;
    }
    private static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * (position - center) + center;
    }
    private static float ComputeRotateValue(Human human, Rigidbody rigidbody)
    {
        float num = 1f - rigidbody.velocity.magnitude / 40f;
        return Vector3.Distance(human.ragdoll.partBall.rigidbody.transform.position, rigidbody.transform.position) * num;
    }
    private static Vector3 ComputeShrinkValue(Human human, Rigidbody rigidbody)
    {
        float d = Math.Max(0f, rigidbody.velocity.magnitude - 2f);
        return 50f * (human.ragdoll.partBall.rigidbody.transform.position - rigidbody.transform.position).normalized * d;
    }
    public static void ZhuanQuan_Fun(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (!human.zhuanquan)
        {
            return;
        }
        if (human.controls.jump)
        {
            float magnitude = (human.ragdoll.partHead.transform.position - human.ragdoll.partHips.transform.position).magnitude;
            Vector3 normalized = (human.ragdoll.partHead.transform.position - human.ragdoll.partHips.transform.position).normalized;
            Vector3 verticalDirAccurate = GetVerticalDirAccurate(new Vector3(normalized.x, 0f, normalized.z).normalized);
            Vector3 a = 200f * verticalDirAccurate / magnitude;
            human.controls.leftGrab = true;
            human.controls.rightGrab = true;
            human.ragdoll.partChest.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partChest.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partChest.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partChest.rigidbody), ForceMode.Force);
            human.ragdoll.partHead.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partHead.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partHead.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partHead.rigidbody), ForceMode.Force);
            human.ragdoll.partHips.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partHips.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partHips.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partHips.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftArm.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partLeftArm.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftArm.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftArm.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftFoot.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partLeftFoot.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftFoot.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftFoot.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftForearm.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partLeftForearm.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftForearm.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftForearm.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftHand.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partLeftHand.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftHand.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftHand.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftLeg.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partLeftLeg.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftLeg.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftLeg.rigidbody), ForceMode.Force);
            human.ragdoll.partLeftThigh.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partLeftThigh.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partLeftThigh.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partLeftThigh.rigidbody), ForceMode.Force);
            human.ragdoll.partRightArm.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partRightArm.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightArm.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightArm.rigidbody), ForceMode.Force);
            human.ragdoll.partRightFoot.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partRightFoot.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightFoot.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightFoot.rigidbody), ForceMode.Force);
            human.ragdoll.partRightForearm.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partRightForearm.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightForearm.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightForearm.rigidbody), ForceMode.Force);
            human.ragdoll.partRightHand.rigidbody.AddForce(ComputeRotateValue(human, human.ragdoll.partRightHand.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightHand.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightHand.rigidbody), ForceMode.Force);
            human.ragdoll.partRightLeg.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partRightLeg.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightLeg.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightLeg.rigidbody), ForceMode.Force);
            human.ragdoll.partRightThigh.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partRightThigh.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partRightThigh.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partRightThigh.rigidbody), ForceMode.Force);
            human.ragdoll.partWaist.rigidbody.AddForce(-1f * ComputeRotateValue(human, human.ragdoll.partWaist.rigidbody) * a, ForceMode.Force);
            human.ragdoll.partWaist.rigidbody.AddForce(ComputeShrinkValue(human, human.ragdoll.partWaist.rigidbody), ForceMode.Force);
        }
    }
    public static void TuoLuo_Fun(Human human)//陀螺
    {
        if (human.tuoluo)
        {
            human.transform.RotateAround(human.transform.position, Vector3.up, Time.time * 1f);
        }
    }
    public static void QianShui_Fun(Human human)//潜水
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (!human.qianshui)
        {
            return;
        }
        WaterBody waterBody = human.GetComponentInChildren<HumanHead>().waterBody;
        if (waterBody == null)
        {
            return;
        }
        Vector3 vector;
        float num = waterBody.SampleDepth(human.ragdoll.partHead.rigidbody.transform.position, out vector);
        if (num > 0f)
        {
            vector = Vector3.up * 70f * num;
            if (human.controls.walkSpeed != 0f && (human.controls.leftGrab || human.controls.rightGrab))
            {
                vector -= Vector3.up * 40f * num;
                vector += human.targetDirection * 40f;
            }
            human.ragdoll.partBall.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partChest.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partHead.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partHips.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partLeftArm.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partLeftForearm.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partLeftHand.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partLeftThigh.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partRightArm.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partRightForearm.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partRightThigh.rigidbody.AddForce(vector, ForceMode.Force);
            human.ragdoll.partWaist.rigidbody.AddForce(vector, ForceMode.Force);
        }
    }
    public static void ChaiChu(Human human)//拆除
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (!human.chaichu)
        {
            return;
        }
        //GameObject grabObject = NetGame.instance.local.players[0].human.ragdoll.partLeftHand.sensor.grabObject;
        GameObject grabObject = human.ragdoll.partLeftHand.sensor.grabObject;
        if (grabObject == null)
        {
            return;
        }
        Transform transform = grabObject.transform.parent;
        if (transform == null)
        {
            return;
        }
        float num = 1000f;
        if (transform == Game.currentLevel.transform || transform.childCount > 200)
        {
            num = 20f;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform transform2 = transform.GetChild(i);
            if (transform2.GetComponentInParent<Human>() == null && transform2.GetComponentInChildren<Human>() == null && transform2.GetComponent<Collider>() != null && (transform2.GetComponent<MeshCollider>() == null || (transform2.GetComponent<MeshCollider>() != null && transform2.GetComponent<MeshCollider>().bounds.size.x * transform2.GetComponent<MeshCollider>().bounds.size.z < 50f)) && Vector3.Distance(transform2.position, grabObject.transform.position) < num)
            {
                if (transform2.GetComponent<Rigidbody>() == null)
                {
                    transform2.gameObject.AddComponent<Rigidbody>();
                }
                if (transform2.GetComponent<NetBody>() != null)
                {
                    transform2.GetComponent<NetBody>().respawn = false;
                }
                Joint[] componentsInChildren = transform2.GetComponentsInChildren<Joint>();
                for (int j = 0; j < componentsInChildren.Length; j++)
                {
                    componentsInChildren[j].breakForce = 0f;
                }
                transform2.parent = Game.currentLevel.transform;
            }
        }
    }
    private static void ShootRigidbody(Human human, Rigidbody rigidbody) //射击
    {
        float num = Vector3.Distance(rigidbody.transform.position, human.transform.position);
        if (num > 0.3f && num < 50f)
        {
            Vector3 a = Quaternion.Euler(human.controls.targetPitchAngle, human.controls.targetYawAngle, 0f) * Vector3.forward;
            Vector3 vector = human.ragdoll.partLeftHand.transform.position - human.ragdoll.partLeftArm.transform.position;
            Vector3 rhs = rigidbody.transform.position - human.ragdoll.partLeftArm.transform.position;
            Human componentInParent = rigidbody.GetComponentInParent<Human>();
            if (componentInParent != null)
            {
                bool flag;
                flag = (componentInParent != human && Vector3.Dot(vector, rhs) > 0.98f * vector.magnitude * rhs.magnitude);

                if (flag)
                {
                    human.ragdoll.partLeftArm.rigidbody.AddForce(-30f * a / num, ForceMode.Impulse);
                    human.ragdoll.partChest.rigidbody.AddForce(-15f * a / num, ForceMode.Impulse);

                    rigidbody.AddForce(rigidbody.mass * (vector * 10f + Vector3.up * 5f) * 16.5f / num, ForceMode.Impulse);
                    return;
                }
            }
            else if (Vector3.Dot(vector, rhs) > 0.965f * vector.magnitude * rhs.magnitude)
            {
                human.ragdoll.partLeftArm.rigidbody.AddForce(-140f * a / num, ForceMode.Impulse);
                human.ragdoll.partChest.rigidbody.AddForce(-70f * a / num, ForceMode.Impulse);
                rigidbody.AddForce(rigidbody.mass * (vector * 100f + Vector3.up * 50f) * 5f / num, ForceMode.Impulse);
            }
        }
    }
    public static void ShootRigidbody(Human human)//射击
    {
        foreach (Rigidbody rigidbody in UnityEngine.Object.FindObjectsOfType(Type.GetTypeFromHandle(typeof(Rigidbody).TypeHandle)))
        {

            ShootRigidbody(human, rigidbody);
        }
    }

    public static void QuXiaoQianShou(Human human)
    {
        //if(human.qianshou_zuo)
        //{
        //    if (human.qianshou_zuo_humanHand == human.qianshou_zuo_human.ragdoll.partLeftHand)
        //    {
        //        //Debug.Log("左牵左");
        //        human.qianshou_zuo_human.beiqianshou_zuo = false;
        //    }
        //    else if (human.qianshou_zuo_humanHand == human.qianshou_zuo_human.ragdoll.partRightHand)
        //    {
        //        //Debug.Log("左牵右");
        //        human.qianshou_zuo_human.beiqianshou_you = false;
        //    }
        //}
        //if(human.qianshou_you)
        //{
        //    if (human.qianshou_you_humanHand == human.qianshou_you_human.ragdoll.partLeftHand)
        //    {
        //        //Debug.Log("右牵左");
        //        human.qianshou_you_human.beiqianshou_zuo = false;
        //    }
        //    else if (human.qianshou_you_humanHand == human.qianshou_you_human.ragdoll.partRightHand)
        //    {
        //        //Debug.Log("右牵右");
        //        human.qianshou_you_human.beiqianshou_you = false;
        //    }
        //}

        human.qianshou_zuo = human.qianshou_you = false;
        ////human.beiqianshou_zuo = human.beiqianshou_you = false;
        //human.qianshou_zuo_human = human.qianshou_you_human = null;
        //human.qianshou_zuo_humanHand = human.qianshou_you_humanHand = null;
    }
    private static bool ZuoShou(Human human)//左手是否空着
    {
        return (!human.qianshou_zuo);// && !human.beiqianshou_zuo
    }
    private static bool YouShou(Human human)//右手是否空着
    {
        return (!human.qianshou_you);// && !human.beiqianshou_you
    }
    public static void SetQianShou(Human human, Human human2, int qianshou_stype = 0)//牵human2的手
    {
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng.qianshouxitong_KaiGuan)
        {
            Chat.TiShi(human.player.host, "牵手系统已关闭");
            return;
        }

        if (human2 == null)
        {
            return;
        }
        if (human == human2)
        {
            //Debug.Log("不能牵自己");
            Chat.TiShi(human.player.host, "不能牵自己~");
            return;
        }
        if (!ZuoShou(human) && !YouShou(human))
        {
            //Debug.Log("你没有第三只手");
            Chat.TiShi(human.player.host, "你没有空闲的手~");
            return;
        }
        //if (!ZuoShou(human2) && !YouShou(human2))
        //{
        //    //Debug.Log("你没有第三只手");
        //    Chat.TiShi(human.player.host, $"{human2.player.host.name} 没有空闲的手~");
        //    return;
        //}

        /////不能牵两只手
        //if ((human.qianshou_zuo_human == human2) || (human.qianshou_you_human == human2))
        //{
        //    Chat.TiShi(human.player.host, "你已经牵着他了~");
        //    return;
        //}

        switch (qianshou_stype)
        {
            case 0:
                float juli1 = Vector3.Distance(human.ragdoll.partLeftHand.transform.position, human2.ragdoll.partRightHand.transform.position);
                float juli2 = Vector3.Distance(human.ragdoll.partRightHand.transform.position, human2.ragdoll.partLeftHand.transform.position);
                float juli3 = Vector3.Distance(human.ragdoll.partLeftHand.transform.position, human2.ragdoll.partLeftHand.transform.position);
                float juli4 = Vector3.Distance(human.ragdoll.partRightHand.transform.position, human2.ragdoll.partRightHand.transform.position);
                float zuixiaozhi = 3f;
                if (ZuoShou(human) && YouShou(human))
                {
                    if (ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = Math.Min(Math.Min(juli1, juli2), Math.Min(juli3, juli4));
                    }
                    else if (!ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = Math.Min(juli1, juli4);
                    }
                    else if (ZuoShou(human2) && !YouShou(human2))
                    {
                        zuixiaozhi = Math.Min(juli2, juli3);
                    }
                }

                else if (!ZuoShou(human) && YouShou(human))
                {
                    if (ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = Math.Min(juli2, juli4);
                    }
                    else if (!ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = juli4;
                    }
                    else if (ZuoShou(human2) && !YouShou(human2))
                    {
                        zuixiaozhi = juli2;
                    }
                }

                else if (ZuoShou(human) && !YouShou(human))
                {
                    if (ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = Math.Min(juli1, juli3);
                    }
                    else if (!ZuoShou(human2) && YouShou(human2))
                    {
                        zuixiaozhi = juli1;
                    }
                    else if (ZuoShou(human2) && !YouShou(human2))
                    {
                        zuixiaozhi = juli3;
                    }
                }

                Debug.Log(zuixiaozhi);

                if (zuixiaozhi > 1f)
                {
                    Chat.TiShi(human.player.host, "牵他就靠近她~");
                    //return;
                }
                if (zuixiaozhi == juli1)
                {
                    if (human.qianshou_you_human != human2)
                    {
                        human.qianshou_zuo = true;
                        human.qianshou_zuo_human = human2;
                        human.qianshou_zuo_humanHand = human2.ragdoll.partRightHand;
                        //    human2.beiqianshou_you = true;
                    }

                }
                else if (zuixiaozhi == juli2)
                {
                    if (human.qianshou_zuo_human != human2)
                    {
                        human.qianshou_you = true;
                        human.qianshou_you_human = human2;
                        human.qianshou_you_humanHand = human2.ragdoll.partLeftHand;
                        //    human2.beiqianshou_zuo = true;
                    }
                }
                else if (zuixiaozhi == juli3)
                {
                    if (human.qianshou_you_human != human2)
                    {
                        human.qianshou_zuo = true;
                        human.qianshou_zuo_human = human2;
                        human.qianshou_zuo_humanHand = human2.ragdoll.partLeftHand;
                        //    human2.beiqianshou_zuo = true;
                    }
                }
                else if (zuixiaozhi == juli4)
                {
                    if (human.qianshou_zuo_human != human2)
                    {
                        human.qianshou_you = true;
                        human.qianshou_you_human = human2;
                        human.qianshou_you_humanHand = human2.ragdoll.partRightHand;
                        //    human2.beiqianshou_you = true;
                    }
                }
                break;
            //case 1:
            //    human.qianshou_zuo = true;
            //    human.qianshou_zuo_human = human2;
            //    human.qianshou_zuo_humanHand = human2.ragdoll.partRightHand;
            //    break;
            //case 2:
            //    human.qianshou_you = true;
            //    human.qianshou_you_human = human2;
            //    human.qianshou_you_humanHand = human2.ragdoll.partLeftHand;
            //    break;
            //case 3:
            //    human.qianshou_zuo = true;
            //    human.qianshou_zuo_human = human2;
            //    human.qianshou_zuo_humanHand = human2.ragdoll.partLeftHand;
            //    break;
            //case 4:
            //    human.qianshou_you = true;
            //    human.qianshou_you_human = human2;
            //    human.qianshou_you_humanHand = human2.ragdoll.partRightHand;
            //    break;

        }
    }
    public static void QianShou_Fun(Human human)
    {
        ///1:左手牵右手   2:右手牵左手    3:左牵左     4:右牵右
        if (human == null)
        {
            return;
        }
        if (!UI_GongNeng. qianshouxitong_KaiGuan)
        {
            human.qianshou_zuo = human.qianshou_you = false;
            //human.beiqianshou_zuo = human.beiqianshou_you = false;
            human.qianshou_zuo_human = human.qianshou_you_human = null;
            human.qianshou_zuo_humanHand = human.qianshou_you_humanHand = null;
            return; 
        }

        if (human.qianshou_zuo_human == null || human.qianshou_zuo_humanHand == null)
        {
            human.qianshou_zuo = false;
            human.qianshou_zuo_human = null;
            human.qianshou_zuo_humanHand = null;
        }
        if (human.qianshou_you_human == null || human.qianshou_you_humanHand == null)
        {
            human.qianshou_you = false;
            human.qianshou_you_human = null;
            human.qianshou_you_humanHand = null;
        }

        try
        {
            HumanSegment Hand = new HumanSegment();
            HumanSegment Hand2 = new HumanSegment();

            if (human.qianshou_zuo && human.qianshou_zuo_humanHand != null)
            {
                Hand = human.ragdoll.partLeftHand;
                Hand2 = human.qianshou_zuo_humanHand;

                float zuo_juli = Vector3.Distance(Hand.transform.position, Hand2.transform.position);

                if (zuo_juli <= 1f)
                {
                    Zhua(Hand, Hand2);
                }
            }
            else
            {
                human.qianshou_zuo = false;
                human.qianshou_zuo_human = null;
                human.qianshou_zuo_humanHand = null;
            }
            if (human.qianshou_you && human.qianshou_you_humanHand != null)
            {
                Hand = human.ragdoll.partRightHand;
                Hand2 = human.qianshou_you_humanHand;

                float you_juli = Vector3.Distance(Hand.transform.position, Hand2.transform.position);

                if (you_juli <= 1f)
                {
                    Zhua(Hand, Hand2);
                }
            }
            else
            {
                human.qianshou_you = false;
                human.qianshou_you_human = null;
                human.qianshou_you_humanHand = null;
            }
        }
        catch 
        {
            Debug.Log("牵手出错");
            human.qianshou_zuo = false;
            human.qianshou_zuo_human = null;
            human.qianshou_zuo_humanHand = null;
            human.qianshou_you = false;
            human.qianshou_you_human = null;
            human.qianshou_you_humanHand = null;
        }

    }

    //<拳击>
    public static void QuanJiAnimation(Human human)  //y5后 拳击动作动画
    {
        if (human.quanji)
        {
            //human.quanji = true;
            human.ReleaseGrab(0.1f);   ///手滑
            Vector3 worldPos = Vector3.zero;
            Vector3 worldPos2 = Vector3.zero;
            float targetPitchAngle = human.controls.targetPitchAngle;
            float targetYawAngle = human.controls.targetYawAngle;
            Quaternion quaternion = Quaternion.Euler(targetPitchAngle, targetYawAngle, 0f);
            Quaternion quaternion2 = Quaternion.Euler(0f, targetYawAngle, 0f);
            float num = 0f;
            float z = 0f;
            if (targetPitchAngle > 0f && human.onGround)
            {
                z = 0.4f * targetPitchAngle / 90f;
            }

            if (targetPitchAngle > 0f)
            {
                num = -0.2f * targetPitchAngle / 90f;
            }
            if (human.ragdoll.partLeftHand.sensor.grabJoint != null)
            {
                z = ((!human.isClimbing) ? 0f : (-0.2f));
            }
            worldPos = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(-0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.leftExtend * human.ragdoll.handLength);

            if (targetPitchAngle > 0f)
            {
                num = -0.2f * targetPitchAngle / 90f;
            }
            if (human.ragdoll.partRightHand.sensor.grabJoint != null)
            {
                z = ((!human.isClimbing) ? 0f : (-0.2f));
            }
            worldPos2 = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.rightExtend * human.ragdoll.handLength);

            //TargetingMode targetingMode = ((!(human.ragdoll.partLeftHand.sensor.grabJoint != null)) ? this.targetingMode : grabTargetingMode);
            //TargetingMode targetingMode2 = ((!(human.ragdoll.partRightHand.sensor.grabJoint != null)) ? this.targetingMode : grabTargetingMode);
            //switch (targetingMode)
            //{
            //    case TargetingMode.Shoulder:
            //        worldPos = human.ragdoll.partLeftArm.transform.position + quaternion * new Vector3(0f, 0f, human.controls.leftExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Chest:
            //        worldPos = human.ragdoll.partChest.transform.position + quaternion2 * new Vector3(-0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, human.controls.leftExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Hips:
            //        if (targetPitchAngle > 0f)
            //        {
            //            num = -0.3f * targetPitchAngle / 90f;
            //        }
            //        worldPos = human.ragdoll.partHips.transform.position + quaternion2 * new Vector3(-0.2f, 0.65f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.leftExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Ball:
            //        if (targetPitchAngle > 0f)
            //        {
            //            num = -0.2f * targetPitchAngle / 90f;
            //        }
            //        if (human.ragdoll.partLeftHand.sensor.grabJoint != null)
            //        {
            //            z = ((!human.isClimbing) ? 0f : (-0.2f));
            //        }
            //        worldPos = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(-0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.leftExtend * human.ragdoll.handLength);
            //        break;
            //}
            //switch (targetingMode2)
            //{
            //    case TargetingMode.Shoulder:
            //        worldPos2 = human.ragdoll.partRightArm.transform.position + quaternion * new Vector3(0f, 0f, human.controls.rightExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Chest:
            //        worldPos2 = human.ragdoll.partChest.transform.position + quaternion2 * new Vector3(0.2f, 0.15f, 0f) + quaternion * new Vector3(0f, 0f, human.controls.rightExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Hips:
            //        if (targetPitchAngle > 0f)
            //        {
            //            num = -0.3f * targetPitchAngle / 90f;
            //        }
            //        worldPos2 = human.ragdoll.partHips.transform.position + quaternion2 * new Vector3(0.2f, 0.65f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.rightExtend * human.ragdoll.handLength);
            //        break;
            //    case TargetingMode.Ball:
            //        if (targetPitchAngle > 0f)
            //        {
            //            num = -0.2f * targetPitchAngle / 90f;
            //        }
            //        if (human.ragdoll.partRightHand.sensor.grabJoint != null)
            //        {
            //            z = ((!human.isClimbing) ? 0f : (-0.2f));
            //        }
            //        worldPos2 = human.ragdoll.partBall.transform.position + quaternion2 * new Vector3(0.2f, 0.7f + num, z) + quaternion * new Vector3(0f, 0f, human.controls.rightExtend * human.ragdoll.handLength);
            //        break;
            //}
            if (!human.controls.leftGrab || human.controls.rightGrab)
            {
                PlaceHand(human, human.ragdoll.partLeftArm, human.ragdoll.partLeftHand, worldPos, active: true, human.ragdoll.partLeftHand.sensor.grabJoint != null, human.ragdoll.partLeftHand.sensor.grabBody);
            }
            if(!human.controls.rightGrab || human.controls.leftGrab)
            {
                PlaceHand(human, human.ragdoll.partRightArm, human.ragdoll.partRightHand, worldPos2, active: true, human.ragdoll.partRightHand.sensor.grabJoint != null, human.ragdoll.partRightHand.sensor.grabBody);
            }
            

            if (human.ragdoll.partLeftHand.sensor.grabBody != null)
            {
                LiftBody(human, human.ragdoll.partLeftHand, human.ragdoll.partLeftHand.sensor.grabBody);
            }
            human.ragdoll.partLeftHand.sensor.grabPosition = worldPos;

            if (!human.controls.leftGrab && !human.controls.rightGrab)
            {
                human.chuquan = true;
            }

        }

    }
    private static void LiftBody(Human human, HumanSegment hand, Rigidbody body)
    {
        float maxLiftForce = 500f;
        float maxPushForce = 200f;
        float liftDampSqr = 0.1f;
        float liftDamp = 0.1f;
        if (human.GetComponent<GroundManager>().IsStanding(body.gameObject) || body.tag == "NoLift")
        {
            return;
        }
        float num = 0.5f + 0.5f * Mathf.InverseLerp(0f, 100f, body.mass);
        Vector3 vector = (human.targetLiftDirection.ZeroY() * maxPushForce).SetY(Mathf.Max(0f, human.targetLiftDirection.y) * maxLiftForce);
        float magnitude = (hand.transform.position - body.worldCenterOfMass).magnitude;
        float num2 = num;
        float num3 = 1f;
        float num4 = 1f;
        Carryable component = body.GetComponent<Carryable>();
        if (component != null)
        {
            num2 *= component.liftForceMultiplier;
            num3 = component.forceHalfDistance;
            num4 = component.damping;
            if (num3 <= 0f)
            {
                throw new InvalidOperationException("halfdistance cant be 0 or less!");
            }
        }
        float num5 = num3 / (num3 + magnitude);
        vector *= num2;
        vector *= num5;
        body.SafeAddForce(vector);
        hand.rigidbody.SafeAddForce(-vector * 0.5f);
        human.ragdoll.partChest.rigidbody.SafeAddForce(-vector * 0.5f);
        body.SafeAddTorque(-body.angularVelocity * liftDamp * num4, ForceMode.Acceleration);
        body.SafeAddTorque(-body.angularVelocity.normalized * body.angularVelocity.sqrMagnitude * liftDampSqr * num4, ForceMode.Acceleration);
        if (!(component != null) || component.aiming == CarryableAiming.None)
        {
            return;
        }
        Vector3 vector2 = human.targetLiftDirection;
        if (component.limitAlignToHorizontal)
        {
            vector2.y = 0f;
            vector2.Normalize();
        }
        Vector3 vector3 = ((component.aiming != CarryableAiming.ForwardAxis) ? (body.worldCenterOfMass - hand.transform.position).normalized : body.transform.forward);
        float aimSpring = component.aimSpring;
        float num6 = ((!(component.aimTorque < float.PositiveInfinity)) ? aimSpring : component.aimTorque);
        if (!component.alwaysForward)
        {
            float num7 = Vector3.Dot(vector3, vector2);
            if (num7 < 0f)
            {
                vector2 = -vector2;
                num7 = 0f - num7;
            }
            num6 *= Mathf.Pow(num7, component.aimAnglePower);
        }
        else
        {
            float num8 = Vector3.Dot(vector3, vector2);
            num8 = 0.5f + num8 / 2f;
            num6 *= Mathf.Pow(num8, component.aimAnglePower);
        }
        if (component.aimDistPower != 0f)
        {
            num6 *= Mathf.Pow((body.worldCenterOfMass - hand.transform.position).magnitude, component.aimDistPower);
        }
        HumanMotion2.AlignToVector(body, vector3, vector2, aimSpring, num6);
    }
    private static void PlaceHand(Human human, HumanSegment arm, HumanSegment hand, Vector3 worldPos, bool active, bool grabbed, Rigidbody grabbedBody)
    {
        if (!active)
        {
            return;
        }
        Rigidbody rigidbody = hand.rigidbody;
        Vector3 worldCenterOfMass = rigidbody.worldCenterOfMass;
        Vector3 offset = worldPos - worldCenterOfMass;
        Vector3 vector = new Vector3(0f, offset.y, 0f);
        Vector3 velocity = rigidbody.velocity - human.ragdoll.partBall.rigidbody.velocity;
        float armMass = 20f;
        float num = armMass;
        float maxForce = 300f;
        float num2 = maxForce;

        float bodyMass = 50f;
        float grabMaxForce = 450f;
        float climbMaxForce = 800f;
        float gravityForce = 100f;



        if (grabbed)
        {
            if (grabbedBody != null)
            {
                num += Mathf.Clamp(grabbedBody.mass / 2f, 0f, bodyMass);
                num2 = Mathf.Lerp(grabMaxForce, climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
            }
            else
            {
                num += bodyMass;
                num2 = Mathf.Lerp(grabMaxForce, climbMaxForce, (human.controls.targetPitchAngle - 50f) / 30f);
            }
        }
        float maxAcceleration = num2 / num;
        Vector3 vector2 = ConstantAccelerationControl.Solve(offset, velocity, maxAcceleration, 0.1f);
        int num3 = 600;
        Vector3 vector3 = vector2 * num + Vector3.up * gravityForce;
        if (human.grabbedByHuman != null && human.grabbedByHuman.state == HumanState.Climb)
        {
            vector3 *= 1.7f;
            num3 *= 2;
        }
        if (!grabbed)
        {
            rigidbody.SafeAddForce(vector3);
            human.ragdoll.partHips.rigidbody.SafeAddForce(-vector3);
            return;
        }
        Vector3 normalized = human.targetDirection.ZeroY().normalized;
        Vector3 vector4 = Mathf.Min(0f, Vector3.Dot(normalized, vector3)) * normalized;
        Vector3 vector5 = vector3 - vector4;
        Vector3 vector6 = vector3.SetX(0f).SetZ(0f);
        Vector3 b = -vector3 * 0.25f;
        Vector3 b2 = -vector3 * 0.75f;
        Vector3 a = -vector3 * 0.1f - vector6 * 0.5f - vector5 * 0.25f;
        Vector3 a2 = -vector6 * 0.2f - vector5 * 0.4f;
        if (grabbedBody != null)
        {
            Carryable component = grabbedBody.GetComponent<Carryable>();
            if (component != null)
            {
                b *= component.handForceMultiplier;
                b2 *= component.handForceMultiplier;
            }
        }
        float num4 = ((human.state != HumanState.Climb) ? 1f : Mathf.Clamp01((human.controls.targetPitchAngle - 10f) / 60f));
        Vector3 vector7 = Vector3.Lerp(a, b, offset.y + 0.5f) * num4;
        Vector3 vector8 = Vector3.Lerp(a2, b2, offset.y + 0.5f) * num4;
        float num5 = Mathf.Abs(vector7.y + vector8.y);
        if (num5 > (float)num3)
        {
            vector7 *= (float)num3 / num5;
            vector8 *= (float)num3 / num5;
        }
        human.ragdoll.partChest.rigidbody.SafeAddForce(vector7);
        human.ragdoll.partBall.rigidbody.SafeAddForce(vector8);
        rigidbody.SafeAddForce(-vector7 - vector8);
    }

    public static void QuanJi_Fun(Human human)//出拳打到
    {
        if (!human.quanji || human == null)
        {
            //Debug.Log($"{human.titui}");
            return;
        }
        if (!human.chuquan)
        { return; }
        if (!UI_GongNeng.jifeixitong_KaiGuan)
        {
            //Chat.TiShi(human.player.host, "Y键击飞系统已关闭");
            return;
        }
        //human.chuquan = true;
        foreach (Rigidbody rigidbody in UnityEngine.Object.FindObjectsOfType(Type.GetTypeFromHandle(typeof(Rigidbody).TypeHandle)))
        {
            QuanJi(human, rigidbody);
        }
        //human.chuquan = false;

    }
    private static void QuanJi(Human human, Rigidbody targetRigidbody)
    {
        //if (!human.chuquan)
        //{ return; }
        // 获取踢腿部位的碰撞器

        Collider leftHandCollider = human.ragdoll.partLeftHand.collider;
        Collider rightHandCollider = human.ragdoll.partRightHand.collider;

        if (leftHandCollider == null || rightHandCollider == null)
        {
            return;
        }

        // 获取目标刚体的所有碰撞器
        Collider[] targetColliders = targetRigidbody.GetComponents<Collider>();

        foreach (Collider targetCollider in targetColliders)
        {
            if(targetCollider.transform.name == "LightWoodMechLargeCatapult1" || 
               targetCollider.transform.name == "LeftHand" || 
               targetCollider.transform.name == "RightHand" || 
               targetCollider.transform.name == "LeftForearm" || 
               targetCollider.transform.name == "RightForearm")
            {
                return;
            }    

            // 检测踢腿部位的碰撞器是否与目标刚体的碰撞器相交
            if (leftHandCollider.bounds.Intersects(targetCollider.bounds) || rightHandCollider.bounds.Intersects(targetCollider.bounds))
            {
                // 如果碰撞器相交，执行踢击操作
                Human componentInParent = targetCollider.GetComponentInParent<Human>();
                if (human.controls.leftGrab || human.controls.rightGrab)
                {
                    float juli = 1;
                    if (human.controls.leftGrab)
                    {
                        //juli = Vector3.Distance(human.ragdoll.partLeftHand.transform.position, human.ragdoll.partHead.transform.position);
                        juli = Vector3.Distance(human.ragdoll.partLeftHand.transform.position, human.ragdoll.partLeftArm.transform.position);
                    }
                    else if (human.controls.rightGrab)
                    {
                        //juli = Vector3.Distance(human.ragdoll.partRightHand.transform.position, human.ragdoll.partHead.transform.position);
                        juli = Vector3.Distance(human.ragdoll.partLeftHand.transform.position, human.ragdoll.partRightArm.transform.position);
                    }

                    if (componentInParent != null)
                    {
                        if (componentInParent != human)
                        {
                            for (int j = 0; j < componentInParent.rigidbodies.Length; j++)
                            {
                                //componentInParent.ragdoll.partBall.rigidbody.AddForce(targetRigidbody.mass * (human.ragdoll.partHead.transform.forward * 100f + Vector3.up * 50f) * 16.5f / 1, ForceMode.Impulse);
                                componentInParent.rigidbodies[j].AddForce(componentInParent.rigidbodies[j].mass * (human.ragdoll.partHead.transform.forward * 10f + Vector3.up * 5f) * juli * 10f / componentInParent.rigidbodies.Length, ForceMode.Impulse);
                            }
                            human.chuquan = false;
                            Debug.Log($" 击飞人 {targetCollider.transform.name}");
                            break;
                        }
                    }
                    else
                    {
                        //targetRigidbody = targetRigidbody.GetComponentInParent<Rigidbody>();
                        //targetRigidbody.AddForce(targetRigidbody.mass * (human.ragdoll.partHead.transform.forward * 10f + Vector3.up * 5f) * 2f, ForceMode.Impulse);
                        targetRigidbody.AddForce(targetRigidbody.mass * (human.ragdoll.partHead.transform.forward * 10f + Vector3.up * 5f) * juli * 1f, ForceMode.Impulse);
                        human.chuquan = false;
                        //Debug.Log($"击飞物 {targetCollider.transform.name}");
                        break;
                    }
                }
            }
        }
    }
    //</拳击> //
    private static void Zhua(HumanSegment Hand, HumanSegment Hand2)
    {
        //GameObject grabObject = Hand2.rigidbody.gameObject;
        Rigidbody targetRigidbody = Hand2.rigidbody;//grabObject.GetComponent<Rigidbody>();
        // 检查目标是否合法
        if (targetRigidbody != null)
        {
            Vector3 grabPosition = Hand2.transform.position;
            Vector3 position = Hand.transform.position;
            Vector3 vector = grabPosition - position;
            if (Hand.sensor.grabJoint == null)
            {
                Hand.sensor.grabJoint = Hand.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
                Hand.sensor.grabJoint.autoConfigureConnectedAnchor = false;
                Hand.sensor.grabJoint.anchor = Hand.rigidbody.transform.InverseTransformPoint(position + vector.normalized * 0.1f);//
                if ((bool)targetRigidbody)
                {
                    Hand.sensor.grabJoint.connectedBody = targetRigidbody;
                    Hand.sensor.grabJoint.connectedAnchor = targetRigidbody.transform.InverseTransformPoint(grabPosition);
                }
                else
                {
                    Hand.sensor.grabJoint.connectedAnchor = grabPosition;
                }
                Hand.sensor.grabJoint.xMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.yMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.zMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.angularXMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.angularYMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.angularZMotion = ConfigurableJointMotion.Locked;
                Hand.sensor.grabJoint.breakForce = 20000f;
                Hand.sensor.grabJoint.breakTorque = 20000f;
                Hand.sensor.grabJoint.enablePreprocessing = false;
                Hand.sensor.grabBody = targetRigidbody;

                // 触发抓取事件，通知系统等
                //human.grabManager.ObjectGrabbed(human2.ragdoll.partRightHand.rigidbody.gameObject);

            }

        }
    }

    public static void SanJiTiao_Fun(Human human)//三级跳
    {
        //if (human.sanjitiao)
        //{
        if (human.controls.jump && human.jumpDelay <= 0f && human.tiaoing)
        {
            human.onGround = true;
            if (human.tiaoing)
            {
                human.Jump_Times++;
                if (human.Jump_Times == 3)
                {
                    human.onGround = false;
                    human.Jump_Times = 0;
                    human.tiaoing = false;
                }
            }
        }
        else
        {
            human.onGround = (human.groundDelay <= 0f && human.groundManager.onGround);
        }
        if (human.groundManager.onGround)
        {
            human.tiaoing = true;
            human.Jump_Times = 0;
        }
        //}
    }
    public static void KongQiPao_Fun(Human human)//射击空气炮
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (!human.kongqipao)
        {
            return;
        }
        if (human.cannon_used && !human.controls.leftGrab)
        {
            human.cannon_used = false;
            human.extend_time = 0f;
        }
        if (!human.cannon_used && human.controls.leftGrab)
        {
            human.extend_time += Time.fixedDeltaTime;
        }
        if (!human.cannon_used && human.extend_time > 0.5f)
        {
            human.cannon_used = true;
            //Debug.Log("开火!");
            ShootRigidbody(human);
        }
    }
    public static void PangXie_Fun(Human human)//螃蟹
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.pangxie)
        {
            Vector3 a = Quaternion.Euler(0f, human.controls.cameraYawAngle - 90f, 0f) * Vector3.forward;

            human.ragdoll.partLeftThigh.rigidbody.AddForce(a * 1000f, ForceMode.Force);
            human.ragdoll.partRightThigh.rigidbody.AddForce(a * 1000f * -1f, ForceMode.Force);

            human.ragdoll.partLeftLeg.rigidbody.AddForce(a * 1000f * 2f, ForceMode.Force);
            human.ragdoll.partRightLeg.rigidbody.AddForce(a * 1000f * -2f, ForceMode.Force);

            human.ragdoll.partLeftFoot.rigidbody.AddForce(a * 1000f * 3f, ForceMode.Force);
            human.ragdoll.partRightFoot.rigidbody.AddForce(a * 1000f * -3f, ForceMode.Force);
        }
    }
    private static GrabManager GetGrabManager(Human human)
    {
        if (human.grabManager == null)
        {
            human.grabManager = human.GetComponent<GrabManager>();
        }
        return human.grabManager;
    }
    public static void DaoLi_Fun(Human human)//倒立
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.daoli)
        {
            if (human.ragdoll.partLeftHand.sensor.grabObject != null && human.ragdoll.partRightHand.sensor.grabObject != null)
            {
                human.state = HumanState.Unconscious;
                Rigidbody[] rigidbodies = human.rigidbodies;
                for (int i = 0; i < rigidbodies.Length; i++)
                {
                    rigidbodies[i].useGravity = false;
                }
                human.ragdoll.partLeftFoot.rigidbody.velocity = Vector3.up * 4f;
                human.ragdoll.partRightFoot.rigidbody.velocity = Vector3.up * 4f;
                return;
            }
            Rigidbody[] rigidbodies2 = human.rigidbodies;
            for (int j = 0; j < rigidbodies2.Length; j++)
            {
                rigidbodies2[j].useGravity = true;
            }
        }
    }
    public static void DaoLi(Human human)
    {
        //human.daoli = !human.daoli;

        if (human.daoli)
        {
            human.ReleaseGrab(1f);
            human.motionControl2.hands.maxPushForce = 200f;
            human.motionControl2.hands.maxForce = 1500f;
        }
        else
        {
            human.motionControl2.hands.maxForce = 300f;
            human.motionControl2.hands.maxPushForce = 500f;
        }
    }
    public static void DiaoSiGui_Fun(Human human)//吊死鬼
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if (human.diaosigui)
        {
            human.state = HumanState.Unconscious;
            if (human.diaosiguiTime < 0.5f)
            {
                human.diaosiguiTime += Time.deltaTime;
                for (int i = 0; i < human.rigidbodies.Length; i++)
                {
                    human.rigidbodies[i].velocity = Vector3.zero;
                }
                human.SetPosition(human.transform.position + 0.2f * Vector3.up);
                return;
            }

        }


    }
    public static void DiaoSiGui(Human human)//吊死鬼
    {
        //human.diaosigui = !human.diaosigui;
        if (human.diaosigui)
        {
            human.ragdoll.partRightLeg.rigidbody.isKinematic = true;
            return;
        }
        human.ragdoll.partRightLeg.rigidbody.isKinematic = false;
        human.diaosiguiTime = 0f;
    }
    public static void TuiQue_Fun(Human human)
    {
        if (human == null)
        { return; }
        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            return;
        }
        if ((human.tuique || human.tuiguai) && human.state == HumanState.Jump)
        {
            human.state = HumanState.Idle;
        }
    }
    public static void TuiQue(Human human)
    {
        //human.tuique = !human.tuique;
        human.ragdoll.partLeftFoot.rigidbody.freezeRotation = human.tuique;
        human.ragdoll.partRightFoot.rigidbody.freezeRotation = human.tuique;
        human.ragdoll.partRightLeg.rigidbody.freezeRotation = human.tuique;
        human.ragdoll.partLeftLeg.rigidbody.freezeRotation = human.tuique;

    }
    public static void TuiGuai(Human human)
    {
        //human.tuiguai = !human.tuiguai;
        human.ragdoll.partLeftFoot.rigidbody.gameObject.SetActive(!human.tuiguai);
        human.ragdoll.partLeftLeg.rigidbody.gameObject.SetActive(!human.tuiguai);
        human.ragdoll.partRightLeg.rigidbody.freezeRotation = human.tuiguai;
        human.ragdoll.partLeftLeg.rigidbody.freezeRotation = human.tuiguai;

    }
    public static Vector3 Human_Looking_Direction(Human human)
    {
        return Quaternion.Euler(human.controls.targetPitchAngle, human.controls.targetYawAngle, 0f) * Vector3.forward;
    }

    public static Human ChaHumanId(string txt, Human whohuman)//查 Human
    {
        if (string.IsNullOrEmpty(txt))
        {
            return null;
        }
        string[] array = txt.Split(new char[]
        {
            ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (array.Length != 1)
        {
            return null;
        }
        int num = 0;
        if (int.TryParse(array[0], out num))
        {
            if (num == 1)
            {
                return NetGame.instance.server.players[NetGame.instance.server.players.Count - 1].human;
            }
            if (num - 2 < NetGame.instance.readyclients.Count)
            {
                return NetGame.instance.readyclients[num - 2].players[NetGame.instance.readyclients[num - 2].players.Count - 1].human;
            }
        }
        Chat.TiShi(whohuman.player.host, $"{num}号玩家不存在!");
        return null;
    }

}
