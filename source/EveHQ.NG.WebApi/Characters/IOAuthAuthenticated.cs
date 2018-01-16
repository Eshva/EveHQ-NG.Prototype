// Проект: EveHQ.NG.WebApi
// Имя файла: IOAuthAuthenticated.cs
// GUID файла: 320B2EF5-75FF-46E3-BFBA-9C5AD8BED62C
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 03.01.2018

#region Usings

using System;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	public interface IOAuthAuthenticated
	{
		string AccessToken { get; set; }

		string RefreshToken { get; set; }

		DateTimeOffset AccessTokenValidTill { get; set; }
	}
}
