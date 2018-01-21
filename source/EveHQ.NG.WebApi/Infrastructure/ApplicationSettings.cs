// Проект: EveHQ.NG.WebApi
// Имя файла: ApplicationSettings.cs
// GUID файла: 7EA22B8F-0E98-4DF7-9680-8F9540A09AD2
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 20.01.2018

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

#endregion


namespace EveHQ.NG.WebApi.Infrastructure
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Created by IoC-container.")]
	public sealed class ApplicationSettings
	{
		public string ApplicationDataFolder { get; set; } = string.Empty;

		public string TemporaryDataFolder { get; set; } = string.Empty;

		public ServiceSettings DefaultFolders
		{
			get => _defaultFolders;
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}

				_defaultFolders = value;
				ApplicationDataFolder = !string.IsNullOrEmpty(value.ApplicationDataFolder) ? value.ApplicationDataFolder : string.Empty;
				TemporaryDataFolder = !string.IsNullOrEmpty(value.TemporaryDataFolder) ? value.TemporaryDataFolder : string.Empty;
			}
		}

		private ServiceSettings _defaultFolders = new ServiceSettings();
	}
}
