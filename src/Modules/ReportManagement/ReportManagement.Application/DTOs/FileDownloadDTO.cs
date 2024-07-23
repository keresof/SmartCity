namespace ReportManagement.Application.DTOs;
public class FileDownloadDto
    {
        public Stream FileStream { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }