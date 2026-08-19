using UnityEngine;

namespace FPSStarter
{
    /// <summary>
    /// Drop this component onto any GameObject in your scene (e.g. an empty
    /// "Bootstrap" object). On Play it will:
    ///   1. Disable every existing Camera in the scene (the orphan Main Camera).
    ///   2. Spawn a Player capsule with a CharacterController.
    ///   3. Create a "View Camera" child at eye height (y = 1.6 m), exactly
    ///      like the Framework's FpsStarterBootstrap.CreatePlayer().
    ///   4. Add FirstPersonController, PlayerInteractor and AudioListener so
    ///      mouse-look and interaction work immediately.
    ///
    /// You only need this if you are NOT using FpsStarterBootstrap.
    /// If FpsStarterBootstrap is already in the scene, remove this component.
    /// </summary>
    public sealed class CameraParenter : MonoBehaviour
    {
        [Header("Spawn")]
        [Tooltip("World position to place the player at the start of the scene.")]
        [SerializeField] private Vector3 spawnPosition = new Vector3(0f, 1f, -7f);

        [Header("Capsule")]
        [SerializeField] private float capsuleRadius = 0.35f;
        [SerializeField] private float capsuleHeight = 1.8f;

        [Header("Camera")]
        [SerializeField] private float eyeHeight = 1.6f;
        [SerializeField] private float nearClipPlane = 0.03f;

        private void Awake()
        {
            // Disable every existing camera so we start clean.
            foreach (Camera cam in FindObjectsByType<Camera>(FindObjectsSortMode.None))
                cam.gameObject.SetActive(false);

            BuildPlayer();
        }

        private void BuildPlayer()
        {
            // --- player root ---
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            player.transform.position = spawnPosition;

            CharacterController cc = player.AddComponent<CharacterController>();
            cc.radius = capsuleRadius;
            cc.height = capsuleHeight;

            // --- view camera child (the Framework pattern) ---
            GameObject viewObj = new GameObject("View Camera");
            viewObj.transform.SetParent(player.transform);
            viewObj.transform.localPosition = new Vector3(0f, eyeHeight, 0f);
            viewObj.transform.localRotation = Quaternion.identity;
            viewObj.tag = "MainCamera";

            Camera cam2 = viewObj.AddComponent<Camera>();
            cam2.nearClipPlane = nearClipPlane;

            viewObj.AddComponent<AudioListener>();

            // Add FPS components AFTER the view child exists so Awake() finds it.
            player.AddComponent<FirstPersonController>();
            player.AddComponent<PlayerInteractor>();
        }
    }
}
