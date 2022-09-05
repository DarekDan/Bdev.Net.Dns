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
    /// <summary>Thrown when the server does not respond.</summary>
    [Serializable]
    public class NoResponseException : Exception
    {
        public NoResponseException() : base("No answer from the DNS server during the stipulated timeout period.") { /* empty */ }
        public NoResponseException(Exception innerException) : base(message: null, innerException) { /* empty */ }
        public NoResponseException(string message, Exception innerException) : base(message, innerException) { /* empty */ }
        protected NoResponseException(SerializationInfo info, StreamingContext context) : base(info, context) { /* empty */ }
    }
}
