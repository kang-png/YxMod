using DigitalOpus.MB.Core;
using UnityEngine;

public class MB2_MeshBakerLODTestHarness : MonoBehaviour
{
	private class Test
	{
		public enum Action
		{
			none,
			disable,
			enable,
			destroy,
			activate,
			deactivate,
			custom
		}

		public enum When
		{
			fixedUpdate,
			update,
			lateUpdate,
			preRender,
			postRender
		}

		public MB2_LOD target;

		public MB2_LODCamera camera;

		public MB2_LODManager manager;

		public int level;

		public bool doBake;

		public Action act;

		public When whenToAct = When.update;

		public float[] distances = new float[5] { 10f, 30f, 70f, 150f, 250f };

		public bool int_inCombined;

		public bool int_inQueue;

		public MB2_LODOperation int_action;

		public int int_currentIdx;

		public int int_nextIdx;

		public bool fin_inCombined;

		public bool fin_inQueue;

		public MB2_LODOperation fin_action;

		public int fin_currentIdx;

		public int fin_nextIdx;

		public CustomAction customAction;

		public Test()
		{
		}

		public Test(int level, bool doBake, bool int_inCombined, bool int_inQueue, MB2_LODOperation int_action, int int_nextIdx, int int_currentIdx, bool fin_inCombined, bool fin_inQueue, MB2_LODOperation fin_action, int fin_nextIdx, int fin_currentIdx, Action act = Action.none, When when = When.update, CustomAction a = null)
		{
			this.level = level;
			this.doBake = doBake;
			this.int_inCombined = int_inCombined;
			this.int_inQueue = int_inQueue;
			this.int_action = int_action;
			this.int_currentIdx = int_currentIdx;
			this.int_nextIdx = int_nextIdx;
			this.fin_inCombined = fin_inCombined;
			this.fin_inQueue = fin_inQueue;
			this.fin_action = fin_action;
			this.fin_currentIdx = fin_currentIdx;
			this.fin_nextIdx = fin_nextIdx;
			this.act = act;
			whenToAct = when;
			customAction = a;
		}

		public void SetupTest(MB2_LOD targ, MB2_LODCamera cam, MB2_LODManager m)
		{
			target = targ;
			camera = cam;
			float num = distances[level];
			Debug.Log("fr=" + Time.frameCount + " PreRender SetupTest moving to dist " + num);
			Vector3 position = cam.transform.position;
			position.z = num;
			cam.transform.position = position;
			manager = m;
			manager.baking_enabled = doBake;
		}

		public void CheckStateBetweenUpdateAndBake()
		{
			Debug.Log("fr=" + Time.frameCount + " CheckStateBetweenUpdateAndBake");
			target.CheckState(int_inCombined, int_inQueue, int_action, int_nextIdx, int_currentIdx);
		}

		public void CheckStateAfterBake()
		{
			Debug.Log("fr=" + Time.frameCount + " CheckStateAfterBake");
			target.CheckState(fin_inCombined, fin_inQueue, fin_action, fin_nextIdx, fin_currentIdx);
		}

		public void DoActions()
		{
			Debug.Log(string.Concat("fr=", Time.frameCount, " DoActions ", act, " ", whenToAct));
			if (act == Action.activate)
			{
				MB2_Version.SetActive(target.gameObject, isActive: true);
			}
			if (act == Action.deactivate)
			{
				MB2_Version.SetActive(target.gameObject, isActive: false);
			}
			if (act == Action.enable)
			{
				target.enabled = true;
			}
			if (act == Action.disable)
			{
				target.enabled = false;
			}
			if (act == Action.destroy)
			{
				MB2_LODManager.Manager().LODDestroy(target);
			}
			if (act == Action.custom)
			{
				customAction.DoAction();
			}
		}
	}

	private interface CustomAction
	{
		void DoAction();
	}

	private class ActionForceToLevel : CustomAction
	{
		private int level;

		public ActionForceToLevel(int l)
		{
			level = l;
		}

		public void DoAction()
		{
			harness.lod.forceToLevel = level;
			Debug.Log("ActionForceToLevel forcing to " + level);
		}
	}

	public static MB2_MeshBakerLODTestHarness harness;

	private MB2_LODManager manager;

	private MB2_LODCamera cam;

	public MB2_LOD lod;

