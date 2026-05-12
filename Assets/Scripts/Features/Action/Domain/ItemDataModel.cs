using System.Text.RegularExpressions;
using Configuration.ExcelData.DataClass;

namespace Features.Action.Domain
{
    public class ItemDataModel
    {
        public PropInfo Config { get; }

        public int FogRevealRange;
        public int DigCount;
        public int EnergyGain;
        public int HealPercent;
        public bool RestoreAllMP;
        public int BonusEnergyCap;
        public bool CleanseAll;

        public ItemDataModel(PropInfo config)
        {
            Config = config;
            ParseDesc(config.Desc ?? "");
        }

        private void ParseDesc(string desc)
        {
            Match fog = Regex.Match(desc, @"解开视野(\d+)");
            if (fog.Success) FogRevealRange = int.Parse(fog.Groups[1].Value);

            Match dig = Regex.Match(desc, @"可以挖(\d+)次");
            if (dig.Success) DigCount = int.Parse(dig.Groups[1].Value);

            Match energy = Regex.Match(desc, @"能量点数\+(\d+)");
            if (energy.Success) EnergyGain = int.Parse(energy.Groups[1].Value);

            Match heal = Regex.Match(desc, @"(\d+)%");
            if (heal.Success) HealPercent = int.Parse(heal.Groups[1].Value);

            RestoreAllMP = desc.Contains("所有Mp");

            Match bonus = Regex.Match(desc, @"额外(\d+)能量上限");
            if (bonus.Success) BonusEnergyCap = int.Parse(bonus.Groups[1].Value);

            CleanseAll = desc.Contains("清除所有");
        }
    }
}