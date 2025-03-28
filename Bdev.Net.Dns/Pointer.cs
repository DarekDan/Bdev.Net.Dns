#region

//
// Bdev.Net.Dns by Rob Philpott, Big Developments Ltd. Please send all bugs/enhancements to
// rob@bigdevelopments.co.uk  This file and the code contained within is freeware and may be
// distributed and edited without restriction.
// 

#endregion

using System;
using System.Linq;
using System.Text;

namespace Bdev.Net.Dns
{
    /// <summary>
    ///     Logical representation of a pointer, but in fact a byte array reference and a position in it. This
    ///     is used to read logical units (bytes, shorts, integers, domain names etc.) from a byte array, keeping
    ///     the pointer updated and pointing to the next record. This type of Pointer can be considered the logical
    ///     equivalent of an (unsigned char*) in C++
    /// </summary>
    internal class Pointer
    {
        // a pointer is a reference to the message and an index
        private readonly byte[] _message;
        private int _position;

        // pointers can only be created by passing on an existing message
        public Pointer(byte[] message, int position)
        {
            _message = message;
            _position = position;
        }

        /// <summary>
        ///     Shallow copy function
        /// </summary>
        /// <returns></returns>
        public Pointer Copy()
        {
            return new Pointer(_message, _position);
        }

        /// <summary>
        ///     Adjust the pointers position within the message
        /// </summary>
        /// <param name="position">new position in the message</param>
        public void SetPosition(int position)
        {
            _position = position;
        }

        /// <summary>
        ///     Gets the cursor position
        /// </summary>
        public int Position
        {
            get { return _position; }
        }

        /// <summary>
        ///     Overloads the + operator to allow advancing the pointer by so many bytes
        /// </summary>
        /// <param name="pointer">the initial pointer</param>
        /// <param name="offset">the offset to add to the pointer in bytes</param>
        /// <returns>a reference to a new pointer moved forward by offset bytes</returns>
        public static Pointer operator +(Pointer pointer, int offset)
        {
            return new Pointer(pointer._message, pointer._position + offset);
        }

        /// <summary>
        ///     Reads a single byte at the current pointer, does not advance pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public byte Peek()
        {
            return _message[_position];
        }

        /// <summary>
        ///     Reads a single byte at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public byte ReadByte()
        {
            return _message[_position++];
        }

        /// <summary>
        ///     Reads two bytes to form a short at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public short ReadShort()
        {
            return (short)((ReadByte() << 8) | ReadByte());
        }

        /// <summary>
        ///     Reads two bytes to form an unsigned short at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public ushort ReadUShort()
        {
            return (ushort)((ReadByte() << 8) | ReadByte());
        }

        /// <summary>
        ///     Reads four bytes to form a int at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public int ReadInt()
        {
            return (ReadByte() << 24 | ReadByte() << 16 | ReadByte() << 8 | ReadByte());
        }

        /// <summary>
        ///     Reads a single byte as a char at the current pointer, advancing pointer
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public char ReadChar()
        {
            return (char)ReadByte();
        }

        /// <summary>
        ///     Reads a domain name from the byte array. The method by which this works is described
        ///     in RFC1035 - 4.1.4. Essentially to minimize the size of the message, if part of a domain
        ///     name already been seen in the message, rather than repeating it, a pointer to the existing
        ///     definition is used. Each word in a domain name is a label, and is preceded by its length
        ///     eg. bigdevelopments.co.uk
        ///     is [15] (size of bigdevelopments) + "bigdevelopments"
        ///     [2]  "co"
        ///     [2]  "uk"
        ///     [1]  0 (NULL)
        /// </summary>
        /// <returns>the byte at the pointer</returns>
        public string ReadDomain()
        {
            var domain = new StringBuilder();
            int length;

            // get  the length of the first label
            while ((length = ReadByte()) != 0)
            {
                // top 2 bits set denotes domain name compression and to reference elsewhere
                if ((length & 0xc0) == 0xc0)
                {
                    // work out the existing domain name, copy this pointer
                    var newPointer = Copy();

                    // and move it to where specified here
                    newPointer.SetPosition(((length & 0x3f) << 8) | ReadByte());

                    // repeat call recursively
                    domain.Append(newPointer.ReadDomain());
                    return domain.ToString();
                }

                // if not using compression, copy a char at a time to the domain name
                while (length > 0)
                {
                    domain.Append(ReadChar());
                    length--;
                }

                // if size of next label isn't null (end of domain name) add a period ready for next label
                if (Peek() != 0) domain.Append('.');
            }

            // and return
            return domain.ToString();
        }

        public void Seek(int offset)
        {
            _position += offset;
        }

        public override string ToString()
        {
            return string.Concat(_message.Select(s => $"{s:x2}"));
        }

        public byte[] ReadBytes(int length)
        {
            var data = new byte[length];

            Buffer.BlockCopy(_message, _position, data, 0, length);
            _position += length;

            return data;
        }
    }
}