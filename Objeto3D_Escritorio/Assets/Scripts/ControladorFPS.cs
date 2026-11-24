using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ControladorFPS : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 4.5f;
    [SerializeField] private float gravedad = -20f;

    [Header("Cámara")]
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private float sensibilidadMouse = 2f;
    [SerializeField] private float limiteVertical = 80f;

    private CharacterController characterController;
    private float velocidadVertical;
    private float rotacionVertical;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (camaraJugador == null)
            camaraJugador = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        BloquearMouse();
    }

    private void Update()
    {
        GestionarCursor();
        GestionarCamara();
        GestionarMovimiento();
    }

    private void GestionarCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState != CursorLockMode.Locked && Input.GetMouseButtonDown(0))
            BloquearMouse();
    }

    private void BloquearMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void GestionarCamara()
    {
        if (camaraJugador == null || Cursor.lockState != CursorLockMode.Locked)
            return;

        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        transform.Rotate(Vector3.up * mouseX);
        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -limiteVertical, limiteVertical);
        camaraJugador.transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
    }

    private void GestionarMovimiento()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direccion = (transform.right * horizontal + transform.forward * vertical).normalized;
        Vector3 movimiento = direccion * velocidad;

        if (characterController.isGrounded && velocidadVertical < 0f)
            velocidadVertical = -2f;
        else
            velocidadVertical += gravedad * Time.deltaTime;

        movimiento.y = velocidadVertical;
        characterController.Move(movimiento * Time.deltaTime);
    }

    // Permite ajustar la cámara desde el generador del Editor.
    public void ConfigurarCamara(Camera nuevaCamara)
    {
        camaraJugador = nuevaCamara;
    }
}
