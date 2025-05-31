using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using HumanAPI;
using System;  // 引入 System 命名空间用于时间处理

public class FPSCounter : MonoBehaviour
{
    private float elapsedTime;
    private int frameCount;
    private int fps;
    private float latency;
    private ulong lastLevelId = 0;

    private WorkshopLevelMetadata currentWorkshopMetadata;  // 缓存元数据

    public static bool showFPS;

    void Start()
    {
        LoadWorkshopMetadata();
    }

    void LoadWorkshopMetadata()
    {
        if (WorkshopRepository.instance != null && WorkshopRepository.instance.levelRepo != null && NetGame.instance != null)
        {
            WorkshopRepository.instance.levelRepo.GetLevel(
                NetGame.instance.currentLevel,
                NetGame.instance.currentLevelType,
                (metadata) =>
                {
                    Debug.Log("Workshop metadata loaded: " + (metadata != null));
                    if (metadata != null)
                    {
                        currentWorkshopMetadata = metadata;
                    }
                });
        }
    }


    void Update()
    {
        if (showFPS)
        {
            elapsedTime += Time.deltaTime;
            frameCount++;

            if (elapsedTime >= 0.5f)
            {
                fps = Mathf.RoundToInt(frameCount / elapsedTime);
                elapsedTime = 0f;
                frameCount = 0;

                if (NetGame.instance != null && NetGame.instance.clientLatency != null)
                {
                    latency = NetGame.instance.clientLatency.latency;
                }
            }
            // 监控关卡变化
            if (NetGame.instance != null)
            {
                ulong currentLevelId = NetGame.instance.currentLevel; 

                if (currentLevelId != lastLevelId)
                {
                    lastLevelId = currentLevelId;
                    LoadWorkshopMetadata();
                }
            }
        }
    }

    private void OnGUI()
    {
        if (showFPS)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 22;
            style.normal.textColor = Color.white;

            Rect areaRect = new Rect(10, 10, 400, 100); // 高度减小，只留标题和文字
            GUILayout.BeginArea(areaRect);

            GUILayout.Label("帧数: " + fps + " FPS", style);
            GUILayout.Label("延迟: " + latency.ToString("F0") + " ms", style);
            GUILayout.Label("时间: " + DateTime.Now.ToString("HH:mm:ss"), style);

            if (currentWorkshopMetadata != null && Game.instance != null && Game.currentLevel != null && Game.currentLevel.checkpoints != null)
            {
                int totalCheckpoints = Game.currentLevel.checkpoints.Length;
                int currentCheckpoint = Mathf.Clamp(Game.instance.currentCheckpointNumber, 0, totalCheckpoints - 1);

                // 标题和关卡进度
                string combinedText = $"{currentWorkshopMetadata.title} ：{currentCheckpoint}/{totalCheckpoints}";
                GUILayout.Label(combinedText, style);
            }

            GUILayout.EndArea();
        }
    }
}
