public interface IUINonConsumable
{
    int ID { get; set; }
    void SetText(string text);
    void Init();
    void UIOnPurchaseComplete();
}
