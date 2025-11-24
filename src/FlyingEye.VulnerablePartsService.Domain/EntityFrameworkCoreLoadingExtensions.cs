using System.Runtime.CompilerServices;

namespace FlyingEye
{
    public static class EntityFrameworkCoreLoadingExtensions
    {
        public static TRelated Load<TRelated>(
            this Action<object, string> loader,
            object entity,
            ref TRelated navigationField,
            [CallerMemberName] string navigationName = "")
            where TRelated : class
        {
            loader?.Invoke(entity, navigationName);

            return navigationField;
        }

#nullable disable // 禁用可空警告
        public static TRelated Load<TEntity, TRelated>(this TEntity entity,
            ref TRelated navigationField,
            [CallerMemberName] string navigationName = "") where TEntity : IPropertyLoader where TRelated : class
        {
            // 因为 LazyLoader 是由 EF 注入，当我们手动创建 model 时 LazyLoader 是空的
            // 所以这里使用 ? 表达式，此时我们直接返回 navigationField 即可
            entity.LazyLoader?.Invoke(entity, navigationName);
            return navigationField;
        }
#nullable restore // 恢复上下文可空警告
    }
}
