using AutoMapper;
using Microsoft.Extensions.Options;
using nagiashraf.CoursesApp.Services.Catalog.API.DTOs;
using nagiashraf.CoursesApp.Services.Catalog.Core.Entities;

namespace nagiashraf.CoursesApp.Services.Catalog.API.Helpers.AutoMapper;

public class FileUrlResolver : IValueResolver<DownloadableFile, DownloadableFileDto, string?>
{
    private readonly ApiUrl _apiUrl;
    public FileUrlResolver(IOptions<ApiUrl> apiUrl)
    {
        _apiUrl = apiUrl.Value;
    }

    public string Resolve(DownloadableFile source, DownloadableFileDto destination, string? destMember, ResolutionContext context)
    {
        return _apiUrl.BaseUrl + source.FileUrlPath;
    }
}
