using System;
using System.IO;
using UnityEngine;

namespace fs
{
    /// <summary>
    /// 读buff
    /// @author hannibal
    /// @time 2017-11-29
    /// </summary>
    public sealed class BufferReader
    {
        private MemoryStream m_stream = null;
        private BinaryReader m_reader = null;
        private bool m_bigEndian = true;//java服务器使用大端编码

        public MemoryStream stream
        {
            get { return m_stream; }
        }

        public int position
        {
            get { return (int)m_stream.Position; }
        }

        public bool bigEndian
        {
            get { return m_bigEndian; }
            set { m_bigEndian = value; }
        }

        public BufferReader(bool is_big_endian)
        {
            m_stream = new MemoryStream();
            m_reader = new BinaryReader(m_stream);
            m_bigEndian = is_big_endian;
        }

        public void Load(byte[] data)
        {
            Load(data, 0, data.Length);
        }

        public void Load(byte[] data, int index, int size)
        {
            m_stream.Write(data, index, size);
            m_stream.Position = 0;
        }

        public BufferReader Read(ref sbyte value)
        {
            value = (sbyte)m_stream.ReadByte();
            return this;
        }

        public BufferReader Read(ref byte value)
        {
            value = (byte)m_stream.ReadByte();
            return this;
        }

        public BufferReader Read(ref short value)
        {
            value = m_reader.ReadInt16();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref ushort value)
        {
            value = m_reader.ReadUInt16();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref int value)
        {
            value = m_reader.ReadInt32();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref uint value)
        {
            value = m_reader.ReadUInt32();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref long value)
        {
            value = m_reader.ReadInt64();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref ulong value)
        {
            value = m_reader.ReadUInt64();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref float value)
        {
            value = m_reader.ReadSingle();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(ref double value)
        {
            value = m_reader.ReadDouble();
            if (m_bigEndian)
                value = Converter.GetBigEndian(value);
            return this;
        }

        public BufferReader Read(byte[] value, int len)
        {
            Debug.Assert(len >= 0 && len <= value.Length, "读取长度错误:" + len);
            m_reader.Read(value, 0, len);
            return this;
        }

        public BufferReader Read(byte[] value)
        {
            this.Read(value, value.Length);
            return this;
        }

        public BufferReader Read(ref string value, int len)
        {
            byte[] bytes = m_reader.ReadBytes(len);
            value = BitConverter.ToString(bytes);
            return this;
        }

        public BufferReader Read(MemoryStream value, int len)
        {
            byte[] bytes = m_reader.ReadBytes(len);
            value.Write(bytes, 0, len);
            return this;
        } 
    }
}