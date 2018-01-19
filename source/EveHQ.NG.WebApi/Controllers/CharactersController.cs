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
		public IActionResult Get() => Json(_loggedInCharacterRepository.CharacterInfos);

		[HttpGet("{id}/info")]
		public IActionResult GetInfo([FromRoute] ulong id) => Json(_loggedInCharacterRepository.CharacterInfos.Single(info => info.Id == id));

		private readonly ILoggedInCharacterRepository _loggedInCharacterRepository;
	}
}
