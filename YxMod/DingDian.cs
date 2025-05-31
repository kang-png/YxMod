using Multiplayer;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class DingDian
{
    public bool q = true;
    public bool se = true;
    public bool kaiguan = true;
    public bool huisu;
    public bool guanxing;
    public float gaodu = 0.1f;
    public int geshu = 2;
    public string tishiStr = "已存点";    //定点提示文本

    private int m_geshu = 0;
    private int cundianId;
    private int cundianCount;
    private int qudianId;

    private bool E_anxia;
    private bool A_anxia;
    private bool D_anxia;

    private Dictionary<int, Vector3> now_positions = new Dictionary<int, Vector3>();
    private Dictionary<int, Vector3[]> now_rigi_speeds = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> now_rigi_inertiaTensors = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Vector3[]> now_rigi_positions = new Dictionary<int, Vector3[]>();
    private Dictionary<int, Quaternion[]> now_rigi_rotations = new Dictionary<int, Quaternion[]>();

    public DingDian()
    {
        if (NetGame.isServer)
        {
            DuQuSheZhi();
        }
        else if (NetGame.isClient)
        {
            UI_SheZhi.KeJiChuShiHua();
        }
    }
    private void DuQuSheZhi()
    {

        //Debug.Log("DuQuSheZhi");
    }

    public void ChuShiHuaUpdata()
    {
        if (m_geshu == geshu)
        {
            return;
        }
        m_geshu = geshu;
        cundianId = qudianId = cundianCount = 0;
        now_positions.Clear();
        now_rigi_speeds.Clear();
        now_rigi_inertiaTensors.Clear();
        now_rigi_positions.Clear();
        now_rigi_rotations.Clear();

        for (int i = 1; i <= geshu; i++)
        {
            now_positions.Add(i, Vector3.zero);
            now_rigi_speeds.Add(i, new Vector3[30]);
            now_rigi_inertiaTensors.Add(i, new Vector3[30]);
            now_rigi_positions.Add(i, new Vector3[30]);
            now_rigi_rotations.Add(i, new Quaternion[30]);
        }
        //Debug.Log("定点数改变,所有定点已清空");
        //Chat.TiShi(NetGame.instance.local,"定点数改变,所有定点已清空");
    }

    public static void Update()
    {
        if ((!NetGame.isServer && !NetGame.isClient) || NetChat.typing || Shell.visible)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (NetGame.isServer)
            {
                if (!UI_GongNeng.dingdian_KaiGuan)
                {
                    //Chat.TiShi(Human.all[0].player.host, $"定点系统已关闭");
                    return;
                }
                if (!Human.all[0].dingdian.kaiguan)//Human.all[0].dingdian.
                {
                    //Chat.TiShi(Human.all[0].player.host, $"你的个人定点已关闭");
                    return;
                }
                //if (Human.all[0].dingdian.DingDianFangShi == 0 || Human.all[0].dingdian.DingDianFangShi == 2)
                if (Human.all[0].dingdian.q)
                {
                    Human.all[0].dingdian.CunDian(Human.all[0], true);//Human.all[0].dingdian.
                }
            }
            else if (NetGame.isClient)
            {
                Human human2 = NetGame.instance.local.players[0].human;
                human2.player.SendMove(-1f, human2.controls.walkLocalDirection.x, human2.controls.cameraPitchAngle, human2.controls.cameraYawAngle, human2.controls.leftExtend, human2.controls.rightExtend, human2.controls.jump, false, false);
                human2.player.SendMove(-1f, human2.controls.walkLocalDirection.x, human2.controls.cameraPitchAngle, human2.controls.cameraYawAngle, human2.controls.leftExtend, human2.controls.rightExtend, human2.controls.jump, false, true);
                human2.player.SendMove(-1f, human2.controls.walkLocalDirection.x, human2.controls.cameraPitchAngle, human2.controls.cameraYawAngle, human2.controls.leftExtend, human2.controls.rightExtend, human2.controls.jump, false, false);
            }

        }
    }

    public void DingDian_Fun(Human human)
    {

        if (!UI_GongNeng.dingdian_KaiGuan || !human.dingdian.kaiguan)
        {
            return;//Chat.TiShi(human.player.host, $"定点系统已关闭");
        }

        ChuShiHuaUpdata();

        if (human.controls.walkLocalDirection.x >= 0f && human.dingdian.A_anxia)
        {
            human.dingdian.A_anxia = false;
        }
        if (human.controls.walkLocalDirection.x <= 0f && human.dingdian.D_anxia)
        {
            human.dingdian.D_anxia = false;
        }
        if (!human.controls.shootingFirework && human.dingdian.E_anxia)
        {
            human.dingdian.E_anxia = false;
        }

        if (human.controls.shootingFirework)
        {
            //S+E
            if (human.controls.walkLocalDirection.z < 0f && !human.dingdian.E_anxia)
            {
                human.dingdian.E_anxia = true;
                if (human.dingdian.se)
                {
                    CunDian(human, true);
                }
            }
            //A+E
            else if (human.controls.walkLocalDirection.x < 0f && !human.dingdian.A_anxia && !human.dingdian.E_anxia)//&& !human_ShuXing.A_anxia
            {
                human.dingdian.A_anxia = true;
                CunDian(human, false, -1);
            }
            //D+E
            else if (human.controls.walkLocalDirection.x > 0f && !human.dingdian.D_anxia && !human.dingdian.E_anxia)// && !human_ShuXing.D_anxia
            {
                human.dingdian.D_anxia = true;
                CunDian(human, false, 1);
            }
            //取点
            else if (!human.dingdian.E_anxia)
            {
                CunDian(human, false);
            }
        }
    }

    public void CunDian(Human human, bool isCunDian, int add = 0)//时空回溯
    {
        if (isCunDian)
        {
            cundianId += 1;
            if (cundianId > geshu)
            {
                cundianId = 1;
            }
            cundianCount = Math.Max(cundianCount, cundianId);
            qudianId = cundianId;

            now_positions[cundianId] = human.transform.position;
            for (int i = 0; i < human.rigidbodies.Length; i++)
            {
                //now_rigi_speed[i] = human.rigidbodies[i].velocity;
                now_rigi_speeds[cundianId][i] = human.rigidbodies[i].velocity;
                now_rigi_positions[cundianId][i] = human.rigidbodies[i].transform.position;

                now_rigi_rotations[cundianId][i] = human.rigidbodies[i].transform.rotation;

                now_rigi_inertiaTensors[cundianId][i] = human.rigidbodies[i].inertiaTensor;

            }
            Chat.TiShi(human.player.host, $"{cundianId}:{tishiStr}", TiShiMsgId.GeRenTiShi);
            return;
        }

        if (cundianCount == 0)
        {
            return;
        }
        if (add == -1)
        {
            qudianId -= 1;
            if (qudianId == 0)
            {
                qudianId = cundianCount; //geshu;
            }
        }
        else if (add == 1)
        {
            qudianId += 1;
            if (qudianId == cundianCount + 1)
            {
                qudianId = 1; //geshu;
            }
        }

        //if (!now_positions.ContainsKey(qudianId))//now_positions[qudianId] == Vector3.zero
        //{
        //    return;
        //}


        if (!huisu)
        {
            human.transform.position = now_positions[qudianId] + new Vector3(0, gaodu, 0);
            for (int j = 0; j < human.rigidbodies.Length; j++)
            {
                human.rigidbodies[j].velocity = guanxing ? (now_rigi_speeds[qudianId][j]) : Vector3.zero;
            }
            return;
        }
        for (int j = 0; j < human.rigidbodies.Length; j++)
        {
            human.rigidbodies[j].transform.position = now_rigi_positions[qudianId][j];
            human.rigidbodies[j].transform.rotation = now_rigi_rotations[qudianId][j];
            human.rigidbodies[j].inertiaTensor = now_rigi_inertiaTensors[qudianId][j];
            human.rigidbodies[j].velocity = guanxing ? (now_rigi_speeds[qudianId][j]) : Vector3.zero;
        }
    }
}
