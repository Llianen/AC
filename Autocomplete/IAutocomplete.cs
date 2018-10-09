namespace Autocomplete
{
    public interface IAutocomplete
    {
        char[][] Filter(char c, out char[] options);
        char[][] Search(char[] criteria, out char[] options);
    }
}