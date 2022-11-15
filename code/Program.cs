using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace HocusCodus // Note: actual namespace depends on the project name.
{
	internal class Program
	{
		static void Main(string[] args)
		{
			getResponse();
		}

		static async void getResponse() {
			// Swagger
			// https://app-htf-2022.azurewebsites.net/swagger/index.html
			// De httpclient die we gebruiken om http calls te maken
			var client = new HttpClient();

			// De base url die voor alle calls hetzelfde is
			client.BaseAddress = new Uri("https://app-htf-2022.azurewebsites.net");

			// De token die je gebruikt om je team te authenticeren, deze kan je via de swagger ophalen met je teamname + password
			var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiOSIsIm5iZiI6MTY2ODUwMzc1MSwiZXhwIjoxNjY4NTkwMTUxLCJpYXQiOjE2Njg1MDM3NTF9.u_Vw7P1JL9cUX_h3LxVUFZLIt4h3IOQOcZFWksMK0En_OXUaRHmvatBUw78B5itcCWQmwy-ngk6Pu5IY6VJ1Ug";

			// We stellen de token in zodat die wordt meegestuurd bij alle calls, anders krijgen we een 401 Unauthorized response op onze calls
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// De url om de challenge te starten
			var startUrl = "api/path/1/easy/Start";

			// We voeren de call uit en wachten op de response
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await
			// De start endpoint moet je 1 keer oproepen voordat je aan een challenge kan beginnen
			// Krijg je een 403 Forbidden response op je Sample of Puzzle calls? Dat betekent dat de challenge niet gestart is
			var startResponse = await client.GetAsync(startUrl);

			// De url om de sample challenge data op te halen
			var sampleUrl = "api/path/1/easy/Puzzle";

			// We doen de GET request en wachten op de het antwoord
			// De response die we verwachten is een lijst van getallen dus gebruiken we List<int>
			var sampleGetResponse = await client.GetFromJsonAsync<List<int>>(sampleUrl);

			Console.WriteLine(sampleGetResponse);
		}
	}
}
