using UnityEngine;
using UnityEngine.UI;

namespace UI.View
{
    public class MainPanelUIView : MonoBehaviour
    {
        public GameObject mobilePanel;
        public Button microphoneButton;
        public Image microphoneImage;
        public Image microphoneIconImage;
        public Button speakerButton;
        public Image speakerImage;
        public Image speakerIconImage;
        public Button exitButton;

        [Header("Mobile UI")]
        public Button jumpButton;
    }
}