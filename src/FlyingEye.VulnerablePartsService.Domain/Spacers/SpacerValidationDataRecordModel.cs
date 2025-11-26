using Volo.Abp.Domain.Entities.Auditing;

namespace FlyingEye.Spacers
{
    /// <summary>
    /// 垫片信息记录
    /// </summary>
    public class SpacerValidationDataRecordModel : AuditedEntity<Guid>
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
            string aBSite)
        {
            Site = site;
            ResourceId = resourceId;
            Operator = @operator;
            ModelPn = modelPn;
            Date = date;
            BigCoatingWidth = bigCoatingWidth;
            SmallCoatingWidth = smallCoatingWidth;
            WhiteSpaceWidth = whiteSpaceWidth;
            AT11Width = aT11Width;
            Thickness = thickness;
            ABSite = aBSite;
        }

        /// <summary>
        /// 基地编号，例如：HD
        /// </summary>
        public string Site { get; set; }

        /// <summary>
        /// 设备资源号
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// 操作员卡号
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 1. 机种
        /// </summary>
        public string ModelPn { get; set; }

        /// <summary>
        /// 2. 时间
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 3. 大膜宽
        /// </summary>
        public string BigCoatingWidth { get; set; }

        /// <summary>
        /// 4. 小膜宽
        /// </summary>
        public string SmallCoatingWidth { get; set; }

        /// <summary>
        /// 5. 极耳宽度
        /// </summary>
        public string WhiteSpaceWidth { get; set; }

        /// <summary>
        /// 6. AT11 宽度
        /// </summary>
        public string AT11Width { get; set; }

        /// <summary>
        /// 7. 垫片厚度
        /// </summary>
        public string Thickness { get; set; }

        /// <summary>
        /// 8. A/B 面
        /// </summary>
        public string ABSite { get; set; }
    }
}
