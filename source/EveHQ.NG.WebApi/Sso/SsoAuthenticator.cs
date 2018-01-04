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
		public SsoAuthenticator(
			IAuthenticationSecretsStorage authenticationSecretsStorage,
			ITokenStorage tokenStorage,
			IAuthenticationNotificationService authenticationNotificationService)
		{
			_authenticationSecretsStorage = authenticationSecretsStorage;
			_tokenStorage = tokenStorage;
			_authenticationNotificationService = authenticationNotificationService;
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
				throw new ApplicationException("Authentication faild because state key was wrong. Seams like someone wants to steal your credentials.");
			}

			var authorizationCode = ExtractCodeFromCodeUri(codeUri);

			using (var httpClient = new HttpClient())
			{
				await GetTokens(httpClient, authorizationCode);
				await GetCharacterInfo(httpClient);

				Console.WriteLine($"Gotten tokens for character '{_tokenStorage.CharacterName}' with ID {_tokenStorage.CharacterId}.");
				NotifyClientsAboutCharacterChanged();
			}
		}

		public void Logout()
		{
			Console.WriteLine($"Logged out character '{_tokenStorage.CharacterName}' with ID {_tokenStorage.CharacterId}.");

			_tokenStorage.CharacterId = 0;
			_tokenStorage.CharacterName = string.Empty;
			_tokenStorage.AccessToken = string.Empty;
			_tokenStorage.RefreshToken = string.Empty;

			NotifyClientsAboutCharacterChanged();
		}

		private async Task GetTokens(HttpClient httpClient, string authorizationCode)
		{
			using (var request = CreateAuthorizationRequest(authorizationCode))
			{
				using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
				{
					ValidateResponse(response);

					var jsonResponse = await response.Content.ReadAsStringAsync();
					var authorizationResponse = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(jsonResponse);
					_tokenStorage.AccessToken = authorizationResponse.AccessToken;
					_tokenStorage.RefreshToken = authorizationResponse.RefreshToken;
				}
			}
		}

		private async Task GetCharacterInfo(HttpClient httpClient)
		{
			using (var request = CreateGetCharacterIdRequest())
			{
				using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
				{
					ValidateResponse(response);

					var jsonResponse = await response.Content.ReadAsStringAsync();
					var characterInfoResponse = JsonConvert.DeserializeObject<SsoAuthenticatedCharacterInfo>(jsonResponse);
					_tokenStorage.CharacterId = characterInfoResponse.CharacterId;
					_tokenStorage.CharacterName = characterInfoResponse.CharacterName;
				}
			}
		}

		private static void ValidateResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(
					$"Authentication failed with code: {response.StatusCode} and message: '{response.ReasonPhrase}'.");
			}
		}

		private HttpRequestMessage CreateGetCharacterIdRequest()
		{
			var message = new HttpRequestMessage(HttpMethod.Get, "https://login.eveonline.com/oauth/verify");
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {_tokenStorage.AccessToken}");
			message.Headers.Host = HostUri;

			return message;
		}

		private string ExtractCodeFromCodeUri(string codeUri)
		{
			var codeExtractionRegex = new Regex($@"^{_authenticationSecretsStorage.RedirectUri}\?code=(?<code>.*)$");
			return codeExtractionRegex.Match(codeUri).Groups["code"].Value;
		}

		private HttpRequestMessage CreateAuthorizationRequest(string authorizationCode)
		{
			var requestContent = new FormUrlEncodedContent(
				new[]
				{
					new KeyValuePair<string, string>("grant_type", "authorization_code"),
					new KeyValuePair<string, string>("code", authorizationCode)
				});
			var message = new HttpRequestMessage(HttpMethod.Post, "https://login.eveonline.com/oauth/token") { Content = requestContent };
			var authorizationIdAndSecret =
				Base64UrlTextEncoder.Encode(
					Encoding.ASCII.GetBytes($"{_authenticationSecretsStorage.ClientId}:{_authenticationSecretsStorage.ClientSecret}"));
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorizationIdAndSecret}==");
			message.Headers.Host = HostUri;
			return message;
		}

		private void NotifyClientsAboutCharacterChanged() =>
			_authenticationNotificationService.NotifyAboutLoginStatusChanged(_tokenStorage.CharacterId);

		private readonly IAuthenticationSecretsStorage _authenticationSecretsStorage;
		private readonly ITokenStorage _tokenStorage;
		private readonly IAuthenticationNotificationService _authenticationNotificationService;
		private const string HostUri = "login.eveonline.com";
	}
}
