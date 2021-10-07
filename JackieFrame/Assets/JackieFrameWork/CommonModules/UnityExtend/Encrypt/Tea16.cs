using System;
using UnityEngine;
using System.Collections;
using System.IO;

namespace fs
{
    public class Tea16
    {
        // 默认的密码
        static uint[] m_defaultKey = new uint[]
        {
            0x3687C5E3,
            0xB7EF3327,
            0xE3791011,
            0x84E2D3BC
        };

        static int ReadInt32(byte[] data, int offset)
        {
            int i = ((data[offset + 3] & 0xFF) << 0)
                + ((data[offset + 2] & 0xFF) << 8)
                + ((data[offset + 1] & 0xFF) << 16)
                + ((data[offset + 0] & 0xFF) << 24);

            return i;
        }

        static void Write(int value, byte[] data, int offset)
        {
            data[offset + 3] = (byte)(value >> 0);
            data[offset + 2] = (byte)(value >> 8);
            data[offset + 1] = (byte)(value >> 16);
            data[offset + 0] = (byte)(value >> 24);
        }

        static void Encrypt(uint[] v, uint[] k)
        {
            uint v0 = v[0], v1 = v[1], sum = 0, i;           /* set up */
            uint delta = 0x9e3779b9;                     /* a key schedule constant */
            uint k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];   /* cache key */
            for (i = 0; i < 32; i++)
            {
                /* basic cycle start */
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            } /* end cycle */

            v[0] = v0;
            v[1] = v1;
        }

        static void Decrypt(uint[] v, uint[] k)
        {
            uint v0 = v[0], v1 = v[1], sum = 0xE3779B90, i;  /* set up */
            uint delta = 0x9e3779b9;                     /* a key schedule constant */
            uint k0 = k[0], k1 = k[1], k2 = k[2], k3 = k[3];   /* cache key */
            for (i = 0; i < 32; i++)
            {
                /* basic cycle start */
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            } /* end cycle */

            v[0] = v0;
            v[1] = v1;
        }

        // 加密，如果lenght不是8的倍数，那么超过的字节不会加密
        public static void Encrypt(byte[] src, int start, int lenght, uint[] k)
        {
            if (k == null)
                k = m_defaultKey;

            lenght = lenght / 8;
            uint[] v = new uint[2] { 0, 0 };
            for (int i = 0; i < lenght; ++i)
            {
                v[0] = (uint)ReadInt32(src, start);
                v[1] = (uint)ReadInt32(src, start + 4);
                Encrypt(v, k);

                Write((int)v[0], src, start);
                Write((int)v[1], src, start + 4);

                start += 8;
            }
        }

        // 解密，如果lenght不是8的倍数，那么超过的字节不会解密
        public static void Decrypt(byte[] src, int start, int lenght, uint[] k)
        {
            if (k == null)
                k = m_defaultKey;

            lenght = lenght / 8;
            uint[] v = new uint[2] { 0, 0 };
            for (int i = 0; i < lenght; ++i)
            {
                v[0] = (uint)ReadInt32(src, start);
                v[1] = (uint)ReadInt32(src, start + 4);

                Decrypt(v, k);

                Write((int)v[0], src, start);
                Write((int)v[1], src, start + 4);

                start += 8;
            }
        }
    }
}