namespace FoundryReports.Core.Source
{
    public interface IAddAndRemove<T>
    {
        T AddItem();

        void Remove(T item);
    }
}