// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using static System.Console;

namespace matermind
{
    class Exe
    {
        static IEnumerable<string> VALID_INPUT_KEYS_ARRAY;
        static string Read_GuessKeys()
        {
            string result=null;
            var keys=new ArrayList();
            do
            {
                var k=ReadKey();
                if(k.Key == ConsoleKey.Enter)
                {
                    var text=new StringBuilder();
                    foreach(var c in keys) text.Append(c);
                    result=$"{text}";
                    break;
                }
                if(k.Key == ConsoleKey.Backspace && keys.Count > 0)
                {
                    keys.RemoveAt(keys.Count-1);
                    Write(" \b");
                }
                else if(VALID_INPUT_KEYS_ARRAY.Any(c=>c==$"{k.Key}")) keys.Add(k.KeyChar);
            }while(true);
            return result;
        }
        static IEnumerable<int> Get_GameCode()
        {
            List<int> result=new ();
            do
            {
                var c=System.Security.Cryptography.RandomNumberGenerator.GetInt32(0,10);
                if(!result.Contains(c)) result.Add(c);
            }while(result.Count != 4);
            return result;    
        }
        static string Read_Guess(string Prompt)
        {
            Write(Prompt);
            return Read_GuessKeys();
        }
        static IEnumerable<int> Get_Guess(string guess_input,bool separator)
        {
            IEnumerable<int> result=null;
            if(guess_input?.Length == 4) result=guess_input.Select(c=>Convert.ToInt32($"{c}"));
            return result;
        }
        static (int Whites, int Reds) Get_Hint(IEnumerable<int> guess, IEnumerable<int> toguess)
        {
            int white=0,red=0;
            for(int k=0; k<4; ++k)
            {
                if(guess.ElementAt(k) == toguess.ElementAt(k)) { ++red; }
                else if(toguess.Contains(guess.ElementAt(k))) { ++white; }
            }
            return (white,red);
        }
        static void TopEntryPoint(uint trylimit = 12, bool separator = false)
        {
            WriteLine();
            var toguess = Get_GameCode();
//WriteLine(string.Join(" ",toguess));
            var askguess = "Which are the four numbers? : ";
            int trycount = 0;
            do
            {
                if(trycount >= trylimit) { WriteLine($"\n\nYou reached the attempts limit of {trylimit}: The game won!!!\nThe numbers were {string.Join(" ",toguess)}\n"); break;  }
                var guess_input = Read_Guess(askguess);
                var guess = Get_Guess(guess_input,separator);
                ++trycount;
                if(guess==null) {WriteLine();continue;}
                if(guess.SequenceEqual(toguess)) { WriteLine($"\n\nYou won in {trycount} attempts!!!\n"); break; }
                var trylabel = $"Attempt #{trycount,-2}>";
                var hint = Get_Hint(guess, toguess);
        WriteLine($"\r{askguess}{guess_input} {trylabel} Misplaced: {hint.Whites} Exact: {hint.Reds}");


            }while(true);
        }
        static void Main(string[] args)
        {
            VALID_INPUT_KEYS_ARRAY = System.Enum.GetNames(typeof(System.ConsoleKey)).Where(k=>k.Contains("NumPad") || System.Text.RegularExpressions.Regex.Match(k,@"^D\d$").Success || k=="Spacebar");
            TopEntryPoint();

            //WriteLine(string.Join(" ",VALID_INPUT_KEYS_ARRAY));
            //WriteLine($"\n\n{Read_GuessKeys()}");

            //var k = System.Console.ReadKey();
            //WriteLine($"\n{k.Key}\n");
            //WriteLine(string.Join(" ",Enum.GetNames(typeof(ConsoleKey))));
        }
    }
}