using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

public class LoginController
{
    private string user = "";
    private string pass = "";

	public LoginController(string username, string password)
	{
        user = username;
        pass = password;
	}

    //Asks the webserver to create a new user with the given info
    public bool login()
    {
        NameValueCollection data = new NameValueCollection();
        //COMMAND is what the php looks for to determine it's actions
        data.Add("COMMAND", "login");
        data.Add("name", user);
        data.Add("pass", pass);
        using (WebClient wc = new WebClient())
        {
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
            var encresult = Encoding.ASCII.GetString(result);
            Console.WriteLine("\nResponse received was :\n{0}", encresult);
            if (encresult.Contains("INVALID"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

public class NewUserController
{
    private string user = "";
    private string pass = "";

    public NewUserController(string username, string password)
    {
        user = username;
        pass = password;
    }

    //Asks the webserver to create a new user with the given info
    public void createUser()
    {
        NameValueCollection data = new NameValueCollection();
        //COMMAND is what the php looks for to determine it's actions
        data.Add("COMMAND", "createUser");
        data.Add("name", user);
        data.Add("pass", pass);
        using (WebClient wc = new WebClient())
        {
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            var result = wc.UploadValues("http://proj-309-yt-01.cs.iastate.edu/login.php", "POST", data);
            Console.WriteLine("\nResponse received was :\n{0}", Encoding.ASCII.GetString(result));
        }
    }
}
