
using Data_Layer.Interfaces;
using Data_Layer.Data;
using Business_Layer.Interfaces;
using Business_Layer.Business;
using Presentation_Layer.Filters;
using Presentation_Layer.Middlewares;
using Options;


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Presentation_Layer.Authentication;
using Presentation_Layer.Authorization;
using Business_Layer.Timer;
using Business_Layer.SearchTries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionBaseAuthorizationFilter>();
});

//builder.Services.AddAntiforgery(options =>
//  options.HeaderName = "CSRF-TOKEN"
//);

builder.Services.AddOpenApi();


var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtOption = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
var paypalOptions = builder.Configuration.GetSection("PayPalKeys").Get<PaypalOptions>();
var paypalUrls = builder.Configuration.GetSection("Urls").Get<PaypalUrls>();
var storeUrls = builder.Configuration.GetSection("StoreUrls").Get<StoreUrls>();
var inventoryOptions = builder.Configuration.GetSection("Ecommerce_Inventory_Shared_Key").Get<InventoryOptions>();
var productTrie = new ProductTrie();



builder.Services.AddSingleton<string>(connectionString);
builder.Services.AddSingleton<JwtOptions>(jwtOption);
builder.Services.AddSingleton<PaypalOptions>(paypalOptions);
builder.Services.AddSingleton<PaypalUrls>(paypalUrls);
builder.Services.AddSingleton<StoreUrls>(storeUrls);
builder.Services.AddSingleton<InventoryOptions>(inventoryOptions);
builder.Services.AddSingleton<ProductTrie>(productTrie);




builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOption.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOption.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.SigningKey))
        };
    });



builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IImagesBusiness, ImagesBusiness>();
builder.Services.AddScoped<IUsersBusiness, UsersBusiness>();
builder.Services.AddScoped<ICartItemBusiness, CartItemBusiness>();
builder.Services.AddScoped<ICartsBusiness, CartsBusiness>();
builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
builder.Services.AddScoped<IProductsBusiness, ProductsBusinees>();
builder.Services.AddScoped<IPayPalBusiness, PayPalBusiness>();
builder.Services.AddScoped<IPromoCodeBusiness, PromoCodeBusiness>();
builder.Services.AddScoped<ISalesBusiness, SalesBusiness>();
builder.Services.AddScoped<IOrdersBusiness, OrdersBusiness>();
builder.Services.AddScoped<IEmailBusiness, EmailBusiness>();
builder.Services.AddScoped<ISellerBusiness, SellerBusiness>();
builder.Services.AddScoped<IAuthorizeBusiness, AuthorizeBusiness>();
builder.Services.AddScoped<IInventoryKeyGenerator, InventoryKeyGenerator>();
builder.Services.AddSingleton<IFileSystem, FileSystem>();

builder.Services.AddScoped<IUsersData, UsersData>();
builder.Services.AddScoped<ICartItemData, CartItemsData>();
builder.Services.AddScoped<ICartsData, CartsData>();
builder.Services.AddScoped<ICategoryData, CategoryData>();
builder.Services.AddScoped<IProductsData, ProductData>();
builder.Services.AddScoped<IPayPalData, PayPalData>();
builder.Services.AddScoped<IPromoCodeData, PromoCodeData>();
builder.Services.AddScoped<ISalesData, SalesData>();
builder.Services.AddScoped<IOrdersData, OrderData>();
builder.Services.AddScoped<ISellerData, SellerData>();
builder.Services.AddScoped<IEmailData, EmailData>();
builder.Services.AddScoped<IAuthorizeData, AuthorizeData>();

builder.Services.AddScoped<AuthenticateHelper>();
builder.Services.AddScoped<AuthorizeHelper>();


builder.Services.AddSingleton<TimerService>(provider =>
{
    var productsBusiness = provider.GetRequiredService<IProductsBusiness>();
    var productTrie = provider.GetRequiredService<ProductTrie>();
    return new TimerService(productsBusiness, productTrie);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MorfaCors", policy =>
        policy.WithOrigins("https://morfa-shop.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/ProductImage")),
    RequestPath = "/Images/ProductImage"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images/CategoryImage")),
    RequestPath = "/Images/CategoryImage"
});
app.UseHttpsRedirection();


//app.UseAntiforgery();

app.UseMiddleware<ContentSecurityPolicyMiddleware>();
app.UseCors("MorfaCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();