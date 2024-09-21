namespace Tazkarti.Utitly
{
    public class DocumentSetting
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            string FolderName = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", folderName);
            string FileName = $"{Guid.NewGuid()}-{file.FileName}";
            string FilePath = Path.Combine(FolderName, FileName);
            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return FileName;
        }
        public static void DeleteFile(string fileName, string folderName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", folderName, fileName);
            if (File.Exists(FilePath))
                File.Delete(FilePath);
        }
    }
}
