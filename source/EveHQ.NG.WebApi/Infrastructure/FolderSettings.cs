// Проект: EveHQ.NG.WebApi
// Имя файла: FolderSettings.cs
// GUID файла: E4177558-1D50-47EA-B316-C7216A655973
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 29.01.2018

namespace EveHQ.NG.WebApi.Infrastructure
{
	public sealed class FolderSettings
	{
		public string ApplicationDataFolder { get; set; } = string.Empty;

		public string TemporaryDataFolder { get; set; } = string.Empty;
	}
}
