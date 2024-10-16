public class ModalState
{
    public bool IsModalVisible { get; private set; } = false;

    public event Action OnChange;

    public void ShowModal()
    {
        IsModalVisible = true;
        NotifyStateChanged();
    }

    public void HideModal()
    {
        IsModalVisible = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
