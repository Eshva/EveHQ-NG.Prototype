// Проект: EveHQ.NG.WebApi
// Имя файла: CachingCharacterInfoProvider.cs
// GUID файла: 6443AF16-63EE-461E-A75C-B108A6002D08
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC container.")]
	public sealed class CachingCharacterInfoProvider : ICharacterInfoProvider
	{
		public CachingCharacterInfoProvider(ICharactersApi charactersApi)
		{
			_charactersApi = charactersApi;
		}

		public async Task<CharacterInfo> GetInfo(ulong id)
		{
			lock (_charactersCacheLock)
			{
				var result = _charactersCache.SingleOrDefault(character => character.CharacterId == id);

				if (result != null)
				{
					return result;
				}
			}

			return await GetAndCacheCharacterInfoFromEsi(id);
		}

		public async Task<string> GetPortraitUri(ulong id, ImageSize imageSize)
		{
			var characterInfo = await GetInfo(id);
			if (!characterInfo.PortraitUris.Any())
			{
				await _charactersApi.GetPortrait(characterInfo);
			}

			return characterInfo.PortraitUris[imageSize];
		}

		private async Task<CharacterInfo> GetAndCacheCharacterInfoFromEsi(ulong id)
		{
			var result = await _charactersApi.GetInfo(id);
			lock (_charactersCacheLock)
			{
				if (_charactersCache.All(character => character.CharacterId != id))
				{
					_charactersCache.Add(result);
				}

				return result;
			}
		}

		private readonly object _charactersCacheLock = new object();
		private readonly ICharactersApi _charactersApi;
		private readonly IList<CharacterInfo> _charactersCache = new List<CharacterInfo>();
	}
}
