// Проект: EveHQ.NG.WebApi
// Имя файла: ICharactersApiUriProvider.cs
// GUID файла: DC0B2DBD-892A-4B6D-AF29-BA12471EB8F2
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 20.01.2018

namespace EveHQ.NG.WebApi.Characters
{
	public interface ICharactersApiUriProvider
	{
		string GetSkillQueueUri(Character character);

		string GetPortraitsUri(Character character);

		string GetInfoUri(uint characterId);
	}
}
