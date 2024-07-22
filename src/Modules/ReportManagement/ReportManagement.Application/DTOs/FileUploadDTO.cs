
using Microsoft.AspNetCore.Http;
public class FileUploadDto
{
    public string FileName { get; set; }
    public IFormFile File { get; set; }
}