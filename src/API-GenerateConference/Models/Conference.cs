namespace API_GenerateConference.Models
{
    public class Conference
    {
        public string ConferenceTalk;
        public List<string> ImagesUrl;

        public Conference()
        {
            ImagesUrl = new List<string>();
        }


    }
}
