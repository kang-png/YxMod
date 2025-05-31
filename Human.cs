using System;
using System.Collections.Generic;
using HumanAPI;
using Multiplayer;
using UnityEngine;

public class Human : HumanBase
{


    public static Human instance;

    public static List<Human> all = new List<Human>();

    public bool spawning;

    private static Human localPlayer = null;

    public Vector3 targetDirection;

    public Vector3 targetLiftDirection;

    public bool jump;

    public bool disableInput;

    public NetPlayer player;

    public Ragdoll ragdoll;

    public HumanControls controls;

    internal GroundManager groundManager;

    internal GrabManager grabManager;

    [NonSerialized]
    public HumanMotion2 motionControl2;

    public HumanState state;

    public bool onGround;

    public float groundAngle;

    public bool hasGrabbed;

    public bool isClimbing;

    public Human grabbedByHuman;

    public float wakeUpTime;

    internal float maxWakeUpTime = 2f;

    public float unconsciousTime;

    private float maxUnconsciousTime = 3f;

    private Vector3 grabStartPosition;

    private HumanHead humanHead;

    [NonSerialized]
    public Rigidbody[] rigidbodies;

    private Vector3[] velocities;

    public float weight;

    public float mass;

    private Vector3 lastVelocity;

    private float totalHit;

    private float lastFrameHit;

    private float thisFrameHit;

    private float fallTimer;

    public float groundDelay;

    public float jumpDelay;

    private float slideTimer;

    private float[] groundAngles = new float[60];

    private int groundAnglesIdx;

    private float groundAnglesSum;

    private float lastGroundAngle;

    private uint evtScroll;

    private NetIdentity identity;

    private FixedJoint hook;

    public bool skipLimiting;

    private bool isFallSpeedInitialized;

    private bool isFallSpeedLimited;

    public bool overridenDrag;

    public static Human Localplayer
    {
        get
        {
            if (localPlayer != null && localPlayer.IsLocalPlayer)
            {
                return localPlayer;
            }
            foreach (Human item in all)
            {
                if (item != null && item.IsLocalPlayer)
                {
                    localPlayer = item;
                }
            }
            return localPlayer;
        }
    }

    public bool IsLocalPlayer => player != null && player.isLocalPlayer;

