using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using Interactables.Items;
using Interactables.Props;
using Ludiq;
using Player.Controller;
using UnityEngine;
using UnityEngine.Playables;

public class PuzzlePieceManager : MonoBehaviour
{

    [SerializeField] private GameObject PuzzlePieceVFXWorld;
    [SerializeField] private GameObject PuzzlePieceVFXUI;
 
    [SerializeField] private float GlobalSizePuzzlePieceVFX = 1f;
    [SerializeField] private float PositionOffsetY = 0f;
    
    [SerializeField] private List<GameObject> Interactables;
    [SerializeField] private List<GameObject> ActivePuzzlePieces;

    [SerializeField] private bool DisplayPuzzlePiecesInScreenSpace = false;
    
    private Vector3 _positionOffsetY;
    private GameObject _viableInteractableInRange;
    private GameObject _viableInteractableInRangeVfx;
    private int _indexViableObjectInRange;
    private GameObject PuzzlePieceVFX;
    
    private void Start()
    {
       ActivePuzzlePieces = new List<GameObject>();
       Interactables = new List<GameObject>();

       if (DisplayPuzzlePiecesInScreenSpace)
       {
           PuzzlePieceVFX = PuzzlePieceVFXUI;
       }
       else
       {
           PuzzlePieceVFX = PuzzlePieceVFXWorld;
       }
    }
    
    
    void OnEnable()
    {
        GameEventManager.AddListener<ItemPickedUpEvent>(OnItemPickedUp);
        GameEventManager.AddListener<ItemDroppedEvent>(OnItemDropped);
        GameEventManager.AddListener<RecipeAvailableEvent>(OnRecipeAvailable);
        GameEventManager.AddListener<PropInteractionAvailableEvent>(OnPropInteractionAvailable);
        GameEventManager.AddListener<InteractionHappenedEvent>(OnInteractionHappened);
        GameEventManager.AddListener<CraftingHappenedEvent>(OnCraftingHappened);
        GameEventManager.AddListener<EnteringViableInteractableRangeEvent>(OnEnteringViableInteractableRange);
        GameEventManager.AddListener<ExitingViableInteractableRangeEvent>(OnExitingViableInteractableRange);
    }

    void OnDisable ()
    {
        GameEventManager.RemoveListener<ItemPickedUpEvent>(OnItemPickedUp);
        GameEventManager.RemoveListener<ItemDroppedEvent>(OnItemDropped);
        GameEventManager.RemoveListener<RecipeAvailableEvent>(OnRecipeAvailable);
        GameEventManager.RemoveListener<PropInteractionAvailableEvent>(OnPropInteractionAvailable);
        GameEventManager.RemoveListener<InteractionHappenedEvent>(OnInteractionHappened);
        GameEventManager.RemoveListener<CraftingHappenedEvent>(OnCraftingHappened);
        GameEventManager.RemoveListener<EnteringViableInteractableRangeEvent>(OnEnteringViableInteractableRange);
        GameEventManager.RemoveListener<ExitingViableInteractableRangeEvent>(OnExitingViableInteractableRange);
    }

    private void Update()
    {
        _positionOffsetY = PositionOffsetY * Vector3.up;
        
        if (DisplayPuzzlePiecesInScreenSpace)
        {
            PuzzlePieceVFX = PuzzlePieceVFXUI;
        }
        else
        {
            PuzzlePieceVFX = PuzzlePieceVFXWorld;
        }

        for (int i = 0; i < ActivePuzzlePieces.Count; i++) 
        {
            if (ActivePuzzlePieces[i] != null && Interactables[i] != null)
            {
                UpdatePuzzlePieceVFXPosition(ActivePuzzlePieces[i], Interactables[i]); 
                UpdatePuzzlePieceVFXGlobalSize(ActivePuzzlePieces[i]);
            }
        }
        
    }
    

    void OnItemPickedUp (ItemPickedUpEvent e)
    {
        // PlayerController.OnItemPickUp;

        // e.Item;
        // e.Controller;
        //--------------------------------------------------------------------------------------------------------------
    }
    
    void OnItemDropped (ItemDroppedEvent e)  
    {
        // PlayerController.DropHeldItem;
        
        // e.Item;
        // e.Controller;
        //--------------------------------------------------------------------------------------------------------------
        
        for (int i = 0; i < ActivePuzzlePieces.Count; i++) // TODO: Fix this index out of Range Error that happens when crafting/interaction has took place in some shape or form.
        {
            if (ActivePuzzlePieces[i] != null)
            {
                VFXCancel(ActivePuzzlePieces[i]);
            }
            if (Interactables[i] != null)
            {
                ToggleOutline(false, Interactables[i]);
            }
        }
        
        ActivePuzzlePieces.Clear();
        Interactables.Clear();
    }

