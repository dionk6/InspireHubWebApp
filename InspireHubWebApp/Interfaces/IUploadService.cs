namespace InspireHubWebApp.Interfaces
{
    public interface IUploadService
    {
        Task<string> Upload(IFormFile file, string Directory, string Id);
        Task<string> UploadSlider(IFormFile file, string Directory);
        void Remove(string Directory);
    }
}