    public Vector3 momentum
    {
        get
        {
            Vector3 zero = Vector3.zero;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                Rigidbody rigidbody = rigidbodies[i];
                zero += rigidbody.velocity * rigidbody.mass;
            }
            return zero;
        }
    }

    public Vector3 velocity => momentum / mass;



    /// <增加的>
    public bool isClient;

    //屏蔽炸房相关
    public string lastFaYanStr;
    public int lastFaYanCount = 1;
    public float lastFaYanTimer;
    //挂机提醒
    public bool YiGuaJi;
    public float lastCaoZuoTime = Time.time;

    public float cameraPitchAngle;
    public float cameraYawAngle;
    public Human guajiNtpHuman;

    public bool liaotiankuangquanxian;
    public bool kejiquanxian;
    public bool jinzhibeikong;
    public bool wujiasi;
    public bool wupengzhuang;
    public DingDian dingdian;//个人定点

    public bool zuoxia;//坐下
    public bool yizuoxia;
    public float zuoxiaTime;
    public bool guixia;//跪下
    public float guixiaTime;
    public bool yizima;//一字马
    public bool titui;//踢腿
    public bool yititui;//
    public bool quanji;//Y5拳击
    public bool chuquan;//出拳
    public int numY;//Y   0 晕倒    1 坐下     2 跪下    3 一字马   4 踢腿   5拳王

    public Human ntp_human;
    public bool ntp;
    public bool fzntp;//房主的ntphuman
    public Vector3 ntp_Offset;

    public bool Fly; //飞天
    public bool shanxian; //闪现
    public bool flowing;
    public bool feitian;
    public float flowing_speed = 300f;
    public float extend_time_rush;
    public bool rush;

    public bool chaoren;//咸蛋超人
    public bool enshan;//咸蛋超人

    public bool qianshui;//潜水
    public bool pangxie;//螃蟹
    public bool tuique;//腿瘸
    public bool tuiguai;//腿拐
    public bool bengdi;//蹦迪
                       //public int tiaowu_i;
                       //public bool camPitch_mode;
    public bool diantun;//电臀
    public int diantun_i;
    public bool Diantun_Mod;
    public bool qiqiu;//气球
    public bool qiqiuxifa;//气球戏法
    public bool sanjitiao;//三级跳
    public bool tiaoing;//正在三级跳
    public int Jump_Times;
    public bool chaojitiao;//超级跳


    public float groundDelay2;
    public float jumpDelay2;



    public bool daoli;
    public bool zhuanquan; //转圈圈
    public bool ketouguai;
    public bool diaosigui;
    public float diaosiguiTime;

    public bool chaichu;

    public bool kongqipao;//空气炮
    public bool cannon_used;
    public float extend_time;
    public bool tuoluo;  //陀螺

    public bool dongjie;//冻结

    //public bool qianshou;//牵手
    public bool qianshou_zuo;//牵手左
    //public bool beiqianshou_zuo;//被牵手左 
    public Human qianshou_zuo_human;//牵手左
    public HumanSegment qianshou_zuo_humanHand;//牵手左humanHand



    public bool qianshou_you;//牵手右
    //public bool beiqianshou_you;//被牵手左
    public Human qianshou_you_human;//牵手右
    public HumanSegment qianshou_you_humanHand;//牵手左humanHand

    public bool banshen;

    private void YxModChuShiHua()
    {
        dingdian = new DingDian();

        if (this == Human.all[0])
        {
            Human.all[0].dingdian.huisu = UI_SheZhi.huisu;
            Human.all[0].dingdian.guanxing = UI_SheZhi.guanxing;
            Human.all[0].dingdian.q = UI_SheZhi.q;
            Human.all[0].dingdian.se = UI_SheZhi.se;
            Human.all[0].dingdian.gaodu = UI_SheZhi.gaodu;
            Human.all[0].dingdian.geshu = UI_SheZhi.geshu;//m_geshu = geshu;
            Human.all[0].dingdian.tishiStr = UI_SheZhi.tishiStr;
        }

        lastCaoZuoTime = Time.time;
        qianshou_zuo_humanHand = new HumanSegment();
        qianshou_you_humanHand = new HumanSegment();

        kejiquanxian = UI_WanJia.allkejiquanxian;
        liaotiankuangquanxian = UI_WanJia.allliaotiankuangquanxian;
        dingdian.kaiguan = UI_WanJia.alldingdian;
        wujiasi = UI_WanJia.allwujiasi;
        wupengzhuang = UI_WanJia.allwupengzhuang;
        feitian = UI_WanJia.allfeitian;
        if (feitian)
        {
            YxMod.SetFeiTian(this);
        }
        chaoren = UI_WanJia.allchaoren;
        shanxian = UI_WanJia.allshanxian;
        dongjie = UI_WanJia.alldongjie;
        if (dongjie)
        {
            YxMod.DongJie(this);
        }
        banshen = UI_WanJia.allbanshen;
        YxMod.BanShen(this);

        bengdi = UI_WanJia.allbengdi;
        if (bengdi)
        {
            YxMod.BengDi(this);
        }
        sanjitiao = UI_WanJia.allsanjitiao;
        diantun = UI_WanJia.alldiantun;
        if (diantun)
        {
            YxMod.DianTun(this);
        }
        chaojitiao = UI_WanJia.allchaojitiao;
        if (chaojitiao)
        { 
            YxMod.chaojitiao(this);

        }
        qiqiu = UI_WanJia.allqiqiu;
        if (qiqiu)
        {
            YxMod.QiQiu(this);
        }
        qiqiuxifa = UI_WanJia.allqiqiuxifa;
        daoli = UI_WanJia.alldaoli;
        if (daoli)
        {
            YxMod.DaoLi(this);
        }
        zhuanquan = UI_WanJia.allzhuanquan;
        tuoluo = UI_WanJia.alltuoluo;

        ketouguai = UI_WanJia.allketouguai;
        diaosigui = UI_WanJia.alldiaosigui;
        if (diaosigui)
        {
            YxMod.DiaoSiGui(this);
        }
        pangxie = UI_WanJia.allpangxie;
        qianshui = UI_WanJia.allqianshui;

        tuique = UI_WanJia.alltuique;
        if (tuique)
        {
            YxMod.TuiQue(this);
        }
        tuiguai = UI_WanJia.alltuiguai;
        if (tuiguai)
        {
            YxMod.TuiGuai(this);
        }
        chaichu = UI_WanJia.allchaichu;
        kongqipao = UI_WanJia.allkongqipao;


    }

    /// </增加的>
    /// 



    private void OnEnable()
    {
        all.Add(this);
        instance = this;
        grabManager = GetComponent<GrabManager>();
        groundManager = GetComponent<GroundManager>();
        motionControl2 = GetComponent<HumanMotion2>();
        controls = GetComponentInParent<HumanControls>();
    }

    private void OnDisable()
    {
        all.Remove(this);
    }

    public void Initialize()
    {
        ragdoll = GetComponentInChildren<Ragdoll>();
        motionControl2.Initialize();
        ServoSound componentInChildren = GetComponentInChildren<ServoSound>();
        humanHead = ragdoll.partHead.transform.gameObject.AddComponent<HumanHead>();
        humanHead.sounds = componentInChildren;
        humanHead.humanAudio = GetComponentInChildren<HumanAudio>();
        componentInChildren.transform.SetParent(humanHead.transform, worldPositionStays: false);
        InitializeBodies();
        ///增加/修改
        YxModChuShiHua();
        ///增加
	}

    private void InitializeBodies()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        velocities = new Vector3[rigidbodies.Length];
        mass = 0f;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody rigidbody = rigidbodies[i];
            if (rigidbody != null)
            {
                rigidbody.maxAngularVelocity = 10f;
                mass += rigidbody.mass;
            }
        }
        weight = mass * 9.81f;
    }

    internal void ReceiveHit(Vector3 impulse)
    {
        thisFrameHit = Mathf.Max(thisFrameHit, impulse.magnitude);
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (thisFrameHit + lastFrameHit > 30f)
        {
            MakeUnconscious();
            ReleaseGrab(3f);
        }
        lastFrameHit = thisFrameHit;
        thisFrameHit = 0f;
        jumpDelay -= Time.fixedDeltaTime;
        groundDelay -= Time.fixedDeltaTime;
        if (!disableInput)
        {
            ProcessInput();
        }
        LimitFallSpeed();
        Quaternion quaternion = Quaternion.Euler(controls.targetPitchAngle, controls.targetYawAngle, 0f);
        targetDirection = quaternion * Vector3.forward;
        targetLiftDirection = Quaternion.Euler(Mathf.Clamp(controls.targetPitchAngle, -70f, 80f), controls.targetYawAngle, 0f) * Vector3.forward;
        if (NetGame.isClient || ReplayRecorder.isPlaying)
        {
            return;
        }
        if (state == HumanState.Dead || state == HumanState.Unconscious || state == HumanState.Spawning)
        {
            controls.leftGrab = (controls.rightGrab = false);
            controls.shootingFirework = false;
        }
        groundAngle = 90f;
        groundAngle = Mathf.Min(groundAngle, ragdoll.partBall.sensor.groundAngle);
        groundAngle = Mathf.Min(groundAngle, ragdoll.partLeftFoot.sensor.groundAngle);
        groundAngle = Mathf.Min(groundAngle, ragdoll.partRightFoot.sensor.groundAngle);
        bool flag = hasGrabbed;
        //onGround = groundDelay <= 0f && groundManager.onGround;
        if (sanjitiao && UI_GongNeng.yulexitong_KaiGuan)
        {
            YxMod.SanJiTiao_Fun(this);
        }
        else
        {
            onGround = groundDelay <= 0f && groundManager.onGround;
        }
        hasGrabbed = grabManager.hasGrabbed;
        ragdoll.partBall.sensor.groundAngle = (ragdoll.partLeftFoot.sensor.groundAngle = (ragdoll.partRightFoot.sensor.groundAngle = 90f));
        if (hasGrabbed && base.transform.position.y < grabStartPosition.y)
        {
            grabStartPosition = base.transform.position;
        }
        if (hasGrabbed && base.transform.position.y - grabStartPosition.y > 0.5f)
        {
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
        if (flag != hasGrabbed && hasGrabbed)
        {
            grabStartPosition = base.transform.position;
        }
        if (state == HumanState.Spawning)
        {
            spawning = true;
            if (onGround)
            {
                MakeUnconscious();
            }
        }
        else
        {
            spawning = false;
        }
        ProcessUnconscious();
        if (state != HumanState.Dead && state != HumanState.Unconscious && state != HumanState.Spawning)
        {
            ProcessFall();
            if (onGround)
            {
                if (chaojitiao && UI_GongNeng.yulexitong_KaiGuan)
                {
                    if (controls.walkSpeed > 0f)
                    {
                        state = HumanState.Walk;
                    }
                    else
                    {
                        state = HumanState.Idle;
                    }
                }
                else if (controls.jump && jumpDelay <= 0f)
                {
                    state = HumanState.Jump;
                    jump = true;
                    jumpDelay = 0.5f;
                    groundDelay = 0.2f;
                }
                else if (controls.walkSpeed > 0f)
                {
                    state = HumanState.Walk;
                }
                else
                {
                    state = HumanState.Idle;
                }
            }
            else if (ragdoll.partLeftHand.sensor.grabObject != null || ragdoll.partRightHand.sensor.grabObject != null)
            {
                state = HumanState.Climb;
            }
        }
        if (skipLimiting)
        {
            skipLimiting = false;
            return;
        }
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 vector = velocities[i];
            Vector3 vector2 = rigidbodies[i].velocity;
            Vector3 vector3 = vector2 - vector;
            if (Vector3.Dot(vector, vector3) < 0f)
            {
                Vector3 normalized = vector.normalized;
                float magnitude = vector.magnitude;
                float value = 0f - Vector3.Dot(normalized, vector3);
                float num = Mathf.Clamp(value, 0f, magnitude);
                vector3 += normalized * num;
            }
            float num2 = 1000f * Time.deltaTime;
            if (vector3.magnitude > num2)
            {
                Vector3 vector4 = Vector3.ClampMagnitude(vector3, num2);
                vector2 -= vector3 - vector4;
                rigidbodies[i].velocity = vector2;
            }
            velocities[i] = vector2;
        }
        /////增加的内容
        UI_SheZhi.GuaJiTiXing_Fun(this);
        YxMod.ZuoXia_Fun(this);//坐下
        YxMod.YiZiMa_Fun(this);//一字马
        YxMod.JiFei_Fun(this);//踢左腿
        YxMod.QuanJi_Fun(this);//拳击

        dingdian.DingDian_Fun(this);//定点   
        YxMod.WuPengZhuang_Fun(this);//无碰撞
        YxMod.GuaJian_Fun(this);//挂件
        YxMod.WuJiaSi_Fun(this);//无假死

        if (!UI_GongNeng.yulexitong_KaiGuan)
        {
            if (sanjitiao) { sanjitiao = false; }
            if (bengdi) { YxMod.BengDi(this); }
            if (qianshui) { qianshui = false; }
            if (pangxie) { pangxie = false; }
            if (zhuanquan) { zhuanquan = false; }
            if (tuoluo) { tuoluo = false; }
            if (ketouguai) { ketouguai = false; }
            if (diaosigui) { YxMod.DiaoSiGui(this); }
            if (daoli) { YxMod.DaoLi(this); }
            if (chaichu) { chaichu = false; }
            if (kongqipao) { kongqipao = false; }
            if (tuique) { YxMod.TuiQue(this); }
            if (tuiguai) { YxMod.TuiGuai(this); }
            if (diantun) { YxMod.DianTun(this); }
            if (qiqiu) { YxMod.QiQiu(this); }

            if (chaojitiao) { YxMod.chaojitiao(this); }
        }
        YxMod.QianShou_Fun(this);//牵手
        YxMod.QianShui_Fun(this);//潜水                   
        YxMod.PangXie_Fun(this);//螃蟹                     
        YxMod.ZhuanQuan_Fun(this);//转圈
        YxMod.TuoLuo_Fun(this);  //陀螺
        YxMod.KeTouGuai_Fun(this);//磕头怪              
        YxMod.DiaoSiGui_Fun(this);//吊死鬼            
        YxMod.DaoLi_Fun(this);//倒立        
        YxMod.ChaiChu(this);//拆除        
        YxMod.KongQiPao_Fun(this);//空气炮                
        YxMod.TuiQue_Fun(this);//腿瘸
        YxMod.DianTun_Fun(this);//电臀        
        YxMod.QiQiu_Fun(this);//气球         
        YxMod.QiQiuXiFa_Fun(this);//气球戏法
        YxMod.chaojitiao(this);//超级跳

        YxMod.FeiTian_Fun(this); //飞天
        YxMod.ShanXian_Fun(this);//闪现
        YxMod.ChaoRen_Fun(this);//超人

        //YxMod.BanShen_Fun(this);//半身不遂
        YxMod.QuXiaoBanShen(this);
        

        /////增加的内容
    }

    private void ProcessInput()
    {
        if (!NetGame.isClient && !ReplayRecorder.isPlaying)
        {
            if (controls.unconscious)
            {
                if (numY == 0)
                {
                    MakeUnconscious();
                }
                else if (numY == 1)//坐下
                {
                    YxMod.ZuoXia(this);
                }
                else if (numY == 2)//跪下
                {
                    YxMod.ZuoXia(this, true);
                }
                else if (numY == 3)//一字马
                {
                    YxMod.YiZiMa(this);
                }
                else if (numY == 4)//踢腿
                {
                    YxMod.TiTui(this);
                }
                else if (numY == 5)//拳王
                {
                    quanji = true;
                    YxMod.QuanJiAnimation(this);
                }
            }
            else
            {
                yititui = false;
                titui = false;
                quanji = false;
            }
            //if (controls.unconscious)
            //{
            //	MakeUnconscious();
            //}
            if (motionControl2.enabled)
            {
                motionControl2.OnFixedUpdate();
            }
        }
    }

    private void PushGroundAngle()
    {
        float num = (lastGroundAngle = ((!onGround || !(groundAngle < 80f)) ? lastGroundAngle : groundAngle));
        groundAnglesSum -= groundAngles[groundAnglesIdx];
        groundAnglesSum += num;
        groundAngles[groundAnglesIdx] = num;
        groundAnglesIdx = (groundAnglesIdx + 1) % groundAngles.Length;
    }

    private void ProcessFall()
    {
        PushGroundAngle();
        bool flag = false;
        if (groundAnglesSum / (float)groundAngles.Length > 45f)
        {
            flag = true;
            slideTimer = 0f;
            onGround = false;
            state = HumanState.Slide;
        }
        else if (state == HumanState.Slide && groundAnglesSum / (float)groundAngles.Length < 37f && ragdoll.partBall.rigidbody.velocity.y > -1f)
        {
            slideTimer += Time.fixedDeltaTime;
            if (slideTimer < 0.003f)
            {
                onGround = false;
            }
        }
        if (!onGround && !flag)
        {
            if (fallTimer < 5f)
            {
                fallTimer += Time.deltaTime;
            }
            if (state == HumanState.Climb)
            {
                fallTimer = 0f;
            }
            if (fallTimer > 3f)
            {
                state = HumanState.FreeFall;
            }
            else if (fallTimer > 1f)
            {
                state = HumanState.Fall;
            }
        }
        else
        {
            fallTimer = 0f;
        }
    }

    private void ProcessUnconscious()
    {
        if (state == HumanState.Unconscious)
        {
            unconsciousTime -= Time.fixedDeltaTime;
            if (unconsciousTime <= 0f)
            {
                state = HumanState.Fall;
                wakeUpTime = maxWakeUpTime;
                unconsciousTime = 0f;
            }
        }
        if (wakeUpTime > 0f)
        {
            wakeUpTime -= Time.fixedDeltaTime;
            if (wakeUpTime <= 0f)
            {
                wakeUpTime = 0f;
            }
        }
    }

    public void MakeUnconscious(float time)
    {
        unconsciousTime = time;
        state = HumanState.Unconscious;
    }

    public void MakeUnconscious()
    {
        unconsciousTime = maxUnconsciousTime;
        state = HumanState.Unconscious;
    }

    public void Reset()
    {
        groundManager.Reset();
        grabManager.Reset();
        for (int i = 0; i < groundAngles.Length; i++)
        {
            groundAngles[i] = 0f;
        }
        groundAnglesSum = 0f;
        humanHead.Reset();
    }

    public void SpawnAt(Vector3 pos)
    {
        state = HumanState.Spawning;
        Vector3 vector = KillHorizontalVelocity();
        int num = 2;
        if (Game.currentLevel != null)
        {
            float maxHumanVelocity = Game.currentLevel.MaxHumanVelocity;
            if (vector.magnitude > maxHumanVelocity)
            {
                ControlVelocity(maxHumanVelocity, killHorizontal: false);
                vector = new Vector3(0f, 0f - maxHumanVelocity, 0f);
            }
        }
        Vector3 position = pos - vector * num - Physics.gravity * num * num / 2f;
        SetPosition(position);
        if (vector.magnitude < 5f)
        {
            AddRandomTorque(1f);
        }
        Reset();
    }

    public void SpawnAt(Transform spawnPoint, Vector3 offset)
    {
        SpawnAt(offset + spawnPoint.position);
    }

    public Vector3 LimitHorizontalVelocity(float max)
    {
        Rigidbody[] array = rigidbodies;
        Vector3 vector = velocity;
        Vector3 vector2 = vector;
        vector2.y = 0f;
        if (vector2.magnitude < max)
        {
            return vector;
        }
        vector2 -= Vector3.ClampMagnitude(vector2, max);
        vector -= vector2;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].velocity += -vector2;
        }
        return vector;
    }

    public Vector3 KillHorizontalVelocity()
    {
        Rigidbody[] array = rigidbodies;
        Vector3 vector = velocity;
        Vector3 vector2 = vector;
        vector2.y = 0f;
        vector -= vector2;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].velocity += -vector2;
        }
        return vector;
    }

    public Vector3 ControlVelocity(float maxVelocity, bool killHorizontal)
    {
        Rigidbody[] array = rigidbodies;
        Vector3 vector = velocity;
        Vector3 vector2 = vector;
        vector2.y = 0f;
        Vector3 vector3 = ((!killHorizontal) ? (Vector3.ClampMagnitude(vector, maxVelocity) - vector) : (Vector3.ClampMagnitude(vector - vector2, maxVelocity) - vector));
        for (int i = 0; i < array.Length; i++)
        {
            array[i].velocity += vector3;
        }
        return vector;
    }

    public void AddRandomTorque(float multiplier)
    {
        Vector3 torque = UnityEngine.Random.onUnitSphere * 100f * multiplier;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody body = rigidbodies[i];
            body.SafeAddTorque(torque, ForceMode.VelocityChange);
        }
    }

    private void Start()
    {
        identity = GetComponentInParent<NetIdentity>();
        if (identity != null)
        {
            evtScroll = identity.RegisterEvent(OnScroll);
        }
    }

    private void OnScroll(NetStream stream)
    {
        Vector3 scroll = NetVector3.Read(stream, 12).Dequantize(500f);
        Scroll(scroll);
    }

    public void SetPosition(Vector3 spawnPos)
    {
        if (!NetGame.isClient && !ReplayRecorder.isPlaying)
        {
            Vector3 scroll = spawnPos - base.transform.position;
            Scroll(scroll);
        }
    }

    private void Scroll(Vector3 scroll)
    {
        if (!NetGame.isClient && !ReplayRecorder.isPlaying)
        {
            base.transform.position += scroll;
        }
        if (player.isLocalPlayer)
        {
            CloudSystem.instance.Scroll(scroll);
            player.cameraController.Scroll(scroll);
            for (int i = 0; i < CloudBox.all.Count; i++)
            {
                CloudBox.all[i].FadeIn(1f);
            }
        }
        if (identity != null && (NetGame.isServer || ReplayRecorder.isRecording))
        {
            NetStream stream = identity.BeginEvent(evtScroll);
            NetVector3.Quantize(scroll, 500f, 12).Write(stream);
            identity.EndEvent();
        }
    }

    public void ReleaseGrab(float blockTime = 0f)
    {
        ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
        ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
    }

    public void ReleaseGrab(GameObject item, float blockTime = 0f)
    {
        if (ragdoll.partLeftHand.sensor.IsGrabbed(item))
        {
            ragdoll.partLeftHand.sensor.ReleaseGrab(blockTime);
        }
        if (ragdoll.partRightHand.sensor.IsGrabbed(item))
        {
            ragdoll.partRightHand.sensor.ReleaseGrab(blockTime);
        }
    }

    internal void Show()
    {
        UnityEngine.Object.Destroy(hook);
        SetPosition(new Vector3(0f, 50f, 0f));
    }

    internal void Hide()
    {
        SetPosition(new Vector3(0f, 500f, 0f));
        hook = ragdoll.partHead.rigidbody.gameObject.AddComponent<FixedJoint>();
    }

    public void SetDrag(float drag, bool external = true)
    {
        if (external || !overridenDrag)
        {
            overridenDrag = external;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].drag = drag;
            }
        }
    }

    public void ResetDrag()
    {
        overridenDrag = false;
        isFallSpeedInitialized = false;
        LimitFallSpeed();
    }

    private void LimitFallSpeed()
    {
        bool flag = Game.instance.state != GameState.PlayingLevel;
        if (isFallSpeedLimited != flag || !isFallSpeedInitialized)
        {
            isFallSpeedInitialized = true;
            isFallSpeedLimited = flag;
            if (flag)
            {
                SetDrag(0.1f, external: false);
            }
            else
            {
                SetDrag(0.05f, external: false);
            }
        }
    }

    public void SetFallState()
    {
        lastGroundAngle = 0f;
        groundAngle = 0f;
        state = HumanState.Fall;
    }
}
