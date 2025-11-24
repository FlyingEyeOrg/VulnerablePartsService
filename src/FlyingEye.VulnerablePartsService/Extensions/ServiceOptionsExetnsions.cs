

namespace FlyingEye.Extensions
{
    public static class ServiceOptionsExetnsions
    {
        public static WebApplicationBuilder ConfigOptions(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            // services.Configure<DeviceClientServiceOptions>(configuration.GetSection(key: nameof(DeviceClientServiceOptions)));
            // services.Configure<HttpClientServiceOptions>(configuration.GetSection(key: nameof(HttpClientServiceOptions)));
            // services.Configure<DeviceClientServiceOptions>(configuration.GetSection(key: nameof(DeviceClientServiceOptions)));
            // services.Configure<ComeUserNotificationOptions>(configuration.GetSection(key: nameof(ComeUserNotificationOptions)));
            // services.Configure<ComeGroupNotificationOptions>(configuration.GetSection(key: nameof(ComeGroupNotificationOptions)));

            return builder;
        }
    }
}
