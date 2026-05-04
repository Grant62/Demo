using System;
using System.Collections.Generic;

namespace Features.Town.Domain
{
    [Serializable]
    public class RecruitSaveData
    {
        public List<int> occupationIds = new();
    }
}