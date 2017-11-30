// Проект: EveHQ.NG.WebApi
// Имя файла: AuthenticationController.cs
// GUID файла: C1B3CE30-1AB6-41FA-A66F-DCA1BA8E8C47
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 30.11.2017

#region Usings

using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]/[action]")]
	public sealed class AuthenticationController : Controller
	{
		[HttpGet]
		public IActionResult GetAuthenticationUri()
		{
			const string RedirectUri = "eveauth-evehq-ng://sso-auth/";
			const string ClientId = "9158bdcbc32a49e29044be4266b029dd";
			const string Scopes = "esi-skills.read_skillqueue.v1";
			const string StateKey = "auth-state-key";

			var uriBuilder = new StringBuilder("https://login.eveonline.com/oauth/authorize/");
			uriBuilder.Append("?response_type=code")
					.Append($"&redirect_uri={WebUtility.UrlEncode(RedirectUri)}")
					.Append($"&client_id={ClientId}")
					.Append($"&scope={WebUtility.UrlEncode(Scopes)}")
					.Append($"&state={StateKey}");

			return Json(uriBuilder.ToString());
		}
	}
}
