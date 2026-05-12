using Configuration.ExcelData.DataClass;
using UnityEngine;

namespace Features.Action.Domain
{
    public abstract class EntryEffect
    {
        public int EntryId { get; protected set; }

        public virtual void OnUse(MercenaryModel model, ActionResult result, BattleContext context) { }
        public virtual void OnTakeDamage(MercenaryModel model, ref int damage, ActionResult result, BattleContext context) { }
        public virtual void OnKill(MercenaryModel model, ActionResult result, BattleContext context) { }
        public virtual void OnTurnStart(MercenaryModel model, ActionResult result, BattleContext context) { }

        public virtual int GetForcedTargetIndex(MercenaryModel model, BattleContext context)
        {
            return -1;
        }

        public static EntryEffect Create(EntryInfo entry)
        {
            return entry?.Id switch
            {
                1 => new TenacityEffect(),
                2 => new RevitalizeEffect(),
                3 => new CounterEffect(),
                4 => new BloodthirstEffect(),
                5 => new ComboEffect(),
                6 => new SlowEffect(),
                7 => new StealEffect(),
                _ => new NullEffect()
            };
        }
    }

    // ============================================================
    // 空效果
    // ============================================================
    public class NullEffect : EntryEffect
    {
        public NullEffect() { EntryId = 0; }
    }

    // ============================================================
    // Entry 1 坚韧: 减伤50%，消耗一层
    // ============================================================
    public class TenacityEffect : EntryEffect
    {
        public TenacityEffect() { EntryId = 1; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context) { }

        public override void OnTakeDamage(MercenaryModel model, ref int damage, ActionResult result, BattleContext context)
        {
            if (model.TenacityStacks > 0)
            {
                damage = Mathf.CeilToInt(damage * 0.5f);
                model.TenacityStacks--;
            }
        }
    }

    // ============================================================
    // Entry 2 回春: 下回合恢复等量生命
    // ============================================================
    public class RevitalizeEffect : EntryEffect
    {
        public RevitalizeEffect() { EntryId = 2; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            model.HasRevitalize = true;
            model.RevitalizeAmount = result.Healing;
        }

        public override void OnTurnStart(MercenaryModel model, ActionResult result, BattleContext context)
        {
            if (model.HasRevitalize && model.RevitalizeAmount > 0)
            {
                result.Healing += model.RevitalizeAmount;
                model.HasRevitalize = false;
                model.RevitalizeAmount = 0;
            }
        }
    }

    // ============================================================
    // Entry 3 反击: 概率闪避并触发免费行动
    // ============================================================
    public class CounterEffect : EntryEffect
    {
        public CounterEffect() { EntryId = 3; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            model.HasCounter = true;
        }

        public override void OnTakeDamage(MercenaryModel model, ref int damage, ActionResult result, BattleContext context)
        {
            if (model.HasCounter && Random.value < model.CounterChance)
            {
                damage = 0;
                result.TriggerFreeAction = true;
                model.HasCounter = false;
            }
        }
    }

    // ============================================================
    // Entry 4 渴血: 锁定最虚弱目标，击杀翻倍吸血
    // ============================================================
    public class BloodthirstEffect : EntryEffect
    {
        public BloodthirstEffect() { EntryId = 4; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            result.Lifesteal = model.LifestealPerHit;
        }

        public override int GetForcedTargetIndex(MercenaryModel model, BattleContext context)
        {
            if (model.BloodthirstRestrictionRemoved) return -1;

            int weakest = -1;
            int minHp = int.MaxValue;
            for (int i = 0; i < context.AllEnemies.Count; i++)
            {
                if (context.AllEnemies[i].IsAlive && context.AllEnemies[i].CurrentHP < minHp)
                {
                    minHp = context.AllEnemies[i].CurrentHP;
                    weakest = i;
                }
            }

            return weakest;
        }

        public override void OnKill(MercenaryModel model, ActionResult result, BattleContext context)
        {
            result.Lifesteal *= 2;
        }
    }

    // ============================================================
    // Entry 5 连击: 目标受伤次数越多伤害越高
    // ============================================================
    public class ComboEffect : EntryEffect
    {
        public ComboEffect() { EntryId = 5; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            EnemyTarget target = context.AllEnemies[result.TargetIndex];
            result.Damage += model.ComboLevel * target.HitCountThisTurn;
        }
    }

    // ============================================================
    // Entry 6 迟缓: 敌人跳过下一回合
    // ============================================================
    public class SlowEffect : EntryEffect
    {
        public SlowEffect() { EntryId = 6; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            context.AllEnemies[result.TargetIndex].HasSlow = true;
        }
    }

    // ============================================================
    // Entry 7 盗窃: 概率盗取
    // ============================================================
    public class StealEffect : EntryEffect
    {
        public StealEffect() { EntryId = 7; }

        public override void OnUse(MercenaryModel model, ActionResult result, BattleContext context)
        {
            if (Random.value < model.StealChance)
            {
                result.Stolen = true;
            }
        }
    }
}