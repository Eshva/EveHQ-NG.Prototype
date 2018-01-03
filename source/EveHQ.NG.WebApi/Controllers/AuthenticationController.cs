// Проект: EveHQ.NG.WebApi
// Имя файла: AuthenticationController.cs
// GUID файла: C1B3CE30-1AB6-41FA-A66F-DCA1BA8E8C47
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 30.11.2017

#region Usings

using System.Threading.Tasks;
using EveHQ.NG.WebApi.Sso;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public sealed class AuthenticationController : Controller
	{
		[HttpGet("GetAuthenticationUri")]
		public IActionResult GetAuthenticationUri()
		{
			return Json(_authenticator.GetAuthenticationUri());
		}

		[HttpPost("SetAuthorizationCode")]
		public async Task<IActionResult> SetAuthorizationCodeAsync([FromQuery] string codeUri, [FromQuery] string state)
		{
			await _authenticator.SetAuthorizationCodeAsync(codeUri, state);
			return Ok();
		}

		private static readonly IOAuthAuthenticatior _authenticator = new SsoAuthenticator();
	}
}
