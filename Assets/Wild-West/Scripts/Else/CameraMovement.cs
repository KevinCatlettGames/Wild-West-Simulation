using UnityEngine;

/// <summary>
/// A simple camera movement script using the old Input System.
/// </summary>
public class CameraMovement : MonoBehaviour
{
    #region Variables

    [Tooltip("Normal speed of camera movement.")]
    [SerializeField] private float movementSpeed = 10f;

    [Tooltip("Speed of camera movement when shift is held down")]
    [SerializeField] private float fastMovementSpeed = 100f;

    [Tooltip("Sensitivity for free look.")]
    [SerializeField] private float freeLookSensitivity = 3f;

    [Tooltip("Amount to zoom the camera when using the mouse wheel.")]
    [SerializeField] private float zoomSensitivity = 10f;

    [Tooltip("Amount to zoom the camera when using the mouse wheel (fast mode)")]
    [SerializeField] private float fastZoomSensitivity = 50f;

    /// <summary>
    /// Set to true when free looking (on right mouse button).
    /// </summary>
    private bool looking = false;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Disables the Cursor and locks it.
    /// </summary>
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Calls the Movement and Rotation methods.
    /// </summary>
    private void Update()
    {
        Movement();
        Rotation();
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Moves the gameobject using the old input system.
    /// </summary>
    private void Movement()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        var movementSpeed = fastMode ? this.fastMovementSpeed : this.movementSpeed;

        // Move left
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);

        // Move right
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);

        // Move forward
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);

        // Move back
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);

        // Move up
        if (Input.GetKey(KeyCode.Q))
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);

        // Move up
        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.PageUp))
            transform.position = transform.position + (Vector3.up * movementSpeed * Time.deltaTime);

        // Move down
        if (Input.GetKey(KeyCode.E))
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);

        // Move down
        if (Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.PageDown))
            transform.position = transform.position + (-Vector3.up * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Rotates the gameObject using the old input system.
    /// </summary>
    private void Rotation()
    {
        var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (looking)
        {
            float newRotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * freeLookSensitivity;
            float newRotationY = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * freeLookSensitivity;
            transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
        }

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0)
        {
            var zoomSensitivity = fastMode ? this.fastZoomSensitivity : this.zoomSensitivity;
            transform.position = transform.position + transform.forward * axis * zoomSensitivity;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            StartLooking();
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            StopLooking();
        }
    }

    /// <summary>
    /// Enable free looking.
    /// </summary>
    private void StartLooking()
    {
        looking = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Disable free looking.
    /// </summary>
    private void StopLooking()
    {
        looking = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    #endregion Methods
}