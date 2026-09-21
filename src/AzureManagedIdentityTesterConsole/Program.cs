using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using System.IdentityModel.Tokens.Jwt;


internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("AzureManagedIdentityTesterConsole - Hello, World!");

        /*
            new DefaultAzureCredential();   // Only to run code locally.
            new ClientSecretCredential(tenantId, clientId, clientSecret); 
        */

        //var credential = new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential);

        async Task PrintUserIndentityInfo(TokenCredential credential)
        {
            try
            {
                // 1. Request a token for Azure Management API
                var tokenRequestContext = new TokenRequestContext(new[] { "https://azure.com" });
                var tokenResult = await credential.GetTokenAsync(tokenRequestContext, default);

                // 2. Parse the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenResult.Token);

                // 3. Extract identity claims
                // "appid" or "azp" (authorized party) contains the Application/Client ID
                string clientId = jwtToken.Payload.TryGetValue("appid", out var appid) ? appid.ToString() : null
                                  ?? (jwtToken.Payload.TryGetValue("azp", out var azp) ? azp.ToString() : "Unknown");

                string tenantId = jwtToken.Payload.TryGetValue("tid", out var tid) ? tid.ToString() : "Unknown";

                Console.WriteLine($"[Success] ArmClient is running as identity:");
                Console.WriteLine($"Tenant ID: {tenantId}");
                Console.WriteLine($"Client ID / App ID: {clientId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Could not retrieve identity token: {ex.Message}");
            }
        }

        await PrintUserIndentityInfo(credential);

        Console.Read();
        Console.WriteLine("Exit");
    }
}