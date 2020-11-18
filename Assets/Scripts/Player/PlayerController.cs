using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerController : MonoBehaviour
{

    public bool canMove = true;

    public Transform PlayerModel;

    public Vector2 MinPositions, MaxPositions;

    [Header("Speeds")]
    public float MovementSpeed;
    public float MaxRotationSpeed;
    public Vector2 MaxDragVelocity;

    [Header("3D Model")]
    public Transform ModelParent;
    public float RotationForce;
    public float LerpSpeed;

    public Transform NormalModel, BrokenModel;

    [Header("Camera")]
    public Camera _Camera;

    [Header("Audio Source")]
    public AudioSource ASource;
    public AudioClip A_Explosion;

    [HideInInspector]
    public Rigidbody Rb3d;
    private List<Rigidbody> brokenRbs = new List<Rigidbody>();
    private BoxCollider[] p_collider;
    private Vector3 playerPos;
    private Vector2 startDragPos, dragDiff, prevdragDiff, dragVelocity, moveDir;

    private PlayerUI _PlayerUI;

    private void Awake()
    {
        Rb3d = PlayerModel.GetComponent<Rigidbody>();
        p_collider = PlayerModel.GetComponents<BoxCollider>();
        _PlayerUI = GetComponent<PlayerUI>();

        foreach(Transform t in BrokenModel)
        {
            brokenRbs.Add(t.GetComponent<Rigidbody>());
        }
    }

    private void Update()
    {
        if(canMove)
        {
            MovementUpdate();
            RotationUpdate();
        }

        DragDetection();
    }

    private void MovementUpdate()
    {
        transform.Translate(transform.forward * MovementSpeed * Time.deltaTime);

        moveDir = dragVelocity * MaxRotationSpeed; //Mathf.Clamp(MovementSpeed, 0, MaxRotationSpeed);
        playerPos.x = PlayerModel.position.x;
        playerPos.y = PlayerModel.position.y;

        playerPos.x = Mathf.Clamp(playerPos.x, MinPositions.x, MaxPositions.x);
        playerPos.y = Mathf.Clamp(playerPos.y, MinPositions.y, MaxPositions.y);

        Rb3d.velocity = Vector3.Lerp(Rb3d.velocity, new Vector3(moveDir.x, moveDir.y, 0), Time.deltaTime * 5);

        PlayerModel.position = playerPos + transform.position;
    }

    private void RotationUpdate()
    {
        Vector3 eulerRot = new Vector3(-moveDir.y, 0, -moveDir.x) * RotationForce;

        eulerRot.x = Mathf.Clamp(eulerRot.x, -25, 25);
        eulerRot.z = Mathf.Clamp(eulerRot.z, -25, 25);
        Quaternion quatRot = Quaternion.Euler(eulerRot);

        ModelParent.localRotation = Quaternion.Slerp(ModelParent.localRotation, quatRot, LerpSpeed * Time.deltaTime);
    }

    private void DragDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startDragPos = Input.mousePosition;
            dragDiff = Vector2.zero;
        }

        if (Input.GetMouseButton(0))
        {
            dragDiff = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - startDragPos;
            dragVelocity = (dragDiff - prevdragDiff) / Time.deltaTime * 0.1f;
            prevdragDiff = dragDiff;

            dragVelocity.x = Mathf.Clamp(dragVelocity.x, -MaxDragVelocity.x, MaxDragVelocity.x);
            dragVelocity.y = Mathf.Clamp(dragVelocity.y, -MaxDragVelocity.y, MaxDragVelocity.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            startDragPos = Vector2.zero;
            dragDiff = Vector2.zero;
            dragVelocity = Vector2.zero;
            prevdragDiff = Vector2.zero;
        }
    }

    public void OnColEnter(Collision collision)
    {
        float collisionForce = (collision.impulse.magnitude / Time.fixedDeltaTime) + 1;
        //Debug.Log(collisionForce);

        canMove = false;

        foreach(BoxCollider b in p_collider)
            b.enabled = false;

        NormalModel.gameObject.SetActive(false);
        BrokenModel.gameObject.SetActive(true);

        foreach(Rigidbody r in brokenRbs)
        {
            r.AddExplosionForce(250 * MovementSpeed, collision.contacts[0].point, 25.0f);
        }

        Rb3d.freezeRotation = false;
        Rb3d.velocity = Vector3.zero;
        MovementSpeed = 0.0f;

        CameraShaker.Instance.ShakeOnce(15f, 5f, 0.0f, 2.5f);

        ASource.PlayOneShot(A_Explosion);

        _PlayerUI.RetryScreen.SetActive(true);
        LeanTween.scale(_PlayerUI.RetryScreen, new Vector3(0, 0, 0), 0);
        LeanTween.scale(_PlayerUI.RetryScreen, new Vector3(1, 1, 1), .25f);
    }

    public bool isUpsideDown()
    {
        //Debug.Log(Vector3.Dot(transform.up, Vector3.up));
        return Vector3.Dot(transform.up, Vector3.up) < -1f;
    }

    public float booltoneg(bool b)
    {
        if (b)
            return 1.0f;
        return -1.0f;
    }

    private void OnGUI()
    {
        //GUI.color = new Color(1, 1, 1, 1);
        //GUI.Label(new Rect(2, 2, Screen.width, 100), new GUIContent(dragVelocity.ToString()), new GUIStyle() { fontSize = 20, normal = new GUIStyleState() { textColor = Color.white } });
    }
}