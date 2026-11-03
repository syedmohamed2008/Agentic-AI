
using SharpToken;

class Program
{
    static void Main()
    {
        string input =
            "Explain Dependency Injection in ASP.NET Core.";

        var encoding =
            GptEncoding.GetEncoding("cl100k_base");

        var tokens = encoding.Encode(input);

        Console.WriteLine("Token Count: " + tokens.Count);

        Console.WriteLine("Token IDs:");

        foreach (var token in tokens)
        {
            var decoded = encoding.Decode(new[] { token });

            Console.WriteLine(
                $"Token ID: {token} | Token Text: {decoded}");
        }
    }
}