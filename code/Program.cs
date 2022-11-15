using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace HocusCodus
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			await getEverything();
		}

		public static int RomanToIntSum(List<String> lijst)
		{
			int som = 0;
			Dictionary<char, int> romanNumbersDictionary = new() {
				{ 'I', 1 },
				{ 'V', 5 },
				{ 'X', 10 },
				{ 'L', 50 },
				{ 'C', 100 },
				{ 'D', 500 },
				{ 'M', 1000 }
			};
			foreach (string s in lijst)
			{
				for (int i = 0; i < s.Length; i++)
				{
					char currentRomanChar = s[i];
					romanNumbersDictionary.TryGetValue(currentRomanChar, out int num);
					if (i + 1 < s.Length && romanNumbersDictionary[s[i + 1]] > romanNumbersDictionary[currentRomanChar])
					{ som -= num; }
					else { som += num; }
				}
			}
			return som;
		}
		public static string IntToRoman(int num)
		{
			string romanResult = string.Empty;
			string[] romanLetters = {
				"M",
				"CM",
				"D",
				"CD",
				"C",
				"XC",
				"L",
				"XL",
				"X",
				"IX",
				"V",
				"IV",
				"I"
			};
			int[] numbers = {
			1000,
			900,
			500,
			400,
			100,
			90,
			50,
			40,
			10,
			9,
			5,
			4,
			1
			};
			int i = 0;
			while (num != 0)
			{
				if (num >= numbers[i])
				{
					num -= numbers[i];
					romanResult += romanLetters[i];
				}
				else
				{
					i++;
				}
			}
			return romanResult;
		}

		public static String GetAnswer(List<String> lijst)
		{
			List<String> rCijfers = new List<string>();
			foreach (String item in lijst)
			{
				rCijfers.Add(item);
			}
			int num = RomanToIntSum(rCijfers);
			string answer = IntToRoman(num);
			return answer;
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
			var startUrl = "api/path/1/easy/Start";

			// We voeren de call uit en wachten op de response
			// https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/await
			// De start endpoint moet je 1 keer oproepen voordat je aan een challenge kan beginnen
			// Krijg je een 403 Forbidden response op je Sample of Puzzle calls? Dat betekent dat de challenge niet gestart is
			var startResponse = await client.GetAsync(startUrl);

			// De url om de sample challenge data op te halen
			var sampleUrl = "api/path/1/easy/Sample";

			// We doen de GET request en wachten op de het antwoord
			// De response die we verwachten is een lijst van getallen dus gebruiken we List<int>
			var sampleGetResponse = await client.GetFromJsonAsync<List<string>>(sampleUrl);

			var sampleAnswer = GetAnswer(sampleGetResponse);

			// We sturen het antwoord met een POST request
			// Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// De response die we krijgen zal ons zeggen of ons antwoord juist was
			var samplePostResponse = await client.PostAsJsonAsync<String>(sampleUrl, sampleAnswer);

			// Om te zien of ons antwoord juist was moeten we de response uitlezen
			// Een 200 status code betekent dus niet dat je antwoord juist was!
			var samplePostResponseValue = await samplePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(samplePostResponseValue);

			// De url om de puzzle challenge data op te halen
			var puzzleUrl = "api/path/1/easy/Puzzle";
			// We doen de GET request en wachten op de het antwoord
			// De response die we verwachten is een lijst van getallen dus gebruiken we List<int>
			var puzzleGetResponse = await client.GetFromJsonAsync<List<String>>(puzzleUrl);
			Console.WriteLine(puzzleGetResponse);
			// Je zoekt het antwoord
			var puzzleAnswer = GetAnswer(puzzleGetResponse);

			// We sturen het antwoord met een POST request
			// Het antwoord dat we moeten versturen is een getal dus gebruiken we int
			// De response die we krijgen zal ons zeggen of ons antwoord juist was
			var puzzlePostResponse = await client.PostAsJsonAsync<String>(puzzleUrl, puzzleAnswer);

			// Om te zien of ons antwoord juist was moeten we de response uitlezen
			// Een 200 status code betekent dus niet dat je antwoord juist was!
			var puzzlePostResponseValue = await puzzlePostResponse.Content.ReadAsStringAsync();

			Console.WriteLine(puzzlePostResponseValue);
		}
	}
}
