using Newtonsoft.Json;

namespace GrabItems
{
	internal class Error
	{
		[JsonProperty("message")]
		public string Message
		{
			get;
			set;
		}
	}
}