#region Usings

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

#endregion


namespace EveHQ.NG.WebApi
{
	public class Program
	{
		public static void Main(string[] args)
		{
			WebHost.CreateDefaultBuilder(args)
					.UseKestrel()
					.UseStartup<Startup>()
					.Build().Run();
		}
	}
}
