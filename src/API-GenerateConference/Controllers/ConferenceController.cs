using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Azure;
using Azure.AI.OpenAI;
using static System.Environment;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using API_GenerateConference.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System.Net;
using System.ComponentModel;
using static System.Reflection.Metadata.BlobBuilder;
using Microsoft.AspNetCore.Cors;


namespace API_GenerateConference.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConferenceController : Controller
    {
        private readonly OpenAiFunctions _openAiFunctions;
        public ConferenceController()
        {
            _openAiFunctions = new OpenAiFunctions();
        }
       
        [HttpPost("CreateConference")]
        public async Task<ActionResult> GenerateConference([FromBody] ConferenceDataModel args)
        {
            Uri blobUri = new Uri(AppSettings.Instance.IASettings.blob_conference);
            var blobServiceClient = new BlobServiceClient(blobUri);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("");
            try
            {
                var conferenceText = await _openAiFunctions.GenerateText(args.Prompt);
                Conference newConference = new Conference();
                if (conferenceText.Value != null)
                {
                    try
                    {
                        newConference.ConferenceTalk = conferenceText.Value;
                        for (int iParagraph = 1; iParagraph <= 4; iParagraph++)
                        {
                            var imageUrlResult = await _openAiFunctions.GenerateUrlImage(newConference.ConferenceTalk, iParagraph);
                            if (imageUrlResult != null && imageUrlResult.Value != null)
                            {
                                newConference.ImagesUrl.Add(imageUrlResult.Value);
                            }
                        }
                        using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(newConference))))
                        {
                            var blobName = "Conference";
                            if (await blobContainerClient.GetBlobClient(blobName).ExistsAsync())
                            {
                                await blobContainerClient.DeleteBlobIfExistsAsync(blobName);
                            }
                            await blobContainerClient.UploadBlobAsync(blobName,ms);
                        }
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else { return NotFound(); }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        
        [HttpGet("GetConference")]
        public async Task<ActionResult<JsonContent>> GetFirstBlob()
        {
            Uri blobUri = new Uri(AppSettings.Instance.IASettings.blob_conference);
            var blobServiceClient = new BlobServiceClient(blobUri);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("");

            var blob = blobContainerClient.GetBlobs().FirstOrDefault();

            if (blob != null)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                var response = await blobClient.DownloadContentAsync();
                var content = response.Value.Content.ToString();
                return Ok(content);
            }
            return NotFound();
        }

        [HttpDelete("DeleteConference")]
        public async Task<IActionResult> DeleteBlob()
        {
            Uri blobUri = new Uri(AppSettings.Instance.IASettings.blob_conference);
            var blobServiceClient = new BlobServiceClient(blobUri);
            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient("");
            var blob = blobContainerClient.GetBlobs().FirstOrDefault();
            try
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                var deleted = await blobClient.DeleteIfExistsAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
