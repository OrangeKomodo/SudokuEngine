using System;
using System.Collections.Generic;
using System.Text;
using DataTypes.Abstracts;
using DataTypes.CellAreas;
using DataTypes.Deductions;
using Definitions;
using UnityEngine;

namespace DataTypes
{
    public class Puzzle
    {
        #region Private Variables

        // Compile-time variables
        private OneDimensionalCellArea[] _rows = new OneDimensionalCellArea[9];
        private OneDimensionalCellArea[] _columns = new OneDimensionalCellArea[9];
        private TwoDimensionalCellArea[] _twoDimensionalAreas = new TwoDimensionalCellArea[9];
        private Cell[] _cells = new Cell[81];
    
        // Runtime variables
        private List<Deduction> _deductions = new List<Deduction>();
        private Dictionary<string, bool> _groups = new Dictionary<string, bool>(); // This will be something more later
        private Dictionary<string, bool> _pointers = new Dictionary<string, bool>(); // This will be something more later
        private int _stepCount = 0;
    
        #endregion
    
        #region Public Accessors

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
                _rows[areaIndex] = new OneDimensionalCellArea();
                _columns[areaIndex] = new OneDimensionalCellArea();
                _twoDimensionalAreas[areaIndex] = new TwoDimensionalCellArea();
            
                _rows[areaIndex].PopulateData(AreaType.Row, areaIndex);
                _columns[areaIndex].PopulateData(AreaType.Column, areaIndex);
                _twoDimensionalAreas[areaIndex].PopulateData(AreaType.Square, areaIndex);
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
            
                SetStarterValue(cellIndex, cellChar - '0');
            }
        }

        // Get rid of extraneous characters
        private string FormatPuzzleString(string puzzleString)
        {
            return puzzleString.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        }
    
        #endregion
    
        #region Public Functions
    
        public bool SolvePuzzle()
        {
            int passCount = 0;
            
            do
            {
                Debug.Log($"Solve Pass: {++passCount}");
            
                // Check 2D areas
                for (int tdaIndex = 0; tdaIndex < _twoDimensionalAreas.Length; ++tdaIndex)
                {
                    _twoDimensionalAreas[tdaIndex].FindSinglesInArea(ref _deductions);
                }

                if (_deductions.Count > 0)
                {
                    continue;
                }
            
                // Check rows
                for (int rowIndex = 0; rowIndex < _rows.Length; ++rowIndex)
                {
                    _rows[rowIndex].FindSinglesInArea(ref _deductions);
                }

                if (_deductions.Count > 0)
                {
                    continue;
                }
            
                // Check columns
                for (int columnIndex = 0; columnIndex < _columns.Length; ++columnIndex)
                {
                    _columns[columnIndex].FindSinglesInArea(ref _deductions);
                }

                if (_deductions.Count > 0)
                {
                    continue;
                }
            
                // Check all cells
                for (int cellIndex = 0; cellIndex < _cells.Length; ++cellIndex)
                {
                    _cells[cellIndex].CheckIfOnlyOnePossibility(ref _deductions);
                }

                if (_deductions.Count > 0)
                {
                    continue;
                }
                
                // Find Pointers
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

                /*if (_deductions.Count > 0)
                {
                    continue;
                }

                // Check for X-Wings
                for (int columnIndex = 0; columnIndex < columns.Length; ++columnIndex)
                {

                }*/

            } while (ImplementExistingDeductions());
        
            return CheckSolved();
        }

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

        private void SetStarterValue(int cellIndex, int starterValue)
        {
            _cells[cellIndex].SetStarterValue(starterValue);

            /*ResetAllSolvedValues();
            stepCount = 0;
            SolvePuzzle();*/
        }

        private bool ImplementExistingDeductions()
        {
            if (_deductions.Count == 0)
            {
                return false;
            }

            bool deductionUtilized = false;
        
            for (int deductionIndex = 0; deductionIndex < _deductions.Count; ++deductionIndex)
            {
                Deduction deduction = _deductions[deductionIndex];

                switch (deduction.Cause)
                {
                    case Cause.Area:
                    case Cause.Cell:
                    {
                        SingleDeduction singleDeduction = deduction as SingleDeduction;

                        if (singleDeduction == null)
                        {
                            throw new NullReferenceException("Somehow the single deduction is not a SingleDeduction");
                        }
                        
                        singleDeduction.Cell.SetSolvedValue(singleDeduction.SolvedValue);
                        deductionUtilized = true;
                        break;
                    }
                    case Cause.PointerGroup:
                    {
                        PointerDeduction pointerDeduction = deduction as PointerDeduction;

                        if (pointerDeduction == null)
                        {
                            throw new NullReferenceException("Somehow the pointer deduction is not a PointerDeduction");
                        }
                        
                        if (_pointers.ContainsKey(pointerDeduction.PointerID))
                        {
                            continue;
                        }

                        _pointers.Add(pointerDeduction.PointerID, true);

                        Cell pointingCell = pointerDeduction.Cells[0];
                        ACellArea pointedToCellArea = pointerDeduction.IntersectorAreaType switch
                        {
                            AreaType.Square => pointingCell.TwoDimensionalCellArea,
                            AreaType.Custom => pointingCell.TwoDimensionalCellArea,
                            AreaType.Row => pointingCell.RowCellArea,
                            AreaType.Column => pointingCell.ColumnCellArea,
                            _ => null
                        };
                        
                        pointedToCellArea?.MarkPointedToCells(pointerDeduction.Cells, pointerDeduction.PointerValue);
                        deductionUtilized = true;
                        break;
                    }
                    case Cause.NakedGroup:
                    case Cause.HiddenGroup:
                    {
                        GroupDeduction groupDeduction = deduction as GroupDeduction;

                        if (groupDeduction == null)
                        {
                            throw new NullReferenceException("Somehow the group deduction is not a GroupDeduction");
                        }

                        //Group newGroup = new Group(groupDeduction.Cells, groupDeduction.GroupedValues, groupDeduction.AreaType);
                        if (_groups.ContainsKey(groupDeduction.GroupID))
                        {
                            continue;
                        }

                        _groups.Add(groupDeduction.GroupID, true);
                        groupDeduction.CellArea.MarkGroupedCells(groupDeduction.Cells, groupDeduction.GroupedValues);
                        deductionUtilized = true;
                        break;
                    }
                    case Cause.ConstrainedGroup:
                    case Cause.XWingGroup:
                    default:
                    {
                        break;
                    }
                }

                if (deductionUtilized)
                {
                    LogDeduction(deduction);
                }
            }
        
            _deductions.Clear();
            return deductionUtilized;
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
