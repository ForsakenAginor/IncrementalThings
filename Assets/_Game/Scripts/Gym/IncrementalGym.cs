using System;
using ScriptGoblin.NumberForge;
using ScriptGoblin.NumberForge.Formatting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IncrementalGym : MonoBehaviour
{
    [SerializeField] private TMP_Text _valueView;
    [SerializeField] private Button _doubleButton;
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _subtractButton;

    private BigNumber _bigNumber = 10;
    
    private void Awake()
    {
        BigNumberFormatter.Current = new AlphabetSuffixFormat();
        _valueView.text = _bigNumber.ToFormattedString();
        
        _doubleButton.onClick.AddListener(Double);
        _addButton.onClick.AddListener(Add);
        _subtractButton.onClick.AddListener(Subtract);
    }

    private void OnDestroy()
    {
        _doubleButton.onClick.RemoveAllListeners();
        _addButton.onClick.RemoveAllListeners();
        _subtractButton.onClick.RemoveAllListeners();
    }

    private void Subtract()
    {
        _bigNumber = _bigNumber * 0.8f;
        _valueView.text = _bigNumber.ToFormattedString();
    }

    private void Add()
    {
        _bigNumber = _bigNumber * 1.2f;
        _valueView.text = _bigNumber.ToFormattedString();
    }

    private void Double()
    {
        _bigNumber = _bigNumber * 2;
        _valueView.text = _bigNumber.ToFormattedString();
    }
}
