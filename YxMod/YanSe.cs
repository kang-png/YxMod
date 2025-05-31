using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public class CHOOSECOLOR
{
    public Int32 lStructSize;
    public Int32 hwndOwner;
    public Int32 hInstance;
    public Int32 rgbResult;
    public IntPtr lpCustColors;
    public Int32 Flags;
    public Int32 lCustData;
    public Int32 lpfnHook;
    public Int32 lpTemplateName;
}
internal class YanSe
{
    [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
    public static extern bool ChooseColorA(CHOOSECOLOR pChoosecplor);//对应的win32API
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    public static IntPtr hwnd;
    private static bool ChooseColorA1(CHOOSECOLOR pChoosecplor)
    {
        return ChooseColorA(pChoosecplor);
    }

    public static void GetHumanHwnd()
    {
        string unityWindowClass = "UnityWndClass"; // 或者尝试 "UnityEditor.MainWindow" 等
        string windowTitle = "Human"; // 游戏窗口标题
        hwnd = FindWindow(unityWindowClass, windowTitle);
    }
    public static bool GetYanSeWindow(ref string colorStr)
    {
        CHOOSECOLOR choosecolor = new CHOOSECOLOR();
        choosecolor.lStructSize = Marshal.SizeOf(typeof(CHOOSECOLOR));
        choosecolor.hwndOwner = (int)hwnd;//0;
        choosecolor.rgbResult = 0x808080;//颜色转换成int
        choosecolor.lpCustColors = Marshal.AllocCoTaskMem(64);
        choosecolor.Flags = 0x00000002 | 0x00000001;
        bool res = false;
        if (ChooseColorA1(choosecolor))
        {
            int a = choosecolor.rgbResult;//获取int型颜色值  rgba 由这个值组成
            colorStr = Convert.ToString(a, 16).ToUpper().PadLeft(6, '0');//十进制转16大写
            string bb = colorStr.Substring(0, 2);
            string gg = colorStr.Substring(2, 2);
            string rr = colorStr.Substring(4, 2);
            colorStr = "#" + rr + gg + bb;
            //colorStr = Convert.ToString(a, 16);
            //Debug.Log(colorStr);
            res = true;
        }
        return res;
    }
}
