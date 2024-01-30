using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Changes the time of day by invoking a event that all systems needing to react to.
/// Changes the skybox according to the current time of day.
/// </summary>
public class WorldTime : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// This as singleton.
    /// </summary>
    public static WorldTime Instance;

    [Header("Sky")]
    [Tooltip("The skybox material that is currently active.")]
    [SerializeField] private Material skybox;

    [Tooltip("The light of the sun which is deactivated during nighttime.")]
    [SerializeField] private Light sun;

    [Tooltip("The light of the moon which is deactivated during daytime.")]
    [SerializeField] private Light moon;

    [Tooltip("The speed in which the skybox changes.")]
    [SerializeField] private float transitionSpeed;

    [Tooltip("The beginning rotation of this transform, which is the parent of the sun and moon.")]
    [SerializeField, Range(0f, 1f)] private float startRotation;

    [Header("Day/Night cycle values.")]
    [Tooltip("How long it takes for one the currently active daytime to switch.")]
    [SerializeField, Min(1)] private float cycleLength = 30f;

    [Tooltip("The speed in which time passes.")]
    [SerializeField] private float cycleSpeed;

    [Tooltip("The color of the fog during nighttime.")]
    [SerializeField] private Color nightFogColor = Color.grey;

    [Tooltip("The speed in which this rotates, which is the parent of the sun and moon.")]
    [SerializeField] private float angularSpeed;

    /// <summary>
    /// Is it day or night?
    /// </summary>
    public bool isDay = true;

    /// <summary>
    /// When invoked many systems change their behaviour, for example Bisons go to sleep or ambient sound changes.
    /// </summary>
    public UnityAction dayTimeChanged;

    /// <summary>
    /// The color of the fog during the day. Set in Awake.
    /// </summary>
    private Color dayFogColor;

    #endregion Variables

    #region Unity Methods

    /// <summary>
    /// Sets this as singleton and initializes the daytime fog.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dayFogColor = RenderSettings.fogColor;
    }

    /// <summary>
    /// Makes sure initialization occurs when the scene first loads.
    /// </summary>
    private void Start()
    {
        Initialization();
    }

    /// <summary>
    /// Rotates this transform which is the parent of the sun and moon and makes sure the skybox transitions.
    /// </summary>
    private void Update()
    {
        transform.Rotate(Vector3.right, angularSpeed * Time.deltaTime * cycleSpeed);
        SunAndMoonActivation();
        SkyboxTransitioning();
    }

    /// <summary>
    /// Deactivates the skybox transitioning if this is disabled.
    /// </summary>
    private void OnDisable()
    {
        skybox.SetFloat("_LerpVector", 0);
    }

    #endregion Unity Methods

    #region Methods

    /// <summary>
    /// Sets the start rotation of this transform and makes sure the sun and moon are activated correctly.
    /// </summary>
    private void Initialization()
    {
        transform.eulerAngles = new Vector3(startRotation, 0, 0);
        SunAndMoonActivation();
    }

    /// <summary>
    /// Checks wether the skybox should change and changes it in the corresponding amount if needed.
    /// </summary>
    private void SkyboxTransitioning()
    {
        if (isDay && skybox.GetFloat("_LerpVector") > 0)
            skybox.SetFloat("_LerpVector", skybox.GetFloat("_LerpVector") - transitionSpeed * Time.deltaTime);
        else if (!isDay && skybox.GetFloat("_LerpVector") < 1)
            skybox.SetFloat("_LerpVector", skybox.GetFloat("_LerpVector") + transitionSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Set the current time of day if the rotation of the sun and moon are correct.
    /// </summary>
    private void SunAndMoonActivation()
    {
        // Is the sun at the correct position and is it currently day? Then turn to night.
        if (sun.transform.position.y < 0 && isDay)
        {
            // Enable the moon.
            sun.gameObject.SetActive(false);
            moon.gameObject.SetActive(true);

            // Set the nighttime light.
            RenderSettings.sun = moon;

            // Set the nighttime fog.
            RenderSettings.fogColor = nightFogColor;

            isDay = false;

            // Invoke the dayTimeChanged event.
            dayTimeChanged?.Invoke();
        }

        // Is the moon at the correct position and is it currently night? Then turn to day.
        else if (moon.transform.position.y < 0 && !isDay)
        {
            // Enable the sun.
            sun.gameObject.SetActive(true);
            moon.gameObject.SetActive(false);

            // Set the daytime light.
            RenderSettings.sun = sun;

            // Set the daytime fog.
            RenderSettings.fogColor = dayFogColor;

            isDay = true;

            // Invoke the dayTimeChanged event.
            dayTimeChanged?.Invoke();
        }
    }

    #endregion Methods
}