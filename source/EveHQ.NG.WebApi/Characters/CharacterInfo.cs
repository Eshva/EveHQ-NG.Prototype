// Проект: EveHQ.NG.WebApi
// Имя файла: CharacterInfo.cs
// GUID файла: E16D0B2B-57ED-438B-AFD5-1A7C973142D6
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 15.01.2018

#region Usings

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Characters
{
	public sealed class CharacterInfo
	{
		[JsonProperty("id")]
		public ulong Id { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; } = "";

		[JsonProperty("bornOn")]
		public DateTime BornOn { get; set; }

		[JsonProperty("portraitUris")]
		public Dictionary<ImageSize, string> PortraitUris { get; set; } = new Dictionary<ImageSize, string>();
	}
}
