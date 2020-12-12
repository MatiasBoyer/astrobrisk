using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerUI : MonoBehaviour
{

    public GameObject RetryScreen;

    [Header("Game")]
    public TextMeshProUGUI G_SpeedUPS;
    private float SpeedUPS;
    
    [Header("Game:MoneyVisualizer")]
    public GameObject G_MV_MoneyTextPrefab;
    public Transform G_MV_MoneyParent;
    public TextMeshProUGUI G_MV_MoneyViewer;

    public RectTransform G_Curtain;

    private Vector3 finalPos;
    private PlayerController PControl;
    private Rigidbody Rb;

    private void Start()
    {
        PControl = GetComponent<PlayerController>();
        Rb = PControl.Rb3d;

        LeanTween.move(G_Curtain, new Vector2(0, 0), 0).setOnComplete(() => {
            G_Curtain.gameObject.SetActive(true);
            LeanTween.move(G_Curtain, new Vector2(-2000, 0), 1.0f).setOnComplete(() => { G_Curtain.gameObject.SetActive(false); });
            });
    }

    private void FixedUpdate()
    {
        //SPEED
        float sups = Rb.velocity.magnitude + PControl.MovementSpeed;
        SpeedUPS = Mathf.Lerp(SpeedUPS, sups, Time.deltaTime * 5);
        G_SpeedUPS.text = string.Format("{0} u/s", SpeedUPS.ToString("0.00"));

        //MONEY
        G_MV_MoneyViewer.text = string.Format("${0}", PowerUpManager.instance.CurrentMoney);
    }

    public void LoadLevelWithAnimation(int levelId)
    {
        LeanTween.move(G_Curtain, new Vector2(0, 0), 1.0f).setEaseInOutCubic().setOnStart(() => { G_Curtain.gameObject.SetActive(true); });

        EventsCouroutiner.instance.CallEventAfter(0.9f, () => { 
            SceneManager.LoadScene(levelId); 
        });
    }

    public void SpawnMoneyTextOnPos(int money, Vector2 screenPos, float time)
    {
        if (money == 0)
            return;

        Vector3 finalPos = Camera.main.WorldToScreenPoint(G_MV_MoneyViewer.transform.position)
            - new Vector3(Screen.width / 2, Screen.height / 2)
            - new Vector3(0, G_MV_MoneyViewer.preferredHeight / 2);

        //spawn obj
        GameObject spawned_text = (GameObject)Instantiate(G_MV_MoneyTextPrefab, G_MV_MoneyParent);
        TextMeshProUGUI s_text = spawned_text.GetComponent<TextMeshProUGUI>();
        LeanTween.scale(spawned_text, Vector3.zero, 0);

        //assign data to obj
        s_text.text = string.Format("${0}", money.ToString());
        s_text.rectTransform.anchoredPosition = screenPos;

        /*float test_a = time * (1 / 4);
        float test_b = time * 1/4;

        Debug.Log("test_a: " + test_a.ToString());
        Debug.Log("test_b: " + test_b.ToString());*/

        //animate obj
        // (1/4) time: spawn object and do an animation bounce
        // (3/4) time: go to finalPos on an easeInOutCubic, then scale and destroy.
        LeanTween.scale(spawned_text, new Vector3(1, 1, 1) * .5f, time * 1/4).setEaseInOutBounce().setOnComplete(() =>
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
}