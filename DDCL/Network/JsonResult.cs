namespace DDCL.Network
{
	public class JsonResult
	{
		public bool success;
		public string message;
		public int tryCount;

		public JsonResult(bool success, string message, int tryCount = 0)
		{
			this.success = success;
			this.message = message;
			this.tryCount = tryCount;
		}
	}
}