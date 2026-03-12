using TMPro;
using UnityEngine;

namespace UI.Views
{
    public class InfoPanelView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsText;
        [SerializeField] private TextMeshProUGUI weightText;

        public void UpdateInfo(int coins, float weight)
        {
            coinsText.text = $"Coins: {coins}";
            weightText.text = $"Weight: {weight:F2} kg";
        }
    }
}
