using System.Text.RegularExpressions;
using Configuration.ExcelData.DataClass;

namespace Features.Action.Domain
{
    public class MercenaryModel
    {
        public OccupationInfo Occupation { get; }
        public EntryInfo Entry { get; }
        public string ProcessedDesc { get; private set; }

        // ====== 基础动作 ======
        public int BaseDamage;
        public int SplashDamage;
        public bool IsAOE;
        public int HitCount = 1;
        public int HealAmount;

        // ====== Entry 1 坚韧 ======
        public int TenacityStacks;
        public int RemoveDebuffCount;

        // ====== Entry 2 回春 ======
        public bool HasRevitalize;
        public float RevitalizeChance;
        public int RevitalizeAmount;

        // ====== Entry 3 反击 ======
        public bool HasCounter;
        public float CounterChance;

        // ====== Entry 4 渴血 ======
        public bool HasBloodthirst;
        public bool BloodthirstRestrictionRemoved;
        public int LifestealPerHit;

        // ====== Entry 5 连击 ======
        public int ComboLevel;

        // ====== Entry 6 迟缓 ======
        public float SlowChance;
        public int SlowDuration;

        // ====== Entry 7 盗窃 ======
        public bool HasSteal;
        public float StealChance;
        public float ArtifactStealChance;

        public MercenaryModel(OccupationInfo occupation, EntryInfo entry)
        {
            Occupation = occupation;
            Entry = entry;
            string desc = occupation.Desc.Replace("\\n", "\n");
            ParseBase(desc);
            ParseEntry(desc, entry);
            ProcessedDesc = desc;
        }

        private void ParseBase(string desc)
        {
            Match damageMatch = Regex.Match(desc, @"造成(\d+)");
            if (damageMatch.Success)
                BaseDamage = int.Parse(damageMatch.Groups[1].Value);

            Match splashMatch = Regex.Match(desc, @"其他目标造成(\d+)");
            if (splashMatch.Success)
                SplashDamage = int.Parse(splashMatch.Groups[1].Value);

            IsAOE = desc.Contains("群体伤害");

            Match hitMatch = Regex.Match(desc, @"x(\d+)次");
            if (hitMatch.Success)
                HitCount = int.Parse(hitMatch.Groups[1].Value);

            Match healMatch = Regex.Match(desc, @"回复(\d+)");
            if (healMatch.Success)
                HealAmount = int.Parse(healMatch.Groups[1].Value);
        }

        private void ParseEntry(string desc, EntryInfo entry)
        {
            switch (entry?.Id)
            {
                case 1:
                    Match tenacityMatch = Regex.Match(desc, @"获得(\d+)层坚韧");
                    if (tenacityMatch.Success)
                        TenacityStacks = int.Parse(tenacityMatch.Groups[1].Value);
                    Match removeMatch = Regex.Match(desc, @"移除(\d+)个随机");
                    if (removeMatch.Success)
                        RemoveDebuffCount = int.Parse(removeMatch.Groups[1].Value);
                    break;

                case 2:
                    HasRevitalize = true;
                    Match reviveMatch = Regex.Match(desc, @"(\d+)%概率回春");
                    if (reviveMatch.Success)
                        RevitalizeChance = int.Parse(reviveMatch.Groups[1].Value) / 100f;
                    break;

                case 3:
                    HasCounter = true;
                    Match counterMatch = Regex.Match(desc, @"概率(\d+)%");
                    if (counterMatch.Success)
                        CounterChance = int.Parse(counterMatch.Groups[1].Value) / 100f;
                    break;

                case 4:
                    HasBloodthirst = true;
                    BloodthirstRestrictionRemoved = desc.Contains("渴血解除攻击目标限制");
                    Match lifestealMatch = Regex.Match(desc, @"吸血(\d+)");
                    if (lifestealMatch.Success)
                        LifestealPerHit = int.Parse(lifestealMatch.Groups[1].Value);
                    break;

                case 5:
                    Match comboMatch = Regex.Match(desc, @"连击(\d+)");
                    if (comboMatch.Success)
                        ComboLevel = int.Parse(comboMatch.Groups[1].Value);
                    break;

                case 6:
                    Match slowMatch = Regex.Match(desc, @"(\d+)%概率获得迟缓");
                    if (slowMatch.Success)
                        SlowChance = int.Parse(slowMatch.Groups[1].Value) / 100f;
                    Match slowDurMatch = Regex.Match(desc, @"迟缓\*(\d+)");
                    if (slowDurMatch.Success)
                        SlowDuration = int.Parse(slowDurMatch.Groups[1].Value);
                    break;

                case 7:
                    HasSteal = true;
                    Match stealMatch = Regex.Match(desc, @"(\d+)%概率盗窃");
                    if (stealMatch.Success)
                        StealChance = int.Parse(stealMatch.Groups[1].Value) / 100f;
                    Match artifactMatch = Regex.Match(desc, @"(\d+)%的概率盗窃到神器");
                    if (artifactMatch.Success)
                        ArtifactStealChance = int.Parse(artifactMatch.Groups[1].Value) / 100f;
                    break;
            }
        }

        public EntryEffect EntryEffect { get; private set; }

        public void SetEntryEffect(EntryEffect effect)
        {
            EntryEffect = effect;
        }

        public int EnergyCost { get => Occupation.Point; }

        public string DisplayName { get => Occupation.Name; }

        public string DisplayDesc { get => Occupation.Desc; }

        public string ResAddress { get => Occupation.ResAddress; }
    }
}