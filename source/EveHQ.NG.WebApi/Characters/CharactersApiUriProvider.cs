// Проект: EveHQ.NG.WebApi
// Имя файла: CharactersApiUriProvider.cs
// GUID файла: EB0E7503-3253-4FB0-A167-FA6A6FF53200
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 20.01.2018

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Infrastructure;
using EveHQ.NG.WebApi.Sso;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by IoC-container.")]
	public sealed class CharactersApiUriProvider : ApiUriProviderBase, ICharactersApiUriProvider
	{
		public CharactersApiUriProvider(
			IOAuthAuthenticator authenticator,
			IClock clock)
			: base(authenticator, clock)
		{
			// TODO: Extract latest and tranquility as settings.
		}

		public async Task<string> GetInfoUri(uint characterId) =>
			await new Task<string>(() => $"{ApiUri}/{characterId}/?datasource=tranquility");

		public async Task<string> GetPortraitsUri(Character character) =>
			await new Task<string>(() => $"{ApiUri}/{character.Information.Id}/portrait/?datasource=tranquility");

		public async Task<string> GetSkillQueueUri(Character character)
		{
			var characterId = character.Information.Id;
			var token = await GetActualAccessTokenForCharacterAsync(character.Tokens);
			return $"{ApiUri}/{characterId}/skillqueue/?datasource=tranquility;token={token}";
		}

		private const string ApiUri = "https://esi.tech.ccp.is/latest/characters";
	}
}
