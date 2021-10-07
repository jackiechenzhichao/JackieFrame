using System;
using System.IO;


namespace fs
{
    /// <summary>
    /// 写buff
    /// @author hannibal
    /// @time 2017-11-29
    /// </summary>
    public sealed class BufferWriter
    {
        private MemoryStream m_stream = null;
        private BinaryWriter m_writer = null;
        private bool m_bigEndian = true;//java服务器使用大端编码

        public MemoryStream stream
        {
            get { return m_stream; }
        }

        public bool bigEndian
        {
            get { return m_bigEndian; }
            set { m_bigEndian = value; }
        }

        public BufferWriter(bool is_big_endian)
        {
            m_stream = new MemoryStream();
            m_writer = new BinaryWriter(m_stream);
            m_bigEndian = is_big_endian;
        }

        public BufferWriter(int size)
        {
            m_stream = new MemoryStream(size);
            m_writer = new BinaryWriter(m_stream);
        }

        public BufferWriter Write(sbyte value)
        {
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(byte value)
        {
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(short value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(ushort value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(int value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(uint value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(long value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(ulong value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(float value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(double value)
        {
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            m_writer.Write(value);
            return this;
        }

        public BufferWriter Write(byte[] value, int len)
        {
            m_writer.Write(value, 0, len);
            return this;
        }
        public BufferWriter Write(byte[] value)
        {
            return this.Write(value, value.Length);
        }

        public BufferWriter Write(string value)
        {
            char[] chars = value.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                m_writer.Write((byte)chars[i]);
            }
            return this;
        }

        public void Clear()
        {
            m_stream.Position = 0;
            m_stream.SetLength(0);
        }
    }
}