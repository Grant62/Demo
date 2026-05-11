using Configuration.ExcelData.DataClass;

namespace Features.Action.Domain
{
    public class MercenaryModel
    {
        public OccupationInfo Occupation { get; }
        public EntryInfo Entry { get; }

        // Entry 1 坚韧
        public int TenacityStacks;

        // Entry 2 回春
        public bool HasRevitalize;
        public int RevitalizeAmount;

        // Entry 3 反击
        public bool HasCounter;
        public float CounterChance;

        // Entry 4 渴血
        public bool HasBloodthirst;
        public bool BloodthirstRestrictionRemoved;
        public int LifestealPerHit;

        // Entry 5 连击
        public int ComboLevel;

        // Entry 6 迟缓 (debuff 挂在敌人身上，不需要模型状态)

        // Entry 7 盗窃
        public bool HasSteal;
        public float StealChance;

        public MercenaryModel(OccupationInfo occupation, EntryInfo entry)
        {
            Occupation = occupation;
            Entry = entry;

            switch (entry?.Id)
            {
                case 1:  // 坚韧
                    TenacityStacks = occupation.Id switch
                    {
                        1 or 2 => 1,
                        3 or 4 => 2,
                        _ => 1
                    };
                    break;

                case 2:  // 回春
                    HasRevitalize = true;
                    break;

                case 3:  // 反击
                    HasCounter = true;
                    CounterChance = occupation.Id switch
                    {
                        13 => 0.10f,
                        14 => 0.20f,
                        15 => 0.25f,
                        16 => 0.30f,
                        _ => 0
                    };
                    break;

                case 4:  // 渴血
                    HasBloodthirst = true;
                    BloodthirstRestrictionRemoved = occupation.Id == 20;
                    LifestealPerHit = occupation.Id switch
                    {
                        17 or 18 => 1,
                        19 or 20 => 2,
                        _ => 0
                    };
                    break;

                case 5:  // 连击
                    ComboLevel = occupation.Id switch
                    {
                        21 => 1,
                        22 or 23 => 2,
                        24 => 3,
                        _ => 0
                    };
                    break;

                case 6:  // 迟缓
                    break;

                case 7:  // 盗窃
                    HasSteal = true;
                    StealChance = occupation.Id switch
                    {
                        29 => 0.10f,
                        30 => 0.15f,
                        31 => 0.20f,
                        32 => 0.25f,
                        _ => 0
                    };
                    break;
            }
        }

        public EntryEffect EntryEffect { get; private set; }

        public void SetEntryEffect(EntryEffect effect)
        {
            EntryEffect = effect;
        }

        public int EnergyCost => Occupation.Point;

        public string DisplayName => Occupation.Name;

        public string DisplayDesc => Occupation.Desc;

        public string ResAddress => Occupation.ResAddress;
    }
}
