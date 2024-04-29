using Newtonsoft.Json;

namespace API_GenerateConference.Models
{
    public class IASettings
    {
        public string azure_endpoint { get; set; }
        public string azure_Key { get; set; }
        public string dalle_endpoint { get; set; }

        public string blob_conference {  get; set; }
        public string speech_key { get; set; }

    }
}
