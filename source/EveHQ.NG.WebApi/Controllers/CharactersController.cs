// Проект: EveHQ.NG.WebApi
// Имя файла: CharactersController.cs
// GUID файла: 63A4046A-1A31-44F1-9654-EFBB8F3E4F5D
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Threading.Tasks;
using EveHQ.NG.WebApi.Characters;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public sealed class CharactersController : Controller
	{
		public CharactersController(ICharacterInfoProvider characterInfoProvider)
		{
			_characterInfoProvider = characterInfoProvider;
		}

		[HttpGet("{id}/info")]
		public async Task<IActionResult> GetInfo([FromRoute] ulong id)
		{
			return Json(await _characterInfoProvider.GetInfo(id));
		}

		[HttpGet("{id}/portrait/{size?}")]
		public async Task<IActionResult> GetPortrait([FromRoute] ulong id, [FromRoute] string size)
		{
			return Json(await _characterInfoProvider.GetPortraitUri(id, ConvertImageSize(size)));
		}

		private ImageSize ConvertImageSize(string size, ImageSize defaultSize = ImageSize.Image512x512)
		{
			switch (size)
			{
				case "64x64":
					return ImageSize.Image64x64;
				case "128x128":
					return ImageSize.Image128x128;
				case "256x256":
					return ImageSize.Image256x256;
				case "512x512":
					return ImageSize.Image512x512;
				default:
					return defaultSize;
			}
		}

		private readonly ICharacterInfoProvider _characterInfoProvider;
	}
}
