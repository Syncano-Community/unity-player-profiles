using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class TestProfile : Profile {

	[JsonProperty("victories")]
	public int Victories { get; set; }

	[JsonProperty("defeats")]
	public int Defeats { get; set; }

	[JsonProperty("draws")]
	public int Draws { get; set; }

	[JsonProperty("country")]
	public string Country { get; set; }

	[JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
	public SyncanoFile Avatar { get; set; }

	[JsonProperty("trophy_1")]
	public int GoldenTrophies { get; set; }

	[JsonProperty("trophy_2")]
	public int SilverTrophies { get; set; }

	[JsonProperty("trophy_3")]
	public int BronzeTrophies { get; set; }

	public TestProfile() { }

	public TestProfile (int victories, int defeats, string country, SyncanoFile avatar, int goldenTrophies, int silverTrophies, int bronzeTrophies)
	{
		Victories = victories;
		Defeats = defeats;
		Country = country;
		Avatar = avatar;
		GoldenTrophies = goldenTrophies;
		SilverTrophies = silverTrophies;
		BronzeTrophies = bronzeTrophies;
	}
}
