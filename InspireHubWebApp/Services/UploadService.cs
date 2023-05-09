using InspireHubWebApp.Interfaces;

namespace InspireHubWebApp.Services
{
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment _env;

        public UploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> Upload(IFormFile file, string DirectoryOne, string Id)
        {
            var filePath = Path.Combine(_env.WebRootPath, "Uploads", DirectoryOne, Id);

            Directory.CreateDirectory(filePath);

            var fullPath = Path.Combine(filePath, file.FileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = "/Uploads/" + DirectoryOne + "/" + Id + "/" + file.FileName;

            return result;
        }

        public async Task<string> UploadSlider(IFormFile file, string DirectoryOne)
        {
            var filePath = Path.Combine(_env.WebRootPath, "Uploads", DirectoryOne);

            Directory.CreateDirectory(filePath);

            var fullPath = Path.Combine(filePath, file.FileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var result = "/Uploads/" + DirectoryOne + "/" + file.FileName;

            return result;
        }

        public void Remove(string Directory)
        {
            var path = _env.WebRootPath + Directory.Replace("/", @"\");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
