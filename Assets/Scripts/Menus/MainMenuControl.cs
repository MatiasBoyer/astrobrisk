using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{

    public GameObject Curtain;

    [Header("OnLoadLevel")]
    public Button LoadLevelButton;
    public RectTransform LoadingImage;
    public Transform T_Spaceship;


    private void Start()
    {
        //spaceshipPosition
        //LeanTween.move(T_Spaceship.gameObject, T_Spaceship.up * 5.0f, 0);
        LeanTween.move(T_Spaceship.gameObject, new Vector3(1, 0, -1.5f), 1.5f).setEaseInOutCubic().setOnComplete(() => LoadLevelButton.interactable = true);

        LeanTween.move(Curtain, new Vector2(Screen.width/2, Screen.height/2), 0).setOnComplete(() => { Curtain.gameObject.SetActive(true); });
        LeanTween.move(Curtain, new Vector2(-Screen.width*2, Screen.height / 2), 1.0f).setEaseInOutCubic().setOnComplete(() => { Curtain.gameObject.SetActive(false); });
    }

    public void LoadLevel()
    {
        LoadLevelButton.interactable = false;
        UI_SFX.instance.PlayClip(UI_SFX.clipType.CLICK);

        //spaceship pos THEN load
        Vector3 pos = new Vector3(Screen.width / 2, Screen.height*2);
        LeanTween.move(T_Spaceship.gameObject, -T_Spaceship.up * 15.0f, 1.25f).setEaseInOutCubic().setOnComplete(() => {
            LeanTween.move(LoadingImage.gameObject, pos, 2.0f).setEaseInOutCubic().setOnComplete(() => { SceneManager.LoadScene(1); });
        });
    }
}