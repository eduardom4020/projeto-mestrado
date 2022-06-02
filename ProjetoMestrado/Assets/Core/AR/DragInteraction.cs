using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DragInteraction : MonoBehaviour
{
    [SerializeField]
    public ARRaycastManager RaycastManager;
    [SerializeField]
    public Camera ARCamera;
    private InputController controller;
    private Coroutine dragAction;

    // Start is called before the first frame update
    void Awake()
    {
        controller = new InputController();
    }

    private void OnEnable()
    {
        controller.Enable();
    }

    private void OnDisable()
    {
        controller.Disable();
    }

    void Start()
    {
        controller.Player.TouchPoint.performed += ctx =>
        {
            RaycastHit hit;
            var coor = controller.Player.Position.ReadValue<Vector2>();
            if (Physics.Raycast(ARCamera.ScreenPointToRay(coor), out hit))
            {
                hit.collider.gameObject.SetActive(false);
            }
        };

        controller.Player.Drag.performed += ctx =>
        {
            dragAction = StartCoroutine(DragAction());
        };

        controller.Player.Drag.canceled += ctx =>
        {
            StopCoroutine(dragAction);
        };
    }

    private IEnumerator DragAction() {
        RaycastHit hit;
        var coor = controller.Player.Position.ReadValue<Vector2>();
        if (Physics.Raycast(ARCamera.ScreenPointToRay(coor), out hit))
        {
            var initialDistance = hit.distance;

            while (true)
            {
                yield return new WaitForFixedUpdate();

                var ray = ARCamera.ScreenPointToRay(controller.Player.Position.ReadValue<Vector2>());
                var velocity = Vector3.zero;

                hit.collider.gameObject.transform.position = Vector3.SmoothDamp
                (
                    hit.collider.gameObject.transform.position,
                    ray.GetPoint(initialDistance),
                    ref velocity,
                    .05f
                );
            }
        }
    }
}
