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
    using System.Collections;
    using System.Numerics;
    using System.Text;

    public class AppSettings
    {
        public BlobStorageSettings BlobStorage { get; set; }
    }
    public class BlobStorageSettings
    {
        public string BlobUrl { get; set; }
    }

    public class App : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<App> _logger;
        private readonly ICloudApplication _app;
        private readonly AppSettings _appSettings;
        private CancellationTokenSource cts1 = new CancellationTokenSource();
        private CancellationTokenSource cts2 = new CancellationTokenSource();
        private CancellationTokenSource cts3 = new CancellationTokenSource();
        private CancellationTokenSource cts4 = new CancellationTokenSource();
        private CancellationTokenSource cts5 = new CancellationTokenSource();
        private Dictionary<string, Vector3> destinationsList = new Dictionary<string, Vector3>
        {
            {"Cafe", new Vector3(-2, 0.1f, 3)}, //Machine � caf�
            {"Innover", new Vector3(-27, 0.1f, 5)}, // Innover
            {"Loft", new Vector3()}, // Loft
        };
        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
            _appSettings = LoadSettings();
            //_personFlow = new PersonFlow();
            //_qLearnings = new List<QLearning>();
            for (int i = 0; i < 5; i++)
            {
                int numStates = 1800;
                //_qLearnings.Add(new QLearning(numStates, 8, 0.7, 0.7, 1, 0));
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
            //Deep Eyes
            //var btnSphere = (TransformNode)_app.Scene.FindChildByPath("CubeOrSphere/ButtonSphere");
            //var sensorSphere = btnSphere.FindFirstChild<InteractableNode>();
            //var btnCube = (TransformNode)_app.Scene.FindChildByPath("CubeOrSphere/ButtonCube");
            //var sensorCube = btnCube.FindFirstChild<InteractableNode>();
            //sensorCube.Selected += async (sender, args) =>
            //{
            //    await UploadImageToBlobStorage(1, _appSettings);
            //    btnCube.IsActive = false;
            //};
            //sensorSphere.Selected += async (sender, args) =>
            //{
            //    await UploadImageToBlobStorage(2, _appSettings);
            //    btnSphere.IsActive = false;
            //};
            // Machine Learning



            List<DescriptionLogitech> descriptions = new List<DescriptionLogitech>();

            descriptions = await GetLogitechDescriptions(new Uri(_appSettings.BlobStorage.BlobUrl));

            if (descriptions != null)
            {
                // Site Logitech
                var siteCockpit = (TransformNode)_app.Scene.FindChildByPath("Logitech/SiteCockpit");
                var screenCockpit = (TransformNode)siteCockpit.FindChildByPath("WebSlate");
                var webCockpit = screenCockpit.FindFirstChild<WebSlateNode>();

                var siteScheduler1 = (TransformNode)_app.Scene.FindChildByPath("Logitech/SiteSchedulerCockpit");
                var screenScheduler = (TransformNode)siteCockpit.FindChildByPath("WebSlate");
                var webScheduler1 = screenScheduler.FindFirstChild<WebSlateNode>();

                var siteScheduler2 = (TransformNode)_app.Scene.FindChildByPath("Logitech/SiteStudio");
                var screenScheduler2 = (TransformNode)siteCockpit.FindChildByPath("WebSlate");
                var webScheduler2 = screenScheduler2.FindFirstChild<WebSlateNode>();


                // Logitech
                var btnLogiSight = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracSight");
                var sensorLogiSight = btnLogiSight.FindFirstChild<InteractableNode>();

                var btnTapIp = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracTapIP");
                var sensorTapIp = btnTapIp.FindFirstChild<InteractableNode>();

                var btnScribe = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracScribe");
                var sensorScribe = btnScribe.FindFirstChild<InteractableNode>();

                var btnRallyBar = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracRallyBar");
                var sensorRallyBar = btnRallyBar.FindFirstChild<InteractableNode>();

                var btnScheduler1 = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracScheduler1");
                var sensorScheduler1 = btnScheduler1.FindFirstChild<InteractableNode>();

                var btnScheduler2 = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracScheduler2");
                var sensorScheduler2 = btnScheduler2.FindFirstChild<InteractableNode>();

                var btnScheduler3 = (TransformNode)_app.Scene.FindChildByPath("Logitech/InteracScheduler3");
                var sensorScheduler3 = btnScheduler3.FindFirstChild<InteractableNode>();



                sensorLogiSight.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Sight");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteCockpit.IsActive)
                    {
                        siteCockpit.IsActive = false;
                        Task.Delay(3000);
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                    else
                    {
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                };

                sensorTapIp.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "TapIP");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteCockpit.IsActive)
                    {
                        siteCockpit.IsActive = false;
                        Task.Delay(3000);
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                    else
                    {
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                };

                sensorScribe.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Scribe");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteCockpit.IsActive)
                    {
                        siteCockpit.IsActive = false;
                        Task.Delay(3000);
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                    else
                    {
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                };

                sensorRallyBar.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Rally Bar");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteCockpit.IsActive)
                    {
                        siteCockpit.IsActive = false;
                        Task.Delay(3000);
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                    else
                    {
                        webCockpit.Url = newUrl;
                        siteCockpit.IsActive = true;
                    }
                };

                sensorScheduler1.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Scheduler");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteScheduler1.IsActive)
                    {
                        siteScheduler1.IsActive = false;
                        Task.Delay(3000);
                        webScheduler1.Url = newUrl;
                        siteScheduler1.IsActive = true;
                    }
                    else
                    {
                        webScheduler1.Url = newUrl;
                        siteScheduler1.IsActive = true;
                    }
                };

                sensorScheduler2.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Scheduler");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteScheduler1.IsActive)
                    {
                        siteScheduler1.IsActive = false;
                        Task.Delay(3000);
                        webScheduler1.Url = newUrl;
                        siteScheduler1.IsActive = true;
                    }
                    else
                    {
                        webScheduler1.Url = newUrl;
                        siteScheduler1.IsActive = true;
                    }
                };

                sensorScheduler3.Selected += async (sender, args) =>
                {
                    DescriptionLogitech descriptionTrouvee = descriptions.FirstOrDefault(desc => desc.Nom == "Scheduler");
                    Uri newUrl = new Uri(descriptionTrouvee.Lien);
                    if (siteScheduler2.IsActive)
                    {
                        siteScheduler2.IsActive = false;
                        Task.Delay(3000);
                        webScheduler2.Url = newUrl;
                        siteScheduler2.IsActive = true;
                    }
                    else
                    {
                        webScheduler2.Url = newUrl;
                        siteScheduler2.IsActive = true;
                    }
                };





            }
        }


        static async Task<List<DescriptionLogitech>> GetLogitechDescriptions()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "https://stma86a4449dc84f4d2bwe.blob.core.windows.net/logitech-descriptions/logitechDescriptions.json?sp=r&st=2024-04-26T09:20:01Z&se=2024-04-26T17:20:01Z&spr=https&sv=2022-11-02&sr=c&sig=Udz0t8FcEqd6SR4pXdrjfvoI51U2sdRFvPU627Hq32o%3D");
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();


                        List<DescriptionLogitech> descriptions = JsonConvert.DeserializeObject<List<DescriptionLogitech>>(jsonString);

                        return descriptions;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    // Gérer les exceptions
                    Console.WriteLine($"Une erreur s'est produite lors de la récupération du contenu du blob : {ex.Message}");
                    return null;
                }
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

        static async Task<List<DescriptionLogitech>> GetLogitechDescriptions(Uri blobUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, blobUrl);
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = await response.Content.ReadAsStringAsync();
                        List<DescriptionLogitech> descriptions = JsonConvert.DeserializeObject<List<DescriptionLogitech>>(jsonString);
                        return descriptions;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    // Gérer les exceptions
                    Console.WriteLine($"Une erreur s'est produite lors de la récupération du contenu du blob : {ex.Message}");
                    return null;
                }
            }
        }

        //static async Task UploadImageToBlobStorage(int choice, AppSettings appSettings)
        //{
        //    string blobName = choice == 1 ? "cube.jpg" : "sphere.jpg";
        //    string imageUrl = choice == 1 ? "https://images.pexels.com/photos/1340185/pexels-photo-1340185.jpeg?cs=srgb&dl=pexels-magda-ehlers-1340185.jpg&fm=jpg" : "https://images.unsplash.com/photo-1617358142775-4be217d9cc19?q=80&w=1000&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxzZWFyY2h8Mnx8c3BoZXJlfGVufDB8fDB8fHww";
        //    HttpClient httpClient = new HttpClient();
        //    var imageData = await httpClient.GetByteArrayAsync(imageUrl);
        //    string blobUrl = $"{appSettings.DeepEyes.Url}{blobName}{appSettings.DeepEyes.Token}";
        //    using (MemoryStream memoryStream = new MemoryStream(imageData))
        //    {
        //        using (HttpClient client = new HttpClient())
        //        {
        //            ByteArrayContent content = new ByteArrayContent(memoryStream.ToArray());
        //            content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
        //            content.Headers.Add("x-ms-blob-type", "BlockBlob");
        //            HttpResponseMessage response = await client.PutAsync(blobUrl, content);
        //            Console.WriteLine(response.StatusCode);
        //        }
        //    }
        //}


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