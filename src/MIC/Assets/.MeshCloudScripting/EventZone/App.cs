namespace CloudScripting.Sample
{
    using EventZone;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Mesh.CloudScripting;
    using Newtonsoft.Json;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text.Json.Nodes;
    using static System.Net.Mime.MediaTypeNames;
   

    public class App : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<App> _logger;
        private readonly ICloudApplication _app;



        public App(ICloudApplication app, ILogger<App> logger)
        {
            _app = app;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken token)
        {
            // Cube
            var transform = (TransformNode)_app.Scene.FindChildByPath("Cube");
            var sensor = transform.FindFirstChild<InteractableNode>();

            // Text
            var label = (TransformNode)_app.Scene.FindChildByPath("Text");
            var text = label.FindFirstChild<TextNode>();

            // Handle a button click    
            sensor.Selected +=  (_, _) =>
            {
                //await GetImage();
                 UploadImageToBlobStorage();
            };
        }


        private void Sensor_Selected(object? sender, InteractableSelectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /*
        private async Task GetImage()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.artic.edu/api/v1/artworks/129884");
            var response = await client.SendAsync(request); // Attendre la complétion de la tâche
            response.EnsureSuccessStatusCode(); // S'assurer que la requête a réussi
            var responseContent = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<Thumbnail>(responseContent);
            byte[] imageBytes = Convert.FromBase64String(data.imageBase64);
            File.WriteAllBytes("image.jpg", imageBytes);
        }
        */

        private void UploadImageToBlobStorage()
        {    
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Put, $"https://deepeyes0822sa.blob.core.windows.net/raw-pics/test?sp=rw&st=2024-02-16T10:16:33Z&se=2024-04-19T17:16:33Z&sv=2022-11-02&sr=c&sig=hFcbfxXecvFK7RKZ7%2ForDtaXq5YN6ikQatsFrhueix4%3D");
                request.Headers.Add("x-ms-blob-type", "BlockBlob");
                request.Content = new StreamContent(File.OpenRead("rat.jpg"));
                var response =  client.Send(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une exception s'est produite : {ex.Message}");
            }
        }



        private async Task<string> GetTemperature()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://agromet.be/fr/agromet/api/v3/get_pameseb_hourly/tsa,plu,hra/1,26/2018-09-01/2018-09-05/");
            request.Headers.Add("917e4abf34505f810a7d6ef2057571e914a0c67a", "");
            request.Headers.Add("Cookie", "django_language=fr");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
       
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tempData = JsonConvert.DeserializeObject<ObjJson>(responseContent);
                return tempData.Frequency; 
                 
            }

            return "Erreur";
        }




        public Task StopAsync(CancellationToken token)
        {
            // Custom logic could be added here for user apps
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

