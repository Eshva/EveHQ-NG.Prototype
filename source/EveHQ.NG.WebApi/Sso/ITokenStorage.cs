// Проект: EveHQ.NG.WebApi
// Имя файла: ITokenStorage.cs
// GUID файла: A2A28F15-4B83-41EE-B2A5-21F0AE8DEF67
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

namespace EveHQ.NG.WebApi.Sso
{
	public interface ITokenStorage
	{
		string AccessToken { get; set; }

		string RefreshToken { get; set; }
	}
}