    void OnRecipeAvailable(RecipeAvailableEvent e)
    {
        // PlayerController.OnItemPickUp --> Item.OnItemPickedUp
        
        // e.Target;
        // e.HeldItem;
        // e.Recipe;
        //--------------------------------------------------------------------------------------------------------------
        
        SpawnVFXInstanceItem(e.HeldItem, e.Target);
    }

    void OnPropInteractionAvailable(PropInteractionAvailableEvent e)
    {
        // PlayerController.OnItemPickup --> Prop.OnItemPickedUp
        
        // e.Target;
        // e.Controller;
        //--------------------------------------------------------------------------------------------------------------

        if (e.Target != null)
        {
            SpawnVFXInstanceProp(e.Target);
        }
    }

    void OnInteractionHappened(InteractionHappenedEvent e)
    {
        // e.PropInteractedWith
        //--------------------------------------------------------------------------------------------------------------
        
        // TODO: TriggerVFX not visible. Why?
        
        #region DebugLogs
        /*
        Debug.Log("OnInteractionHappened: viableInteractableInRange: "+ viableInteractableInRange + " , "+viableInteractableInRange.GetInstanceID());
        Debug.Log("OnInteractionHappened: viableInteractableInRangeVFX: "+ viableInteractableInRangeVFX + " , "+viableInteractableInRangeVFX.GetInstanceID());
        Debug.Log("OnInteractionHappened: InteractionHappenedEvent e: "+ e.PropInteractedWith.gameObject + " , "+e.PropInteractedWith.gameObject.GetInstanceID());
        
        for (int i = 0; i < ActivePuzzlePieces.Count; i++)
        {
            Debug.Log("OnInteractionHappened: Interactable: "+ Interactables[i] + " , "+Interactables[i].GetInstanceID());
        }
        */
        #endregion
        
        // Temporary Solution, that at least doesn't break the game, but also doesn't actually portray the VFXTriggerState on the interacted prop.
        if (ActivePuzzlePieces != null && Interactables != null && ActivePuzzlePieces.Count == Interactables.Count)
        {
            for (int i = 0; i < Interactables.Count; i++)
            {
                if (ActivePuzzlePieces[i] != null)
                {
                    _viableInteractableInRange = null;
                    _viableInteractableInRangeVfx = null;
                    
                    if (e.PropInteractedWith.gameObject == Interactables[i])
                    {
                        Debug.Log("OnInteractionHappened: "+ActivePuzzlePieces[i]);
                        //VFXCancel(ActivePuzzlePieces[i]);
                        //GameEventManager.Raise(new ExitingViableInteractableRangeEvent(Interactables[i]));

                        VFXTrigger(ActivePuzzlePieces[i]);     // TODO: Why does VFXTrigger not work here, but EVERY OTHER state does?  --> only an issue if OnEntering/ExitingInteractableRange handles VFXStates. They overlap as it seems.     
                    }
                    else if (e.PropInteractedWith.gameObject != Interactables[i])
                    {
                        VFXCancel(ActivePuzzlePieces[i]);
                        //VFXTrigger(ActivePuzzlePieces[i]); 
                    }
                    

                    //VFXCancel(ActivePuzzlePieces[i]);
                    Debug.Log("Interacted With: "+e.PropInteractedWith+ " ------gameobject----- "+e.PropInteractedWith.gameObject);
                    Debug.Log("Cancelling VFX of: "+Interactables[i]+" ------- " +ActivePuzzlePieces[i]);
                    ToggleOutline(false, Interactables[i]);
                }
            }
        }
        
        e.PropInteractedWith.ChangeFallbackLayer("Interactable");        // Used to be "Default"
        ActivePuzzlePieces.Clear();
        Interactables.Clear();

    }


