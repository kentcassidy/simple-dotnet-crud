using IntroSite.Components;
using Microsoft.EntityFrameworkCore;
using IntroSite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<AppDb>(o => o.UseSqlite("Data Source=tracker.db"));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

using (var db = app.Services.GetRequiredService<IDbContextFactory<AppDb>>().CreateDbContext())
    db.Database.EnsureCreated();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

var api = app.MapGroup("/api/testpoints");

api.MapGet("/", (AppDb db) => db.TestPoints.ToListAsync());

api.MapGet("/{id:int}", async (int id, AppDb db) =>
    await db.TestPoints.FindAsync(id) is { } tp ? Results.Ok(tp) : Results.NotFound());

api.MapPost("/", async (TestPoint tp, AppDb db) =>
{
    db.TestPoints.Add(tp);
    await db.SaveChangesAsync();
    return Results.Created($"/api/testpoints/{tp.Id}", tp);
});

api.MapPut("/{id:int}", async (int id, TestPoint input, AppDb db) =>
{
    var tp = await db.TestPoints.FindAsync(id);
    if (tp == null) return Results.NotFound();

    tp.Name = input.Name;
    tp.Status = input.Status;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

api.MapDelete("/{id:int}", async (int id, AppDb db) =>
{
    var tp = await db.TestPoints.FindAsync(id);
    if (tp == null) return Results.NotFound();
    db.TestPoints.Remove(tp);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();