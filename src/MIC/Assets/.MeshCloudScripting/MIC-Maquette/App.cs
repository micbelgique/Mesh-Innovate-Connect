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
    using Presentation1.Models;
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
        private readonly PersonFlow _personFlow;
        private readonly List<QLearning> _qLearnings;
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
            //Deep Eyes
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
            // Machine Learning
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

            // Creation of new conference
            var keys = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys");
            var btnA = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/A");
            var sensorA = btnA.FindFirstChild<InteractableNode>();
            var btnB = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/B");
            var sensorB = btnB.FindFirstChild<InteractableNode>();
            var btnC = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/C");
            var sensorC = btnC.FindFirstChild<InteractableNode>();
            var btnD = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/D");
            var sensorD = btnD.FindFirstChild<InteractableNode>();
            var btnE = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/E");
            var sensorE = btnE.FindFirstChild<InteractableNode>();
            var btnF = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/F");
            var sensorF = btnF.FindFirstChild<InteractableNode>();
            var btnG = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/G");
            var sensorG = btnG.FindFirstChild<InteractableNode>();
            var btnH = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/H");
            var sensorH = btnH.FindFirstChild<InteractableNode>();
            var btnI = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/I");
            var sensorI = btnI.FindFirstChild<InteractableNode>();
            var btnJ = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/J");
            var sensorJ = btnJ.FindFirstChild<InteractableNode>();
            var btnK = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/K");
            var sensorK = btnK.FindFirstChild<InteractableNode>();
            var btnL = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/L");
            var sensorL = btnL.FindFirstChild<InteractableNode>();
            var btnM = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/M");
            var sensorM = btnM.FindFirstChild<InteractableNode>();
            var btnN = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/N");
            var sensorN = btnN.FindFirstChild<InteractableNode>();
            var btnO = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/O");
            var sensorO = btnO.FindFirstChild<InteractableNode>();
            var btnP = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/P");
            var sensorP = btnP.FindFirstChild<InteractableNode>();
            var btnQ = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Q");
            var sensorQ = btnQ.FindFirstChild<InteractableNode>();
            var btnR = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/R");
            var sensorR = btnR.FindFirstChild<InteractableNode>();
            var btnS = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/S");
            var sensorS = btnS.FindFirstChild<InteractableNode>();
            var btnT = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/T");
            var sensorT = btnT.FindFirstChild<InteractableNode>();
            var btnU = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/U");
            var sensorU = btnU.FindFirstChild<InteractableNode>();
            var btnV = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/V");
            var sensorV = btnV.FindFirstChild<InteractableNode>();
            var btnW = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/W");
            var sensorW = btnW.FindFirstChild<InteractableNode>();
            var btnX = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/X");
            var sensorX = btnX.FindFirstChild<InteractableNode>();
            var btnY = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Y");
            var sensorY = btnY.FindFirstChild<InteractableNode>();
            var btnZ = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Z");
            var sensorZ = btnZ.FindFirstChild<InteractableNode>();
            var btn0 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/0");
            var sensor0 = btn0.FindFirstChild<InteractableNode>();
            var btn1 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/1");
            var sensor1 = btn1.FindFirstChild<InteractableNode>();
            var btn2 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/2");
            var sensor2 = btn2.FindFirstChild<InteractableNode>();
            var btn3 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/3");
            var sensor3 = btn3.FindFirstChild<InteractableNode>();
            var btn4 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/4");
            var sensor4 = btn4.FindFirstChild<InteractableNode>();
            var btn5 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/5");
            var sensor5 = btn5.FindFirstChild<InteractableNode>();
            var btn6 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/6");
            var sensor6 = btn6.FindFirstChild<InteractableNode>();
            var btn7 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/7");
            var sensor7 = btn7.FindFirstChild<InteractableNode>();
            var btn8 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/8");
            var sensor8 = btn8.FindFirstChild<InteractableNode>();
            var btn9 = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/9");
            var sensor9 = btn9.FindFirstChild<InteractableNode>();
            var btnDelete = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Delete");
            var sensorDelete = btnDelete.FindFirstChild<InteractableNode>();
            var btnEnter = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Enter");
            var sensorEnter = btnEnter.FindFirstChild<InteractableNode>();
            var btnEspace = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Keys/Espace");
            var sensorEspace = btnEspace.FindFirstChild<InteractableNode>();
            var text = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/ConferenceValue");
            var valueText = text.FindFirstChild<TextNode>();
            var site = (TransformNode)_app.Scene.FindChildByPath("ConferenceRoom/Site");

            sensorA.Selected += async (sender, args) =>
            {
                valueText.Text += 'a';
            };
            sensorB.Selected += async (sender, args) =>
            {
                valueText.Text += 'b';
            };
            sensorC.Selected += async (sender, args) =>
            {
                valueText.Text += 'c';
            };
            sensorD.Selected += async (sender, args) =>
            {
                valueText.Text += 'd';
            };
            sensorE.Selected += async (sender, args) =>
            {
                valueText.Text += 'e';
            };
            sensorF.Selected += async (sender, args) =>
            {
                valueText.Text += 'f';
            };
            sensorG.Selected += async (sender, args) =>
            {
                valueText.Text += 'g';
            };
            sensorH.Selected += async (sender, args) =>
            {
                valueText.Text += 'h';
            };
            sensorI.Selected += async (sender, args) =>
            {
                valueText.Text += 'i';
            };
            sensorJ.Selected += async (sender, args) =>
            {
                valueText.Text += 'j';
            };
            sensorK.Selected += async (sender, args) =>
            {
                valueText.Text += 'k';
            };
            sensorL.Selected += async (sender, args) =>
            {
                valueText.Text += 'l';
            };
            sensorM.Selected += async (sender, args) =>
            {
                valueText.Text += 'm';
            };
            sensorN.Selected += async (sender, args) =>
            {
                valueText.Text += 'n';
            };
            sensorO.Selected += async (sender, args) =>
            {
                valueText.Text += 'o';
            };
            sensorP.Selected += async (sender, args) =>
            {
                valueText.Text += 'p';
            };
            sensorQ.Selected += async (sender, args) =>
            {
                valueText.Text += 'q';
            };
            sensorR.Selected += async (sender, args) =>
            {
                valueText.Text += 'r';
            };
            sensorS.Selected += async (sender, args) =>
            {
                valueText.Text += 's';
            };
            sensorT.Selected += async (sender, args) =>
            {
                valueText.Text += 't';
            };
            sensorU.Selected += async (sender, args) =>
            {
                valueText.Text += 'u';
            };
            sensorV.Selected += async (sender, args) =>
            {
                valueText.Text += 'v';
            };
            sensorW.Selected += async (sender, args) =>
            {
                valueText.Text += 'w';
            };
            sensorX.Selected += async (sender, args) =>
            {
                valueText.Text += 'x';
            };
            sensorY.Selected += async (sender, args) =>
            {
                valueText.Text += 'y';
            };
            sensorZ.Selected += async (sender, args) =>
            {
                valueText.Text += 'z';
            };
            sensor0.Selected += async (sender, args) =>
            {
                valueText.Text += '0';
            };
            sensor1.Selected += async (sender, args) =>
            {
                valueText.Text += '1';
            };
            sensor2.Selected += async (sender, args) =>
            {
                valueText.Text += '2';
            };
            sensor3.Selected += async (sender, args) =>
            {
                valueText.Text += '3';
            };
            sensor4.Selected += async (sender, args) =>
            {
                valueText.Text += '4';
            };
            sensor5.Selected += async (sender, args) =>
            {
                valueText.Text += '5';
            };
            sensor6.Selected += async (sender, args) =>
            {
                valueText.Text += '6';
            };
            sensor7.Selected += async (sender, args) =>
            {
                valueText.Text += '7';
            };
            sensor8.Selected += async (sender, args) =>
            {
                valueText.Text += '8';
            };
            sensor9.Selected += async (sender, args) =>
            {
                valueText.Text += '9';
            };
            sensorEspace.Selected += async (sender, args) =>
            {
                valueText.Text += ' ';
            };
            sensorDelete.Selected += async (sender, args) =>
            {
                if (!string.IsNullOrEmpty(valueText.Text))
                {
                    valueText.Text = valueText.Text.Substring(0, valueText.Text.Length - 1);
                }
            };
            sensorEnter.Selected += async (sender, args) =>
            {
                keys.IsActive = false;
                try 
                {
                    await AskNewConference(valueText.Text);
                    valueText.Text = "Votre conférence a été généré et va démarrer";
                    Task.Delay(3000);
                    site.IsActive = true;
                    // à adapter quand on pourra choisir la durée de la réunion
                    Task.Delay(300000);
                    site.IsActive = false;
                    keys.IsActive = true;
                    valueText.Text = "";
                } 
                catch(Exception e) 
                {
                    valueText.Text = "Votre conférence n'a pas été généré";
                    keys.IsActive = true;
                    Task.Delay(3000);
                    valueText.Text = "";
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
            _qLearnings[2].MoveAction(npc3, destination, simulationAction, 2, 10000, cts3.Token);
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