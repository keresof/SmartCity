namespace UserManagement.Application.DTOs;

public class HttpContextInfo
{
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public string Referrer { get; set; }
}