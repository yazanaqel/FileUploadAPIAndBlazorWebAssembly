using FileUploadAPIAndBlazorWebAssembly.Server.Data;
using FileUploadAPIAndBlazorWebAssembly.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;

namespace FileUploadAPIAndBlazorWebAssembly.Server.Controllers;
[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
	private readonly IWebHostEnvironment webHost;
	private readonly DataContext dataContext;

	public FilesController(IWebHostEnvironment webHost, DataContext dataContext)
	{
		this.webHost = webHost;
		this.dataContext = dataContext;
	}

	[HttpGet("{fileName}")]
	public async Task<IActionResult> Download(string fileName)
	{
		var uploadResult = await dataContext.Uploads.FirstOrDefaultAsync(x => x.StoredFileName.Equals(fileName));

		if (uploadResult is null)
			return BadRequest("Not Found!");

		var path = Path.Combine(webHost.ContentRootPath, "Uploads", fileName);

		var memory = new MemoryStream();
		using (var stream = new FileStream(path, FileMode.Open))
		{
			await stream.CopyToAsync(memory);
		}

		memory.Position = 0;

		return File(memory, uploadResult.ContentType, Path.GetFileName(path));
	}

	[HttpPost("Upload")]
	public async Task<ActionResult<List<UploadResult>>> Upload(List<IFormFile> files)
	{
		List<UploadResult> uploadResults = new List<UploadResult>();

		foreach (var file in files)
		{
			var uploadResult = new UploadResult();

			string trustedFileNameForFileStorage;

			var unTrustedFileName = file.FileName;

			uploadResult.FileName = unTrustedFileName;

			//var trustedFileNameForDisplay = WebUtility.HtmlEncode(unTrustedFileName);

			trustedFileNameForFileStorage = Path.GetRandomFileName();

			var path = Path.Combine(webHost.ContentRootPath, "Uploads", trustedFileNameForFileStorage);

			await using FileStream stream = new(path, FileMode.Create);
			await stream.CopyToAsync(stream);

			uploadResult.StoredFileName = trustedFileNameForFileStorage;
			uploadResult.ContentType = file.ContentType;

			uploadResults.Add(uploadResult);

			dataContext.Uploads.Add(uploadResult);
		}

		await dataContext.SaveChangesAsync();

		return Ok(uploadResults);
	}
}
