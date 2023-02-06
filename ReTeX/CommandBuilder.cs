namespace ReTeX
{
    public class CommandBuilder
    {
        public static string BuildCompile(string template, string filename, string username)
            => string.Format(template, filename, username);
    }
}
