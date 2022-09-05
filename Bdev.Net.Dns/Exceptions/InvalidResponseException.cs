#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
//

#endregion

using System.Runtime.Serialization;

namespace Bdev.Net.Dns.Exceptions
{
    /// <summary>Thrown when the server delivers a response we are not expecting to hear.</summary>
    [Serializable]
    public class InvalidResponseException : Exception
    {
        const string Msg = "Something grim has happened, we can't continue...";
        public InvalidResponseException() : base(Msg) { /* empty */ }
        public InvalidResponseException(Exception innerException) : base(Msg, innerException) { /* empty */ }
        public InvalidResponseException(string message, Exception innerException) : base(message, innerException) { /* empty */ }
        protected InvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { /* empty */ }
    }
}
