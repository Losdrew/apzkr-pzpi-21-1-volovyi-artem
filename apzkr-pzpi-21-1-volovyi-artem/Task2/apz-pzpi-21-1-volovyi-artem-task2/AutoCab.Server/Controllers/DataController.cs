using AutoCab.Db.DbContexts;
using AutoCab.Server.Controllers.Base;
using AutoCab.Server.Extensions;
using AutoCab.Shared.Dto.Error;
using AutoCab.Shared.Helpers;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace AutoCab.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DataController(IMapper mapper, IMediator mediator,
        ApplicationDbContext context, IConfiguration configuration)
        : base(mapper, mediator)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("export-database")]
    [Authorize(Roles = Roles.Administrator)]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    [ProducesResponseType(typeof(ErrorDto), 400)]
    [FileDownload(FileName = "database.tar")]
    public IActionResult ExportDatabase()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\PostgreSQL\16\bin\pg_dump.exe",
            Arguments = $"-U {_configuration["DatabaseUsername"]} -F t {_configuration["DatabaseName"]}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        startInfo.Environment["PGPASSWORD"] = _configuration["DatabasePassword"];

        var process = new Process { StartInfo = startInfo };
        process.Start();

        using var ms = new MemoryStream();
        process.StandardOutput.BaseStream.CopyTo(ms);

        var bytes = ms.ToArray();
        return new FileContentResult(bytes, "application/octet-stream");
    }

    [HttpPost("import-database")]
    [Authorize(Roles = Roles.Administrator)]
    public async Task<IActionResult> ImportDatabase(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var tempFilePath = Path.GetTempFileName();

        using (var stream = System.IO.File.Create(tempFilePath))
        {
            await file.CopyToAsync(stream);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = @"C:\Program Files\PostgreSQL\16\bin\pg_restore.exe",
            Arguments = $"-U {_configuration["DatabaseUsername"]} -c -d {_configuration["DatabaseName"]} -F t {tempFilePath}",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true
        };

        startInfo.Environment["PGPASSWORD"] = _configuration["DatabasePassword"];

        var process = new Process
        {
            StartInfo = startInfo,
            EnableRaisingEvents = true
        };
        var stderr = new StringBuilder();
        process.ErrorDataReceived += (sender, args) => stderr.AppendLine(args.Data);

        process.Start();
        process.BeginErrorReadLine();
        process.WaitForExit();

        System.IO.File.Delete(tempFilePath);

        if (process.ExitCode != 0)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, stderr.ToString());
        }

        return Ok("Database imported successfully");
    }
}
