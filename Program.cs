using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace MyProgram
{
    class Program
    {
        /// <summary>
        /// Ассинхронная функция чтения текста
        /// </summary>
        /// <returns>Текст из файла</returns>
        static async Task<string> ReadText()
        {
            string path = @"text.txt";
            string text;

            /// Использование объекта класс StreamReader для чтения из текстового файла, и функции ReadToEndAsync ассинхронно считывающую текст
            using (StreamReader stream = new StreamReader(path))
            {
                text = await stream.ReadToEndAsync();
            }

            return text;
        }

        /// <summary>
        /// Функция принимающая на вход текст, разделяющая его на триплеты и выводящая самые часто встречаемые
        /// </summary>
        /// <param name="textObj">текст из файла</param>
        static void CropText(object textObj)
        {
            /// Приведение объекта к строке, ведь параметризированный поток не может работать со строками
            string text = (string)textObj;

            string[] symbols = text.Split();

            List<string> triplets = new List<string>();

            /// Разделение текста на триплеты
            for (int j = 0; j < symbols.Length; j++)
            {
                text = symbols[j];
                for (int i = 0; i < text.Length - 2; i++)
                {
                    triplets.Add(text.Substring(i, 3));
                }
            }

            /// Объединение строк LINQ запроса, который группирует элементы по ключу и упорядочивает элементы по убыванию, вывода 10 самых встречаемых триплетов и их количество
            string result = string.Join
                (
                    "\n",
                    triplets
                        .GroupBy(str => str)
                        .OrderByDescending(gr => gr.Count()).Take(10).Select(gr => $"\"{gr.Key}\" было {gr.Count()} раз")
                );
            Console.WriteLine(result);
        }

        /// <summary>
        /// Главная функция определяющая время работы программы
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            /// Объект класса Stopwatch, использующийся для работы со временм
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string text  = await ReadText();

            Console.WriteLine(text);

            /// Создание параметризированного потока для функции работы с текстом
            Thread cropThread = new Thread(new ParameterizedThreadStart(CropText));
            cropThread.Start(text);

            stopWatch.Stop();

            /// Вывод времени в коносоль в формале: минуты, секунды, миллисекунды
            TimeSpan timespan = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00} минуты, {1:00} секунды, {2:000} миллисекунды", timespan.Minutes, timespan.Seconds, timespan.Milliseconds);
            Console.WriteLine("\nProgram execution time: " + elapsedTime + "\n");

            Console.ReadKey();
        }
    }
}