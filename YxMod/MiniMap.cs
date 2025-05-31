using Multiplayer;
using UnityEngine;


public class MiniMap : MonoBehaviour
{
    private static Camera camera_0;

    private static GameObject gameObject_0;

    private static RenderTexture renderTexture_0;

    private float float_0;

    public bool SetMiniMap;

    public static MiniMap instance;

    private Color color_0;

    private RaycastHit raycastHit_0;

    private GameObject gameObject_1;

    private int int_0;

    private int int_1;

    private int int_2;

    private int int_3;

    private void Awake()
    {
        instance = this;
    }

    private void OnGUI()
    {
        if ( !SetMiniMap || !(GameObject.Find("Level") != null))//UI.Home ||
        {
            return;
        }
        try
        {
            if (camera_0 == null)
            {
                gameObject_0 = new GameObject();
                camera_0 = gameObject_0.AddComponent<Camera>();
                camera_0.tag = "Untagged";
                float_0 = 10f;
            }
            if (renderTexture_0 == null)
            {
                renderTexture_0 = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            }
            if (!renderTexture_0.IsCreated())
            {
                renderTexture_0.Create();
            }
            if ((camera_0 != null) && (gameObject_0 != null))
            {
                gameObject_0.transform.position = NetGame.instance.local.players[0].human.transform.position + new Vector3(0f, float_0, 0f);
                Vector3 vector = NetGame.instance.local.players[0].human.transform.rotation * Vector3.one;
                camera_0.transform.rotation = Quaternion.Euler(90f, NetGame.instance.local.players[0].human.controls.cameraYawAngle, vector.z);
            }
            if (Input.GetAxis("Mouse ScrollWheel") != 0f && float_0 > 6f)
            {
                float_0 += Input.GetAxis("Mouse ScrollWheel") * 3f;
                if (float_0 < 6f)
                {
                    float_0 = 6.1f;
                }
            }
            camera_0.targetTexture = renderTexture_0;
            GUILayout.Box(renderTexture_0);
        }
        catch
        {
        }
    }

    private void FixedUpdate()
    {
        if (GameObject.Find("Level") == null)
        {
            gameObject_1 = null;
        }
        else
        {
            if ((SetMiniMap ) || (!SetMiniMap && gameObject_1 == null))//&& UI.Home
            {
                return;
            }
            try
            {
                if (SetMiniMap && (GameObject.Find("Level") != null))
                {
                    Vector3 vector = transform.TransformDirection(Vector3.down);
                    if (Physics.Raycast(gameObject_0.transform.position - Vector3.down, vector, out raycastHit_0) && raycastHit_0.collider.gameObject.GetComponent<MeshRenderer>() != null)
                    {
                        Material material = raycastHit_0.collider.gameObject.GetComponent<MeshRenderer>().material;
                        if (gameObject_1 == null || gameObject_1 != raycastHit_0.collider.gameObject)
                        {
                            if (gameObject_1 != null && gameObject_1 != raycastHit_0.collider.gameObject)
                            {
                                Material material2 = gameObject_1.GetComponent<MeshRenderer>().material;
                                material2.color = color_0;
                                material2.SetInt("_SrcBlend", int_0);
                                material2.SetInt("_DstBlend", int_1);
                                material2.SetInt("_ZWrite", int_2);
                                material2.renderQueue = int_3;
                            }
                            gameObject_1 = raycastHit_0.collider.gameObject;
                            color_0 = new Color(material.color.r, material.color.g, material.color.b, material.color.a);
                            material.color = new Color(material.color.r, material.color.g, material.color.b, 0f);
                            int_0 = material.GetInt("_SrcBlend");
                            material.SetInt("_SrcBlend", 1);
                            int_1 = material.GetInt("_DstBlend");
                            material.SetInt("_DstBlend", 2);
                            int_2 = material.GetInt("_ZWrite");
                            material.SetInt("_ZWrite", 0);
                            material.DisableKeyword("_ALPHATEST_ON");
                            material.DisableKeyword("_ALPHABLEND_ON");
                            material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                            int_3 = material.renderQueue;
                            material.renderQueue = 3000;
                        }
                    }
                    else if (gameObject_1 != null && gameObject_1 != raycastHit_0.collider.gameObject)
                    {
                        Material material3 = gameObject_1.GetComponent<MeshRenderer>().material;
                        material3.color = color_0;
                        material3.SetInt("_SrcBlend", int_0);
                        material3.SetInt("_DstBlend", int_1);
                        material3.SetInt("_ZWrite", int_2);
                        material3.renderQueue = int_3;
                        gameObject_1 = null;
                    }
                }
                else if (GameObject.Find("Level") != null && gameObject_1 != null)
                {
                    Material material4 = gameObject_1.GetComponent<MeshRenderer>().material;
                    material4.color = color_0;
                    material4.SetInt("_SrcBlend", int_0);
                    material4.SetInt("_DstBlend", int_1);
                    material4.SetInt("_ZWrite", int_2);
                    material4.renderQueue = int_3;
                    gameObject_1 = null;
                }
            }
            catch
            {
            }
        }
    }
}
