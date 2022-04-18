using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn, continueBtn, quitBtn;
    PlayableDirector director;

    void Awake()
    {
        director = FindObjectOfType<PlayableDirector>();
        director.stopped += NewGame;

        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeline);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);
    }

    void PlayTimeline()
    {
        director.Play();

    }

    void NewGame(PlayableDirector obj)  // 此处obj没有任何意义，只是为了满足被director.stopped委托
    {
        PlayerPrefs.DeleteAll();
        SceneLoadManager.Instance.LoadFirstLevel();
    }

    void ContinueGame()
    {
        SceneLoadManager.Instance.ContinueScene();
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
