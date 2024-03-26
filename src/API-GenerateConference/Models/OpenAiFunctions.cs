using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace API_GenerateConference.Models
{
    public class OpenAiFunctions
    {

        public  async Task<ActionResult<string>> GenerateText(string context)
        {
            OpenAIClient client = new(new Uri(AppSettings.Instance.IASettings.azure_endpoint), new AzureKeyCredential(AppSettings.Instance.IASettings.azure_Key));
            StringBuilder generatedText = new StringBuilder("");
            var prompt = "You are tasked with delivering a french presentation about "
                + context +
                " you'll need a well-defined structure with 4 paragraphs. " +
                "first paragraph Introduce the context, " +
                "second paragraph : talk about its history, " +
                "third paragraph : explain what can be done with the context today, " +
                "fourth paragraph : then draw up a conclusion that covers all the points of the conference.";

            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-35-turbo",
                    Temperature = 0.4f,
                    Messages = { new ChatRequestSystemMessage(prompt), }
                };

                await foreach (StreamingChatCompletionsUpdate chatUpdate in client.GetChatCompletionsStreaming(chatCompletionsOptions))
                {
                    if (chatUpdate.Role.HasValue)
                    {
                        generatedText.Append($"{chatUpdate.Role.Value.ToString().ToUpperInvariant()}: ");
                    }
                    if (!string.IsNullOrEmpty(chatUpdate.ContentUpdate))
                    {
                        generatedText.Append(chatUpdate.ContentUpdate);
                    }
                }
                return generatedText.ToString();
            }
            catch
            {
                return null;
            }
        }


        public  async Task<ActionResult<string>> GenerateUrlImage(string context, int whichParagraph)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettings.Instance.IASettings.dalle_endpoint);
                request.Headers.Add("api-key", AppSettings.Instance.IASettings.azure_Key);

                var jsonContent = new JsonDalle
                {
                    prompt = $" in the following context {context} generates a photo in relation to the {whichParagraph} paragraph ",
                    size = "1024x1024",
                    n = 1,
                    quality = "hd",
                    style = "vivid"
                };

                var serializedContent = JsonConvert.SerializeObject(jsonContent);
                var content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
                request.Content = content;

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseData = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(responseData);
                var imageUrl = jsonObject["data"][0]["url"].ToString();

                return imageUrl;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
