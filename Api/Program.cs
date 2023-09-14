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
        //! Token'da akt�r (actor) bilgisinin do�rulan�p do�rulanmayaca��n� belirler. Genellikle false olarak ayarlan�r, ��nk� bu bilgi genellikle kullan�lmaz.
        ValidateActor = true,

        //! Token'�n verici (issuer) bilgisinin do�rulan�p do�rulanmayaca��n� belirler. Token'�n hangi servis taraf�ndan olu�turuldu�unu kontrol etmek i�in kullan�l�r.
        ValidateIssuer = true,

        //! Token'�n hedef kitlesi (audience) bilgisinin do�rulan�p do�rulanmayaca��n� belirler. Token'�n hangi hedef kitlesi i�in olu�turuldu�unu kontrol etmek i�in kullan�l�r.
        ValidateAudience = true,

        //! Token'�n s�resinin dolup dolmad���n� kontrol eder ve ge�erlilik s�resi (expiration time) olmas� zorunlu hale getirilir.
        RequireExpirationTime = true,

        //! Token'�n do�rulama anahtar�n�n do�rulan�p do�rulanmayaca��n� belirler. Token'�n imzalanm�� olup olmad���n� kontrol etmek i�in kullan�l�r.
        ValidateIssuerSigningKey = true,

        //! Token'�n do�rulama i�leminde kabul edilecek verici bilgisini belirtir. Bu, JWT olu�tururken belirledi�iniz verici ile e�le�melidir.
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,

        //! Token'�n do�rulama i�leminde kabul edilecek hedef kitle bilgisini belirtir. Bu, JWT olu�tururken belirledi�iniz hedef kitlesi ile e�le�melidir.
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
