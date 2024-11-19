namespace Connexien.Lib.Communication
{
    public interface ICommunication<T> where T : IEmailParameters
    {
        void SendMessage(T communicationType);
    }
}
