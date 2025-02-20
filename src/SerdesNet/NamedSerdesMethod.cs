﻿namespace SerdesNet;

/// <summary>
/// Delegate for reading/writing a value of type T.
/// </summary>
/// <typeparam name="T">The type of value to read/write</typeparam>
/// <param name="name">The name of the value in its context</param>
/// <param name="value">The value to use when writing</param>
/// <param name="s">The serdes responsible for reading/writing</param>
/// <returns>The value that was read (or 'value' when writing)</returns>
/// <returns></returns>
public delegate T NamedSerdesMethod<T>(string name, T value, ISerdes s);