namespace CallbackFunction
{
    static public class CallbackFunction
    {
        static public void OutputCallback(string output)
        {
            if (output == null)
            {
                // Handle the null value
                return;
            }

            // Print word by word
            string[] words = output.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                Console.Write(words[i]);
                
                // Add space after each word except the last one
                if (i < words.Length - 1)
                {
                    Console.Write(" ");
                }
                
                Thread.Sleep(10);
            }
        }
    }
}