namespace DingusGaming.Events
{
	public interface Event
	{
		void startEvent();
		void stopEvent();
		string ToString();
	}
}