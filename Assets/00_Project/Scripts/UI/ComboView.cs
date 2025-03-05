using DamageNumbersPro;
using UnityEngine;

namespace LegionKnight
{
    public partial class ComboView : UIView
    {
        [SerializeField]
        private DamageNumberGUI m_ComboNumber;
        public void SpawnComboNumber(int combo)
        {
            ShowInternal();
            string number = $"{combo}x PERFECT";
            DamageNumber damageNumber = m_ComboNumber.Spawn(m_Content.transform.position, number);
            damageNumber.SetAnchoredPosition(RectContent, RectContent.anchoredPosition);
            Debug.Log($"Combo {combo}");
        }
    }
    public partial class GameplayPanel
    {
        private ComboView GetComboView()
        {
            return GetBinding<ComboView>();
        }
        public void SpawnComboNumber(int combo)
        {
            GetComboView().SpawnComboNumber(combo);
        }
    }
    public partial class GameManager
    {
        public void SpawnComboNumberUI(int combo)
        {
            GetPanelInternal<GameplayPanel>().SpawnComboNumber(combo);
        }
    }
}
