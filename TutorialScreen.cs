using HumanAPI;
using I2.Loc;
using UnityEngine;
using UnityEngine.Video;

public class TutorialScreen : MonoBehaviour
{
	public string videoName;

	public TextAsset subtitlesAsset;

	public Material blankMaterial;

	public Material movieMaterial;

	public Sound2 onSound;

	public Sound2 offSound;

	public Sound2 tutSound;

	public float width = 16f;

	private Renderer renderer;

	private BoxCollider collider;

	private VideoPlayer movie;

	private bool videoLoaded;

	private bool reinitMaterial;

	private AudioSource audiosource;

	private MeshFilter filter;

	private SrtSubtitles subtitles;

	private Mesh mesh;

	private Vector3[] meshVerts;

	private Vector2[] uvs;

	private Material movieMaterialInstance;

	private float endTimer;

	private static TutorialScreen playingScreen;

	private float pendingPlay = -1f;

	private float audiosourceStart;

	private float transitionSpeed;

	private float transitionPhase;

	private bool inTransition;

	private SrtSubtitles.SrtLine currentLine;

	public float time;

	public bool isVisible;

	public static bool lockedVOAndSubtitles { get; protected set; }

	private float audiosourceTime
	{
		get
		{
			if (audiosource == null)
			{
				return 0f;
			}
			float num = audiosource.time;
			if (float.IsInfinity(num) || float.IsNaN(num))
			{
				num = 1f * (float)audiosource.timeSamples / 48000f;
			}
			if (num == 0f)
			{
				return Time.unscaledTime - audiosourceStart;
			}
			return num;
		}
	}

	private void Start()
	{
		LoadMovie(videoName);
		TextAsset videoSrt = HFFResources.instance.GetVideoSrt(videoName + "Srt");
		subtitles = new SrtSubtitles();
		subtitles.Load(videoName, videoSrt.text);
		videoSrt = null;
		collider = GetComponent<BoxCollider>();
		renderer = GetComponent<Renderer>();
		filter = GetComponent<MeshFilter>();
		mesh = new Mesh();
		mesh.name = "rope " + base.name;
		meshVerts = new Vector3[8];
		uvs = new Vector2[8];
		Create2SidedMesh(0f, -4.5f, 0f, 4.5f, 0f, 0f, 1f, 1f);
		mesh.vertices = meshVerts;
		mesh.uv = uvs;
		mesh.triangles = new int[12]
		{
			0, 1, 2, 0, 2, 3, 4, 5, 6, 4,
			6, 7
		};
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		filter.sharedMesh = mesh;
	}

	private void PrepareCompleted(VideoPlayer source)
	{
		source.isLooping = false;
		videoLoaded = true;
	}

	private void LoadMovie(string file)
	{
		if (movie == null)
		{
			movie = base.gameObject.GetComponent<VideoPlayer>();
			if (movie == null)
			{
				movie = base.gameObject.AddComponent<VideoPlayer>();
			}
		}
		if (audiosource == null)
		{
			audiosource = base.gameObject.GetComponent<AudioSource>();
			if (audiosource == null)
			{
				audiosource = base.gameObject.AddComponent<AudioSource>();
			}
			if (audiosource != null)
			{
				VideoLogMenu menu = MenuSystem.instance.GetMenu<VideoLogMenu>();
				if (menu != null)
				{
					audiosource.outputAudioMixerGroup = menu.audioSource.outputAudioMixerGroup;
				}
			}
		}
		movie.playOnAwake = false;
		audiosource.playOnAwake = false;
		movie.source = VideoSource.Url;
		string url = Application.streamingAssetsPath + "/Videos/" + file + "Video.mp4";
		movie.url = url;
		movie.audioOutputMode = VideoAudioOutputMode.AudioSource;
		movie.controlledAudioTrackCount = 1;
		movie.EnableAudioTrack(0, enabled: true);
		movie.SetTargetAudioSource(0, audiosource);
		movie.prepareCompleted += PrepareCompleted;
		movie.Prepare();
	}

