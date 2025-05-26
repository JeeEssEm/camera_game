using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ListManager : MonoBehaviour
    {
        private int selectedIndex = -1;

        public void SelectItem(GameObject item)
        {

        }
        private void UpdateLayout()
        {
            if (TryGetComponent(out VerticalLayoutGroup layoutGroup))
                LayoutRebuilder.ForceRebuildLayoutImmediate(
                    (RectTransform)layoutGroup.transform
                );
        }
    }
}
