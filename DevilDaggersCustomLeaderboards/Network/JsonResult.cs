namespace DevilDaggersCustomLeaderboards.Network
{
	public class JsonResult
	{
		public bool success;
		public string message;

		public JsonResult(bool success, string message)
		{
			this.success = success;
			this.message = message;
		}
	}
}