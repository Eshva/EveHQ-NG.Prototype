#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EveHQ.NG.WebApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

#endregion


namespace EveHQ.NG.WebApi
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constructed by ASP.NET Core")]
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by ASP.NET Core")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		public IConfiguration Configuration { get; }

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Called by ASP.NET Core")]
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddMvc().AddJsonOptions(
				options =>
				{
					//return json format with Camel Case
					options.SerializerSettings.ContractResolver = new DefaultContractResolver();
				});

			_applicationContainer = new IocContainerBootstrapper().BuildContainer(services);
			return new AutofacServiceProvider(_applicationContainer);
		}

		[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Called by ASP.NET Core")]
		public void Configure(
			IApplicationBuilder applicationBuilder,
			IHostingEnvironment hostingEnvironment,
			IApplicationLifetime applicationLifetime)
		{
			if (hostingEnvironment.IsDevelopment())
			{
				applicationBuilder.UseDeveloperExceptionPage();
			}

			applicationBuilder.UseMvc();
			applicationLifetime.ApplicationStopped.Register(() => _applicationContainer.Dispose());
		}

		private IContainer _applicationContainer;
	}
}
