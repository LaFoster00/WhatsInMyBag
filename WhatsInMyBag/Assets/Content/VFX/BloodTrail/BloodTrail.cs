using System;
using System.Collections;
using System.Collections.Generic;
using Ludiq;
using MiscUtil.Linq.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

//[RequireComponent(typeof(NavMeshAgent))] [RequireComponent(typeof(VFXPropertyBinder))] [RequireComponent(typeof(VisualEffect))]
public class BloodTrail : MonoBehaviour
{
	[Tooltip("This rigidbody's velocity is used to determine whether the Corpse is currently in a moving-state or not. Root_M")]
	[SerializeField]
	private Rigidbody deadBodyRigidbody;

	[SerializeField]
	private GameObject bloodDecal;

	[Tooltip("head_R; Root of the Corpse")]
	[SerializeField]
	public GameObject bloodSpawnPosition;

	[Tooltip("Base-Size of a new blood pool with intensity 1.")]
	[SerializeField]
	private float bloodSpawnSize = 1f;

	[Tooltip("If the distance of the corpse to a preexisting blood-pool is less than this value, then instead of spawning a new blood-pool on top of it, the already existing one increases in size.")]
	[SerializeField]
	private float bloodSpawnPositionDeadzone = 0;

	[Tooltip("Corpse will only bleed when its velocity is below this value. Acts as substitute for checking if its currently being carried or not.")]
	[SerializeField]
	private float restingVelocityDeadzone = 0.1f;

	[Tooltip("Cooldown in-between each bleeding")]
	[SerializeField]
	private float bleedCooldown = 5f;

	[SerializeField]
	private bool resetBleedCooldownUponMovingCorpse = true;

	[Tooltip("Fade-in speed of new Blood")]
	[SerializeField]
	private float fadeSpeed = 1f;

	[Tooltip("Grow speed of new Blood")]
	[SerializeField]
	private float enlargenSpeed = 1f;

	[Tooltip("By how much blood grows in size if it gets enlargened")]
	[SerializeField]
	private float enlargeFactor = 0.25f;

	[Tooltip(
		"New Blood starts with Intensity 1. Each time it gets enlargened, its Intensity increases by 1. Set a maximum value for the Intensity to control max size of each blood.")]
	[SerializeField]
	private int maxIntensity = 3;

	[SerializeField]
	private GameObject cleaningLady;

	[Tooltip(
		"Used when the currently existing Bloodtrail doesn't have any more ways of calculating a new path between 2 points of interest.")]
	[SerializeField]
	private GameObject fallbackPoint;

	[Tooltip(
		"The higher this value, the less likely are pathfinding errors (trails through walls etc.) going to occur. But it will also take longer to occur because this time refers to each individual pathfinding portion.")]
	[SerializeField, Range(0.1f, 3.0f)]
	private float timeAvailableForPathfinding = 1.0f;

	
	[Tooltip("Stores the actual blood GameObjects")]
	[SerializeField]
	private List<GameObject> bloodtrail;

	[Tooltip("Stores the intensity of the individual blood-puddles")]
	[SerializeField]
	private List<int> bloodIntensity;

	[Header("Set by other Scripts"), Space(20)]
	[Tooltip("This boolean is set by SafeZone GameObjects in Scene.")]
	public bool corpseInSafeZone = false;
	
	
	// Needed for new Blood
	private bool  _isMoving = false;
	private float _remainingBleedCooldown;
	private bool  _fadeInBlood = false;
	private float _t           = 0;

	// Needed for preexisting Blood, that shall grow in size
	private float _e                        = 0;
	private float _startSizeBloodToEnlargen = 0f;
	private bool  _enlargeBlood            = false;
	private int   _indexOfBloodToEnlargen   = 0;

	private NavMeshAgent _myNavMeshAgent;
	private LineRenderer _myLineRenderer;

	private VFXCustomPositionBinder _myCustomPositionBinder;

	private List<Vector3> _bloodTrailPoints;

	


	// TODO: Make use of this Event in the Cleaning Lady AI.
	public delegate List<GameObject> BloodSpawned(List<GameObject> bloodtrail, List<int> bloodIntensity);

