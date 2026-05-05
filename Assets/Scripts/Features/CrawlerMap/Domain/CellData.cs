using System;

namespace Features.CrawlerMap.Domain
{
    [Serializable]
    public class CellData
    {
        public CellContentType ContentType = CellContentType.Empty;
        public CellContentType OverlayType = CellContentType.Empty;
        public int ContentId;
        public string ContentName = string.Empty;

        public bool HasOverlay => OverlayType != CellContentType.Empty
                                  && OverlayType != CellContentType.Eraser
                                  && OverlayType != CellContentType.Space
                                  && OverlayType != CellContentType.Wall;

        public CellData Clone()
        {
            return new CellData
            {
                ContentType = ContentType,
                OverlayType = OverlayType,
                ContentId = ContentId,
                ContentName = ContentName
            };
        }
    }
}