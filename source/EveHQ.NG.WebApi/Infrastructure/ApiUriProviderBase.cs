// Проект: EveHQ.NG.WebApi
// Имя файла: ApiUriProviderBase.cs
// GUID файла: 047BC9B4-D677-4603-AC81-7D3649BE64DF
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 20.01.2018

#region Usings

using System;
using EveHQ.NG.WebApi.Characters;
using EveHQ.NG.WebApi.Sso;

#endregion


namespace EveHQ.NG.WebApi.Infrastructure
{
	public abstract class ApiUriProviderBase
	{
		protected ApiUriProviderBase(IOAuthAuthenticator authenticator)
		{
			_authenticator = authenticator;
		}

		protected string GetActualAccessTokenForCharacter(CharacterTokens tokens)
		{
			if (tokens.AccessTokenValidTill < DateTimeOffset.Now)
			{
				_authenticator.RefreshTokens(tokens);
			}

			return tokens.AccessToken;
		}

		private readonly IOAuthAuthenticator _authenticator;
	}
}
