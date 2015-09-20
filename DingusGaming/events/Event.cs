namespace DingusGaming.Events
{
	public interface Event
	{
		void startEvent();
		void stopEvent();
		string ToString();
	    string countDown(uint secondsLeft);
	}
}