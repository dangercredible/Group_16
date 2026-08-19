using UnityEngine;

namespace FPSStarter
{
    /// <summary>
    /// Attach this to the Main Camera. At runtime it parents the camera to
    /// the player capsule and positions it at eye level for a true first-person
    /// view. The FirstPersonController then picks it up as its 'view' and
    /// drives all mouse-look rotation.
    ///
    /// Setup:
    ///   1. Add this component to your Main Camera.
    ///   2. Either drag the player into the 'Player' field, OR tag the player
    ///      GameObject as "Player" and it will be found automatically.
    ///   3. Hit play — the camera locks to the capsule's eye height.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraParenter : MonoBehaviour
    {
        [Tooltip("The player GameObject with FirstPersonController + CharacterController. " +
                 "Leave blank to auto-find by the 'Player' tag.")]
        [SerializeField] private Transform player;

        [Tooltip("Fraction of the capsule height at which the camera sits. " +
                 "0.85 places it near the top like real eye level.")]
        [SerializeField, Range(0.5f, 0.95f)] private float eyeHeightRatio = 0.85f;

        private void Awake()
        {
            if (player == null)
            {
                GameObject tagged = GameObject.FindWithTag("Player");
                if (tagged != null) player = tagged.transform;
            }

            if (player == null)
            {
                FirstPersonController fps = FindFirstObjectByType<FirstPersonController>();
                if (fps != null) player = fps.transform;
            }

            if (player == null)
            {
                Debug.LogError("[CameraParenter] No player found — tag your player as 'Player' " +
                               "or assign the field in the Inspector.", this);
                return;
            }

            transform.SetParent(player, false);

            float eyeY = CalculateEyeHeight();
            transform.localPosition = new Vector3(0f, eyeY, 0f);
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        private float CalculateEyeHeight()
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc != null)
            {
                float top = cc.center.y + cc.height * 0.5f;
                float bottom = cc.center.y - cc.height * 0.5f;
                return Mathf.Lerp(bottom, top, eyeHeightRatio);
            }

            CapsuleCollider capsule = player.GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                float top = capsule.center.y + capsule.height * 0.5f;
                float bottom = capsule.center.y - capsule.height * 0.5f;
                return Mathf.Lerp(bottom, top, eyeHeightRatio);
            }

            return 1.6f;
        }
    }
}
