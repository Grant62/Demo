using System.Collections.Generic;
using Configuration.ExcelData.DataClass;
using Features.Action.Domain;
using Features.Battle.Domain;

namespace Features.Action.Application
{
    public static class MercenaryService
    {
        public static ActionResult Execute(MercenaryModel model, int targetIndex, BattleContext context)
        {
            ActionResult result = new();
            result.TargetIndex = targetIndex;

            if (!EnergySystem.TrySpendByPoint(model.EnergyCost))
            {
                return result;
            }

            EnemyTarget target = context.AllEnemies[targetIndex];
            bool isHealer = model.Occupation.OccuId == 3;

            if (isHealer)
            {
                result.Healing = model.HealAmount;
            }
            else
            {
                result.Damage = model.BaseDamage;
            }

            model.EntryEffect.OnUse(model, result, context);

            if (!isHealer)
            {
                target.CurrentHP -= result.Damage;
                target.HitCountThisTurn++;
                if (target.CurrentHP <= 0)
                {
                    result.TargetKilled = true;
                    model.EntryEffect.OnKill(model, result, context);
                }

                if (result.Lifesteal > 0)
                {
                    result.Healing += result.Lifesteal;
                }
            }
            else
            {
                if (model.HasRevitalize)
                {
                    model.RevitalizeAmount = result.Healing;
                }
            }

            return result;
        }

        public static int ProcessPlayerDamage(List<MercenaryModel> activeMercenaries, int incomingDamage, BattleContext context, out bool triggerFreeAction)
        {
            ActionResult result = new();
            int damage = incomingDamage;
            triggerFreeAction = false;

            foreach (MercenaryModel model in activeMercenaries)
            {
                model.EntryEffect.OnTakeDamage(model, ref damage, result, context);
                if (result.TriggerFreeAction)
                {
                    triggerFreeAction = true;
                }
            }

            return damage;
        }

        public static ActionResult ProcessTurnStart(MercenaryModel model, BattleContext context)
        {
            ActionResult result = new();
            model.EntryEffect.OnTurnStart(model, result, context);
            return result;
        }

        public static int GetForcedTarget(MercenaryModel model, BattleContext context)
        {
            return model.EntryEffect.GetForcedTargetIndex(model, context);
        }

        public static MercenaryModel CreateModel(OccupationInfo occupation, EntryInfo entry)
        {
            MercenaryModel model = new(occupation, entry);
            model.SetEntryEffect(EntryEffect.Create(entry));
            return model;
        }
    }
}