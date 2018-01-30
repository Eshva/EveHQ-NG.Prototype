#region Usings

using EveHQ.NG.WebApi.Infrastructure;
using EveHQ.NG.WebApi.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	public sealed class SettingsController : ApiControllerBase
	{
		public SettingsController(IWritableOptions<ApplicationSettings> applicationSettings)
		{
			_applicationSettings = applicationSettings;
		}

		[HttpPost("folders")]
		public IActionResult SetApplicationSettings([FromBody] FolderSettings settings)
		{
			_applicationSettings.Update(
				applicationSettings =>
				{
					applicationSettings.FolderSettings.ApplicationDataFolder = settings.ApplicationDataFolder;
				});
			return Ok();
		}

		private readonly IWritableOptions<ApplicationSettings> _applicationSettings;
	}
}
