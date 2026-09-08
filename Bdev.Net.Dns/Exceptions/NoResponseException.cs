#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Runtime.Serialization;

namespace Bdev.Net.Dns.Exceptions
{
    /// <summary>
    ///     Thrown when the server does not respond
    /// </summary>
    [Serializable]
    public class NoResponseException : SystemException
    {
        public NoResponseException()
        {
            // no implementation
        }

        // Add standard message-only constructor so callers can throw with an explanatory message.
        public NoResponseException(string message) : base(message)
        {
            // no implementation
        }

        // Preserve existing constructor patterns, but forward a useful message when only an inner exception is provided.
        public NoResponseException(Exception innerException) : base(innerException?.Message ?? "No response from server", innerException)
        {
            // no implementation
        }

        public NoResponseException(string message, Exception innerException) : base(message, innerException)
        {
            // no implementation
        }

#if !NET7_0_OR_GREATER
        protected NoResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            // no implementation
        }
#endif
    }
}
