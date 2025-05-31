using Multiplayer;
using UnityEngine;

public class KeyDisplayUI : MonoBehaviour
{
    public static bool showKeys;
    public Texture2D triangleTexture;
    public Texture2D rectangleTexture;

    private void Update()
    {
        //if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.G))
        //{
        //    ToggleKeyDisplay();
        //}
    }
    private void OnGUI()
    {
        if (showKeys)
        {
            float currentX = 50f;
            float yOffset = 150f;
            float num = 100f;
            float width = 80f;
            float height = 50f;
            DrawKeyIfPressed(KeyCode.A, "A", triangleTexture, ref currentX, yOffset, num);
            DrawKeyIfPressed(KeyCode.S, "S", triangleTexture, ref currentX, yOffset, num);
            DrawKeyIfPressed(KeyCode.D, "D", triangleTexture, ref currentX, yOffset, num);
            DrawKeyIfPressed(KeyCode.W, "W", triangleTexture, ref currentX, yOffset, num);
            DrawKeyIfPressed(KeyCode.Space, "空格", rectangleTexture, ref currentX, yOffset, num * 2f);
            DrawMouseKeyIfPressed(0, "左键", rectangleTexture, ref currentX, yOffset, width, height);
            DrawMouseKeyIfPressed(1, "右键", rectangleTexture, ref currentX, yOffset, width, height);
            DrawKeyIfPressed(KeyCode.Y, "Y", triangleTexture, ref currentX, yOffset, num);
        }
    }

    private void DrawKey(Rect position, string keyText, Texture2D keyTexture)
    {
        GUIStyle gUIStyle = new GUIStyle((GUI.skin).button);
        (gUIStyle).normal.background = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.6f));
        gUIStyle.normal.textColor = Color.black;
        gUIStyle.fontSize = 16;
        gUIStyle.border = new RectOffset(10, 10, 10, 10);
        GUI.Box(position, keyText, gUIStyle);
        GUI.DrawTexture(position, keyTexture, ScaleMode.ScaleToFit, alphaBlend: true);
    }

    private void DrawKeyIfPressed(KeyCode keyCode, string keyText, Texture2D keyTexture, ref float currentX, float yOffset, float keySize)
    {
        if (Input.GetKey(keyCode))
        {
            DrawKey(new Rect(currentX, 100f + yOffset, keySize, keySize), keyText, keyTexture);
            currentX += keySize + 20f;
        }
    }

    private void DrawMouseKeyIfPressed(int mouseButton, string keyText, Texture2D keyTexture, ref float currentX, float yOffset, float width, float height)
    {
        if (Input.GetMouseButton(mouseButton))
        {
            DrawKey(new Rect(currentX, 100f + yOffset, width, height), keyText, keyTexture);
            currentX += width + 20f;
        }
    }

    private static Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] array = new Color[width * height];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = color;
        }
        Texture2D texture2D = new Texture2D(width, height);
        texture2D.SetPixels(array);
        texture2D.Apply();
        return texture2D;
    }

    public static void ToggleKeyDisplay()
    {
        //KeyDisplayUI keyDisplayUI = Object.FindObjectOfType<KeyDisplayUI>();
        //if (keyDisplayUI != null)
        //{
        showKeys = !showKeys;
        //string msg = (showKeys ? "显示按键信息已 开启" : "显示按键信息已 关闭");
        //Chat.TiShi(NetGame.instance.local, msg);
        //Host_Func.Custom_Messages(0u, ColorfulSpeek.colorshow(msg), "");
        //}
    }

}
