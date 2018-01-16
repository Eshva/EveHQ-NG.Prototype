// Проект: EveHQ.NG.WebApi
// Имя файла: CharactersController.cs
// GUID файла: 63A4046A-1A31-44F1-9654-EFBB8F3E4F5D
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System.Linq;
using EveHQ.NG.WebApi.Characters;
using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public sealed class CharactersController : Controller
	{
		public CharactersController(ILoggedInCharacterRepository loggedInCharacterRepository)
		{
			_loggedInCharacterRepository = loggedInCharacterRepository;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Json(_loggedInCharacterRepository.CharacterInfos);
		}

		[HttpGet("{id}/info")]
		public IActionResult GetInfo([FromRoute] ulong id)
		{
			return Json(_loggedInCharacterRepository.CharacterInfos.Single(info => info.Id == id));
		}

		[HttpGet("{id}/portrait/{size?}")]
		public IActionResult GetPortrait([FromRoute] ulong id, [FromRoute] string size)
		{
			return Json(
				_loggedInCharacterRepository.CharacterInfos
											.Single(info => info.Id == id)
											.PortraitUris[ConvertImageSize(size)]);
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

		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
	}
}
