using UnityEngine;

/// <summary>
/// Attached to Placeable Objects that belong to a objectpool.
/// Disables this object if OnTriggerEnter is called with a layermask, so that no overlapping with
/// objects occur that shouldn't.
/// </summary>
public class DisableOnTrigger : MonoBehaviour
{
    [Tooltip("The layermask that should be used to check for collision.")]
    [SerializeField] private LayerMask layerMask;

    public LayerMask LayerMask
    { get { return layerMask; } set { layerMask = value; } }

    private void OnTriggerEnter(Collider other)
    {
        // Is the layer of the collided object toggled on in the layermask?
        if ((layerMask & (1 << other.transform.gameObject.layer)) != 0)
            DisableThis();
    }

    /// <summary>
    /// Reuses this object in the objectpooling system if there is one or just deactivates it.
    /// </summary>
    private void DisableThis()
    {
        if (ObjectPool.Instance)
            ObjectPool.Instance.ReuseObject(this.transform);
        else
            transform.gameObject.SetActive(false);
    }
}