namespace CloudScripting.Sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;
    using System.Net.Http.Headers;
    using Presentation1;
    using System.Threading;
    using System.Collections;
    using System.Numerics;

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
        private readonly PersonFlow _personFlow;
        private readonly List<QLearning> _qLearnings;
        private CancellationTokenSource cts1 = new CancellationTokenSource();
        private CancellationTokenSource cts2 = new CancellationTokenSource();
        private CancellationTokenSource cts3 = new CancellationTokenSource();
        private CancellationTokenSource cts4 = new CancellationTokenSource();
        private CancellationTokenSource cts5 = new CancellationTokenSource();
        private Dictionary<string, Vector3> destinationsList = new Dictionary<string, Vector3>
        {
            {"Cafe", new Vector3(-2, 0.1f, 3)}, //Machine à café
            {"Innover", new Vector3(-27, 0.1f, 5)}, // Innover
            {"Loft", new Vector3()}, // Loft
        };
        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
            _appSettings = LoadSettings();
            _personFlow = new PersonFlow();
            _qLearnings = new List<QLearning>();
            for (int i = 0; i < 5; i++)
            {
                int numStates = 1800;
                _qLearnings.Add(new QLearning(numStates, 8, 0.7, 0.7, 1, 0));
            }
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

            var btnSimulationCafe = (TransformNode)_app.Scene.FindChildByPath("Simulation/ButtonCafe");
            var sensorCafe = btnSimulationCafe.FindFirstChild<InteractableNode>();
            var btnSImulationInnover = (TransformNode)_app.Scene.FindChildByPath("Simulation/ButtonInnover");
            var sensorInnover = btnSImulationInnover.FindFirstChild<InteractableNode>();
            sensorCafe.Selected += async (sender, args) =>
            {
                await StartSimulation("Cafe");
            };
            sensorInnover.Selected += async (sender, args) =>
            {
                await StartSimulation("Innover");
            };
        }

        public async Task StartSimulation(string simulationAction) //0 to go to the coffee 
        {
            Vector3 destination = destinationsList[simulationAction];
            var npc1 = (TransformNode)_app.Scene.FindChildByPath("HumanMale_Character");
            var npc2 = (TransformNode)_app.Scene.FindChildByPath("HumanMale_Character1");
            var npc3 = (TransformNode)_app.Scene.FindChildByPath("HumanMale_Character2");
            var npc4 = (TransformNode)_app.Scene.FindChildByPath("HumanMale_Character3");
            var npc5 = (TransformNode)_app.Scene.FindChildByPath("HumanMale_Character4");

            cts1.Cancel();
            cts2.Cancel();
            cts3.Cancel();
            cts4.Cancel();
            cts5.Cancel();

            cts1 = new CancellationTokenSource();
            cts2 = new CancellationTokenSource();
            cts3 = new CancellationTokenSource();
            cts4 = new CancellationTokenSource();
            cts5 = new CancellationTokenSource();

            _qLearnings[0].MoveAction(npc1, destination, simulationAction, 0, 10000, cts1.Token);
            await Task.Delay(50);
            _qLearnings[1].MoveAction(npc2, destination, simulationAction, 1, 10000, cts2.Token);
            await Task.Delay(50);
            _qLearnings[2].MoveAction(npc3, destination, simulationAction, 2,  10000, cts3.Token);
            await Task.Delay(50);
            _qLearnings[3].MoveAction(npc4, destination, simulationAction, 3, 10000, cts4.Token);
            await Task.Delay(50);
            _qLearnings[4].MoveAction(npc5, destination, simulationAction, 4, 10000, cts5.Token);
        }

        public async Task<string> GetImage(TransformNode node, string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                var webview = node.FindFirstChild<WebSlateNode>(true);
                Uri imageUri = new Uri(imageUrl);
                webview.Url = imageUri;
                return imageUrl;
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
        /// <inheritdoc/>
        public Task StopAsync(CancellationToken token)
        {
            // Custom logic could be added here for user apps
            return Task.CompletedTask;
        }
        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await StopAsync(CancellationToken.None)
                .ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
    }
}