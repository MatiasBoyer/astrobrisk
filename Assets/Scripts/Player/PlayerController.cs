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

    private ShaderEffect_Scanner SE_Scanner;

    private void Awake()
    {
        Rb3d = PlayerModel.GetComponent<Rigidbody>();
        p_collider = PlayerModel.GetComponents<BoxCollider>();
        _PlayerUI = GetComponent<PlayerUI>();

        foreach(Transform t in BrokenModel)
        {
            brokenRbs.Add(t.GetComponent<Rigidbody>());
        }

        SE_Scanner = _Camera.GetComponent<ShaderEffect_Scanner>();
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
        if (collision.transform.tag == "ScenePowerup")
            return;

        float collisionForce = (collision.impulse.magnitude / Time.fixedDeltaTime) + 1;
        //Debug.Log(collisionForce);

        canMove = false;

        EnableColliders(false);

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

        _PlayerUI.ShowRetryScreen();

        float t = 0.0f;
        EventsCouroutiner.instance.CallEventFor(5.0f, () =>
        {
            t += Time.deltaTime / 50.0f;
            SE_Scanner.area = Mathf.Lerp(SE_Scanner.area, 1.0f, t);
        });
    }

    public void EnableColliders(bool _enable)
    {
        foreach (BoxCollider b in p_collider)
            b.enabled = _enable;
    }

    private void OnGUI()
    {
        //GUI.color = new Color(1, 1, 1, 1);
        //GUI.Label(new Rect(2, 2, Screen.width, 100), new GUIContent(dragVelocity.ToString()), new GUIStyle() { fontSize = 20, normal = new GUIStyleState() { textColor = Color.white } });
    }
}