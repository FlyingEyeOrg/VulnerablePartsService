namespace FlyingEye.Spacers
{
    /// <summary>
    /// 垫片信息记录
    /// </summary>
    public class SpacerValidationDataRecordModel : SpacerValidationDataModel
    {
        public SpacerValidationDataRecordModel(
            string site,
            string resourceId,
            string @operator,
            string modelPn,
            string date,
            string bigCoatingWidth,
            string smallCoatingWidth,
            string whiteSpaceWidth,
            string aT11Width,
            string thickness,
            string aBSite) : base(site, resourceId, @operator, modelPn, date, bigCoatingWidth, smallCoatingWidth, whiteSpaceWidth, aT11Width, thickness, aBSite)
        {
        }
    }
}
