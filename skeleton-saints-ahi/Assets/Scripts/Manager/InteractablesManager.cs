using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablesManager : MonoBehaviour
{
	public InputReader inputReader;

	public float ReachDistance = 5f;

	public LayerMask interactableLayer;
	IInteractable oldInteractable;
	IInteractable currentInteractable;

	public void Update()
	{
		if (gameManager.instance.PlayStarted() && gameManager.instance.PlayerScript() != null)
		{
			if (Camera.main != null)
			{
				Ray inputRay = Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f));
				RaycastHit interactingHit;
				
				if (Physics.Raycast(inputRay, out interactingHit, ReachDistance, interactableLayer))
				{

					if (interactingHit.collider != null)
					{
						currentInteractable = interactingHit.collider.GetComponent<IInteractable>();	
					}
					else
					{
						currentInteractable = null;
					}
					oldInteractable = currentInteractable;
					if (currentInteractable != null)
					{
						oldInteractable = currentInteractable;
						inputReader.DisplayMessage(currentInteractable.currentInteractionText); // the line so that the item names appear on screen
						if (Input.GetButtonDown(playerPreferences.instance.Button_Interact))
						{
							currentInteractable.Interact();
							inputReader.ClearMessage();
						}
					}
					else
					{
						inputReader.ClearMessage();

					}
                }
				else
				{
					inputReader.ClearMessage();
					if (oldInteractable != null)
					{
						oldInteractable.ResetInteractionText();
					}
				}
			}
		}
	}
}
