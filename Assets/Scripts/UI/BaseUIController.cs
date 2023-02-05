using UnityEngine;

namespace UI
{
    public abstract class BaseUIController : MonoBehaviour
    {
        /// <summary>
        /// Called at start to initialize the screen's component like event, item list, etc.
        /// </summary>
        public abstract void InitScreen();

        /// <summary>
        /// Rearrange the state of the screen, display the opening animation of the screen.
        /// </summary>
        public abstract void OpenScreen();

        /// <summary>
        /// Display the closing animation of the screen.
        /// </summary>
        public abstract void CloseScreen();

        private void Start()
        {
            InitScreen();
        }
    }
}
