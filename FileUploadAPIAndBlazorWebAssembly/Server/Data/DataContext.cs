using FileUploadAPIAndBlazorWebAssembly.Shared;
using Microsoft.EntityFrameworkCore;

namespace FileUploadAPIAndBlazorWebAssembly.Server.Data;

public class DataContext : DbContext
{
	public DataContext(DbContextOptions<DataContext> options) : base(options)
	{

	}

    public DbSet<UploadResult> Uploads { get; set; }
}
