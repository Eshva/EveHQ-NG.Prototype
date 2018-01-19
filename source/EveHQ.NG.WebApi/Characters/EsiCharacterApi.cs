// Проект: EveHQ.NG.WebApi
// Имя файла: EsiCharacterApi.cs
// GUID файла: 1084F88C-043A-4692-B8C9-5B066CC5C809
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC container.")]
	public sealed class EsiCharacterApi : ICharactersApi
	{
		public async Task<CharacterInfo> GetInfo(ulong id)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(HttpMethod.Get, $"https://esi.tech.ccp.is/latest/characters/{id}/?datasource=tranquility"))
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						var jsonResponse = await response.Content.ReadAsStringAsync();
						var dto = JsonConvert.DeserializeObject<EsiCharacterInfo>(jsonResponse);
						var result = new CharacterInfo { Id = id, Name = dto.Name, BornOn = dto.BornOn };
						return result;
					}
				}
			}
		}

		public async Task GetPortraits(CharacterInfo character)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(
					HttpMethod.Get,
					$"https://esi.tech.ccp.is/latest/characters/{character.Id}/portrait/?datasource=tranquility"))
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						await response.Content.ReadAsStringAsync().ContinueWith(
							task =>
							{
								var dto = JsonConvert.DeserializeObject<EsiPortraitUris>(task.Result);
								character.Portrait64Uri = dto.Image64x64Uri;
								character.Portrait128Uri = dto.Image128x128Uri;
								character.Portrait256Uri = dto.Image256x256Uri;
								character.Portrait512Uri = dto.Image512x512Uri;
							});
					}
				}
			}
		}
	}
}
