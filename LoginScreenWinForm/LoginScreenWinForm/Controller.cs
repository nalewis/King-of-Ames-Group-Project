using Controllers.test;
using Controllers.User;
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
        var response = Helpers.WebMessage(data);
        Console.WriteLine("\nResponse received was :\n{0}", response);
        if (response.Contains("INVALID"))
        {
            return false;
        }
        else
        {
            //Set the static user class to have player information available globally
            User.username = user;
            User.localIp = Helpers.GetLocalIPAddress();
            return true;
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
        Helpers.WebMessage(data);
    }
}
