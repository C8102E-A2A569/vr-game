using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [System.Serializable]
    public class InventorySlot
    {
        public Image icon;
        public Text countText;
        [HideInInspector] public int count = 0;
        [HideInInspector] public Sprite currentItem = null;
    }

    public InventorySlot[] slots;

    // Добавляем предмет по иконке
    public bool AddItem(Sprite itemIcon)
    {
        // Ищем, есть ли уже такой предмет в инвентаре
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == itemIcon)
            {
                slots[i].count++;
                slots[i].countText.text = slots[i].count.ToString();
                return true;
            }
        }

        // Если такого нет — ищем пустой слот
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null)
            {
                slots[i].currentItem = itemIcon;
                slots[i].icon.sprite = itemIcon;
                slots[i].icon.enabled = true;
                slots[i].count = 1;
                slots[i].countText.text = "1";
                return true;
            }
        }

        // Свободных слотов нет
        return false;
    }
}