	private void Update()
	{
		if (pendingPlay > 0f)
		{
			float num = pendingPlay - Time.deltaTime;
			Show();
			pendingPlay = num;
		}
		if (!videoLoaded)
		{
			return;
		}
		if (endTimer > 0f)
		{
			endTimer -= Time.deltaTime;
			if (endTimer <= 0f)
			{
				StopMovie();
			}
		}
		if (movie != null && movieMaterialInstance == null)
		{
			if (movie.texture != null)
			{
				movieMaterialInstance = Object.Instantiate(HFFResources.instance.PCLinearMovieFixGame);
			}
			reinitMaterial = true;
		}
		if (reinitMaterial && movie != null && movieMaterialInstance != null)
		{
			reinitMaterial = false;
			if (movieMaterialInstance != null)
			{
				movieMaterialInstance.mainTexture = movie.texture;
				movieMaterialInstance.SetTexture("_EmissionMap", movie.texture);
			}
			renderer.sharedMaterial = movieMaterialInstance;
		}
		if (movie != null && subtitles != null && transitionPhase != 0f && transitionSpeed > 0f && inTransition)
		{
			time = audiosourceTime;
			if (currentLine != null && !currentLine.ShouldShow(time))
			{
				if (lockedVOAndSubtitles)
				{
					SubtitleManager.instance.ClearSubtitle();
				}
				currentLine = null;
			}
			if (currentLine == null)
			{
				currentLine = subtitles.GetLineToDisplay(time);
				if (currentLine != null && lockedVOAndSubtitles)
				{
					SubtitleManager.instance.SetSubtitle(ScriptLocalization.Get("SUBTITLES/" + currentLine.key));
				}
			}
		}
		if (!inTransition)
		{
			return;
		}
		if (transitionPhase == 0f && transitionSpeed > 0f && movie != null)
		{
			renderer.sharedMaterial = movieMaterialInstance;
			audiosourceStart = Time.unscaledTime;
			lockedVOAndSubtitles = true;
			movie.Play();
		}
		transitionPhase = Mathf.Clamp01(transitionPhase + Time.deltaTime * transitionSpeed);
		float num2 = Ease.easeInOutQuad(0f, 1f, transitionPhase);
		float num3 = width / 16f * 9f;
		Create2SidedMesh((0f - width) / 2f * num2, (0f - num3) / 2f, width / 2f * num2, num3 / 2f, 0.5f - num2 / 2f, 0f, 0.5f + num2 / 2f, 1f);
		mesh.vertices = meshVerts;
		mesh.uv = uvs;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		filter.sharedMesh = mesh;
		collider.size = ((num2 != 0f) ? new Vector3(width * num2, num3, 0.1f) : Vector3.zero);
		if (transitionPhase == 0f && transitionSpeed < 0f)
		{
			if (playingScreen == this)
			{
				playingScreen = null;
			}
			inTransition = false;
			StopMovie();
			RemoveSubtitles();
			renderer.sharedMaterial = blankMaterial;
			GrabManager.Release(base.gameObject);
		}
	}

	internal void StopXBAudio()
	{
	}

	internal void RemoveSubtitles()
	{
		if (currentLine != null || lockedVOAndSubtitles)
		{
			SubtitleManager.instance.ClearSubtitle();
		}
		currentLine = null;
		lockedVOAndSubtitles = false;
	}

	internal void StopMovie()
	{
		if (!(movie == null))
		{
			movie.Stop();
			if (videoLoaded)
			{
				videoLoaded = false;
				movie.Prepare();
			}
			reinitMaterial = true;
			StopXBAudio();
			RemoveSubtitles();
		}
	}

	public void OnDestroy()
	{
		InstantKill();
	}

	public void OnDisable()
	{
		InstantKill();
	}

	protected void InstantKill()
	{
		if (playingScreen == this)
		{
			StopMovie();
			RemoveSubtitles();
			playingScreen = null;
			transitionSpeed = -2f;
		}
	}

	private void Create2SidedMesh(float xMin, float yMin, float xMax, float yMax, float uMin, float vMin, float uMax, float vMax)
	{
		ref Vector3 reference = ref meshVerts[0];
		ref Vector3 reference2 = ref meshVerts[7];
		reference = (reference2 = new Vector3(xMin, yMin, 0f));
		ref Vector3 reference3 = ref meshVerts[1];
		ref Vector3 reference4 = ref meshVerts[6];
		reference3 = (reference4 = new Vector3(xMin, yMax, 0f));
		ref Vector3 reference5 = ref meshVerts[2];
		ref Vector3 reference6 = ref meshVerts[5];
		reference5 = (reference6 = new Vector3(xMax, yMax, 0f));
		ref Vector3 reference7 = ref meshVerts[3];
		ref Vector3 reference8 = ref meshVerts[4];
		reference7 = (reference8 = new Vector3(xMax, yMin, 0f));
		ref Vector2 reference9 = ref uvs[0];
		ref Vector2 reference10 = ref uvs[4];
		reference9 = (reference10 = new Vector2(uMin, vMin));
		ref Vector2 reference11 = ref uvs[1];
		ref Vector2 reference12 = ref uvs[5];
		reference11 = (reference12 = new Vector2(uMin, vMax));
		ref Vector2 reference13 = ref uvs[2];
		ref Vector2 reference14 = ref uvs[6];
		reference13 = (reference14 = new Vector2(uMax, vMax));
		ref Vector2 reference15 = ref uvs[3];
		ref Vector2 reference16 = ref uvs[7];
		reference15 = (reference16 = new Vector2(uMax, vMin));
	}

	internal void Show()
	{
		if (playingScreen != null || isVisible || inTransition)
		{
			pendingPlay = 1f;
			return;
		}
		pendingPlay = -1f;
		isVisible = true;
		playingScreen = this;
		inTransition = true;
		transitionSpeed = 0.8f;
		MusicManager.instance.Pause();
		onSound.PlayOneShot();
		lockedVOAndSubtitles = true;
		currentLine = null;
		SubtitleManager.instance.ClearSubtitle();
	}

	internal void Hide()
	{
		pendingPlay = -1f;
		if (!(playingScreen != this))
		{
			isVisible = false;
			offSound.PlayOneShot();
			inTransition = true;
			transitionSpeed = -2f;
			MusicManager.instance.Resume();
			StopXBAudio();
			RemoveSubtitles();
		}
	}
}
