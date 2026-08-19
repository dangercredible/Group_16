using UnityEngine;

namespace FPSStarter
{
    /// <summary>
    /// Attach this to the Main Camera. At runtime it finds the player's view
    /// transform (or the FirstPersonController root) and parents itself to it,
    /// then zeroes its local transform so the FPS controller's look system
    /// takes over completely.
    ///
    /// Usage: place this component on your Camera GameObject. Assign the
    /// Player field in the Inspector (or leave it blank to auto-find by tag
    /// "Player"). The camera will be reparented during Awake so that
    /// FirstPersonController's 'view' assignment picks it up correctly.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class CameraParenter : MonoBehaviour
    {
        [Tooltip("The player GameObject that has a FirstPersonController. " +
                 "Leave blank to auto-find by the 'Player' tag.")]
        [SerializeField] private Transform player;

        [Tooltip("If the FirstPersonController has a named child to use as " +
                 "the camera pivot (e.g. 'CameraPivot'), set its name here. " +
                 "Leave blank to parent directly to the player root.")]
        [SerializeField] private string cameraPivotName = "";

        private void Awake()
        {
            ResolvePlayer();
            if (player == null)
            {
                Debug.LogWarning("[CameraParenter] No player found. Camera will not be parented.", this);
                return;
            }

            Transform target = ResolveTarget();
            transform.SetParent(target, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            Debug.Log($"[CameraParenter] Camera parented to '{target.name}'.", this);
        }

        private void ResolvePlayer()
        {
            if (player != null) return;
            GameObject tagged = GameObject.FindWithTag("Player");
            if (tagged != null) player = tagged.transform;
        }

        private Transform ResolveTarget()
        {
            if (!string.IsNullOrWhiteSpace(cameraPivotName))
            {
                Transform pivot = player.Find(cameraPivotName);
                if (pivot != null) return pivot;
                Debug.LogWarning($"[CameraParenter] Could not find pivot child '{cameraPivotName}' on player. Parenting to player root instead.", this);
            }
            return player;
        }
    }
}
