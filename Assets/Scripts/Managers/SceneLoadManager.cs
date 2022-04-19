using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>, IEndGameObserver
{
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    Transform dst;
    bool sceneLoadComplete;
    bool fadeInGameOver;  // 保证收到EndNotify只执行一次SceneFade
    public bool canTeleport;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeInGameOver = true;
    }

    public void TeleportToPortal(Portal portal)
    {
        switch (portal.transitionType)
        {
            case Portal.TransitionType.SameScene:
                Teleport(portal.dstTag);
                break;
            case Portal.TransitionType.DifferentScene:
                Teleport(portal.dstTag, portal.sceneName);
                break;
        }
    }

    void Teleport(Portal.PortalTag dstTag, string sceneName = null)
    {
        if (sceneName != null)  // 不同关卡传送
        {
            SaveManager.Instance.SavePlayerData();
            StartCoroutine(LoadScene(sceneName, dstTag));
            StartCoroutine(LoadData());
            sceneLoadComplete = false;
        }
        else  // 同关卡传送
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            dst = GetDestination(dstTag);

            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(dst.position, dst.rotation);
            playerAgent.enabled = true;
        }
    }

    IEnumerator LoadScene(string sceneName, Portal.PortalTag dstTag = Portal.PortalTag.ENTER)
    {
        // 开始加载画面渐出
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(1.7f));

        yield return SceneManager.LoadSceneAsync(sceneName);
        dst = GetDestination(dstTag);

        if (sceneName != "Main Menu")
        {
            yield return Instantiate(playerPrefab, dst.position, dst.rotation);
            sceneLoadComplete = true;  // 场景和角色加载完成
        }

        // 加载完毕画面渐进
        yield return StartCoroutine(fade.FadeIn(2f));

        yield break;
    }

    IEnumerator SaveData()
    {
        while (!sceneLoadComplete)
            yield return null;

        SaveManager.Instance.SavePlayerData();
        sceneLoadComplete = false;
        yield break;
    }

    IEnumerator LoadData()
    {
        while (!sceneLoadComplete)
            yield return null;

        SaveManager.Instance.LoadPlayerData();
        sceneLoadComplete = false;
        yield break;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene("Main Menu"));
    }

    public void LoadFirstLevel()
    {
        StartCoroutine(LoadScene("Dungeon"));
        StartCoroutine(SaveData());
    }

    public void ContinueScene()
    {
        StartCoroutine(LoadScene(SaveManager.Instance.SavedScene));
        StartCoroutine(LoadData());
    }

    private Transform GetDestination(Portal.PortalTag dstTag)
    {
        foreach (var i in FindObjectsOfType<Portal>())
        {
            if (i.portalTag == dstTag)
                return i.Exit;
        }

        return null;
    }

    public void EndNotify()
    {
        if (fadeInGameOver)
        {
            fadeInGameOver = false;
            LoadMainMenu();
        }
    }
}
