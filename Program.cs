using System.Text.Json;
using System.Text.Json.Serialization;

// QuickSheet Budget Extension — budget envelope visualizer
// Protocol: JSON-lines on stdin/stdout

var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

// Send register
var register = new { type = "register", prefix = "budget", name = "Budget Envelope", version = "1.0.0" };
Console.WriteLine(JsonSerializer.Serialize(register, options));
Console.Out.Flush();

// Read messages
while (Console.ReadLine() is { } line)
{
    if (string.IsNullOrWhiteSpace(line)) continue;

    try
    {
        using var doc = JsonDocument.Parse(line);
        var root = doc.RootElement;
        string msgType = root.GetProperty("type").GetString() ?? "";

        if (msgType == "activate")
        {
            string id = root.GetProperty("id").GetString() ?? "";
            var parms = root.GetProperty("params");
            int gridRows = root.TryGetProperty("gridRows", out var gr) ? gr.GetInt32() : 3;

            HandleActivate(id, parms, gridRows, options);
        }
    }
    catch { /* ignore malformed messages */ }
}

static void HandleActivate(string id, JsonElement parms, int gridRows, JsonSerializerOptions options)
{
    // Parse params: category, budgetAmount, spentAmount
    // Format: budget: CategoryName, 800, 623
    string[] args = new string[parms.GetArrayLength()];
    for (int i = 0; i < args.Length; i++)
        args[i] = parms[i].GetString()?.Trim() ?? "";

    if (args.Length < 3)
    {
        SendError(id, "Usage: budget: Category, budgetAmount, spentAmount", options);
        return;
    }

    string category = args[0];
    if (!double.TryParse(args[1], out double budget) || !double.TryParse(args[2], out double spent))
    {
        SendError(id, "budget and spent must be numbers", options);
        return;
    }

    // Calculate
    double remaining = budget - spent;
    double pct = budget > 0 ? (spent / budget) * 100 : 0;
    pct = Math.Min(pct, 999); // cap display

    // Build progress bar (20 chars wide)
    int barWidth = 20;
    int filled = (int)Math.Min(Math.Round(pct / 100.0 * barWidth), barWidth);
    string bar = new string('█', filled) + new string('░', barWidth - filled);

    // Status emoji based on percentage
    string status = pct switch
    {
        <= 50 => "🟢",
        <= 75 => "🟡",
        <= 90 => "🟠",
        _ => "🔴"
    };

    // Build output cells
    var cells = new List<object>();

    // Row 0: Category name + percentage
    cells.Add(new { r = 0, c = 0, v = $"{status} {category}" });
    cells.Add(new { r = 0, c = 1, v = $"{pct:F1}%  [{bar}]" });

    // Row 1: Spent / Budget
    cells.Add(new { r = 1, c = 0, v = $"Spent: ${spent:N2}" });
    cells.Add(new { r = 1, c = 1, v = $"Budget: ${budget:N2}" });

    // Row 2: Remaining (surplus or deficit)
    string remainLabel = remaining >= 0 ? $"✅ ${remaining:N2} remaining" : $"⚠️ ${Math.Abs(remaining):N2} over budget!";
    cells.Add(new { r = 2, c = 0, v = remainLabel });

    // Send write message
    var write = new { type = "write", id, cells };
    Console.WriteLine(JsonSerializer.Serialize(write, options));
    Console.Out.Flush();
}

static void SendError(string id, string message, JsonSerializerOptions options)
{
    var error = new { type = "error", id, message };
    Console.WriteLine(JsonSerializer.Serialize(error, options));
    Console.Out.Flush();
}
