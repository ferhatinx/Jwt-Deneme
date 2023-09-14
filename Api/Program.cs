using Api.Context;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddDbContext<JwtContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("local"));
});
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 0;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = false;
    options.SignIn.RequireConfirmedEmail = true;

})
   .AddEntityFrameworkStores<JwtContext>()
   .AddDefaultTokenProviders();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOpt =>
{
    jwtOpt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        //! Token'da aktör (actor) bilgisinin doðrulanýp doðrulanmayacaðýný belirler. Genellikle false olarak ayarlanýr, çünkü bu bilgi genellikle kullanýlmaz.
        ValidateActor = true,

        //! Token'ýn verici (issuer) bilgisinin doðrulanýp doðrulanmayacaðýný belirler. Token'ýn hangi servis tarafýndan oluþturulduðunu kontrol etmek için kullanýlýr.
        ValidateIssuer = true,

        //! Token'ýn hedef kitlesi (audience) bilgisinin doðrulanýp doðrulanmayacaðýný belirler. Token'ýn hangi hedef kitlesi için oluþturulduðunu kontrol etmek için kullanýlýr.
        ValidateAudience = true,

        //! Token'ýn süresinin dolup dolmadýðýný kontrol eder ve geçerlilik süresi (expiration time) olmasý zorunlu hale getirilir.
        RequireExpirationTime = true,

        //! Token'ýn doðrulama anahtarýnýn doðrulanýp doðrulanmayacaðýný belirler. Token'ýn imzalanmýþ olup olmadýðýný kontrol etmek için kullanýlýr.
        ValidateIssuerSigningKey = true,

        //! Token'ýn doðrulama iþleminde kabul edilecek verici bilgisini belirtir. Bu, JWT oluþtururken belirlediðiniz verici ile eþleþmelidir.
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,

        //! Token'ýn doðrulama iþleminde kabul edilecek hedef kitle bilgisini belirtir. Bu, JWT oluþtururken belirlediðiniz hedef kitlesi ile eþleþmelidir.
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
};
});

//builder.Services.ConfigureApplicationCookie(opt =>
//{
//    opt.Cookie.Name = "JwtDeneme";
//    opt.Cookie.HttpOnly = true;
//    opt.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//    opt.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
//    opt.SlidingExpiration = true;
//});
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
