using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Wordle_Project_Final
{
    public class GetWordsFile
    {
        public GetWordsFile()
        {
            Download();
        }
        public static async Task Download()
        {
            var path = FileSystem.Current.AppDataDirectory;
            var fullPath = Path.Combine(path, "words.txt");

            string url = "https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt";
            //check for file
            if (!File.Exists(fullPath))
            {
                // no file --> download it and save
                try
                {
                    string content = await DownloadWords(url);
                    SaveToFile(fullPath, content);
                    await Shell.Current.DisplayAlert("Game saved", "File has been saved", "Ok");
                }
                catch (Exception)
                {
                    await Shell.Current.DisplayAlert("Error", "File could not save", "Error");
                }
            }
        }

        static async Task<string> DownloadWords(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        static void SaveToFile(string filepath, string content)
        {
            System.IO.File.WriteAllText(filepath, content);
        }
    }



}
