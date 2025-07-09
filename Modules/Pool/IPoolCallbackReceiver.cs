namespace LazyCoder.Pool
{
    public interface IPoolCallbackReceiver 
    {
        void OnGet();
        void OnRelease();
    }
}
