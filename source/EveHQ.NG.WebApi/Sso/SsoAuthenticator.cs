// Проект: EveHQ.NG.WebApi
// Имя файла: SsoAuthenticator.cs
// GUID файла: 382761A0-3072-4A22-805B-C6D687D4BDCF
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC container.")]
	public sealed class SsoAuthenticator : IOAuthAuthenticator
	{
		public SsoAuthenticator(IAuthenticationSecretsStorage authenticationSecretsStorage, ITokenStorage tokenStorage)
		{
			_authenticationSecretsStorage = authenticationSecretsStorage;
			_tokenStorage = tokenStorage;
		}

		public string GetAuthenticationUri()
		{
			var uriBuilder = new StringBuilder("https://login.eveonline.com/oauth/authorize/");
			uriBuilder.Append("?response_type=code")
					.Append($"&redirect_uri={WebUtility.UrlEncode(_authenticationSecretsStorage.RedirectUri)}")
					.Append($"&client_id={_authenticationSecretsStorage.ClientId}")
					.Append($"&scope={WebUtility.UrlEncode(_authenticationSecretsStorage.Scopes)}")
					.Append($"&state={_authenticationSecretsStorage.StateKey}");

			return uriBuilder.ToString();
		}

		public async Task SetAuthorizationCodeAsync(string codeUri, string state)
		{
			if (!state.Equals(_authenticationSecretsStorage.StateKey, StringComparison.OrdinalIgnoreCase))
			{
				throw new ApplicationException("Authentication faild because state key was wrong. Seams like someone want to steal your credentials.");
			}

			var code = ExtractCodeFromCodeUri(codeUri);

			using (var httpClient = new HttpClient())
			{
				using (var requestMessage = CreateAuthorizationRequest(code))
				{
					using (var responseMessage = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseContentRead))
					{
						if (!responseMessage.IsSuccessStatusCode)
						{
							throw new ApplicationException(
								$"Authentication failed with code: {responseMessage.StatusCode} and message: '{responseMessage.ReasonPhrase}'.");
						}

						var jsonResponse = await responseMessage.Content.ReadAsStringAsync();
						var authorizationResponse = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(jsonResponse);
						_tokenStorage.AccessToken = authorizationResponse.AccessToken;
						_tokenStorage.RefreshToken = authorizationResponse.RefreshToken;
					}
				}
			}
		}

		private string ExtractCodeFromCodeUri(string codeUri)
		{
			var codeExtractionRegex = new Regex($@"^{_authenticationSecretsStorage.RedirectUri}\?code=(?<code>.*)$");
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
			var authorizationIdAndSecret =
				Base64UrlTextEncoder.Encode(Encoding.ASCII.GetBytes($"{_authenticationSecretsStorage.ClientId}:{_authenticationSecretsStorage.ClientSecret}"));
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorizationIdAndSecret}==");
			message.Headers.Host = "login.eveonline.com";
			return message;
		}

		private readonly IAuthenticationSecretsStorage _authenticationSecretsStorage;
		private readonly ITokenStorage _tokenStorage;
	}
}
