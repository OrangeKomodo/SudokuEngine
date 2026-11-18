using System;
using System.Collections.Generic;
using System.Text;
using DataTypes.Deductions;
using Definitions;
using UnityEngine;

namespace DataTypes
{
    public class Puzzle
    {
        #region State Machine

        public enum PuzzleState
        {
            WaitToStart,
            FindSingles,
            FindPointers,
            GenerateGroups,
            FindXWings,
            // ...
            ResolveGroups,
            Finished
        }
        
        #endregion
        
        #region Private Variables

        // Compile-time variables
        private CellArea[] _rows = new CellArea[9];
        private CellArea[] _columns = new CellArea[9];
        private CellArea[] _twoDimensionalAreas = new CellArea[9];
        private Cell[] _cells = new Cell[81];
    
        // Runtime variables
        private PuzzleState _state = PuzzleState.WaitToStart;
        private List<Deduction> _deductions = new List<Deduction>();
        private Dictionary<string, Group> _groups = new Dictionary<string, Group>(); // This will be something more later
        private Dictionary<string, bool> _pointers = new Dictionary<string, bool>(); // This will be something more later
        private int _stepCount = 0;
    
        #endregion
    
        #region Public Accessors

        public PuzzleState State => _state;
        public Cell[] Cells => _cells;
    
        #endregion
    
        #region Constructors

        public Puzzle()
        {
            CreateStandardPuzzle();
        }

        public Puzzle(string puzzleString)
        {
            CreateStandardPuzzle();
        
            if (string.IsNullOrWhiteSpace(puzzleString))
            {
                return;
            }
        
            puzzleString = FormatPuzzleString(puzzleString);
            PopulatePuzzleWithString(puzzleString);
        }
    
        // TODO: Create a Constructor that takes a custom rule set type and creates a puzzle with custom 2D areas.

        private void CreateStandardPuzzle()
        {
            // Populate the 9 row, column, and square areas
            for (int areaIndex = 0; areaIndex < 9; ++areaIndex)
            {
                _rows[areaIndex] = new CellArea(AreaType.Row, areaIndex);
                _columns[areaIndex] = new CellArea(AreaType.Column, areaIndex);
                _twoDimensionalAreas[areaIndex] = new CellArea(AreaType.Square, areaIndex);
            }

            // Populate the 81 cells, and connect them to the appropriate areas (and vice versa)
            for (int cellIndex = 0; cellIndex < 81; ++cellIndex)
            {
                int rowIndex = StandardPuzzleSettings.GetRowIndex(cellIndex);
                int columnIndex = StandardPuzzleSettings.GetColumnIndex(cellIndex);
                int squareIndex = StandardPuzzleSettings.GetSquareIndex(cellIndex);
            
                Cell cell = new Cell(cellIndex, _rows[rowIndex], _columns[columnIndex], _twoDimensionalAreas[squareIndex]);
                _cells[cellIndex] = cell;
            
                _rows[rowIndex].AddCell(cell);
                _columns[columnIndex].AddCell(cell);
                _twoDimensionalAreas[squareIndex].AddCell(cell);
            }
        }

        private void PopulatePuzzleWithString(string puzzleString)
        {
            for (int cellIndex = 0; cellIndex < puzzleString.Length; ++cellIndex)
            {
                if (cellIndex >= puzzleString.Length)
                {
                    break;
                }
            
                char cellChar = puzzleString[cellIndex];

                if (cellChar == ' ' || cellChar == '0' || cellChar == '?')
                {
                    continue;
                }
            
                SetStarterValue(cellIndex, (Value)(cellChar - '0'));
            }
        }

        // Get rid of extraneous characters
        private string FormatPuzzleString(string puzzleString)
        {
            return puzzleString.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }
    
        #endregion
        
        #region Solve Puzzle Functions
    
