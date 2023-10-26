using FileUploadAPIAndBlazorWebAssembly.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace FileUploadAPIAndBlazorWebAssembly.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
	private readonly IWebHostEnvironment webHost;

	public FilesController(IWebHostEnvironment webHost)
	{
		this.webHost = webHost;
	}

	[HttpPost]
	public async Task<ActionResult<List<UploadResult>>> Upload(List<IFormFile> files)
	{
		List<UploadResult> uploadResults = new List<UploadResult>();

		foreach (var file in files)
		{
			var uploadResult = new UploadResult();

			string trustedFileNameForFileStorage;

			var unTrustedFileName = file.FileName;

			uploadResult.FileName = unTrustedFileName;

			var trustedFileNameForDisplay = WebUtility.HtmlEncode(unTrustedFileName);

			trustedFileNameForFileStorage = Path.GetRandomFileName();

			var path = Path.Combine(webHost.ContentRootPath, "Uploads",trustedFileNameForFileStorage);

			await using FileStream stream = new(path, FileMode.Create);
			await stream.CopyToAsync(stream);

			uploadResult.StoredFileName = trustedFileNameForFileStorage;
			uploadResults.Add(uploadResult);
		}

		return Ok(uploadResults);
	}
}
