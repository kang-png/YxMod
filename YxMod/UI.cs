using Multiplayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;




internal class UI
{
    public static Texture2D anniuTexture = new Texture2D(1, 1);
    public static Texture2D anniuTexture2 = new Texture2D(1, 1);//选中按下时
    public static Texture2D anniuTexture3 = new Texture2D(1, 1);//移动至按钮时

    private static string[] humanNames = new string[0]; // 初始为空的按钮名称数组
    public static int ZiJiId;
    public static float buttonHeight;

    public static void UI_ChuShiHua()
    {
        anniuTexture.SetPixel(0, 0, new Color32(130, 130, 130, 255));//按钮未按下背景色
        anniuTexture.Apply();

        anniuTexture2.SetPixel(0, 0, new Color32(0, 0xbe, 0x92, 255));//按钮按下
        anniuTexture2.Apply();

        anniuTexture3.SetPixel(0, 0, new Color32(0, 0x70, 0xd7, 255));//按钮按下
        anniuTexture3.Apply();

        
        //Debug.Log("buttonHeight : " + buttonHeight);
    }
    public static GUIStyle styleSelectionGrid()  ///按下松开效果
    {
        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.normal.background = anniuTexture; // 没选中的
        styleButton.onNormal.background = anniuTexture2;//选中的
        styleButton.active.background = anniuTexture2; //鼠标按下的
        styleButton.hover.background = styleButton.onNormal.background;//鼠标移动的
        styleButton.onHover.background = styleButton.onNormal.background;//选中的  鼠标移动的
        styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
        styleButton.hover.textColor = Color.white;
        styleButton.alignment = TextAnchor.MiddleLeft;// MiddleCenter;
        styleButton.fontSize = 16;
        return styleButton;
    }
    public static int CreatHumanList(int index,bool quanyuankongzhi=true)
    {
        try
        {
            //清空//
            //List<string> tempList = new List<string>(humanNames);
            //tempList.Clear();
            //humanNames = tempList.ToArray();
            humanNames = new string[0];
            //清空//
            if (quanyuankongzhi)
            {
                string quanyuan = "全员控制";
                AddHumanListButton(quanyuan);
            }

            string fzname = $"1.{NetGame.instance.server.name}";
            //if (NetGame.isServer || (NetGame.isClient && YxMod.YxModServer))
            //{
            //    fzname = $"★{fzname}";
            //}

            AddHumanListButton(fzname);
            
            //if(NetGame.isServer)
            //{
            //    ZiJiId = 1;
            //}
            List<NetHost> hosts = NetGame.instance.readyclients;//客户机名


            for (int j = 0; j < hosts.Count; j++)
            {
                string name = $"{j+2}.{hosts[j].name}";
                //if ((NetGame.isServer && players[j].players[0].human.isClient) || (NetGame.isClient && players[j].hostId == NetGame.instance.local.hostId))
                //if ((NetGame.isServer && players[j].players[0].human.isClient) || (NetGame.isClient && players[j].players[0].human.isClient))
                //{
                //    name = $"☆{name}";
                //}
                AddHumanListButton(name);
                //if(NetGame.isClient && hosts[j].hostId == NetGame.instance.local.hostId)
                //{
                //    ZiJiId = j;
                //}

            }

            index = GUILayout.SelectionGrid(index, humanNames, 1, styleSelectionGrid());
            //Debug.Log("当前选中：" + index + "   客户机总数：" + hosts.Count);
            index = (index > (quanyuankongzhi? hosts.Count + 1:hosts.Count)) ? 0 : index;
            return index;
        }
        catch
        {
            Debug.Log("选中出错了");
            return 0;
        }
    }

