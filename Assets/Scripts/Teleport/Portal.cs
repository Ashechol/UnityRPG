using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public enum TransitionType { SameScene, DifferentScene };  // 场景转换类型
    public enum PortalTag { ENTER, A, B, C };

    private Transform exit;
    private Transform cam;
    private bool canTeleport;

    [Header("Information")]
    public TransitionType transitionType;
    public string sceneName;
    public PortalTag portalTag;
    public PortalTag dstTag;

    public Transform Exit { get { return exit; } }

    void Awake()
    {
        exit = transform.GetChild(0);
    }

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTeleport)
        {
            TeleportManager.Instance.TeleportToPortal(this);
        }
    }

    void LateUpdate()
    {
        transform.forward = -cam.forward;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTeleport = TeleportManager.Instance.canTeleport = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTeleport = TeleportManager.Instance.canTeleport = false;
    }
}
