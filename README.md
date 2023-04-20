#Mock Stock API

This is an API I created to return stock information, created to match the schema of the following API: https://iexcloud.io/docs/core/HISTORICAL_PRICES

Running the project will create the API on a local host and open a Swagger window to let you test the different method options.

## GET Daily Returns for Stock with Date Range
{url}/api/StockHistory/DailyReturns/{symbol}
  Optional Parameters:
    fromDate - First date of range to return info on (If no date is passed in, assumed first day of current year)
    toDate - Last date of range to return info on (If no date is passed in, assume the current date)

Range must be 5 years or less

##Example of how to connect to this API (C#):

```
public async Task<DailyReturnsData[]> getDailyReturns(string symbol, string? fromDate = null, string? toDate = null)
{
  DailyReturnsData[] dailyReturns;
  string URL = "https://localhost:7131/api/StockHistory/DailyReturns/" + symbol;
  NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
  if (fromDate != null){
    queryString.Add("fromDate", fromDate);
  }
  if (toDate != null){
    queryString.Add("toDate", toDate);
  }
  URL += (queryString.ToString().Length != 0 ? "?" : "") + queryString.ToString();

  using (var client = new HttpClient())
  {
    var request = new HttpRequestMessage(HttpMethod.Get, @URL);
    var response = await client.SendAsync(request);
    try
    {
      response.EnsureSuccessStatusCode();
    }
    catch (Exception e)
    {
      Console.WriteLine("Error occured: " + e.Message);
      throw new Exception("Failed to get data, " + e.Message);
    }
    string jsonString = await response.Content.ReadAsStringAsync();
    dailyReturns = JsonConvert.DeserializeObject<DailyReturnsData[]>(jsonString);
  }
  return dailyReturns;
}
```
##Example Request Body for POST
```
{
    "close": 278.8,
    "fclose": 278.8,
    "fhigh": 291.6,
    "flow": 286.16,
    "fopen": 289.93,
    "fvolume": 23836223,
    "high": 291.6,
    "low": 286.16,
    "open": 289.93,
    "priceDate": "2023-04-19",
    "symbol": "MSFT",
    "uclose": 278.8,
    "uhigh": 291.6,
    "ulow": 286.16,
    "uopen": 289.93,
    "uvolume": 23836223,
    "volume": 23836223,
    "id": "HISTORICAL_PRICES",
    "key": "MSFT",
    "subkey": "",
    "date": 1681689600000,
    "updated": 1681783247000
  }
```
