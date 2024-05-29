using UnityEngine;
using UnityEngine.UI;

public class MapResetButton : MonoBehaviour
{
    [SerializeField]
    private Button _resetButton;
    [SerializeField]
    private Mapbox.Examples.MapController _mapController;

    void Start()
    {
        _resetButton.onClick.AddListener(_mapController.ResetToInitialValues);
    }
}
