namespace Autocomplete
{
    public interface IAutocomplete
    {
        char[][] AllItems { get; }
        char[] InitialOptions { get; }
        char[][] Filter(char c, out char[] options);
        char[][] Search(char[] criteria, out char[] options);
    }
}