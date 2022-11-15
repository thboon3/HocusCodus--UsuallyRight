using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace HocusCodusA2
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			await getEverything();
		}

		public static String GetAnswer(BattleSimObject battleSimObject)
		{
			do
			{
				Wizard firstWizardA = battleSimObject.TeamA.First();
				Wizard firstWizardB = battleSimObject.TeamB.First();
				if (firstWizardA.Speed > firstWizardB.Speed)
				{
					firstWizardB.Health -= firstWizardA.Strength;
					if (firstWizardB.Health <= 0)
					{
						battleSimObject.TeamB.RemoveAt(firstWizardB);
					} else {
						firstWizardA.Health -= firstWizardB.Strength;
					}
				} else {
					firstWizardA.Health -= firstWizardB.Strength;
					if (firstWizardA.Health <= 0)
					{
						battleSimObject.TeamA.Remove(firstWizardA);
					} else {
						firstWizardB.Health -= firstWizardA.Strength;
					}
				}
			} while (battleSimObject.TeamA.Count != 0 && battleSimObject.TeamB.Count != 0 );
			if (battleSimObject.TeamA.Any()) {
				return(nameof(battleSimObject.TeamA));
			} else {
				return(nameof(battleSimObject.TeamB));
			}
			// var any = battleSimObject.TeamA.Any(wizard => wizard.Health > 10);

			// battleSimObject.TeamA.Remove(firstWizardA);
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
			var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiOSIsIm5iZiI6MTY2ODUwMzc1MSwiZXhwIjoxNjY4NTkwMTUxLCJpYXQiOjE2Njg1MDM3NTF9.u_Vw7P1JL9cUX_h3LxVUFZLIt4h3IOQOcZFWksMK0En_OXUaRHmvatBUw78B5itcCWQmwy-ngk6Pu5IY6VJ1Ug";

			// We stellen de token in zodat die wordt meegestuurd bij alle calls, anders krijgen we een 401 Unauthorized response op onze calls
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			// De url om de challenge te starten
			var startUrl = "api/path/1/medium/Start";

			// We voeren de call uit en wachten op de response
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await
			// De start endpoint moet je 1 keer oproepen voordat je aan een challenge kan beginnen
			// Krijg je een 403 Forbidden response op je Sample of Puzzle calls? Dat betekent dat de challenge niet gestart is
			var startResponse = await client.GetAsync(startUrl);

			// De url om de sample challenge data op te halen
			var sampleUrl = "api/path/1/medium/Sample";

			// We doen de GET request en wachten op de het antwoord
			// De response die we verwachten is een lijst van Strings dus gebruiken we List<String>
			var sampleGetResponse = await client.GetFromJsonAsync<BattleSimObject>(sampleUrl);

			var sampleAnswer = GetAnswer(sampleGetResponse);

			// // We sturen het antwoord met een POST request
			// // Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// // De response die we krijgen zal ons zeggen of ons antwoord juist was
			var samplePostResponse = await client.PostAsJsonAsync<string>(sampleUrl, sampleAnswer);

			// // Om te zien of ons antwoord juist was moeten we de response uitlezen
			// // Een 200 status code betekent dus niet dat je antwoord juist was!
			var samplePostResponseValue = await samplePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(samplePostResponseValue);

			// // De url om de puzzle challenge data op te halen
			var puzzleUrl = "api/path/1/medium/Puzzle";
			// // We doen de GET request en wachten op de het antwoord
			// // De response die we verwachten is een lijst van strings dus gebruiken we List<String>
			var puzzleGetResponse = await client.GetFromJsonAsync<BattleSimObject>(puzzleUrl);
			// Console.WriteLine(puzzleGetResponse);
			// // Je zoekt het antwoord
			var puzzleAnswer = GetAnswer(puzzleGetResponse);

			// // We sturen het antwoord met een POST request
			// // Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// // De response die we krijgen zal ons zeggen of ons antwoord juist was
			var puzzlePostResponse = await client.PostAsJsonAsync<string>(puzzleUrl, puzzleAnswer);

			// // Om te zien of ons antwoord juist was moeten we de response uitlezen
			// // Een 200 status code betekent dus niet dat je antwoord juist was!
			var puzzlePostResponseValue = await puzzlePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(puzzlePostResponseValue);
		}
	}
}
