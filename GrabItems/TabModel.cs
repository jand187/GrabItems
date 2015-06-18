using Newtonsoft.Json;

namespace GrabItems
{
	internal class TabModel
	{
		[JsonProperty("numTabs", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public int NumberOfTabs
		{
			get;
			set;
		}

		[JsonProperty("error")]
		public Error Error
		{
			get;
			set;
		}
	}
}