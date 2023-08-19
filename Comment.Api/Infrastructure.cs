using Comment.Api.Constants;
using Comment.Api.Context;
using Comment.Api.Filters;
using Comment.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Comment.Api
{
    public class Infrastructure
    {
        public static WebApplication ConfigureApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMvcCore(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)));

            builder.Services.AddDbContext<CommentContext>(options => options
                .UseSqlServer(builder.Configuration[ConfigurationConstants.ConnectionString]));
            builder.Services.AddControllers();

            builder.Services.AddScoped<CommentService>();

            return UseApplication(builder);
        }

        public static WebApplication UseApplication(WebApplicationBuilder builder)
        {
            var app = builder.Build();

            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Token-Expired")
                .SetIsOriginAllowed(origin => true)
                .AllowCredentials());
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();

            return app;
        }

    }
}
