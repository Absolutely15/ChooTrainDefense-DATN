public interface IUnlockObject
{
    int UnlockCost { get; set; }
    void Init();
    void UnlockSuccessfully();
    
}