        public bool SolvePuzzle()
        {
            int passCount = 0;
            
            do
            {
                Debug.Log($"Solve Pass: {++passCount}");

                switch (_state)
                {
                    case PuzzleState.WaitToStart:
                        _state = PuzzleState.FindSingles;
                        goto case PuzzleState.FindSingles;
                    case PuzzleState.FindSingles:
                        SolvePuzzle_FindSingles();
                        break;
                    case PuzzleState.FindPointers:
                        SolvePuzzle_FindPointers();
                        break;
                    case PuzzleState.GenerateGroups:
                        SolvePuzzle_GenerateGroups();
                        break;
                    case PuzzleState.FindXWings:
                        break;
                    case PuzzleState.ResolveGroups:
                        break;
                    case PuzzleState.Finished:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (CheckSolved())
                {
                    _state = PuzzleState.Finished;
                }

            } while (_state != PuzzleState.Finished && passCount < 100);

            if (passCount >= 100)
            {
                Debug.LogError("Infinite Loop Detected. This should not happen!");
            }
        
            return _state == PuzzleState.Finished;
        }

        private void SolvePuzzle_FindSingles()
        {
            // Check 2D areas for single instances of Values
            for (int tdaIndex = 0; tdaIndex < _twoDimensionalAreas.Length; ++tdaIndex)
            {
                _twoDimensionalAreas[tdaIndex].FindSinglesInArea(ref _deductions);
            }

            if (ImplementDeductions_Singles())
            {
                return;
            }
            
            // Check rows for single instances of Values
            for (int rowIndex = 0; rowIndex < _rows.Length; ++rowIndex)
            {
                _rows[rowIndex].FindSinglesInArea(ref _deductions);
            }

            if (ImplementDeductions_Singles())
            {
                return;
            }
            
            // Check columns for single instances of Values
            for (int columnIndex = 0; columnIndex < _columns.Length; ++columnIndex)
            {
                _columns[columnIndex].FindSinglesInArea(ref _deductions);
            }

            if (ImplementDeductions_Singles())
            {
                return;
            }
            
            // Check for cells with only one possible Value
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                _cells[cellIndex].CheckIfOnlyOnePossibility(ref _deductions);
            }

            if (ImplementDeductions_Singles())
            {
                return;
            }
            
            // If deductions could NOT be found, move on to the next method
            _state = PuzzleState.FindPointers;
        }

        private void SolvePuzzle_FindPointers()
        {
            // Find Pointer Formations
            // These are when all possibilities for a Value within an Area are also within a conjoining Area,
            // meaning that Value is not possible anywhere else within the conjoined Area.
            for (int tdaIndex = 0; tdaIndex < _twoDimensionalAreas.Length; ++tdaIndex)
            {
                _twoDimensionalAreas[tdaIndex].FindPointers(ref _deductions);
            }
            for (int rowIndex = 0; rowIndex < _rows.Length; ++rowIndex)
            {
                _rows[rowIndex].FindPointers(ref _deductions);
            }
            for (int columnIndex = 0; columnIndex < _columns.Length; ++columnIndex)
            {
                _columns[columnIndex].FindPointers(ref _deductions);
            }

            // If at least one actionable deduction could be found, go back to basics
            if (ImplementDeductions_Pointers())
            {
                _state = PuzzleState.FindSingles;
                return;
            }
            
            // If deductions could NOT be found, move on to the next method
            _state = PuzzleState.GenerateGroups;
        }

