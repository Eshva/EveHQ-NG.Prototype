// Проект: EveHQ.NG.WebApi
// Имя файла: HttpService.cs
// GUID файла: B4E1470E-9A28-4422-910F-3A96EBA27B83
// Автор: Mike Eshva (mike@eshva.ru)
// Дата создания: 21.01.2018

#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

#endregion


namespace EveHQ.NG.WebApi.Infrastructure
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Constracted by IoC-container.")]
	public class HttpService : IHttpService
	{
		public async Task<TResult> CallAsync<TResult>(
			HttpMethod httpMethod,
			Func<string> getUri,
			Func<HttpResponseMessage, Task<TResult>> prepareResult) =>
			await CallAsync(() => new HttpRequestMessage(httpMethod, getUri()), prepareResult);

		public async Task CallAsync(
			HttpMethod httpMethod,
			Func<string> getUri,
			Func<HttpResponseMessage, Task> prepareResult) =>
			await CallAsync(() => new HttpRequestMessage(httpMethod, getUri()), prepareResult);

		public async Task<TResult> CallAsync<TResult>(
			Func<HttpRequestMessage> createRequest,
			Func<HttpResponseMessage, Task<TResult>> prepareResult)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = createRequest())
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						return await prepareResult(response);
					}
				}
			}
		}

		public async Task CallAsync(
			Func<HttpRequestMessage> createRequest,
			Func<HttpResponseMessage, Task> prepareResult)
		{
			using (var httpClient = new HttpClient())
			{
				using (var request = createRequest())
				{
					using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
					{
						await prepareResult(response);
					}
				}
			}
		}
	}
}
