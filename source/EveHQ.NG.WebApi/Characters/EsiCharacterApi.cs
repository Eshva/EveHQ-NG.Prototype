// Проект: EveHQ.NG.WebApi
// Имя файла: EsiCharacterApi.cs
// GUID файла: 1084F88C-043A-4692-B8C9-5B066CC5C809
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Infrastructure;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC-container.")]
	public sealed class EsiCharacterApi : ICharactersApi
	{
		public EsiCharacterApi(
			ICharactersApiUriProvider charactersApiUriProvider,
			IHttpService httpService)
		{
			_charactersApiUriProvider = charactersApiUriProvider;
			_httpService = httpService;
		}

		public async Task<CharacterInfo> GetInfo(uint id)
		{
			CharacterInfo PrepareResult(Task<string> task)
			{
				var dto = JsonConvert.DeserializeObject<EsiCharacterInfo>(task.Result);
				return new CharacterInfo { Id = id, Name = dto.Name, BornOn = dto.BornOn };
			}

			return await _httpService.CallAsync(
				HttpMethod.Get,
				() => _charactersApiUriProvider.GetInfoUri(id),
				response => response.Content.ReadAsStringAsync().ContinueWith(PrepareResult));
		}

		public async Task GetPortraits(Character character)
		{
			void PrepareResult(Task<string> task)
			{
				var dto = JsonConvert.DeserializeObject<EsiPortraitUris>(task.Result);
				character.Information.Portrait64Uri = dto.Image64x64Uri;
				character.Information.Portrait128Uri = dto.Image128x128Uri;
				character.Information.Portrait256Uri = dto.Image256x256Uri;
				character.Information.Portrait512Uri = dto.Image512x512Uri;
			}

			await _httpService.CallAsync(
				HttpMethod.Get,
				() => _charactersApiUriProvider.GetPortraitsUri(character),
				response => response.Content.ReadAsStringAsync().ContinueWith(PrepareResult));
		}

		public async Task<IEnumerable<SkillQueueItem>> GetSkillQueue(Character character)
		{
			SkillQueueItem MapSkillQueueItem(EsiSkillQueueItem dto) =>
				new SkillQueueItem
				{
					SkillId = dto.SkillId,
					SkillName = $"Skill with ID {dto.SkillId}",
					WillFinishOn = dto.WillFinishOn,
					StartedOn = dto.StartedOn,
					FinishedLevel = dto.FinishedLevel,
					QueuePosition = dto.QueuePosition,
					TrainingStartSkillPoints = dto.TrainingStartSkillPoints,
					LevelEndSkillPoints = dto.LevelEndSkillPoints,
					LevelStartSkillPoints = dto.LevelStartSkillPoints
				};

			return await _httpService.CallAsync(
				HttpMethod.Get,
				() => _charactersApiUriProvider.GetSkillQueueUri(character),
				response => response.Content
									.ReadAsStringAsync()
									.ContinueWith(task => JsonConvert.DeserializeObject<EsiSkillQueueItem[]>(task.Result).Select(MapSkillQueueItem)));
		}

		private readonly ICharactersApiUriProvider _charactersApiUriProvider;
		private readonly IHttpService _httpService;
	}
}