    void OnCraftingHappened(CraftingHappenedEvent e)
    {
        // e.PropInteractedWith
        //--------------------------------------------------------------------------------------------------------------
        
        #region alternate approach 1
        /*
        VFXTrigger(ActivePuzzlePieces[_indexViableObjectInRange]);
        ActivePuzzlePieces.RemoveAt(_indexViableObjectInRange);

        for (int i = 0; i < ActivePuzzlePieces.Count; i++)
        {
            VFXCancel(ActivePuzzlePieces[i]);
        }
        */ // Why does this not work?
        #endregion

        #region alternate approach 2
        /*
        if (ActivePuzzlePieces != null && Interactables != null && ActivePuzzlePieces.Count == Interactables.Count)
        {
            for (int i = 0; i < Interactables.Count; i++)
            {
                if (e.ItemInteractedWith.gameObject == Interactables[i])
                {
                    Debug.Log("OnCraftingHappened: "+ActivePuzzlePieces[i]);
                    //VFXCancel(ActivePuzzlePieces[i]);
                    ActivePuzzlePieces.Remove(ActivePuzzlePieces[i]);
                    VFXTrigger(_viableInteractableInRangeVfx);     // TODO: Why does VFXTrigger not work here, but EVERY OTHER state does?   

                }
                else
                {
                    VFXCancel(ActivePuzzlePieces[i]);
                }
            }
        }    // After this if statement, there's 1 prop vfx still referenced in the list. Possibly gets skipped due to the removal of a slot.
        
        for (int i = 0; i < ActivePuzzlePieces.Count; i++)
        {
            if (ActivePuzzlePieces[i] != null)
            {
                Destroy(ActivePuzzlePieces[i]);
            }
        }
        */
        #endregion

        #region alternate approach 3
        /*
        int count = ActivePuzzlePieces.Count;
        int i = 0;

        while (i < count)
        {
            if (e.ItemInteractedWith.gameObject == Interactables[i])
            {
                VFXTrigger(ActivePuzzlePieces[i]);
                ActivePuzzlePieces.Remove(ActivePuzzlePieces[i]);
                Interactables.RemoveAt(i);
                count--;
            }
            else
            {
                VFXCancel(ActivePuzzlePieces[i]);
                i++;
            }

        }
        */
        #endregion
        
        // Temporary Solution, that at least doesn't break the game, but also doesn't actually portray the VFXTriggerState on the interacted Item.
        if (ActivePuzzlePieces != null && Interactables != null && ActivePuzzlePieces.Count == Interactables.Count)
        {
            for (int i = 0; i < Interactables.Count; i++)
            {
                if (ActivePuzzlePieces[i] != null)
                {
                    if (e.ItemInteractedWith.gameObject == Interactables[i])
                    {
                        Debug.Log("OnInteractionHappened: "+ActivePuzzlePieces[i]);
                        //VFXCancel(ActivePuzzlePieces[i]);
                        VFXTrigger(ActivePuzzlePieces[i]);     // TODO: Why does VFXTrigger not work here, but EVERY OTHER state does?       
                    }
                    else
                    {
                        VFXCancel(ActivePuzzlePieces[i]);
                        //VFXTrigger(ActivePuzzlePieces[i]); 
                    }
                    ToggleOutline(false, Interactables[i]);
                }
            }
        }
        
        
        ActivePuzzlePieces.Clear();
        Interactables.Clear();

        #region old
        /*
        
        // TODO: TriggerVFX overlaps with standard one multiple times/ FLICKERING --> then ultimately the standard one remains visible. Why?
        // ^- 1 vfx in start mode remains - FROM COFFEE MACHINE ?!
        for (int i = 0; i < Interactables.Count; i++)
        {
            // TODO: FIRST find the triggerpiece and trigger it, then, afterwards, cancel the rest of them - otherwise index ids constantly change!
            if (e.ItemInteractedWith.gameObject == Interactables[i])
            {
                VFXTrigger(ActivePuzzlePieces[i]);
                ActivePuzzlePieces.Remove(ActivePuzzlePieces[i]);
            }
        }

        for (int i = 0; i < Interactables.Count; i++)
        {
            if (e.ItemInteractedWith.gameObject != Interactables[i])    // This is technically already guaranteed to always be true at this point.
            {
                VFXCancel(ActivePuzzlePieces[i]);
            }
        }
        
        ActivePuzzlePieces.Clear();
        Interactables.Clear();
        
        */
        #endregion

    }

    void OnEnteringViableInteractableRange(EnteringViableInteractableRangeEvent e)
    {
        // e.HighlightedInteractable;
        //--------------------------------------------------------------------------------------------------------------

        if (ActivePuzzlePieces != null)
        {
            _viableInteractableInRange = e.HighlightedInteractable;

            for (int i = 0; i < Interactables.Count; i++)
            {
                if (e.HighlightedInteractable == Interactables[i] && ActivePuzzlePieces[i] != null)
                {
                    _viableInteractableInRange = Interactables[i];
                    _viableInteractableInRangeVfx = ActivePuzzlePieces[i];
                    VFXClosestViable(ActivePuzzlePieces[i]);    
                    _indexViableObjectInRange = i;
                }
            }
        }
    }
    
