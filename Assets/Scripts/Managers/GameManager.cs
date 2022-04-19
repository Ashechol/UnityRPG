using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public PlayerStats playerStats;
    private CinemachineFreeLook followCam;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();
    List<Portal> Portals = new List<Portal>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RegisterPlayer(PlayerStats player)
    {
        playerStats = player;

        // 转换场景保证cinemachine跟随玩家角色
        followCam = FindObjectOfType<CinemachineFreeLook>();
        if (followCam != null)
        {
            followCam.Follow = player.transform;
            followCam.LookAt = player.transform.GetChild(2).transform;
        }
    }

    public void RegisterPortal(Portal portal)
    {
        Portals.Add(portal);
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
        if (endGameObservers.Count == 1)  // 有 SceneLoadManager
        {
            foreach (var p in Portals)
            {
                p.isClosed = false;
            }
        }
    }

    public void NotifyObservers()
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }
}
