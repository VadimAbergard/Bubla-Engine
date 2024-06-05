using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_Winforms.utils
{
    public class Encryption
    {
        private static string[] randomSymbol = { "q", "w", "e", "r", "t", "y", "u", "i", "o", "p", "[", "]", "a", "s", "d", "f", "g", "h", "j",
            "k", "l", ";", "\"", "z", "x", "c", "v", "b", "n", "m", ",", ".", "/", ">", "<", "!", "@", "?","#","$","%",
            "^","&","*","(",")","-","=","_","+","1","2","3","4","5","6","7","8","9","0","`","~"};

        public static void Encod(string encodingFile, string endFile)
        {
            StreamReader fsRead = new StreamReader(File.Open(encodingFile, FileMode.Open));

            
            string textRead = "";
            Random random = new Random();
            int data;
            while ((data = fsRead.Read()) != -1)
            {
                textRead += (char)data + randomSymbol[random.Next(randomSymbol.Length)];
            }
            fsRead.Close();

            FileStream fsWrite = new FileStream(endFile, FileMode.Create);

            byte[] bytesWrite = Encoding.UTF8.GetBytes(textRead);

            fsWrite.Write(bytesWrite, 0, bytesWrite.Length);

            fsWrite.Close();
        }

        public static void EncodTextAndExport(string text, string endFile)
        {
            string textRead = "";
            Random random = new Random();
            foreach(char _char in text.ToCharArray())
            {
                textRead += _char + randomSymbol[random.Next(randomSymbol.Length)];
            }

            FileStream fsWrite = new FileStream(endFile, FileMode.Create);

            byte[] bytesWrite = Encoding.UTF8.GetBytes(textRead);

            fsWrite.Write(bytesWrite, 0, bytesWrite.Length);

            fsWrite.Close();

            //Console.WriteLine(luaText);
        }

        private static void Dencryp(string dencodingFile, string endFile)
        {
            FileStream fsRead = new FileStream(dencodingFile, FileMode.Open);

            string textRead = "";
            int data = 0;
            int scipSymbol = 0;
            bool scip = endFile.EndsWith(".xml");
            while ((data = fsRead.ReadByte()) != -1)
            {
                if (scip && scipSymbol < 3)
                {
                    scipSymbol++;
                    continue;
                }
                if ((char)(byte)data != '◄' && (char)(byte)data != 'Ω' && (char)(byte)data != 'இ' &&
                    (char)(byte)data != '▻' && (char)(byte)data != '◘' && (char)(byte)data != '♇' &&
                    (char)(byte)data != '︶' && (char)(byte)data != '〡' && (char)(byte)data != '░' &&
                    (char)(byte)data != '♡' && (char)(byte)data != '~' && (char)(byte)data != '@' &&
                    (char)(byte)data != '#' && (char)(byte)data != '%' && (char)(byte)data != '^') textRead += (char)data;
            }
            fsRead.Close();
            textRead = textRead.Replace("`", " ");


            FileStream fsWrite = new FileStream(endFile, FileMode.Create);

            byte[] bytesWrite = Encoding.UTF8.GetBytes(textRead);
            string finalText = "";
            for (int i = 0; i < bytesWrite.Length; i++)
            {
                finalText += bytesWrite[i];
                fsWrite.WriteByte(bytesWrite[i]);
            }
            fsWrite.Close();
        }
    }
}
