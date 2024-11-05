using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;

var EndPoint = "https://localhost:8201/";
var httpClientHandler = new HttpClientHandler();
httpClientHandler.ServerCertificateCustomValidationCallback =
(message, cert, chain, sslPolicyErrors) => { return true; };

// Initialize one of the several auth methods.
IAuthMethodInfo authMethod =
new TokenAuthMethodInfo("00000000-0000-0000-0000-000000000000");
// Initialize settings. You can also set proxies, custom delegates etc. here.
var vaultClientSettings = new VaultClientSettings(EndPoint, authMethod)
{
    Namespace = "",
    MyHttpClientProviderFunc = handler
    => new HttpClient(httpClientHandler)
    {
        BaseAddress = new Uri(EndPoint)
    }
};
IVaultClient vaultClient = new VaultClient(vaultClientSettings);

// Use client to read a key-value secret.
/* Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2
.ReadSecretAsync(path: "hemmeligheder", mountPoint: "secret");
var minkode = kv2Secret.Data.Data["MinKode"];
Console.WriteLine($"MinKode: {minkode}"); */


// Extra Opgave
Secret<SecretData> kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2
.ReadSecretAsync(path: "nye-hemmeligheder", mountPoint: "secret");
var nyKode = kv2Secret.Data.Data["NyKode"];
Console.WriteLine($"NyKode: {nyKode}");

                // Del 1: Hvor benytter vi path og mount oprettet i opgave 4?
/*
    **Tilslutning til Vault-serveren:**
 * 
 * Klient-applikationen opretter forbindelse til Vault-serveren ved at 
 * instantiere et `VaultClient`-objekt. Objektet tager `VaultClientSettings` som 
 * argument.
 * 
 * `VaultClientSettings` indeholder:
 *  - **EndPoint:** URL'en til Vault-serveren (f.eks., "https://localhost:8201/").
 *  - **AuthMethod:** Godkendelsesmetode, her en token.
 *  - **Namespace:** Navnerum i Vault (tomt i denne opgave).
 *  - **MyHttpClientProviderFunc:** Funktion til at oprette en `HttpClient`, 
 *    her med bypass af validering af self-signed certifikat.
 */

                // Del 2: Path og Mount

/*
    **Brug af path og mount:**
 * 
 * I opgave B defineres hemmeligheden med **path** "hemmeligheder" under 
 * **mount** "secret".
 * 
 * For at hente hemmeligheden, bruges `ReadSecretAsync` med:
 *  - `path`: "hemmeligheder"
 *  - `mountPoint`: "secret"
 * 
 * Dette sikrer at den korrekte hemmelighed hentes fra Vault.
 */

                // Del 3: Hvordan findes MinKode fra det hentede data?

/*
    **Uddrag af MinKode:**
 * 
 * Hemmeligheden gemmes i `kv2Secret`-variablen, der indeholder et 
 * `SecretData`-objekt. `SecretData` indeholder key/value-parrene.
 * 
 * For at udtrække værdien af "MinKode" bruges:
 *  - `kv2Secret.Data.Data["MinKode"]`
 * 
 * `Data` er en dictionary i `SecretData`, og "MinKode" er nøglen til den 
 * ønskede værdi.
 */