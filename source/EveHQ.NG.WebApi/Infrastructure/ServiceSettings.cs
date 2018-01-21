// Проект: EveHQ.NG.WebApi
// Имя файла: ServiceSettings.cs
// GUID файла: 5B1B0593-1179-4757-A0E9-9BD7546F53FD
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 20.01.2018

namespace EveHQ.NG.WebApi.Infrastructure
{
	public sealed class ServiceSettings
	{
		public string ApplicationFolder { get; set; }

		public string ApplicationDataFolder { get; set; }

		public string TemporaryDataFolder { get; set; }
	}
}
