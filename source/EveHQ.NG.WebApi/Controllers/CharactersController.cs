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
		public CharactersController(
			ILoggedInCharacterRepository characterRepository,
			ICharactersApi charactersApi)
		{
			_characterRepository = characterRepository;
			_charactersApi = charactersApi;
		}

		[HttpGet]
		public IActionResult Get() => Json(_characterRepository.CharacterInfos);

		[HttpGet("{id}/info")]
		public IActionResult GetInfo([FromRoute] uint id) =>
			Json(_characterRepository.GetCharacterById(id).Information);

		[HttpGet("{id}/skillqueue")]
		public async Task<IActionResult> GetSkillQueue([FromRoute] uint id) =>
			Json(await _charactersApi.GetSkillQueue(_characterRepository.GetCharacterById(id)));

		private readonly ILoggedInCharacterRepository _characterRepository;
		private readonly ICharactersApi _charactersApi;
	}
}
