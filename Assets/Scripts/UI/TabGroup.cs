using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        [SerializeField] private List<Tab> tabs;
        private int _currentTabIndex;
        
        public Action<Tab> OnTabSelected;

        private void Awake()
        {
            for (var i = 0; i < tabs.Count; i++)
            {
                int index = i;
                var tab = tabs[index];
                tab.tabButton.onClick.AddListener(delegate { SelectTab(index); });
                tab.tabContent.SetActive(false);
            }
            
            SelectTab(_currentTabIndex);
        }

        private void SelectTab(int index)
        {
            tabs[_currentTabIndex].tabContent.SetActive(false);
                
            _currentTabIndex = index;
            tabs[_currentTabIndex].tabContent.SetActive(true);

            OnTabSelected?.Invoke(tabs[index]);
        }
    }
    
    [Serializable]
    public struct Tab
    {
        public Button tabButton;
        public GameObject tabContent;
    }
}