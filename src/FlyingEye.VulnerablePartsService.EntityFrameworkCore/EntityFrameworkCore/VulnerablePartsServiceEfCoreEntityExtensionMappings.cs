using Volo.Abp.Threading;

namespace FlyingEye.EntityFrameworkCore
{
    /// <summary>
    /// 实体扩展映射。
    /// </summary>
    public static class VulnerablePartsServiceEfCoreEntityExtensionMappings
    {
        private static readonly OneTimeRunner OneTimeRunner = new OneTimeRunner();

        public static void Configure()
        {
            VulnerablePartsServiceModulePropertyConfigurator.Configure();

            OneTimeRunner.Run(() =>
            {
                /* You can configure entity extension properties for the
                 * entities defined in the used modules.
                 *
                 * The properties defined here becomes table fields.
                 * If you want to use the ExtraProperties dictionary of the entity
                 * instead of creating a new field, then define the property in the
                 * BookStoreDomainObjectExtensions class.
                 *
                 * Example:
                 *
                 * ObjectExtensionManager.Instance
                 *    .MapEfCoreProperty<IdentityUser, string>(
                 *        "MyProperty",
                 *        b => b.HasMaxLength(128)
                 *    );
                 *
                 * See the documentation for more:
                 * https://docs.abp.io/en/abp/latest/Customizing-Application-Modules-Extending-Entities
                 */

                ////SET AN EXTRA PROPERTY
                //var user = await _identityUserRepository.GetAsync(userId);
                //user.SetProperty("Title", "My custom title value!");
                //await _identityUserRepository.UpdateAsync(user);

                ////GET AN EXTRA PROPERTY
                //var user = await _identityUserRepository.GetAsync(userId);
                //return user.GetProperty<string>("Title");

            });
        }
    }
}
