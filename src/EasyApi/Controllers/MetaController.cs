using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;

namespace EasyApi.Controller
{
    [ApiController]
    public class MetaController : ControllerBase
    {

        private readonly IWebHostEnvironment _hostingEnvironment;

        public MetaController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet("/info")]
        public ActionResult<string> Info()
        {
            var assembly = typeof(MetaController).Assembly;
            var lastUpdate = System.IO.File.GetLastWriteTime(assembly.Location);
            var version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
            var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {assembly.GetName().Name}");
            sb.AppendLine($"Version: {version}");
            sb.AppendLine($"Last Updated: {lastUpdate}");
            sb.AppendLine($"MachineName: {System.Environment.MachineName}");
            sb.AppendLine($"Time: {DateTime.Now} ");
            sb.AppendLine($"Directory: {System.IO.Directory.GetCurrentDirectory()} ");
            sb.AppendLine($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            sb.AppendLine($"OS Version: {System.Environment.OSVersion}");
            sb.AppendLine($"64-bit OS: {System.Environment.Is64BitOperatingSystem}");
            sb.AppendLine($".NET Version: {System.Environment.Version}");
            sb.AppendLine($"Runtime Framework: {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName}");
            sb.AppendLine($"Web Root Path: {_hostingEnvironment.WebRootPath}");
            sb.AppendLine($"Content Root Path: {_hostingEnvironment.ContentRootPath}");
            sb.AppendLine($"Is Development: {_hostingEnvironment.IsDevelopment()}");
            sb.AppendLine($"StartTime: {Process.GetCurrentProcess().StartTime}");
            sb.AppendLine($"Uptime: {uptime:c}");


            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //sb.AppendLine("Loaded Assemblies:");
            //foreach (var asm in assemblies)
            //{
            //    sb.AppendLine($"{asm.GetName().Name}: {asm.GetName().Version}");
            //}

            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Request Headers:");
            foreach (var header in HttpContext.Request.Headers)
            {
                sb.AppendLine($"{header.Key}: {header.Value}");
            }

            return Ok(sb.ToString());
        }
    }
}
