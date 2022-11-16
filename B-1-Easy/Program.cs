using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace HocusCodusB1
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			await getEverything();
		}

		public static String GetAnswer(List<string> test)
		{
			List<char> test2 = new List<char>();		
			string answer = "";

			foreach (string s in test)
			{
				test2.Clear();
				for (int i = 1; i <= s.Distinct().Count(); i++)
				{
					foreach (char c in s)
					{
						if ((s.Count(x => x == c)) == i)
						{
							if (!test2.Contains(c))
							{
								test2.Add(c);
							}
						}
					}
				}
				foreach (char c in test2)
				{
					answer += c.ToString();
				}
				answer += " ";
			}	
			return answer.TrimEnd();
		}
		static async Task getEverything()
		{
			// Swagger
			// https://app-htf-2022.azurewebsites.net/swagger/index.html
			// De httpclient die we gebruiken om http calls te maken
			var client = new HttpClient();

			// De base url die voor alle calls hetzelfde is
			client.BaseAddress = new Uri("https://app-htf-2022.azurewebsites.net");

			// De token die je gebruikt om je team te authenticeren, deze kan je via de swagger ophalen met je teamname + password
			var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiOSIsIm5iZiI6MTY2ODU5OTI0MCwiZXhwIjoxNjY4Njg1NjQwLCJpYXQiOjE2Njg1OTkyNDB9.axNkaFhLt6GFWtLvrLsmJ_Qo2GalaBl-6w_tpuGUTNqJq8z18tpSpTBMosMskMjrb4xiCagudx4stFXVXUXU-w";

			// We stellen de token in zodat die wordt meegestuurd bij alle calls, anders krijgen we een 401 Unauthorized response op onze calls
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// De url om de challenge te starten
			var startUrl = "api/path/2/easy/Start";

			// We voeren de call uit en wachten op de response
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await
			// De start endpoint moet je 1 keer oproepen voordat je aan een challenge kan beginnen
			// Krijg je een 403 Forbidden response op je Sample of Puzzle calls? Dat betekent dat de challenge niet gestart is
			var startResponse = await client.GetAsync(startUrl);

			// De url om de sample challenge data op te halen
			var sampleUrl = "api/path/2/easy/Sample";

			// We doen de GET request en wachten op de het antwoord
			// De response die we verwachten is een lijst van Strings dus gebruiken we List<String>
			var sampleGetResponse = await client.GetFromJsonAsync<List<string>>(sampleUrl);


			var sampleAnswer = GetAnswer(sampleGetResponse);

			// We sturen het antwoord met een POST request
			// Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// De response die we krijgen zal ons zeggen of ons antwoord juist was
			var samplePostResponse = await client.PostAsJsonAsync<string>(sampleUrl, sampleAnswer);

			// Om te zien of ons antwoord juist was moeten we de response uitlezen
			// Een 200 status code betekent dus niet dat je antwoord juist was!
			var samplePostResponseValue = await samplePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(samplePostResponseValue);

			// // De url om de puzzle challenge data op te halen
			var puzzleUrl = "api/path/2/easy/Puzzle";
			// // We doen de GET request en wachten op de het antwoord
			// // De response die we verwachten is een lijst van strings dus gebruiken we List<String>
			var puzzleGetResponse = await client.GetFromJsonAsync<List<String>>(puzzleUrl);
			// Console.WriteLine(puzzleGetResponse);
			// // Je zoekt het antwoord
			var puzzleAnswer = GetAnswer(puzzleGetResponse);

			// // We sturen het antwoord met een POST request
			// // Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// // De response die we krijgen zal ons zeggen of ons antwoord juist was
			var puzzlePostResponse = await client.PostAsJsonAsync<String>(puzzleUrl, puzzleAnswer);

			// // Om te zien of ons antwoord juist was moeten we de response uitlezen
			// // Een 200 status code betekent dus niet dat je antwoord juist was!
			var puzzlePostResponseValue = await puzzlePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(puzzlePostResponseValue);
		}
	}
}
