// Проект: EveHQ.NG.WebApi
// Имя файла: ILoggedInCharacterRepository.cs
// GUID файла: C55B52E6-B234-4CFC-AF8F-78FF36CCE5EA
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 14.01.2018

#region Usings

using System.Collections.Generic;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	public interface ILoggedInCharacterRepository
	{
		IReadOnlyList<CharacterInfo> CharacterInfos { get; }

		IReadOnlyList<Character> Characters { get; }

		void AddOrReplaceLoggedInCharacter(Character character);

		void RemoveLoggedOutCharacter(ulong characterId);
	}
}