        private void SolvePuzzle_GenerateGroups()
        {
            // Create Groups
            for (int tdaIndex = 0; tdaIndex < _twoDimensionalAreas.Length; ++tdaIndex)
            {
                _twoDimensionalAreas[tdaIndex].FindGroupsInArea(ref _deductions);
            }
            for (int rowIndex = 0; rowIndex < _rows.Length; ++rowIndex)
            {
                _rows[rowIndex].FindGroupsInArea(ref _deductions);
            }
            for (int columnIndex = 0; columnIndex < _columns.Length; ++columnIndex)
            {
                _columns[columnIndex].FindGroupsInArea(ref _deductions);
            }

            // If at least one actionable deduction could be found, go back to basics
            if (ImplementDeductions_Groups())
            {
                _state = PuzzleState.FindSingles;
                return;
            }
            
            // If no deductions could be found, move on to the next method
            //_state = PuzzleState.FindXWings;
            
            // TEMP: This just finishes the puzzle if nothing can be found
            _state = PuzzleState.Finished;
        }
        
        #endregion
        
        #region Implement Deductions Functions

        private bool ImplementDeductions_Singles()
        {
            if (_deductions.Count == 0)
            {
                return false;
            }

            for (int deductionIndex = 0; deductionIndex < _deductions.Count; ++deductionIndex)
            {
                Deduction deduction = _deductions[deductionIndex];
                
                if (deduction is not SingleDeduction singleDeduction)
                {
                    throw new NullReferenceException("Somehow the single deduction is not a SingleDeduction");
                }
                
                singleDeduction.Cell.SetSolvedValue(singleDeduction.SolvedValue);
                LogDeduction(deduction);
            }

            _deductions.Clear();
            return true;
        }

        private bool ImplementDeductions_Pointers()
        {
            if (_deductions.Count == 0)
            {
                return false;
            }

            bool deductionUtilized = false;

            for (int deductionIndex = 0; deductionIndex < _deductions.Count; ++deductionIndex)
            {
                Deduction deduction = _deductions[deductionIndex];

                if (deduction is not PointerDeduction pointerDeduction)
                {
                    throw new NullReferenceException("Somehow the pointer deduction is not a PointerDeduction");
                }

                if (_pointers.ContainsKey(pointerDeduction.PointerID))
                {
                    continue;
                }
                
                _pointers.Add(pointerDeduction.PointerID, true);

                Cell pointingCell = pointerDeduction.Cells[0];
                CellArea pointedToCellArea = pointerDeduction.IntersectorAreaType switch
                {
                    AreaType.Square => pointingCell.TwoDimensionalCellArea,
                    AreaType.Custom => pointingCell.TwoDimensionalCellArea,
                    AreaType.Row => pointingCell.RowCellArea,
                    AreaType.Column => pointingCell.ColumnCellArea,
                    _ => throw new ArgumentOutOfRangeException()
                };
                        
                bool changeOccurred = pointedToCellArea.MarkPointedToCells(pointerDeduction.Cells, pointerDeduction.PointerValue);

                if (!changeOccurred)
                {
                    continue;
                }
                
                LogDeduction(deduction);
                deductionUtilized = true;
            }

            _deductions.Clear();
            return deductionUtilized;
        }

        private bool ImplementDeductions_Groups()
        {
            if (_deductions.Count == 0)
            {
                return false;
            }

            bool deductionUtilized = false;
        
            for (int deductionIndex = 0; deductionIndex < _deductions.Count; ++deductionIndex)
            {
                Deduction deduction = _deductions[deductionIndex];
                
                if (deduction is not GroupDeduction groupDeduction)
                {
                    throw new NullReferenceException("Somehow the group deduction is not a GroupDeduction");
                }

                if (_groups.ContainsKey(groupDeduction.GroupID))
                {
                    continue;
                }
                
                Group newGroup = new Group(groupDeduction.Cells);
                _groups.Add(groupDeduction.GroupID, newGroup);

                bool changeOccurred = groupDeduction.CellArea.MarkGroupedCells(groupDeduction.Cells, groupDeduction.GroupedValues);

                if (!changeOccurred)
                {
                    continue;
                }

                LogDeduction(deduction);
                deductionUtilized = true;
            }
        
            _deductions.Clear();
            return deductionUtilized;
        }
        
        #endregion
    
        #region Public Functions

        public override string ToString() => ToString(false);

