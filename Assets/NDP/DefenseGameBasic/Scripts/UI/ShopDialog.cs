using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UDEV.DefenseBasic
{
    public class ShopDialog : Dialog, IComponentChecking
    {
        public Transform gridRoot;
        public ShopItemUI itemUIPrefab;
        private GameManager m_gm;

        public override void Show(bool isShow)
        {
            //Pref.coins = 10000;
            base.Show(isShow);

            UpdateUI();
        }

        public bool IsComponentsNull()
        {
            return ShopManager.Ins == null && m_gm == null || gridRoot == null;
        }

        private void UpdateUI()
        {
            if (IsComponentsNull()) return;

            ClearChilds();

            var items = ShopManager.Ins.items;

            if(items == null || items.Length <= 0) return;

            for (int i = 0; i < items.Length; i++)
            {
                int idx = i;

                var item = items[idx];

                var itemUIClone = Instantiate(itemUIPrefab, Vector3.zero, Quaternion.identity);

                itemUIClone.transform.SetParent(gridRoot);

                itemUIClone.transform.localScale = Vector3.one; //(1, 1, 1);

                itemUIClone.transform.localPosition = Vector3.zero;// (0, 0, 0)

                itemUIClone.UpdateUI(item, idx);

                if (itemUIClone.btn)
                {
                    itemUIClone.btn.onClick.RemoveAllListeners();
                    itemUIClone.btn.onClick.AddListener(() => ItemEvent(item, idx));
                }
            }
        }

        private void ItemEvent(ShopItem item, int itemIdx)
        {
            if(item == null) return;

            bool isUnlocked = Pref.GetBool(Const.PLAYER_PREFIX_PREF + itemIdx);

            if (isUnlocked)
            {
                if (itemIdx == Pref.curPlayerId) return;

                Pref.curPlayerId = itemIdx;

                UpdateUI();
            }else if(Pref.coins >= item.price)
            {
                Pref.coins -= item.price;
                Pref.SetBool(Const.PLAYER_PREFIX_PREF + itemIdx, true);
                Pref.curPlayerId = itemIdx;

                UpdateUI();

                GUIManager.Ins.UpdateMainCoins();

            }else
            {
                Debug.Log("You dont have enough money!");
            }
        }

        public void ClearChilds()
        {
            if (gridRoot == null || gridRoot.childCount <= 0) return;

            for (int i = 0; i < gridRoot.childCount; i++)
            {
                var child = gridRoot.GetChild(i);

                if (child)
                    Destroy(child.gameObject);
            }
        }
    }
}
