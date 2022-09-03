#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     A Request logically consists of a number of questions to ask the DNS Server. Create a request and
    ///     add questions to it, then pass the request to Resolver.Lookup to query the DNS Server. It is important
    ///     to note that many DNS Servers DO NOT SUPPORT MORE THAN 1 QUESTION PER REQUEST, and it is advised that
    ///     you only add one question to a request. If not ensure you check Response.ReturnCode to see what the
    ///     server has to say about it.
    /// </summary>
    public class Request
    {
        // A request is a series of questions, an 'opcode' (RFC1035 4.1.1) and a flag to denote
        // whether recursion is required (don't ask..., just assume it is)
        private readonly List<Question> _questions = new List<Question>();

        /// <summary>
        ///     Construct this object with the default values and a List to hold
        ///     the questions as they are added
        /// </summary>
        public Request()
        {
            // default for a request is that recursion is desired and using standard query
            RecursionDesired = true;
            Opcode = Opcode.StandardQuery;

        }

        public static Request Question(Question question)
        {
            return new Request().WithQuestion(question);
        }

        public Request WithQuestion(Question question)
        {
            AddQuestion(question);
            return this;
        }

        public bool RecursionDesired { get; set; }

        public Opcode Opcode { get; set; }

        /// <summary>
        ///     Adds a question to the request to be sent to the DNS server.
        /// </summary>
        /// <param name="question">The question to add to the request</param>
        public void AddQuestion(Question question)
        {
            // abandon if null
            if (question == null) throw new ArgumentNullException(nameof(question));

            // add this question to our collection
            _questions.Add(question);
        }

        /// <summary>
        ///     Convert this request into a byte array ready to send direct to the DNS server
        /// </summary>
        /// <returns></returns>
        public byte[] GetMessage()
        {
            using (var data = new MemoryStream())
            {
                // the id of this message - this will be filled in by the resolver
                data.WriteByte(0);
                data.WriteByte(0);

                // write the bit fields
                data.WriteByte((byte) (((byte) Opcode << 3) | (RecursionDesired ? 0x01 : 0)));
                data.WriteByte(0);

                // tell it how many questions
                unchecked
                {
                    data.WriteByte((byte) (_questions.Count >> 8));
                    data.WriteByte((byte) _questions.Count);
                }

                // the are no requests, name servers or additional records in a request
                data.WriteByte(0);
                data.WriteByte(0);
                data.WriteByte(0);
                data.WriteByte(0);
                data.WriteByte(0);
                data.WriteByte(0);

                // that's the header done - now add the questions
                foreach (Question question in _questions)
                {
                    AddDomain(data, question.Domain);
                    unchecked
                    {
                        data.WriteByte(0);
                        data.WriteByte((byte) question.Type);
                        data.WriteByte(0);
                        data.WriteByte((byte) question.Class);
                    }
                }
                return data.ToArray();
            }
        }

        /// <summary>
        ///     Adds a domain name to the array of bytes. This implementation does not use
        ///     the domain name compression used in the class Pointer - maybe it should.
        /// </summary>
        /// <param name="data">The memory stream to which add</param>
        /// <param name="domainName">the domain name to encode and add to the array</param>
        private static void AddDomain(Stream data, string domainName)
        {
            var splits = domainName.Split('.');
            foreach (var split in splits)
            {
                data.WriteByte((byte)split.Length);
                foreach (var c in split.ToCharArray())
                {
                    data.WriteByte((byte)c);
                }
            }
            // end of domain names
            data.WriteByte(0);
        }
    }
}