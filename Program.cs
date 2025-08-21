using AchillesChat.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC (controllers + views)
builder.Services.AddControllersWithViews();

// Register services
builder.Services.AddSingleton<KnowledgeBaseService>();
builder.Services.AddSingleton<MLSimilarityService>();   // ⬅️ use ML.NET similarity
builder.Services.AddSingleton<CharacterReplyService>();
builder.Services.AddSingleton<AIChatService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.Run();