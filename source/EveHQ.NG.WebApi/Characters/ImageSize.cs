// Проект: EveHQ.NG.WebApi
// Имя файла: ImageSize.cs
// GUID файла: C48E411D-FABF-4988-9D95-8FAE2B978676
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Diagnostics.CodeAnalysis;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Just a batter naming for this purprose.")]
	public enum ImageSize
	{
		Image64x64,
		Image128x128,
		Image256x256,
		Image512x512
	}
}
