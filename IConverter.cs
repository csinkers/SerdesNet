namespace SerdesNet
{
    public interface IConverter<TNumeric, T>
    {
        TNumeric ToNumeric(T memory);
        T FromNumeric(TNumeric persistent);
        string ToSymbolic(T memory);
        T FromSymbolic(string symbolic);
    }
}
