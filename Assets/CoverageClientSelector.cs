using System.Collections.Generic;
using System.Linq;

using Niantic.Lightship.AR;
using Niantic.Lightship.AR.VpsCoverage;

using UnityEngine;
using UnityEngine.UI;

public class CoverageClientSelector : MonoBehaviour
{
    // References to components
    [SerializeField]
    public CoverageClientManager CoverageClient;

    // Note: This refers to the legacy Unity UI Dropdown component.
    //  If your project uses TMP, you will need to use TMP_Dropdown instead.
    [SerializeField]
    public Dropdown DropdownSelector;

    // This will be populated by selecting an area target by name in the UI dropdown
    public string SelectedPayload;

    private Dictionary<string, string> LocationToPayload = new();

    void Start()
    {
        CoverageClient.TryGetCoverage(OnTryGetCoverage);
        DropdownSelector.onValueChanged.AddListener(OnValueChanged);
    }

    private void OnValueChanged(int arg)
    {
        SelectedPayload = LocationToPayload[DropdownSelector.options[arg].text];
    }

    private void OnTryGetCoverage(AreaTargetsResult args)
    {
        // Clear any previous data
        DropdownSelector.ClearOptions();
        LocationToPayload.Clear();
        SelectedPayload = null;

        var areaTargets = args.AreaTargets;

        // Sort the area targets by proximity to the user
        areaTargets.Sort((a, b) =>
            a.Area.Centroid.Distance(args.QueryLocation).CompareTo(
                b.Area.Centroid.Distance(args.QueryLocation)));

        // Only populate the dropdown with the closest 5 locations.
        // For a full sample with UI and image hints, see the VPSColocalization sample
        for (var i = 0; i < 5; i++)
        {
            LocationToPayload[areaTargets[i].Target.Name] = areaTargets[i].Target.DefaultAnchor;
        }

        DropdownSelector.AddOptions(LocationToPayload.Keys.ToList());
    }
}