using Sefer.Backend.Api.Models.Admin.File;
using Sefer.Backend.Api.Services.FileStorage.AbstractStorage;
using Sefer.Backend.Api.Views.Admin.Files;
using Shared_FileView = Sefer.Backend.Api.Views.Shared.FileView;

namespace Sefer.Backend.Api.Controllers.Admin;

public class FileController(IServiceProvider serviceProvider) : BaseController(serviceProvider)
{
    private readonly IFileStorageService _fileStorageService = serviceProvider.GetService<IFileStorageService>();

    private readonly IUserAuthenticationService _authenticationService = serviceProvider.GetService<IUserAuthenticationService>();

    [HttpPost("/content/directories")]
    [ProducesResponseType(typeof(DirectoryView), 201)]
    [Authorize(Roles = "Admin,CourseMaker")]
    public async Task<IActionResult> AddDirectory([FromBody] DirectoryPostModel directory)
    {
        // First check the right information is supplied
        if (directory == null) return BadRequest();
        if (string.IsNullOrEmpty(directory.Path) || string.IsNullOrEmpty(directory.Name)) return BadRequest();
        var result = SharedStorageService.CleanUrl(directory.Path);
        if (string.IsNullOrEmpty(result) || string.IsNullOrEmpty(directory.Name)) return BadRequest();

        // check if the directory could be resolved
        var parent = await _fileStorageService.ResolveDirectoryAsync(directory.Path);
        if (parent == null) return NotFound();

        // Check if a child already exists
        var child = await parent.FindDirectoryAsync(SharedStorageService.CleanUrl(directory.Name));
        if (child != null) return NoContent();

        // Now add the directory
        var created = await parent.AddDirectoryAsync(SharedStorageService.CleanUrl(directory.Name));
        var view = await DirectoryView.CreateViewAsync(created, false, false);
        return Json(view, 201);
    }

    [HttpGet("/content/directories/{*path}")]
    [ProducesResponseType(typeof(DirectoryView), 200)]
    [Authorize(Roles = "Admin,CourseMaker")]
    public async Task<IActionResult> GetDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var directory = await _fileStorageService.ResolveDirectoryAsync(path);
        if (directory == null) return NotFound();
        var view = await DirectoryView.CreateViewAsync(directory, false, true);
        return Json(view);
    }

    [HttpDelete("/content/directories/{*path}")]
    [ProducesResponseType(204)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var directory = await _fileStorageService.ResolveDirectoryAsync(path);
        if (directory == null) return NotFound();
        await directory.DeleteAsync();
        return NoContent();
    }

    [HttpPost("/content/files")]
    [ProducesResponseType(201)]
    [RequestSizeLimit(2147483647)]
    public async Task<IActionResult> UploadCourseImage(IFormFile file, string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var directory = await _fileStorageService.ResolveDirectoryAsync(path);
        if (directory == null) return BadRequest();
        var uploadedFile = await directory.AppendAsync(file);

        if (uploadedFile == null) return BadRequest();
        return Json(new Shared_FileView(uploadedFile), 201);
    }

    [HttpGet("/content/files/{*path}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var file = await _fileStorageService.ResolveFileAsync(path);
        if (file == null) return NotFound();

        if (file.IsPublic == false && _authenticationService.IsAuthenticated == false) return Forbid();

        var response = File(file.GetReadStream(), file.ContentType);
        return response;
    }

    [HttpGet("/content/download/{*path}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> DownloadFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var file = await _fileStorageService.ResolveFileAsync(path);
        if (file == null) return NotFound();

        if (file.IsPublic == false && _authenticationService.IsFileAuthenticated() == false) return Forbid();

        // Response...
        return file.DownloadActionResult;
    }

    [HttpDelete("/content/files/{*path}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> DeleteFile(string path)
    {
        if (string.IsNullOrEmpty(path)) return NotFound();
        path = SharedStorageService.CleanUrl(path);
        var file = await _fileStorageService.ResolveFileAsync(path);
        if (file == null) return NotFound();
        await file.DeleteAsync();
        return NoContent();

    }
}