	public event BloodSpawned CorpseBleedingEvent;

	// Start is called before the first frame update
	void Start()
	{
		bloodtrail = new List<GameObject>();
		bloodIntensity = new List<int>();

		bloodDecal.transform.localScale = bloodSpawnSize * Vector3.one;
		_remainingBleedCooldown = bleedCooldown;

		_myNavMeshAgent = GetComponent<NavMeshAgent>();
		_myLineRenderer = GetComponent<LineRenderer>();
		_myLineRenderer.startColor = Color.red;
		_myLineRenderer.endColor = Color.red;


		_myCustomPositionBinder = GetComponent<VFXCustomPositionBinder>();

		GameObject bloodPuddles = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
		bloodPuddles.name = "Blood Puddles";

		_bloodTrailPoints = new List<Vector3>();

		for (int i = 0; i < _myCustomPositionBinder.Targets.Length; i++)
		{
			//Debug.Log(myCustomPositionBinder.Targets[i]);
		}

		//Debug.Log(bindings[0]+" is of type "+ bindings[0].GetType());

		//TODO specifically get the Binding of Type VFXCustomPositionBinder, so that it doesn't necessarily have to be in first place in the list of bindings.
		//myCustomBinders = new List<VFXCustomPositionBinder>();
		//myCustomBinders = myVFXPropertyBinder.GetPropertyBinders<VFXCustomPositionBinder>();
		//Debug.Log(myCustomBinders.GetEnumerator().Current);


		_myLineRenderer.startWidth = 0.15f;
		_myLineRenderer.endWidth = 0.15f;
		_myLineRenderer.positionCount = 0;


		// TODO Subscribe to Event telling us when Corpse is being dropped AND picked up.
		// That would get rid of unnecessary checks on the Corpse's speed to determine whether its being moved or not.
	}


	#region Reminder How List of Lists works

	//List<List<int>> numbers = new List<List<int>>();     // List of List<int>
	//numbers.Add(new List<int>());                        // Adding a new List<int> to the List.
	//numbers[0].Add(2);                                   // Add the integer '2' to the List<int> at index '0' of numbers.

	#endregion
	


