// Проект: EveHQ.NG.WebApi
// Имя файла: ContactsController.cs
// GUID файла: 6C468A3B-48DB-4D9B-B406-C2D771A6AB18
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 13.10.2017

#region Usings

using Microsoft.AspNetCore.Mvc;

#endregion


namespace EveHQ.NG.WebApi.Controllers
{
	[Route("api/[controller]")]
	public class ContactsController : Controller
	{
		// GET api/contacts
		[HttpGet]
		public IActionResult Get()
		{
			var result = new[]
						{
							new { FirstName = "John", LastName = "Doe" },
							new { FirstName = "Mike", LastName = "Smith" }
						};

			return Ok(result);
		}
	}
}
