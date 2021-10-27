using Application.Storage;

namespace Domain
{
    public class Photo
    {
        public string Id { get; set; }
        public string Url { get; set; }

        public Photo(PhotoUploadResult result)
        {
            Id = result.PublicId;
            Url = result.Url;
        }
    }
}