using UnityEngine;

namespace Settings
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/PlayerSettings")]
    public class PlayerSettings : ScriptableObject
    {
        [Header("Locomotion Setting")]
        public float moveSpeed;
        public float jumpForce;
        public float minimumFallForce;
        public float fallForceOffset;
        public float delayAfterFall;
        
        // Non VR
        public float cameraLerpSpeed;
        public float avatarTurningSpeed;
        
        // VR
        public float vrTurnAmount;
        
        [Header("Locomotion Sound Effects")]
        public AudioClip sfxWalk;
        public AudioClip sfxRun;
        public AudioClip sfxLanding;

        [Header("Avatar Setting")] 
        public float defaultAvatarHeight;
        
        [Header("Drone Setting")]
        public float droneSpeed;
        public float altitudeSpeed;
        public float droneMaxControlDistance;
        
        [Header("Control Setting")]
        public float mouseSensitivity;
        public float touchSensitivity;
        public float joystickSensitivity;
    }
}