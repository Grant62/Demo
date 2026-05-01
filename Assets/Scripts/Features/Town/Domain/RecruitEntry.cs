namespace Features.Town.Domain
{
    public enum RecruitCategory
    {
        兵种,
        法术
    }

    public enum UnlockSource
    {
        步兵营,
        教堂,
        哨兵所,
        盗贼工会,
        狂战士营地,
        修道院,
        魔法师协会
    }

    public class RecruitEntry
    {
        public string displayName;
        public string description;
        public int energyCost;
        public RecruitCategory category;
        public UnlockSource unlockSource;
        public int maxUses = -1;

        public RecruitEntry(
            string displayName,
            string description,
            int energyCost,
            RecruitCategory category,
            UnlockSource unlockSource,
            int maxUses = -1)
        {
            this.displayName = displayName;
            this.description = description;
            this.energyCost = energyCost;
            this.category = category;
            this.unlockSource = unlockSource;
            this.maxUses = maxUses;
        }
    }
}