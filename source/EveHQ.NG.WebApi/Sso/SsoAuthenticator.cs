// Проект: EveHQ.NG.WebApi
// Имя файла: SsoAuthenticator.cs
// GUID файла: 382761A0-3072-4A22-805B-C6D687D4BDCF
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Controllers;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Sso
{
	public class SsoAuthenticator : IOAuthAuthenticatior
	{
		public string GetAuthenticationUri()
		{
			var uriBuilder = new StringBuilder("https://login.eveonline.com/oauth/authorize/");
			uriBuilder.Append("?response_type=code")
					.Append($"&redirect_uri={WebUtility.UrlEncode(RedirectUri)}")
					.Append($"&client_id={ClientId}")
					.Append($"&scope={WebUtility.UrlEncode(Scopes)}")
					.Append($"&state={StateKey}");

			return uriBuilder.ToString();
		}

		public async Task SetAuthorizationCodeAsync(string codeUri, string state)
		{
			var code = ExtractCodeFromCodeUri(codeUri);

			using (var httpClient = new HttpClient())
			{
				using (var requestMessage = CreateAuthorizationRequest(code))
				{
					using (var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead))
					{
						var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
						var authorizationResponse = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(jsonResponse);
					}
				}
			}
		}

		private string ExtractCodeFromCodeUri(string codeUri)
		{
			var codeExtractionRegex = new Regex($@"^{RedirectUri}\?code=(?<code>.*)$");
			return codeExtractionRegex.Match(codeUri).Groups["code"].Value;
		}

		private HttpRequestMessage CreateAuthorizationRequest(string code)
		{
			var requestContent = new FormUrlEncodedContent(
				new[]
				{
					new KeyValuePair<string, string>("grant_type", "authorization_code"),
					new KeyValuePair<string, string>("code", code)
				});
			var message = new HttpRequestMessage(HttpMethod.Post, "https://login.eveonline.com/oauth/token") { Content = requestContent };
			var authorizationIdAndSecret = Base64UrlTextEncoder.Encode(Encoding.ASCII.GetBytes($"{ClientId}:{ClientSecret}"));
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorizationIdAndSecret}==");
			message.Headers.Host = "login.eveonline.com";
			return message;
		}

		private const string RedirectUri = "eveauth-evehq-ng://sso-auth/";
		private const string ClientId = "9158bdcbc32a49e29044be4266b029dd";
		private const string ClientSecret = "SJb4jaOUHbVm3KSrrPsJKo82cmiYxvoXtlEIgu5R";
		private const string Scopes = "esi-skills.read_skillqueue.v1";
		private const string StateKey = "auth-state-key";
	}
}
