// Проект: EveHQ.NG.WebApi
// Имя файла: SsoAuthenticator.cs
// GUID файла: 382761A0-3072-4A22-805B-C6D687D4BDCF
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Characters;
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
			ILoggedInCharacterRepository loggedInCharacterRepository)
		{
			_authenticationSecretsStorage = authenticationSecretsStorage;
			_loggedInCharacterRepository = loggedInCharacterRepository;
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

		public async Task<CharacterTokens> AuthenticateCharacterWithAutharizationCode(string codeUri, string state)
		{
			if (!state.Equals(_authenticationSecretsStorage.StateKey, StringComparison.OrdinalIgnoreCase))
			{
				throw new ApplicationException("Authentication faild because state key was wrong. Seams like someone wants to steal your credentials.");
			}

			var authorizationCode = ExtractCodeFromCodeUri(codeUri);

			using (var httpClient = new HttpClient())
			{
				return await GetTokens(httpClient, authorizationCode);
			}
		}

		public async Task RefreshTokens(CharacterTokens tokens)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = CreateRefreshTokenRequest(tokens.RefreshToken))
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						ValidateResponse(response);

						var jsonResponse = await response.Content.ReadAsStringAsync();
						var authorizationResponse = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(jsonResponse);
						tokens.AccessToken = authorizationResponse.AccessToken;
						tokens.RefreshToken = authorizationResponse.RefreshToken;
						tokens.AccessTokenValidTill = DateTimeOffset.Now.AddSeconds(authorizationResponse.ExpirationTimeInSeconds);
					}
				}
			}
		}

		public void Logout(uint characterId)
		{
			var character = _loggedInCharacterRepository.CharacterInfos.Single(info => info.Id == characterId);
			Console.WriteLine($"Logged out character '{character.Name}' with ID {character.Id}.");

			_loggedInCharacterRepository.RemoveCharacter(characterId);
		}

		private async Task<CharacterTokens> GetTokens(HttpClient httpClient, string authorizationCode)
		{
			using (var request = CreateAuthorizationRequest(authorizationCode))
			{
				using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
				{
					ValidateResponse(response);

					var jsonResponse = await response.Content.ReadAsStringAsync();
					var authorizationResponse = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(jsonResponse);
					return new CharacterTokens
							{
								AccessToken = authorizationResponse.AccessToken,
								RefreshToken = authorizationResponse.RefreshToken,
								AccessTokenValidTill = DateTimeOffset.Now.AddSeconds(authorizationResponse.ExpirationTimeInSeconds)
							};
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

		private string ExtractCodeFromCodeUri(string codeUri)
		{
			var codeExtractionRegex = new Regex($@"^{_authenticationSecretsStorage.RedirectUri}\?code=(?<code>.*)$");
			return codeExtractionRegex.Match(codeUri).Groups["code"].Value;
		}

		private HttpRequestMessage CreateAuthorizationRequest(string authorizationCode)
		{
			return CreateTokenRequest(
				() => new[]
					{
						new KeyValuePair<string, string>("grant_type", "authorization_code"),
						new KeyValuePair<string, string>("code", authorizationCode)
					});
		}

		private HttpRequestMessage CreateRefreshTokenRequest(string refreshToken)
		{
			return CreateTokenRequest(
				() => new[]
					{
						new KeyValuePair<string, string>("grant_type", "refresh_token"),
						new KeyValuePair<string, string>("refresh_token", refreshToken)
					});
		}

		private HttpRequestMessage CreateTokenRequest(Func<IEnumerable<KeyValuePair<string, string>>> getRequestParameters)
		{
			var requestContent = new FormUrlEncodedContent(getRequestParameters());
			var message = new HttpRequestMessage(HttpMethod.Post, "https://login.eveonline.com/oauth/token") { Content = requestContent };
			var authorizationIdAndSecret =
				Base64UrlTextEncoder.Encode(
					Encoding.ASCII.GetBytes($"{_authenticationSecretsStorage.ClientId}:{_authenticationSecretsStorage.ClientSecret}"));
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorizationIdAndSecret}==");
			message.Headers.Host = HostUri;
			return message;
		}

		private readonly IAuthenticationSecretsStorage _authenticationSecretsStorage;
		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
		private const string HostUri = "login.eveonline.com";
	}
}
