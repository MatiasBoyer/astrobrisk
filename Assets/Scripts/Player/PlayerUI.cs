using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    public RectTransform CanvasRect;
    public Camera UI_Cam;

    public RectTransform G_Curtain;

    [Header("Game")]
    public TextMeshProUGUI G_SpeedUPS;
    private float SpeedUPS;

    [Header("Game:MoneyVisualizer")]
    public GameObject G_MV_MoneyTextPrefab;
    public Transform G_MV_MoneyParent;
    public TextMeshProUGUI G_MV_MoneyViewer;

    [Header("Game:RetryScreen")]
    public GameObject G_RS_RetryScreen;
    public TextMeshProUGUI G_RS_MaxDistance;

    [Header("Game:MusicPlayer")]
    public RectTransform G_MP_CurrentSongParent;
    public TextMeshProUGUI G_MP_CurrentSong;

    private Vector3 finalPos;
    private PlayerController PControl;
    private Rigidbody Rb;

    private CanvasScaler CScaler;

    private PostProcessVolume ppv;
    private LensDistortion ldi;

    private void Start()
    {
        PControl = GetComponent<PlayerController>();
        CScaler = CanvasRect.GetComponent<CanvasScaler>();
        Rb = PControl.Rb3d;

        ppv = Camera.main.GetComponent<PostProcessVolume>();
        ppv.profile.TryGetSettings(out ldi);

        LeanTween.move(G_Curtain, new Vector2(0, 0), 0).setOnComplete(() => {
            G_Curtain.gameObject.SetActive(true);
            LeanTween.move(G_Curtain, new Vector2(-2000, 0), 1.0f).setOnComplete(() => { G_Curtain.gameObject.SetActive(false); });
        });
    }

    private void FixedUpdate()
    {
        //SPEED
        float sups = PControl.MovementSpeed; //Rb.velocity.magnitude + PControl.MovementSpeed;
        SpeedUPS = Mathf.Lerp(SpeedUPS, sups, Time.deltaTime * 2.5f);
        G_SpeedUPS.text = string.Format("{0} u/s", SpeedUPS.ToString("0.00"));

        //MONEY
        G_MV_MoneyViewer.text = string.Format("${0}", PowerUpManager.instance.CurrentMoney);
    }

    public void LoadLevelWithAnimation(int levelId)
    {
        LeanTween.move(G_Curtain, new Vector2(0, 0), 1.0f).setEaseInOutCubic().setOnStart(() => { G_Curtain.gameObject.SetActive(true); });

        EventsCouroutiner.instance.CallEventAfter(0.9f, () => {
            LeanTween.cancelAll();
            SceneManager.LoadScene(levelId);
        });
    }

    public void SpawnMoneyTextOnPos(int money, Vector3 worldpos, float time)
    {
        if (money == 0)
            return;

        Vector3 finalPos = Extensions.WorldToCanvas(CanvasRect, Camera.main.WorldToScreenPoint(G_MV_MoneyViewer.transform.position));

        //spawn obj
        GameObject spawned_text = (GameObject)Instantiate(G_MV_MoneyTextPrefab, G_MV_MoneyParent);
        TextMeshProUGUI s_text = spawned_text.GetComponent<TextMeshProUGUI>();
        LeanTween.scale(spawned_text, Vector3.zero, 0);

        //assign data to obj
        s_text.text = string.Format("${0}", money.ToString());
        s_text.rectTransform.anchoredPosition = Extensions.WorldToCanvas(CanvasRect, Camera.main.WorldToScreenPoint(worldpos));

        /*float test_a = time * (1 / 4);
        float test_b = time * 1/4;

        Debug.Log("test_a: " + test_a.ToString());
        Debug.Log("test_b: " + test_b.ToString());*/

        //animate obj
        // (1/4) time: spawn object and do an animation bounce
        // (3/4) time: go to finalPos on an easeInOutCubic, then scale and destroy.
        LeanTween.scale(spawned_text, new Vector3(1, 1, 1) * .5f, time * 1 / 4).setEaseInOutBounce().setOnComplete(() =>
          {
              EventsCouroutiner.instance.CallEventAfterFor((time * 1 / 4) / 2, time * 3 / 4, () =>
                        {
                            s_text.color = Color.Lerp(s_text.color, new Color(1, 1, 1, 0), Time.deltaTime / 2);
                        });
              EventsCouroutiner.instance.CallEventAfter((time * 1 / 4) / 2, () =>
                    {
                        LeanTween.scale(spawned_text, Vector3.zero, time * 3 / 4).setEaseInOutCubic();
                    });

              LeanTween.moveLocal(spawned_text, finalPos, time * 3 / 4).setEaseInOutCubic().setOnComplete(() =>
              {
                  Destroy(spawned_text);
              });
          });
    }

    public void ShowRetryScreen()
    {
        G_RS_MaxDistance.text = string.Format("{0}u", transform.position.z.ToString("0.00"));

        G_RS_RetryScreen.SetActive(true);
        LeanTween.scale(G_RS_RetryScreen, new Vector3(0, 0, 0), 0);
        LeanTween.scale(G_RS_RetryScreen, new Vector3(1, 1, 1), .25f).setOnComplete(() =>
        {
            int earnedmoney = (int)((transform.position.z) / 10);
            PowerUpManager.instance.AddMoneyToCurrent(earnedmoney, G_RS_MaxDistance.transform.position);
        });

        float t = 0.0f;
        LeanTween.value(t, 1.0f, 2.5f).setOnUpdate((float v) =>
        {
            ldi.intensity.value = -v * 100.0f;
            ldi.scale.value = 1 - v;
        }).setEaseInOutElastic();
    }

    public void DisplaySongFor(string song_artist, string song_name, float time)
    {
        G_MP_CurrentSong.text = string.Format("{0}\n{1}", song_name, song_artist);

        LeanTween.moveX(G_MP_CurrentSongParent, 
            0,
            2.0f).setEaseInOutExpo().setOnComplete(() =>
            {
                EventsCouroutiner.instance.CallEventAfter(time, () =>
                {
                    LeanTween.moveX(G_MP_CurrentSongParent,
                        CScaler.referenceResolution.x * 1.5f,
                        2.0f).setEaseInOutExpo();
                });
            });
    }
}