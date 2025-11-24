namespace FlyingEye.Extensions
{
    public static class ServiceOptionsExetnsions
    {
        public static WebApplicationBuilder ConfigOptions(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // register you options here, such as:
            // services.Configure<DeviceClientServiceOptions>(configuration.GetSection(key: nameof(DeviceClientServiceOptions)));

            return builder;
        }
    }
}
