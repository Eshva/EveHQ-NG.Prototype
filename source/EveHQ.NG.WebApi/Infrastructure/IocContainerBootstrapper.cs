// Проект: EveHQ.NG.WebApi
// Имя файла: IocContainerBootstrapper.cs
// GUID файла: 11C89282-53E2-4F17-9B3D-A247B4A19082
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 04.01.2018

#region Usings

using Autofac;
using Autofac.Extensions.DependencyInjection;
using EveHQ.NG.WebApi.Characters;
using EveHQ.NG.WebApi.Sso;
using Microsoft.Extensions.DependencyInjection;

#endregion


namespace EveHQ.NG.WebApi.Infrastructure
{
	public sealed class IocContainerBootstrapper
	{
		public IContainer BuildContainer(IServiceCollection services)
		{
			var builder = new ContainerBuilder();

			RegisterServicesOverridableByAspDotNetCore(builder);
			builder.Populate(services);
			RegisterServicesOverridingOnesOfAspDotNetCore(builder);

			return builder.Build();
		}

		/// <remarks>
		/// If you need to register services that should be overrided by ASP.NET Core if specified by it, do register them in this method.
		/// </remarks>
		private void RegisterServicesOverridableByAspDotNetCore(ContainerBuilder builder)
		{
		}

		/// <remarks>
		/// If you need to register services that should overrid services registered by ASP.NET Core do register them in this method.
		/// </remarks>
		private void RegisterServicesOverridingOnesOfAspDotNetCore(ContainerBuilder builder)
		{
			builder.RegisterType<SsoAuthenticator>().As<IOAuthAuthenticator>().SingleInstance();
			builder.RegisterType<PrototypeAuthenticationSecretsStorage>().As<IAuthenticationSecretsStorage>().SingleInstance();
			builder.RegisterType<PrototypeTokenStorage>().As<ITokenStorage>().SingleInstance();
			builder.RegisterType<EsiCharacterApi>().As<ICharactersApi>().SingleInstance();
			builder.RegisterType<AuthenticationNotificationHub>()
					.As<IAuthenticationNotificationService, AuthenticationNotificationHub>().SingleInstance();
			builder.RegisterType<FileLoggedInCharacterRepository>().As<ILoggedInCharacterRepository>().SingleInstance();
		}
	}
}
