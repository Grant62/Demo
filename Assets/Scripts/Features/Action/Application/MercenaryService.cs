using System.Collections.Generic;
using System.Text.RegularExpressions;
using Configuration.ExcelData.DataClass;
using Features.Action.Domain;
using Features.Battle.Domain;

namespace Features.Action.Application
{
    public static class MercenaryService
    {
        /// <summary>
        /// 执行雇佣兵行动，返回行动结果
        /// </summary>
        public static ActionResult Execute(MercenaryModel model, int targetIndex, BattleContext context)
        {
            ActionResult result = new ActionResult();
            result.TargetIndex = targetIndex;

            if (!EnergySystem.TrySpendByPoint(model.EnergyCost))
            {
                return result;
            }

            EnemyTarget target = context.AllEnemies[targetIndex];
            bool isHealer = IsHealer(model.Occupation);

            if (isHealer)
            {
                result.Healing = ExtractHealAmount(model.Occupation);
            }
            else
            {
                result.Damage = ExtractDamage(model.Occupation);
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

        /// <summary>
        /// 处理玩家受到伤害，遍历所有已上阵佣兵的词条效果（坚韧减伤、反击闪避等）
        /// </summary>
        public static int ProcessPlayerDamage(List<MercenaryModel> activeMercenaries, int incomingDamage, BattleContext context, out bool triggerFreeAction)
        {
            ActionResult result = new ActionResult();
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

        /// <summary>
        /// 处理回合开始时的词条结算（回春等）
        /// </summary>
        public static ActionResult ProcessTurnStart(MercenaryModel model, BattleContext context)
        {
            ActionResult result = new ActionResult();
            model.EntryEffect.OnTurnStart(model, result, context);
            return result;
        }

        /// <summary>
        /// 获取词条强制指定的目标索引，-1 表示自由选择
        /// </summary>
        public static int GetForcedTarget(MercenaryModel model, BattleContext context)
        {
            return model.EntryEffect.GetForcedTargetIndex(model, context);
        }

        /// <summary>
        /// 从职业描述中解析基础伤害值
        /// "造成{n}伤害" / "造成{n}单体伤害" / "造成{n}法术伤害x次"
        /// </summary>
        public static int ExtractDamage(OccupationInfo occupation)
        {
            Match match = Regex.Match(occupation.Desc, @"造成(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return 0;
        }

        /// <summary>
        /// 从职业描述中解析治疗量
        /// "回复{n}生命"
        /// </summary>
        public static int ExtractHealAmount(OccupationInfo occupation)
        {
            Match match = Regex.Match(occupation.Desc, @"回复(\d+)");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return 0;
        }

        /// <summary>
        /// 判断是否为治疗型职业（OccuId 3 修女系）
        /// </summary>
        public static bool IsHealer(OccupationInfo occupation)
        {
            return occupation.OccuId == 3;
        }

        /// <summary>
        /// 为雇佣兵创建完整运行时模型
        /// </summary>
        public static MercenaryModel CreateModel(OccupationInfo occupation, EntryInfo entry)
        {
            MercenaryModel model = new MercenaryModel(occupation, entry);
            model.SetEntryEffect(EntryEffect.Create(entry));
            return model;
        }
    }
}
