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
                StartCoroutine(Teleport(portal.dstTag));
                break;
            case Portal.TransitionType.DifferentScene:
                StartCoroutine(Teleport(portal.dstTag, portal.sceneName));
                break;
        }
    }

    IEnumerator Teleport(Portal.PortalTag dstTag, string sceneName = null)
    {
        //保存数据

        if (sceneName != null)
        {
            SaveManager.Instance.SavePlayerData();
            yield return SceneManager.LoadSceneAsync(sceneName);
            dst = GetDestination(dstTag);
            yield return Instantiate(playerPrefab, dst.position, dst.rotation);
            SaveManager.Instance.LoadPlayerData();
            yield break;
        }
        else
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            dst = GetDestination(dstTag);

            playerAgent.enabled = false;
            player.transform.SetPositionAndRotation(dst.position, dst.rotation);
            playerAgent.enabled = true;
            yield return null;
        }
    }

    IEnumerator LoadScene(string sceneName, Portal.PortalTag dstTag = Portal.PortalTag.ENTER)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
        dst = GetDestination(dstTag);

        if (sceneName != "Main Menu")
        {
            yield return Instantiate(playerPrefab, dst.position, dst.rotation);
            SaveManager.Instance.SavePlayerData();
        }
        yield break;
    }

    IEnumerator LoadSavedScene(string sceneName, Portal.PortalTag dstTag = Portal.PortalTag.ENTER)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);

        dst = GetDestination(dstTag);

        yield return Instantiate(playerPrefab, dst.position, dst.rotation);

        SaveManager.Instance.LoadPlayerData();
        yield break;
    }

    public void LoadMainMenu()
    {
        StartCoroutine(LoadScene("Main Menu"));
    }

    public void LoadFirstLevel()
    {
        StartCoroutine(LoadScene("Big Plain"));
    }

    public void ContinueScene()
    {
        StartCoroutine(LoadSavedScene(SaveManager.Instance.SavedScene));
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
