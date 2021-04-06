using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace game
{
    class Program
    {
        static void Main(string[] args)
        {
            checkArg(args);
        }

        static void checkArg(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("\nArguments entered incorrectly!!! There must be 3 arguments or more...\nExample: rock paper scissors");
            }
            else if (args.Length % 2 == 0)
            {
                Console.WriteLine("\nThere must be an unpaired number of arguments!!!\nExample: \"rock paper scissors\" or \" rock paper scissors lizard spock\"");
            }
            else
            {
                for (int i = 0; i < args.Length-1; i++)
                {
                    for (int j = i + 1; j < args.Length; j++) {
                        if (args[i] == args[j])
                        {
                            Console.WriteLine("\nArguments must not be repeated!!!\nExample: \"argument1\", \"argument2\", \"argument3\"");
                            return;
                        }      
                    }               
                }
                startGame(args);
            }
        }

        static void startGame(string[] args) {

            int value = selectValue(args);
            int userSelectParse = 0;
            int resultGame = 0;
            string secretKey = getKey();
            string hash = getHMAC(secretKey, args[value]);
          

            bool success = true;

            Console.WriteLine($"\nHMAC: {hash}\nAvailable moves:");

            do
            {
                int n;
                Console.WriteLine();

                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {args[i]}");
                }

                Console.WriteLine("0 - exit\n");
                Console.Write("Enter your move: ");
                string userSelect = Console.ReadLine();

                if (int.TryParse(userSelect, out n))
                {
                    userSelectParse = Convert.ToInt32(userSelect);
                    if(userSelectParse == 0)
                    {
                        return;
                    }
                    success = true;
                } else
                {
                    success = false;
                }
          
            } while (userSelectParse >= args.Length + 1 || userSelectParse < 0 || !success);
        

            Console.WriteLine($"\nYour move: {args[userSelectParse - 1]}");
            Console.WriteLine($"Computer move: {args[value]}");

            resultGame = determineWinner(userSelectParse - 1, value, args);

            if(resultGame == 2)
            {
                Console.WriteLine("\nYou win!");
            } else if (resultGame == 1)
            {
                Console.WriteLine("\nYou lose!");
            } else
            {
                Console.WriteLine("\nDraw!");
            }

            Console.WriteLine($"HMAC key: {secretKey}");
        }

        static int selectValue(string[] args)
        {
            Random rndNumber = new Random();
        
            return rndNumber.Next(0, args.Length);
        }

        static string getKey()
        {
            byte[] data = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(data);
            }

            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

        static string getHMAC(string key, string word)
        {
            byte[] bkey = Encoding.Default.GetBytes(key);
            using (var hmac = new HMACSHA256(bkey))
            {
                byte[] bstr = Encoding.Default.GetBytes(word);
                var bhash = hmac.ComputeHash(bstr);
                return BitConverter.ToString(bhash).Replace("-", string.Empty);
            }
        }

        static int determineWinner(int userSelect, int computerSelect, string[] args)
        {
            int moves = args.Length / 2;
            List<string> wins = new List<string>();  


            if (args[userSelect] == args[computerSelect])
            {
                return 0;
            }

            for (int i = 0, increment = userSelect; i < moves; i++)
            {
                if (increment == args.Length - 1)
                {
                    increment = 0;
                } else
                {
                    increment++;
                }

                wins.Add(args[increment]);
            }

            foreach(string str in wins)
            {
                if (str == args[computerSelect])
                {
                    return 1;
                } 
            }

            return 2;
        }
    }
}
