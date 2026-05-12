using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;

namespace AlmondHousing
{
    public static unsafe class InventoryScanner
    {
        // 修复：去掉了容易引发枚举报错的房屋仓库，专注于玩家常规背包和陆行鸟鞍包
        private static readonly InventoryType[] InventoriesToCheck = new[]
        {
            InventoryType.Inventory1, 
            InventoryType.Inventory2, 
            InventoryType.Inventory3, 
            InventoryType.Inventory4,
            InventoryType.SaddleBag1, 
            InventoryType.SaddleBag2,
            InventoryType.PremiumSaddleBag1, 
            InventoryType.PremiumSaddleBag2
        };

        public static int GetOwnedCount(uint itemId)
        {
            var inventoryManager = InventoryManager.Instance();
            if (inventoryManager == null) return 0;

            int total = 0;
            foreach (var type in InventoriesToCheck)
            {
                var container = inventoryManager->GetInventoryContainer(type);
                
                // 🚀 修复1：在最新的 FFXIVClientStructs 中，加载状态叫 IsLoaded
                if (container == null || !container->IsLoaded) continue;

                for (int i = 0; i < container->Size; i++)
                {
                    var slot = container->GetInventorySlot(i);
                    
                    // 🚀 修复2：在最新的 FFXIVClientStructs 中，物品 ID 叫 ItemId
                    if (slot != null && slot->ItemId == itemId)
                    {
                        total += (int)slot->Quantity;
                    }
                }
            }
            return total;
        }
    }
}