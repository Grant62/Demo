using System.Text.RegularExpressions;
using Configuration.ExcelData.DataClass;

namespace Features.Action.Domain
{
    public class SpellDataModel
    {
        public SpellInfo Config { get; }

        public int MaxCharges;
        public int BaseDamage;
        public bool UnlockNextOnKill;
        public float[] DamageMultipliers;

        public SpellDataModel(SpellInfo config)
        {
            Config = config;
            ParseDesc(config.Desc ?? "");
        }

        private void ParseDesc(string desc)
        {
            Match chargeMatch = Regex.Match(desc, @"(\d+)段使用次数");
            if (chargeMatch.Success)
                MaxCharges = int.Parse(chargeMatch.Groups[1].Value);

            Match dmgMatch = Regex.Match(desc, @"基础伤害(\d+)");
            if (dmgMatch.Success)
                BaseDamage = int.Parse(dmgMatch.Groups[1].Value);

            UnlockNextOnKill = desc.Contains("击杀敌人解锁");

            Match multiMatch = Regex.Match(desc, @"倍率x([\d.,]+)");
            if (multiMatch.Success)
            {
                string[] parts = multiMatch.Groups[1].Value.Split(',');
                DamageMultipliers = new float[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    if (float.TryParse(parts[i], out float val))
                        DamageMultipliers[i] = val;
                }
            }
        }
    }
}