	private Test[] tests = new Test[28]
	{
		new Test(2, doBake: true, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 2, 2),
		new Test(3, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.update, 3, 2, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 3, 3),
		new Test(1, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.update, 1, 3, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 1, 1),
		new Test(4, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 1, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(2, doBake: false, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: false, fin_inQueue: true, MB2_LODOperation.toAdd, 2, 4),
		new Test(4, doBake: true, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 4, 4, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(2, doBake: false, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: false, fin_inQueue: true, MB2_LODOperation.toAdd, 2, 4),
		new Test(0, doBake: false, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 0, 0, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 0, 0),
		new Test(4, doBake: false, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 4, 4, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(2, doBake: false, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: false, fin_inQueue: true, MB2_LODOperation.toAdd, 2, 4),
		new Test(0, doBake: false, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 0, 0, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 0, 0),
		new Test(3, doBake: true, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 3, 0, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 3, 3),
		new Test(4, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 3, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(2, doBake: true, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 2, 2),
		new Test(4, doBake: false, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 2, fin_inCombined: true, fin_inQueue: true, MB2_LODOperation.delete, 4, 2),
		new Test(3, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.update, 3, 2, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 3, 3),
		new Test(0, doBake: false, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 0, 3, fin_inCombined: true, fin_inQueue: true, MB2_LODOperation.delete, 0, 3),
		new Test(4, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 3, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(4, doBake: true, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 4, 4, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4, Test.Action.disable, Test.When.lateUpdate),
		new Test(1, doBake: true, int_inCombined: false, int_inQueue: false, MB2_LODOperation.none, 4, 4, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4, Test.Action.enable, Test.When.lateUpdate),
		new Test(1, doBake: true, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 1, 4, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 1, 1),
		new Test(4, doBake: false, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 1, fin_inCombined: true, fin_inQueue: true, MB2_LODOperation.delete, 4, 1, Test.Action.disable, Test.When.lateUpdate),
		new Test(1, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 1, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4),
		new Test(2, doBake: true, int_inCombined: false, int_inQueue: true, MB2_LODOperation.toAdd, 2, 4, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 2, 2, Test.Action.enable, Test.When.fixedUpdate),
		new Test(3, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.update, 1, 2, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 1, 1, Test.Action.custom, Test.When.fixedUpdate, new ActionForceToLevel(1)),
		new Test(4, doBake: true, int_inCombined: true, int_inQueue: false, MB2_LODOperation.none, 1, 1, fin_inCombined: true, fin_inQueue: false, MB2_LODOperation.none, 1, 1, Test.Action.custom, Test.When.preRender, new ActionForceToLevel(-1)),
		new Test(3, doBake: false, int_inCombined: true, int_inQueue: true, MB2_LODOperation.update, 3, 1, fin_inCombined: true, fin_inQueue: true, MB2_LODOperation.delete, 4, 1, Test.Action.destroy, Test.When.preRender),
		new Test(3, doBake: true, int_inCombined: true, int_inQueue: true, MB2_LODOperation.delete, 4, 1, fin_inCombined: false, fin_inQueue: false, MB2_LODOperation.none, 4, 4)
	};

	private Test currentTest;

	private int testNum;

	private void Start()
	{
		harness = this;
		manager = MB2_LODManager.Manager();
		manager.checkScheduler.FORCE_CHECK_EVERY_FRAME = true;
		cam = GetComponent<MB2_LODCamera>();
		manager.LOG_LEVEL = MB2_LogLevel.trace;
		lod.LOG_LEVEL = MB2_LogLevel.trace;
	}

	private void FixedUpdate()
	{
		if (currentTest != null && currentTest.whenToAct == Test.When.fixedUpdate)
		{
			currentTest.DoActions();
		}
	}

	private void Update()
	{
		if (currentTest != null && currentTest.whenToAct == Test.When.update)
		{
			currentTest.DoActions();
		}
		for (int i = 0; i < manager.bakers.Length; i++)
		{
			manager.bakers[i].baker.LOG_LEVEL = MB2_LogLevel.trace;
		}
	}

	private void LateUpdate()
	{
		if (currentTest != null)
		{
			currentTest.CheckStateBetweenUpdateAndBake();
		}
		if (currentTest != null && currentTest.whenToAct == Test.When.lateUpdate)
		{
			currentTest.DoActions();
		}
	}

	private void OnPreRender()
	{
		if (currentTest != null && currentTest.whenToAct == Test.When.preRender)
		{
			currentTest.DoActions();
		}
	}

	private void OnPostRender()
	{
		if (currentTest != null)
		{
			currentTest.CheckStateAfterBake();
		}
		if (currentTest != null && currentTest.whenToAct == Test.When.postRender)
		{
			currentTest.DoActions();
		}
		currentTest = null;
		if (testNum >= tests.Length)
		{
			Debug.Log("Done testing");
			return;
		}
		Debug.Log("fr=" + Time.frameCount + " ======= starting test " + testNum);
		currentTest = tests[testNum++];
		currentTest.SetupTest(lod, cam, manager);
	}
}
