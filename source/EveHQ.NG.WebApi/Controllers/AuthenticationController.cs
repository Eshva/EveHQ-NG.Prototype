// Проект: EveHQ.NG.WebApi
// Имя файла: AuthenticationController.cs
// GUID файла: C1B3CE30-1AB6-41FA-A66F-DCA1BA8E8C47
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 30.11.2017

#region Usings

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Characters;
using EveHQ.NG.WebApi.Sso;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public sealed class AuthenticationController : Controller
	{
		public AuthenticationController(
			IOAuthAuthenticator authenticator,
			ICharactersApi charactersApi,
			ILoggedInCharacterRepository loggedInCharacterRepository)
		{
			_authenticator = authenticator;
			_charactersApi = charactersApi;
			_loggedInCharacterRepository = loggedInCharacterRepository;
		}

		[HttpGet("GetAuthenticationUri")]
		public IActionResult GetAuthenticationUri()
		{
			return Json(_authenticator.GetAuthenticationUri());
		}

		[HttpPost("AuthenticatioWithCode")]
		public async Task<IActionResult> AuthenticateCharacterWithAutharizationCode([FromQuery] string codeUri, [FromQuery] string state)
		{
			var tokens = await _authenticator.AuthenticateCharacterWithAutharizationCode(codeUri, state);

			using (var httpClient = new HttpClient())
			{
				var info = await GetCharacterInfo(httpClient, tokens.AccessToken);
				var character = new Character { Information = info, Tokens = tokens };
				await _charactersApi.GetPortraits(character);

				_loggedInCharacterRepository.AddOrReplaceCharacter(character);
				Console.WriteLine($"Gotten tokens for character '{info.Name}' with ID {info.Id}.");
			}

			return Ok();
		}

		[HttpPost("{characterId}/logout")]
		public IActionResult Logout([FromRoute] uint characterId)
		{
			_authenticator.Logout(characterId);
			return Ok();
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

		private HttpRequestMessage CreateGetCharacterIdRequest(string accessToken)
		{
			var message = new HttpRequestMessage(HttpMethod.Get, "https://login.eveonline.com/oauth/verify");
			message.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
			message.Headers.Host = HostUri;

			return message;
		}

		private static void ValidateResponse(HttpResponseMessage response)
		{
			if (!response.IsSuccessStatusCode)
			{
				throw new ApplicationException(
					$"Authentication failed with code: {response.StatusCode} and message: '{response.ReasonPhrase}'.");
			}
		}

		private readonly IOAuthAuthenticator _authenticator;
		private readonly ICharactersApi _charactersApi;
		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
		private const string HostUri = "login.eveonline.com";
	}
}
