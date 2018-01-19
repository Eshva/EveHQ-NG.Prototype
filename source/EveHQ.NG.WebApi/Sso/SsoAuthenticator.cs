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
			ICharactersApi charactersApi,
			ILoggedInCharacterRepository loggedInCharacterRepository)
		{
			_authenticationSecretsStorage = authenticationSecretsStorage;
			_charactersApi = charactersApi;
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

		public async Task AuthenticateCharacterWithAutharizationCode(string codeUri, string state)
		{
			if (!state.Equals(_authenticationSecretsStorage.StateKey, StringComparison.OrdinalIgnoreCase))
			{
				throw new ApplicationException("Authentication faild because state key was wrong. Seams like someone wants to steal your credentials.");
			}

			var authorizationCode = ExtractCodeFromCodeUri(codeUri);

			using (var httpClient = new HttpClient())
			{
				var tokens = await GetTokens(httpClient, authorizationCode);
				var info = await GetCharacterInfo(httpClient, tokens.AccessToken);
				await _charactersApi.GetPortraits(info);

				var character = new Character { Information = info, Tokens = tokens };

				_loggedInCharacterRepository.AddOrReplaceCharacter(character);
				Console.WriteLine($"Gotten tokens for character '{info.Name}' with ID {info.Id}.");
			}
		}

		public void Logout(ulong characterId)
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

		private async Task<CharacterInfo> GetCharacterInfo(HttpClient httpClient, string accessToken)
		{
			using (var request = CreateGetCharacterIdRequest(accessToken))
			{
				using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
				{
					ValidateResponse(response);

					var jsonResponse = await response.Content.ReadAsStringAsync();
					var characterInfoResponse = JsonConvert.DeserializeObject<SsoAuthenticatedCharacterInfo>(jsonResponse);

					return await _charactersApi.GetInfo(characterInfoResponse.CharacterId);
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

		private HttpRequestMessage CreateGetCharacterIdRequest(string accessToken)
		{
			var message = new HttpRequestMessage(HttpMethod.Get, "https://login.eveonline.com/oauth/verify");
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
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

		private readonly IAuthenticationSecretsStorage _authenticationSecretsStorage;
		private readonly ICharactersApi _charactersApi;
		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
		private const string HostUri = "login.eveonline.com";
	}
}