    private static void AddHumanListButton(string name)
    {
        // 创建一个新的字符串数组，长度比原来多一个
        List<string> tempList = new List<string>(humanNames);
        tempList.Add(name); // 向列表中添加新的按钮名称
        humanNames = tempList.ToArray(); // 将列表转换回数组

        // 更新SelectionGrid以反映变化
        UpdateSelectionGrid();
    }
    private static void UpdateSelectionGrid()
    {
        // 强制Unity重新绘制GUI，确保SelectionGrid更新
        GUI.changed = true;
        // 注意：在某些情况下，可能需要使用Layout.Repaint()来强制更新GUI
        // UnityEngine.Experimental.UIElements.Layout.Repaint();
    }
    private static GUIStyle styleButton_Tab(bool tab)  ///按下松开效果
    {
        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.normal.background = !tab ? anniuTexture : anniuTexture2; // 按钮的背景纹理
        //styleButton.active.background = anniuTexture2; 
        styleButton.hover.background = styleButton.normal.background;
        styleButton.normal.textColor = new Color32(220, 220, 220, 255);// Color.white;
        styleButton.hover.textColor = Color.white;
        styleButton.alignment = TextAnchor.MiddleCenter;
        styleButton.fontSize = 16;
        return styleButton;
    }

    public static GUIStyle styleButton() /// 滑动效果
    {
        GUIStyle styleButton = new GUIStyle(GUI.skin.button);
        styleButton.normal.background = anniuTexture; // 按钮的背景纹理
        styleButton.active.background = anniuTexture2; // 假设按下时的纹理
        styleButton.hover.background = styleButton.active.background;
        styleButton.normal.textColor = Color.white;
        styleButton.hover.textColor = Color.black;
        styleButton.alignment = TextAnchor.MiddleCenter;
        styleButton.fontSize = 16;

        return styleButton;
    }
    public static float GetButtonHeight(GUIStyle style, string sampleText = "按钮")
    {
        //GUIStyle style = styleButton();
        // 使用GUI.skin.button的字体大小作为参考，如果style中有自定义字体大小则应直接使用style.fontSize
        float lineHeight = style.fontSize + style.padding.vertical; // 基于字体大小和垂直内边距估算高度
                                                                    // 使用CalcSize进一步细化估算，考虑到文本的实际大小可能影响按钮高度
        Vector2 size = GUI.skin.button.CalcSize(new GUIContent(sampleText));
        return Mathf.Max(lineHeight, size.y); // 选取字体行高和文本内容高度中的较大者作为估算值
    }
    public static GUIStyle SetTxtStyle() /// 设置文本框的样式
    {
        GUIStyle styleTxt = new GUIStyle(GUI.skin.textField);
        //style.normal.background = MakeTex(500, 50, Color.white); // 创建一个纯白色的背景纹理
        styleTxt.normal.textColor = Color.white; // 设置字体颜色为黑色
        styleTxt.alignment = TextAnchor.MiddleLeft;
        styleTxt.fontSize = 16;
        return styleTxt;
    }
    public static GUIStyle SetLabelStyle_JuZhong()
    {
        // 创建或获取一个居中的GUIStyle
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter; // 设置文本居中对齐
        style.normal.textColor = Color.gray;
        style.fontSize = 16;
        return style;
    }
    public static void CreatShuZhi(string name, ref float zhi, float min, float max, float add, Action callback = null)//创建加减数值的按钮
    {
        GUILayout.BeginHorizontal();
        ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
        GUILayout.Label($"<size=16>{name}：</size>");
        //GUILayout.Space(5);
        if (GUILayout.Button("-", UI.styleButton()))
        {
            zhi -= add;
            zhi = float.Parse(zhi.ToString("0.0"));
            if (zhi < min)
            {
                zhi = min;
            }
            else
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }
        }

        GUILayout.Label($"{zhi:0.0}", SetLabelStyle_JuZhong(), GUILayout.Width(30));

