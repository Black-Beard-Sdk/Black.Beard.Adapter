namespace Bb.Diagrams
{
    public interface IEventArgInterceptor<T>
        where T : EventArgs
    {

        public void Invoke(object sender, T eventArgs);

    }

}
