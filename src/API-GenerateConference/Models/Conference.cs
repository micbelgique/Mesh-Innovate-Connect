namespace API_GenerateConference.Models
{
    public class Conference
    {
        public string ConferenceTalk;
        public List<string> ImagesUrls;

        public Conference()
        {
            ImagesUrls = new List<string>();
        }


    }
}
