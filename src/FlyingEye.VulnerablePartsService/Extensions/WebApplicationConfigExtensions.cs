namespace FlyingEye.Extensions
{
    public static class WebApplicationConfigExtensions
    {
        public static WebApplication ConfigWebApplication(this WebApplication app)
        {
            app.UseAuthorization();

            if (app.Environment.IsDevelopment())
            {
                app.UseVersionedSwagger();
            }

            app.MapControllers();

            return app;
        }
    }
}
