using API_GenerateConference.Models;
using Newtonsoft.Json;

public class AppSettings
{
    private static readonly Lazy<AppSettings> instance = new Lazy<AppSettings>(() => LoadSettings());

    public static AppSettings Instance => instance.Value;

    public IASettings IASettings { get; set; }

    private AppSettings() { }

    private static AppSettings LoadSettings()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        else
        {
            Console.WriteLine("appsettings.json not found.");
        }

        return new AppSettings();
    }
}
