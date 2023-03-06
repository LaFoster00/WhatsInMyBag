using System;
using Ludiq;
using Player.Controller;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OffScreenEnemyIndicator : MonoBehaviour
{

    [Header("NECESSARY REFERENCES")]
    [SerializeField] private GameObject IndicatorFrame;
    [SerializeField] private GameObject IndicatorFrameGarbageDump;
    
    [Header("Enemy Indicator"), Space(20)]
    [SerializeField] private float MaxTrackingDistance = 30f;

    [SerializeField, Range(0, 1)] private float EnemyIndicatorPadding = 0.1f;
    
    [SerializeField] private bool ShowDistanceTracker = true;
    [SerializeField] private bool ScaleEnemyIndicatorWithDistance = true;
    //[SerializeField] private bool AdjustPaddingWhenScaling = false;
    [SerializeField] private float MaxSizeIndicator = 1.0f;
    [SerializeField] private float MinSizeIndicator = 0.5f;

    [Header("Garbage Dump Indicator"), Space(20)]
    [SerializeField] private float GarbageDumpIndicatorScale = 1.0f;
    [SerializeField, Range(0, 1)] private float GarbageDumpIndicatorPadding = 0.1f;
    [SerializeField] private Gradient GarbageDumpPulseColor;
    [SerializeField] private float MaxDistancePlayerToGarbageDump = 100f;

    
    [Header("DEBUGGING"), Space(20)]
    [SerializeField] private int IndexChildHeadIcon;
    [SerializeField] private float CharacterCenterY = 0.5f;
    //[SerializeField] private GameObject DebugObject;
    
    [SerializeField] private GameObject[] Indicators;
    [SerializeField] private GameObject[] Colleagues;
    [SerializeField] private bool[] VisibilityTracker;

    private GameObject Mahlee;
    private GameObject GarbageDump;

    
    private GameObject _garbageDump;
    private GameObject _garbageDumpIndicator;

    private Vector3 _colleagueScreenPosition;
    private Vector3 _indicatorPosition;
    private float _slope;

    private float _defaultIndicatorScale;
    private float _differenceMinMaxScale;
    private int _additionalPaddingDueToScaling = 0;
    
    private float _distancePlayerToColleague;
    
    void Start()
    {
        Colleagues = GameObject.FindGameObjectsWithTag("Colleague");
        if (null == Colleagues)
        {
            Debug.Log("OffScreenEnemyIndicator: Couldn't find any active Colleagues in Scene. There will be no indicators.");
        }
        
        Mahlee = FindObjectOfType<PlayerController>().gameObject;
        if (Mahlee == null)
        {
            Debug.Log("OffScreenEnemyIndicator: Could not find any PlayerControllers in Scene.");
        }

        GarbageDump = FindObjectOfType<DeadBodyDisposal>().gameObject;
        if (GarbageDump == null)
        {
            Debug.Log("OffScreenEnemyIndicator: Could not find any Object of Type DeadBodyDisposal in the Scene.");
        }

        Indicators = new GameObject[Colleagues.Length];
        VisibilityTracker = new bool[Colleagues.Length];

        _garbageDump = GarbageDump;
        _defaultIndicatorScale = IndicatorFrame.transform.localScale.x;
        
        CreateIndicatorsForColleagues();
        CreateIndicatorsForGarbageDump();
    }



    void Update()
    {
        _differenceMinMaxScale = MaxSizeIndicator - MinSizeIndicator;
        _garbageDumpIndicator.transform.localScale = GarbageDumpIndicatorScale * Vector3.one;
        
        if (Colleagues != null)
        {
            TrackVisibilityOfColleagues();
        }

        if (GarbageDump != null)
        {
            TrackVisibilityOfGarbageDump();
            UpdateGarbageDumpRingPulse();
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < Indicators.Length; i++)
        {
            UpdateActiveIndicatorTransforms(Indicators[i], Colleagues[i], ScaleEnemyIndicatorWithDistance, ShowDistanceTracker, _defaultIndicatorScale, EnemyIndicatorPadding);
        }
        UpdateActiveIndicatorTransforms(_garbageDumpIndicator, GarbageDump, false, false, GarbageDumpIndicatorScale, GarbageDumpIndicatorPadding);
    }

    // Ignore this for now - EXPERIMENTAL
    
    private void UpdateGarbageDumpRingPulse()
    {
        Vector3 garbageDumpViewPort = Camera.main.WorldToViewportPoint(GarbageDump.transform.position);

        // Mathf.Clamp01 clamps values above 1 to 1, and below 0 to 0. --> Used to check screen boundaries.
        Vector3 screenPosClamped =
            new Vector3(Mathf.Clamp01(garbageDumpViewPort.x), Mathf.Clamp01(garbageDumpViewPort.y), 0);

        // remap screenPostClamped (Range 0-1) to padded Screen.width and Screen.height.
        Vector3 v3 = new Vector3(
            math.remap(0, 1, 0 + (int)(GarbageDumpIndicatorPadding * Screen.width / 2), Screen.width - (int)(GarbageDumpIndicatorPadding * Screen.width / 2),
                screenPosClamped.x),
            math.remap(0, 1, 0 + (int)(GarbageDumpIndicatorPadding * Screen.height / 2), Screen.height - (int)(GarbageDumpIndicatorPadding * Screen.height / 2),
                screenPosClamped.y),
            0);
        
        
        Vector3 characterCenter = Mahlee.transform.position.y * Vector3.up;

        Ray indicatorRay = Camera.main.ScreenPointToRay(v3);
        new Plane(Vector3.up, characterCenter).Raycast(indicatorRay, out float enter);

            
        float distanceMarkerToEnemy = math.distance(indicatorRay.GetPoint(enter), GarbageDump.transform.position);

        IndicatorPulseRing indicatorPulseRing = _garbageDumpIndicator.GetComponent<IndicatorFrame>().PulseRings.GetComponent<IndicatorPulseRing>();

        Color newColor = GarbageDumpPulseColor.Evaluate(Mathf.Clamp01(distanceMarkerToEnemy/MaxDistancePlayerToGarbageDump));

        indicatorPulseRing.PulseRingV1.GetComponent<PulseRing>().UpdateColor(new Vector3(newColor.r, newColor.g, newColor.b));
        indicatorPulseRing.PulseRingV2.GetComponent<PulseRing>().UpdateColor(new Vector3(newColor.r, newColor.g, newColor.b));
        indicatorPulseRing.PulseRingV3.GetComponent<PulseRing>().UpdateColor(new Vector3(newColor.r, newColor.g, newColor.b));


    }
    

    private void TrackVisibilityOfColleagues()
    {
        for (int i = 0; i < Colleagues.Length; i++)
        {
            _colleagueScreenPosition = Camera.main.WorldToScreenPoint(Colleagues[i].transform.position);
            _distancePlayerToColleague = Mathf.Abs(Vector3.Distance(Mahlee.transform.position, Colleagues[i].transform.position));

            if (_distancePlayerToColleague < MaxTrackingDistance)
            {
                if(_colleagueScreenPosition.x > 0 && _colleagueScreenPosition.x < Screen.width && _colleagueScreenPosition.y > 0 && _colleagueScreenPosition.y < Screen.height)
                {
                    VisibilityTracker[i] = true;
                    Indicators[i].SetActive(false);
                }

                else if(_colleagueScreenPosition.x < 0 || _colleagueScreenPosition.x > Screen.width || _colleagueScreenPosition.y < 0 || _colleagueScreenPosition.y > Screen.height )
                {
                    VisibilityTracker[i] = false;
                    Indicators[i].SetActive(true);
                } 
            }
            else
            {
                VisibilityTracker[i] = false;
                Indicators[i].SetActive(false);
            }
        }
    }
    
    
    private void TrackVisibilityOfGarbageDump()
    {
        Vector3 garbageDumpScreenPosition = Camera.main.WorldToScreenPoint(GarbageDump.transform.position);
        float distancePlayerToGarbageDump =
            Mathf.Abs(Vector3.Distance(Mahlee.transform.position, GarbageDump.transform.position));



        if (garbageDumpScreenPosition.x > 0 && garbageDumpScreenPosition.x < Screen.width &&
            garbageDumpScreenPosition.y > 0 && garbageDumpScreenPosition.y < Screen.height)
        {
            _garbageDumpIndicator.SetActive(false);
        }
    
        else if (garbageDumpScreenPosition.x < 0 || garbageDumpScreenPosition.x > Screen.width ||
                 garbageDumpScreenPosition.y < 0 || garbageDumpScreenPosition.y > Screen.height)
        {
            _garbageDumpIndicator.SetActive(true);
        }



        else
        {
            _garbageDumpIndicator.SetActive(false);
        }
    }

    
    private void UpdateActiveIndicatorTransforms(GameObject indicator, GameObject colleague, bool scaleIndicatorWithDistance, bool showDistanceTracker, float defaultIndicatorScale, float padding)
    {

        if (!indicator || !indicator.activeSelf)
        {
            return;
        }
        
        // Reminder: ViewPort: BottomLeft = (0,0); BottomRight = (1,0); TopLeft = (0,1); TopRight = (1,1); Center = (0.5,0.5)
        Vector3 colleagueViewPort = Camera.main.WorldToViewportPoint(colleague.transform.position);

        // Mathf.Clamp01 clamps values above 1 to 1, and below 0 to 0. --> Used to check screen boundaries.
        Vector3 screenPosClamped =
            new Vector3(Mathf.Clamp01(colleagueViewPort.x), Mathf.Clamp01(colleagueViewPort.y), 0);

        // remap screenPostClamped (Range 0-1) to padded Screen.width and Screen.height.
        Vector3 v3 = new Vector3(
            math.remap(0, 1, 0 + (padding * Screen.width / 2), Screen.width - (padding * Screen.width / 2),
                screenPosClamped.x),
            math.remap(0, 1, 0 + (padding * Screen.width / 2), Screen.height - (padding * Screen.width / 2),
                screenPosClamped.y),
            0);
        
        indicator.transform.position = v3;

        //indicator.transform.position = _indicatorPosition;    // Updating >> INDICATOR POSITION <<
        Vector3 viewPortCenter = new Vector3(0.5f, 0.5f, 0f);

        indicator.GetComponent<IndicatorFrame>().IndicatorArrow.transform.rotation =
            Quaternion.FromToRotation(Vector3.up,
                screenPosClamped - viewPortCenter); // Updating: Indicator >> ARROW ROTATION <<




        #region DistanceTracker & Dynamic Scaling   
        if (showDistanceTracker || scaleIndicatorWithDistance)
        {
            indicator.GetComponent<IndicatorFrame>().IndicatorDistance.SetActive(true);

            // >> WORLD SPACE <<
            Vector3 colleaguePositionWS = colleague.transform.position;
            colleaguePositionWS.y = CharacterCenterY;
            Vector3 mahleePositionWS = Mahlee.transform.position;
            mahleePositionWS.y = CharacterCenterY;


            // This is used to place the plane at the height of Characters regardless of the entire scene setup relative to WS.
            // TODO: !! However, needs further adjustments if you plan on having multiple storeys (building) in your scene, as characters may differ in ther global y position.
            Vector3 characterCenter = colleague.transform.position.y * Vector3.up;

            Ray indicatorRay = Camera.main.ScreenPointToRay(v3);
            new Plane(Vector3.up, characterCenter).Raycast(indicatorRay, out float enter);

            
            float distanceMarkerToEnemy =
                math.distance(indicatorRay.GetPoint(enter), colleaguePositionWS);

            float distancePlayerToEnemy = math.distance(colleaguePositionWS, mahleePositionWS);
            

            if (scaleIndicatorWithDistance) // Updating: >> INDICATOR SCALE <<
            {
                indicator.transform.localScale =
                    math.remap(0, 1, MaxSizeIndicator, MinSizeIndicator,
                        distanceMarkerToEnemy / MaxTrackingDistance) * Vector3.one;
            }

            if (showDistanceTracker) // Updating: >> DISTANCE TRACKER <<
            {
                
                indicator.GetComponent<IndicatorFrame>().IndicatorDistance.GetComponent<Text>().text =
                    (int)(distanceMarkerToEnemy) + "m";     
                
            }

        }

        if (!scaleIndicatorWithDistance)
        {
            indicator.transform.localScale = defaultIndicatorScale * Vector3.one;
            _additionalPaddingDueToScaling = 0;
        }

        if (!showDistanceTracker)
        {
            indicator.GetComponent<IndicatorFrame>().IndicatorDistance.SetActive(false);
        }

        #endregion


    }

    private void CreateIndicatorsForColleagues()    // >> DONE <<
    {
        for (int i = 0; i < Colleagues.Length; i++)
        {
            GameObject newIndicator = Instantiate(IndicatorFrame, _indicatorPosition, quaternion.identity);
            newIndicator.transform.SetParent(this.gameObject.transform, false);
            newIndicator.name = "Indicator of: "+Colleagues[i].name;

            Indicators[i] = newIndicator;

            
            if (newIndicator.GetComponent<IndicatorFrame>().Head)
            {
                IndicatorIcon IconToDisplay = Colleagues[i].GetComponent<IndicatorIcon>();
                newIndicator.GetComponent<IndicatorFrame>().Head.GetComponent<Image>().sprite =
                    IconToDisplay.IndicatorImage;

                Vector3 positionOffset = Vector3.right * IconToDisplay.IconOffset.x + Vector3.up * IconToDisplay.IconOffset.y +
                                         Vector3.zero;
                newIndicator.GetComponent<IndicatorFrame>().Head.gameObject.transform.localPosition += positionOffset;
                newIndicator.GetComponent<IndicatorFrame>().Head.gameObject.transform.localScale = IconToDisplay.IconScale;

            }
        }
    }
    
    private void CreateIndicatorsForGarbageDump()    // >> DONE <<
    {
        GameObject newIndicator = Instantiate(IndicatorFrameGarbageDump, _indicatorPosition, quaternion.identity);
        newIndicator.transform.SetParent(this.gameObject.transform, false);
        newIndicator.name = "Indicator of: "+ GarbageDump.name;
        
        _garbageDumpIndicator = newIndicator;

        
        if (_garbageDumpIndicator.GetComponent<IndicatorFrame>().PulseRings.GetComponent<PlayableDirector>()) 
        {
            _garbageDumpIndicator.GetComponent<IndicatorFrame>().PulseRings.GetComponent<PlayableDirector>().Play();
        }
    }


}
    
    
    
