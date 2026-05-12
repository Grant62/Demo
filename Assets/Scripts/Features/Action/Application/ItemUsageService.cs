using Features.Action.Domain;

namespace Features.Action.Application
{
    public static class ItemUsageService
    {
        public static void Use(ItemDataModel model)
        {
            if (model.FogRevealRange > 0) { }

            if (model.DigCount > 0) { }

            if (model.EnergyGain > 0) { }

            if (model.HealPercent > 0) { }

            if (model.RestoreAllMP) { }

            if (model.BonusEnergyCap > 0) { }

            if (model.CleanseAll) { }
        }
    }
}