﻿using System;
using System.Collections.Generic;

namespace SerdesNet
{
    /// <summary>
    /// Stub serializer for representing an empty file
    /// </summary>
    public class EmptySerializer : ISerializer
    {
        public SerializerFlags Flags => SerializerFlags.Read;
        public long Offset => 0;
        public long BytesRemaining => 0;
        public void Dispose() { } 
        public void Comment(string comment, bool inline) { } 
        public void Begin(string name = null) { } 
        public void End() { } 
        public void NewLine() { } 
        public void Seek(long offset) => throw new NotSupportedException();
        public void Assert(bool condition, string message) { } 
        public void Pad(int count, byte value) => throw new NotSupportedException();
        public void Pad(string name, int count, byte value) => throw new NotSupportedException();

        public sbyte Int8(int n, sbyte value, sbyte defaultValue = 0) => throw new NotSupportedException();
        public short Int16(int n, short value, short defaultValue = 0) => throw new NotSupportedException();
        public int Int32(int n, int value, int defaultValue = 0) => throw new NotSupportedException();
        public long Int64(int n, long value, long defaultValue = 0) => throw new NotSupportedException();
        public byte UInt8(int n, byte value, byte defaultValue = 0) => throw new NotSupportedException();
        public ushort UInt16(int n, ushort value, ushort defaultValue = 0) => throw new NotSupportedException();
        public uint UInt32(int n, uint value, uint defaultValue = 0) => throw new NotSupportedException();
        public ulong UInt64(int n, ulong value, ulong defaultValue = 0) => throw new NotSupportedException();

        public sbyte Int8(string name, sbyte value, sbyte defaultValue = 0) => throw new NotSupportedException();
        public short Int16(string name, short value, short defaultValue = 0) => throw new NotSupportedException();
        public int Int32(string name, int value, int defaultValue = 0) => throw new NotSupportedException();
        public long Int64(string name, long value, long defaultValue = 0) => throw new NotSupportedException();
        public byte UInt8(string name, byte value, byte defaultValue = 0) => throw new NotSupportedException();
        public ushort UInt16(string name, ushort value, ushort defaultValue = 0) => throw new NotSupportedException();
        public uint UInt32(string name, uint value, uint defaultValue = 0) => throw new NotSupportedException();
        public ulong UInt64(string name, ulong value, ulong defaultValue = 0) => throw new NotSupportedException();

        public T EnumU8<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();
        public T EnumU16<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();
        public T EnumU32<T>(int n, T value) where T : unmanaged, Enum => throw new NotSupportedException();

        public T EnumU8<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();
        public T EnumU16<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();
        public T EnumU32<T>(string name, T value) where T : unmanaged, Enum => throw new NotSupportedException();

        public Guid Guid(string name, Guid value) => throw new NotSupportedException();
        public byte[] Bytes(string name, byte[] value, int length) => throw new NotSupportedException();
        public string NullTerminatedString(string name, string value) => throw new NotSupportedException();
        public string FixedLengthString(string name, string value, int length) => throw new NotSupportedException();
        public IList<TTarget> List<TTarget>(
            string name, 
            IList<TTarget> list,
            int count,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

        public IList<TTarget> List<TTarget>(
            string name,
            IList<TTarget> list,
            int count,
            int offset,
            SerdesMethod<TTarget> serdes,
            Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

        public IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();

        public IList<TTarget> List<TTarget, TContext>(
            string name,
            IList<TTarget> list,
            TContext context,
            int count,
            int offset,
            SerdesContextMethod<TTarget, TContext> serdes,
            Func<int, IList<TTarget>> initialiser = null) => throw new NotSupportedException();
    }
}