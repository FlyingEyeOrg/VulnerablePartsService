namespace FlyingEye
{
    public interface IPropertyLoader
    {
        /// <summary>
        /// 导航属性加载器
        /// </summary>
        Action<object, string> LazyLoader { get; }
    }
}
