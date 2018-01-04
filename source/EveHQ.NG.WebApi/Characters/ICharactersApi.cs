// Проект: EveHQ.NG.WebApi
// Имя файла: ICharactersApi.cs
// GUID файла: A6C452D7-E741-4BFE-AB6B-F23EC0D5FA3B
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

using System.Threading.Tasks;


namespace EveHQ.NG.WebApi.Characters
{
	public interface ICharactersApi
	{
		Task<CharacterInfo> GetInfo(ulong id);

		Task GetPortrait(CharacterInfo characterInfo);
	}
}
