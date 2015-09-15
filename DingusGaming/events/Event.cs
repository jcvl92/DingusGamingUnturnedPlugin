namespace DingusGaming.Events
{
    public interface Event
    {
    	public void beginEvent();
    	public void endEvent();
    	public void fireEvent();
    	public void toString();
    }
}