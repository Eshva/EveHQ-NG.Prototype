// Проект: EveHQ.NG.WebApi
// Имя файла: CharacterInfo.cs
// GUID файла: E16D0B2B-57ED-438B-AFD5-1A7C973142D6
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 15.01.2018

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "DTO")]
	public sealed class CharacterInfo
	{
		public uint Id { get; set; }

		public string Name { get; set; }

		public DateTime BornOn { get; set; }

		public string Portrait64Uri { get; set; }

		public string Portrait128Uri { get; set; }

		public string Portrait256Uri { get; set; }

		public string Portrait512Uri { get; set; }
	}
}
