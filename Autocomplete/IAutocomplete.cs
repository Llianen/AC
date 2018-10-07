namespace Autocomplete
{
    public interface IAutocomplete
    {
        string[] start(out char[] options);
        string[] filter(char c, out char[] options);
        string[] filterByString(string s, out char[] options);
    }
}