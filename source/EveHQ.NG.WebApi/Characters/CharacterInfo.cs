// Проект: EveHQ.NG.WebApi
// Имя файла: CharacterInfo.cs
// GUID файла: 320B2EF5-75FF-46E3-BFBA-9C5AD3BED62C
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	public sealed class CharacterInfo
	{
		public CharacterInfo(ulong characterId, string characterName, DateTime bornOn)
		{
			CharacterId = characterId;
			CharacterName = characterName;
			BornOn = bornOn;
		}

		public ulong CharacterId { get; }

		public string CharacterName { get; }

		public DateTime BornOn { get; }

		public IReadOnlyDictionary<ImageSize, string> PortraitUris
		{
			get
			{
				lock (_portraitUrisLock)
				{
					return new ReadOnlyDictionary<ImageSize, string>(_portraitUris);
				}
			}
		}

		public void SetPortraitsUris(IReadOnlyDictionary<ImageSize, string> portraitUris)
		{
			lock (_portraitUrisLock)
			{
				_portraitUris.Clear();
				foreach (var pair in portraitUris)
				{
					_portraitUris.Add(pair);
				}
			}
		}

		private readonly IDictionary<ImageSize, string> _portraitUris = new Dictionary<ImageSize, string>();
		private readonly object _portraitUrisLock = new object();
	}
}
