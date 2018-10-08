namespace Autocomplete
{
    public interface IAutocomplete
    {
        char[][] NewSearch(out char[] options);
        char[][] filter(char c, out char[] options);
    }
}