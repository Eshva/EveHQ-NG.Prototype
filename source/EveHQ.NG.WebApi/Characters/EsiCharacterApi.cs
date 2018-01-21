// Проект: EveHQ.NG.WebApi
// Имя файла: EsiCharacterApi.cs
// GUID файла: 1084F88C-043A-4692-B8C9-5B066CC5C809
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC-container.")]
	public sealed class EsiCharacterApi : ICharactersApi
	{
		public EsiCharacterApi(ICharactersApiUriProvider charactersApiUriProvider)
		{
			_charactersApiUriProvider = charactersApiUriProvider;
		}

		public async Task<CharacterInfo> GetInfo(uint id)
		{
			return await CallEsiWebService(
				() => _charactersApiUriProvider.GetInfoUri(id),
				HttpMethod.Get,
				response => response.Content
									.ReadAsStringAsync()
									.ContinueWith(
										task =>
										{
											var dto = JsonConvert.DeserializeObject<EsiCharacterInfo>(task.Result);
											return new CharacterInfo { Id = id, Name = dto.Name, BornOn = dto.BornOn };
										}));
		}

		public async Task GetPortraits(Character character)
		{
			await CallEsiWebService(
				() => _charactersApiUriProvider.GetPortraitsUri(character),
				HttpMethod.Get,
				response => response.Content
									.ReadAsStringAsync()
									.ContinueWith(
										task =>
										{
											var dto = JsonConvert.DeserializeObject<EsiPortraitUris>(task.Result);
											character.Information.Portrait64Uri = dto.Image64x64Uri;
											character.Information.Portrait128Uri = dto.Image128x128Uri;
											character.Information.Portrait256Uri = dto.Image256x256Uri;
											character.Information.Portrait512Uri = dto.Image512x512Uri;
										}));
		}

		public async Task<IEnumerable<SkillQueueItem>> GetSkillQueue(Character character)
		{
			return await CallEsiWebService(
				() => _charactersApiUriProvider.GetSkillQueueUri(character),
				HttpMethod.Get,
				response => response.Content
									.ReadAsStringAsync()
									.ContinueWith(task => JsonConvert.DeserializeObject<EsiSkillQueueItem[]>(task.Result).Select(MapSkillQueueItem)));
		}

		private static SkillQueueItem MapSkillQueueItem(EsiSkillQueueItem dto) =>
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

		private async Task<TResult> CallEsiWebService<TResult>(
			Func<string> getUri,
			HttpMethod httpMethod,
			Func<HttpResponseMessage, Task<TResult>> prepareResult)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(httpMethod, getUri()))
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						return await prepareResult(response);
					}
				}
			}
		}

		private async Task CallEsiWebService(
			Func<string> getUri,
			HttpMethod httpMethod,
			Func<HttpResponseMessage, Task> prepareResult)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(httpMethod, getUri()))
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						await prepareResult(response);
					}
				}
			}
		}

		private readonly ICharactersApiUriProvider _charactersApiUriProvider;
	}
}
