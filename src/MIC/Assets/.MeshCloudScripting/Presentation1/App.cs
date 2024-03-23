namespace CloudScripting.Sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;
    using System.Net.Http.Headers;
    using System.Threading;
    using Presentation1.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Text;

    public class AppSettings
    {
        public DeepEyesSettings DeepEyes { get; set; }
    }
    public class DeepEyesSettings
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }
    public class App : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<App> _logger;
        private readonly ICloudApplication _app;
        private readonly AppSettings _appSettings;

        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
            _appSettings = LoadSettings();
        }
        private AppSettings? LoadSettings()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<AppSettings>(json);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error loading appsettings.json: {ex.Message}");
                    return null;
                }
            }
            else
            {
                _logger.LogWarning("appsettings.json not found.");
                return null;
            }
        }
        public async Task StartAsync(CancellationToken token)
        {
            // Geestion of Deep Eyes
            var btnSphere = (TransformNode)_app.Scene.FindChildByPath("CubeOrSphere/ButtonSphere");
            var sensorSphere = btnSphere.FindFirstChild<InteractableNode>();
            var btnCube = (TransformNode)_app.Scene.FindChildByPath("CubeOrSphere/ButtonCube");
            var sensorCube = btnCube.FindFirstChild<InteractableNode>();
            sensorCube.Selected += async (sender, args) =>
            {
                await UploadImageToBlobStorage(1, _appSettings);
                btnCube.IsActive = false;
            };
            sensorSphere.Selected += async (sender, args) =>
            {
                await UploadImageToBlobStorage(2, _appSettings);
                btnSphere.IsActive = false;
            };

            // Gestion of conference 
            var layoutBtn = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/ChoiceConference");
            var btnIA = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/ChoiceConference/IA/Button");
            var sensorIA = btnIA.FindFirstChild<InteractableNode>();
            var btnVR = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/ChoiceConference/VR/Button");
            var sensorVR = btnVR.FindFirstChild<InteractableNode>();
            var btnChatBox = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/ChoiceConference/ChatBox/Button");
            var sensorChatBox = btnChatBox.FindFirstChild<InteractableNode>();
            var site = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Site");

            sensorIA.Selected += async (sender, args) =>
            {
                layoutBtn.IsActive = false;
                site.IsActive = true;
                bool reponse = await AskNewConference("IA");
                if (true)
                {
                    Task.Delay(5000);
                    site.IsActive = true;
                    Task.Delay(300000);
                    site.IsActive = false;
                    layoutBtn.IsActive = true;
                }
                else
                {
                    layoutBtn.IsActive = true;
                }
            };


            sensorVR.Selected += async (sender, args) =>
            {
                layoutBtn.IsActive = false;
                site.IsActive = true;
                bool reponse = await AskNewConference("Virtual Reality");
                if (true)
                {
                    Task.Delay(5000);
                    site.IsActive = true;
                    Task.Delay(300000);
                    site.IsActive = false;
                    layoutBtn.IsActive = true;
                }
                else
                {
                    layoutBtn.IsActive = true;
                }
            };

            sensorChatBox.Selected += async (sender, args) =>
            {
                layoutBtn.IsActive = false;
                site.IsActive = true;
                bool reponse = await AskNewConference("ChatBox IA");
                if (true)
                {
                    Task.Delay(5000);
                    site.IsActive = true;
                    Task.Delay(300000);
                    site.IsActive = false;
                    layoutBtn.IsActive = true;
                }
                else
                {
                    layoutBtn.IsActive = true;
                }
            };
        }




        

        public static async Task<bool> AskNewConference(string prompt)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7177/Conference/CreateConference");
                    request.Headers.Add("Content", "application/json");
                    var jsonContent = JsonConvert.SerializeObject(new ConferenceDataModel { Prompt = prompt });
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }












        static async Task UploadImageToBlobStorage(int choice, AppSettings appSettings)
        {
            string blobName = choice == 1 ? "cube.jpg" : "sphere.jpg";
            string imageUrl = choice == 1 ? "https://images.pexels.com/photos/1340185/pexels-photo-1340185.jpeg?cs=srgb&dl=pexels-magda-ehlers-1340185.jpg&fm=jpg" : "https://images.unsplash.com/photo-1617358142775-4be217d9cc19?q=80&w=1000&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8c3BoZXJlfGVufDB8fDB8fHww";
            HttpClient httpClient = new HttpClient();
            var imageData = await httpClient.GetByteArrayAsync(imageUrl);
            string blobUrl = $"{appSettings.DeepEyes.Url}{blobName}{appSettings.DeepEyes.Token}";
            using (MemoryStream memoryStream = new MemoryStream(imageData))
            {
                using (HttpClient client = new HttpClient())
                {
                    ByteArrayContent content = new ByteArrayContent(memoryStream.ToArray());
                    content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
                    content.Headers.Add("x-ms-blob-type", "BlockBlob");
                    HttpResponseMessage response = await client.PutAsync(blobUrl, content);
                    Console.WriteLine(response.StatusCode);
                }
            }
        }




        public Task StopAsync(CancellationToken token)
        {
            return Task.CompletedTask;
        }
      
        public async ValueTask DisposeAsync()
        {
            await StopAsync(CancellationToken.None)
                .ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}