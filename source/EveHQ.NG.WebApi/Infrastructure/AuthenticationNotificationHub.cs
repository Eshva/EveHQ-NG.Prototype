// Проект: EveHQ.NG.WebApi
// Имя файла: AuthenticationNotificationHub.cs
// GUID файла: B8FAE79F-A09D-4581-993A-153837A957FD
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using EveHQ.NG.WebApi.Characters;
using EveHQ.NG.WebApi.Sso;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

#endregion


namespace EveHQ.NG.WebApi.Infrastructure
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by ASP.NET Core.")]
	public sealed class AuthenticationNotificationHub : Hub, IAuthenticationNotificationService
	{
		public AuthenticationNotificationHub()
		{
			_id = new Random().Next();
			Console.WriteLine($"A hub with ID {_id} created.");
		}

		public override Task OnConnectedAsync()
		{
			Console.WriteLine($"A user connected to the hub with ID {_id}.");
			return base.OnConnectedAsync();
		}

		public void NotifyClientsAboutCharacterListChanged(IReadOnlyList<CharacterInfo> characters)
		{
			Console.WriteLine(
				$"Sending notification to clients of the hub with ID {_id}. Logged in characters: {JsonConvert.SerializeObject(characters)}.");
			Clients.All.InvokeAsync("LoggedInCharacterListChanged", characters);
		}

		private readonly int _id;
	}
}
