
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DataTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SudokuExchange
{
    public class SudokuExchangeLoader : MonoBehaviour
    {
        private struct FileData
        {
            public FileData(string path)
            {
                Path = path;
                Data = new List<SudokuExchangeData>();
            }

            public string Path;
            public List<SudokuExchangeData> Data;
        }
        
        private struct SudokuExchangeData
        {
            public SudokuExchangeData(string[] strings)
            {
                ID = strings[0];
                PuzzleString = strings[1];
                Difficulty = float.Parse(strings[3]);
            }
            
            public string ID;
            public string PuzzleString;
            public float Difficulty;
        }

        [Header("Folder Location")]
        [SerializeField] private string PuzzleFolderName = "../puzzles/sudoku_exchange";
        
        private string PuzzleFolderString => Path.Combine(Application.dataPath, PuzzleFolderName);
        
        [Header("File Data")]
        [SerializeField] private string EasyFileName = "easy";
        [SerializeField] private string MediumFileName = "medium";
        [SerializeField] private string HardFileName = "hard";
        [SerializeField] private string DiabolicalFileName = "diabolical";
        
        [SerializeField] private bool LoadEasyOnStart = true;
        [SerializeField] private bool LoadMediumOnStart = true;
        [SerializeField] private bool LoadHardOnStart = true;
        [SerializeField] private bool LoadDiabolicalOnStart = true;
        
        private string EasyFilePath => Path.Combine(PuzzleFolderString, EasyFileName) + TEXT_FILE_TYPE;
        private string MediumFilePath => Path.Combine(PuzzleFolderString, MediumFileName) + TEXT_FILE_TYPE;
        private string HardFilePath => Path.Combine(PuzzleFolderString, HardFileName) + TEXT_FILE_TYPE;
        private string DiabolicalFilePath => Path.Combine(PuzzleFolderString, DiabolicalFileName) + TEXT_FILE_TYPE;

        [Header("GUI Assets")]
        [SerializeField] private GameObject LoadingText;
        [SerializeField] private GameObject InterfaceContainer;
        
        [SerializeField] private Button EasyPuzzleButton;
        [SerializeField] private Button MediumPuzzleButton;
        [SerializeField] private Button HardPuzzleButton;
        [SerializeField] private Button DiabolicalPuzzleButton;
        
        [SerializeField] private TMP_Text IDText;
        [SerializeField] private TMP_Text DifficultyText;
        
        private FileData _easyData;
        private FileData _mediumData;
        private FileData _hardData;
        private FileData _diabolicalData;

        private SudokuExchangeData _currentData;

        private const string TEXT_FILE_TYPE = ".txt";

        private int _asyncActionCount = 4;

        private void Awake()
        {
            SudokuEngine.PuzzleComplete += OnPuzzleComplete;

            _easyData = new FileData(EasyFilePath);
            _mediumData = new FileData(MediumFilePath);
            _hardData = new FileData(HardFilePath);
            _diabolicalData = new FileData(DiabolicalFilePath);
            
            EasyPuzzleButton.onClick.AddListener(SolveEasyPuzzle);
            MediumPuzzleButton.onClick.AddListener(SolveMediumPuzzle);
            HardPuzzleButton.onClick.AddListener(SolveHardPuzzle);
            DiabolicalPuzzleButton.onClick.AddListener(SolveDiabolicalPuzzle);
        }
        
        private void Start()
        {
            IDText.text = "";
            DifficultyText.text = "";

            LoadAllPuzzles();
        }

        private void OnDestroy()
        {
            SudokuEngine.PuzzleComplete -= OnPuzzleComplete;
            
            EasyPuzzleButton.onClick.RemoveAllListeners();
            MediumPuzzleButton.onClick.RemoveAllListeners();
            HardPuzzleButton.onClick.RemoveAllListeners();
            DiabolicalPuzzleButton.onClick.RemoveAllListeners();
        }

        private void SolveEasyPuzzle() => SolveRandomPuzzle(_easyData.Data);
        private void SolveMediumPuzzle() => SolveRandomPuzzle(_mediumData.Data);
        private void SolveHardPuzzle() => SolveRandomPuzzle(_hardData.Data);
        private void SolveDiabolicalPuzzle() => SolveRandomPuzzle(_diabolicalData.Data);

        private void LoadAllPuzzles()
        {
            LoadingText.SetActive(true);
            InterfaceContainer.SetActive(false);
            
            if (LoadEasyOnStart)
            {
                LoadPuzzles(_easyData);
            }
            
            if (LoadMediumOnStart)
            {
                LoadPuzzles(_mediumData);
            }
            
            if (LoadHardOnStart)
            {
                LoadPuzzles(_hardData);
            }
            
            if (LoadDiabolicalOnStart)
            {
                LoadPuzzles(_diabolicalData);
            }
        }

        private async void LoadPuzzles(FileData fileData)
        {
            try
            {
                await foreach (SudokuExchangeData data in LoadFileData(fileData.Path))
                {
                    fileData.Data.Add(data);
                }
            
                AsyncLoadFinished();
            }
            catch (Exception e)
            {
                throw new FileLoadException(e.Message, e);
            }
        }

        private async IAsyncEnumerable<SudokuExchangeData> LoadFileData(string fileString)
        {
            if (!File.Exists(fileString))
            {
                Debug.LogError($"File {fileString} not found!");
                yield break;
            }
        
            Debug.Log($"Sudoku Exchange Loading Start: ({fileString})");
        
            string[] lines = await File.ReadAllLinesAsync(fileString);
        
            for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
            {
                string line = lines[lineIndex];

                SudokuExchangeData lineData = new SudokuExchangeData(line.Split(' '));
                yield return lineData;
            }

            Debug.Log($"Sudoku Exchange Loading End: ({fileString})");
        }

        private void AsyncLoadFinished()
        {
            --_asyncActionCount;

            if (_asyncActionCount == 0)
            {
                LoadingText.SetActive(false);
                InterfaceContainer.SetActive(true);
            }
        }

        private void SolveRandomPuzzle(List<SudokuExchangeData> data)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            int puzzleCount = data.Count;
            SudokuExchangeData randomData;
            do
            {
                int randomIndex = Random.Range(0, puzzleCount);
                randomData = data[randomIndex];
            } while (string.IsNullOrWhiteSpace(randomData.PuzzleString));
            
            _currentData = randomData;
            SudokuEngine.SolvePuzzle(randomData.PuzzleString);
        }

        private void OnPuzzleComplete(Puzzle puzzle)
        {
            IDText.text = _currentData.ID;
            DifficultyText.text = _currentData.Difficulty.ToString(CultureInfo.CurrentCulture);
        }
    }
}
