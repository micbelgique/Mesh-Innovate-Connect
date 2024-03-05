namespace CloudScripting.Sample
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Web;
    using System.IO;
    using System.Numerics;
    using System.Linq;
    using System.Xml.Linq;
    using System.Reflection;
    using System.Net.Http.Headers;
    using Presentation1;
    using System.Security.Cryptography.X509Certificates;
    using MeshApp.Animations;
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
        private readonly List<Avatar> _playersEscape;
        private readonly TeamEscape redTeam;
        private readonly TeamEscape greenTeam;
        private int playerNumber;
        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
            _appSettings = LoadSettings();
            _playersEscape = new List<Avatar>();
            redTeam = new TeamEscape("Red");
            greenTeam = new TeamEscape("Green");
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
            var image = (TransformNode)_app.Scene.FindChildByPath("MultipleImport/WebSlate");
            var salameche = (TransformNode)_app.Scene.FindChildByPath("MultipleImport/ButtonSalameche/Sphere");
            var buttonSalameche = salameche.FindFirstChild<InteractableNode>();
            buttonSalameche.Selected += async (_, args) =>
            {
                await GetImage(image, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/4.png");
            };
            var bulbizare = (TransformNode)_app.Scene.FindChildByPath("MultipleImport/ButtonBulbizare/Sphere");
            var buttonBulbizare = bulbizare.FindFirstChild<InteractableNode>();
            buttonBulbizare.Selected += async (_, args) =>
            {
                await GetImage(image, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/1.png");
            };
            var carapuce = (TransformNode)_app.Scene.FindChildByPath("MultipleImport/ButtonCarapuce/Sphere");
            var buttonCarapuce = carapuce.FindFirstChild<InteractableNode>();
            buttonCarapuce.Selected += async (_, args) =>
            {
                await GetImage(image, "https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork/7.png");
            };
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
            var transformTriggerZone = (TransformNode)_app.Scene.FindChildByPath("TriggerEscapeZone");
            var triggerZone = (BoxGeometryNode)transformTriggerZone.FindFirstChild<BoxGeometryNode>();
            triggerZone.Entered += (sender, args) =>
            {
                countPlayer(args.Avatar, true);
                if (playerNumber >= 2)
                {
                    TeamEscape.InitTeams(redTeam, greenTeam, _playersEscape);
                    UpdateLabelTeams();
                }
            };
            triggerZone.Exited += async (sender, args) =>
            {
                countPlayer(args.Avatar, false);
            };
        }
        private void UpdateLabelTeams()
        {
            var transformGreen = (TransformNode)_app.Scene.FindChildByPath("TriggerEscapeZone/TeamBoard/Tags/TeamGreen/Members");
            var membersGreenTeam = transformGreen.FindFirstChild<TextNode>();
            var transformRed = (TransformNode)_app.Scene.FindChildByPath("TriggerEscapeZone/TeamBoard/Tags/TeamRed/Members");
            var membersRedTeam = transformRed.FindFirstChild<TextNode>();
            membersGreenTeam.Text = "";
            membersRedTeam.Text = "";
            var spawnEquipe = (TransformNode)_app.Scene.FindChildByPath("TriggerEscapeZone/TravelPointGroup/spawnEquipe");
            var spawnEquipeNode = (TravelPointNode)spawnEquipe.FindFirstChild<TravelPointNode>();

            foreach (Avatar player in greenTeam.Participants)
            {
                membersGreenTeam.Text = membersGreenTeam.Text + player.Participant.DisplayName + '\n';
                player.TravelTo(spawnEquipeNode);
            }

            foreach (Avatar player in redTeam.Participants)
            {
                membersRedTeam.Text = membersGreenTeam.Text + player.Participant.DisplayName + '\n';
                player.TravelTo(spawnEquipeNode);
            }
        }

        private void countPlayer(Avatar player, bool enter)
        {
            var playerNumberDisplay = (TransformNode)_app.Scene.FindChildByPath("TriggerEscapeZone/TeamBoard/Tags/NumberTag");
            var playerNumberText = playerNumberDisplay.FindFirstChild<TextNode>();
            if (enter)
            {
                playerNumber++;
                _playersEscape.Add(player);
                playerNumberText.Text = playerNumber + "/10";
            }
            else
            {
                playerNumber--;
                _playersEscape.Remove(player);
                playerNumberText.Text = playerNumber + "/10";
            }
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