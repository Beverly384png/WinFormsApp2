using System;

namespace WinFormsApp2.Services
{
    public static class ProductCalculator
    {
        public static int CalculateProductQuantity(
            int productTypeId,
            int materialTypeId,
            int rawMaterialQty,
            double param1,
            double param2)
        {
            if (productTypeId <= 0 || materialTypeId <= 0 || rawMaterialQty <= 0 || param1 <= 0 || param2 <= 0)
            {
                return -1;
            }

            try
            {
                double productCoef = 1.5;
                double lossPercent = 0.10;

                double materialPerUnit = param1 * param2 * productCoef;

                if (materialPerUnit <= 0) return -1;

                double baseQuantity = rawMaterialQty / materialPerUnit;

                double finalQuantity = baseQuantity * (1 - lossPercent);

                return (int)Math.Floor(finalQuantity);
            }
            catch
            {
                return -1;
            }
        }
    }
}