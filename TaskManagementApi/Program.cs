using NLog;
using NLog.Web;
using System;
using TaskManagementApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Data;

//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
//logger.Debug("init main");

//try
//{

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TaskManagementApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagementApiContext") ?? throw new InvalidOperationException("Connection string 'TaskManagementApiContext' not found.")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<JWTBase, JWTServices>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{
    // You need to import package as follow
    // using Microsoft.IdentityModel.Tokens;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        #region  配置驗證發行者

        ValidateIssuer = true, // 是否要啟用驗證發行者
        ValidIssuer = builder.Configuration.GetSection("JWTConfig").GetValue<string>("Issuer"),

        #endregion

        #region 配置驗證接收方

        ValidateAudience = false, // 是否要啟用驗證接收者
                                  // ValidAudience = "" // 如果不需要驗證接收者可以註解

        #endregion

        #region 配置驗證Token有效期間

        ValidateLifetime = true, // 是否要啟用驗證有效時間

        #endregion

        #region 配置驗證金鑰

        ValidateIssuerSigningKey = false, // 是否要啟用驗證金鑰，一般不需要去驗證，因為通常Token內只會有簽章

        #endregion

        #region 配置簽章驗證用金鑰

        // 這裡配置是用來解Http Request內Token加密
        // 如果Secret Key跟當初建立Token所使用的Secret Key不一樣的話會導致驗證失敗
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("JWTConfig").GetValue<string>("SignKey")
            )
        )

        #endregion
    };
});

builder.Logging.ClearProviders();
builder.Host.UseNLog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

//}
//catch (Exception ex)
//{
//    logger.Error(ex, "Stopped program because of exception");
//    throw;
//}
//finally
//{
//    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
//    NLog.LogManager.Shutdown();
//}
