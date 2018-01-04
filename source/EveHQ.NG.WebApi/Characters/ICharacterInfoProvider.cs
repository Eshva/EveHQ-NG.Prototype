// Проект: EveHQ.NG.WebApi
// Имя файла: ICharacterInfoProvider.cs
// GUID файла: D47B72A7-D74E-45EC-9558-E34409360139
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Threading.Tasks;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	public interface ICharacterInfoProvider
	{
		Task<CharacterInfo> GetInfo(ulong id);

		Task<string> GetPortraitUri(ulong id, ImageSize imageSize);
	}
}
