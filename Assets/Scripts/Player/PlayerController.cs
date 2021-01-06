using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Transform CameraPivot;
    public Camera _Camera;

    [Header("Audio Source")]
    public AudioSource ASource;
    public AudioClip A_Explosion;

    private MusicPlayer _MusicPlayer;

    [HideInInspector]
    public Rigidbody Rb3d;
    private List<Rigidbody> brokenRbs = new List<Rigidbody>();
    private BoxCollider[] p_collider;
    private Vector3 playerPos, startcamlocalpos, camlocaloffset, startcamlocaleul, camlocaleul;
    private Vector2 startDragPos, dragDiff, prevdragDiff, dragVelocity, moveDir;

    private PlayerUI _PlayerUI;

    private void Awake()
    {
        Rb3d = PlayerModel.GetComponent<Rigidbody>();
        p_collider = PlayerModel.GetComponents<BoxCollider>();
        _PlayerUI = GetComponent<PlayerUI>();
        _MusicPlayer = GameObject.FindObjectOfType<MusicPlayer>();

        startcamlocalpos = CameraPivot.transform.localPosition;
        startcamlocaleul = CameraPivot.transform.localEulerAngles;

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

        camlocaloffset.x = Extensions.Remap(playerPos.x, MinPositions.x, MaxPositions.x, -4, 4);
        camlocaloffset.y = Extensions.Remap(playerPos.y, MinPositions.y, MaxPositions.y, -4, 4);
        CameraPivot.transform.localPosition = startcamlocalpos + camlocaloffset;

        camlocaleul.y = Extensions.Remap(playerPos.x, MinPositions.x, MaxPositions.x, -4, 4);
        camlocaleul.x = -Extensions.Remap(playerPos.y, MinPositions.y, MaxPositions.y, -4, 4);
        CameraPivot.transform.localEulerAngles = startcamlocaleul + camlocaleul;
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

        _MusicPlayer.StopMusic(2.0f);

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

        CameraShake.instance.ShakeOnce(new CameraShake.ShakeSettings() { duration = 2.0f });

        ASource.PlayOneShot(A_Explosion);

        _PlayerUI.ShowRetryScreen();
    }

    public void EnableColliders(bool _enable)
    {
        foreach (BoxCollider b in p_collider)
            b.enabled = _enable;
    }

    public Collider[] ReturnPlayerColliders()
    {
        return p_collider;
    }

    private void OnGUI()
    {
        //GUI.color = new Color(1, 1, 1, 1);
        //GUI.Label(new Rect(2, 2, Screen.width, 100), new GUIContent(dragVelocity.ToString()), new GUIStyle() { fontSize = 20, normal = new GUIStyleState() { textColor = Color.white } });
    }
}