using UI.View;
using UnityEngine;

namespace UI.Controller
{
    public class MainPanelUIController : BaseUIController
    {
        [SerializeField] private MainPanelUIView view;
        [SerializeField] private Sprite[] microphoneIcons;
        [SerializeField] private Sprite[] speakerIcons;
        [SerializeField] private Color[] hotBarButtonColors;

        private bool _isMuted = true;
        private bool _isDeafen;
        
        public override void InitScreen()
        {
            view.mobilePanel.SetActive(MultiPlatformHelper.Instance.IsPlayingOnAndroid());
            
            view.microphoneButton.onClick.AddListener(ToggleMicrophone);
            view.speakerButton.onClick.AddListener(ToggleSpeaker);
            view.exitButton.onClick.AddListener(ExitSession);
            
            view.jumpButton.onClick.AddListener(Jump);
        }

        public override void OpenScreen()
        {
        }

        public override void CloseScreen()
        {
        }

        private void ToggleMicrophone()
        {
            _isMuted = !_isMuted;

            view.microphoneImage.color = hotBarButtonColors[_isMuted ? 1 : 0];
            view.microphoneIconImage.sprite = microphoneIcons[_isMuted ? 1 : 0];
        }

        private void ToggleSpeaker()
        {
            _isDeafen = !_isDeafen;

            view.speakerImage.color = hotBarButtonColors[_isDeafen ? 1 : 0];
            view.speakerIconImage.sprite = speakerIcons[_isDeafen ? 1 : 0];
        }

        private void ExitSession()
        {
            NetworkManager.Instance.LeaveRoom();
        }

        private void Jump()
        {
            NonVRPlayerController.Instance.Jump();
        }
    }
}