	// Update is called once per frame
	void Update()
	{
		/* Had to Comment this out because the newly 'WWise Fixed Project' has changed some player input handling which breaks the script
		if (Input.GetMouseButtonDown(0))
		{
		    ClickToMove();
		}
		*/

		/*
		if (Input.GetKeyDown("k"))
		{
		    if (Bloodtrail.Count > 0)
		    {
		        Destroy(Bloodtrail[0]);
		        
		        Bloodtrail.Remove(Bloodtrail[0]);
		        BloodIntensity.Remove(BloodIntensity[0]);

		        StartCoroutine(CalculateBloodTrail());
		    }
		}

		if (Input.GetKeyDown("l"))
		{
		    StartCoroutine(CalculateBloodTrail());
		}
		
		if (Input.GetKeyDown("m"))
		{
		    DrawPath();
		}
		*/


		if (_myNavMeshAgent.hasPath)
		{
			DrawPath();
		}


		#region Fade in newly spawned Blood-puddles

		if (_fadeInBlood) // Fully works on all new Blood
		{
			if (bloodtrail[bloodtrail.Count - 1].GetComponent<DecalProjector>().fadeFactor < 1)
			{
				bloodtrail[bloodtrail.Count - 1].GetComponent<DecalProjector>().fadeFactor = Mathf.Lerp(0, 1, _t);
				bloodtrail[bloodtrail.Count - 1].transform.localScale = Mathf.Lerp(0, bloodSpawnSize, _t) * Vector3.one;
				_t += fadeSpeed * Time.deltaTime;
			}
			else
			{
				_t = 0;
				_fadeInBlood = false;
			}
		}

		#endregion


		#region Increase existing Blood-puddle

		if (_enlargeBlood)
		{
			//Debug.Log(Bloodtrail.Count);   
			//Debug.Log(IndexOfBloodToEnlargen); 

			if (bloodtrail.Count > _indexOfBloodToEnlargen)
			{
				if (_startSizeBloodToEnlargen < bloodtrail[_indexOfBloodToEnlargen].transform.localScale.x +
					(bloodIntensity[_indexOfBloodToEnlargen] - 1) * enlargeFactor)
				{
					bloodtrail[_indexOfBloodToEnlargen].GetComponent<DecalProjector>().size =
						Mathf.Lerp(_startSizeBloodToEnlargen,
						           _startSizeBloodToEnlargen +
						           (bloodIntensity[_indexOfBloodToEnlargen] - 1) * enlargeFactor, _e) * Vector3.one;
					_e += enlargenSpeed * Time.deltaTime;
				}
				else
				{
					_e = 0;
					_enlargeBlood = false;
				}
			}
		}

		#endregion


		if (CheckIfMoving())
		{
			if (resetBleedCooldownUponMovingCorpse)
			{
				ResetBleedCooldown();
			}

			return;
		}

		#region Logic: When to start bleeding

		if (!corpseInSafeZone)
		{
			if (!CheckIfMoving())
			{
				if (_remainingBleedCooldown <= 0f)
				{
					if (bloodtrail.Count == 0)
					{
						Bleed(); // Spawns Blood if none is on the map.
					}
					else if (bloodtrail.Count != 0)
					{
						if (Mathf.Abs(Vector3.Distance(bloodtrail[GetIndexClosestBlood()].transform.position,
							    bloodSpawnPosition.transform.position)) > bloodSpawnPositionDeadzone)
						{
							Bleed();
						}
						else
						{
							_indexOfBloodToEnlargen = GetIndexClosestBlood();
							if (bloodIntensity[_indexOfBloodToEnlargen] < maxIntensity)
							{
								_e = 0;
								_enlargeBlood = true;
								_startSizeBloodToEnlargen =
									bloodtrail[_indexOfBloodToEnlargen].GetComponent<DecalProjector>().size.x;
								bloodIntensity[_indexOfBloodToEnlargen] += 1;
								bloodtrail[_indexOfBloodToEnlargen].name = "Blood_Nr." + (_indexOfBloodToEnlargen + 1) +
								                                           "_Intensity_" +
								                                           bloodIntensity[_indexOfBloodToEnlargen];
							}
						}
					}


					ResetBleedCooldown();
					_fadeInBlood = true;

					CorpseBleedingEvent?.Invoke(
						bloodtrail,
						bloodIntensity); // passes list of blood GOs, and list of their Intensities to Observers.
				}
				else if (_remainingBleedCooldown > 0f)
				{
					_remainingBleedCooldown -= Time.deltaTime;
				}
			}
		}


		#endregion
	}


	#region Methods for Blood-Puddle Spawn functionality

	private void Bleed()
	{
		GameObject spawnedBlood =
			Instantiate(bloodDecal, bloodSpawnPosition.transform.position, Quaternion.Euler(90, 0, 0));
		//spawnedBlood.transform.parent = this.transform;    // TODO Parent new blood to a separate GO.
		_fadeInBlood = true;
		//spawnedBlood.transform.parent = BloodPuddles.transform;


		bloodtrail.Add(spawnedBlood);
		bloodIntensity.Add(1);
		spawnedBlood.name = "Blood_Nr." + bloodtrail.Count + "_Intensity_" + bloodIntensity[bloodIntensity.Count - 1];

		StartCoroutine(CalculateBloodTrail());
	}


	private bool CheckIfMoving()
	{
		if (Mathf.Abs(Vector3.Magnitude(deadBodyRigidbody.velocity)) < restingVelocityDeadzone)
		{
			_isMoving = false;
		}
		else
		{
			_isMoving = true;
		}

		return _isMoving;
	}

	private void ResetBleedCooldown()
	{
		_remainingBleedCooldown = bleedCooldown;
	}

