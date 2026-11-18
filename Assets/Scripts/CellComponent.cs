
using DataTypes;
using Definitions;
using TMPro;
using UnityEngine;

public class CellComponent : MonoBehaviour
{
    [Header("Cell Text")]
    [SerializeField] private TMP_Text CellText;
    
    [Header("Notation Texts")]
    [SerializeField] private GameObject PossibleValuesParent;
    [SerializeField] private GameObject[] PossibleValuesTexts; // <- It is imperative that these are in order in the Inspector

    private void Start()
    {
        CellText.text = "";
        CellText.color = Util.StarterCharColor;
        CellText.fontStyle = FontStyles.Normal;
        UpdatePossibleValuesTexts(new bool[9]);
    }

    public void DisplayCell(Cell cell)
    {
        switch (cell.State)
        {
            case CellState.Grouped:
            case CellState.Uncertain:
            {
                //CellText.text = "?";
                CellText.text = "";
                CellText.color = Util.UncertainCharColor;
                CellText.fontStyle = FontStyles.Normal;
                PossibleValuesParent.SetActive(true);
                
                break;
            }
            case CellState.Solved:
            {
                CellText.text = ((int)cell.CellValue).ToString();
                CellText.color = Util.SolvedCharColor;
                CellText.fontStyle = FontStyles.Normal;
                PossibleValuesParent.SetActive(false);

                break;
            }
            case CellState.Starter:
            default:
            {
                CellText.text = ((int)cell.CellValue).ToString();
                CellText.color = Util.StarterCharColor;
                CellText.fontStyle = FontStyles.Bold;
                PossibleValuesParent.SetActive(false);

                break;
            }
        }
        
        UpdatePossibleValuesTexts(cell.PossibleValues);
    }

    private void UpdatePossibleValuesTexts(bool[] possibleValues)
    {
        for (int valueIndex = 0; valueIndex < possibleValues.Length; ++valueIndex)
        {
            PossibleValuesTexts[valueIndex].SetActive(possibleValues[valueIndex]);
        }
    }
    
}
