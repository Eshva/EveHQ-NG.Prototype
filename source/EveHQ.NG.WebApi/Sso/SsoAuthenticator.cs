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
using EveHQ.NG.WebApi.Infrastructure;
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
			ILoggedInCharacterRepository loggedInCharacterRepository,
			IHttpService httpService)
		{
			_authenticationSecretsStorage = authenticationSecretsStorage;
			_loggedInCharacterRepository = loggedInCharacterRepository;
			_httpService = httpService;
		}

		public string GetAuthenticationUri()
		{
			var uriBuilder = new StringBuilder($"https://{HostUri}/oauth/authorize/");
			uriBuilder.Append("?response_type=code")
					.Append($"&redirect_uri={WebUtility.UrlEncode(_authenticationSecretsStorage.RedirectUri)}")
					.Append($"&client_id={_authenticationSecretsStorage.ClientId}")
					.Append($"&scope={WebUtility.UrlEncode(_authenticationSecretsStorage.Scopes)}")
					.Append($"&state={_authenticationSecretsStorage.StateKey}");

			return uriBuilder.ToString();
		}

		public async Task<CharacterTokens> AuthenticateCharacterWithAutharizationCode(string codeUri, string state)
		{
			string ExtractCodeFromCodeUri()
			{
				var codeExtractionRegex = new Regex($@"^{_authenticationSecretsStorage.RedirectUri}\?code=(?<code>.*)$");
				return codeExtractionRegex.Match(codeUri).Groups["code"].Value;
			}

			HttpRequestMessage CreateAuthorizationRequest() =>
				CreateTokenRequest(
					() => new[]
						{
							new KeyValuePair<string, string>("grant_type", "authorization_code"),
							new KeyValuePair<string, string>("code", ExtractCodeFromCodeUri())
						});

			CharacterTokens PrepareResult(Task<string> task)
			{
				var response = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(task.Result);
				return new CharacterTokens
						{
							AccessToken = response.AccessToken,
							RefreshToken = response.RefreshToken,
							// TODO: Add IClock service.
							AccessTokenValidTill = DateTimeOffset.Now.AddSeconds(response.ExpirationTimeInSeconds)
						};
			}

			if (!state.Equals(_authenticationSecretsStorage.StateKey, StringComparison.OrdinalIgnoreCase))
			{
				throw new ApplicationException("Authentication faild because state key was wrong. Seams like someone wants to steal your credentials.");
			}

			return await _httpService.CallAsync(
				CreateAuthorizationRequest,
				response => response.Content.ReadAsStringAsync().ContinueWith(PrepareResult));
		}

		public async Task RefreshTokens(CharacterTokens tokens)
		{
			HttpRequestMessage CreateRefreshTokenRequest() =>
				CreateTokenRequest(
					() => new[]
						{
							new KeyValuePair<string, string>("grant_type", "refresh_token"),
							new KeyValuePair<string, string>("refresh_token", tokens.RefreshToken)
						});

			void PrepareResult(Task<string> task)
			{
				var response = JsonConvert.DeserializeObject<SsoAuthorizationResponse>(task.Result);
				tokens.AccessToken = response.AccessToken;
				tokens.RefreshToken = response.RefreshToken;
				// TODO: Add IClock service.
				tokens.AccessTokenValidTill = DateTimeOffset.Now.AddSeconds(response.ExpirationTimeInSeconds);
			}

			await _httpService.CallAsync(
				CreateRefreshTokenRequest,
				response => response.Content.ReadAsStringAsync().ContinueWith(PrepareResult));
		}

		public void Logout(uint characterId)
		{
			var character = _loggedInCharacterRepository.CharacterInfos.Single(info => info.Id == characterId);
			Console.WriteLine($"Logged out character '{character.Name}' with ID {character.Id}.");

			_loggedInCharacterRepository.RemoveCharacter(characterId);
		}

		private HttpRequestMessage CreateTokenRequest(Func<IEnumerable<KeyValuePair<string, string>>> getRequestParameters)
		{
			var requestContent = new FormUrlEncodedContent(getRequestParameters());
			var message = new HttpRequestMessage(HttpMethod.Post, $"https://{HostUri}/oauth/token") { Content = requestContent };
			var authorizationIdAndSecret =
				Base64UrlTextEncoder.Encode(
					Encoding.ASCII.GetBytes($"{_authenticationSecretsStorage.ClientId}:{_authenticationSecretsStorage.ClientSecret}"));
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorizationIdAndSecret}==");
			message.Headers.Host = HostUri;
			return message;
		}

		private readonly IAuthenticationSecretsStorage _authenticationSecretsStorage;
		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
		private readonly IHttpService _httpService;
		private const string HostUri = "login.eveonline.com";
	}
}
