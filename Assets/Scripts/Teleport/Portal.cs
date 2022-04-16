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
    public PortalTag portalTag;
    public TransitionType transitionType;
    public string sceneName;
    public PortalTag dstTag;

    public Transform Exit { get { return exit; } }

    void Awake()
    {
        exit = transform.GetComponentInParent<Transform>().GetChild(1);
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
        transform.GetChild(0).forward = -cam.forward;
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
