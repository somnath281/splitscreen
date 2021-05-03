using System;
using System.IO;

class Param
{
    //Param values and default values
    private string _urlToOpen = "http://fiorilaunchpad.sap.com/";
    private string _defaultBrowser = "chrome";
    private int _timeTakingToOpenBrowser = 5;


    public string urlToOpen { get { return _urlToOpen; } }
    public string defaultBrowser { get { return _defaultBrowser; } }
    public int timeTakingToOpenBrowser { get { return _timeTakingToOpenBrowser; } }
   
    private string _url;
    
    public Param(string url)
    {
        _url = url;

        if (!File.Exists(_url))
        {
            using (StreamWriter writer = new StreamWriter(_url))
            {
                writer.WriteLine("urlToOpen|" + _urlToOpen);
                writer.WriteLine("defaultBrowser|" + _defaultBrowser);
                writer.WriteLine("timeTakingToOpenBrowser|" + _timeTakingToOpenBrowser);
               
            }
        }
        getParamDetails();
    }
    
    private void getParamDetails()
    {
        string paramKey = "";
        string paramValue = "";

        foreach (string strLine in File.ReadAllLines(_url))
        {
            if (strLine == "" || strLine == null)
            {

            }
            else
            {
                paramKey = strLine.Split('|')[0];
                paramValue = strLine.Split('|')[1];

                if (paramKey == "urlToOpen")
                {
                    try
                    {
                        _urlToOpen = paramValue;
                    }
                    catch
                    {
                        throw new Exception("Error occurred while reading parameter '_urlToOpen' value. Parameter should be String.");
                    }

                }
                else if (paramKey == "defaultBrowser")
                {
                    try
                    {
                        _defaultBrowser = paramValue;
                    }
                    catch
                    {
                        throw new Exception("Error occurred while reading parameter '_defaultBrowser' value. Parameter should be String.");
                    }
                }
                else if (paramKey == "timeTakingToOpenBrowser")
                {
                    try
                    {
                        _timeTakingToOpenBrowser = int.Parse(paramValue);
                    }
                    catch
                    {
                        throw new Exception("Error occurred while reading parameter '_timeTakingToOpenBrowser' value. Parameter should be Int.");
                    }
                }
            }
        }
    }
}