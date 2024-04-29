using Azure.AI.OpenAI;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API_GenerateConference.Models
{
    public class OpenAiFunctions
    {

        public  async Task<ActionResult<string>> GenerateTextIntro(string context)
        {
            OpenAIClient client = new(new Uri(AppSettings.Instance.IASettings.azure_endpoint), new AzureKeyCredential(AppSettings.Instance.IASettings.azure_Key));
            StringBuilder generatedText = new StringBuilder("");
            var prompt = "En tant que présentateur professionnel français , Tu es sur le point de prendre la parole lors d'une conférence devant des clients . " +
                "Notre sujet de discussion est " + context +
                " Dans cette introduction, votre objectif est de captiver immédiatement l'attention de l'audience et de leur fournir une perspective claire sur le sujet à venir. " +
                "Tu dois établir une connexion pertinente avec leur domaine d'intérêt, en mettant en évidence les points clés et les avantages qu'ils peuvent tirer de cette présentation. " +
                "Assurez-vous que l'introduction est à la fois professionnelle et engageante, de manière à ce que les clients comprennent immédiatement l'importance et la pertinence du sujet pour leur activité. ";


            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-35-turbo",
                    Temperature = 0.1f,
                    Messages = { new ChatRequestSystemMessage(prompt),},
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
                string texte = SupprimerPrefixe(generatedText.ToString());
                return texte;
            }
            catch(Exception ex) 
            {
                return ex.ToString();
            }
            
        }

        public async Task<ActionResult<string>> GenerateTextHistory(string context)
        {
            OpenAIClient client = new(new Uri(AppSettings.Instance.IASettings.azure_endpoint), new AzureKeyCredential(AppSettings.Instance.IASettings.azure_Key));
            StringBuilder generatedText = new StringBuilder("");
            var prompt = 
                "Tu vas recevoir un texte qui comporte déjà l'introduction" +
                "ta tâche consiste à générer seulement la partie sur l'histoire du sujet " +
                "l'objectif est de présenter cette section de manière à ce que les clients comprennent pleinement l'importance historique du sujet, ainsi que sa pertinence continue dans le paysage technologique actuel. " +
                "Assurez-vous d'offrir une analyse approfondie et pertinente, en captivant l'attention de votre audience dès les premiers instants de votre présentation." + 
                "voici le texte : " + context;


            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-35-turbo",
                    Temperature = 0.1f,
                    Messages = { new ChatRequestSystemMessage(prompt), },
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
                string texte = SupprimerPrefixe(generatedText.ToString());
                return texte;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public async Task<ActionResult<string>> GenerateTextEvolution(string context)
        {
            OpenAIClient client = new(new Uri(AppSettings.Instance.IASettings.azure_endpoint), new AzureKeyCredential(AppSettings.Instance.IASettings.azure_Key));
            StringBuilder generatedText = new StringBuilder("");
            var prompt =
                "tu vas recevoir une introduction ainsi que l'histoire d'un sujet, je veux que tu génére seulement la partie sur l'évolution de celui-ci " +
                "Votre objectif est de fournir une analyse détaillée et pertinente de l'état actuel du sujet, en offrant à l'audience une perspective claire sur les tendances émergentes, les innovations récentes et les défis contemporains rencontrés dans ce domaine. " +
                "voici l'introduction et l'histoire du sujet " + context;

            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-35-turbo",
                    Temperature = 0.1f,
                    Messages = { new ChatRequestSystemMessage(prompt), },
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
                string texte = SupprimerPrefixe(generatedText.ToString());
                return texte;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public async Task<ActionResult<string>> GenerateTextConclusion(string context)
        {
            OpenAIClient client = new(new Uri(AppSettings.Instance.IASettings.azure_endpoint), new AzureKeyCredential(AppSettings.Instance.IASettings.azure_Key));
            StringBuilder generatedText = new StringBuilder("");
            var prompt =
                "Tu analysera un texte  qui comporte une introduction , l'histoire de celui-ci ainsi que son évolution " +
                "Je veux que tu me génère une conclusion qui reprend ce que la conférence à présenter ainsi qu'un exemple d'un futur possible pour le sujet" + 
                "voici le texte en question : " + context;

            try
            {
                var chatCompletionsOptions = new ChatCompletionsOptions()
                {
                    DeploymentName = "gpt-35-turbo",
                    Temperature = 0.1f,
                    Messages = { new ChatRequestSystemMessage(prompt), },
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
                string texte = SupprimerPrefixe(generatedText.ToString());
                return texte;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }

        public  async Task<ActionResult<string>> GenerateUrlImage(string context)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, AppSettings.Instance.IASettings.dalle_endpoint);
                request.Headers.Add("api-key", AppSettings.Instance.IASettings.azure_Key);

                var jsonContent = new JsonDalle
                {
                    prompt = "Par  rapport à ce context : " + context + "tu vas générer une imagine profesionnelle et réaliste qui y correspond",
                    size = "1792x1024",
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



        public string SupprimerPrefixe(string texte)
        {
            const string prefixe = "ASSISTANT: ";
            if (texte.StartsWith(prefixe))
            {
                return texte.Substring(prefixe.Length);
            }
            else
            {
                return texte;
            }
        }
    }

}



