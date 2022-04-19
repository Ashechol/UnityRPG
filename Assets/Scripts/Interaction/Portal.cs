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
    public bool isClosed = false;

    void Awake()
    {
        exit = transform.GetComponentInParent<Transform>().GetChild(1);
        if (isClosed)
        {
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    void Start()
    {
        cam = Camera.main.transform;
        GameManager.Instance.RegisterPortal(this);
    }

    void Update()
    {
        if (!isClosed)
        {
            GetComponent<Collider>().enabled = true;
            GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && canTeleport)
        {
            SceneLoadManager.Instance.TeleportToPortal(this);
        }
    }

    void LateUpdate()
    {
        transform.GetChild(0).forward = -cam.forward;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = SceneLoadManager.Instance.canTeleport = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTeleport = SceneLoadManager.Instance.canTeleport = false;
    }
}
