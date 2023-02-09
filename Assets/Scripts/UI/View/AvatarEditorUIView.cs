using HSVPicker;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.View
{
    public class AvatarEditorUIView : MonoBehaviour
    {
        public TouchArea touchArea;
        public EventTrigger rotateLeftEventTrigger;
        public EventTrigger rotateRightEventTrigger;
        public TMP_Text nicknameText;
        public Slider scaleSlider;

        public Button backButton;
        public Button confirmButton;
        public Button randomButton;

        public TMP_InputField nicknameInputField;
        public Button maleGenderButton;
        public Button femaleGenderButton;
        public TMP_Text heightText;
        public Slider heightSlider;
        
        public AvatarDataList skinToneDataList;
        public AvatarDataList faceDataList;
        public AvatarDataList hairDataList;
        public AvatarDataList outfitDataList;
        public AvatarDataList glassesDataList;

        public ColorPicker hairColorPicker;
    }
}
