#region Usings

using EveHQ.NG.WebApi.Infrastructure;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public sealed class SettingsController : Controller
	{
		public SettingsController(ApplicationSettings applicationSettings)
		{
			_applicationSettings = applicationSettings;
		}

		[HttpPost("setDefaults")]
		public IActionResult SetDefults([FromBody] ServiceSettings settings)
		{
			_applicationSettings.DefaultFolders = settings;
			return Ok();
		}

		private readonly ApplicationSettings _applicationSettings;
	}
}
