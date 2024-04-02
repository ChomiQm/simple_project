namespace Task_Backend.Middlewares
{
    public static class LastActivityMiddlewareExtensions
    {
        public static IApplicationBuilder UseLastActivity(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LastActivityMiddleware>();
        }
    }
}
