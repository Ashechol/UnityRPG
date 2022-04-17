using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public GameObject playerPrefab;
    GameObject player;
    NavMeshAgent playerAgent;
    Transform dst;
    bool loadingComplete;
    public bool canTeleport;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
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
            if (loadingComplete)
                SaveManager.Instance.LoadPlayerData();
            loadingComplete = false;
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
        yield return SceneManager.LoadSceneAsync(sceneName);
        dst = GetDestination(dstTag);

        if (sceneName != "Main Menu")
            yield return Instantiate(playerPrefab, dst.position, dst.rotation);

        loadingComplete = true;  // 场景和角色加载完成
        yield break;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene("Main Menu"));
    }

    public void LoadFirstLevel()
    {
        StartCoroutine(LoadScene("Big Plain"));
        if (loadingComplete)
            SaveManager.Instance.SavePlayerData();
        loadingComplete = false;
    }

    public void ContinueScene()
    {
        StartCoroutine(LoadScene(SaveManager.Instance.SavedScene));
        if (loadingComplete)
            SaveManager.Instance.LoadPlayerData();
        loadingComplete = false;
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
}
