namespace DingusGaming.Events
{
	public interface Event
	{
		public void startEvent();
		public void stopEvent();
		public string toString();
	}
}