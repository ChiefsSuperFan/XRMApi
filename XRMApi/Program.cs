using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;




namespace XRMApi
{
  class Program
  {
    private static IAccount _firstAccount;
    private static AuthenticationResult _authResult = null;
    public static IPublicClientApplication _clientApp;
    private static string _tenant = "organizations";
    private static string _instance = "https://login.microsoftonline.com/";
 
    private static string _url = "";

    private static string _crmUrl = "https://org20b73f4c.crm.dynamics.com";
    private static string _version = "9.2";
     private static string _clientID = "6212365b-75ec-4c9b-86de-e8636790a7a4";
    private const string DEF_SCOPE = "/.default";
    
  
    static void Main(string[] args)
    {
      try
      {
        var connect = Task.Run(async () => await Authenticate());
        Task.WaitAll(connect);

        if(_authResult!=null)
        {
          var runQuery = Task.Run(async () => await QueryAccounts());
          Task.WaitAll(runQuery);
 
        }
       
      }
      catch
      {

      }
    }
    private static async Task Authenticate()
    {
      try
      {
        var builder = PublicClientApplicationBuilder.Create(_clientID)
       .WithAuthority($"{_instance}{_tenant}")
       .WithDefaultRedirectUri();


        _clientApp = builder.Build();

        var x = _clientApp;

        TokenCacheHelper.EnableSerialization(_clientApp.UserTokenCache);

        var z = _clientApp;
        _url = $"{_crmUrl}/api/data/v{_version}/";

    
        var accounts = await _clientApp.GetAccountsAsync();

        _firstAccount = accounts.FirstOrDefault();

        string scope1 = _crmUrl + DEF_SCOPE;
        string[] scopes = { scope1 };

        _authResult = await _clientApp.AcquireTokenInteractive(scopes)
      .WithAccount(_firstAccount)
      .WithPrompt(Prompt.SelectAccount)
      .ExecuteAsync();


      }
      catch(Exception ex)
      {

      }
    }



    private static async Task QueryAccounts()
    {
      try
      {
        string queryParams = "/api/data/v9.1/accounts?$select=accountcategorycode,accountclassificationcode,accountid," +
          "accountnumber,accountratingcode,address1_addressid,address1_addresstypecode,address1_city,address1_country," +
          "address1_county,address1_fax,address1_freighttermscode,address1_latitude,address1_line1,address1_line2,address1_line3," +
          "address1_longitude,address1_name,address1_postalcode,address1_postofficebox,address1_primarycontactname," +
          "address1_shippingmethodcode,address1_stateorprovince,address1_telephone1,address1_telephone2,address1_telephone3," +
          "address1_upszone,address1_utcoffset,_createdby_value,createdon,_ownerid_value,telephone1";

        string queryURL = _crmUrl + queryParams;

        var httpClient = new System.Net.Http.HttpClient();
        httpClient.BaseAddress = new Uri(_url);
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authResult.AccessToken);

        //var response = httpClient.GetAsync("WhoAmI").Result;
        //string queryURL = "https://org20b73f4c.crm.dynamics.com//api/data/v9.1/accounts?$select=accountid,accountnumber,address1_addressid,address2_city,websiteurl,name";

        var query = await httpClient.GetAsync(queryURL).Result.Content.ReadAsStringAsync();

        

      }
      catch(Exception ex)
      {

      }
    }
  
  }
}