	private int GetIndexClosestBlood()
	{
		int minIndex = 0;

		if (bloodtrail == null) 
			return minIndex;
		
		float min = Mathf.Abs(Vector3.Distance(bloodtrail[0].transform.position,
		                                       bloodSpawnPosition.transform.position));

		for (int i = 1; i < bloodtrail.Count; i++)
		{
			if (Mathf.Abs(Vector3.Distance(bloodtrail[i].transform.position,
			                               bloodSpawnPosition.transform.position)) < min)
			{
				min = Mathf.Abs(Vector3.Distance(bloodtrail[i].transform.position,
				                                 bloodSpawnPosition.transform.position));
				minIndex = i;
			}
		}

		return minIndex;
	}

	#endregion


	#region Pathfinding BloodTrail

	private void ClickToMove()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		bool hasHit = Physics.Raycast(ray, out hit);
		if (hasHit)
		{
			SetDestination(hit.point);
		}
	}


	private IEnumerator CalculateBloodTrail()
	{
		// TODO: More Optimization --> when new blood is added/removed, only recalculate that specific sub-path, and leave the rest of the chain as is
		// TODO: This may require a List of Arrays with each Array containing the corner-points from one blood to the next one?


		var updatedBloodTrailPoints = new List<Vector3>();

		// TODO only update the blood-puddle trail chain when a blood-puddle is added or removed.
		if (bloodtrail.Count >= 2)
		{
			for (int i = bloodtrail.Count - 1; i >= 1; i--)
			{
				this.gameObject.transform.position = bloodtrail[i].transform.position;
				_myNavMeshAgent.SetDestination(bloodtrail[i - 1].transform.position);

				while (_myNavMeshAgent.pathPending)
				{
					yield return new WaitForSeconds(timeAvailableForPathfinding);
				}

				for (int l = 0; l < _myNavMeshAgent.path.corners.Length - 1; l++)
				{
					updatedBloodTrailPoints.Add(_myNavMeshAgent.path.corners[l]);
				}
			}

			updatedBloodTrailPoints.Add(_myNavMeshAgent.path.corners[_myNavMeshAgent.path.corners.Length - 1]);
		}


		if (bloodtrail.Count == 1)
		{
			updatedBloodTrailPoints.Add(bloodtrail[0].transform.position);
		}


		if (bloodtrail.Count == 0)
		{
			updatedBloodTrailPoints.Add(fallbackPoint.transform.position);
		}


		// TODO: Give Trail from oldest blood to CleaningLady an own separate path to avoid having to recalculate everything from scratch every time the CleaningLady moves
		if (cleaningLady && bloodtrail.Count >= 1)
		{
			int k = 0;
			this.gameObject.transform.position = bloodtrail[k].transform.position;
			_myNavMeshAgent.SetDestination(cleaningLady.transform.position);

			for (int l = 0; l < _myNavMeshAgent.path.corners.Length; l++)
			{
				while (_myNavMeshAgent.pathPending)
				{
					yield return new WaitForSeconds(timeAvailableForPathfinding);
				}

				updatedBloodTrailPoints.Add(_myNavMeshAgent.path.corners[l]);
			}
		}

		_bloodTrailPoints = updatedBloodTrailPoints;

		// Debug.Log("Current BloodTrailPoints.Count: " + _bloodTrailPoints.Count);
		// Debug.Log("Has Path: " + _myNavMeshAgent.hasPath);
	}

	private void SetDestination(Vector3 target)
	{
		_myNavMeshAgent.SetDestination(target);
	}

	private void DrawPath()
	{
		_myLineRenderer.positionCount = _bloodTrailPoints.Count;

		if (_myLineRenderer.positionCount > 0)
		{
			_myLineRenderer.SetPosition(0, transform.position);
		}


		Vector3[] corners = new Vector3[_bloodTrailPoints.Count];


		if (_bloodTrailPoints.Count >= 0)
		{
			for (int i = 0; i < _bloodTrailPoints.Count; i++)
			{
				Vector3 pointPosition =
					new Vector3(_bloodTrailPoints[i].x, _bloodTrailPoints[i].y, _bloodTrailPoints[i].z);

				corners[i] = pointPosition;
				_myLineRenderer.SetPosition(i, pointPosition);
			}
		}


		_myCustomPositionBinder.Targets = corners;
	}

	#endregion
}
