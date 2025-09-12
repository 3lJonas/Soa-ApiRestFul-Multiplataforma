using ApiCliente.Services;
using Polly;
using Polly.Extensions.Http;

namespace ApiCliente
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();

            // HttpClient base a PedidosApi
            builder.Services.AddHttpClient<PedidosClient>(client =>
            {
                var baseUrl = builder.Configuration["PedidosApi:BaseUrl"] ?? "http://localhost:5089";
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => (int)msg.StatusCode == 429)
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)))
            );

            // UsersClient reutiliza la misma base
            builder.Services.AddHttpClient<UsersClient>(client =>
            {
                var baseUrl = builder.Configuration["PedidosApi:BaseUrl"] ?? "http://localhost:5089";
                client.BaseAddress = new Uri(baseUrl);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => (int)msg.StatusCode == 429)
                .WaitAndRetryAsync(3, i => TimeSpan.FromMilliseconds(200 * Math.Pow(2, i)))
            );

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.Run();
        }
    }
}