        public string ToString(bool printWithLineBreaks)
        {
            StringBuilder puzzleResult = new StringBuilder();

            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                char cellChar = cell.IsSolved() ? (char)(cell.CellValue + '0') : '?';
                puzzleResult.Append(cellChar);

                if (printWithLineBreaks && cellIndex % 9 == 8)
                {
                    puzzleResult.Append("\n");
                }
            }
        
            return puzzleResult.ToString();
        }
    
        #endregion
    
        #region Helper Functions

        private void SetStarterValue(int cellIndex, Value starterValue)
        {
            _cells[cellIndex].SetStarterValue(starterValue);

            /*ResetAllSolvedValues();
            stepCount = 0;
            SolvePuzzle();*/
        }

        private bool CheckSolved()
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                CellState cellState = cell.State;
                if (cellState != CellState.Starter && cellState != CellState.Solved)
                {
                    return false;
                }
            }
        
            return true;
        }

        private void ResetAllSolvedValues()
        {
            for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
            {
                Cell cell = _cells[cellIndex];
                cell.ResetSolvedValue();
            }
        }

        private void LogDeduction(Deduction deduction)
        {
            string log = "";
            
            if (deduction is SingleDeduction singleDeduction)
            {
                string value = $"Value {(int)singleDeduction.SolvedValue}";
                
                string cellLocation = $"was set at location {GetLocationStringFromCell(singleDeduction.Cell)}";
                
                string cause = deduction.Cause switch
                {
                    Cause.Area => $"as the only possibility within the {singleDeduction.AreaType}",
                    Cause.Cell => $"as the only possibility in the cell",
                    _ => null
                };

                log = $"{value} {cellLocation} {cause}";
            }

            if (deduction is PointerDeduction pointerDeduction)
            {
                string groupWord = pointerDeduction.Cells.Count switch
                {
                    2 => "Pair",
                    3 => "Triplet",
                    _ => "Group"
                };

                string cause = $"A pointer {groupWord}";
                
                string value = $"involving Value {(int)pointerDeduction.PointerValue}";
                
                string locations = $"was found at locations {GetLocationStringFromCells(pointerDeduction.Cells)}";

                string startingArea = $"starting from their shared {pointerDeduction.AreaType}";

                string endingArea = $"pointing to their shared {pointerDeduction.IntersectorAreaType}";

                log = $"{cause} {value} {locations} {startingArea} {endingArea}";
            }
            
            if (deduction is GroupDeduction groupDeduction)
            {
                string cause = groupDeduction.Cause switch
                {
                    Cause.ConstrainedGroup => $"Constrained",
                    Cause.NakedGroup => $"Naked",
                    Cause.HiddenGroup => $"Hidden",
                    Cause.XWingGroup => $"XWing",
                    _ => null
                };

                string groupWord = groupDeduction.Cells.Count switch
                {
                    2 => "Pair",
                    3 => "Triplet",
                    _ => "Group"
                };

                string values = $"involving values {Util.BoolArrayToString(groupDeduction.GroupedValues)}";
                
                string locations = $"was found at locations {GetLocationStringFromCells(groupDeduction.Cells)}";

                string area = $"within their shared {groupDeduction.AreaType}";
                
                log = $"{cause} {groupWord} {values} {locations} {area}";
            }

            Debug.Log($"{++_stepCount}. {log}");
        }

        private string GetLocationStringFromCell(Cell cell) => $"({cell.RowIndex}, {cell.ColumnIndex})";
        
        private string GetLocationStringFromCells(List<Cell> cells)
        {
            string[] locationStrings = new string[cells.Count];
            
            for (int cellIndex = 0; cellIndex < cells.Count; ++cellIndex)
            {
                Cell cell = cells[cellIndex];
                locationStrings[cellIndex] = GetLocationStringFromCell(cell);
            }

            return string.Join(", ", locationStrings);
        }
    
        #endregion
    }
}
