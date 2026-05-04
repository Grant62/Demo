using System;

namespace Features.CrawlerMap.Domain
{
    [Serializable]
    public class CellData
    {
        public CellContentType ContentType = CellContentType.Empty;
        public int ContentId;
        public string ContentName = string.Empty;

        public CellData Clone()
        {
            return new CellData
            {
                ContentType = ContentType,
                ContentId = ContentId,
                ContentName = ContentName
            };
        }
    }
}