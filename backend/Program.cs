using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader());
});

var app = builder.Build();

app.UseCors("AllowAll");

string dataDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "data");
if (!Directory.Exists(dataDir))
{
    dataDir = Path.Combine(Directory.GetCurrentDirectory(), "data");
}

string usersPath = Path.Combine(dataDir, "users.json");
string facilitiesPath = Path.Combine(dataDir, "facilities.json");
string analyticsPath = Path.Combine(dataDir, "analytics.json");

// FR1: Get users for preset selector
app.MapGet("/api/users", async () =>
{
    if (!File.Exists(usersPath)) return Results.NotFound("users.json not found");
    var json = await File.ReadAllTextAsync(usersPath);
    return Results.Content(json, "application/json");
});

// FR3: Get facilities dynamically filtered or all
app.MapGet("/api/facilities", async (string? country) =>
{
    if (!File.Exists(facilitiesPath)) return Results.NotFound("facilities.json not found");
    var json = await File.ReadAllTextAsync(facilitiesPath);
    var doc = JsonDocument.Parse(json);
    if (string.IsNullOrEmpty(country))
    {
        return Results.Content(json, "application/json");
    }
    
    var filtered = doc.RootElement.EnumerateArray()
        .Where(f => f.GetProperty("country").GetString()?.Equals(country, StringComparison.OrdinalIgnoreCase) == true)
        .ToList();
    
    return Results.Ok(filtered);
});

// FR5 & FR11: Book or release a slot
app.MapPost("/api/facilities/slot", async (SlotUpdateRequest request) =>
{
    if (!File.Exists(facilitiesPath)) return Results.NotFound("facilities.json not found");
    var json = await File.ReadAllTextAsync(facilitiesPath);
    var facilities = JsonSerializer.Deserialize<List<FacilityModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

    var targetFacility = facilities.FirstOrDefault(f => f.Id == request.FacilityId);
    if (targetFacility == null) return Results.NotFound($"Facility {request.FacilityId} not found");

    var targetResource = targetFacility.Resources.FirstOrDefault(r => r.Id == request.ResourceId);
    if (targetResource == null) return Results.NotFound($"Resource {request.ResourceId} not found");

    var targetSlot = targetResource.Slots.FirstOrDefault(s => s.Time == request.Time);
    if (targetSlot == null) return Results.NotFound($"Slot at {request.Time} not found");

    targetSlot.Status = request.Action == "book" ? "booked" : "available";
    targetSlot.BookedBy = request.Action == "book" ? request.UserId : null;

    var updatedJson = JsonSerializer.Serialize(facilities, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(facilitiesPath, updatedJson);

    return Results.Ok(new { message = $"Slot {request.Time} updated to {targetSlot.Status}", slot = targetSlot });
});

// FR10: Scaling Wizard (Add facility/region)
app.MapPost("/api/facilities", async (FacilityModel newFacility) =>
{
    if (!File.Exists(facilitiesPath)) return Results.NotFound("facilities.json not found");
    var json = await File.ReadAllTextAsync(facilitiesPath);
    var facilities = JsonSerializer.Deserialize<List<FacilityModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

    newFacility.Id = $"fac_{Guid.NewGuid().ToString("N")[..8]}";
    facilities.Add(newFacility);

    var updatedJson = JsonSerializer.Serialize(facilities, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(facilitiesPath, updatedJson);

    return Results.Created($"/api/facilities/{newFacility.Id}", newFacility);
});

// NFR4: Analytics
app.MapGet("/api/analytics", async () =>
{
    if (!File.Exists(analyticsPath)) return Results.NotFound("analytics.json not found");
    var json = await File.ReadAllTextAsync(analyticsPath);
    return Results.Content(json, "application/json");
});

app.MapGet("/health", () => Results.Ok(new { status = "online", timestamp = DateTime.UtcNow }));

// Serve frontend static files
app.UseDefaultFiles(new DefaultFilesOptions
{
    DefaultFileNames = new List<string> { "index.html" }
});
string clientPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "src");
if (!Directory.Exists(clientPath))
{
    clientPath = Path.Combine(Directory.GetCurrentDirectory(), "src");
}
if (Directory.Exists(clientPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(clientPath),
        RequestPath = ""
    });
}
if (Directory.Exists(dataDir))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(dataDir),
        RequestPath = "/data"
    });
}

// Fallback route for SPA client navigation (/workspace, /legacy-loop, /admin)
app.MapFallback(async (HttpContext context) =>
{
    string indexPath = Path.Combine(clientPath, "index.html");
    if (File.Exists(indexPath))
    {
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(indexPath);
    }
    else
    {
        context.Response.StatusCode = 404;
    }
});

app.Run("http://localhost:5005");

public record SlotUpdateRequest(string FacilityId, string ResourceId, string Time, string Action, string UserId);

public class FacilityModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Country { get; set; } = "";
    public string Region { get; set; } = "";
    public string City { get; set; } = "";
    public string Address { get; set; } = "";
    public List<ResourceModel> Resources { get; set; } = new();
}

public class ResourceModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public int Capacity { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<SlotModel> Slots { get; set; } = new();
}

public class SlotModel
{
    public string Time { get; set; } = "";
    public string Status { get; set; } = "available";
    public string? BookedBy { get; set; }
}
