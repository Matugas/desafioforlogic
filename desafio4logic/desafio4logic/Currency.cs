using System;
using Newtonsoft.Json;

public class Currency
{
    public string id { get; set; }
    [JsonProperty(PropertyName = "currencyName")]
    public string name { get; set; }
    [JsonProperty(PropertyName = "currencySymbol")]
    public string symbol { get; set; }

	public Currency()
	{
	}
}
