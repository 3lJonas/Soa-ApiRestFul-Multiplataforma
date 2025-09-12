namespace ApiPedidos.Middleware
{
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        public async Task Invoke(HttpContext context)
        {
            var id = Guid.NewGuid().ToString("n")[..8];
            var path = context.Request.Path;
            var method = context.Request.Method;

            logger.LogInformation("REQ {Id} {Method} {Path}", id, method, path);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                await next(context);
            }
            finally
            {
                sw.Stop();
                logger.LogInformation("RES {Id} {Status} {Elapsed}ms", id, context.Response.StatusCode, sw.ElapsedMilliseconds);
            }
        }
    }
}
