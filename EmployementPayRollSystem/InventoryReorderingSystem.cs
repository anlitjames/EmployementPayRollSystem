using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Collections.Generic;

namespace EmployementPayRollSystem
{
    public class InventoryItem
    {
        public int ItemId { get; set; }
        public int CurrentStock { get; set; }
        public int ForecastedDemand { get; set; }
        public double ReorderCostPerUnit { get; set; }
        public int ReorderBatchSize { get; set; }
    }

    public class InventoryReorderingSystem
    {
        public List<InventoryItem> Items { get; set; }

        public InventoryReorderingSystem(List<InventoryItem> items)
        {
            Items = items;
        }

        public List<(int ItemId, int UnitsToOrder)> GenerateReorderingPlan()
        {
            List<(int ItemId, int UnitsToOrder)> reorderPlan = new List<(int, int)>();

            foreach (var item in Items)
            {
                int unitsToOrder = 0;

                // Check if the stock level is lower than the forecasted demand
                if (item.CurrentStock < item.ForecastedDemand)
                {
                    // Calculate how many units to reorder, ensuring no stock-outs
                    int deficit = item.ForecastedDemand - item.CurrentStock;

                    // Calculate reorder batches required
                    unitsToOrder = (int)Math.Ceiling((double)deficit / item.ReorderBatchSize) * item.ReorderBatchSize;
                }

                if (unitsToOrder > 0)
                {
                    reorderPlan.Add((item.ItemId, unitsToOrder));
                }
            }

            return reorderPlan;
        }
    }

    
}

