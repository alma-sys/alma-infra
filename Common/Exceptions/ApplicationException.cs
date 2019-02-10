namespace Alma.Common
{
#if NETSTANDARD1_6
    public class ApplicationException : System.Exception
    {
        public ApplicationException(string message) : base(message)
        {

        }
    }
#endif
}