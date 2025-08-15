namespace SerdesNet;

/// <summary>
/// Delegate for reading/writing a value of type T as part of a list with some supplied context.
/// </summary>
/// <typeparam name="T">The type of value to read/write</typeparam>
/// <typeparam name="TContext">The type of the context</typeparam>
/// <param name="name">The name of the value in its context</param>
/// <param name="value">The value to use when writing</param>
/// <param name="context">The context value</param>
/// <param name="s">The serdes responsible for reading/writing</param>
/// <returns>The value that was read (or 'value' when writing)</returns>
/// <returns></returns>
public delegate T SerdesContextMethod<T, in TContext>(SerdesName name, T value, TContext context, ISerdes s);