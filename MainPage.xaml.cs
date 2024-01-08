using System.Reflection.Metadata;

namespace Wordle_Project_Final
{
    public partial class MainPage : ContentPage
    {
        //variables
        private static String path;
        private static String fullPath;
        private static String word;
        private int tries = 0;

        public MainPage()
        {
            InitializeComponent();
            // get list of words
            path = FileSystem.Current.AppDataDirectory;
            fullPath = Path.Combine(path, "words.txt");
            //download words from file
            GetWordsFile download = new GetWordsFile();
        }

        private void ResetGame()
        {
            // Resets vars
            word = null;
            tries = 0;

            // Clears textbox for new input
            UserInput.Text = "-----";

            // Reset labels and backgrounds in the grid
            foreach (Frame frame in GameGrid.Children.OfType<Frame>())
            {
                frame.BackgroundColor = Colors.Black;
                frame.BorderColor = Color.FromArgb("#bbbbbf");
                Label boxes = frame.Content as Label;
                if (boxes != null)
                {
                    boxes.Text = " ";
                }
            }
            // Disable entry field
            UserInput.IsVisible = false;
            UserInput.IsEnabled = false;

            // Disable enter button
            EnterButton.IsVisible = false;
            EnterButton.IsEnabled = false;

            // Enable new wordle button
            NewWord.IsVisible = true;
            NewWord.IsEnabled = true;

            // Disable reset button
            ResetButton.IsVisible = false;
            ResetButton.IsEnabled = false;
        }
        private void ResetGame_Clicked(object sender, EventArgs e)
        {
            //reset and generate new word from ResetGame()
            ResetGame();
        }

        string CheckInput(string str)
        {
            if (CheckForWordInFile(fullPath, str))
            {
                return str;
            }
            else
            {
                DisplayAlert("Check word", "Word not in list :(", "Ok");
                return "";
            }
        }

        static string RandWord(string filePath)
        {
            // Read all words in file
            string[] FileLines = File.ReadAllLines(filePath);
            // Random num chooses word
            Random rand = new Random();
            int randIndex = rand.Next(0, FileLines.Length);
            string randomLine = FileLines[randIndex];
            string[] word = randomLine.Split(' ');//separates file lines into individual words
            return word[0];
        }

        bool CheckForWordInFile(string filePath, string userInput)
        {
            // Read words from file
            string[] lines = File.ReadAllLines(filePath);
            //checks if any line = player guess
            return lines.Any(line => line.Equals(userInput));
        }
        private void NewWord_Clicked(object sender, EventArgs e)
        {
            //enable input box
            UserInput.IsVisible = true;
            UserInput.IsEnabled = true;
            //enable enter button
            EnterButton.IsVisible = true;
            EnterButton.IsEnabled = true;
            //disable new word button
            NewWord.IsVisible = false;
            NewWord.IsEnabled = false;
            //Enable reset button
            ResetButton.IsVisible = true;
            ResetButton.IsEnabled = false;
            //get new word
            word = RandWord(fullPath);
            
        }

        //count letters
        static Dictionary<char, int> CountSameLetters(string input)
        {
            Dictionary<char, int> letterInputs = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (char.IsLetter(c))
                {
                    if (letterInputs.ContainsKey(c))
                    {
                        letterInputs[c]++;
                    }
                    else
                    {
                        letterInputs[c] = 1;
                    }
                }
            }
            return letterInputs;
        }

        private async void EnterButton_Clicked(object sender, EventArgs e)
        {
            EnterButton.IsEnabled = false;
            ResetButton.IsEnabled = false;

            //count correct inputs
            bool[] correctLetters = new bool[word.Length];

            //keep of track get letters
            Dictionary<char, int> letterCounts = CountSameLetters(word);
            Dictionary<char, int> userInputLetterCounts = new Dictionary<char, int>();

            //check for valid input
            if (!string.IsNullOrEmpty(UserInput.Text)) 
            {
                string userGuess = UserInput.Text;
                if (CheckInput(userGuess) != "")
                {
                    tries++;
                    // Identify correct letters
                    for (int i = 0; i < word.Length; i++)
                    {
                        char playerLetter = userGuess[i]; // letter from user
                        char wordLetter = word[i]; // letter from word
                        if (playerLetter == wordLetter)
                        {
                            correctLetters[i] = true;
                            letterCounts[wordLetter]--; // Decrement for used letter
                        }
                    }
                    //Loop through all rows
                    for (int row = tries - 1; row < tries; row++)
                    {
                        // Loop through all columns
                        for (int column = 0; column < GameGrid.ColumnDefinitions.Count; column++)
                        {
                            // locate frame for row and column
                            Frame frame = GameGrid.Children.OfType<Frame>().FirstOrDefault(f => Grid.GetRow(f) == row && Grid.GetColumn(f) == column);
                            if (frame != null)
                            {
                                // opens frame label
                                Label label = frame.Content as Label;
                                if (label != null)
                                {
                                    char playerLetter = userGuess[column]; // letter from user
                                    char wordLetter = word[column]; // letter from word
                                    label.Text = playerLetter.ToString();
                                    // Change color depending on letter
                                    if (correctLetters[column])
                                    {
                                        //correct letter ==> green
                                        frame.BackgroundColor = Color.FromArgb("#42f56f");
                                        frame.BorderColor = Color.FromArgb("#0d4d1d");
                                    }
                                    else if (word.Contains(playerLetter.ToString()) && letterCounts[playerLetter] > 0)
                                    {
                                        // Check if letter is in input
                                        if (userInputLetterCounts.ContainsKey(playerLetter) && userInputLetterCounts[playerLetter] > 0)
                                        {
                                            // previous letter ==> red
                                            frame.BackgroundColor = Color.FromArgb("#d11b1b");
                                            frame.BorderColor = Color.FromArgb("#631c21");
                                        }
                                        else
                                        {
                                            //correct letter wrong place ==> yellow
                                            frame.BackgroundColor = Color.FromArgb("#cfae3a");
                                            frame.BorderColor = Color.FromArgb("#c4c22d");
                                            // Increment the count for the used letter in user input
                                            userInputLetterCounts[playerLetter] = 1;
                                        }
                                    }
                                    else
                                    {
                                        // Else color red
                                        frame.BackgroundColor = Color.FromArgb("#d11b1b");
                                    }
                                }
                            }
                        }
                    }
                    // Break if correct guess. Reset game
                    if (correctLetters.All(x => x))
                    {
                        await DisplayAlert("Correct, well done :)", "The word is: " + word + "\nNumber of tries: " + tries, "OK");
                        // Disable entry field
                        UserInput.IsVisible = false;
                        UserInput.IsEnabled = false;
                        // Disable enter button
                        EnterButton.IsVisible = false;
                        EnterButton.IsEnabled = false;
                    }
                    // Break the loop if run out of tries
                    else if (tries == 6)
                    {
                        // Disable enter button
                        EnterButton.IsEnabled = false;
                        EnterButton.IsVisible = false;

                        // Disable entry field
                        UserInput.IsEnabled = false;
                        UserInput.IsVisible = false;

                        await DisplayAlert("Unlucky, try again next time", "The word is: " + word + "\n", "OK");
                    }
                }
            }
            // Enable buttons
            EnterButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }
    }
}