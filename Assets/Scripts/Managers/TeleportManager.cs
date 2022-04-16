using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TeleportManager : Singleton<TeleportManager>
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

    private Transform GetDestination(Portal.PortalTag dstTag)
    {
        var portals = FindObjectsOfType<Portal>();

        foreach (var i in portals)
        {
            if (i.portalTag == dstTag)
                return i.Exit;
        }

        return null;
    }
}
