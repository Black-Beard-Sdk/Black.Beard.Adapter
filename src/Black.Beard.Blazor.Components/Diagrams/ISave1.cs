namespace Bb.Diagrams
{
    public interface ISave<T> : ISave
    {

        void SetSave(Action<T> save);
        

    }

}
