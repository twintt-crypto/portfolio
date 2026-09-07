using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

namespace S7
{
    public class UIFieldQuestTracker : MonoBehaviour
    {
        [SerializeField] private Button _questPopupButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        public void Initialize()
        {
            Debug.Log("[UIFieldQuestTracker] Initialize");
            _questPopupButton.onClick.AddListener(OnClickQuestPopup);
            Hide();
        }

        public void Release()
        {
            _questPopupButton.onClick.RemoveListener(OnClickQuestPopup);
        }

        public void Show(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Debug.Log("[UIFieldQuestTracker] Hide");
            gameObject.SetActive(false);
        }

        public void UpdateDescription(string description)
        {
            _descriptionText.text = description;
        }

        private void OnClickQuestPopup()
        {
            UIManager.Instance.OpenPanelAsync("UIPopupQuest").Forget();
        }
    }
}