        if (GUILayout.Button("+", UI.styleButton()))
        {
            zhi += add;
            zhi = float.Parse(zhi.ToString("0.0"));
            if (zhi > max)
            {
                zhi = max;
            }
            else
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
    public static void CreatShuZhi(string name, ref int zhi, int min, int max, int add, Action callback = null)//创建加减数值的按钮
    {
        GUILayout.BeginHorizontal();
        ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
        GUILayout.Label($"<size=16>{name}：</size>");
        //GUILayout.Space(5);
        if (GUILayout.Button("-", UI.styleButton()))
        {
            zhi -= add;
            //zhi = float.Parse(zhi.ToString("0.0"));
            if (zhi < min)
            {
                zhi = min;
            }
            else
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }
        }

        GUILayout.Label($"{zhi}", SetLabelStyle_JuZhong(), GUILayout.Width(30));

        if (GUILayout.Button("+", UI.styleButton()))
        {
            zhi += add;
            //zhi = float.Parse(zhi.ToString("0.0"));
            if (zhi > max)
            {
                zhi = max;
            }
            else
            {
                callback?.Invoke(); // 如果callback不为null，则调用它
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    }
    public static void CreatWenBenKuang(string name, ref string str, int maxChang, int KuanDuan,Action callback = null)//创建文本框
    {
        GUILayout.BeginHorizontal();
        ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
        if (name != null)
        {
            GUILayout.Label($"<size=16>{name}：</size>");
        }
      
        string str1 = GUILayout.TextField(str, SetTxtStyle(), GUILayout.Width(KuanDuan));
        if(str != str1)
        {
            str = str1;
            if (str.Length > maxChang)
            {
                //str = str.PadLeft(maxChang);
                str = str.Substring(0, maxChang); // 超过长度则截取
            }
            callback?.Invoke(); // 如果callback不为null，则调用它

        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    public static void CreatYanSeKuang(string name, string strYanSe, Action callback)//创建文本框
    {
        GUILayout.BeginHorizontal();
        ///$"<b><size=16>YxMod <i><color=grey>{BanBen}</color></i></size></b>"
        if (name != null)
        {
            GUILayout.Label($"<size=16>{name}：</size>");
        }

        UI.CreatAnNiu($"<color={strYanSe}>{strYanSe}</color>", false, callback);

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    public static void CreatFenGeXian()
    {
        // 初始化一个自定义的GUIStyle并设置边框宽度为0以去除边框
        GUIStyle LineStyle = new GUIStyle(GUI.skin.box);
        LineStyle.border = new RectOffset(0, 0, 0, 0); // 设置四个边的边框宽度都为0

        GUILayout.Box("", LineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2)); // 高度设为2代表线条的粗细

    }


public static void CreatAnNiu(string name, bool chuizhijuzhong = true, Action callback = null)//一般按钮
    {
        if (chuizhijuzhong)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
        }

        if (GUILayout.Button(name, styleButton()))
        {
            callback?.Invoke(); // 如果callback不为null，则调用它
        }
        //int topBottomPadding = (GUI.skin.button.padding.top + GUI.skin.button.padding.bottom); // 这是按钮样式自带的上下内边距之和
        //float estimatedButtonHeight = GUI.skin.button.CalcSize(new GUIContent("Sample Text")).y + topBottomPadding;
        if (chuizhijuzhong)
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        buttonHeight = GetButtonHeight(styleButton(), name)+6;
        //Debug.Log(buttonHeight);
    }

    public static void CreatAnNiu_AnXia(string name, ref bool tab, bool chuizhijuzhong = true, Action callback = null)//Tab按钮
    {
        if (chuizhijuzhong)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
        }

        if (GUILayout.Button(name, styleButton_Tab(tab)))
        {
            tab = !tab;
            callback?.Invoke(); // 如果callback不为null，则调用它
        }
        if (chuizhijuzhong)
        {
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
        buttonHeight = GetButtonHeight(styleButton(), name)+6;
    }


    public static void CreatUiBox(Rect rect, Texture2D texture)//创建界面Box
    {
        GUIStyle myGuiStyle = new GUIStyle(GUI.skin.box)
        {
            normal = { background = texture },
        };
        GUI.Box(rect, GUIContent.none, myGuiStyle);//GUIContent.none      "<b><size=16>菜 单</size></b>"
    }
}