    void OnExitingViableInteractableRange(ExitingViableInteractableRangeEvent e)
    {
        // e.HighlightedInteractable;
        //--------------------------------------------------------------------------------------------------------------
        
        if (_viableInteractableInRangeVfx != null)
        {
            VFXViable(_viableInteractableInRangeVfx);    // <--  BUG: THIS COLLIDES WITH TRIGGERVFX BEHAVIOR -- WHY?
            _viableInteractableInRange = null;
        }
    }



    private void SpawnVFXInstanceItem(Item heldItem, Item interactable)
    {
        if (heldItem == interactable)
        {
            return;
        }
        else if (interactable != null)
        {
            interactable.ChangeFallbackLayer("OutlineCraftable");
            SpawnVFX(interactable.gameObject);
        }
    }
    
    
    private void SpawnVFXInstanceProp(Prop interactable)
    {
        interactable.ChangeFallbackLayer("OutlineCraftable");
        SpawnVFX(interactable.GameObject.gameObject);
    }

    private void SpawnVFX(GameObject interactable)
    {
        Collider collider = interactable.GetComponent<Collider>();
        Vector3 ScreenSpacePosition = GuiUtility.GuiUtility.GetScreenPointAboveObject(collider);
        
        GameObject newPuzzlePieceVFX = Instantiate(PuzzlePieceVFX, ScreenSpacePosition + _positionOffsetY, Quaternion.identity);
        newPuzzlePieceVFX.transform.SetParent(this.transform, true);
        
        newPuzzlePieceVFX.SetActive(true);
        newPuzzlePieceVFX.name = "PuzzlePieceVFX_of_"+interactable.name;
            
        Interactables.Add(interactable);
        ActivePuzzlePieces.Add(newPuzzlePieceVFX);
        
        ToggleOutline(true, interactable);
        VFXStart(newPuzzlePieceVFX);
    }


    private void UpdatePuzzlePieceVFXPosition(GameObject puzzlePiece, GameObject interactable)
    {
        if (puzzlePiece != null)
        {
            Collider collider = interactable.GetComponent<Collider>();
            Vector3 ScreenSpacePosition = GuiUtility.GuiUtility.GetScreenPointAboveObject(collider);

            if (DisplayPuzzlePiecesInScreenSpace)
            {
                puzzlePiece.transform.position = ScreenSpacePosition + _positionOffsetY;
            }
            else
            {
                puzzlePiece.transform.position = Camera.main.ScreenToWorldPoint(ScreenSpacePosition) + _positionOffsetY;
            }
        }
    }

    private void UpdatePuzzlePieceVFXGlobalSize(GameObject puzzlePiece)
    {
        if (puzzlePiece != null)
        {
            puzzlePiece.transform.localScale = GlobalSizePuzzlePieceVFX * Vector3.one;
        }
    }
    
    
    private void ToggleOutline(bool activateOutline, GameObject puzzlePiece)
    {
        if (activateOutline)
        { 
            ChangeLayerRecursive(puzzlePiece, 12);    //LayerMask.NameToLayer("OutlineCraftable");
        }
        else
        {
            ChangeLayerRecursive(puzzlePiece, LayerMask.NameToLayer("Interactable"));    // Standard-Layer, the Object usually has. Used to be "Default"
        }
    }
    
