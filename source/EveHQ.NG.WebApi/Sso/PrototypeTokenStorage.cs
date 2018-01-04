// Проект: EveHQ.NG.WebApi
// Имя файла: PrototypeTokenStorage.cs
// GUID файла: 112D7F1F-18BE-4797-A1E5-2FE920DB7B35
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion


namespace EveHQ.NG.WebApi.Sso
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by IoC container.")]
	public sealed class PrototypeTokenStorage : ITokenStorage
	{
		public string AccessToken { get; set; }

		public string RefreshToken { get; set; }

		public ulong CharacterId { get; set; }

		public string CharacterName { get; set; }
	}
}