    private static void ChangeLayerRecursive(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            ChangeLayerRecursive(gameObject.transform.GetChild(i).gameObject, layer);
        }
    }

    #region VFX States

    private void VFXStart(GameObject puzzlePieceVFX)
    {
        PuzzlePieceVFX puzzle = puzzlePieceVFX.GetComponent<PuzzlePieceVFX>();
        
        puzzle.Start.SetActive(true);
        puzzle.CloseBy.SetActive(false);
        puzzle.Trigger.SetActive(false);
        puzzle.Cancel.SetActive(false);
            
        puzzle.Start.GetComponent<PlayableDirector>().Play();
    }

    private void VFXClosestViable(GameObject puzzlePieceVFX)
    {
        PuzzlePieceVFX puzzle = puzzlePieceVFX.GetComponent<PuzzlePieceVFX>();

        puzzle.Start.SetActive(true);
        puzzle.CloseBy.SetActive(true);
        puzzle.Trigger.SetActive(false);
        puzzle.Cancel.SetActive(false);

        puzzle.CloseBy.GetComponent<PlayableDirector>().Play();
    }

    private void VFXViable(GameObject puzzlePieceVFX)
    {
        PuzzlePieceVFX puzzle = puzzlePieceVFX.GetComponent<PuzzlePieceVFX>();

        //puzzle.Start.SetActive(false);
        puzzle.Start.SetActive(true);
        
        puzzle.CloseBy.GetComponent<PlayableDirector>().Stop();
        
        puzzle.CloseBy.SetActive(false);
        puzzle.Trigger.SetActive(false);
        puzzle.Cancel.SetActive(false);
    }

    private void VFXTrigger(GameObject puzzlePieceVFX)
    {
        PuzzlePieceVFX puzzle = puzzlePieceVFX.GetComponent<PuzzlePieceVFX>();

        puzzle.Start.SetActive(false);
        puzzle.CloseBy.SetActive(false);
        puzzle.Trigger.SetActive(true);
        puzzle.Cancel.SetActive(false);
                
        //Debug.Log("DisplayCraftingSuccess");
        puzzle.Trigger.GetComponent<PlayableDirector>().Play();
        
        StartCoroutine(DestroyAfterSeconds(puzzlePieceVFX, 1f));
    }

    private void VFXCancel(GameObject puzzlePieceVFX)
    {
        PuzzlePieceVFX puzzle = puzzlePieceVFX.GetComponent<PuzzlePieceVFX>();
        puzzle.Start.GetComponent<PlayableDirector>().Pause();

        
        puzzle.Start.SetActive(false);
        puzzle.CloseBy.SetActive(false);
        puzzle.Trigger.SetActive(false);
        puzzle.Cancel.SetActive(true);

        puzzle.Cancel.GetComponent<PlayableDirector>().Play();

        StartCoroutine(DestroyAfterSeconds(puzzlePieceVFX, .5f));
    }



    private IEnumerator DestroyAfterSeconds(GameObject puzzlePiece,float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(puzzlePiece);
    }

    #endregion

    
    #region BackupCode
    
    /*
    public enum GetAppropriateCraftingState
    {
        FadeInDisplayViableCraftingItem,
        DisplayClosestViableCraftingItemInRange,
        DisplayViableCraftingItem,
        DisplayCraftingSuccess,
        FadeOutViableCraftingItem
    }

    
    
    public void DisplayCraftingVFX(GetAppropriateCraftingState appropriateState)
    {
        switch (appropriateState)
        {
            case GetAppropriateCraftingState.FadeInDisplayViableCraftingItem : 
                
                StartPiece.SetActive(true);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CancelPiece.SetActive(false);
            
                StartPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            
            case GetAppropriateCraftingState.DisplayClosestViableCraftingItemInRange : 
                
                //StartPiece.SetActive(false);
                CloseByPiece.SetActive(true);
                TriggerPiece.SetActive(false);
                CancelPiece.SetActive(false);

                CloseByPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            case GetAppropriateCraftingState.DisplayViableCraftingItem :
                
                //StartPiece.SetActive(false);
                StartPiece.SetActive(true);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CancelPiece.SetActive(false);
                break;
            
            case GetAppropriateCraftingState.DisplayCraftingSuccess : 
                
                StartPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(true);
                CancelPiece.SetActive(false);
                
                //Debug.Log("DisplayCraftingSuccess");
                TriggerPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            case GetAppropriateCraftingState.FadeOutViableCraftingItem :
                
                StartPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CancelPiece.SetActive(true);

                CancelPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            
            //_______________________________________________________________________________________________________
            
            
            case GetAppropriateCraftingState.FadeInDisplayViableCraftingItem : 
                CancelPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                StartPiece.SetActive(true);
            
                StartPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            
            case GetAppropriateCraftingState.DisplayClosestViableCraftingItemInRange : 
                CancelPiece.SetActive(false);
                //StartPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CloseByPiece.SetActive(true);

                CloseByPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            case GetAppropriateCraftingState.DisplayViableCraftingItem :
                CancelPiece.SetActive(false);
                //StartPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                StartPiece.SetActive(true);
                break;
            
            case GetAppropriateCraftingState.DisplayCraftingSuccess : 
                CancelPiece.SetActive(false);
                StartPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(true);
                Debug.Log("DisplayCraftingSuccess");
                TriggerPiece.GetComponent<PlayableDirector>().Play();
                break;
            
            case GetAppropriateCraftingState.FadeOutViableCraftingItem :
                StartPiece.SetActive(false);
                CloseByPiece.SetActive(false);
                TriggerPiece.SetActive(false);
                CancelPiece.SetActive(true);

                CancelPiece.GetComponent<PlayableDirector>().Play();
                break;
            
                
        }
        
    }
    */
    
    #endregion
}
